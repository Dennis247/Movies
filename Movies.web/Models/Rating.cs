using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.web.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public string Source { get; set; }
        public string Value { get; set; }
    }
}
