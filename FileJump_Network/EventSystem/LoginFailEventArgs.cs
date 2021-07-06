using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class LoginFailEventArgs : EventArgs
    {
        public LoginFailEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
