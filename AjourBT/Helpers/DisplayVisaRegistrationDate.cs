using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayVisaRegistrationDate
    {
        public static MvcHtmlString CustomDisplayVisaRegistrationDateForEMP(this HtmlHelper helper, VisaRegistrationDate visaRegDate)
        {
            StringBuilder builder = new StringBuilder("");

            if (visaRegDate == null || visaRegDate.RegistrationDate == default(DateTime))
            {
                builder.Append("");
            }
            else
            {
                if (visaRegDate.RegistrationDate.HasValue)
                {
                    builder.AppendFormat("<tr><td rowspan=\"4\"><b>Visa Registration Date</b></td> <td><customBlueItalic>Date: </customBlueItalic> {0:dd.MM.yyyy} </td></tr> <br />", visaRegDate.RegistrationDate.Value.Date);
                }
                if (visaRegDate.RegistrationTime == null || visaRegDate.RegistrationTime == "")
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>Time:</customBlueItalic> - </td></tr>");
                }
                else
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>Time:</customBlueItalic> {0}</td></tr>", visaRegDate.RegistrationTime);
                }

                if (visaRegDate.City == null || visaRegDate.City == "")
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>City:</customBlueItalic> - </td></tr>");
                }
                else
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>City:</customBlueItalic> {0}</td></tr>", visaRegDate.City);
                }

                if (visaRegDate.RegistrationNumber == null || visaRegDate.RegistrationNumber == "")
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>Registration Number:</customBlueItalic> - </td> </tr>");
                }
                else
                {
                    builder.AppendFormat("<tr><td><customBlueItalic>Registration Number:</customBlueItalic> {0}</td> <td> <br /> </td> </tr>", visaRegDate.RegistrationNumber);
                }

                builder.Append("<tr> <td><br/></td> </tr>");

            }
            return new MvcHtmlString(builder.ToString());
        }

    }
}