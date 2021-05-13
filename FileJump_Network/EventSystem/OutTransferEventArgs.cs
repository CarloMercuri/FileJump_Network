using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class OutTransferEventArgs
    {
        /// <summary>
        /// The full path of the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// True if successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Message that came from the server
        /// </summary>
        public string Message { get; set; }

        public OutTransferEventArgs(string _filePath, bool _successful, string _msg)
        {
            FilePath = _filePath;
            IsSuccessful = _successful;
            Message = _msg;
        }
    }
}
