using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayBTsDatesActionLink
    {
        public static MvcHtmlString CustomDisplayBTsDatesActionLink(this HtmlHelper helper, BusinessTrip businessTrip, string selectedDepartment = "")
        {
            string tag = "a";
            string href = "href=\"{0}{1}?selectedDepartment={2}\" ";
            if (businessTrip != null)
            {
                if (businessTrip.Status == (BTStatus.Confirmed | BTStatus.Modified))
                {
                    tag = "span";
                    href = "";
                }
                string dateFormat = MvcApplication.JSDatePattern;

                if (businessTrip.OrderEndDate.HasValue && businessTrip.OrderStartDate.HasValue)
                {
                    return new MvcHtmlString(String.Format("<" + tag + " id=\"EditReportedBTACC\" " + href + "data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </" + tag + ">", "/ACC/EditReportedBT/", businessTrip.BusinessTripID, selectedDepartment, businessTrip.StartDate.ToShortDateString(), businessTrip.EndDate.ToShortDateString(), businessTrip.OrderStartDate.Value.ToShortDateString(), businessTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, businessTrip.DaysInBtForOrder));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<" + tag + " id=\"EditReportedBTACC\" " + href + "data-date-format=\"{5}\"> {3} - {4} </" + tag + ">", "/ACC/EditReportedBT/", businessTrip.BusinessTripID, selectedDepartment, businessTrip.StartDate.ToShortDateString(), businessTrip.EndDate.ToShortDateString(), dateFormat));
                }
            }

            return new MvcHtmlString("");
        }

        public static MvcHtmlString CustomDisplayAccountableBTsDatesActionLink(this HtmlHelper helper, BusinessTrip businessTrip, string selectedDepartment = "")
        {
            if (businessTrip != null)
            {
                string dateFormat = MvcApplication.JSDatePattern;

                if (businessTrip.OrderEndDate.HasValue && businessTrip.OrderStartDate.HasValue)
                {
                    return new MvcHtmlString(String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/ShowAccountableBTData/", businessTrip.BusinessTripID, selectedDepartment, businessTrip.StartDate.ToShortDateString(), businessTrip.EndDate.ToShortDateString(), businessTrip.OrderStartDate.Value.ToShortDateString(), businessTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, businessTrip.DaysInBtForOrder));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/ACC/ShowAccountableBTData/", businessTrip.BusinessTripID, selectedDepartment, businessTrip.StartDate.ToShortDateString(), businessTrip.EndDate.ToShortDateString(), dateFormat));
                }
            }

            return new MvcHtmlString("");
        }

        public static MvcHtmlString CustomDisplayClosedBTsDatesActionLink(this HtmlHelper helper, BusinessTrip bt, string searchString = "")
        {
            string href = "href=\"{0}{1}?searchString={2}\" ";
            string dateFormat = MvcApplication.JSDatePattern;

            if(bt != null)
            {
                if (bt.OrderEndDate.HasValue && bt.OrderStartDate.HasValue)
                {
                    return new MvcHtmlString(String.Format("<a id=\"ShowClosedBTACC\" " + href + "data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/ShowClosedBT/", bt.BusinessTripID, searchString, bt.StartDate.ToShortDateString(), bt.EndDate.ToShortDateString(), bt.OrderStartDate.Value.ToShortDateString(), bt.OrderEndDate.Value.ToShortDateString(), dateFormat, bt.DaysInBtForOrder));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<a id=\"ShowClosedBTACC\" " + href + "data-date-format=\"{5}\"> {3} - {4} </a>", "/ACC/ShowClosedBT/", bt.BusinessTripID, searchString, bt.StartDate.ToShortDateString(), bt.EndDate.ToShortDateString(), dateFormat));
                }
            }

            return new MvcHtmlString("");
        }
    }
}