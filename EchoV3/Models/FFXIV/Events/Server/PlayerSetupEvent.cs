using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class PlayerSetupEvent : BaseFFXIVEvent
    {
        public static EventHandler<PlayerSetupEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (PlayerSetupEvent)xivEvent);
        }

        public required uint PlayerId { get; set; }
        public required string PlayerName { get; set; }

        public override string ToString()
        {
            return $"PlayerSetup({SourceActorId}) {PlayerName}";
        }
    }
}
