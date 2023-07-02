using EchoV3.Interfaces.Deucalion;
using EchoV3.Interfaces.FFXIV;
using EchoV3.Models.FFXIV.Events;
using EchoV3.Models.FFXIV.Events.Server;
using EchoV3.Models.FFXIV.GameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.FFXIV.Events.Server
{
    public class ActorMoveEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                byte headDir = reader.ReadByte();
                byte direction = reader.ReadByte();
                byte animType = reader.ReadByte();
                byte animState = reader.ReadByte();
                byte animSpeed = reader.ReadByte();
                byte padding = reader.ReadByte();

                ushort xPos = reader.ReadUInt16();
                ushort yPos = reader.ReadUInt16();
                ushort zPos = reader.ReadUInt16();

                uint unknown = reader.ReadUInt32();

                return new ActorMoveEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    Direction = direction,
                    HeadDirection = headDir,
                    AnimationType = animType,
                    AnimationState = animState,
                    AnimationSpeed = animSpeed,
                    Position = new GamePosition
                    {
                        X = xPos,
                        Y = yPos,
                        Z = zPos,
                    },
                };
            }
        }
    }
}
