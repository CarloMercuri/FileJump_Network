using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class DataCarryPacket : INetworkPacket
    {
        // { 0, 1, 2, 3 } string PacketType
        // { 4, 5, 6, 7 } int TransferID
        // { 8, 9, 10, 11 } UInt32 PacketNumber
        // { 12 } bool IsLast
        // { 13++ } Data

        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public UInt32 PacketNumber { get; set; }

        private int TransferID { get; set; }

        public bool IsLast { get; set; }

        public DataCarryPacket(UInt32 _packetNumber, int _transferID, bool _isLast, byte[] buffer)
        {
            PacketType = "_DCP";
            PacketNumber = _packetNumber;
            TransferID = _transferID;
            IsLast = _isLast;
            PacketData = buffer;
        }

        public DataCarryPacket(byte[] buffer)
        {
            PacketType = Encoding.ASCII.GetString(buffer, 0, 4);
            TransferID = BitConverter.ToInt32(buffer, 4);
            PacketNumber = BitConverter.ToUInt32(buffer, 8);
            IsLast = BitConverter.ToBoolean(buffer, 12);

            PacketData = new byte[buffer.Length - 13];
            Array.Copy(buffer, 13, PacketData, 0, PacketData.Length);
        }

        public int GetReceiverTransferID()
        {
            return TransferID;
        }


        /// <summary>
        /// Returns the serialized packet
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            // Using this method instead of using Packetbuffer because of performance.
            // This method is around 3 times faster
            byte[] data = new byte[PacketData.Length + 13];

            Array.Copy(Encoding.ASCII.GetBytes(PacketType), data, 4);
            Array.Copy(BitConverter.GetBytes(TransferID), 0,  data, 4, 4);
            Array.Copy(BitConverter.GetBytes(PacketNumber), 0,  data, 8, 4);
            data[12] = Convert.ToByte(IsLast);
            Array.Copy(PacketData, 0, data, 13, PacketData.Length);

            return data;
        }

    }
}
