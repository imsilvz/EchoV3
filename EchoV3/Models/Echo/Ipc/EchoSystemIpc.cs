using EchoV3.Models.FFXIV.GameData;

namespace EchoV3.Models.Echo.Ipc
{
    public class EchoSystemIpc : BaseEchoIpc
    {
        public override string EchoType => "System";
        public required string Message { get; set; }
    }
}
