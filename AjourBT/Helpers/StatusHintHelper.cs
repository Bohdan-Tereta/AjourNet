using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class StatusHintHelper
    {
        public static string CustomStatusHint(this HtmlHelper helper, BusinessTrip businessTrip)
        {
            StringBuilder hintBuilder = new StringBuilder("");

            if (businessTrip.Status == (BTStatus.Planned | BTStatus.Modified)
                ||(businessTrip.Status == (BTStatus.Registered | BTStatus.Modified))
                && (businessTrip.RejectComment == null || businessTrip.RejectComment == String.Empty))
            {
                hintBuilder.Append("BT was Replanned!");
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
                hintBuilder.Append("Not enough info for Registration.");
            }

            if (businessTrip.Status.HasFlag(BTStatus.Modified)
                && !businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && businessTrip.RejectComment != String.Empty
                && businessTrip.RejectComment != null)
            {
                hintBuilder.Append("BT was Rejected!");
            }

            if (businessTrip.Status.HasFlag(BTStatus.Reported))
            {
                hintBuilder.Append("BT is Reported \n (Contact ACC to cancel BT)");
            }

            if (businessTrip.Status.HasFlag(BTStatus.Confirmed)
                && !businessTrip.Status.HasFlag(BTStatus.Reported)
                && !businessTrip.Status.HasFlag(BTStatus.Cancelled)
                && (businessTrip.RejectComment == String.Empty || businessTrip.RejectComment == null))
            {
                if(businessTrip.Status.HasFlag(BTStatus.Modified))
                { 
                    hintBuilder.Append("BT is Modified. Contact BTM to cancel BT"); 
                }
                else
                {
                    hintBuilder.Append("BT is Confirmed. Contact BTM to cancel BT"); 
                }
            }

            return hintBuilder.ToString();
        }
    }
}