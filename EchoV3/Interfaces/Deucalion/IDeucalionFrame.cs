using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Interfaces.Deucalion
{
    public interface IDeucalionFrame
    {
        [Flags]
        public enum DeucalionChannel : uint
        {
            Lobby = 0,
            Zone = 1,
            Chat = 2,
            Hello = 9000,
        }

        [Flags]
        public enum DeucalionOperation : uint
        {
            Debug = 0,
            Ping = 1,
            Exit = 2,
            Recv = 3,
            Send = 4,
            Option = 5,
            RecvOther = 6,
            SendOther = 7,
        }

        public uint Length { get; set; }
        public DeucalionOperation Operation { get; set; }
        public DeucalionChannel Channel { get; set; }

        // methods
        public byte[] ToBuffer();
    }
}
