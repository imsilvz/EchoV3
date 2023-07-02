using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.Events
{
    public abstract class BaseFFXIVEvent
    {
        public required ushort EventType { get; set; }
        public required DateTime Timestamp { get; set; }
        public required uint SourceActorId { get; set; }
        public required uint DestinationActorId { get; set; }
    }
}
