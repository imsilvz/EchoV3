using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Models.Echo.Ipc
{
    public class EchoLocalTargetIpc : BaseEchoIpc
    {
        public override string EchoType => "LocalTarget";
        public required ulong? TargetId { get; set; }
    }
}
