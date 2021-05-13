using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public static class Constants
    {
        public static string TRANSFER_SUCCESSFUL = "File Transfer Successful";
        public static string TRANSFER_TERMINATED = "Transfer has been terminated";
        public static string DEVICE_TIMEOUT = "Target device timed out.";
    }

    public enum SendProcessState
    {
        WaitingForAck = 1,
        SentTransferRequest = 2,
        Inactive = 3,
        Terminating = 4,

    }

    public enum FileQueueState
    {
        Inactive = 1,
        Active = 2,
        Finished = 3,
    }

    public enum PacketType
    {
        Ack = 1,
        DATA_CARRIER = 2,
        SCOUT = 3, // The LAN Broadcast message packet
        SCOUT_ANSWER = 4,


    }

    public enum NetworkDeviceType
    {
        Desktop = 1,
        Laptop = 2,
        MobilePhone = 3,
    }
}
