using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network.EventSystem
{
    public class FileTransferProgressEventArgs
    {
        public string FileName { get; set; }
        public int PercentProgress { get; set; }

        public int ID { get; set; }
    }
}
