using FileJump.Network.EventSystem;
using FileJump_Network.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public static class DataProcessor
    {
        // Events

        public static event EventHandler<NetworkDiscoveryEventArgs> NetworkDiscoveryEvent;

        public static event EventHandler<OutTransferEventArgs> OutboundTransferFinished;

        public static event EventHandler<OutTransferEventArgs> OutboundTransferStarted;

        public static event EventHandler<FileTransferProgressEventArgs> OutboundTransferProgress;

        public static event EventHandler<InboundTransferEventArgs> IncomingTransferFinished;

        public static event EventHandler<InboundTextTransferEventArgs> InboundTextTransferFinished;

        private static int MaxInboundTransfers = 300;
        private static int MaxOutgoingTransfers = 30;

        //private static List<InboundTransferProcess> ActiveInboundProcesses;

        private static InboundTransferProcess[] ActiveInboundTransferProcesses;

        public static OutBoundTransferProcess ActiveOutboundTransferProcess;

        private static List<LocalFileStructure> QueuedOutboundFiles = new List<LocalFileStructure>();

        private static IPEndPoint TargetEndPoint { get; set; }

        private static UInt32 MaxChunkSize = 40000;

        private static bool OutboundTransfersAllowed { get; set; }

        private static IFileHandler _fileHandler { get; set; }



        private static int TestPacketsCount { get; set; } = 0;

        public static void InitializeDataProcessor(IFileHandler handler)
        {
            _fileHandler = handler;

            ActiveInboundTransferProcesses = new InboundTransferProcess[MaxInboundTransfers];

            for (int i = 0; i < ActiveInboundTransferProcesses.Length; i++)
            {
                ActiveInboundTransferProcesses[i] = null;
            }

        }

        /// <summary>
        /// Takes in the raw buffer received and processes it forward
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endPoint"></param>
        public static void ProcessRawData(byte[] data, IPEndPoint endPoint)
        {
            // Grab the first 4 bytes, which are the packet type
            string pType = Encoding.Default.GetString(data.Take(4).ToArray());

            switch (pType)
            {   
                case "_SCT": // Scout

                    // If the origin is local, ignore
                    if (endPoint.Address.ToString() == NetComm.localAddress.ToString())
                    {
                        //return;
                    }
                    

                    SendScoutingInformation(endPoint);
                    break;

                case "_SCA": // Scout Answer
                    ProcessReceivedScoutingInformation(new ScoutResponsePacket(data, endPoint));
                    break;

                case "_TFR": // Request File Transfer
                    ProcessFileTransferRequest(endPoint, new TransferRequestPacket(data));
                    break;

                case "_TFA": // Accepted file transfer
                    ProcessTFA(endPoint, new TransferAcceptPacket(data));
                    break;

                case "_DFT": // Denied file transfer
                    break;

                case "_ACK":
                    ProcessAcknowledgePacket(new AcknowledgePacket(data));
                    break;

                case "_DCP": // Data Carry Packet

                    ProcessDataCarryingPacket(new DataCarryPacket(data), endPoint);
                    break;

                case "_TTR":  // Transfer termination
                    ProcessTransferTermination(new TransferTerminationPacket(data));
                    break;

                case "_TTP": // Text Transfer Packet
                    ProcessTextTransferPacket(new TextTransferPacket(data));
                    break;
            }
        }

        /// <summary>
        /// Sends a text message or a link to the end point
        /// </summary>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        public static void SendTextMessage(string message, IPEndPoint endPoint)
        {
            int s = 2;
            TextTransferPacket ttp = new TextTransferPacket(message);

            NetComm.SendByteArray(ttp.ToByteArray(), endPoint);
        }
            

        /// <summary>
        /// Deals with Process Termination request
        /// </summary>
        /// <param name="ttp"></param>
        private static void ProcessTransferTermination(TransferTerminationPacket ttp)
        {
            if(ActiveOutboundTransferProcess != null)
            {
                ActiveOutboundTransferProcess.ReceivePacket(ttp);
            }

        }

        private static void ProcessTextTransferPacket(TextTransferPacket ttp)
        {
            InboundTextTransferFinished?.Invoke(null, new InboundTextTransferEventArgs(ttp.GetTextMessage()));
        }

        private static void ProcessAcknowledgePacket(AcknowledgePacket ack)
        {

            int id = ack.GetTransferID();
            if(ActiveOutboundTransferProcess != null)
            {
                ActiveOutboundTransferProcess.ReceivePacket(ack);
            }


        }

        /// <summary>
        /// Processes the Data Carrying Packet and delivers it to the correct process
        /// </summary>
        /// <param name="dcp"></param>
        /// <param name="ep"></param>
        private static void ProcessDataCarryingPacket(DataCarryPacket dcp, IPEndPoint ep)
        {
            int id = dcp.GetReceiverTransferID();
            

            if(ActiveInboundTransferProcesses[id] != null)
            {
                ActiveInboundTransferProcesses[id].ReceivePacket(dcp);
            }
            else
            {
                SendTransferTermination(ep, id);
            }
        }

        /// <summary>
        /// Sends an immediate termination request to the target device's process
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="id"></param>
        private static void SendTransferTermination(IPEndPoint ep, int id)
        {
            TransferTerminationPacket ttp = new TransferTerminationPacket(id);
            NetComm.SendByteArray(ttp.ToByteArray(), ep);
        }


        private static void ProcessTFA(IPEndPoint ep, TransferAcceptPacket tap)
        {
            int senderID = tap.GetSenderTransferID();
            if (ActiveOutboundTransferProcess != null)
            {
                ActiveOutboundTransferProcess.ReceivePacket(tap);
            }
            
        }

        private static void ProcessFileTransferRequest(IPEndPoint ep, TransferRequestPacket packet)
        {
            LocalFileStructure fStruct = packet.GetFileStructure();

            if (fStruct == null)
            {

                // Send error
                return;
            }
            // Check that its ok

            //if(fStruct.FileSize > 500000000) // Refuse if over 500mb
            //{
            //    // Send transfer refusal

            //    return;
            //}


            // set up

            int slot = GetAviableInboundTransferProcessID();

            if(slot == -1)
            {
                // Send transfer refusal. No slots aviable
                return;
            }

            // Create a new inbound process
            ActiveInboundTransferProcesses[slot] = new InboundTransferProcess(packet.GetSenderTransferID(),
                                                                              fStruct,
                                                                              ep,
                                                                              _fileHandler);

            ActiveInboundTransferProcesses[slot].OnTransferFinished += InboundTransferFinished;

            // Send Transfer acceptal
            TransferAcceptPacket ta = new TransferAcceptPacket(slot, packet.GetSenderTransferID());
            NetComm.SendByteArray(ta.ToByteArray(), ep);


        }

        private static void InboundTransferFinished(object sender, InboundTransferEventArgs args)
        {
           ActiveInboundTransferProcesses[args.TransferID] = null;
           IncomingTransferFinished?.Invoke(null, args);
        }


        /// <summary>
        /// Sends the information about the device to the endpoint requesting it. Follows a broadcast request
        /// </summary>
        /// <param name="ep"></param>
        private static void SendScoutingInformation(IPEndPoint ep)
        {

            ScoutResponsePacket packet = new ScoutResponsePacket(new NetworkDevice()
            {
                Name = ProgramSettings.DeviceName,
                Type = ProgramSettings.DeviceType
            });

            

            NetComm.SendByteArray(packet.ToByteArray(), ep);
        }

        private static void ProcessReceivedScoutingInformation(ScoutResponsePacket packet)
        {
            NetworkDevice device = packet.GetNetworkDevice();

            if(device == null)
            {
                return;
            }


            NetworkDiscoveryEvent?.Invoke(null, new NetworkDiscoveryEventArgs(device));
        }
        /// <summary>
        /// Tries to find an aviable slot for a new inbound transfer process. Returns -1 if none aviable
        /// </summary>
        /// <returns></returns>
        private static int GetAviableInboundTransferProcessID()
        {
            RunMaintenance();

            for (int i = 0; i < ActiveInboundTransferProcesses.Length; i++)
            {
                if(ActiveInboundTransferProcesses[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }


        public static void SendFiles(List<LocalFileStructure> fileStructures, IPEndPoint endPoint)
        {
            OutboundTransfersAllowed = true;
            // Assign the endpoint
            TargetEndPoint = endPoint;


            // Clear the queue
            QueuedOutboundFiles = new List<LocalFileStructure>();

            QueuedOutboundFiles = fileStructures;

            // Start sending the first file
            SendNextFile();
        }

        private static void SendNextFile()
        {
            if (QueuedOutboundFiles.Count <= 0) return;

            SendFile(QueuedOutboundFiles[0]);
        }

        /// <summary>
        /// Attempts to start the process of sending a file, returning the ID of the slot. If unsuccessful, returns -1
        /// </summary>
        /// <param name="fStruct"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static bool SendFile(LocalFileStructure fStruct)
        {
            if(ActiveOutboundTransferProcess != null)
            {
                return false;
            }

            ActiveOutboundTransferProcess = new OutBoundTransferProcess(fStruct, TargetEndPoint, MaxChunkSize, fStruct.FileID);
            ActiveOutboundTransferProcess.OutTransferFinished += OutTransferFinished;
            ActiveOutboundTransferProcess.OutTransferStarted += OutTransferStarted;
            ActiveOutboundTransferProcess.OutTransferProgress += OutTransferProgress;
            ActiveOutboundTransferProcess.Start();

            return true;
        }

        private static void OutTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            OutboundTransferProgress?.Invoke(null, e);
        }

        private static void OutTransferStarted(object sender, OutTransferEventArgs args)
        {
            // Call the event for the frontend
            OutboundTransferStarted?.Invoke(null, args);

            // Remove the file from the queued list
            for (int i = 0; i < QueuedOutboundFiles.Count; i++)
            {
                if(QueuedOutboundFiles[i].FilePath == args.FilePath)
                {
                    QueuedOutboundFiles.RemoveAt(i);
                }
            }
        }

        public static void TerminateTransfers()
        {
            OutboundTransfersAllowed = false;

            if(ActiveOutboundTransferProcess != null)
            {
                ActiveOutboundTransferProcess.TerminateTransfer();
            }
        }

        private static void OutTransferFinished(object sender, OutTransferEventArgs args)
        {
            ActiveOutboundTransferProcess = null;

            OutboundTransferFinished?.Invoke(null, args);

            if (OutboundTransfersAllowed)
            {
                SendNextFile();
            }
            else
            {
                OutboundTransfersAllowed = true;
            }
           

        }

        /// <summary>
        /// Checks all the processes for inactive ones
        /// </summary>
        private static void RunMaintenance()
        {

        }

    }
}
