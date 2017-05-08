using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class StatusHintHelperForVUPreps
    {
        public static string CustomStatusHintForVUPreps(this HtmlHelper helper, BusinessTrip businessTrip)
        {
            StringBuilder hintBuilder = new StringBuilder("");

            if (businessTrip.Status.HasFlag(BTStatus.Modified)
                && !businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && (businessTrip.RejectComment == null || businessTrip.RejectComment == String.Empty))
            {
                hintBuilder.Append("BT was Modified!");
                hintBuilder.AppendLine();
            }

            if (businessTrip.Status.HasFlag(BTStatus.Modified)
                && businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && (businessTrip.RejectComment != null || businessTrip.RejectComment != String.Empty ))
            {
                hintBuilder.Append("BT was first Modified then cancelled!");
                hintBuilder.AppendLine();
            }

            if (!businessTrip.Status.HasFlag(BTStatus.Modified)
                && businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && (businessTrip.RejectComment != null || businessTrip.RejectComment != String.Empty))
            {
                hintBuilder.Append("BT was cancelled!");
                hintBuilder.AppendLine();
            }

            if (businessTrip.Status.HasFlag(BTStatus.Planned)
                && (businessTrip.Purpose == null || businessTrip.Purpose == String.Empty
                    || businessTrip.Manager == null || businessTrip.Manager == String.Empty
                    || businessTrip.Responsible == null || businessTrip.Responsible == String.Empty))
            {
                hintBuilder.Append("Not enough info for registration. ");
            }

            if (businessTrip.Status.HasFlag(BTStatus.Modified)
                && !businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && businessTrip.RejectComment != String.Empty
                && businessTrip.RejectComment != null)
            {
                hintBuilder.Append("BT is Rejected!");
            }

            if (businessTrip.Status.HasFlag(BTStatus.Reported))
            {
                hintBuilder.Append("BT is Reported!");
            }

            return hintBuilder.ToString();
        }
    }
}