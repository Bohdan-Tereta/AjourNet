using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class VacationController : Controller
    {
        private IRepository repository;

        public VacationController(IRepository repo)
        {
            this.repository = repo;
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Create(int id, string from, string to, string type)
        {
            Employee emp = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();

            if (emp != null)
            {
                ViewBag.EmployeeLastName = emp.LastName;
                VacationType eventType = VacationType.PaidVacation;
                CalendarItemType calendarType = CalendarItemType.BT;

                DateTime parseFromDate;
                DateTime parseToDate;

                try
                {
                    parseFromDate = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseToDate = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                switch (type)
                {
                    case "Paid Vacation":
                    case "PaidVacation" :
                        {
                            eventType = VacationType.PaidVacation;
                            calendarType = CalendarItemType.PaidVacation;
                            Vacation vacation = new Vacation { EmployeeID = emp.EmployeeID, Employee = emp, From = parseFromDate, To = parseToDate, Type = eventType };
                            CalendarItem item = new CalendarItem { Employee = emp, EmployeeID = emp.EmployeeID, From = parseFromDate, To = parseToDate, Type = calendarType };
                            emp.CalendarItems.Add(item);

                            repository.SaveVacation(vacation);
                            repository.SaveCalendarItem(item);

                        }
                        return true;

                    case "Unpaid Vacation":
                    case "UnpaidVacation" :
                        {
                            eventType = VacationType.UnpaidVacation;
                            calendarType = CalendarItemType.UnpaidVacation;
                            Vacation vacation = new Vacation { EmployeeID = emp.EmployeeID, Employee = emp, From = parseFromDate, To = parseToDate, Type = eventType };
                            CalendarItem item = new CalendarItem { Employee = emp, EmployeeID = emp.EmployeeID, From = parseFromDate, To = parseToDate, Type = calendarType };
                            emp.CalendarItems.Add(item);

                            repository.SaveVacation(vacation);
                            repository.SaveCalendarItem(item);
                        }
                        return true;

                    default:
                        return false;
                }
            }

            return false;
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Edit(int id, string oldFrom, string oldTo, string newFrom, string newTo, string type)
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseFromDate;
                DateTime parseToDate;
                DateTime parseNewFromDate;
                DateTime parseNewToDate;

                try
                {
                    parseFromDate = DateTime.ParseExact(oldFrom, "dd.MM.yyyy", null);
                    parseToDate = DateTime.ParseExact(oldTo, "dd.MM.yyyy", null);
                    parseNewFromDate = DateTime.ParseExact(newFrom, "dd.MM.yyyy", null);
                    parseNewToDate = DateTime.ParseExact(newTo, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                VacationType vType = VacationType.PaidVacation;
                CalendarItemType cType = CalendarItemType.BT;

                switch (type)
                {
                    case "PaidVacation" :
                    case "Paid Vacation":
                        {
                            vType = VacationType.PaidVacation;
                            cType = CalendarItemType.PaidVacation;
                        }
                        break;

                    case "UnpaidVacation" :
                    case "Unpaid Vacation":
                        {
                            vType = VacationType.UnpaidVacation;
                            cType = CalendarItemType.UnpaidVacation;
                        }
                        break;
                    default:
                        return false;
                }


                Vacation vacation = (from vac in employee.Vacations
                                     where vac.EmployeeID == id &&
                                     vac.Employee == employee &&
                                     vac.From == parseFromDate &&
                                     vac.To == parseToDate
                                     && vac.Type == vType
                                     select vac).FirstOrDefault();

                CalendarItem item = (from i in employee.CalendarItems
                                     where i.From == parseFromDate &&
                                     i.To == parseToDate &&
                                     i.Type == cType &&
                                     i.EmployeeID == employee.EmployeeID &&
                                     i.Employee == employee
                                     select i).FirstOrDefault();

                if (vacation != null && item != null)
                {
                    vacation.From = parseNewFromDate;
                    vacation.To = parseNewToDate;
                    repository.SaveVacation(vacation);

                    item.From = parseNewFromDate;
                    item.To = parseNewToDate;
                    repository.SaveCalendarItem(item);
                }
                return true;
            }
            return false;
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Delete(int id, string from, string to, string type)
        {
            Employee emp = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (emp != null)
            {
                VacationType vType = VacationType.PaidVacation;
                CalendarItemType cType = CalendarItemType.PaidVacation;

                DateTime parseFromDate;
                DateTime parseToDate;

                try
                {
                    parseFromDate = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseToDate = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }


                switch (type)
                {
                    case "PaidVacation" :
                    case "Paid Vacation":
                        {
                            vType = VacationType.PaidVacation;
                            cType = CalendarItemType.PaidVacation;
                        }
                        break;

                    case "UnpaidVacation" :
                    case "Unpaid Vacation":
                        {
                            vType = VacationType.UnpaidVacation;
                            cType = CalendarItemType.UnpaidVacation;
                        }
                        break;
                    default:
                        return false;

                }

                Vacation vacation = (from vac in emp.Vacations
                                     where vac.EmployeeID == emp.EmployeeID &&
                                           vac.From == parseFromDate &&
                                           vac.To == parseToDate &&
                                           vac.Type == vType
                                     select vac).FirstOrDefault();

                CalendarItem item = (from calendarItem in emp.CalendarItems
                                     where calendarItem.EmployeeID == emp.EmployeeID &&
                                                  calendarItem.From == parseFromDate &&
                                                  calendarItem.To == parseToDate &&
                                                  calendarItem.Type == cType
                                     select calendarItem).FirstOrDefault();

                if (vacation != null && item != null)
                {
                    repository.DeleteVacation(vacation.VacationID);
                    emp.CalendarItems.Remove(item);
                    repository.DeleteCalendarItem(item.CalendarItemID);
                }
                return true;
            }
            return false;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public int CalculateVacation(string fromDate, string toDate)
        {
            DateTime parseFrom, parseTo;

            try
            {
                parseFrom = DateTime.ParseExact(fromDate, "dd.MM.yyyy", null);
                parseTo = DateTime.ParseExact(toDate, "dd.MM.yyyy", null);
            }

            catch (SystemException)
            {
                return 0;
            }

            int numberOfWorkingDays = 0;
            DateTime loopFrom = parseFrom;
            while (loopFrom <= parseTo)
            {
                if (ISWorkDay(loopFrom) && IsNotNationalHolidayExcludeWeekends(loopFrom))
                {
                    numberOfWorkingDays++;
                }
                loopFrom = new DateTime(loopFrom.Year, loopFrom.Month, loopFrom.Day).Date.AddDays(1);
            }

            return numberOfWorkingDays;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public int CalculateOverralDays(string fromDate, string toDate)
        {
            DateTime parseFrom, parseTo;

            try
            {
                parseFrom = DateTime.ParseExact(fromDate, "dd.MM.yyyy", null);
                parseTo = DateTime.ParseExact(toDate, "dd.MM.yyyy", null);
            }

            catch (SystemException)
            {
                return 0;
            }

            int daysCount = (parseTo - parseFrom).Days;

            if (daysCount < 0)
                return 0;

            return daysCount + 1;
        }

        private static bool ISWorkDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private bool IsNotNationalHolidayExcludeWeekends(DateTime date)
        {
            List<DateTime> holidays = (from h in repository.Holidays where (h.CountryID == 1) && (h.HolidayDate.DayOfWeek != DayOfWeek.Saturday && h.HolidayDate.DayOfWeek != DayOfWeek.Sunday) select h.HolidayDate).ToList();
            return !holidays.Contains(date);
        }
    }
}
