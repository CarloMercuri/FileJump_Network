using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class TransferRequestPacket : INetworkPacket
    {
        // 4 bytes header for type. 4 bytes int for id, 8 bytes long for size, string for name, string for extension
        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public TransferRequestPacket(byte[] data)
        {
            PacketType = Encoding.Default.GetString(data.Take(4).ToArray());
            PacketData = new byte[data.Length - 4];
            Array.Copy(data, 4, PacketData, 0, PacketData.Length);
        }

        public TransferRequestPacket(FileStructure fStruct, int localID)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger(localID);
            buffer.WriteLong(fStruct.FileSize);
            buffer.WriteString(fStruct.FileName);
            buffer.WriteString(fStruct.FileExtension);
            PacketData = buffer.ToArray();
            PacketType = "_TFR";
        }

        public int GetSenderTransferID()
        {
            return BitConverter.ToInt32(PacketData, 0);
        }

        /// <summary>
        /// Returns a FileStructure object containing the information
        /// </summary>
        /// <returns></returns>
        public FileStructure GetFileStructure()
        {
            if (PacketData == null || PacketData.Length < 12)
            {
                return null;
            }

            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(PacketData);
            /*
            FileStructure fStruct = new FileStructure()
            {
                FileSize = buffer.ReadLong(),
                FileName = buffer.ReadString(),
                FileExtension = buffer.ReadString()
            };

            
            */

            FileStructure fStruct = new FileStructure();
            buffer.ReadInteger(); // Ignore the local ID
            fStruct.FileSize = buffer.ReadLong();
            fStruct.FileName = buffer.ReadString();
            fStruct.FileExtension = buffer.ReadString();

            buffer.Dispose();

            return fStruct;
        }

        /// <summary>
        /// Serializes the packet into a byte array for sending
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            if(PacketData == null)
            {
                return null;
            }

            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(Encoding.ASCII.GetBytes(PacketType));
            buffer.WriteBytes(PacketData);
            return buffer.ToArray();
        }
    }
}
