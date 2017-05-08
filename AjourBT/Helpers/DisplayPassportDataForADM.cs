using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayPassportDataForADM
    {
        public static MvcHtmlString CustomDisplayPassportDataForADM(this HtmlHelper helper, Passport passport) 
        {
            StringBuilder builder = new StringBuilder("");

            if (passport != null)
            {

                    if (passport.EndDate.HasValue)
                    {
                        string endDate = passport.EndDate.Value.ToString(String.Format("dd'.'MM'.'yyyy"));                       
                        builder.AppendFormat("till {0}", endDate);
                    }
                    else
                    {
                        builder.Append("yes");
                    }             

            }
            else
            {
                builder.Append("<b><font color=\"Red\">no</font></b>");
            }

            return new MvcHtmlString(builder.ToString());
        }
    }
}