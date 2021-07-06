using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class ThumbnailReceivedEventArgs : EventArgs
    {
        public byte[] ImageBytes { get; set; }
    }
}
