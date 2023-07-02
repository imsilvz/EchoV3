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
using EchoV3.Models.FFXIV;

namespace EchoV3.Factories.FFXIV.Events.Server
{
    public class PlayerSpawnEventFactory : IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame)
        {
            using (var stream = new MemoryStream(frame.SegmentData))
            using (var reader = new BinaryReader(stream))
            {
                ushort title = reader.ReadUInt16(); // 2
                ushort unk1 = reader.ReadUInt16(); // 4
                ushort serverId = reader.ReadUInt16(); // 6
                ushort homeServerId = reader.ReadUInt16(); // 8

                byte gmRank = reader.ReadByte(); // 9
                byte unk2 = reader.ReadByte(); // 10
                byte unk3 = reader.ReadByte(); // 11
                byte onlineStatus = reader.ReadByte(); // 12

                byte pose = reader.ReadByte(); // 13
                byte unk4 = reader.ReadByte(); // 14
                byte unk5 = reader.ReadByte(); // 15
                byte unk6 = reader.ReadByte(); // 16

                ulong targetId = reader.ReadUInt64(); // 24
                uint unk7 = reader.ReadUInt32(); // 28
                uint unk8 = reader.ReadUInt32(); // 32
                ulong weaponModel = reader.ReadUInt64(); // 40
                ulong subWeaponModel = reader.ReadUInt64(); // 48
                ulong craftWeaponModel = reader.ReadUInt64(); // 56

                uint unk9 = reader.ReadUInt32(); // 60
                uint unk10 = reader.ReadUInt32(); // 64
                uint npcBase = reader.ReadUInt32(); // 68
                uint npcName = reader.ReadUInt32(); // 72

                uint unk11 = reader.ReadUInt32(); // 76
                uint unk12 = reader.ReadUInt32(); // 80
                uint directorId = reader.ReadUInt32(); // 84
                uint ownerId = reader.ReadUInt32(); // 88

                uint unk13 = reader.ReadUInt32(); // 92
                uint maxHp = reader.ReadUInt32(); // 96
                uint currHp = reader.ReadUInt32(); // 100
                uint displayFlags = reader.ReadUInt32(); // 104

                ushort fateId = reader.ReadUInt16(); // 106
                ushort currMp = reader.ReadUInt16(); // 108
                //ushort currTp = reader.ReadUInt16();
                ushort maxMp = reader.ReadUInt16(); // 110
                ushort unk14 = reader.ReadUInt16(); // 112

                ushort modelCharA = reader.ReadUInt16(); // 114
                ushort rotation = reader.ReadUInt16(); // 116
                ushort currMount = reader.ReadUInt16(); // 118
                ushort minion = reader.ReadUInt16(); // 120

                byte u23 = reader.ReadByte(); // 121
                byte u24 = reader.ReadByte(); // 122

                byte spawnIndex = reader.ReadByte(); // 123
                byte state = reader.ReadByte(); // 124
                byte emote = reader.ReadByte(); // 125
                
                byte modelType = reader.ReadByte(); // 126
                byte modelSubType = reader.ReadByte(); // 127
                byte voice = reader.ReadByte(); // 128

                ushort unk16 = reader.ReadUInt16(); // 130
                byte enemyType = reader.ReadByte(); // 131
                byte level = reader.ReadByte(); // 132
                byte jobId = reader.ReadByte(); // 133
                byte unk17 = reader.ReadByte(); // 134
                ushort unk18 = reader.ReadUInt16(); // 135

                // mount data
                byte mountHead = reader.ReadByte();
                byte mountBody = reader.ReadByte();
                byte mountFeet = reader.ReadByte();
                byte mountColor = reader.ReadByte();

                byte scale = reader.ReadByte();
                byte[] elementData = reader.ReadBytes(6);
                byte unk5_5 = reader.ReadByte();

                // array of status effects
                byte[] statusEffects = reader.ReadBytes(30 * 12);

                // position data
                float xPos = reader.ReadUInt32();
                float yPos = reader.ReadUInt32();
                float zPos = reader.ReadUInt32();

                // gear
                byte[] playerGear = reader.ReadBytes(10 * 4);

                // name!
                string playerName = Encoding.UTF8.GetString(reader.ReadBytes(32)).TrimEnd('\0');

                // player model info
                byte[] playerModel = reader.ReadBytes(26);

                string fcTag = Encoding.UTF8.GetString(reader.ReadBytes(6)).TrimEnd('\0');
                ulong unk20 = reader.ReadUInt64();

                return new PlayerSpawnEvent
                {
                    EventType = frame.FFXIVIpcHeader.OpCode,
                    Timestamp = frame.Timestamp,
                    SourceActorId = frame.SourceActorId,
                    DestinationActorId = frame.DestinationActorId,

                    Name = playerName,
                    ClanTag = fcTag,

                    Job = (Common.ClassJob)jobId,
                    JobLevel = level,

                    TitleId = title,
                    ServerId = serverId,
                    HomeServerId = homeServerId,
                    OnlineStatus = onlineStatus,
                };
            }
        }
    }
}
