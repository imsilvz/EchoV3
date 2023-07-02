using EchoV3.Models.FFXIV.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.Echo.Ipc
{
    public class EchoChatIpc : BaseEchoIpc
    {
        public override string EchoType => "Chat";
        public required string MessageType { get; set; }
        public required uint SenderId { get; set; }
        public required string SenderName { get; set; }
        public BaseActor? SenderActor { get; set; }
        public required string Message { get; set; }
    }
}
