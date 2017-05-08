using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Abstract
{
    public interface IMessage
    {
        int MessageID { get; set; }
        string Role { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        string Link { get; set; }
        DateTime TimeStamp { get; set; }
        string ReplyTo { get; set; }
        MessageType messageType { get; set; }
        List<BusinessTrip> BTList { get; set; }
        Employee employee { get; set; }
        string FullName { get; set; }
    }
}