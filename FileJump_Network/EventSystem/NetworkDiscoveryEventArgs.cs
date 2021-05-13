using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class NetworkDiscoveryEventArgs : EventArgs
    {
        public NetworkDevice device { get; set; }

        public NetworkDiscoveryEventArgs(NetworkDevice _device)
        {
            device = _device;
        }
    }
}
