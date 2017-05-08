using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    class JourneyComparer: IEqualityComparer<Journey>
{


    public bool Equals(Journey x, Journey y)
    {
        return x.BusinessTripID == y.BusinessTripID && x.Date == y.Date;
    }

    public int GetHashCode(Journey obj)
    {
       return obj.BusinessTripID.GetHashCode() ^ obj.Date.GetHashCode();
    }
}
}
