using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers//TODO: Add items to CalendarItem (Employee)
{
    public class JourneyController : Controller
    {
        private IRepository repository;
        string ModelError = "The record you attempted to edit "
                             + "was modified by another user after you got the original value. The "
                             + "edit operation was canceled.";

        public JourneyController(IRepository repo)
        {
            repository = repo;
        }


        public ViewResult GetJourney(string searchString = "")
        {
            ViewBag.SearchString = searchString;

            return View();
        }


        //
        // GET: /Journey/

        [Authorize(Roles = "ABM, VU")]
        public ViewResult GetJourneyData(string searchString = "")
        {
            List<JourneysAndOvertimesModel> data = GetJourneysAndOvertimes(searchString);

            if (Request.UrlReferrer != null)
            {
                string myUrl = Request.UrlReferrer.OriginalString;
                if (myUrl.Contains("ABMView"))
                {
                    ViewBag.SearchString = searchString;
                    return View(data);
                }
            }
            ViewBag.SearchString = searchString;
            return View("GetJourneyDataForVU", data);
        }

        [Authorize(Roles = "ABM, VU, EMP")]
        public ViewResult GetJourneyDataEMP(string userName = "")
        {
            Employee employee = repository.Employees.FirstOrDefault(e => e.EID == userName);

            List<JourneysAndOvertimesModel> data = GetJourneysAndOvertimes(userName);
            if (employee == null || data == null )
            {
                return View("NoData");
            }
            else
            {
                if (userName != null)
                {
                    ViewBag.UserName = userName;
                }
                return View("GetJourneyDataForEmp", data);
            }
        }


        public List<JourneysByEmployeeViewModel> SearchJourneyData(string searchString)
        {
            var emploeesList = from e in repository.Employees
                               orderby e.DateDismissed,e.Department.DepartmentName, e.LastName
                               select e;
            List<JourneysByEmployeeViewModel> journeysByEmployee = new List<JourneysByEmployeeViewModel>();

            foreach (Employee employee in emploeesList)
            {
                if ((employee.EID.ToLower().Contains(searchString.ToLower()))
                      || (employee.FirstName.ToLower().Contains(searchString.ToLower()))
                      || (employee.LastName.ToLower().Contains(searchString.ToLower())))
                {
                    JourneysByEmployeeViewModel emp = new JourneysByEmployeeViewModel();
                    emp.EmployeeID = employee.EmployeeID;
                    emp.EID = employee.EID;
                    emp.Department = employee.Department.DepartmentName;
                    emp.FirstName = employee.FirstName;
                    emp.LastName = employee.LastName;
                    if (employee.DateDismissed != null)
                    {
                        emp.DateDismissed = employee.DateDismissed.Value.ToShortDateString();
                    }
                    emp.Journeys = new List<Journey>();

                    foreach (BusinessTrip b in employee.BusinessTrips)
                    {
                        foreach (Journey journey in b.Journeys.Where(j => j.DayOff==true))
                        {
                            emp.Journeys.Add(journey);
                        }
                    }
                    emp.Journeys.Sort(new JourneyByDateComparer());
                    journeysByEmployee.Add(emp);
                }
            }

            return journeysByEmployee;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditJourney(int id = 0, DateTime? reclaimDate = null, string searchString = "")
        {
            Journey journeyDate = (from journey in repository.Journeys where (id == journey.JourneyID) select journey).FirstOrDefault();

            if (journeyDate == null)
            {
                ModelState.AddModelError("error", "error");
            }
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (reclaimDate != null && journeyDate.ReclaimDate == null)
                    {
                        CalendarItem item = new CalendarItem
                        {
                            Employee = journeyDate.JourneyOf.BTof,
                            EmployeeID = journeyDate.JourneyOf.EmployeeID,
                            From = reclaimDate.Value,
                            To = reclaimDate.Value,
                            Type = CalendarItemType.ReclaimedOvertime
                        };

                        journeyDate.JourneyOf.BTof.CalendarItems.Add(item);
                        repository.SaveCalendarItem(item);
                    }
                    else if (reclaimDate == null && journeyDate.ReclaimDate != null)
                    {
                        CalendarItem reclaimedItem = journeyDate.JourneyOf.BTof.CalendarItems.Where(
                                c =>
                                c.EmployeeID == journeyDate.JourneyOf.EmployeeID &&
                                c.From == journeyDate.ReclaimDate &&
                                c.To == journeyDate.ReclaimDate &&
                                c.Type == CalendarItemType.ReclaimedOvertime).FirstOrDefault();
                        if (reclaimedItem != null)
                        {
                            journeyDate.JourneyOf.BTof.CalendarItems.Remove(reclaimedItem);
                            repository.DeleteCalendarItem(reclaimedItem.CalendarItemID);
                        }
                    }
                    else if (reclaimDate != null && journeyDate.ReclaimDate != null && reclaimDate != journeyDate.ReclaimDate)
                    {
                        CalendarItem reclaimedItem = journeyDate.JourneyOf.BTof.CalendarItems.Where(
                                 c =>
                                 c.EmployeeID == journeyDate.JourneyOf.BTof.EmployeeID &&
                                 c.From == journeyDate.ReclaimDate &&
                                 c.To == journeyDate.ReclaimDate &&
                                 c.Type == CalendarItemType.ReclaimedOvertime).FirstOrDefault();
                        if (reclaimedItem != null)
                        {
                            journeyDate.JourneyOf.BTof.CalendarItems.Remove(reclaimedItem);
                            repository.DeleteCalendarItem(reclaimedItem.CalendarItemID);
                        }
                        CalendarItem item = new CalendarItem
                        {
                            Employee = journeyDate.JourneyOf.BTof,
                            EmployeeID = journeyDate.JourneyOf.EmployeeID,
                            From = reclaimDate.Value,
                            To = reclaimDate.Value,
                            Type = CalendarItemType.ReclaimedOvertime
                        };

                        journeyDate.JourneyOf.BTof.CalendarItems.Add(item);
                        repository.SaveCalendarItem(item);
                    }
                    journeyDate.ReclaimDate = reclaimDate;
                    repository.SaveJourney(journeyDate);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    return Json(new { error = ModelError });
                }

                
                ViewBag.SearchString = searchString;

                List<JourneysAndOvertimesModel> emp = GetJourneysAndOvertimes(searchString);
                return View("TableViewJourneyAndOvertimeData", emp);
            }

            return View(journeyDate);
        }

        [HttpGet]
        public ActionResult EditJourney(int id = 0, string searchString = "")
        {
            Journey journeyDate = (from journey in repository.Journeys where (id == journey.JourneyID) select journey).FirstOrDefault();
            ViewBag.SearchString = searchString;
            ViewBag.ID = id;
            return View(journeyDate);
        }

        
        public List<JourneysAndOvertimesModel> GetJourneysAndOvertimes(string searchString = "")
        {
            using (OvertimeController controller = new OvertimeController(repository))
            {
                List<JourneysByEmployeeViewModel> selectedData = new List<JourneysByEmployeeViewModel>();
                List<OvertimeByEmployeeModel> overtimeData = new List<OvertimeByEmployeeModel>();
                List<JourneysAndOvertimesModel> overtimeAndJourneysData = new List<JourneysAndOvertimesModel>();

                selectedData = SearchJourneyData(searchString);
                overtimeData = controller.SearchOvertimeData(searchString);

                foreach (var item in selectedData)
                {
                    JourneysAndOvertimesModel model = new JourneysAndOvertimesModel();
                    model.Department = item.Department;
                    model.EID = item.EID;
                    model.EmployeeID = item.EmployeeID;
                    model.FirstName = item.FirstName;
                    model.Journeys = item.Journeys;
                    model.LastName = item.LastName;
                    model.DateDismissed = item.DateDismissed;
                    
                    /*-> improve this*/
                    model.Overtimes = (from e in overtimeData where e.EmployeeID == item.EmployeeID select e.Overtimes).FirstOrDefault();

                    overtimeAndJourneysData.Add(model);
                }

                return (overtimeAndJourneysData);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public MvcHtmlString GetJourneysAndOvertimesForOneEmp(int eID = 0)
        {
            using (OvertimeController controller = new OvertimeController(repository))
            {
                if (repository.Employees.Where(e => e.EmployeeID == eID).Count() != 0)
                {
                List<DateTime> journeyDates = new List<DateTime>();
                List<DateTime> overtimeDates = new List<DateTime>();

                journeyDates = SearchJourneyDataForOneEmp(eID);
                overtimeDates = controller.SearchOvertimeDataForOneEmp(eID);

                List<DateTime> overtimeAndJorneyDates = new List<DateTime>();
                overtimeAndJorneyDates.AddRange(journeyDates);
                overtimeAndJorneyDates.AddRange(overtimeDates);
                overtimeAndJorneyDates.Sort();

                return (CreateDatesToReclaimForDropdown(overtimeAndJorneyDates));
            }
                else
                {
                    return new MvcHtmlString("");
                }
        }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public MvcHtmlString GetJourneysAndOvertimesForOneEmpEdit(int eID, string from)
        {
            using (OvertimeController controller = new OvertimeController(repository))
            {
                if (repository.Employees.Where(e => e.EmployeeID == eID).Count() != 0)
                {
                DateTime parseFrom;
                    try
                    {
                parseFrom = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return new MvcHtmlString("");
                }
                List<DateTime> journeyDates = new List<DateTime>();
                List<DateTime> overtimeDates = new List<DateTime>();

                journeyDates = SearchJourneyDataForOneEmp(eID);
                overtimeDates = controller.SearchOvertimeDataForOneEmp(eID);

                List<DateTime> overtimeAndJorneyDates = new List<DateTime>();
                overtimeAndJorneyDates.AddRange(journeyDates);
                overtimeAndJorneyDates.AddRange(overtimeDates);
                DateTime reclaimDateOvertimes = repository.Overtimes
                    .Where(o => o.EmployeeID == eID &&
                        o.ReclaimDate == parseFrom) 
                    .Select(o => o.Date).FirstOrDefault();
                DateTime reclaimDateJourneys = repository.Journeys
                    .Where(o => o.JourneyOf.EmployeeID == eID &&
                        o.ReclaimDate == parseFrom)
                    .Select(o => o.Date).FirstOrDefault();

                overtimeAndJorneyDates.Sort();

                if (reclaimDateOvertimes != DateTime.MinValue)
                    overtimeAndJorneyDates.Insert(0, reclaimDateOvertimes);
                if (reclaimDateJourneys != DateTime.MinValue)
                    overtimeAndJorneyDates.Insert(0, reclaimDateJourneys);

                return (CreateDatesToReclaimForDropdown(overtimeAndJorneyDates));
            }
                else
                    return new MvcHtmlString("");
        }
        }


       
        public MvcHtmlString CreateDatesToReclaimForDropdown(List<DateTime> reclaimDates)
        {
            string dates = "";
            foreach (DateTime date in reclaimDates)
            {
                dates += String.Format("<option value=\"{0:dd'.'MM'.'yyyy}\">{0:dd'.'MM'.'yyyy}</option>", date);
            }
            return new MvcHtmlString(dates);
        }

        public List<DateTime> SearchJourneyDataForOneEmp(int eID)
        {
            if (repository.Employees.Where(e => e.EmployeeID == eID).Count() != 0)
            {
            var employee = repository.Employees.Where(e => e.EmployeeID == eID).FirstOrDefault();

            List<DateTime> journeys = new List<DateTime>();

                    foreach (BusinessTrip b in employee.BusinessTrips)
                    {
                    foreach (Journey journey in b.Journeys.Where(j => j.ReclaimDate == null && j.DayOff == true))
                        {
                            journeys.Add(journey.Date);
                        }
                    }

                    return journeys;
        }
            else
            {
                return new List<DateTime>();
            }
        }
    }
}
