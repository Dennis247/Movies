using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Movies.web.Models
{
    public class ModelResponse<T>
    {
        public T Payload { get; set; }
        public bool IsSucessFull { get; set; }
        public string Message { get; set; }
        public HttpStatusCode ResponseCode { get; set; }

    }
}
