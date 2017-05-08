using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Helpers
{
    public class Interval
    {
        public int weekFrom { get; set; }
        public int weekTo { get; set; }
        public int year { get; set; }
    }

    public static class WeekIntervalHelper
    {
        public static List<Interval> CustomCreateWeekInterval(this HtmlHelper helper, int YearFrom, int YearTo, int WeekFrom, int WeekTo)
        {
            List<Interval> result = new List<Interval>();

            for (int i = YearFrom; i <= YearTo; i++)
            {
                if (YearFrom == YearTo)
                {
                    result.Add(new Interval { weekFrom = WeekFrom, weekTo = WeekTo, year = YearFrom });
                    break;
                }

                if (i == YearFrom)
                {
                    result.Add(new Interval { weekFrom = WeekFrom, weekTo = 52, year = YearFrom });
                }

                if (i == YearTo)
                {
                    result.Add(new Interval { weekFrom = 1, weekTo = WeekTo, year = YearTo });
                }

                if (i != YearFrom && i != YearTo)
                {
                    result.Add(new Interval { weekFrom = 1, weekTo = 52, year = i });
                }
            }

            return result;
 
        }
    }
}