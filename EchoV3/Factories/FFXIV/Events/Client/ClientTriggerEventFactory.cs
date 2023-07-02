using EchoV3.Interfaces.Deucalion;
using EchoV3.Interfaces.FFXIV;
using EchoV3.Models.FFXIV.Events;
using EchoV3.Models.FFXIV.Events.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.FFXIV.Events.Client
{
    public class ClientTriggerEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                ushort commandId = reader.ReadUInt16();
                ushort unk2 = reader.ReadUInt16();
                uint param1 = reader.ReadUInt32();
                uint param2 = reader.ReadUInt32();
                uint param3 = reader.ReadUInt32();
                uint param4 = reader.ReadUInt32();
                uint param5 = reader.ReadUInt32();
                ulong param6 = reader.ReadUInt64();

                return new ClientTriggerEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    CommandId = (ClientTriggerEvent.ClientTriggerType) commandId,
                    Param1 = param1,
                    Param2 = param2,
                    Param3 = param3,
                    Param4 = param4,
                    Param5 = param5,
                    Param6 = param6,
                };
            }
        }
    }
}
