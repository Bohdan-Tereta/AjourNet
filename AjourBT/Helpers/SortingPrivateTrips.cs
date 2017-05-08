using AjourBT.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class SortingPrivateTrips
    {
        public static IEnumerable<PrivateTrip> CustomSortingPrivateTripsByStartdate(this HtmlHelper helper, IEnumerable<PrivateTrip> list)
        {
            if (list!=null && list.Count()!= 0)
            {
                list = list.OrderBy(el => el.StartDate);
                
            }
            return list;
        }
    }
}