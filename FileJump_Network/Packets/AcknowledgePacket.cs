using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class AcknowledgePacket : INetworkPacket
    {
        // 4 bytes type, 4 bytes uint number, 4 bytes int id

        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        private uint PacketNumber { get; set; }

        private int TransferID { get; set; }

        public AcknowledgePacket(uint _packetNumber, int _transferID)
        {
            PacketType = "_ACK";
            PacketNumber = _packetNumber;
            TransferID = _transferID;
        }

        public AcknowledgePacket(byte[] bytes)
        {
            PacketType = "_ACK";
            PacketNumber = BitConverter.ToUInt32(bytes, 4);
            TransferID = BitConverter.ToInt32(bytes, 8);
        }

        public uint GetPacketNumber()
        {
            return PacketNumber;
        }

        public int GetTransferID()
        {
            return TransferID;
        }

        public byte[] ToByteArray()
        {
            byte[] data = new byte[8];
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(Encoding.ASCII.GetBytes(PacketType));
            buffer.WriteBytes(BitConverter.GetBytes(PacketNumber));
            buffer.WriteBytes(BitConverter.GetBytes(TransferID));
            return buffer.ToArray();

        }
    }
}
