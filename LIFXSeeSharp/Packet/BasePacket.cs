using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LIFXSeeSharp.Packet
{
        class BasePacket : IPacket
        {
                public virtual byte Type { get { return 0x0; } }
                public IPAddress IP { get; private set; }
                public ulong Mac { get; private set; }
                public ulong Site { get; private set; }
                public byte[] Payload { get; private set; }

                public BasePacket()
                {
                }

                public BasePacket(byte[] payload, IPAddress ip)
                {
                        Payload = payload;
                        IP = ip;
                        ProcessPayload();
                }

                protected virtual void ProcessPayload()
                {
                        var macSubArray = new byte[8];
                        var siteAddressSubArray = new byte[8];

                        Array.Copy(Payload, 8, macSubArray, 0, 6);
                        Array.Copy(Payload, 16, siteAddressSubArray, 0, 6);

                        Mac = (ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(macSubArray, 0));
                        Site = (ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(siteAddressSubArray, 0));
                }

                public override string ToString()
                {
                        return "Base Packet: IP " + IP.ToString();
                }
        }
}
