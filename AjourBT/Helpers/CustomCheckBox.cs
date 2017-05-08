using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Entities;

namespace AjourBT.Helpers
{
    public static class CustomCheckBox
    {

        public static MvcHtmlString CreateCustomCheckBox(this HtmlHelper helper, string name, bool isChecked, string role)
        {
            string valueChecked = "";
            if (isChecked == true)
                valueChecked = "checked=\"checked\"";
            return MvcHtmlString.Create(String.Format("<input id=\"{0}\" type=\"checkbox\" value=\"{1}\" name=\"{0}\" {2}></input>", name, role, valueChecked));
        }

        public static MvcHtmlString CreateCustomCheckBox(this HtmlHelper helper, string name, bool isChecked, int businessTripID)
        {
            string valueChecked = "";
            if (isChecked == true)
                valueChecked = "checked=\"checked\"";
            return MvcHtmlString.Create(String.Format("<input id=\"{0}\" type=\"checkbox\" value=\"{1}\" name=\"{0}\" {2}></input>", name, businessTripID.ToString(), valueChecked));
        }

        public static MvcHtmlString CreateCustomCheckBox(this HtmlHelper helper, string name, bool isChecked, BusinessTrip bTrip)
        {
            string valueChecked = "";
            if (isChecked == true)
                valueChecked = "checked=\"checked\"";
            if ((bTrip.Manager != null && bTrip.Manager != String.Empty) &&
            (bTrip.Purpose != null && bTrip.Purpose != String.Empty) &&
            (bTrip.Responsible != null && bTrip.Responsible != String.Empty))
            {
                return MvcHtmlString.Create(String.Format("<input id=\"{0}\" type=\"checkbox\" value=\"{1}\" name=\"{0}\" {2}></input>", name, bTrip.BusinessTripID.ToString(), valueChecked));
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

    }
}
