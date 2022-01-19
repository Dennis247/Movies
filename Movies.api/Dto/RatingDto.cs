using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.api.Dto
{
    public class RatingDto
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Value { get; set; }
    }
}
