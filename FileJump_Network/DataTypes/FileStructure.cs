using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    [DebuggerDisplay("{FileName}")]
    public class FileStructure
    {
        /// <summary>
        /// The Name of the file, without the extension
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The extension of the file, including the dot (e.g. .bmp)
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// The path of the file (Origin if being sent, destination if being received)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        public long FileSize { get; set; }

        public bool Equals(FileStructure other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return string.Equals(FileName, other.FileName)
                && string.Equals(FileExtension, other.FileExtension)
                && FileSize == other.FileSize;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return Equals(obj as FileStructure);
        }

        public static bool operator ==(FileStructure f1, FileStructure f2)
        {
            return Equals(f1, f2);
        }

        public static bool operator !=(FileStructure f1, FileStructure f2)
        {
            return !Equals(f1, f2);
        }


    }
}
