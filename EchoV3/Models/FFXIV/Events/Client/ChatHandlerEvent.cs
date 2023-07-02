using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EchoV3.Models.FFXIV.Events.Server;

namespace EchoV3.Models.FFXIV.Events.Client
{
    public class ChatHandlerEvent : BaseFFXIVEvent
    {
        public static EventHandler<ChatHandlerEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ChatHandlerEvent)xivEvent);
        }

        public required ChatEvent.ChatMessageType MessageType { get; set; }
        public required string SenderName { get; set; }
        public required string Message { get; set; }

        public override string ToString()
        {
            return $"ChatHandler({SourceActorId}) {MessageType} - {Message}";
        }
    }
}
