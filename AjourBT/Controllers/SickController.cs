using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class SickController : Controller
    {
        private IRepository repository;
        public SickController(IRepository repo)
        {
            this.repository = repo;
        }
        //string ModelError = "The record you attempted to edit "
        //                     + "was modified by another user after you got the original value. The "
        //                     + "edit operation was canceled.";

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Create(int id, string from, string to, string type, string sickness)
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseFromDate;
                DateTime parseToDate;

                if (type != "SickAbsence")
                    return false;

                CalendarItemType cType = CalendarItemType.SickAbsence;

                try
                {
                    parseFromDate = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseToDate = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                Sickness sick = new Sickness();
                sick.From = parseFromDate;
                sick.To = parseToDate;
                sick.EmployeeID = employee.EmployeeID;
                sick.SicknessType = sickness;

                CalendarItem item = new CalendarItem
                {
                    Employee = employee,
                    EmployeeID = employee.EmployeeID,
                    From = parseFromDate,
                    To = parseToDate,
                    Type = cType
                };
                employee.CalendarItems.Add(item);
                repository.SaveSick(sick);
                repository.SaveCalendarItem(item);

                return true;
            }
            return false;
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Edit(int id, string oldFrom, string oldTo, string newFrom, string newTo, string type, string sickness)
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseOldFrom;
                DateTime parseOldTo;
                DateTime parseNewFrom;
                DateTime parseNewTo;

                if (type != "SickAbsence")
                {
                    return false;
                }

                CalendarItemType cType = CalendarItemType.SickAbsence;

                try
                {
                    parseOldFrom = DateTime.ParseExact(oldFrom, "dd.MM.yyyy", null);
                    parseOldTo = DateTime.ParseExact(oldTo, "dd.MM.yyyy", null);
                    parseNewFrom = DateTime.ParseExact(newFrom, "dd.MM.yyyy", null);
                    parseNewTo = DateTime.ParseExact(newTo, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }


                Sickness sicks = (from sick in employee.Sicknesses
                                  where sick.From == parseOldFrom &&
                                         sick.To == parseOldTo &&
                                      sick.SickOf == employee &&
                                      sick.EmployeeID == employee.EmployeeID
                                  select sick).FirstOrDefault();

                CalendarItem item = (from i in employee.CalendarItems
                                     where i.From == parseOldFrom &&
                                     i.To == parseOldTo &&
                                     i.Type == cType &&
                                     i.EmployeeID == employee.EmployeeID &&
                                     i.Employee == employee
                                     select i).FirstOrDefault();

                if (sicks != null && item != null)
                {
                    sicks.From = parseNewFrom;
                    sicks.To = parseNewTo;
                    sicks.SicknessType = sickness;
                    repository.SaveSick(sicks);

                    item.From = parseNewFrom;
                    item.To = parseNewTo;

                    repository.SaveCalendarItem(item);
                    return true;
                }
                return false;
            }
            return false;
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Delete(int id, string from, string to, string type, string sickness)
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseFrom;
                DateTime parseTo;

                if (type != "SickAbsence")
                    return false;

                CalendarItemType cType = CalendarItemType.SickAbsence;

                try
                {
                    parseFrom = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseTo = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                Sickness sicks = (from sick in employee.Sicknesses
                                  where sick.From == parseFrom &&
                                        sick.To == parseTo &&
                                      sick.SickOf == employee &&
                                      sick.EmployeeID == employee.EmployeeID
                                  select sick).FirstOrDefault();

                CalendarItem item = (from i in employee.CalendarItems
                                     where i.Employee == employee &&
                                     i.EmployeeID == employee.EmployeeID &&
                                     i.From == parseFrom &&
                                     i.To == parseTo &&
                                     i.Type == cType
                                     select i).FirstOrDefault();

                if (sicks != null && item != null)
                {
                    repository.DeleteSick(sicks.SickID);
                    employee.CalendarItems.Remove(item);
                    repository.DeleteCalendarItem(item.CalendarItemID);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}