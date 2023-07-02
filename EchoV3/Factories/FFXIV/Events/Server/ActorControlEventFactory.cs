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
    public class ActorControlEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                ushort category = reader.ReadUInt16();
                ushort padding = reader.ReadUInt16();
                uint param1 = reader.ReadUInt32();
                uint param2 = reader.ReadUInt32();
                uint param3 = reader.ReadUInt32();
                uint param4 = reader.ReadUInt32();
                uint padding1 = reader.ReadUInt32();

                return new ActorControlEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    Category = (ActorControlEvent.ActorControlType)category,
                    Param1 = param1,
                    Param2 = param2,
                    Param3 = param3,
                    Param4 = param4
                };
            }
        }
    }
}
