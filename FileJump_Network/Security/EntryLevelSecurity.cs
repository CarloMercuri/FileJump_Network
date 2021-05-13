using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network.Security
{
    public static class EntryLevelSecurity
    {
        /// <summary>
        /// General checks about the endpoint
        /// </summary>
        /// <returns></returns>
        public static bool IsEndpointAuthorized(EndPoint ep)
        {
            return true;
        }
    }
}
