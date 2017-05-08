using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayBtStyleForVU
    {

        public static string CustomBtModelStyle(this HtmlHelper helper, BusinessTripViewModel businessTripModel)
        {
            if (businessTripModel != null)
            {
                if (businessTripModel.Status == (BTStatus.Confirmed | BTStatus.Cancelled))
                {
                    return ("color: red; text-decoration: line-through;");
                }
                else if (businessTripModel.Status == (BTStatus.Confirmed | BTStatus.Modified))
                {
                    return "color: blue;";
                }
            }

            return ("");
        }

        public static string CustomBtStyle(this HtmlHelper helper, BusinessTrip businessTrip)
        {
            if (businessTrip != null)
            {
                if (businessTrip.Status == (BTStatus.Confirmed | BTStatus.Cancelled))
                {
                    return ("color: red; text-decoration: line-through;");
                }
            }

            return ("");
        }

       
    }
}