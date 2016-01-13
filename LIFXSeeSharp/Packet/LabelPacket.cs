using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LIFXSeeSharp.Packet
{
    class LabelPacket : BasePacket
    {
        public readonly byte Type = 0x0017;
        public string Label { get; private set; }

        public LabelPacket() : base()
        {
        }

        public LabelPacket(byte[] payload, IPAddress ip) : base(payload, ip)
        {
        }

        protected override void ProcessPayload()
        {
            // Get Label
        }
    }
}
