using AjourBT.Domain.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests
{
    public static class Tools
    {
        public static void HtmlTableToArray(string tbodyName, IWebDriver driver, out string[,] resultArray)
        {
            ReadOnlyCollection<IWebElement> rows;
            ReadOnlyCollection<IWebElement> cols;
            string[,] tableArray = null;

            if (!(string.IsNullOrEmpty(tbodyName) || string.IsNullOrWhiteSpace(tbodyName)))
            {
                rows = driver.FindElements(By.CssSelector("tbody#" + tbodyName + " tr"));

                if (rows == null)
                {
                    rows = driver.FindElements(By.CssSelector("tbody." + tbodyName + " tr"));
                    cols = rows[0].FindElements(By.CssSelector("td"));
                }
                else
                {
                    cols = rows[0].FindElements(By.CssSelector("td"));

                    tableArray = new string[rows.Count - 1, cols.Count - 1];

                    for (int i = 0; i < rows.Count - 1; i++)
                    {
                        cols = rows[i].FindElements(By.CssSelector("td"));

                        for (int j = 0; j < cols.Count - 1; j++)
                        {
                            tableArray[i, j] = cols[j].Text;
                        }
                    }
                }
            }

            resultArray = tableArray;
        }

        public static void FindElementInTable(string[,] sourceArray, string textToFind, out int rowNum, out int cellNum)
        {
            for (int i = 0; i < sourceArray.GetLength(0); i++)
            {
                for (int j = 0; j < sourceArray.GetLength(1); j++)
                {
                    if (sourceArray[i, j] == textToFind)
                    {
                        rowNum = i;
                        cellNum = j;
                        return;
                    }
                }
                i++;
            }
            rowNum = -1;
            cellNum = -1;
        }

        public static void ClickOnTwoDatePickers(IWebDriver driver, WebDriverWait wait)
        {
            ReadOnlyCollection<IWebElement> pictToClick = driver.FindElements(By.XPath("//img[@class='ui-datepicker-trigger']"));

            Browser.ClickOnWebElement(pictToClick[0]);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.SelectDatePickerValue("1", 0);
            Thread.Sleep(Timings.Default_ms*15);

            Browser.ClickOnWebElement(pictToClick[1]);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.SelectDatePickerValue(DateTime.Now.Day.ToString(), 0);
        }

        public static string TestDate
        {
            get
            {
                string firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01).ToString(String.Format("dd.MM.yyyy"));
                string currentDay = DateTime.Now.ToString(String.Format("dd.MM.yyyy"));

                return firstDayOfMonth+"-"+currentDay;
            }

        }

        public static string TodayDate
        {
            get
            {
                return DateTime.Now.ToString(String.Format("dd.MM.yyyy"));
            }
        }

        public static string FirstDayOfMonth
        {
            get
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01).ToString(String.Format("dd.MM.yyyy"));
            }
        }

        public static ReadOnlyCollection<IWebElement> GetAllTableData(string selector, IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> tempCollection = driver.FindElements(By.XPath(selector));
            return tempCollection;
        }

        public static ReadOnlyCollection<IWebElement> FindAllCheckboxes(IWebElement whereToFind)
        {
            ReadOnlyCollection<IWebElement> checkboxes = whereToFind.FindElements(By.XPath("//input[@class='check-box']"));
            return checkboxes;
        }

        public static ReadOnlyCollection<IWebElement> FindAllTextAreas(IWebElement whereToFind)
        {
            ReadOnlyCollection<IWebElement> textareas = whereToFind.FindElements(By.XPath("//textarea"));
            return textareas;
        }

        public static CalendarItem SearchCalendarItem(int EmployeeID, AjourBTForTestContext context, CalendarItemType type, DateTime fromDate, DateTime toDate)
        {
            CalendarItem item;

            switch (type)
            {
                case CalendarItemType.ReclaimedOvertime:
                    {
                        item = ((from calendarItem in context.CalendarItems
                                 where calendarItem.EmployeeID == EmployeeID
                                       && (calendarItem.Type == CalendarItemType.ReclaimedOvertime || calendarItem.Type == CalendarItemType.PrivateMinus)
                                       && ((calendarItem.From <= fromDate && calendarItem.To >= fromDate)
                                       || (calendarItem.From >= fromDate && calendarItem.From <= toDate))
                                 select calendarItem).FirstOrDefault());
                    }
                    break;
                case CalendarItemType.PaidVacation:
                    {
                        item = ((from calendarItem in context.CalendarItems
                                 where calendarItem.EmployeeID == EmployeeID
                                       && (calendarItem.Type == CalendarItemType.PaidVacation || calendarItem.Type == CalendarItemType.UnpaidVacation)
                                       && ((calendarItem.From <= fromDate && calendarItem.To >= fromDate)
                                       || (calendarItem.From >= fromDate && calendarItem.From <= toDate))
                                 select calendarItem).FirstOrDefault());
                    }
                    break;
                default:
                    item = (from calendarItem in context.CalendarItems
                            where calendarItem.EmployeeID == EmployeeID
                                  && calendarItem.Type == type
                                  && ((calendarItem.From <= fromDate && calendarItem.To >= fromDate)
                                  || (calendarItem.From >= fromDate && calendarItem.From <= toDate))
                            select calendarItem).FirstOrDefault();
                    break;
            }

            return item;
        }

        public static List<CalendarItem> SearchCalendarItemForWTR(AjourBTForTestContext context, DateTime fromDate, DateTime toDate)
        {
            List<CalendarItem> result = (from item in context.CalendarItems
                                         where ((item.From <= fromDate && item.To >= fromDate)
                                             || (item.From >= fromDate && item.From <= toDate))
                                         select item).ToList();
            return result;
        }
    }
}
