using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Helpers
{
    public static class DisplayVisaStartAndDueDates
    {
        public static MvcHtmlString CustomDisplayVisaStartAndDueDates(this HtmlHelper helper, Visa visa, string href)
        {
            if (visa != null)
            {
                if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().AddDays(90).Date)
                {
                    return new MvcHtmlString(String.Format("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"{0}\">{1:dd'.'MM'.'yyyy} - {2:dd'.'MM'.'yyyy}</a>", href, visa.StartDate, visa.DueDate));
                }
                else if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().Date)
                {
                    int DaysToVisaExpiration = (int)visa.DueDate.Subtract(DateTime.Now.ToLocalTimeAzure().Date).TotalDays;
                    return new MvcHtmlString(String.Format("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"{0}\" style=\"color: orange; font-weight:bold;\" title=\"Visa expires in {3} days\">{1:dd'.'MM'.'yyyy} - {2:dd'.'MM'.'yyyy}</a>", href, visa.StartDate, visa.DueDate, DaysToVisaExpiration));
                }
                else if (visa.DueDate.Date == DateTime.Now.ToLocalTimeAzure().Date)
                {
                    return new MvcHtmlString(String.Format("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"{0}\" style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">{1:dd'.'MM'.'yyyy} - {2:dd'.'MM'.'yyyy}</a>", href, visa.StartDate, visa.DueDate));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"{0}\" style=\"color: red; font-weight:bold;\" title=\"Visa expired\">{1:dd'.'MM'.'yyyy} - {2:dd'.'MM'.'yyyy}</a>", href, visa.StartDate, visa.DueDate));
                }
            }

            return new MvcHtmlString("");
        }
        public static MvcHtmlString CustomDisplayVisaStartAndDueDates(this HtmlHelper helper, Visa visa)
        {
            if (visa != null)
            {
                if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().AddDays(90).Date)
                {
                    return new MvcHtmlString(String.Format("{0:dd'.'MM'.'yyyy} - {1:dd'.'MM'.'yyyy}", visa.StartDate, visa.DueDate));
                }
                else if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().Date)
                {
                    int DaysToVisaExpiration = (int)visa.DueDate.Subtract(DateTime.Now.ToLocalTimeAzure().Date).TotalDays;
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in {2} days\">{0:dd'.'MM'.'yyyy} - {1:dd'.'MM'.'yyyy}</div>", visa.StartDate, visa.DueDate, DaysToVisaExpiration));
                }
                else if (visa.DueDate.Date == DateTime.Now.ToLocalTimeAzure().Date)
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">{0:dd'.'MM'.'yyyy} - {1:dd'.'MM'.'yyyy}</div>",  visa.StartDate, visa.DueDate));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">{0:dd'.'MM'.'yyyy} - {1:dd'.'MM'.'yyyy}</div>", visa.StartDate, visa.DueDate));
                }
            }

            return new MvcHtmlString("");
        }

        public static MvcHtmlString CustomDisplayVisaStartDate(this HtmlHelper helper, Visa visa)
        {
            if (visa != null)
            {
                if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().AddDays(90).Date)
                {
                    return new MvcHtmlString(String.Format("{0:yyyy'.'MM'.'dd}", visa.StartDate));
                }
                else if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().Date)
                {
                    int DaysToVisaExpiration = (int)visa.DueDate.Subtract(DateTime.Now.ToLocalTimeAzure().Date).TotalDays;
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in {1} days\">{0:yyyy'.'MM'.'dd}</div>", visa.StartDate, DaysToVisaExpiration));
                }
                else if (visa.DueDate.Date == DateTime.Now.ToLocalTimeAzure().Date)
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">{0:yyyy'.'MM'.'dd}</div>", visa.StartDate));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">{0:yyyy'.'MM'.'dd}</div>", visa.StartDate));
                }
            }

            return new MvcHtmlString("");
        }

        public static MvcHtmlString CustomDisplayVisaDueDates(this HtmlHelper helper, Visa visa)
        {
            if (visa != null)
            {
                if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().AddDays(90).Date)
                {
                    return new MvcHtmlString(String.Format("{0:yyyy'.'MM'.'dd}", visa.DueDate));
                }
                else if (visa.DueDate.Date > DateTime.Now.ToLocalTimeAzure().Date)
                {
                    int DaysToVisaExpiration = (int)visa.DueDate.Subtract(DateTime.Now.ToLocalTimeAzure().Date).TotalDays;
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in {1} days\">{0:yyyy'.'MM'.'dd}</div>", visa.DueDate, DaysToVisaExpiration));
                }
                else if (visa.DueDate.Date == DateTime.Now.ToLocalTimeAzure().Date)
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">{0:yyyy'.'MM'.'dd}</div>", visa.DueDate));
                }
                else
                {
                    return new MvcHtmlString(String.Format("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">{0:yyyy'.'MM'.'dd}</div>", visa.DueDate));
                }
            }

            return new MvcHtmlString("");
        }
    }
}
