using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.GameData
{
    public class GamePosition
    {
        public required ushort X { get; set; }
        public required ushort Y { get; set; }
        public required ushort Z { get; set; }
    }
}
