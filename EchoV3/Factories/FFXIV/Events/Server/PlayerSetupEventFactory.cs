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
    public class PlayerSetupEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                // This is a MASSIVE packet!
                // We're only extracting stuff we care about!
                ulong contentId = reader.ReadUInt64(); // 8

                stream.Seek(16, SeekOrigin.Current); // 24

                uint playerId = reader.ReadUInt32(); // 28
                uint restedExp = reader.ReadUInt32(); // 32

                stream.Seek(68, SeekOrigin.Current); // 100

                ushort playerCommendations = reader.ReadUInt16(); // 102

                stream.Seek(24, SeekOrigin.Current); // 126

                byte maxLevel = reader.ReadByte(); // 127
                byte expansion = reader.ReadByte(); // 128

                stream.Seek(3, SeekOrigin.Current); // 131

                byte raceId = reader.ReadByte(); // 132
                byte tribeId = reader.ReadByte(); // 133
                byte genderId = reader.ReadByte(); // 134
                byte currJob = reader.ReadByte(); // 135
                byte currClass = reader.ReadByte(); // 136

                stream.Seek(685, SeekOrigin.Begin); // 685
                byte[] nameBytes = reader.ReadBytes(32);

                return new PlayerSetupEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,
                    PlayerId = playerId,
                    PlayerName = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'),
                };
            }
        }
    }
}
