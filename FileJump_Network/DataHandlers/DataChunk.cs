using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump
{
    public class DataChunk
    {
        /// <summary>
        /// The Sequence Number of the chunk
        /// </summary>
        public uint ChunkNumber { get; set; }

        /// <summary>
        /// The Data (byte array) contained in the chunk
        /// </summary>
        public byte[] Data { get; set; } = new byte[0];

        public DataChunk(uint _number, byte[] _bytes)
        {
            ChunkNumber = _number;
            Data = _bytes;
        }

        
    }
}
