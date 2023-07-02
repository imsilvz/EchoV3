using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static EchoV3.Models.FFXIV.Events.Server.ActorControlEvent;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class ActorControlSelfEvent : BaseFFXIVEvent
    {
        public static EventHandler<ActorControlSelfEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ActorControlSelfEvent)xivEvent);
        }

        public ActorControlType Category { get; set; }
        public uint Param1 { get; set; }
        public uint Param2 { get; set; }
        public uint Param3 { get; set; }
        public uint Param4 { get; set; }
        public uint Param5 { get; set; }
        public uint Param6 { get; set; }

        public override string ToString()
        {
            return $"ActorControlSelf({SourceActorId}) {Category}: {Param1}, {Param2}, {Param3}, {Param4}, {Param5}, {Param6}";
        }
    }
}
