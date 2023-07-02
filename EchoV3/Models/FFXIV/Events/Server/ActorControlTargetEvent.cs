using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EchoV3.Models.FFXIV.Events.Server.ActorControlEvent;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class ActorControlTargetEvent : BaseFFXIVEvent
    {
        public static EventHandler<ActorControlTargetEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ActorControlTargetEvent)xivEvent);
        }

        public ActorControlType Category { get; set; }
        public uint Param1 { get; set; }
        public uint Param2 { get; set; }
        public uint Param3 { get; set; }
        public uint Param4 { get; set; }
        public ulong TargetId { get; set; }

        public override string ToString()
        {
            return $"ActorControlTarget({SourceActorId}) {Category}: {Param1}, {Param2}, {Param3}, {Param4}, {TargetId}";
        }
    }
}
