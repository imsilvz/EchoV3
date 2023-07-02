using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.Echo.Ipc
{
    public abstract class BaseEchoIpc
    {
        public abstract string EchoType { get; }
        public required DateTime Timestamp { get; set; }
        public required uint SourceActorId { get; set; }
        public required uint DestinationActorId { get; set;}
    }
}
