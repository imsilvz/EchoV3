using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoV3.Interfaces.Deucalion;

namespace EchoV3.Models.Deucalion
{
    public class DebugFrame : IDeucalionFrame
    {
        public uint Length { get; set; }
        public IDeucalionFrame.DeucalionOperation Operation { get; set; }
        public IDeucalionFrame.DeucalionChannel Channel { get; set; }
        public string Message { get; set; } = string.Empty;

        public byte[] ToBuffer()
        {
            byte[] buffer = new byte[Length];
            byte[] lengthBytes = BitConverter.GetBytes((int)Length);
            byte[] channelBytes = BitConverter.GetBytes((int)Channel);

            Buffer.BlockCopy(lengthBytes, 0, buffer, 0, 4);
            buffer[4] = (byte)Operation;
            Buffer.BlockCopy(channelBytes, 0, buffer, 5, 4);
            if (Length > 9)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(Message);
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(Message), 0, buffer, 9, messageBytes.Length);
            }

            return buffer;
        }

        public override string ToString()
        {
            return $"DeucalionDebugFrame(Op: {Operation} | Channel: {Channel} | Data: {Message})";
        }
    }
}
