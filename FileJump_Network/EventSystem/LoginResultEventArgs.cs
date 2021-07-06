using FileJump_Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class LoginResultEventArgs : EventArgs 
    {

        public LoginResultEventArgs()
        {

        }

        public LoginResultEventArgs(AccountInfoModel AccountModel, bool successful)
        {
            AccountData = AccountModel;
            Successful = successful;
        }

        public LoginResultEventArgs(AccountInfoModel AccountModel, bool successful, string message)
        {
            AccountData = AccountModel;
            Successful = successful;
            Message = message;
        }

        public string Message { get; set; }

        public AccountInfoModel AccountData { get; set; }
        public bool Successful { get; set; }
    }
}
