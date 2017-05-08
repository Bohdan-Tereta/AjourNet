using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class FactorData
    {
        public CalendarItemType Factor {get; set;}
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public byte Hours { get; set; }
        public int WeekNumber { get; set; }
        public string Location { get; set; }
    }

    public class WTRViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ID { get; set; }
        public List<FactorData> FactorDetails { get; set; }

        [Display(Name = "Date Dismissed")]
        public string DateDismissed { get; set; } 

        public WTRViewModel(){
            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;
        }

        public WTRViewModel(Employee employee, DateTime from, DateTime to)
            :this()
        {

            ID = employee.EID; 
            FirstName = employee.FirstName; 
            LastName = employee.LastName; 
            FactorDetails = new List<FactorData>();
            if (employee.DateDismissed != null)
            {
                DateDismissed = employee.DateDismissed.Value.ToShortDateString();
            }

            Dictionary<int, StartEndDatePair> timeSpanDatePairDict = GetWeeksInTimeSpan(from, to);
            Dictionary<int, StartEndDatePair> calendarItemDatePairDict = new Dictionary<int, StartEndDatePair>();
            Dictionary<int, StartEndDatePair> resultDatePairDict = new Dictionary<int, StartEndDatePair>(); 

            foreach (var calendarItems in employee.CalendarItems.Where((f => (f.From <= from && f.To >= from) || (f.From >= from && f.From <= to))))
            {
                calendarItemDatePairDict = GetWeeksInTimeSpan(calendarItems.From, calendarItems.To);

                resultDatePairDict = IntersectDatePairDicts(timeSpanDatePairDict, calendarItemDatePairDict);
                foreach (int week in resultDatePairDict.Keys)
                {
                    FactorData data = new FactorData();
                    data.Factor = calendarItems.Type;
                    data.Location = calendarItems.Location;
                    data.From = resultDatePairDict[week].startDate;
                    data.To = resultDatePairDict[week].endDate;
                    data.Hours = 0;
                    data.WeekNumber = week;
                    FactorDetails.Add(data);
                } 
            }

            SortFactorDataByStartDate();
        }

        public WTRViewModel(WTRViewModel wtrPerson, int weekNum, int year) 
            :this()
        {

            ID = wtrPerson.ID;
            FirstName = wtrPerson.FirstName;
            LastName = wtrPerson.LastName;
            FactorDetails = new List<FactorData>();

            foreach (var fData in wtrPerson.FactorDetails)
            {
                if (fData.WeekNumber == weekNum && fData.From.Year == year)
                {
                    FactorData data = new FactorData();
                    data.Factor = fData.Factor;
                    data.From = fData.From;
                    data.To = fData.To;
                    data.Hours = 0;
                    data.WeekNumber = fData.WeekNumber;
                    data.Location = fData.Location;
                    FactorDetails.Add(data);
                }
            } 

            SortFactorDataByStartDate();
        }

        public struct StartEndDatePair
        {
            public DateTime startDate;
            public DateTime endDate;
        }

        public Dictionary<int, StartEndDatePair> IntersectDatePairDicts(
            Dictionary<int, StartEndDatePair> firstDatePairDict,
            Dictionary<int, StartEndDatePair> secondDatePairDict)
        {
            DateTime from;
            DateTime to;
            Dictionary<int, StartEndDatePair> resultDataPairs = new Dictionary<int, StartEndDatePair>();

            foreach (int week in firstDatePairDict.Keys.Intersect(secondDatePairDict.Keys))
            {
                from = firstDatePairDict[week].startDate > secondDatePairDict[week].startDate ?
                    firstDatePairDict[week].startDate :
                    secondDatePairDict[week].startDate;
                to = firstDatePairDict[week].endDate < secondDatePairDict[week].endDate ?
                    firstDatePairDict[week].endDate :
                    secondDatePairDict[week].endDate;
                resultDataPairs.Add(week, new StartEndDatePair { startDate = from, endDate = to });
            }

            return resultDataPairs;
        }

        public Dictionary<int, StartEndDatePair> GetWeeksInTimeSpan(DateTime from, DateTime to)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            Dictionary<int, StartEndDatePair> weeks = new Dictionary<int, StartEndDatePair>();
            int weekNumber = 0;

            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                weekNumber = cal.GetWeekOfYear(day, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                if (weeks.Count == 0 || !weeks.Keys.Contains(weekNumber))
                {
                    weeks.Add(weekNumber, new StartEndDatePair { startDate = day, endDate = day });
                }
                else if (weeks.Keys.Contains(weekNumber))
                {
                    weeks[weekNumber] = new StartEndDatePair { startDate = weeks[weekNumber].startDate, endDate = day };
                }

            }

            return weeks;
        }

        public void SortFactorDataByStartDate()
        {

                if (FactorDetails != null && FactorDetails.Count() != 0)
                {
                    FactorDetails = FactorDetails.OrderBy(f => f.From).ToList();
                }
        }
    }
}