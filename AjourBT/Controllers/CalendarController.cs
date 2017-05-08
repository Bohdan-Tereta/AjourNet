using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Infrastructure;

namespace AjourBT.Controllers
{
    public class CalendarController : Controller
    {
        private IRepository repository;
        public CalendarController(IRepository repo)
        {
            this.repository = repo;
        }
        //
        // GET: /Calendar/
        [Authorize(Roles = "ABM, VU")]
        public ViewResult GetCalendar(string selectedDepartment = null)
        {
            ViewBag.DepartmentDropDownList = DepartmentDropDownList();
            ViewBag.SelectedDepartment = selectedDepartment;
            return View();
        }

        public List<DateTime> GetHolidaysData()
        {
            var holidays = (from holiday in repository.Holidays
                            where (holiday.CountryID == 1 && holiday.IsPostponed == false)
                            orderby holiday.HolidayDate
                            select holiday.HolidayDate.ToUniversalTime()).ToList();
            return holidays;
        }

        public List<DateTime> GetPostponedHolidaysData()
        {
            var holidays = (from holiday in repository.Holidays
                            where (holiday.CountryID == 1 && holiday.IsPostponed == true)
                            orderby holiday.HolidayDate
                            select holiday.HolidayDate.ToUniversalTime()).ToList();
            return holidays;
        }
        public SelectList DepartmentDropDownList()
        {
            var depList = from dep in repository.Departments
                          orderby dep.DepartmentName
                          select dep;

            return new SelectList(depList, "DepartmentName", "DepartmentName");

        }

        [Authorize(Roles = "ABM, VU")]
        public PartialViewResult GetCalendarData(string calendarFromDate, string calendarToDate, string selectedDepartment = null)
        {
            DateTime parseFromDate, parseToDate;
            int currentYear = DateTime.Now.Year;

            if (calendarFromDate == "" && calendarToDate == "")
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            try
            {
                parseFromDate = DateTime.ParseExact(calendarFromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(calendarToDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            if (parseFromDate > parseToDate)
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            ViewBag.Holidays = GetHolidaysData();
            ViewBag.PostponedHolidays = GetPostponedHolidaysData();
            List<Employee> empList = SearchEmployeeData(selectedDepartment);
            List<CalendarRowViewModel> rowList = GetCalendarRowData(empList, parseFromDate, parseToDate);
            ViewBag.parseFromDate = parseFromDate;
            ViewBag.parseToDate = parseToDate; 
            //var currentUser = HttpContext.User.Identity.Name;
            if (Request.UrlReferrer != null)
            {
                string myUrl = Request.UrlReferrer.OriginalString;
                if (myUrl.Contains("ABMView"))
                {
                    ViewBag.ItemsPerPage = empList.Count + 1; //+1 for fake row  
                    return PartialView(rowList);
                }
            }
            ViewBag.ItemsPerPage = empList.Count + 1;
            return PartialView("GetCalendarDataVU", rowList);
        }

        public List<Employee> SearchEmployeeData(string selectedDepartment)
        {
            List<Employee> searchList = (from emp in repository.Employees
                                         where emp.DateDismissed == null
                                         join dep in repository.Departments
                                         on emp.DepartmentID equals dep.DepartmentID
                                         where (selectedDepartment == null || selectedDepartment == String.Empty || dep.DepartmentName == selectedDepartment)

                                         orderby emp.LastName ascending
                                         select emp).ToList();
            return searchList;
        }

        public List<CalendarRowViewModel> GetCalendarRowData(List<Employee> empList, DateTime fromDate, DateTime toDate)
        {

            List<CalendarRowViewModel> calendarDataList = new List<CalendarRowViewModel>();

            foreach (var emp in empList)
            {
                CalendarRowViewModel oneRow = new CalendarRowViewModel { id = emp.EmployeeID.ToString(), name = emp.LastName + " " + emp.FirstName, desc = "  ", values = new List<CalendarItemViewModel>() };

                foreach (var calendarItem in emp.CalendarItems)
                {
                    if ((calendarItem.From <= fromDate && calendarItem.To >= fromDate) || (calendarItem.From >= fromDate && calendarItem.From <= toDate))
                    {
                        CalendarItemViewModel oneItem = new CalendarItemViewModel(calendarItem);
                        oneItem.desc += String.Format(" From: {0} - To: {1}", calendarItem.From.ToShortDateString(), calendarItem.To.ToShortDateString());
                        if (calendarItem.Type == CalendarItemType.SickAbsence)
                        {
                            Sickness selectedSick = (from s in repository.Sicknesses where ((s.From == calendarItem.From) && (s.To == calendarItem.To) && s.EmployeeID == calendarItem.EmployeeID) select s).FirstOrDefault();
                            if (selectedSick != null)
                                oneItem.sickType = selectedSick.SicknessType;
                        }

                        CalendarItemViewModel correctedItem = CheckAndCorrectCalendarItem(oneItem, fromDate, toDate);

                        correctedItem.from = correctedItem.from.ToUniversalTime();
                        correctedItem.to = correctedItem.to.ToUniversalTime();
                        oneRow.values.Add(correctedItem);
                    }
                }

                calendarDataList.Add(oneRow);
            }



            List<CalendarRowViewModel> result = InsertFakeEmployee(calendarDataList, fromDate.ToUniversalTime(), toDate.ToUniversalTime());
            return result;
            //return calendarDataList;
        }

        public List<CalendarRowViewModel> InsertFakeEmployee(List<CalendarRowViewModel> dataList, DateTime from, DateTime to)
        {
            DateTime present = from;
            DateTime yearly = to;

            CalendarRowViewModel fakeRow = new CalendarRowViewModel
            {
                id = "fake_row",
                name = " ",
                desc = " ",
                values = new List<CalendarItemViewModel>() { new CalendarItemViewModel{ id = 1, from = present, to = present, customClass = "ganttWhite", desc = "" },
                                                             new CalendarItemViewModel{ id = 1, from = yearly, to = yearly ,customClass = "ganttWhite", desc = ""} }
            };

            //var isValue = (from item in dataList where item.values.Count != 0 select item).FirstOrDefault();
            //if (isValue == null)
            //{
            dataList.Add(fakeRow);
            //}

            return dataList;
        }

        public ActionResult printCalendarToPdf(string calendarFromDate, string calendarToDate,   string selectedDepartment = null)
        {
            DateTime parseFromDate;
            DateTime parseToDate;
            int currentYear = DateTime.Now.Year;

            if (calendarFromDate == "" && calendarToDate == "")
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            try
            {
                parseFromDate = DateTime.ParseExact(calendarFromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(calendarToDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            List<Employee> empList = SearchEmployeeData(selectedDepartment);
            List<CalendarRowViewModel> rowList = GetCalendarRowData(empList, parseFromDate, parseToDate);

            foreach (CalendarRowViewModel rowModel in rowList)
            {
                foreach (CalendarItemViewModel itemModel in rowModel.values)
                {
                    itemModel.from=itemModel.from.ToLocalTime(); 
                    itemModel.to=itemModel.to.ToLocalTime(); 
                }
            }

            List<Holiday> holidays = new List<Holiday>();
            foreach (Holiday holiday in repository.Holidays.Where(h => h.CountryID == 1))
            {
                holidays.Add(holiday);
            }
            return File(CalendarToPdfExporter.GeneratePDF(rowList, holidays, parseFromDate, parseToDate).ToArray(), "application/pdf", "Calendar.pdf");

        }

        private CalendarItemViewModel CheckAndCorrectCalendarItem(CalendarItemViewModel itemToCheck, DateTime from, DateTime to)
        {
            if (itemToCheck.from < from)
            {
                itemToCheck.from = from;
            }
            if (itemToCheck.to > to)
            {
                itemToCheck.to = to;
            }

            return itemToCheck;
        }
               
    }
}