using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayNameOfEmployee
    {

        public static MvcHtmlString CustomDisplayNameOfEmployee(this HtmlHelper helper, Employee employee)
        {
            if(employee != null)
            {
                if (employee.DateDismissed != null)
                {
                    return new MvcHtmlString(String.Format("<a><strike>{0} {1}</strike> </a> <br> <span id='i2'>{2}</span>", employee.LastName, employee.FirstName, employee.DateDismissed.Value.ToShortDateString()));
                }
                else
                {
                    if(employee.IsManager == true)
                    {
                        return new MvcHtmlString(String.Format("<a><b>{0} {1}</b></a>", employee.LastName, employee.FirstName));
                    }
                    else
                    {
                        return new MvcHtmlString(String.Format("<a><m>{0} {1}</m></a>", employee.LastName, employee.FirstName) );
                    }
                }
            }

            return new MvcHtmlString("");
        }


        public static MvcHtmlString CustomDisplayNameOfEmployeeViewModelForVU(this HtmlHelper helper, EmployeeViewModelForVU employeeModel, MvcHtmlString daysUsedInBtHint)
        {
            if (employeeModel != null)
            {
                if (employeeModel.DateDismissed != null && employeeModel.DateDismissed != "")
                {
                    return new MvcHtmlString(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <strike>{1} {2}</strike></a> <br> <span id='i2'>{3}</span>", daysUsedInBtHint, employeeModel.LastName, employeeModel.FirstName, employeeModel.DateDismissed));
                }
                else
                {
                    if (employeeModel.IsManager == true)
                    {
                        return new MvcHtmlString(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <b>{1} {2}</b> </a>", daysUsedInBtHint, employeeModel.LastName, employeeModel.FirstName));
                    }
                    else
                    {
                        return new MvcHtmlString(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <msort>{1} {2}</msort> </a>", daysUsedInBtHint, employeeModel.LastName, employeeModel.FirstName));
                    }
                }
            }

            return new MvcHtmlString("");
        }


    }
}