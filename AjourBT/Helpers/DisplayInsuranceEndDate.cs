using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Helpers
{
    public static class DisplayInsuranceEndDate
    {
        public static MvcHtmlString CustomDisplayInsuranceEndDate(this HtmlHelper helper, Insurance insurance, string href)
        {
            if (insurance != null)
            {
                if (insurance.EndDate.Date > DateTime.Now.ToLocalTimeAzure().AddDays(14).Date)
                {
                    return new MvcHtmlString(String.Format("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"{0}\">{1:dd'.'MM'.'yyyy} ({2})</a>", href, insurance.EndDate, insurance.Days.ToString()));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"{0}\"style=\"color: red;\">{1:dd'.'MM'.'yyyy} ({2})</a>", href, insurance.EndDate, insurance.Days.ToString()));
                }
            }

            return new MvcHtmlString("");
        }
    }
}