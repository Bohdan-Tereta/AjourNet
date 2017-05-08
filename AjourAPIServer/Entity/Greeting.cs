using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Greeting
    {
        [Key]
        public int GreetingId { get; set; }
        [Required]
        public string GreetingHeader { get; set; }
        [Required]
        public string GreetingBody { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}