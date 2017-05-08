using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayPassportDataForEMP
    {
        public static MvcHtmlString CustomDisplayPassportDataForEMP(this HtmlHelper helper, Passport passport)
        {
            StringBuilder builder = new StringBuilder("");

            if (passport != null)
            {

                if (passport.EndDate.HasValue)
                {
                    string endDate = passport.EndDate.Value.ToString(String.Format("dd'.'MM'.'yyyy"));
                    builder.AppendFormat("valid till {0}", endDate);
                }
                else
                {
                    builder.Append("valid");
                }

            }
            else
            {
                builder.Append("<b><font color=\"Red\">non-available</font></b>");
            }

            return new MvcHtmlString(builder.ToString());
        }

    }
}