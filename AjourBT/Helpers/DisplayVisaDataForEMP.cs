using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class DisplayVisaDataForEMP
    {
        public static MvcHtmlString CustomDisplayVisaForEMP(this HtmlHelper helper, Visa visa, int usedDays)
        {
            StringBuilder builder = new StringBuilder("");

            if (visa == null)
            {
                builder.Append("<tr><td><b>Visa</b></td><td>No Visa</td></tr> <br />");
            }
            else
            {
                string daysToDisplay = String.Format("{0}({1})", visa.Days, usedDays);

                int? usedEntries;
                string entriesToDisplay;

                if (visa.Entries == 0)
                {
                    entriesToDisplay = "MULT";
                }
                else
                {
                    usedEntries = visa.EntriesUsedInPrivateTrips.Value + visa.EntriesUsedInBT.Value + visa.CorrectionForVisaEntries.Value;
                    entriesToDisplay = String.Format("{0}({1})", visa.Entries, usedEntries);
                }
                builder.AppendFormat("<tr><td rowspan=\"3\"><b>Visa</b></td> <td><customBlueItalic>Type:</customBlueItalic> {0}</td></tr>", visa.VisaType);
                builder.AppendFormat("<tr><td><customBlueItalic>Dates:</customBlueItalic> {0} - {1}</td></tr>", visa.StartDate.Date.ToShortDateString(), visa.DueDate.Date.ToShortDateString());
                builder.AppendFormat("<tr><td><customBlueItalic>Entries: </customBlueItalic> {0}, <customBlueItalic>Days:</customBlueItalic> {1}</td></tr> <br />", entriesToDisplay, daysToDisplay);
               
            }

            builder.Append(" <tr> <td><br/></td> </tr>");

            return new MvcHtmlString(builder.ToString());
        }
    }
}