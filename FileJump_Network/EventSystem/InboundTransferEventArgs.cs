using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class InboundTransferEventArgs : EventArgs
    {
        public int TransferID { get; set; }

        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public byte[] FileBuffer { get; set; }

        public FileStructure FileStructure { get; set; }

        public InboundTransferEventArgs(int _transferID, bool _successful, byte[] _fileBuffer, FileStructure _fileStructure)
        {
            TransferID = _transferID;
            IsSuccessful = _successful;
            FileBuffer = _fileBuffer;
            FileStructure = _fileStructure;
        }
    }
}
