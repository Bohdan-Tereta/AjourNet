using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    //Don't change props names!!! Need for proper work of gantt chart
    public class CalendarItemViewModel
    {
        
        public int id { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string customClass { get; set; }
        public string desc { get; set; }
        public string label { get; set; }
        public string dataObj {get; set;}
        public string sickType { get; set; }
        public CalendarItemViewModel()
        {

        }
        public CalendarItemViewModel(CalendarItem calendarItem)
        {
            id = calendarItem.CalendarItemID;
            from = calendarItem.From;
            to = calendarItem.To;
            desc = calendarItem.Type.ToString();
            dataObj = "foo.bar[i]";

            switch (calendarItem.Type)
            {
                case CalendarItemType.BT:
                    customClass = "ganttGreen";
                    desc = "BusinessTrip";
                    break;
                case CalendarItemType.Journey:
                    customClass = "ganttDarkGreen";
                    desc = "Journey";
                    break;
                case CalendarItemType.ReclaimedOvertime:
                    customClass = "ganttOrange";
                    desc = "ReclaimedOvertime";
                    break;
                case CalendarItemType.PaidVacation:
                    customClass = "ganttBlue";
                    desc = "PaidVacation";
                    break;
                case CalendarItemType.SickAbsence:
                    customClass = "ganttViolet";
                    desc = "SickAbsence";
                    break;
                case CalendarItemType.UnpaidVacation:
                    desc = "UnpaidVacation";
                    customClass = "ganttRed";
                    break;
                case CalendarItemType.OvertimeForReclaim:
                    customClass = "ganttMagenta";
                    desc = "OvertimeForReclaim";
                    break;
                case CalendarItemType.PrivateMinus:
                    customClass = "ganttYellow";
                    desc = "PrivateMinus";
                    break;
                default:
                    customClass = "ganttWhite";
                    break;
            }
            label = "";
            sickType = "";

        }

    }



}