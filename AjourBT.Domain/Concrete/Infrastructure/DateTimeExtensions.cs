using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace AjourBT.Domain.Infrastructure
{
    public static class DateTimeExtentions
    {
        static TimeZoneInfo localZone;

        static DateTimeExtentions()
        {
            string timeZoneId = WebConfigurationManager.AppSettings["TimeZoneId"].ToString();
            localZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public static DateTime ToLocalTimeAzure(this DateTime time)
        {
            DateTime timeUtc = time.ToUniversalTime();
            DateTime result = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, localZone);
            return result;
        }
    }
}