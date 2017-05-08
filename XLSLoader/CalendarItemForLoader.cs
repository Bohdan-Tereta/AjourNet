using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    class CalendarItemForLoader
    {
        public CalendarItemForLoader(CalendarItem calendarItem)
        {
            this.CalendarItemID = calendarItem.CalendarItemID;
            this.From = calendarItem.From;
            this.To = calendarItem.To;
            this.EmployeeID = calendarItem.EmployeeID;
            this.Location = calendarItem.Location;
            this.type = calendarItem.Type;
        }

        public int CalendarItemID { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int EmployeeID { get; set; }
        public string Location { get; set; }
        private CalendarItemType type;
        public CalendarItemType Type
        {
            get { return type; }
            set { type = value; }
        }


    }
}
