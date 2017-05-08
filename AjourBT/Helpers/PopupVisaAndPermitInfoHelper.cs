using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class PopupVisaAndPermitInfoHelper
    {
        public static MvcHtmlString CustomPopupVisaAndPermit(this HtmlHelper helper, Employee employee)
        {
            if (employee == null)
            {
                return new MvcHtmlString("");
            }

            StringBuilder builder = new StringBuilder();

            if (employee.Passport == null)
            {
                builder.Append("No Passport");
                builder.AppendLine();
            }
            
            if(employee.Visa != null)
            {
                builder.Append(String.Format("V: {0} - {1}", employee.Visa.StartDate.ToShortDateString(), employee.Visa.DueDate.ToShortDateString() ));
            }
            else
            {
                builder.Append("No Visa");
            }

            if (employee.VisaRegistrationDate != null && employee.VisaRegistrationDate.RegistrationDate != null && employee.VisaRegistrationDate.RegistrationDate.HasValue)
            {  
                builder.AppendLine();
                builder.Append(String.Format("Registration Date: {0}", employee.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString() ));
            }

            if (employee.Permit != null)
            {
                
                if(employee.Permit.Number != null && employee.Permit.StartDate != null && employee.Permit.EndDate != null)
                {
                    builder.AppendLine();
                    builder.Append(String.Format("P: {0} - {1}", employee.Permit.StartDate.Value.ToShortDateString(), employee.Permit.EndDate.Value.ToShortDateString() ));
                }

                if (employee.Permit.IsKartaPolaka)
                {
                    builder.AppendLine();
                    builder.Append(String.Format("Karta Polaka"));
                }
            }
            else
            {
                builder.AppendLine();
                builder.Append(String.Format("No Permit"));                
            }
          
            return new MvcHtmlString(builder.ToString());
        }
    }
}