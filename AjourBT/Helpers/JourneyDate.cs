using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class JourneyDate
    {
        public static MvcHtmlString CustomDisplayJourneyDate(this HtmlHelper helper, Journey journey, string searchString = "")
        {
            StringBuilder hintBuilder = new StringBuilder("");
            helper.ViewBag.SearchString = searchString;
            //if (journey.DayOff == true)
            //{
                if (journey.ReclaimDate != null)
                {
                    string statusHint = CustomJourneyDateStatus(journey);
                    helper.ViewBag.SearchString = searchString;
                    return new MvcHtmlString(String.Format("<a id=journeyLink href=/Journey/EditJourney/{2}?searchString={3} <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, journey.Date,journey.JourneyID, searchString));
                }
                else
                {
                    helper.ViewBag.SearchString = searchString;
                    return new MvcHtmlString(String.Format("<a id=journeyLink href=/Journey/EditJourney/{1}?searchString={2} </a> {0:dd'.'MM'.'yyyy} ", journey.Date, journey.JourneyID, searchString));
                } 
            //}

            //return new MvcHtmlString("");
        }


        public static string CustomJourneyDateStatus(Journey journey)
        {
            return String.Format("{0:dd.MM.yyyy}", journey.ReclaimDate);
        }

        public static MvcHtmlString CustomDisplayJourneyDateForVU(this HtmlHelper helper, Journey journey, string searchString = "")
        {
            StringBuilder hintBuilder = new StringBuilder("");
            helper.ViewBag.SearchString = searchString;
            //if (journey.DayOff == true)
            //{
                if (journey.ReclaimDate != null)
                {
                    string statusHint = CustomJourneyDateStatus(journey);
                    helper.ViewBag.SearchString = searchString;
                    return new MvcHtmlString(String.Format("<a id=journeyLink <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, journey.Date));
                }
                else
                {
                    helper.ViewBag.SearchString = searchString;
                    return new MvcHtmlString(String.Format("<a id=journeyLink </a> {0:dd'.'MM'.'yyyy} ", journey.Date));
                }
            //}

            //return new MvcHtmlString("");
        }


    }
}



