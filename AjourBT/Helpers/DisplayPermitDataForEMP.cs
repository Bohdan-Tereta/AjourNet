using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayPermitDataForEMP
    {
        public static MvcHtmlString CustomDisplayPermitDataForEMP(this HtmlHelper helper, Permit permit)
        {
            StringBuilder builder = new StringBuilder("");

            if (permit == null)
            {
                builder.Append("No Permit");
            }
            else
            {

                if (permit.IsKartaPolaka)
                {
                    builder.Append("Karta Polaka <br />");
                }
                if (permit.Number != null && permit.StartDate != null && permit.EndDate != null)
                {
                    builder.AppendFormat("<customBlueItalic>Dates:</customBlueItalic> {0:dd.MM.yyyy} - {1:dd.MM.yyyy}", permit.StartDate.Value.Date, permit.EndDate.Value.Date);
                }
            }

            return new MvcHtmlString(builder.ToString());
        }

    }
}