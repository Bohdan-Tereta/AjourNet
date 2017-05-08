using AjourAPIServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Message
    {
        [NotMapped]
        public MessageType messageType { get; set; }
        [NotMapped]
        public List<BusinessTrip> BTList { get; set; }
        [NotMapped]
        public Employee Author { get; set; }
        [NotMapped]
        public Employee employee { get; set; }

        public int MessageID { get; set; }
        public string Role { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ReplyTo { get; set; }
        public string FullName { get; set; }
    }
}