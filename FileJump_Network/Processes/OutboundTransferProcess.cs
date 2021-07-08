using FileJump.Network.EventSystem;
using FileJump_Network.EventSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public class OutBoundTransferProcess : IDisposable
    {
        /// <summary>
        /// The EndPoint on the receiving end
        /// </summary>
        public IPEndPoint TargetEndPoint { get; set; }

        /// <summary>
        /// The ID of the transfer
        /// </summary>
        public int LocalTransferID { get; set; }

        /// <summary>
        /// The ID given by the server
        /// </summary>
        public int RemoteTransferID { get; set; }

        /// <summary>
        /// The information about the file
        /// </summary>
        private LocalFileStructure SelectedFileStructure { get; set; }

        /// <summary>
        /// The bytes of the selected file
        /// </summary>
        private byte[] FileBuffer { get; set; }

        /// <summary>
        /// The size of each chunk of data being sent
        /// </summary>
        private UInt32 ChunkSize { get; set; }

        /// <summary>
        /// The next block number to be sent
        /// </summary>
        private UInt32 ChunkNumber { get; set; }

        /// <summary>
        /// The index of where we are in the file buffer
        /// </summary>
        private long ReadIndex { get; set; }


        /// <summary>
        /// Returns true if the last chunk has been sent
        /// </summary>
        private bool LastChunkSent { get; set; }

        /// <summary>
        /// If the transfer is active
        /// </summary>
        private bool IsRunning { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        private Thread ProcessThread { get; set; }

        /// <summary>
        /// The timer. TODO: Find some alternative?
        /// </summary>
        private Stopwatch timer { get; set; }

        /// <summary>
        /// Milliseconds since a last action (any)
        /// </summary>
        private long LastActionSnapshot { get; set; }

        /// <summary>
        /// The state of the process
        /// </summary>
        private SendProcessState ProcessState { get; set; }

        /// <summary>
        /// If it is to be terminated
        /// </summary>
        private bool Terminated { get; set; }

        /// <summary>
        /// The last time we have heard from the receiver
        /// </summary>
        private long LastCommunicationSnapshot { get; set; }

        private int DownloadProgressNextTick { get; set; }

        private int DownloadProgressTreshold { get; set; }

        /// <summary>
        /// Returns the time in milliseconds since we last heard from the receiver
        /// </summary>
        public long TimeSinceLastCommunication
        {
            get { return GetTimeSinceLastComm(); }
            private set {;}
        }

        // Events
        public event EventHandler<OutTransferEventArgs> OutTransferFinished;
        public event EventHandler<OutTransferEventArgs> OutTransferStarted;
        public event EventHandler<FileTransferProgressEventArgs> OutTransferProgress;



        public OutBoundTransferProcess(LocalFileStructure _file, IPEndPoint _endPoint, UInt32 _chunkSize, int _localID)
        {
            TargetEndPoint = _endPoint;
            SelectedFileStructure = _file;
            ChunkSize = _chunkSize;
            ReadIndex = 0;
            ChunkNumber = 0;
            timer = new Stopwatch();
            LocalTransferID = _localID;
            ProcessState = SendProcessState.Inactive;
            Terminated = false;
            
            DownloadProgressTreshold = 5; // Update progress every 5%
        }

        public void TerminateTransfer()
        {
            Terminated = true;
        }

        private void LoadFile()
        {
            // Should I put some safety checks? The previous ones should be enough
            FileBuffer = File.ReadAllBytes(SelectedFileStructure.FilePath);
        }

        public void Start()
        {
            timer.Start();
            IsRunning = true;
            ProcessThread = new Thread(MainLoop);
            ProcessThread.Start();
        }

        /// <summary>
        /// The entry point for a packet
        /// </summary>
        /// <param name="packet"></param>
        public void ReceivePacket(object packet)
        {

            switch (packet)
            {
                case TransferAcceptPacket s:
                    LastCommunicationSnapshot = timer.ElapsedMilliseconds;
                    RemoteTransferID = s.GetReceiverTransferID();
                    SendChunk();
                    ProcessState = SendProcessState.WaitingForAck;
                    OutTransferStarted?.Invoke(this, new OutTransferEventArgs(SelectedFileStructure.FilePath, true, ""));
                    break;

                case AcknowledgePacket ack:

                    LastCommunicationSnapshot = timer.ElapsedMilliseconds;

                    if (LastChunkSent)
                    {
                        // If we get the ack for the last packet, then we're done
                        ProcessState = SendProcessState.Terminating;
                        EndProcess();
                        return;
                    }

                    ChunkNumber = ack.GetPacketNumber() + 1;
                    SendChunk();

                    break;

                case TransferTerminationPacket ttp:
                    LastCommunicationSnapshot = timer.ElapsedMilliseconds;  // Probably not necessary. Eh.
                    ProcessState = SendProcessState.Terminating;
                    Terminated = true;
                    return;

                default:
                    break;
            }

            
        }

        /// <summary>
        /// Sends a packet with the new chunk
        /// </summary>
        private void SendChunk()
        {
            // Update progress
            decimal pp = (decimal)ReadIndex / (decimal)FileBuffer.Length;
            int percent = (int)Math.Round(pp * 100);

            if (percent > DownloadProgressNextTick)
            {
                FileTransferProgressEventArgs args = new FileTransferProgressEventArgs();
                args.PercentProgress = percent;
                args.ID = LocalTransferID;
                args.FileName = SelectedFileStructure.FullName;
                OutTransferProgress?.Invoke(null, args);
                DownloadProgressNextTick = percent + DownloadProgressTreshold;
            }

            DataCarryPacket dcp =  CreateDataPacket();
            NetComm.SendByteArray(dcp.ToByteArray(), TargetEndPoint);
            LastActionSnapshot = timer.ElapsedMilliseconds;
            //DataCarryPacket dcp = new DataCarryPacket(NextBlockNumber, TransferID, )
        }


        private void EndProcess()
        {
            IsRunning = false;
        }

        private long GetTimeSinceLastComm()
        {
            if (timer.IsRunning)
            {
                return timer.ElapsedMilliseconds - LastCommunicationSnapshot;
            } 
            else
            {
                return 0;
            }
        }


        private DataCarryPacket CreateDataPacket()
        {

            if (IsLastChunk())
            {
                LastChunkSent = true;
                DataCarryPacket dcp = new DataCarryPacket(ChunkNumber, RemoteTransferID, true, GetChunk(FileBuffer.Length - ReadIndex));
                return dcp;
            }
            else
            {
                DataCarryPacket dcp = new DataCarryPacket(ChunkNumber, RemoteTransferID, false, GetChunk(ChunkSize));
                return dcp;

            }
        }

        private byte[] GetChunk(long lenght)
        {
            byte[] array = new byte[lenght];
            Array.Copy(FileBuffer, ReadIndex, array, 0, lenght);
            ReadIndex += lenght;
            return array;
        }

        private bool IsLastChunk()
        {
            return ReadIndex + ChunkSize >= FileBuffer.Length;
        }

        /// <summary>
        /// The main maintenance loop
        /// </summary>
        private void MainLoop()
        {
            // First we load the file
            LoadFile();

            // Sending the request packet
            TransferRequestPacket trp = new TransferRequestPacket(SelectedFileStructure, LocalTransferID);
            NetComm.SendByteArray(trp.ToByteArray(), TargetEndPoint);

            // Update the state
            ProcessState = SendProcessState.SentTransferRequest;

            // After this it is gonna run passively. It sends a packet, and when ACK comes, sends the next.
            // The following loop checks that ACKs came back etc.

            while (IsRunning)
            {
                Thread.Sleep(20);

                if (Terminated)
                {
                    // Forced termination
                    OutTransferFinished?.Invoke(this, new OutTransferEventArgs(SelectedFileStructure.FilePath, false, Constants.TRANSFER_TERMINATED, LocalTransferID));
                    return;
                }

                // If too much time has passed without an ack, resend the last packet
                if(timer.ElapsedMilliseconds - LastActionSnapshot > 200 && ProcessState == SendProcessState.WaitingForAck)
                {
                    SendChunk();
                }

                // If too much time has passed without hearing from the server, shut down the process
                if(TimeSinceLastCommunication > 3000)
                {
                    OutTransferFinished?.Invoke(this, new OutTransferEventArgs(SelectedFileStructure.FilePath, false, Constants.DEVICE_TIMEOUT, LocalTransferID));
                    return;
                }

            }

            // We are finished
            OutTransferFinished?.Invoke(this, new OutTransferEventArgs(SelectedFileStructure.FilePath, true, Constants.TRANSFER_SUCCESSFUL, LocalTransferID));

            Dispose();
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileBuffer = null;
                }
                
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}
