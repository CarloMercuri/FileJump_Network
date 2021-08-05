using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public interface IFileHandler
    {
        void SaveFileToLocalStorage(byte[] buffer, LocalFileStructure fStruct);
        string GetValidPath(string fileName);
    }
}
