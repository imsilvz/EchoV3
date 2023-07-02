using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EchoV3.Models.FFXIV
{
    public class RegionalOpcodeData
    {
        [JsonPropertyName("version")]
        public required string Version { get; set; }
        [JsonPropertyName("region")]
        public required string Region { get; set; }
        [JsonPropertyName("lists")]
        public required RegionalOpcodeLists Lists { get; set; }
    }

    public class RegionalOpcodeLists
    {
        public required List<OpcodePair> ServerZoneIpcType { get; set; }
        public required List<OpcodePair> ClientZoneIpcType { get; set; }
        public required List<OpcodePair> ServerLobbyIpcType { get; set; }
        public required List<OpcodePair> ClientLobbyIpcType { get; set; }
        public required List<OpcodePair> ServerChatIpcType { get; set; }
        public required List<OpcodePair> ClientChatIpcType { get; set; }
    }

    public class OpcodePair
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("opcode")]
        public required uint Code { get; set; }
    }
}
