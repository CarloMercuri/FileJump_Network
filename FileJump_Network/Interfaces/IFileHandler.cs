using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public interface IFileHandler
    {
        byte[] FileData { get; set; }
        LocalFileStructure FileStructure { get; set; }

        void SaveFileToLocalStorage();
    }
}
