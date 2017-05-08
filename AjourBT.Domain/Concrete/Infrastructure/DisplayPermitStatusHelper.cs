using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.Infrastructure

{
    //TODO: rewrite as extension method for Employee model, this is not an html helper at all //tebo
    public static class DisplayPermitStatusHelper
    {
        public static MvcHtmlString CustomDisplayPermitStatus(this HtmlHelper helper, Employee employee)
        {
            if (employee != null && employee.Permit != null && employee.Permit.EndDate != null)
            {
                if (employee.Permit.CancelRequestDate != null)
                    return new MvcHtmlString(String.Format("<span title=\"Requested to be cancelled\" style=\"color: red\">{0:dd'.'MM'.'yyyy}</span>", employee.Permit.CancelRequestDate));
                if (employee.Permit.ProlongRequestDate != null)
                    return new MvcHtmlString(String.Format("<span title=\"Requested to be prolonged\" style=\"color: green\">{0:dd'.'MM'.'yyyy}</span>", employee.Permit.ProlongRequestDate));

                DateTime StartingPoint = GetStartingDateTimePointForPermitExpiration(employee);

                if (Is60ToLessThan90Days(StartingPoint))
                    return new MvcHtmlString("<span title=\"Last BT more than 60 days ago\" style=\"background-color: orange\">Contact Gov</span>");
                if (Is90DaysAndAbove(StartingPoint))
                    return new MvcHtmlString("<span title=\"Last BT more than 90 days ago\" style=\"background-color: red; color: white\">Contact Gov</span>");
            }
            return new MvcHtmlString("");
        }

        public static DateTime GetStartingDateTimePointForPermitExpiration(Employee employee)
        {
            if (employee != null)
            {
                return GetLastBTForEmployee(employee) != null ?
                            GetLastBTForEmployee(employee).Value :
                            (employee.Permit != null && employee.Permit.StartDate != null) ?
                                employee.Permit.StartDate.Value :
                                DateTime.Now.ToLocalTimeAzure();
            }
            return DateTime.Now.ToLocalTimeAzure();
        }

        public static DateTime? GetLastBTForEmployee(Employee employee)
        {
            if (employee != null && employee.BusinessTrips!=null && employee.Permit !=null && employee.Permit.StartDate!=null && employee.Permit.StartDate.Value!=null)
            {
                return employee.BusinessTrips
                    .Where(b => (b.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date)
                      && (b.EndDate.Date >= employee.Permit.StartDate.Value.Date)
                    && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                    .Select(b => b.EndDate as DateTime?)
                    .LastOrDefault(d => d != default(DateTime));
            }
            return null;
        }

        public static bool IsLessThan60Days(DateTime StartingPoint)
        {
            return (int)(DateTime.Now.ToLocalTimeAzure().Subtract(StartingPoint).TotalDays) < 60;
        }

        public static bool Is60ToLessThan90Days(DateTime StartingPoint)
        {
            return !IsLessThan60Days(StartingPoint) && (int)(DateTime.Now.ToLocalTimeAzure().Subtract(StartingPoint).TotalDays) < 90;
        }

        public static bool Is90DaysAndAbove(DateTime StartingPoint)
        {
            return (int)(DateTime.Now.ToLocalTimeAzure().Subtract(StartingPoint).TotalDays) >= 90;
        }


    }
}
