using FileJump_Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.Models
{
    public class PasswordRecoveryResponseModel : IActionApiResponse
    {
        public bool Successful { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}
