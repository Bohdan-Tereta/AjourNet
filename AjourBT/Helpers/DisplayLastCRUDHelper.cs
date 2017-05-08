using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayLastCRUDHelper
    {
        public static MvcHtmlString DisplayLastCRUD(this HtmlHelper helper, string LastCRUDedBy, DateTime LastCRUDTimestamp)
        {
            if (LastCRUDedBy != null && LastCRUDedBy != String.Empty)
            {
                return new MvcHtmlString(
                    String.Format("Last modified by {0} on {1:dd'.'MM'.'yyyy} at {1:HH':'mm':'ss}",
                        LastCRUDedBy,
                        LastCRUDTimestamp));
            }
            else
                return new MvcHtmlString("");
            //TODO: write to log in else branch or throw an exception
        }
    }
}
