using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileJump_Network
{
    public static class Authentication
    {
        /// <summary>
        /// Encodes a username:password combo into a base 64 string
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public static string Base64Encode(string username, string password)
        {
            string textToEncode = $"{username}:{password}";
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }

       

        public static string GetSessionToken()
        {
            
            return "";
        }

        public static string GetSessionEmail()
        {
            return "";
        }

        public static bool IsLoggedIn(out string sessionEmail)
        {
            sessionEmail = GetSessionEmail();
            return false;
        }
    }
}
