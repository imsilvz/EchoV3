using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV.GameData
{
    [JsonDerivedType(typeof(PlayerActor))]
    public abstract class BaseActor
    {
        public uint ActorId { get; set; }
    }
}
