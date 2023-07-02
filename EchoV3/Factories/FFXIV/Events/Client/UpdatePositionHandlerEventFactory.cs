using EchoV3.Interfaces.Deucalion;
using EchoV3.Interfaces.FFXIV;
using EchoV3.Models.FFXIV.Events;
using EchoV3.Models.FFXIV.Events.Client;
using EchoV3.Models.FFXIV.GameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.FFXIV.Events.Client
{
    public class UpdatePositionHandlerEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                float rotation = reader.ReadSingle();
                byte animationType = reader.ReadByte();
                byte animationState = reader.ReadByte();
                byte clientAnimType = reader.ReadByte();
                byte headPosition = reader.ReadByte();

                ushort xPos = (ushort)(((reader.ReadSingle() + 1000.0f) * 100.0f) * 0.32767501f);
                ushort yPos = (ushort)(((reader.ReadSingle() + 1000.0f) * 100.0f) * 0.32767501f);
                ushort zPos = (ushort)(((reader.ReadSingle() + 1000.0f) * 100.0f) * 0.32767501f);

                reader.ReadBytes(4);

                return new UpdatePositionHandlerEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    Rotation = rotation,
                    AnimationType = animationType,
                    AnimationState = animationState,
                    ClientAnimationType = clientAnimType,
                    HeadPosition = headPosition,
                    Position = new GamePosition
                    { 
                        X = xPos,
                        Y = yPos,
                        Z = zPos
                    }
                };
            }
        }
    }
}
