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

        /// <summary>
        /// The ID of the file
        /// </summary>
        public int FileID { get; set; }

        public OutTransferEventArgs(string _filePath, bool _successful, string _msg, int _fileID = -1)
        {
            FilePath = _filePath;
            IsSuccessful = _successful;
            Message = _msg;
            FileID = _fileID;
        }
    }
}
