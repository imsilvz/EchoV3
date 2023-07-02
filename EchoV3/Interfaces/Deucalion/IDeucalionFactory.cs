using EchoV3.Models.Deucalion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Interfaces.Deucalion
{
    public interface IDeucalionFactory
    {
        public IDeucalionFrame ProcessFrame(DeucalionPayload payload);
    }
}
