using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    [DebuggerDisplay("{FileName}")]
    public class LocalFileStructure
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
        /// Name + extension together. ex. run.exe
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The path of the file (Origin if being sent, destination if being received)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The name of the thumbnail. Empty if there isn't one
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// The number of downloads currently remaining for the file
        /// </summary>
        public int DownloadsRemaining { get; set; }

        public bool Equals(LocalFileStructure other)
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

            return Equals(obj as LocalFileStructure);
        }

        public static bool operator ==(LocalFileStructure f1, LocalFileStructure f2)
        {
            return Equals(f1, f2);
        }

        public static bool operator !=(LocalFileStructure f1, LocalFileStructure f2)
        {
            return !Equals(f1, f2);
        }


    }
}
