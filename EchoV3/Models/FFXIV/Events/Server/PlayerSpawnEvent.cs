using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class PlayerSpawnEvent : BaseFFXIVEvent
    {
        public static EventHandler<PlayerSpawnEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (PlayerSpawnEvent)xivEvent);
        }

        public required string Name { get; set; }
        public required string ClanTag { get; set; }

        public Common.ClassJob Job { get; set; }
        public byte JobLevel { get; set; }

        public required ushort TitleId { get; set; }
        public required ushort ServerId { get; set; }
        public required ushort HomeServerId { get; set; }
        public required byte OnlineStatus { get; set; }

        public override string ToString()
        {
            return $"PlayerSpawn({SourceActorId}) {Name} - Level {JobLevel} {Job}";
        }
    }
}
