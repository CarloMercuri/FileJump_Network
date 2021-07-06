using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class OnlineFileStructure
    {

        /// <summary>
        /// The extension of the file, including the dot (e.g. .bmp)
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Name + extension together. ex. run.exe
        /// </summary>
        public string FullName { get; set; }

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

        /// <summary>
        /// Download progress as a 0-100 value
        /// </summary>
        public int DownloadProgress { get; set; }

        public bool Equals(OnlineFileStructure other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return string.Equals(FullName, other.FullName);
                //&& FileSize == other.FileSize;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return Equals(obj as OnlineFileStructure);
        }

        public static bool operator ==(OnlineFileStructure f1, OnlineFileStructure f2)
        {
            return Equals(f1, f2);
        }

        public static bool operator !=(OnlineFileStructure f1, OnlineFileStructure f2)
        {
            return !Equals(f1, f2);
        }

    }
}
