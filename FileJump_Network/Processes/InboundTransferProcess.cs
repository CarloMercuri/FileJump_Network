using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class InboundTransferProcess
    {
        /// <summary>
        /// The EndPoint sending the data
        /// </summary>
        public IPEndPoint SenderEndPoint { get; private set; }

        private FileStructure IncomingFileStructure { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// The name of the file being received
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// The extension of the file being received
        /// </summary>
        public string FileExtension { get; private set; }

        /// <summary>
        /// The ID of the current transfer
        /// </summary>
        public int TransferID { get; private set; }

        /// <summary>
        /// The chunks being received
        /// </summary>
        private List<DataChunk> ReceivedChunks { get; set; } = new List<DataChunk>();

        /// <summary>
        /// Not sure if it needs to be used
        /// </summary>
        public UInt32 ExpectedChunksAmount { get; set; }

        /// <summary>
        /// The chunks received so far
        /// </summary>
        public UInt32 ReceivedChunksCount { get; set; }

        /// <summary>
        /// The expected next chunk ID. Starts from 0
        /// </summary>
        public UInt32 ExpectedNextChunkNumber { get; set; }



        public event EventHandler<InboundTransferEventArgs> OnTransferFinished;


        public InboundTransferProcess(int _transferID, FileStructure _fileStructure, IPEndPoint _senderEP)
        {
            ExpectedNextChunkNumber = 0;
            ReceivedChunksCount = 0;
            IncomingFileStructure = _fileStructure;
            SenderEndPoint = _senderEP;
            TransferID = _transferID;
        }

        public bool Shutdown()
        {
            return true;
        }

        /// <summary>
        /// Receives and processes an INetworkPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void ReceivePacket(INetworkPacket packet)
        {
            switch (packet)
            {
                case DataCarryPacket dcp:
                    // Send the ack packet back
                    AcknowledgePacket ack = new AcknowledgePacket(dcp.PacketNumber, TransferID);
                    NetComm.SendByteArray(ack.ToByteArray(), SenderEndPoint);

                    // If it's marked as last, process the file
                    if (dcp.IsLast)
                    {
                        ReceivedChunks.Add(new DataChunk(dcp.PacketNumber, dcp.PacketData));
                        ProcessFinishedFile();
                    }
                    else
                    {
                        ReceivedChunks.Add(new DataChunk(dcp.PacketNumber, dcp.PacketData));
                    }
                    
                    break;
            }
        }

  

        private void ProcessFinishedFile()
        {
            // Create the byte buffer and include it in the OnTransferFinished event
            PacketBuffer buffer = new PacketBuffer();

            for (int i = 0; i < ReceivedChunks.Count; i++)
            {
                buffer.WriteBytes(ReceivedChunks[i].Data);
            }

            byte[] fileBuffer = buffer.ToArray();

            OnTransferFinished?.Invoke(null, new InboundTransferEventArgs(TransferID, true, fileBuffer, IncomingFileStructure));
            
        }


    }
}
