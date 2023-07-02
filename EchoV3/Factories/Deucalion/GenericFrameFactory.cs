using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.Deucalion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoV3.Factories.Deucalion
{
    public class GenericFrameFactory : IDeucalionFactory
    {
        public IDeucalionFrame ProcessFrame(DeucalionPayload payload)
        {
            return new GenericFrame()
            {
                Length = payload.Length,
                Operation = payload.Operation,
                Channel = payload.Channel,
                SegmentData = payload.SegmentData
            };
        }
    }
}
