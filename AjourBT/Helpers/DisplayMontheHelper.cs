using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayMonthHelper
    {
        public static MvcHtmlString CustomDisplayMonthInEnglishHelp(this HtmlHelper helper, string date)
        {
            switch (date)
            {
                case "1":
                    date = "Jan";
                    break;
                case "2":
                    date = "Feb";
                    break;
                case "3":
                    date = "Mar";
                    break;
                case "4":
                    date = "Apr";
                    break;
                case "5":
                    date = "May";
                    break;
                case "6":
                    date = "Jun";
                    break;
                case "7":
                    date = "Jul";
                    break;
                case "8":
                    date = "Aug";
                    break;
                case "9":
                    date = "Sep";
                    break;
                case "10":
                    date = "Oct";
                    break;
                case "11":
                    date = "Nov";
                    break;
                case "12":
                    date = "Dec";
                    break;
                default:
                    date = "";
                    break;
            }
            return new MvcHtmlString(date);
        }
    }
}
