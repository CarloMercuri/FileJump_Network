using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class TransferTerminationPacket : INetworkPacket
    {
        // 4 bytes string type, 4 bytes int id
        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public int TransferID { get; set; }

        public TransferTerminationPacket(int id)
        {
            PacketType = "_TTR";
            TransferID = id;
        }

        public TransferTerminationPacket(byte[] data)
        {
            PacketType = Encoding.ASCII.GetString(data, 0, 4);
            TransferID = BitConverter.ToInt32(data, 4);
        }

        public byte[] ToByteArray()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(Encoding.ASCII.GetBytes(PacketType));
            buffer.WriteInteger(TransferID);
            return buffer.ToArray();
        }
    }
}
