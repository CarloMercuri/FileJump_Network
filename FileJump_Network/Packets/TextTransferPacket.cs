using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    class TextTransferPacket : INetworkPacket
    {
        // 4 bytes header, 4 bytes int string lenght, char[]

        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        private string PacketText { get; set; }

        public TextTransferPacket(string text)
        {
            PacketType = "_TTP";
            PacketText = text;
        }

        public TextTransferPacket(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            PacketType = Encoding.ASCII.GetString(buffer.ReadBytes(4));
            PacketText = buffer.ReadString();

            buffer.Dispose();

        }

        public string GetTextMessage()
        {
            return PacketText;
        }

        public byte[] ToByteArray()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(Encoding.ASCII.GetBytes(PacketType));
            buffer.WriteString(PacketText);

            return buffer.ToArray();

        }
    }
}
