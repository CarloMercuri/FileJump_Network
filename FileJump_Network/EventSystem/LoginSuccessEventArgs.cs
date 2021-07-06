using FileJump_Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class LoginSuccessEventArgs : EventArgs
    {

        public LoginSuccessEventArgs(AccountInfoModel AccountModel)
        {
            AccountData = AccountModel;
        }

        public AccountInfoModel AccountData { get; set; }

    }
}
