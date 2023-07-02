using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading;

using EchoV3.Factories.Deucalion;
using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.Deucalion;
namespace EchoV3.Services
{
    public class DeucalionService
    {
        [Flags]
        public enum DeucalionOption : uint
        {
            RECV_LOBBY = (1 << 0),
            RECV_ZONE = (1 << 1),
            RECV_CHAT = (1 << 2),
            SENT_LOBBY = (1 << 3),
            SENT_ZONE = (1 << 4),
            SENT_CHAT = (1 << 5),
            OTHER = (1 << 6),
        }

        internal class ReadFrameResponse
        {
            public int bytesRead { get; set; }
            public DeucalionPayload? payload { get; set; }
        }

        // frame buffer
        private int _totalBytesRead = 0;
        private byte[] _pipeBuffer = new byte[0];
        private DeucalionPayload? _frameBuffer;

        // thread details
        private int _processId = -1;
        private Thread? _pipeThread;

        // events
        public event EventHandler<IDeucalionFrame>? FrameReady;
        public event EventHandler<DebugFrame>? DebugFrameProcessed;
        public event EventHandler<IFFXIVIpcFrame>? IpcFrameProcessed;
        private event EventHandler<DeucalionPayload>? PayloadRecieved;
        public DeucalionService(InjectionService injectionService)
        {
            this.FrameReady += OnFrameReady;
            this.PayloadRecieved += ProcessFrame;
            injectionService.ProcessReady += OnProcessReady;
        }

        private ReadFrameResponse ReadFrame(NamedPipeClientStream pipe)
        {
            if (pipe is null) throw new ArgumentNullException(nameof(pipe));
            if (!pipe.IsConnected) { 
                return new ReadFrameResponse
                {
                    bytesRead = 0,
                    payload = null,
                }; 
            }

            // _pipeBuffer is only 0 if we are starting a new frame
            if (_pipeBuffer.Length == 0)
            {
                _frameBuffer = null;
                _pipeBuffer = new byte[9];
                _totalBytesRead = 0;
            }

            // check if there is more to read!
            // if there is we should read it
            if (_totalBytesRead < _pipeBuffer.Length)
            {
                byte[] readBuffer = new byte[_pipeBuffer.Length - _totalBytesRead];
                int currRead = pipe.Read(readBuffer);

                // copy read data to _pipeBuffer
                Buffer.BlockCopy(readBuffer, 0, _pipeBuffer, _totalBytesRead, currRead);
                _totalBytesRead += currRead;

                // check if we have more to read
                if (_totalBytesRead < _pipeBuffer.Length)
                {
                    return new ReadFrameResponse
                    {
                        bytesRead = currRead,
                        payload = null,
                    };
                }
                else
                {
                    if (_frameBuffer is null)
                    {
                        _frameBuffer = new DeucalionPayload();
                        _frameBuffer.Length = (uint)BitConverter.ToInt16(_pipeBuffer, 0); // 4 byte
                        _frameBuffer.Operation = (IDeucalionFrame.DeucalionOperation)_pipeBuffer[4]; // 1 byte
                        _frameBuffer.Channel = (IDeucalionFrame.DeucalionChannel)BitConverter.ToInt16(_pipeBuffer, 5); // 4 byte
                        _frameBuffer.SegmentData = new byte[_frameBuffer.Length - 9]; // total header length = 9
                        if (_frameBuffer.SegmentData.Length > 0) 
                        {
                            // if there is data attached, prepare next read to fetch that!
                            _pipeBuffer = new byte[_frameBuffer.SegmentData.Length];
                            _totalBytesRead = 0;
                            return new ReadFrameResponse
                            {
                                bytesRead = currRead,
                                payload = null,
                            };
                        }
                    }
                    else
                    {
                        // Block Copy our _pipeBuffer into the Deucalion Frame
                        Buffer.BlockCopy(_pipeBuffer, 0, _frameBuffer.SegmentData, 0, _frameBuffer.SegmentData.Length);
                    }
                }
            }
            // reset pipe buffer!
            _pipeBuffer = new byte[0];
            _totalBytesRead = 0;
            return new ReadFrameResponse
            {
                bytesRead = 0,
                payload = _frameBuffer,
            };
        }

        private void ProcessPipe()
        {
            do
            {
                Thread.Sleep(1000);
            }
            while (!PipeExists(_processId));

            string pipeName = $"deucalion-{_processId}";
            using (var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
            {
                // Connect to the Pipe, then write to it!
                Debug.WriteLine("[+] Connecting to Deucalion Pipe...");
                pipe.Connect(5000);
                Debug.WriteLine("[+] Connected to Deucalion Pipe!");

                // Customize Options
                using (var writer = new BinaryWriter(pipe, Encoding.UTF8, true))
                {
                    GenericFrame optionFrame = new GenericFrame()
                    {
                        Length = 9,
                        Operation = IDeucalionFrame.DeucalionOperation.Option,
                        Channel = (IDeucalionFrame.DeucalionChannel)(
                                DeucalionOption.RECV_LOBBY | 
                                DeucalionOption.SENT_LOBBY | 
                                DeucalionOption.RECV_ZONE |
                                DeucalionOption.SENT_ZONE |
                                DeucalionOption.RECV_CHAT | 
                                DeucalionOption.SENT_CHAT
                            ),
                    };
                    writer.Write(optionFrame.ToBuffer());
                }

                while (pipe.IsConnected)
                {
                    ReadFrameResponse frameResponse = ReadFrame(pipe);
                    if (frameResponse.payload is not null)
                    {
                        PayloadRecieved?.Invoke(this, frameResponse.payload);
                    }

                    // delay if we fetch no data
                    if ((frameResponse.payload is null) && (frameResponse.bytesRead == 0))
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        private void ProcessFrame(object? sender, DeucalionPayload payload)
        {
            IDeucalionFactory? factory = null;
            switch(payload.Operation)
            {
                case IDeucalionFrame.DeucalionOperation.Debug:
                    factory = new DebugFrameFactory();
                    break;
                case IDeucalionFrame.DeucalionOperation.Ping:
                    break;
                case IDeucalionFrame.DeucalionOperation.Recv:
                case IDeucalionFrame.DeucalionOperation.Send:
                    factory = new SendRecvFrameFactory();
                    break;
                default:
                    factory = new GenericFrameFactory();
                    break;
            }

            // do processing
            if (factory is not null)
            {
                FrameReady?.Invoke(this, factory.ProcessFrame(payload));
            }
        }

        private void OnFrameReady(object? sender, IDeucalionFrame frame)
        {
            switch (frame.Operation)
            {
                case IDeucalionFrame.DeucalionOperation.Debug:
                    DebugFrameProcessed?.Invoke(this, (DebugFrame)frame);
                    break;
                case IDeucalionFrame.DeucalionOperation.Ping:
                    break;
                case IDeucalionFrame.DeucalionOperation.Recv:
                    IpcFrameProcessed?.Invoke(this, (RecvFrame)frame);
                    break;
                case IDeucalionFrame.DeucalionOperation.Send:
                    IpcFrameProcessed?.Invoke(this, (SendFrame)frame);
                    break;
                default:
                    break;
            }
        }

        private void OnProcessReady(object? sender, int processId)
        {
            _processId = processId;
            _pipeThread = new Thread(ProcessPipe);
            _pipeThread.IsBackground = true;
            _pipeThread.Start();
        }

        public static bool PipeExists(int processId)
        {
            string pipeName = $"deucalion-{processId}";
            string pipeFullName = $"\\\\.\\pipe\\{pipeName}";
            return File.Exists(pipeFullName);
        }
    }
}
