using EchoV3.Interfaces.Deucalion;
using EchoV3.Interfaces.FFXIV;
using EchoV3.Models.FFXIV.Events;
using EchoV3.Models.FFXIV.Events.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.FFXIV.Events.Server
{
    public class ChatEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                byte[] unk = reader.ReadBytes(8);
                uint playerId = reader.ReadUInt32();
                ushort homeServerId = reader.ReadUInt16();
                ushort messageType = reader.ReadUInt16();
                byte[] nameBuffer = reader.ReadBytes(32);
                byte[] messageBuffer = reader.ReadBytes(1024);

                return new ChatEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    MessageType = (ChatEvent.ChatMessageType)messageType,
                    SenderId = playerId,
                    SenderName = Encoding.UTF8.GetString(nameBuffer).TrimEnd('\0'),
                    Message = Encoding.UTF8.GetString(messageBuffer).TrimEnd('\0'),
                };
            }
        }
    }
}