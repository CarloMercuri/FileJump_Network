using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class ScoutResponsePacket : INetworkPacket
    {
        // STRUCTURE: 4 bytes for type, flexible string for device name, byte for device type, string for ip, int for port
        // The device endpoint will be added at the receiving end
        public string PacketType { get; set; }
        public byte[] PacketData { get; set; }

        public IPEndPoint PacketEndPoint { get; set; }

        /// <summary>
        /// Creates a ScoutResponsePacket given a device name and type
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="deviceType"></param>
        public ScoutResponsePacket(string deviceName, NetworkDeviceType deviceType)
        {
            PacketType = "_SCA";
            PacketBuffer buffer = new PacketBuffer();

            buffer.WriteString(deviceName);
            buffer.WriteByte((byte)deviceType);
            
            PacketData = buffer.ToArray();
        }

        /// <summary>
        /// Creates a ScoutResponsePacket given a NetworkDevice structure
        /// </summary>
        /// <param name="device"></param>
        public ScoutResponsePacket(NetworkDevice device)
        {
            PacketType = "_SCA";
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteString(device.Name);
            buffer.WriteByte((byte)device.Type);

            PacketData = buffer.ToArray();

        }

        /// <summary>
        /// Returns a NetworkDevice object containing the information included in the packet.
        /// </summary>
        /// <returns></returns>
        public NetworkDevice GetNetworkDevice()
        {
            // The packet data needs to be at least 6 bytes long. Strings in the packetbuffer class are saved with a
            // 4-bytes integer for the lenght, and then the string itself
            if(PacketData == null || PacketData.Length < 6)
            {
                return null;
            }

            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(PacketData);
               
            NetworkDevice device = new NetworkDevice();
            device.Name = buffer.ReadString();
            device.Type = (NetworkDeviceType)buffer.ReadByte();
            device.EndPoint = PacketEndPoint;
            buffer.Dispose();

            return device;
        }

        /// <summary>
        /// Deserializes a data buffer into a ScoutResponsePacket
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endPoint"></param>
        public ScoutResponsePacket(byte[] data, IPEndPoint endPoint)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            PacketType = Encoding.Default.GetString(buffer.ReadBytes(4));
            PacketData = new byte[data.Length - 4];
            PacketEndPoint = endPoint;
            Array.Copy(data, 4, PacketData, 0, PacketData.Length);
            buffer.Dispose();
        }

        /// <summary>
        /// Serializes the packet into a byte array for sending
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(Encoding.ASCII.GetBytes(PacketType));
            buffer.WriteBytes(PacketData);
            return buffer.ToArray();
        }
    }
}
