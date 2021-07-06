using FileJump_Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.Models
{
    public class LoginRequestResponseModel : IActionApiResponse
    {
        public AccountInfoModel AccountModel { get; set; }

        public bool Successful { get; set; }
        public HttpResponseMessage Response { get; set; }

        public string Message { get; set; }

        
    }
}
