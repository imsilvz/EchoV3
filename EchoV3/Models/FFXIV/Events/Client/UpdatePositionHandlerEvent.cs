using EchoV3.Models.FFXIV.Events.Server;
using EchoV3.Models.FFXIV.GameData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.Events.Client
{
    public class UpdatePositionHandlerEvent : BaseFFXIVEvent
    {
        public static EventHandler<UpdatePositionHandlerEvent>? OnEventFired;
        public static void RaiseEvent(object? sender, BaseFFXIVEvent xivEvent)
        {
            OnEventFired?.Invoke(sender, (UpdatePositionHandlerEvent)xivEvent);
        }

        public required float Rotation { get; set; }
        public required byte AnimationType { get; set; }
        public required byte AnimationState { get; set; }
        public required byte ClientAnimationType { get; set; }
        public required byte HeadPosition { get; set; }
        public required GamePosition Position { get; set; }

        public override string ToString()
        {
            return $"UpdatePositionHandler({SourceActorId}) Dir: {Rotation}, X: {Position.X}, Y: {Position.Y}, Z: {Position.Z}";
        }
    }
}