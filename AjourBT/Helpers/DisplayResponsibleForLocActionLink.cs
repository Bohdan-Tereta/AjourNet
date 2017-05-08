using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class ResponsibleForLocDisplay
    {
        public static MvcHtmlString CustomResponsibleForLocActionLink(this HtmlHelper helper, Location location)
        {
            if (location != null)
            {

                if (location.ResponsibleForLoc == null || location.ResponsibleForLoc.Trim() == "")
                {
                    return new MvcHtmlString(String.Format("-"));
                }
                else
                {
                    return new MvcHtmlString(String.Format(location.ResponsibleForLoc));
                }
                //if (location.ResponsibleForLoc == null || location.ResponsibleForLoc == "")
                //{
                //    return new MvcHtmlString(String.Format("<a id=\"AddResponsibleForLoc\" href=\"{0}{1}\"><span> Add </span></a>", "/PU/LocationAddResponsibleForLoc/", location.LocationID));
                //}
                //else
                //{
                //    return new MvcHtmlString(String.Format("<a class=\"responsibleForLocEditDialog\" id=\"responsibleForLocEdit\" href=\"{0}{1}\">{2}</a>", "/PU/LocationEditResponsibleForLoc/", location.LocationID, location.ResponsibleForLoc));
                //}

            }
            return new MvcHtmlString("");

        }
    }
}