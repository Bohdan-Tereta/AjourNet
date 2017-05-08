using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class SortingFactorDates
    {
        public static IEnumerable<FactorData> CustomSortingFactorDataByStartDate(this HtmlHelper helper, IEnumerable<FactorData> factorData)
        {
            if (factorData != null && factorData.Count() != 0)
            {
                factorData = factorData.OrderBy(f => f.From);
            }
            return factorData;
        }
    }
}