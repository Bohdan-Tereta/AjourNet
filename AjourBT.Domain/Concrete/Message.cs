using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Abstract;
using System.Web.Configuration;

namespace AjourBT.Domain.Concrete
{
    public class Message : IMessage
    {

        public Message()
        {

        }

        public Message(MessageType messageType, List<BusinessTrip> businessTripList, Employee author, Employee employee = null)
        {
            this.MessageID = 0;
            this.messageType = messageType;
            this.BTList = businessTripList;
            this.Author = author;
            this.employee = employee;
            if (employee != null && employee.EmployeeID != 0)
                this.FullName = employee.FirstName + " " + employee.LastName;
            if (author != null)
                ReplyTo = author.EID + WebConfigurationManager.AppSettings["MailAlias"]; 
            TimeStamp = DateTime.Now.ToLocalTimeAzure();
            Role = GetRole();
            Subject = GetSubject();
            Body = GetBody();
            Link = GetLink();
        }

        public Message( string body, string author, Employee recipient, string subject)
        {
            this.MessageID = 0;
            this.messageType = MessageType.Greeting;
            this.BTList = null;
            this.Author = null;
            this.employee = recipient;
            if (recipient != null && recipient.EmployeeID != 0)
                this.FullName = recipient.FirstName + " " + recipient.LastName;
                ReplyTo = author;
            TimeStamp = DateTime.Now.ToLocalTimeAzure();
            Role = "";
            if(subject!=null && subject!=String.Empty)
            Subject = subject.Insert(subject.Length-1, ", "+employee.FirstName);

            string eid = employee == null ? "" : employee.EID;
            Body = WebConfigurationManager.AppSettings["GreetingsHeader"] + "<br/><b>" + getUkName(recipient) + "</b><br/><br/>" + WebConfigurationManager.AppSettings["GreetingsPhoto"].Replace("*", eid) + " " + WebConfigurationManager.AppSettings["GreetingsProfile"].Replace("*", eid) + "<br/><br/>" + body + "<br/><br/>" + WebConfigurationManager.AppSettings["GreetingsFooter"];
            Link = "";
        }

        public string getUkName(Employee recipient)
        {
            string ukName = "";
            if (recipient != null && recipient.FullNameUk != null)
            {
                string[] names = recipient.FullNameUk.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < Math.Min(names.Length, 2); i++)
                {
                    ukName += names[i];
                    if (i < Math.Min(names.Length, 2) - 1)
                        ukName += " ";
                }
            }
            return ukName;
        }

        public Message(string subject, string body, Employee recipient)
        {
            this.MessageID = 0;
            this.messageType = MessageType.ResetPassword;
            this.BTList = null;
            this.Author = null;
            this.employee = recipient;
            if (recipient != null && recipient.EmployeeID != 0)
                this.FullName = recipient.FirstName + " " + recipient.LastName;
            ReplyTo = WebConfigurationManager.AppSettings["SystemNetMailReplyToDefaultValue"]; 
            TimeStamp = DateTime.Now.ToLocalTimeAzure();
            Role = "";
                Subject = subject;
                Body = body; 
            Link = "";
        }

        public Message(string author, Employee recipient, string subject)
        {
            this.MessageID = 0;
            this.messageType = MessageType.VisaExpirationWarning;
            this.BTList = null;
            this.Author = null;
            this.employee = recipient;
            if (recipient != null && recipient.EmployeeID != 0)
                this.FullName = recipient.FirstName + " " + recipient.LastName;
            ReplyTo = author;
            TimeStamp = DateTime.Now.ToLocalTimeAzure();
            Role = "";
            if (subject != null && subject != String.Empty)
                Subject = subject;

            Body = recipient.FirstName + " " + recipient.LastName +", " + "your visa will expire after 90 days";
            Link = "";
        }

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

        public string GetSubject()
        {
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                    return "For BTM: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                    return "For DIR: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                    return "For EMP: BT Confirmation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                    return "For ACC: BT Confirmation";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                    return "For BTM: BT Registration";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                    return "For EMP: BT Registration";
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                    return "For ACC: BT Registration";
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                    return "For BTM: BT Replanning";
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Replanning";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                    return "For DIR: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Cancellation";
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Update";
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Update";
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Report";
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Report";
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                    return "For EMP: BT Rejection";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.ACCCancelsConfirmedReportedToADM:
                    return "For ADM: BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                    return "For BTM: BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToEMP:
                    return "For EMP: BT Cancellation";
                case MessageType.ACCModifiesConfirmedReportedToADM:
                    return "For ADM: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                    return "For BTM: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    return "For DIR: BT Update";
                case MessageType.ACCModifiesConfirmedReportedToEMP:
                    return "For EMP: BT Update";
                case MessageType.DIRRejectsConfirmedToADM:
                    return "For ADM: BT Rejection";
                case MessageType.DIRRejectsConfirmedToEMP:
                    return "For EMP: BT Rejection";
                case MessageType.DIRRejectsConfirmedToBTM:
                    return "For BTM: BT Rejection";
                case MessageType.DIRRejectsConfirmedToACC:
                    return "For ACC: BT Rejection";
                case MessageType.BTMCancelsPermitToADM:
                    return "For ADM: Permit Cancellation";
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                    return "For BTM: Planned Modified BT Cancellation";
                case MessageType.ADMCancelsPlannedModifiedToACC:
                    return "For ACC: Planned Modified BT Cancellation";
                case MessageType.ADMConfirmsPlannedOrRegisteredToResponsible:
                    return "BT Confirmation";
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible:
                    return "BT Cancellation";
                case MessageType.ACCCancelsConfirmedReportedToResponsible:
                    return "BT Cancellation";
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible:
                    return "BT Rejection";
                case MessageType.DIRRejectsConfirmedToResponsible:
                    return "BT Rejection";
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible:
                    return "BT Update";
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible:
                    return "BT Report";
                case MessageType.ACCModifiesConfirmedReportedToResponsible:
                    return "BT Update";
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                case MessageType.BTMCreateVisaRegistrationDateToBTM:
                    return "Visa Registration Date Creation";
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                case MessageType.BTMUpdateVisaRegistrationDateToBTM:
                    return "Visa Registration Date Update"; 
                case  MessageType.PUEditsFInishedBT:
                    return "Finished BT Modification"; 
                default:
                    return "Unknown Subject";
            }
        }

        public string GetRole()
        {
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                case MessageType.DIRRejectsConfirmedToBTM:
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                case MessageType.BTMUpdateVisaRegistrationDateToBTM:
                case MessageType.BTMCreateVisaRegistrationDateToBTM:
                    return "BTM";

                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    return "DIR";

                case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                case MessageType.ACCCancelsConfirmedReportedToEMP:
                case MessageType.ACCModifiesConfirmedReportedToEMP:
                case MessageType.DIRRejectsConfirmedToEMP:
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                    return "EMP";

                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                case MessageType.DIRRejectsConfirmedToACC:
                case MessageType.ADMCancelsPlannedModifiedToACC:
                    return "ACC";

                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                case MessageType.ACCCancelsConfirmedReportedToADM:
                case MessageType.ACCModifiesConfirmedReportedToADM:
                case MessageType.DIRRejectsConfirmedToADM:
                case MessageType.BTMCancelsPermitToADM:
                    return "ADM";
                
                default:
                    return "Unknown Role";
            }
        }

        public string GetLink()
        {
            //return HtmlHelper.GenerateLink(HttpContext.Current.Request.RequestContext, System.Web.Routing.RouteTable.Routes, "Goto Ajour page", "Default", "GetBusinessTripDataBTM", "BusinessTrip", null, null); 
            //return UrlHelper.GenerateUrl("Default","GetBusinessTripDataADM","BusinessTrip",null, RouteTable.Routes, HttpContext.Current.Request.RequestContext,true);
            StringBuilder hyperLink = new StringBuilder();
            string urlLeftPart = "";
            try
            {
                urlLeftPart = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            }
            catch { }

            string pathAndQuery;
            switch (messageType)
            {
                case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                case MessageType.ACCCancelsConfirmedReportedToBTM:
                case MessageType.ACCModifiesConfirmedReportedToBTM:
                case MessageType.DIRRejectsConfirmedToBTM:
                case MessageType.ADMCancelsPlannedModifiedToBTM:
                case MessageType.BTMUpdateVisaRegistrationDateToBTM:
                case MessageType.BTMCreateVisaRegistrationDateToBTM:
                    pathAndQuery = "/Home/BTMView/?tab="+Tabs.BTM.BTsInProcess;
                    break;
                case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                case MessageType.ACCModifiesConfirmedReportedToDIR:
                    pathAndQuery = "/Home/DIRView/?tab="+Tabs.DIR.BusinessTrips;
                    break;
                case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                case MessageType.ADMConfirmsPlannedOrRegisteredToResponsible:
                case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible:
                case MessageType.ACCCancelsConfirmedReportedToResponsible:
                case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible: 
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible: 
                case MessageType.ACCModifiesConfirmedReportedToResponsible: 
                    pathAndQuery = "/Home/VUView/?tab="+Tabs.VU.BTsInPreparationProcess;
                    break;
                
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    pathAndQuery = "/Home/ACCView/?tab="+Tabs.ACC.CurrentAndFutureBTs;
                    break;

                case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                case MessageType.ACCCancelsConfirmedReportedToADM:
                case MessageType.ACCModifiesConfirmedReportedToADM:
                case MessageType.DIRRejectsConfirmedToADM:
                    pathAndQuery = "/Home/ADMView/?tab="+Tabs.ADM.BTs;
                    break;
               
                case MessageType.BTMCancelsPermitToADM:
                    pathAndQuery = "/Home/ADMView/?tab="+Tabs.ADM.VisasAndPermits;
                    break;
               
                case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP:
                    pathAndQuery = "/Home/EMPView/?tab="+Tabs.EMP.YourBTs;
                    break; 
                case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                case MessageType.BTMCreateVisaRegistrationDateToEMP:
                    pathAndQuery = "/Home/EMPView/?tab="+Tabs.EMP.Visa;
                    break;
 
                default:
                    pathAndQuery = "";
                    break;
            }
            if (pathAndQuery != "")
            {
                hyperLink.AppendFormat("<a href=\"{0}{1}\"> Goto Ajour page </a>", urlLeftPart, pathAndQuery);
            }
            return hyperLink.ToString();
        }

        public string GetBTTemplate(BusinessTrip businessTrip)
        {
            StringBuilder BTTemplate = new StringBuilder();
            if (businessTrip != null)
            {
                if (businessTrip.BTof != null && businessTrip.BTof.LastName != null)
                    BTTemplate.AppendFormat("<b>{0}</b>", businessTrip.BTof.LastName);
                if (businessTrip.BTof != null && businessTrip.BTof.FirstName != null)
                    BTTemplate.AppendFormat(" <b>{0}</b>", businessTrip.BTof.FirstName);
                if (businessTrip.BTof != null && businessTrip.BTof.EID != null)
                    BTTemplate.AppendFormat(" (<b>{0}</b>)", businessTrip.BTof.EID);
                if (businessTrip.BTof != null && businessTrip.BTof.Department != null &&
                        businessTrip.BTof.Department.DepartmentName != null
                    )
                    BTTemplate.AppendFormat(", {0}", businessTrip.BTof.Department.DepartmentName);
                if (businessTrip.Manager != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Manager);
                if (businessTrip.Responsible != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Responsible);
                if (businessTrip.Purpose != null)
                    BTTemplate.AppendFormat(", {0}", businessTrip.Purpose);
                if (businessTrip.Location != null && businessTrip.Location.Title != null)
                    BTTemplate.AppendFormat(", <b>{0}</b>", businessTrip.Location.Title);
                BTTemplate.AppendFormat("<b>, {0:dd'.'MM'.'yyyy} - </b>", businessTrip.StartDate);
                BTTemplate.AppendFormat("<b>{0:dd'.'MM'.'yyyy}</b>", businessTrip.EndDate);
                if (businessTrip.OrderStartDate != null && businessTrip.OrderEndDate != null)
                {
                    BTTemplate.Append(", Order dates: ");
                    BTTemplate.AppendFormat("{0:dd'.'MM'.'yyyy} - ", businessTrip.OrderStartDate);
                    BTTemplate.AppendFormat("{0:dd'.'MM'.'yyyy}", businessTrip.OrderEndDate);
                }
                if (businessTrip.Habitation != null)
                {
                    BTTemplate.Append("<br/>");
                    string habitationConfirmed = "(not confirmed)";
                    if (businessTrip.HabitationConfirmed == true)
                        habitationConfirmed = "(confirmed)";
                    BTTemplate.AppendFormat("<b>Habitation {1}:</b> {0}", businessTrip.Habitation, habitationConfirmed);
                }
                if (businessTrip.Flights != null)
                {
                    BTTemplate.Append("<br/>");
                    string flightsConfirmed = "(not confirmed)";
                    if (businessTrip.FlightsConfirmed == true)
                        flightsConfirmed = "(confirmed)";
                    BTTemplate.AppendFormat("<b>Flights {1}:</b> {0}", businessTrip.Flights, flightsConfirmed);
                }
                if (businessTrip.Invitation == true)
                {
                    BTTemplate.Append("<br/><b>Invitation:</b> confirmed");
                }
                if (businessTrip.Comment != null && businessTrip.Comment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Comment:</b> {0}", businessTrip.Comment);
                if (businessTrip.BTMComment != null && businessTrip.BTMComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>BTM comment:</b> {0}", businessTrip.BTMComment);
                if (businessTrip.RejectComment != null && businessTrip.RejectComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Reject comment:</b> {0}", businessTrip.RejectComment);
                if (businessTrip.CancelComment != null && businessTrip.CancelComment != String.Empty)
                    BTTemplate.AppendFormat("<br/><b>Cancel comment:</b> {0}", businessTrip.CancelComment);
            }
            return BTTemplate.ToString().Trim(new char[] { ' ', ',' });
        }

        public string GetMessageTemplate()
        {
            if (Author != null)
            {
                switch (messageType)
                {
                    case MessageType.ADMConfirmsPlannedOrRegisteredToBTM:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToDIR:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToEMP:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToACC:
                    case MessageType.ADMConfirmsPlannedOrRegisteredToResponsible:
                        return string.Format("<b>BT confirmation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM:
                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP:
                    case MessageType.ADMRegistersPlannedOrPlannedModifiedToACC:
                        return string.Format("<b>BT registration</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM:
                    case MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT replanning</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM:
                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP:
                    case MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));


                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC:
                    case MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible:
                        return string.Format("<b>BT(s) cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC:
                    case MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible:
                        return string.Format("<b>BT update</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC:
                    case MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP: 
                    case MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible: 
                        return string.Format("<b>BT(s) report</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM:
                    case MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC:
                        return string.Format("<b>BT rejection</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM:
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP:
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC:
                    case MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible:
                        return string.Format("<b>BT rejection</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ACCCancelsConfirmedReportedToADM:
                    case MessageType.ACCCancelsConfirmedReportedToBTM:
                    case MessageType.ACCCancelsConfirmedReportedToEMP:
                        case MessageType.ACCCancelsConfirmedReportedToResponsible:
                        return string.Format("<b>BT cancellation</b> by ACC {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.ACCModifiesConfirmedReportedToBTM:
                        return string.Format("<b>BT modification</b> by ACC {0} {1} at {2}</br><b>Please check Order Dates!</b></br>If BT is composed of several parts to different locations their <b>Order Dates should be the same.</b>", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                    case MessageType.ACCModifiesConfirmedReportedToADM:
                    case MessageType.ACCModifiesConfirmedReportedToDIR:
                    case MessageType.ACCModifiesConfirmedReportedToEMP: 
                    case MessageType.ACCModifiesConfirmedReportedToResponsible:
                        return string.Format("<b>BT modification</b> by ACC {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.DIRRejectsConfirmedToADM:
                    case MessageType.DIRRejectsConfirmedToEMP:
                    case MessageType.DIRRejectsConfirmedToBTM:
                    case MessageType.DIRRejectsConfirmedToACC:
                    case MessageType.DIRRejectsConfirmedToResponsible:
                        return string.Format("<b>BT rejection</b> by DIR {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMCancelsPermitToADM:
                        return string.Format("<b>Permit cancellation</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Cancel for permit of {0} {1} ({2}) with dates {3:dd.MM.yyyy} - {4:dd.MM.yyyy} requested at {5:dd.MM.yyyy}", employee.FirstName, employee.LastName, employee.EID,
                            employee.Permit.StartDate, employee.Permit.EndDate, employee.Permit.CancelRequestDate);

                    case MessageType.ADMCancelsPlannedModifiedToBTM:
                    case MessageType.ADMCancelsPlannedModifiedToACC:
                        return string.Format("<b>Planned modified BT cancellation</b> by ADM {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));

                    case MessageType.BTMCreateVisaRegistrationDateToEMP:
                    case MessageType.BTMCreateVisaRegistrationDateToBTM:
                        return string.Format("<b>Visa Registration Date Creation</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName,
                            TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}</br></br>", employee.VisaRegistrationDate.VisaType,
                            employee.VisaRegistrationDate.RegistrationDate, employee.VisaRegistrationDate.RegistrationTime,
                            employee.VisaRegistrationDate.City, employee.VisaRegistrationDate.RegistrationNumber) + ReplaceURLWithHyperlink(WebConfigurationManager.AppSettings["VisaRegInfo"]);
                  
                    case MessageType.BTMUpdateVisaRegistrationDateToEMP:
                    case MessageType.BTMUpdateVisaRegistrationDateToBTM:
                        return string.Format("<b>Visa Registration Date Update</b> by BTM {0} {1} at {2}", Author.FirstName, Author.LastName,
                            TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" +
                            string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}</br></br>", employee.VisaRegistrationDate.VisaType,
                            employee.VisaRegistrationDate.RegistrationDate, employee.VisaRegistrationDate.RegistrationTime,
                            employee.VisaRegistrationDate.City, employee.VisaRegistrationDate.RegistrationNumber) + ReplaceURLWithHyperlink(WebConfigurationManager.AppSettings["VisaRegInfo"]); 
                    case MessageType.PUEditsFInishedBT:
                        return string.Format("<b>Finished BT Modification</b> by PU {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")); 
                    default:
                        return string.Format("Unknown Message Type by {0} {1} at {2}", Author.FirstName, Author.LastName, TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"));
                }
            }
            return "";
        }

        public string GetBody()
        {
            StringBuilder messageBody = new StringBuilder(GetMessageTemplate());
            if (BTList != null)
            {
                messageBody.Append("<br/><br/>");
                foreach (BusinessTrip businessTrip in BTList)
                {
                    messageBody.Append(GetBTTemplate(businessTrip));
                    messageBody.Append("<br/><br/>");
                }
            }
            return messageBody.ToString();
        }

        public string ReplaceURLWithHyperlink(string inputString)
        {
            string outputString; 
            outputString = inputString.Replace("http", "<a href=\"http").Replace(".html", ".html\">link</a>");
            return outputString;
        }

    }
}