using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class TransferAcceptPacket : INetworkPacket
    {
        // 4 bytes header, 4 bytes int receiver transfer ID, 4 bytes int sender transferID
        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public TransferAcceptPacket(int receiverID, int senderID)
        {
            PacketType = "_TFA";
            PacketData = new byte[8];
            Array.Copy(BitConverter.GetBytes(receiverID), 0,  PacketData, 0, 4);
            Array.Copy(BitConverter.GetBytes(senderID), 0,  PacketData, 4, 4);
        }
        /// <summary>
        /// Creates a new TransferAcceptPacket given a buffer
        /// </summary>
        /// <param name="buffer"></param>
        public TransferAcceptPacket(byte[] buffer)
        {
            if(buffer.Length < 8)
            {
                return;
            }

            PacketType = Encoding.ASCII.GetString(buffer, 0, 4);
            PacketData = new byte[8];
            Array.Copy(buffer, 4, PacketData, 0, 8);

        }

        /// <summary>
        /// Returns the ID that the receiver uses for this transfer
        /// </summary>
        /// <returns></returns>
        public int GetReceiverTransferID()
        {
            return BitConverter.ToInt32(PacketData, 0);
        }

        public int GetSenderTransferID()
        {
            return BitConverter.ToInt32(PacketData, 4);
        }

        public byte[] ToByteArray()
        {
            if(PacketData == null)
            {
                return null;
            }

            byte[] data = new byte[12];
            Array.Copy(Encoding.ASCII.GetBytes(PacketType), data, 4);
            Array.Copy(PacketData, 0, data, 4,  8);

            return data;
        }
    }
}
