using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.GameData
{
    public class PlayerActor : BaseActor
    {
        public required string PlayerName { get; set; }
        public Common.ClassJob Job { get; set; }
        public byte JobLevel { get; set; }
    }
}
