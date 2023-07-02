using System;
using System.IO;

using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.Deucalion;
using EchoV3.Models.FFXIV;
using EchoV3.Services;
namespace EchoV3.Factories.Deucalion
{
    public class SendRecvFrameFactory : IDeucalionFactory
    {
        public IDeucalionFrame ProcessFrame(DeucalionPayload payload)
        {
            using(var stream = new MemoryStream(payload.SegmentData))
            {
                // declare buffers
                byte[] srcActorBuffer = new byte[4];
                byte[] dstActorBuffer = new byte[4];
                byte[] timestampBuffer = new byte[8];
                byte[] ipcHeaderBuffer = new byte[16];
                byte[] segmentData = new byte[payload.SegmentData.Length - 32];

                // read from segment
                stream.ReadExactly(srcActorBuffer);
                stream.ReadExactly(dstActorBuffer);
                stream.ReadExactly(timestampBuffer);
                stream.ReadExactly(ipcHeaderBuffer);
                stream.ReadExactly(segmentData);

                // convert types
                uint srcActorId = BitConverter.ToUInt32(srcActorBuffer, 0);
                uint dstActorId = BitConverter.ToUInt32(dstActorBuffer, 0);
                DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(
                        (long)BitConverter.ToUInt64(timestampBuffer)
                    ).DateTime;
                IpcHeader ipcHeader = new IpcHeader(ipcHeaderBuffer);

                if (payload.Operation == IDeucalionFrame.DeucalionOperation.Send)
                {
                    return new SendFrame()
                    {
                        Length = payload.Length,
                        Operation = payload.Operation,
                        Channel = payload.Channel,
                        SourceActorId = srcActorId,
                        DestinationActorId = dstActorId,
                        Timestamp = timestamp,
                        FFXIVIpcHeader = ipcHeader,
                        SegmentData = segmentData,
                    };
                }
                return new RecvFrame()
                {
                    Length = payload.Length,
                    Operation = payload.Operation,
                    Channel = payload.Channel,
                    SourceActorId = srcActorId,
                    DestinationActorId = dstActorId,
                    Timestamp = timestamp,
                    FFXIVIpcHeader = ipcHeader,
                    SegmentData = segmentData,
                };
            }
        }
    }
}
