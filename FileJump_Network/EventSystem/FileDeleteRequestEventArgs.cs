using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class FileDeleteRequestEventArgs
    {
        public bool Successful { get; set; }
        public string FileName { get; set; }
        public int ID { get; set; }
    }
}
