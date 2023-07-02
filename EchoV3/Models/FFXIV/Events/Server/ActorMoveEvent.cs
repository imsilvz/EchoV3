using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoV3.Models.FFXIV.GameData;

namespace EchoV3.Models.FFXIV.Events.Server
{
    public class ActorMoveEvent : BaseFFXIVEvent
    {
        public static EventHandler<ActorMoveEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (ActorMoveEvent)xivEvent);
        }

        public required byte Direction { get; set; }
        public required byte HeadDirection { get; set; }
        public required byte AnimationType { get; set; }
        public required byte AnimationState { get; set; }
        public required byte AnimationSpeed { get; set; }
        public required GamePosition Position { get; set; }

        public override string ToString()
        {
            return $"ActorMove({SourceActorId}) Dir: {Direction}, X: {Position.X}, Y: {Position.Y}, Z: {Position.Z}";
        }
    }
}
