using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class InboundTextTransferEventArgs : EventArgs
    {
        public string Message { get; set; }

        public InboundTextTransferEventArgs(string text)
        {
            Message = text;
        }
    }
}
