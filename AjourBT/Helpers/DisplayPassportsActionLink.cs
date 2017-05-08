using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayPassportsActionLink
    {
        public static MvcHtmlString CustomDisplayPassportsActionLink(this HtmlHelper helper, Passport passport, string searchString = "")
        {
            if (passport != null)
            {
                if (passport.EndDate.HasValue)
                {
                    return new MvcHtmlString(String.Format("<a class=\"passportEditDialog\" id=\"passportEdit\" data-date-format=\"dd.mm.yy\" href=\"{0}{1}?searchString={2}\">{3:dd'.'MM'.'yyyy}</a>", "/BTM/PassportEditDate/", passport.EmployeeID, searchString, passport.EndDate));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<a id=\"AddDatePassport\" data-date-format=\"dd.mm.yy\" href=\"{0}{1}?searchString={2}\">Date</a>", "/BTM/PassportAddDate/", passport.EmployeeID, searchString));
                }               
            }

            return new MvcHtmlString("");
        }
    }
}