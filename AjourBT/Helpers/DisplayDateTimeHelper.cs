using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayDateTimeHelper
    {
        public static MvcHtmlString CustomDisplayDateTime(this HtmlHelper helper, DateTime date)
        {
            return new MvcHtmlString(String.Format("{0:dd'.'MM'.'yyyy}", date));
        }

        public static MvcHtmlString CustomDisplayDateTime(this HtmlHelper helper, DateTime? date)
        {
            if (date == null)
            {
                return new MvcHtmlString(String.Empty);

            }
            else
            {
                return new MvcHtmlString(String.Format("{0:dd'.'MM'.'yyyy}", date));
            }

        }
    }
}
