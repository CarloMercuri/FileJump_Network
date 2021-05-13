using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class NetworkDevice
    {
        /// <summary>
        /// The name given to the device by the owner
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The EndPoint of the device
        /// </summary>
        public IPEndPoint EndPoint { get; set; }

        /// <summary>
        /// The type of the device (Desktop, Laptop, Mobile)
        /// </summary>
        public NetworkDeviceType Type { get; set; }
    }
}
