using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public interface INetworkPacket
    {
        string PacketType { get; set; }
        byte[] PacketData { get; set; }

        byte[] ToByteArray();

    }
}
