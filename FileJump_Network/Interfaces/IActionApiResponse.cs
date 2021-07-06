using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileJump_Network.Interfaces
{
    public interface IActionApiResponse
    {
        bool Successful { get; set; }
        HttpResponseMessage Response { get; set; }
    }
}
