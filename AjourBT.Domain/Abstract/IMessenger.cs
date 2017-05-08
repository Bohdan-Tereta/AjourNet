using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Abstract
{
    public interface IMessenger
    {
        void Notify(IMessage message);
        void StoreMessage(IMessage message);
        string[] GetMailingListForRole(IMessage message);
        void SendToMailingList(string[] mailingList, string [] blindMailingList, IMessage message);
        List<IMessage> GetGreetingMessages(DateTime date);
        string GetRandomGreetingBody(); 
        void SendGreetingMessages(DateTime date);
        void SendVisaWarningMessage(DateTime date);
    }
}
