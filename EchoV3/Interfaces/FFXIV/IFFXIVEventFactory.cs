using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.Deucalion;
using EchoV3.Models.FFXIV;
using EchoV3.Models.FFXIV.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Interfaces.FFXIV
{
    public interface IFFXIVEventFactory
    {
        public BaseFFXIVEvent GetEvent(IFFXIVIpcFrame frame);
    }
}
