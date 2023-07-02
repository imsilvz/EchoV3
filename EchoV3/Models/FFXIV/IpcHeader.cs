using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV
{
    public class IpcHeader
    {
        private UInt16 _reserved;
        public UInt16 OpCode;
        private UInt16 _padding;
        public UInt16 ServerId;
        public UInt32 Timestamp;
        private UInt32 _padding1;

        public IpcHeader(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                _reserved = reader.ReadUInt16();
                OpCode = reader.ReadUInt16();
                _padding = reader.ReadUInt16();
                ServerId = reader.ReadUInt16();
                Timestamp = reader.ReadUInt32();
                _padding1 = reader.ReadUInt32();
            }
        }
    }
}
