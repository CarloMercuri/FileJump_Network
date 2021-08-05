using FileJump.Network.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public static class NetComm
    {
        //NEW
        private static UdpClient socket;

        public static IPEndPoint listenerEP { get; private set; }

        public static List<EndPoint> cooldownEPList;

        public static int packetCount = 1;

        public static int listeningPort = 25000;

        public static IPAddress localAddress;

        public static void InitializeNetwork(string device_name, int device_type, string files_folder, IFileHandler handler)
        {
            ProgramSettings.DeviceName = device_name;
            ProgramSettings.DeviceType = (NetworkDeviceType)device_type;
            ProgramSettings.StorageFolderPath = files_folder;

            DataProcessor.InitializeDataProcessor(handler);
            ApiCommunication.InitializeClient();
            localAddress = IPAddress.Parse(GetLocalIPAddress());
            listenerEP = new IPEndPoint(IPAddress.Any, listeningPort);
            socket = new UdpClient(listeningPort);
            socket.BeginReceive(new AsyncCallback(ReceiveCompletedCallback), null);
        }

        private static void ReceiveCompletedCallback(IAsyncResult aResult)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            byte[] data = socket.EndReceive(aResult, ref remoteEP);
            socket.BeginReceive(ReceiveCompletedCallback, null);

            // All valid packets have to contain a 4 byte header with the packet type
            if (data.Length < 4)
            {
                return;
            }

            // TODO: all
            if (!EntryLevelSecurity.IsEndpointAuthorized(remoteEP))
            {
                return;
            }

            // Forward the buffer directly
            DataProcessor.ProcessRawData(data, remoteEP);




        }

        public static void ScoutNetworkDevices()
        {
            ScoutPacket packet = new ScoutPacket();

            /*
            SendByteArray(packet.ToByteArray(), new IPEndPoint(
                IPAddress.Parse("255.255.255.255"), 25000
                ));
            */

            /*
            SendByteArray(packet.ToByteArray(), new IPEndPoint(
               IPAddress.Parse("192.168.191.255"), 25000
               ));
            */
            SendByteArray(packet.ToByteArray(), new IPEndPoint(
              IPAddress.Broadcast, 25000
              ));

        }

        /// <summary>
        /// Send a byte array to the specified endpoint
        /// </summary>
        /// <param name="data"></param>
        /// <param name="remoteEP"></param>
        public static void SendByteArray(byte[] data, IPEndPoint remoteEP)
        {
            socket.SendAsync(data, data.Length, remoteEP);
        }

        /// <summary>
        /// Returns the local IP address of the machine
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }


}
