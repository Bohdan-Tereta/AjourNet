using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayCityOfVisaRegistration
    {
        public static MvcHtmlString CustomDisplayCityOfVisaRegistration(this HtmlHelper helper, VisaRegistrationDate visaRegistrationDate)
        {
            if (visaRegistrationDate != null)
            {
                if (visaRegistrationDate.City != null && visaRegistrationDate.City != String.Empty)
                {
                    return new MvcHtmlString(String.Format("{0}", visaRegistrationDate.City));
                }
                else
                {
                    return new MvcHtmlString("City is not specified");
                }

            }
            return new MvcHtmlString("");
        }
    }
}