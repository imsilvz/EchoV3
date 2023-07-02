using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoV3.Interfaces.Deucalion;

namespace EchoV3.Models.Deucalion
{
    public class GenericFrame : IDeucalionFrame
    {
        public uint Length { get; set; }
        public IDeucalionFrame.DeucalionOperation Operation { get; set; }
        public IDeucalionFrame.DeucalionChannel Channel { get; set; }
        public byte[] SegmentData { get; set; } = new byte[0];

        public byte[] ToBuffer()
        {
            byte[] buffer = new byte[Length];
            byte[] lengthBytes = BitConverter.GetBytes((int)Length);
            byte[] channelBytes = BitConverter.GetBytes((int)Channel);

            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, 4);
            buffer[4] = (byte)Operation;
            Buffer.BlockCopy(channelBytes, 0, buffer, 5, 4);
            if (SegmentData.Length > 0)
            {
                Buffer.BlockCopy(SegmentData, 0, buffer, 9, SegmentData.Length);
            }

            return buffer;
        }

        public override string ToString()
        {
            return $"GenericFrame(Op: {Operation} | Channel: {Channel})";
        }
    }
}
