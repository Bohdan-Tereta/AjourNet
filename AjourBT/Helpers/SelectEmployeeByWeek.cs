using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public static class SelectEmployeeByWeek
    {
        public static IEnumerable<WTRViewModel> CustomSelectEmployeeByWeek(this HtmlHelper helper, IEnumerable<WTRViewModel> wtrList, int weekNum, int Year)
        {
            List<WTRViewModel> result = new List<WTRViewModel>();
            foreach (var wtrPerson in wtrList)
            {
                WTRViewModel onePerson = new WTRViewModel { ID = wtrPerson.ID, FirstName = wtrPerson.FirstName, LastName = wtrPerson.LastName, FactorDetails = new List<FactorData>() };
                if (wtrPerson.DateDismissed != String.Empty)
                    onePerson.DateDismissed = wtrPerson.DateDismissed;
                foreach (var fData in wtrPerson.FactorDetails)
                {
                    if (fData.WeekNumber == weekNum && fData.From.Year == Year)
                    {
                        FactorData data = new FactorData();
                        data.Factor = fData.Factor;
                        data.From = fData.From;
                        data.To = fData.To;
                        data.Hours = 0;
                        data.WeekNumber = fData.WeekNumber;
                        data.Location = fData.Location;
                        onePerson.FactorDetails.Add(data);
                    }
                }
                if (onePerson.FactorDetails.Count > 0)
                {
                    result.Add(onePerson);
                }
            }
            return result;
        }         
    }
}