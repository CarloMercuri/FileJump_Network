using System;
using System.Collections.Generic;
using System.IO;
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
        //public static string BaseURL = "https://login.taotek.dk";
        
        public static string BaseURL = "https://localhost:44388";
        public static string API_REGISTRATION_POST_URL = "api/useraccounts/RegisterNew";
        public static string API_TEST_POST_URL = "/RegisterNew";
        public static string API_LOGIN_POST = "Authentication/Login";
        //public static string API_TEST_GET_URL = "UserAccounts/5";
        public static string API_TEST_AUTH_GET = "/UserAuthenticate";
        public static string API_NEW_ACCOUNT_POST = "/Accounts/RegisterNew";
        public static string API_CHANGE_PASSWORD_POST = "/Accounts/ChangePassword";
        public static string API_PASSWORD_RECOVERY_POST = "Accounts/RequestPasswordRecovery";

        // File hosting
        public static string API_FILEHOST_GET_THUMBNAIL = "/FileHosting/GetThumbnail";
        public static string API_FILEHOST_GET_FILES_LIST = "/FileHosting/GetUserFiles";
        public static string API_FILEHOST_GET_FILE = "/FileHosting/GetFile";
        public static string API_FILEHOST_DELETE_FILE = "/FileHosting/DeleteFile";
        public static string API_FILEHOST_UPLOAD_FILE = "/FileHosting/FileUpload";

        public static string API_FILE_DOWNLOAD_FOLDER
        {
            get { return GetDownloadsFolder(); }
        }

        private static string GetDownloadsFolder()
        {
            string folder = Path.Combine(ProgramSettings.StorageFolderPath, "Downloads");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

    }

    public enum SendProcessState
    {
        WaitingForAck = 1,
        SentTransferRequest = 2,
        Inactive = 3,
        Terminating = 4,

    }

    public enum FileStatus
    {
        Inactive = 1,
        Active = 2,
        Finished = 3,
        QueuedForDownload = 4,
        Deleted = 5
    }

    public enum PacketType
    {
        Ack = 1,
        DATA_CARRIER = 2,
        SCOUT = 3, // The LAN Broadcast message packet
        SCOUT_ANSWER = 4,
    }

    public enum FileTransferType
    {
        Local = 1,
        Online = 2
    }

    public enum NetworkDeviceType
    {
        Desktop = 1,
        Laptop = 2,
        MobilePhone = 3,
    }
}
