using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.EventSystem
{
    public class RegistrationResultEventArgs : EventArgs
    {
        /// <summary>
        /// The message that came with the HttpResponse
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Wether it was successful or not
        /// </summary>
        public bool Successful { get; set; }
    }
}
