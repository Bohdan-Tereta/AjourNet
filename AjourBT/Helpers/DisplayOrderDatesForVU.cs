using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayOrderDatesForVU
    {
        public static MvcHtmlString CustomDisplayOrderDatesForVU(this HtmlHelper helper, BusinessTrip bt)
        {

            if (bt != null)
            {
                return new MvcHtmlString(String.Format("Order: {0:dd'.'MM'.'yyyy} - {1:dd'.'MM'.'yyyy}", bt.OrderStartDate, bt.OrderEndDate)); 
            }

            return new MvcHtmlString("");
        }
    }
}