using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.Deucalion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.Deucalion
{
    public class DebugFrameFactory : IDeucalionFactory
    {
        public IDeucalionFrame ProcessFrame(DeucalionPayload payload)
        {
            return new DebugFrame()
            {
                Length = payload.Length,
                Operation = payload.Operation,
                Channel = payload.Channel,
                Message = Encoding.UTF8.GetString(payload.SegmentData)
            };
        }
    }
}
