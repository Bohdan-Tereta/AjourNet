using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers //TODO:ADD Items to CalendarItem (Employee)
{
    public class OvertimeController : Controller
    {
        private IRepository repository;
        //string ModelError = "The record you attempted to edit "
        //                     + "was modified by another user after you got the original value. The "
        //                     + "edit operation was canceled.";
        public OvertimeController(IRepository repo)
        {
            this.repository = repo;
        }

        //
        // GET: /Overtime/

        public ActionResult GetOvertimeData(string searchString = "")
        {
            ViewBag.SearchString = searchString;

            List<OvertimeByEmployeeModel> data = SearchOvertimeData(searchString);

            return View(data);
        }

        public ActionResult GetOvertime()
        {
            return View();
        }

        public List<OvertimeByEmployeeModel> SearchOvertimeData(string searchString)
        {
            var emp = from e in repository.Employees
                      orderby e.DepartmentID, e.LastName
                      select e;
            List<OvertimeByEmployeeModel> model = new List<OvertimeByEmployeeModel>();

            foreach (Employee employee in emp)
            {
                if ((employee.EID.ToLower().Contains(searchString.ToLower()))
                      || (employee.FirstName.ToLower().Contains(searchString.ToLower()))
                      || (employee.LastName.ToLower().Contains(searchString.ToLower())))
                {
                    OvertimeByEmployeeModel over = new OvertimeByEmployeeModel();

                    over.Department = employee.Department.DepartmentName;
                    over.EID = employee.EID;
                    over.EmployeeID = employee.EmployeeID;
                    over.FirstName = employee.FirstName;
                    over.LastName = employee.LastName;
                    over.Overtimes = new List<Overtime>();

                    foreach (Overtime overtime in employee.Overtimes.Where(o => o.Type == OvertimeType.Paid))
                    {
                        over.Overtimes.Add(overtime);
                    }
                    over.Overtimes.Sort(new OvertimeByDateComparer());
                    model.Add(over);
                }
            }

            return model;
        }

        [Authorize(Roles = "ABM")]
        [HttpGet]
        public ActionResult EditOvertime(int id, string searchString = "")
        {
            Overtime overtime = (from over in repository.Overtimes where over.OvertimeID == id select over).FirstOrDefault();
            ViewBag.SearchString = searchString;
            ViewBag.ID = id;

            return View(overtime);
        }

        [Authorize(Roles = "ABM")]
        [HttpPost]
        [ActionName("EditOvertime")]
        public ActionResult EditOvertimeConfirmed(int id, DateTime? reclaimDate = null, string searchString = "")
        {
            string ModelError = "";
            Overtime overtime = (from e in repository.Overtimes where e.OvertimeID == id select e).FirstOrDefault();

            if (overtime == null)
            {
                ModelState.AddModelError("error", "error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (reclaimDate != null && overtime.ReclaimDate == null)
                    {
                        overtime.DayOff = true;
                        CalendarItem item = new CalendarItem
                        {
                            Employee = overtime.Employee,
                            EmployeeID = overtime.Employee.EmployeeID,
                            From = reclaimDate.Value,
                            To = reclaimDate.Value,
                            Type = CalendarItemType.ReclaimedOvertime
                        };

                        overtime.Employee.CalendarItems.Add(item);
                        repository.SaveCalendarItem(item);
                    }
                    else if (reclaimDate == null && overtime.ReclaimDate != null)
                    {
                        CalendarItem reclaimedItem = overtime.Employee.CalendarItems.Where(
                                c =>
                                c.EmployeeID == overtime.Employee.EmployeeID &&
                                c.From == overtime.ReclaimDate &&
                                c.To == overtime.ReclaimDate &&
                                c.Type == CalendarItemType.ReclaimedOvertime).FirstOrDefault();
                        if (reclaimedItem != null)
                        {
                            overtime.Employee.CalendarItems.Remove(reclaimedItem);
                            repository.DeleteCalendarItem(reclaimedItem.CalendarItemID);
                        }
                        overtime.DayOff = false;
                    }
                    else if (reclaimDate != null && overtime.ReclaimDate != null && reclaimDate != overtime.ReclaimDate)
                    {
                        CalendarItem reclaimedItem = overtime.Employee.CalendarItems.Where(
                                 c =>
                                 c.EmployeeID == overtime.Employee.EmployeeID &&
                                 //c.From == overtime.ReclaimDate &&
                                 //c.To == overtime.ReclaimDate &&
                                 c.Type == CalendarItemType.ReclaimedOvertime
                                 ).FirstOrDefault();
                        if (reclaimedItem != null)
                        {
                            overtime.Employee.CalendarItems.Remove(reclaimedItem);
                            repository.DeleteCalendarItem(reclaimedItem.CalendarItemID);
                        }
                        CalendarItem item = new CalendarItem
                        {
                            Employee = overtime.Employee,
                            EmployeeID = overtime.Employee.EmployeeID,
                            From = reclaimDate.Value,
                            To = reclaimDate.Value,
                            Type = CalendarItemType.ReclaimedOvertime
                        };

                        overtime.Employee.CalendarItems.Add(item);
                        repository.SaveCalendarItem(item);
                    }
                    overtime.ReclaimDate = reclaimDate;
                    repository.SaveOvertime(overtime);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    ModelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";

                    return Json(new { error = ModelError });
                }

                ViewBag.SearchString = searchString;

                JourneyController controller = new JourneyController(repository);
                List<JourneysAndOvertimesModel> emp = controller.GetJourneysAndOvertimes(searchString);
                return View("TableViewJourneyAndOvertimeData", emp);
            }

            return View(overtime);
        }

        [Authorize(Roles = "ABM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public bool Create(int id, string from, string to, string type, string reclaimDate = "")
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseFromDate;
                DateTime parseToDate;
                DateTime parseReclaimDate;

                OvertimeType oType = OvertimeType.Overtime;
                CalendarItemType cType = CalendarItemType.ReclaimedOvertime;

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
                    case "ReclaimedOvertime":
                        {
                            try
                            {
                                parseReclaimDate = DateTime.ParseExact(reclaimDate, "dd.MM.yyyy", null);
                            }
                            catch (SystemException)
                            {
                                return false;
                            }
                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.ReclaimedOvertime;

                            Overtime overtime = repository.Overtimes.Where(
                                    o => o.EmployeeID == id &&
                                    o.Date == parseReclaimDate &&
                                    o.ReclaimDate == null &&
                                    o.Type == oType).FirstOrDefault();
                            if (overtime == null)
                            {
                                Journey journey = repository.Journeys.Where(
                                    o => o.JourneyOf.EmployeeID == id &&
                                    o.Date == parseReclaimDate &&
                                    o.ReclaimDate == null).FirstOrDefault();
                                if (journey == null)
                                {
                                    return false;
                                }
                                journey.ReclaimDate = parseFromDate;
                                repository.SaveJourney(journey);
                            }
                            else
                            {
                                overtime.ReclaimDate = parseFromDate;
                                overtime.DayOff = true;
                                repository.SaveOvertime(overtime);
                            }
                            CalendarItem item = new CalendarItem
                            {
                                Employee = employee,
                                EmployeeID = employee.EmployeeID,
                                From = parseFromDate,
                                To = parseToDate,
                                Type = cType
                            };

                            employee.CalendarItems.Add(item);
                            repository.SaveCalendarItem(item);
                        }
                        return true;
                    case "OvertimeForReclaim":
                        {
                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.OvertimeForReclaim;

                            Overtime overtime = new Overtime
                            {
                                Date = parseFromDate,
                                DayOff = false,
                                Employee = employee,
                                EmployeeID = employee.EmployeeID,
                                ReclaimDate = null,
                                Type = oType
                            };
                            CalendarItem item = new CalendarItem
                            {
                                Employee = employee,
                                EmployeeID = employee.EmployeeID,
                                From = parseFromDate,
                                To = parseToDate,
                                Type = cType
                            };
                            employee.CalendarItems.Add(item);
                            repository.SaveOvertime(overtime);
                            repository.SaveCalendarItem(item);
                        }
                        return true;
                    case "PrivateMinus":
                        {
                            oType = OvertimeType.Private;
                            cType = CalendarItemType.PrivateMinus;

                            Overtime overtime = new Overtime
                            {
                                Date = parseFromDate,
                                DayOff = false,
                                Employee = employee,
                                EmployeeID = employee.EmployeeID,
                                ReclaimDate = null,
                                Type = oType
                            };

                            CalendarItem item = new CalendarItem
                            {
                                Employee = employee,
                                EmployeeID = employee.EmployeeID,
                                From = parseFromDate,
                                To = parseToDate,
                                Type = cType
                            };
                            employee.CalendarItems.Add(item);
                            repository.SaveOvertime(overtime);
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
        public bool Edit(int id, string oldFrom, string oldTo, string newFrom, string newTo, string type, string reclaimDate = "")
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseOldFrom;
                DateTime parseOldTo;
                DateTime parseNewFrom;
                DateTime parseNewTo;
                DateTime parseReclaimDate = DateTime.MinValue;

                OvertimeType oType = OvertimeType.Overtime;
                CalendarItemType cType = CalendarItemType.ReclaimedOvertime;


                try
                {
                    parseOldFrom = DateTime.ParseExact(oldFrom, "dd.MM.yyyy", null);
                    parseOldTo = DateTime.ParseExact(oldTo, "dd.MM.yyyy", null);
                    parseNewFrom = DateTime.ParseExact(newFrom, "dd.MM.yyyy", null);
                    parseNewTo = DateTime.ParseExact(newTo, "dd.MM.yyyy", null);
                    if (reclaimDate != null && reclaimDate != String.Empty)
                        parseReclaimDate = DateTime.ParseExact(reclaimDate, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                switch (type)
                {
                    case "ReclaimedOvertime":
                        {
                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.ReclaimedOvertime;
                        }
                        break;
                    case "OvertimeForReclaim":
                        {
                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.OvertimeForReclaim;
                        }
                        break;
                    case "PrivateMinus":
                        {
                            oType = OvertimeType.Private;
                            cType = CalendarItemType.PrivateMinus;
                        }
                        break;
                    default:
                        return false;
                }
                Overtime overtime;
                if (type == "ReclaimedOvertime")
                {
                    overtime = repository.Overtimes.Where(
                                    o => o.EmployeeID == id &&
                                    o.ReclaimDate == parseOldFrom &&
                                    o.Type == oType).FirstOrDefault();
                    if (overtime == null)
                    {
                        Journey journey = repository.Journeys.Where(
                            o => o.JourneyOf.EmployeeID == id &&
                           o.ReclaimDate == parseOldFrom
                           ).FirstOrDefault();
                        if (journey == null)
                        {
                            return false;
                        }

                        if (parseReclaimDate != DateTime.MinValue && journey.Date != parseReclaimDate)
                        {
                            Journey journeytoChange = repository.Journeys.Where(
                                j => j.JourneyOf.EmployeeID == id &&
                                    j.Date == parseReclaimDate &&
                                    j.ReclaimDate == null
                                ).FirstOrDefault();
                            if (journeytoChange != null)
                            {
                                journey.ReclaimDate = null;
                                journeytoChange.ReclaimDate = parseNewFrom;
                                repository.SaveJourney(journey);
                                repository.SaveJourney(journeytoChange);
                            }
                            else
                                return false;
                        }
                        else
                        {
                            journey.ReclaimDate = parseNewFrom;
                            repository.SaveJourney(journey);
                        }
                    }
                    else
                    {
                        if (parseReclaimDate != DateTime.MinValue && overtime.Date != parseReclaimDate)
                        {
                            Overtime overtimetoChange = repository.Overtimes.Where(
                                    o => o.EmployeeID == id &&
                                    o.Date == parseReclaimDate &&
                                    o.ReclaimDate == null &&
                                    o.Type == oType).FirstOrDefault();
                            if (overtimetoChange != null)
                            {
                                overtime.ReclaimDate = null;
                                overtimetoChange.ReclaimDate = parseNewFrom;
                                overtimetoChange.DayOff = true;
                                repository.SaveOvertime(overtime);
                                repository.SaveOvertime(overtimetoChange);
                            }
                            else
                                return false;
                        }
                        else
                        {
                            overtime.ReclaimDate = parseNewFrom;
                            overtime.DayOff = true;
                            repository.SaveOvertime(overtime);
                        }

                    }
                }
                else
                {
                    overtime = (from over in employee.Overtimes
                                where over.Date == parseOldFrom &&
                                    over.Employee == employee &&
                                    over.EmployeeID == employee.EmployeeID &&
                                    over.Type == oType
                                select over).FirstOrDefault();
                    if (overtime != null)
                    {
                        overtime.Date = parseNewFrom;
                        repository.SaveOvertime(overtime);
                    }
                }
                CalendarItem item = (from i in employee.CalendarItems
                                     where i.From == parseOldFrom &&
                                     i.To == parseOldTo &&
                                     i.Type == cType &&
                                     i.EmployeeID == employee.EmployeeID &&
                                     i.Employee == employee
                                     select i).FirstOrDefault();


                if (item != null)
                {
                    item.From = parseNewFrom;
                    item.To = parseNewTo;
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
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
            if (employee != null)
            {
                DateTime parseFrom;
                DateTime parseTo;

                OvertimeType oType = OvertimeType.Overtime;
                CalendarItemType cType = CalendarItemType.ReclaimedOvertime;

                try
                {
                    parseFrom = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseTo = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return false;
                }

                switch (type)
                {
                    case "ReclaimedOvertime":
                        {

                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.ReclaimedOvertime;

                            Overtime reclaimedOvertime = (from over in employee.Overtimes
                                                          where over.ReclaimDate == parseFrom &&
                                                              over.EmployeeID == employee.EmployeeID &&
                                                              over.Type == oType
                                                          select over).FirstOrDefault();
                            if (reclaimedOvertime == null)
                            {
                                Journey journey = repository.Journeys.Where(
                                    o => o.JourneyOf.EmployeeID == id &&
                                        o.ReclaimDate == parseFrom).FirstOrDefault();
                                if (journey == null)
                                {
                                    return false;
                                }
                                journey.ReclaimDate = null;
                                repository.SaveJourney(journey);
                            }
                            else
                            {
                                reclaimedOvertime.ReclaimDate = null;
                                reclaimedOvertime.DayOff = false;
                                repository.SaveOvertime(reclaimedOvertime);
                            }
                            CalendarItem reclaimedItem = employee.CalendarItems.Where(
                                c =>
                                c.EmployeeID == employee.EmployeeID &&
                                c.From == parseFrom &&
                                c.To == parseTo &&
                                c.Type == cType).FirstOrDefault();
                            if (reclaimedItem != null)
                            {
                                employee.CalendarItems.Remove(reclaimedItem);
                                repository.DeleteCalendarItem(reclaimedItem.CalendarItemID);
                            }
                            return true;
                        }
                    case "OvertimeForReclaim":
                        {
                            oType = OvertimeType.Paid;
                            cType = CalendarItemType.OvertimeForReclaim;
                        }
                        break;
                    case "PrivateMinus":
                        {
                            oType = OvertimeType.Private;
                            cType = CalendarItemType.PrivateMinus;
                        }
                        break;
                    default:
                        return false;
                }

                Overtime overtime = (from over in employee.Overtimes
                                     where over.Date == parseFrom &&
                                         over.Employee == employee &&
                                         over.EmployeeID == employee.EmployeeID &&
                                         over.Type == oType
                                     select over).FirstOrDefault();
                if (overtime != null)
                {
                    CalendarItem reclaimedItemForOvertime = employee.CalendarItems.Where(
                    c =>
                    c.EmployeeID == employee.EmployeeID &&
                    c.From == overtime.ReclaimDate &&
                    c.To == overtime.ReclaimDate &&
                    c.Type == CalendarItemType.ReclaimedOvertime).FirstOrDefault();
                    if (reclaimedItemForOvertime != null)
                    {
                        employee.CalendarItems.Remove(reclaimedItemForOvertime);
                        repository.DeleteCalendarItem(reclaimedItemForOvertime.CalendarItemID);
                    }
                }

                CalendarItem item = (from i in employee.CalendarItems
                                     where //i.Employee == employee &&
                                     i.EmployeeID == employee.EmployeeID &&
                                     i.From == parseFrom &&
                                     i.To == parseTo &&
                                     i.Type == cType
                                     select i).FirstOrDefault();

                if (overtime != null && item != null)
                {
                    repository.DeleteOvertime(overtime.OvertimeID);
                    employee.CalendarItems.Remove(item);
                    repository.DeleteCalendarItem(item.CalendarItemID);

                }
                return true;
            }
            return false;
        }


        public List<DateTime> SearchOvertimeDataForOneEmp(int eID)
        {
            var employee = repository.Employees.Where(e => e.EmployeeID == eID).FirstOrDefault();

            List<DateTime> over = new List<DateTime>();

            foreach (Overtime overtime in employee.Overtimes.Where(o => o.ReclaimDate == null && o.Type == OvertimeType.Paid))
            {
                over.Add(overtime.Date);
            }
            return over;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GetReclaimedOvertime(int id, string from, string to)
        {
            DateTime parseFrom, parseTo; 
            try
                {
                    parseFrom = DateTime.ParseExact(from, "dd.MM.yyyy", null);
                    parseTo = DateTime.ParseExact(to, "dd.MM.yyyy", null);
                }
                catch (SystemException)
                {
                    return new HttpNotFoundResult();
                }
                Employee employee = (from emp in repository.Employees where emp.EmployeeID == id select emp).FirstOrDefault();
                if(employee!=null)
                {
                    CalendarItem reclaimedItemForOvertime = null;
                     Overtime overtime = (from over in employee.Overtimes
                                     where over.Date == parseFrom &&
                                         over.Employee == employee &&
                                         over.EmployeeID == employee.EmployeeID &&
                                         over.Type == OvertimeType.Paid
                                     select over).FirstOrDefault();
                    if(overtime!=null)
                    {

                                 reclaimedItemForOvertime = employee.CalendarItems.Where(
                    c =>
                    c.EmployeeID == employee.EmployeeID &&
                    c.From == overtime.ReclaimDate &&
                    c.To == overtime.ReclaimDate &&
                    c.Type == CalendarItemType.ReclaimedOvertime).FirstOrDefault();
                        }
                    if(reclaimedItemForOvertime!=null)
                    { 
                return Json(new 
                {
                    id = employee.EmployeeID, 
                    from = reclaimedItemForOvertime.From, 
                    to = reclaimedItemForOvertime.To
                }
                );
                }
                }
                return new HttpNotFoundResult();
        }

    }
}