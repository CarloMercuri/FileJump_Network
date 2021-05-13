using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class ScoutPacket : INetworkPacket
    {
        // STRUCTURE: 4 bytes for type

        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public ScoutPacket()
        {
            PacketType = "_SCT";
            PacketData = null;
        }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(PacketType);
        }
    }
}
