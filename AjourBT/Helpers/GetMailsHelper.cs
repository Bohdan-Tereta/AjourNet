using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;



namespace AjourBT.Helpers
{
    public static class GetMailsHelper
    {
        public static MvcHtmlString GetMailAliasEMailsHelper(this HtmlHelper helper, List<Employee> employees, string userName = "")
        {
            StringBuilder sb = new StringBuilder();
            if (userName != null && userName != "")
            {
                sb.Append("mailto:" + userName + WebConfigurationManager.AppSettings["MailAlias"] + "?bcc=");
            }
            else
            {
                sb.Append("<textarea rows=\"10\" cols=\"45\" name=\"text\">");
            }

            foreach (Employee emp in employees)
            {
                sb.Append(emp.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                sb.Append(", ");
            }
            if (employees.Count > 0)
            {
                RemoveEndingComa(sb);
            }
            if (userName == null || userName == "")
            sb.Append("</textarea>");
          
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GetSecondMailAliasEMailsHelper(this HtmlHelper helper, List<Employee> employees, string userName = "")
        { 
            StringBuilder builder = new StringBuilder();
            if (userName != null && userName != "")
            {
                builder.Append("mailto:" + userName + WebConfigurationManager.AppSettings["MailAlias"] + "?bcc=");
            }
            else
            {
                builder.Append("<textarea rows=\"10\" cols=\"45\" name=\"text\">");
            }
            foreach (Employee emp in employees)
            {
                if (emp.EMail != null && emp.EMail != String.Empty)
                {
                    builder.Append(emp.EMail);
                }
                else
                {
                    builder.Append(emp.EID + WebConfigurationManager.AppSettings["MailAlias"]);
                }
                builder.Append(", ");
            }
            if (employees.Count > 0)
            {
                RemoveEndingComa(builder);
            }
            if (userName == null || userName == "")
                builder.Append("</textarea>");
           
            return new MvcHtmlString(builder.ToString());
        }

        public static void RemoveEndingComa(StringBuilder sb)
        {
            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }
        }


    }
}