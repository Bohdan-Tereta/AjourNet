using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayDaysInBTHelper
    {
        public static MvcHtmlString CustomDisplayNumberOfDaysInBT(this HtmlHelper helper, EmployeeViewModelForVU employee)
        {
            if (employee != null)
            {
                if (employee.DaysUsedInBt != 0)
                {
                    return new MvcHtmlString(String.Format("{0} days in BT", employee.DaysUsedInBt.ToString()));
                }
                else
                {
                    return new MvcHtmlString("No BT during this period");
                }
                 
            }
            return new MvcHtmlString("");
        }
    }
}