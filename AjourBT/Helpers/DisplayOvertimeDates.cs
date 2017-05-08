using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Entities;

namespace AjourBT.Helpers
{
    public static class DisplayOvertimeDates
    {

        public static MvcHtmlString CustomDisplayOvertimeDate(this HtmlHelper helper, Overtime overtime, string searchString = "")
        {
            if (overtime != null)
            {
                //if (overtime.DayOff == true)
                //{
                    if (overtime.ReclaimDate != null)
                    {
                        string statusHint = CustomStatusHint(overtime);
                        return MvcHtmlString.Create(String.Format("<a id=overtimeLink href=/Overtime/EditOvertime/{0}?searchString={1} <strike style=\"color:red\"><redText title=\"{2}\"> {3:dd'.'MM'.'yyyy} </redText></strike> </a>", overtime.OvertimeID, searchString, statusHint, overtime.Date));
                    }
               // }

                helper.ViewBag.SearchString = searchString;
                return MvcHtmlString.Create(String.Format("<a id=overtimeLink href=/Overtime/EditOvertime/{0}?searchString={1} </a> {2:dd'.'MM'.'yyyy}", overtime.OvertimeID, searchString, overtime.Date));
            }

            return MvcHtmlString.Create("");
        }

        public static string CustomStatusHint(Overtime overtime)
        {
            return String.Format("{0:dd.MM.yyyy}", overtime.ReclaimDate);
        }

        public static MvcHtmlString CustomDisplayAbsenceDateForEMP(this HtmlHelper helper, AbsenceFactorData absenceData)
        {
            if (absenceData != null)
            {

                if (absenceData.ReclaimDate != null)
                {
                    string statusHint = CustomStatusHintForAbsenceData(absenceData);
                    return MvcHtmlString.Create(String.Format("<a <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, absenceData.From));
                }


                return MvcHtmlString.Create(String.Format("{0:dd'.'MM'.'yyyy}", absenceData.From));
            }

            return MvcHtmlString.Create("");
        }
  

        public static string CustomStatusHintForAbsenceData(AbsenceFactorData data)
        {
            return String.Format("{0:dd.MM.yyyy}", data.ReclaimDate);
        }

        public static MvcHtmlString CustomDisplayOvertimeDateForVU(this HtmlHelper helper, Overtime overtime, string searchString = "")
        {
            if (overtime != null)
            {
                //if (overtime.DayOff == true)
                //{
                    if (overtime.ReclaimDate != null)
                    {
                        string statusHint = CustomStatusHint(overtime);
                        return MvcHtmlString.Create(String.Format("<a id=overtimeLink <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, overtime.Date));
                    }
                //}

                helper.ViewBag.SearchString = searchString;
                return MvcHtmlString.Create(String.Format("<a id=overtimeLink </a> {0:dd'.'MM'.'yyyy}", overtime.Date));
            }

            return MvcHtmlString.Create("");
        }
    }
}