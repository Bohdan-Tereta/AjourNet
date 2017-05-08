using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    class CalendarItemComparer : IEqualityComparer<CalendarItem>
    {

        public bool Equals(CalendarItem x, CalendarItem y)
        {
            return x.EmployeeID == y.EmployeeID && x.From == y.From && x.To == y.To && x.Location == y.Location
                        && x.Type == y.Type
            ;
        }

        public int GetHashCode(CalendarItem obj)
        {
            return obj.EmployeeID.GetHashCode() ^ obj.From.GetHashCode() ^ obj.To.GetHashCode() ^ obj.Location.GetHashCode()
                        ^ obj.Type.GetHashCode()
                        ;
        }
    }
}
