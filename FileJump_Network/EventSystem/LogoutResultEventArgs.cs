﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class LogoutResultEventArgs : EventArgs
    {
        public bool Successful { get; set; }

        public string Message { get; set; }
    }
}
