using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Helpers
{
    public static class LastBTGetter
    {
        public static BusinessTrip GetLastBT(this HtmlHelper helper, Employee emp)
        {
            if (emp.BusinessTrips != null)
            {
                return
                     emp.BusinessTrips
                        .Where(b => (b.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                            && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                            .OrderBy(b => b.EndDate)
                        .LastOrDefault();
            }
            else
                return null;
        }
    }
}