using AjourBT.Domain.Abstract;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Domino;
using System.Web.Configuration;
using AjourBT.Domain.Entities;
using System.Text.RegularExpressions;
using AjourBT.Domain.Infrastructure;
using System.Data.Objects.SqlClient;

namespace AjourBT.Domain.Concrete
{
    public class Messenger : IMessenger
    {
        Random random = new Random();
        IRepository repository;

        public Messenger(IRepository repository)
        {
            this.repository = repository;

        }

        public void Notify(IMessage message)
        {
            if (message.messageType == MessageType.BTMUpdateVisaRegistrationDateToBTM || message.messageType == MessageType.BTMCreateVisaRegistrationDateToBTM)
            {
                StoreMessage(message);
            }
            else
            {
                StoreMessage(message);
                SendToMailingList(GetMailingListForRole(message), GetBlindCopyMailingList(message), message);
            }
        }


        public void StoreMessage(IMessage message)
        {
                repository.SaveMessage(message);
        }

        public string[] GetMailingListForRole(IMessage message)
        {
            List<string> mailingList = new List<string>();
            if (message != null && message.messageType!= MessageType.PUEditsFInishedBT)
            {

                if (message.messageType == MessageType.VisaExpirationWarning)
                {
                    if (message.employee.IsManager == true)
                    {
                        mailingList.Add(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                    }
                    else
                    {
                        mailingList.Add(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);

                        int empDepartment = message.employee.Department.DepartmentID;
                        Employee manager = (from emp in repository.Employees
                                          where emp.Department.DepartmentID == empDepartment && emp.IsManager == true
                                          select emp).FirstOrDefault(); // ???

                        mailingList.Add(manager.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                    }

                    return mailingList.ToArray<string>();
                }

                if (message.messageType != MessageType.Greeting && message.messageType != MessageType.ResetPassword && message.messageType != MessageType.VisaExpirationWarning)
                {
                    switch (message.Role)
                    {
                        case "EMP":
                            if (message.employee != null)
                            {
                                mailingList.Add(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                                break;
                            }

                            foreach (BusinessTrip bt in message.BTList)
                            {
                                if (!mailingList.Contains(bt.BTof.EID + WebConfigurationManager.AppSettings["MailAlias"]))
                                    mailingList.Add(bt.BTof.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                            }
                            break;
                        case "ADM":
                            if (message.employee != null)
                            {
                                Department depNum = (from dep in repository.Departments
                                                     where dep.DepartmentID == message.employee.DepartmentID
                                                     select dep).FirstOrDefault();
                                foreach (Employee emp in depNum.Employees)
                                {
                                    if (System.Web.Security.Roles.IsUserInRole(emp.EID, "ADM"))
                                        mailingList.Add(emp.EID);
                                }
                                break;
                            }

                            foreach (BusinessTrip bt in message.BTList)
                            {
                                if (!mailingList.Contains(bt.LastCRUDedBy + WebConfigurationManager.AppSettings["MailAlias"]))
                                    mailingList.Add(bt.LastCRUDedBy + WebConfigurationManager.AppSettings["MailAlias"]);
                            }
                            break;
                        case "Unknown Role":
                            {
                                Regex regex = new Regex(@"^[\w]+$", RegexOptions.IgnoreCase);
                                List<string> userIDs = new List<string>();
                                if (message.BTList != null && message.BTList.Count != 0)
                                {
                                    foreach (BusinessTrip bt in message.BTList)
                                    {
                                        if (bt.Location.ResponsibleForLoc != null)
                                        {
                                            userIDs = userIDs.Concat(Regex.Split(bt.Location.ResponsibleForLoc, @"\W+").ToList()).ToList<string>();
                                        }
                                        if (bt.Responsible != null && bt.Responsible != String.Empty)
                                        {

                                            Match match = regex.Match(bt.Responsible.Trim());
                                            if (match.Success)
                                            {
                                                userIDs.Add(bt.Responsible.Trim());
                                            }
                                        }
                                        foreach (string userID in userIDs)
                                        {
                                            if (!mailingList.Contains(userID + WebConfigurationManager.AppSettings["MailAlias"]))
                                                mailingList.Add(userID + WebConfigurationManager.AppSettings["MailAlias"]);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            mailingList = System.Web.Security.Roles.GetUsersInRole(message.Role).ToList<string>();
                            for (int i = 0; i < mailingList.Count(); i++)
                            {
                                mailingList[i] += WebConfigurationManager.AppSettings["MailAlias"];
                            }
                            break;
                    }
                }
                else
                {
                        mailingList.Add(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                }
            }
            return mailingList.ToArray<string>();
        }

        public string[] GetBlindCopyMailingList(IMessage message)
        {
            List<String> blindMialinglist = new List<string>();
            if (message != null && message.messageType == MessageType.Greeting)
            {
                foreach (Employee emp in repository.Employees.Where(e => e.DateDismissed == null))
                {
                    if (emp.IsGreetingMessageAllow)
                    {
                        blindMialinglist.Add(emp.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                    }
                }
                if (message.employee != null && message.employee.EID != null)
                    blindMialinglist.Remove(message.employee.EID + WebConfigurationManager.AppSettings["MailAlias"]);
            }
            return blindMialinglist.ToArray<string>();
        }

        public void SendToMailingList(string[] mailingList, string[] blindMailingList, IMessage message)
        {
            string sendingChoice = WebConfigurationManager.AppSettings["WayOfMessageSending"];
            switch (sendingChoice)
            {
                case "LotusNotes":
                    SendUsingLotusNotes(mailingList, blindMailingList, message);
                    break;
                case "SendGrid":
                    SendUsingSendGrid(mailingList, blindMailingList, message);
                    break;
                case "SystemNetMail":
                    SendUsingSystemNetMail(mailingList, blindMailingList, message);
                    break;
                default:
                    break;
            }
        }

        private static void SendUsingSendGrid(string[] mailingList, string[] blindMailingList, IMessage message)
        {
            //TODO: add sending logic here
            // Setup the email properties.
            var from = new MailAddress(message.ReplyTo);
            var to = new MailAddress[mailingList.Length];
            for (int i = 0; i < mailingList.Length; i++)
            {
                to[i] = new MailAddress(mailingList[i]);
            } 
            var cc = new MailAddress[] { };
            var bcc = new MailAddress[] { };
            for (int i = 0; i < blindMailingList.Length; i++)
            {
                bcc[i] = new MailAddress(blindMailingList[i]);
            }
            var subject = message.Subject;
            var html = message.Body.Replace(Environment.NewLine, "<br/>") + "<br/>" + message.Link;

            // Create an email, passing in the the eight properties as arguments.
            SendGrid myMessage = SendGrid.GetInstance(from, to, cc, bcc, subject, html, null);

            var username = WebConfigurationManager.AppSettings["SendGridLogin"];
            var pswd = WebConfigurationManager.AppSettings["SendGridPassword"];

            var credentials = new NetworkCredential(username, pswd);


            var transportSMTP = SMTP.GetInstance(credentials);


            transportSMTP.Deliver(myMessage);
        }

        private static void SendUsingLotusNotes(string[] mailingList, string[] blindMailingList, IMessage message)
        {
            string serverName = WebConfigurationManager.AppSettings["LotusNotesServerName"];
            string mailFile = WebConfigurationManager.AppSettings["LotusNotesMailFileName"];
            string password = WebConfigurationManager.AppSettings["LotusNotesPassword"];
            string[] sendTo = mailingList;
            string[] copyTo = { };
            string replyTo = message.ReplyTo;
            string[] blindCopyTo = blindMailingList;
            string subject = message.Subject;

            //Create new notes session
            NotesSession notesSession = new NotesSession();
            notesSession.Initialize(password);

            //Get and open NotesDataBase
            NotesDatabase notesDataBase = notesSession.GetDatabase(serverName, mailFile, false);
            if (!notesDataBase.IsOpen) notesDataBase.Open();

            //Create the notes document
            NotesDocument notesDocument = notesDataBase.CreateDocument();

            //Set document type
            notesDocument.ReplaceItemValue("Form", "Memo");

            //Set notes memo fields (To: CC: Bcc: Subject etc) 
            notesDocument.ReplaceItemValue("SendTo", sendTo);
            notesDocument.ReplaceItemValue("CopyTo", copyTo);
            if (blindCopyTo != null)
                notesDocument.ReplaceItemValue("BlindCopyTo", blindCopyTo);
            notesDocument.ReplaceItemValue("ReplyTo", replyTo);
            notesDocument.ReplaceItemValue("Subject", subject);

            //Set notes Body as HTML
            NotesStream notesStream = notesSession.CreateStream();
            var body = message.Body.Replace("\n", "<br/>");
            //notesStream.WriteText(message.Body);
            notesStream.WriteText(body);
            notesStream.WriteText("");
            notesStream.WriteText(message.Link);

            NotesMIMEEntity mimeItem = notesDocument.CreateMIMEEntity("Body");
            mimeItem.SetContentFromText(notesStream, "text/html; charset=UTF-8", MIME_ENCODING.ENC_NONE);

            notesDocument.Send(false);
        }

        public static void SendUsingSystemNetMail(string[] mailingList, string[] blindMailingList, IMessage message)
        {
            MailMessage msg = CreateSytemNetMailMessage(mailingList, blindMailingList, message);

            SmtpClient client = ConfigureSystemNetMailSMTPClient();

            client.Send(msg);

        }

        public static SmtpClient ConfigureSystemNetMailSMTPClient()
        {
            SmtpClient client = new SmtpClient();
            client.Host = WebConfigurationManager.AppSettings["SystemNetMailHost"];
            client.Port = Int32.Parse(WebConfigurationManager.AppSettings["SystemNetMailPort"]);
            if (WebConfigurationManager.AppSettings["SystemNetMailEnableSsl"].ToLower() == "true")
            {
                client.EnableSsl = true;
            }
            else
            {
                client.EnableSsl = false;
            }
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(
                WebConfigurationManager.AppSettings["SystemNetMailLogin"],
                WebConfigurationManager.AppSettings["SystemNetMailPassword"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;
        }

        public static MailMessage CreateSytemNetMailMessage(string[] mailingList, string[] blindMailingList, IMessage message)
        {
            MailMessage msg = new MailMessage(); 
            if(mailingList!= null)
            { 
                foreach (string mail in mailingList)
                {
                    msg.To.Add(mail);
                }
            }
            if (blindMailingList != null)
            {
                foreach (string mail in blindMailingList)
                {
                    msg.Bcc.Add(mail);
                }
            }
            if (message != null)
            {
                if(message.ReplyTo!=null)
                {
                    msg.From = new MailAddress(message.ReplyTo); 
                }
                else
                { 
                msg.From=new MailAddress(WebConfigurationManager.AppSettings["SystemNetMailReplyToDefaultValue"]);
                }
                msg.Subject = message.Subject;
                msg.Body = (message.Body??"").Replace("\n", "<br/>") + ("<br/>") + message.Link;
                msg.IsBodyHtml = true;
            }
            return msg;
        }

        public List<IMessage> GetGreetingMessages(DateTime date)
        {
            random = new Random(); 
            List<IMessage> birthdayMessages = new List<IMessage>();
            List<Employee> employeesBornToday = new List<Employee>();   //!
            //List<Employee> employeesNotGreetedToday = new List<Employee>();
            employeesBornToday = repository.Employees.Where(e => e.BirthDay != null && e.BirthDay.HasValue && e.BirthDay.Value.Day == date.Day &&
                e.BirthDay.Value.Month == date.Month && e.DateDismissed == null).ToList();
            //foreach (var emp in employeesBornToday)
            //{
                //if (repository.Messages.Where(m => (m.messageType == MessageType.Greeting && m.employee != null && m.employee.EID == emp.EID && m.TimeStamp.Year == DateTime.Now.ToLocalTimeAzure().Year)).FirstOrDefault() == null)
                    //employeesNotGreetedToday.Add(emp);
            //}
            foreach (Employee emp in employeesBornToday)
            {
                birthdayMessages.Add(new Message(GetRandomGreetingBody(), WebConfigurationManager.AppSettings["GreetingsAuthor"], emp,
                    WebConfigurationManager.AppSettings["GreetingsSubject"]));
            }
            return birthdayMessages; 
        }

        public List<IMessage> GetVisaWarningMessages(DateTime date)
        {
            List<IMessage> WarningMessages = new List<IMessage>();
            List<Employee> EmployessToBeNotify = new List<Employee>();
            List<Employee> tempEmp = new List<Employee>();
            DateTime today = DateTime.Today;
           
            //EmployessToBeNotify = (from emp in repository.Employees where
            //                           emp.Visa != null &&
            //                          SqlFunctions.DateDiff("day", today, emp.Visa.DueDate) == 90
            //                       select emp).ToList();

            tempEmp = (from emp in repository.Employees where emp.Visa != null select emp).ToList();

            foreach (Employee empl in tempEmp)
            {                
                if ((empl.Visa.DueDate - today).Days == 90)
                    EmployessToBeNotify.Add(empl);
            }

            foreach (Employee emp in EmployessToBeNotify)
            {
                WarningMessages.Add(new Message(WebConfigurationManager.AppSettings["GreetingsAuthor"], emp,
                    "Visa Expiration Warning"));
            }

            return WarningMessages;
        }

        public string GetRandomGreetingBody()
        {
            int greetingsCount = repository.Greetings.Count();
            if (greetingsCount > 0)
            {
                int randomNumber = random.Next(0, greetingsCount);

                if (repository.Greetings.Skip(randomNumber).FirstOrDefault() != null)
                    return repository.Greetings.Skip(randomNumber).FirstOrDefault().GreetingBody;
                else
                    return "";
            }
            return "";
        }

        public void SendGreetingMessages(DateTime date)
        {
            foreach (Message msg in GetGreetingMessages(date))
            {
                Notify(msg);
            }
        }

        public void SendVisaWarningMessage(DateTime date)
        {
            foreach (Message msg in GetVisaWarningMessages(date))
            {
                Notify(msg);
            }
        }
    }
}