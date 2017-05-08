using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Infrastructure
{
    public class JourneyByDateComparer: IComparer<Journey>
    {

        public int Compare(Journey x, Journey y)
        {
            if (x.Date > y.Date)
                return 1;
            if (y.Date > x.Date)
                return -1;
            return 0;
        }
    }
}