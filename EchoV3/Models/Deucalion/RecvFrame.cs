using System;

using EchoV3.Interfaces.Deucalion;
using EchoV3.Models.FFXIV;
using EchoV3.Services;
namespace EchoV3.Models.Deucalion
{
    public class RecvFrame : IFFXIVIpcFrame
    {
        public uint Length { get; set; }
        public IDeucalionFrame.DeucalionOperation Operation { get; set; }
        public IDeucalionFrame.DeucalionChannel Channel { get; set; }
        public required UInt32 SourceActorId { get; set; }
        public required UInt32 DestinationActorId { get; set; }
        public required DateTime Timestamp { get; set; }
        public required IpcHeader FFXIVIpcHeader { get; set; }
        public required byte[] SegmentData { get; set; }

        public byte[] ToBuffer()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"RecvFrame(Channel: {Channel} OpCode: {FFXIVIpcHeader.OpCode} DataLength: {SegmentData.Length})";
        }
    }
}
