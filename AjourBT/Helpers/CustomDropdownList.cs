using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class CustomDropdownList
    {
        public static MvcHtmlString CreateCustomDropdownList(this HtmlHelper helper, List<SelectListItem> dropdownItems, string componentId, string defaultOption = "")
        {
                StringBuilder sb = new StringBuilder();

                sb.Append(String.Format("<select name={0}>", componentId));
                sb.Append(String.Format("<option value={0}>{1}</option>", 0, defaultOption));

                foreach (SelectListItem item in dropdownItems)
                {
                    if (item.Selected)
                    {
                        sb.Append(String.Format("<option value={0} selected=\"\">{1}</option>", item.Value, item.Text));
                    }
                    else
                    {
                        sb.Append(String.Format("<option value={0}>{1}</option>", item.Value, item.Text));
                    }
                }
                sb.Append("</select>");

                return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString CreateDefaultCustomDropdownList(this HtmlHelper helper, List<SelectListItem> dropdownItems, string componentId, string defaultOption = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("<select name={0}>", componentId));
            sb.Append(String.Format("<option value={0} selected=\"\">{1}</option>", 0, defaultOption));

            foreach (SelectListItem item in dropdownItems)
            {
                    sb.Append(String.Format("<option value={0}>{1}</option>", item.Value, item.Text));
            }
            sb.Append("</select>");

            return new MvcHtmlString(sb.ToString());
        }

    }
}
