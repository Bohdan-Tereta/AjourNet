using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayOrderDataForView
    {
        public static MvcHtmlString CustomDisplayOrderStartDate(this HtmlHelper helper, string orderStartDate)
        {

            if (orderStartDate == null)
            {
                return new MvcHtmlString("");
            }
            else
            {
                return new MvcHtmlString(String.Format("<b>Order From</b> {0}", orderStartDate));
            }
        }

        public static MvcHtmlString CustomDisplayOrderEndDate(this HtmlHelper helper, string orderEndDate)
        {

            if (orderEndDate == null)
            {
                return new MvcHtmlString("");
            }
            else
            {
                return new MvcHtmlString(String.Format("<b>Order To &nbsp &nbsp  </b>{0}", orderEndDate));
            }
        }

        public static MvcHtmlString CustomDisplayOrderDays(this HtmlHelper helper, int? orderDays)
        {

            if (orderDays == null)
            {
                return new MvcHtmlString("");
            }
            else
            {
                return new MvcHtmlString(String.Format("<b>Number Of Days</b> {0}", orderDays));
            }
        }

    }
}