using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.ViewModels
{
    public class AbsenceViewModel
    {
        public AbsenceViewModel()
        {
            Journeys = new List<CalendarItem>();
            BusinessTrips = new List<CalendarItem>();
            Overtimes = new List<CalendarItem>();
            Sickness = new List<CalendarItem>();
            Vacations = new List<CalendarItem>();
            FirstName = ""; 
            LastName = "";
            EID = "";
            Department = "";
        }

        [HiddenInput(DisplayValue = false)]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Field First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Field Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Field EID is required")]
        public string EID { get; set; }

        public string Department { get; set; }

        public List<CalendarItem> Journeys { get; set; }
        public List<CalendarItem> BusinessTrips { get; set; }
        public List<CalendarItem> Overtimes { get; set; }
        public List<CalendarItem> Sickness { get; set; }
        public List<CalendarItem> Vacations { get; set; }

        [Display(Name = "Date Dismissed")]
        public string DateDismissed { get; set; }

        public string FullName 
        { 
            get { return (this.LastName.Trim() + " " + this.FirstName.Trim()).Trim(); } 
        }

        public bool ContainsCalendarItems
        {
            get { return Journeys.Count != 0 || BusinessTrips.Count != 0 || Overtimes.Count != 0 || Sickness.Count != 0 || Vacations.Count != 0; }
        }

        public List<string> PrepareToXLSExportABM()
        {
            List<string> stringifiedProperties = new List<string>(); 

            if (this.ContainsCalendarItems)
            {
                stringifiedProperties.Add(this.Department);
                stringifiedProperties.Add(this.FullName);
                stringifiedProperties.Add(this.EID);
                stringifiedProperties.Add(StringifyCalendarItems(this.Journeys));
                stringifiedProperties.Add(StringifyCalendarItems(this.BusinessTrips));
                stringifiedProperties.Add(StringifyCalendarItems(this.Overtimes));
                stringifiedProperties.Add(StringifyCalendarItems(this.Sickness));
                stringifiedProperties.Add(StringifyCalendarItems(this.Vacations));
            }

            return stringifiedProperties; 
        }

        public static string StringifyCalendarItems(List<CalendarItem> calendarItems)
        {
            StringBuilder builder = new StringBuilder(); 
            if(calendarItems.Count!=0)
            { 
                AppendStringifiedCalendarItem(builder, calendarItems[0]);
                for (int index = 1; index < calendarItems.AsEnumerable().Count(); index++)
                {
                    builder.Append("\r\n");
                    AppendStringifiedCalendarItem(builder, calendarItems[index]);
                }
            }
            return builder.ToString();
        }

        public static void AppendStringifiedCalendarItem(StringBuilder sb, CalendarItem calendarItem)
        {
            sb.Append(calendarItem.From.Date.ToString(String.Format("dd.MM.yyyy")));
            sb.Append(" - ");
            sb.Append(calendarItem.To.Date.ToString(String.Format("dd.MM.yyyy")));
            if (calendarItem.Type == CalendarItemType.BT)
            {
                AppendLocationData(sb, calendarItem);
            }
        }

        public static void AppendLocationData(StringBuilder sb, CalendarItem calendarItem)
        {
            sb.Append(" (");
            sb.Append(String.Format(calendarItem.Location));
            sb.Append(")");
        } 
    }
}