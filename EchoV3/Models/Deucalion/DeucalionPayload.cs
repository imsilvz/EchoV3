using EchoV3.Interfaces.Deucalion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.Deucalion
{
    public class DeucalionPayload
    {
        public uint Length { get; set; }
        public IDeucalionFrame.DeucalionOperation Operation { get; set; }
        public IDeucalionFrame.DeucalionChannel Channel { get; set; }
        public byte[] SegmentData { get; set; } = new byte[0];
    }
}
