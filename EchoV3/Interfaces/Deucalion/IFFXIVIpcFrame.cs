using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoV3.Models.FFXIV;

namespace EchoV3.Interfaces.Deucalion
{
    public interface IFFXIVIpcFrame : IDeucalionFrame
    {
        public uint SourceActorId { get; set; }
        public uint DestinationActorId { get; set; }
        public DateTime Timestamp { get; set; }
        public IpcHeader FFXIVIpcHeader { get; set; }
        public byte[] SegmentData { get; set; }
    }
}
