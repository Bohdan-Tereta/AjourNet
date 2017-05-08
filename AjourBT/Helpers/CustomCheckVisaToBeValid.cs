using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Helpers
{
    public static class CustomCheckVisaToBeValid
    {
        public static bool CheckVisaToBeValid(this HtmlHelper helper, Visa visa)
        {
            if (visa != null
                && visa.StartDate <= DateTime.Now.ToLocalTimeAzure().Date
                && visa.DueDate >= DateTime.Now.ToLocalTimeAzure().Date)
            {
                return true;
            }

            return false;
        }

        public static MvcHtmlString CheckVisaToBeValid(this HtmlHelper helper, Employee employee, string searchString = "")
        {
            if (employee.Visa == null)
            {
                return new MvcHtmlString(String.Format("<div class=\"littleSrift\">No Visa</div>"));
            }
            else if (employee.Visa != null
                    && employee.Visa.DueDate < DateTime.Now.ToLocalTimeAzure().Date)
            {
                return new MvcHtmlString(String.Format("<div class=\"littleSrift\">Visa Expired</div>"));
            }
            else
            {
                return new MvcHtmlString(String.Format("<a class=\"ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" id=\"AddPrivateTrip\"  data-date-format=\"dd.mm.yy\" href=\"{0}{1}?searchString={2}\"> Add </a>", "/BTM/PrivateTripCreate/", employee.EmployeeID, searchString));
            }
        }
    }
}