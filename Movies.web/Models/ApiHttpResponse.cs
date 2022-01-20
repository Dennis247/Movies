using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Movies.web.Models
{
    public class ApiHttpResponse
    {
        public string data { get; set; }
        public HttpStatusCode code { get; set; }
    }
}
