using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using AjourBT.Domain.Infrastructure;
using System.Text;
using System.Web.Configuration;
using ExcelLibrary.SpreadSheet;
using System.IO;
using AjourBT.Helpers;
using AjourBT.Infrastructure;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "VU")]
    public class VUController : Controller
    {
        private IRepository repository;
        private IXLSExporter xlsExporter; 
        
        private string defaultAccComment;

        public VUController(IRepository repository, IXLSExporter xlsExporter)
        {
            this.repository = repository;
            this.xlsExporter = xlsExporter; 

            StringBuilder comment = new StringBuilder();
            comment = comment.Append("ВКО №   від   , cума:   UAH.");
            comment = comment.AppendLine();
            comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }

        [Authorize(Roles = "VU")]
        public ActionResult ShowBTInformation(int id = 0)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            ViewBag.DefaultAccComment = defaultAccComment;
            return View(businessTrip);
        }

        #region BTs by Dates/Location tab

        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripByDatesVU(int selectedYear = 0)
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

            return View(selectedYear);
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetBusinessTripDataByDatesVU(int selectedYear = 0)
        {
            var query = from bt in repository.BusinessTrips.AsEnumerable()
                        join emp in repository.Employees on bt.EmployeeID equals emp.EmployeeID
                        join loc in repository.Locations on bt.LocationID equals loc.LocationID
                        where (bt.StartDate.Year == selectedYear
                              && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                        orderby emp.DateDismissed,emp.LastName, bt.StartDate
                        select new BusinessTripViewModel(bt);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(query.ToList());
        }
        
        #endregion

        #region BTs by Quarters tab

        [Authorize(Roles = "VU")]
        public ActionResult GetListOfYearsForQuarterVU(int selectedKey, string selectedDepartment = "", string searchString ="")
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            Dictionary<int, string> values = new Dictionary<int, string>();

            values.Add(0, "current month");
            values.Add(1, "last month(till today)");
            values.Add(3, "last 3 monthes(till today)");
            values.Add(6, "last 6 monthes(till today)");
            for (int i = 0; i < selected.AsEnumerable().Count(); i++)
            {
                values.Add(selected.AsEnumerable().ToArray()[i].Year, selected.AsEnumerable().ToArray()[i].Year.ToString());
            }
            ViewBag.SelectedValues = new SelectList(values, "Key", "Value");
            ViewBag.SearchString = searchString;

            return View(selectedKey);
        }

        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripDataInQuarterVU(int selectedKey, string selectedDepartment = "", string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            DateTime currentDate = DateTime.Now.ToLocalTimeAzure().Date;
            DateTime selectedStartPeriod = currentDate;
            DateTime selectedStartPeriod2 = currentDate;
            string viewName = "";
            List<int> monthes = new List<int>();
            List<int> years = new List<int>();
            switch (selectedKey)
            {
                case 0:
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 1:
                    selectedStartPeriod = currentDate.AddMonths(-1);
                    selectedStartPeriod2 = currentDate.AddMonths(-1);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 3:
                    selectedStartPeriod = currentDate.AddMonths(-3);
                    selectedStartPeriod2 = currentDate.AddMonths(-3);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 6:
                    selectedStartPeriod = currentDate.AddMonths(-6);
                    selectedStartPeriod2 = currentDate.AddMonths(-6);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                default:
                    monthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    selectedStartPeriod = new DateTime(selectedKey, 1, 1);
                    break;
            }

            if (selectedKey < 7)
            {
                //for (int i = 0; i <= ((selectedStartPeriod2.Month + selectedKey) - selectedStartPeriod2.Month); i++)
                for (int i = 0; i <= selectedKey; i++)
                {
                    monthes.Add(selectedStartPeriod2.Month);
                    selectedStartPeriod2 = selectedStartPeriod2.AddMonths(1);
                }

            }
            ViewBag.MonthList = monthes;
            ViewBag.SelectedKey = selectedKey;
            ViewBag.SearchString = searchString;


            var employeeGroups = from e in repository.Employees
                                 where ((selectedDepartment == String.Empty ||
                                        e.Department.DepartmentName == selectedDepartment)
                                        && (e.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                            e.LastName.ToLower().Contains(searchString.ToLower()) ||
                                            e.EID.ToLower().Contains(searchString.ToLower())))


                                 orderby e.DateDismissed, e.LastName
                                 select new
                                 {
                                     e.LastName,
                                     e.FirstName,
                                     e.EID,
                                     e.IsManager,
                                     e.DateDismissed,
                                     MonthGroups = from bt in e.BusinessTrips
                                                   where ((
                                                        (bt.StartDate.Year == selectedKey && selectedKey >= 7) ||
                                                            (selectedKey < 7 &&
                                                                ((bt.StartDate.Month >= selectedStartPeriod.Month && bt.StartDate.Year == selectedStartPeriod.Year && bt.StartDate <= currentDate)
                                                                || (bt.StartDate.Month < selectedStartPeriod2.Month && bt.StartDate.Year == selectedStartPeriod2.Year && bt.StartDate <= currentDate))))
                                                        && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported) || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled))
                                                        )
                                                   group bt by bt.StartDate.Month into MonthGroup
                                                   select new { Month = MonthGroup.Key, Bts = MonthGroup }
                                 };


            List<EmployeeViewModelForVU> employeesBTsByMonthList = new List<EmployeeViewModelForVU>();

            foreach (var emp in employeeGroups)
            {
                EmployeeViewModelForVU employee = new EmployeeViewModelForVU();

                employee.LastName = emp.LastName;
                employee.FirstName = emp.FirstName;
                employee.EID = emp.EID;
                employee.IsManager = emp.IsManager;
                employee.DateDismissed = String.Format("{0:d}", emp.DateDismissed);
                employee.BusinessTripsByMonth = new Dictionary<int, List<BusinessTrip>>();
                employee.DaysUsedInBt = 0;

                foreach (var month in emp.MonthGroups)
                {
                    List<BusinessTrip> monthBTs = new List<BusinessTrip>();
                    int days = 0;

                    foreach (var bt in month.Bts.Where(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                    {
                        days = (bt.EndDate - bt.StartDate).Days + 1;
                        employee.DaysUsedInBt += days;
                        monthBTs.Add(bt);
                    }

                    employee.BusinessTripsByMonth.Add(month.Month, monthBTs);
                }

                employeesBTsByMonthList.Add(employee);

            }
            return View(viewName, employeesBTsByMonthList);
        }

        public ActionResult GetListOfDepartmentsVU(int selectedKey, string selectedDepartment = null)
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;

            return View();
        }
        
        #endregion

        #region "BTs in preparation process" tab

        [Authorize(Roles = "VU")]
        public PartialViewResult GetPrepBusinessTripVU(string searchString = "")
        {

            ViewBag.SearchString = searchString;

            return PartialView();
        }

        [Authorize(Roles = "VU")]
        public ViewResult GetPrepBusinessTripDataVU(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            List<Employee> resultEmps = SearchPrepBusinessTripDataVU(repository.Employees.ToList(), searchString);
            Dictionary<Employee, List<BusinessTrip>> empDictonary = new Dictionary<Employee, List<BusinessTrip>>();

            foreach (Employee currentEmp in resultEmps)
            {
                List<BusinessTrip> tempBTList = repository.BusinessTrips
                    .Where(bt => bt.EmployeeID == currentEmp.EmployeeID
                           && ((bt.Status != BTStatus.Planned)
                           && bt.Status != (BTStatus.Planned | BTStatus.Modified)
                           && bt.Status != (BTStatus.Planned | BTStatus.Cancelled)))
                    .OrderBy(date => date.StartDate)
                    .ToList();

                ViewBag.SearchString = searchString;
                ViewBag.JSDatePatters = MvcApplication.JSDatePattern;

                if (tempBTList.Count > 0)
                {
                    empDictonary.Add(currentEmp, tempBTList);
                }
            }

            return View(empDictonary);
        }

        public List<Employee> SearchPrepBusinessTripDataVU(List<Employee> employees, string searchString)
        {
            List<Employee> emps = repository.Employees.Where(e => e.IsUserOnly == false && e.BusinessTrips.Count > 0 && (e.EID.ToLower().Contains(searchString.ToLower())
                                                             || e.LastName.ToLower().Contains(searchString.ToLower())
                                                             || e.FirstName.ToLower().Contains(searchString.ToLower())))
                     //.OrderByDescending(e => e.IsManager)
                     .OrderBy(e => e.DateDismissed)
                     //.ThenBy(e => e.DateDismissed)
                     .ThenBy(e => e.LastName)
                     .ToList();

            return emps;
        }
        
        #endregion

        #region Private Trips tab

        [Authorize(Roles = "VU")]
        public ViewResult GetPrivateTripDataVU(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selected = repository.Employees.ToList(); ;

            selected = SearchPrivateTripData(selected, searchString);
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return View(selected.ToList());
        }

        [Authorize(Roles = "VU")]
        public ActionResult GetPrivateTripVU(string searchString = "")
        {
            return View((object)searchString);
        }

        //TODO: duplicated in BTM and PrivateTrip controllers
        public List<Employee> SearchPrivateTripData(List<Employee> empList, string searchString)
        {
            List<Employee> selected = (from emp in empList
                                       where emp.DateDismissed == null
                                            && (emp.EID.ToLower().Contains(searchString.ToLower())
                                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                            || emp.LastName.ToLower().Contains(searchString.ToLower()))
                                       orderby emp.IsManager descending, emp.LastName
                                       select emp).ToList();
            return selected;

        }

        #endregion

        #region 'BTs by Units' tab
        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripByUnitsVU(int selectedYear = 0)
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

            return View(selectedYear);
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetBusinessTripDataByUnitsVU(int selectedYear = 0)
        {
            var query = repository.GetBusinessTripDataByUnits(selectedYear);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(query);
        }

        [Authorize(Roles = "VU")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExportBusinessTripByUnitsToExcel(int selectedYear)
                {
            IList<AjourBT.Domain.ViewModels.BusinessTripViewModel> businessTrips = repository.GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(selectedYear);
            return File(xlsExporter.ExportBusinessTripsToExcelVU(businessTrips).ToArray(), "application/vnd.ms-excel", "BusinessTripsByUnits.xls");
                }

        #endregion

        #region Visas and Permits tab

        [Authorize(Roles = "VU")]
        public ActionResult GetVisaVU(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetVisaDataVU(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            IList<Employee>selected = repository.SearchVisaData(searchString);
            ViewBag.SearchString = searchString;
            return PartialView(selected);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "VU")]
        public ActionResult ExportVisasAndPermits(string searchString)
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            IList<Employee> empsWithVisasAndPermits = repository.SearchVisaData(searchString); 
            return File(xlsExporter.ExportVisasAndPermitsVU(empsWithVisasAndPermits), "application/vnd.ms-excel", "VisasAndPermits.xls");
                }


        #endregion

        #region Employees tab

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "VU")]
        private List<EmployeeViewModel> PrepareEmployeeData(string selectedDepartment, string searchString = "")
        {
            searchString = (searchString!= null && searchString != "") ? searchString.Trim() : "";
            List<EmployeeViewModel> data = SearchEmployeeData(repository.Users.ToList(), selectedDepartment, searchString);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return data;
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "VU")]
        private void PrepareGetEmployeeViewBags(string selectedDepartment, string searchString)
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
        }

        [Authorize(Roles = "VU")]
        public ActionResult GetEmployeeReadOnlyVU(string selectedDepartment = null, string searchString = "")
        {
            PrepareGetEmployeeViewBags(selectedDepartment, searchString);
            return View();
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetEmployeeDataReadOnlyVU(string selectedDepartment = null, string searchString = "")
        {
            List<EmployeeViewModel> data = PrepareEmployeeData(selectedDepartment, searchString);
            ViewBag.empsByPositions = GetEmployeesByPositions(selectedDepartment); 
            return PartialView(data);
        }

        [Authorize(Roles = "VU")]
        public Dictionary<string, int> GetEmployeesByPositions(string selectedDepartment ="")
        {
            Dictionary<string, int> empsByPositions = new Dictionary<string, int>();
            int empsCount;
            foreach (Position pos in repository.Positions)
            {
                empsCount = repository.Employees.Where(e => e.Position.PositionID == pos.PositionID && (e.DateDismissed == null) &&
                    (selectedDepartment == null || selectedDepartment == String.Empty || (e.Department.DepartmentName == selectedDepartment))
                    ).Count();
                empsByPositions.Add(pos.TitleEn, empsCount);
            }
            return empsByPositions;
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        public List<EmployeeViewModel> SearchEmployeeData(List<Employee> empList, string selectedDepartment, string searchString)
        {
            List<EmployeeViewModel> data = new List<EmployeeViewModel>();
            data = (from emp in empList
                    where ((selectedDepartment == null || selectedDepartment == String.Empty || (emp.Department != null && emp.Department.DepartmentName == selectedDepartment))
                            && (emp.EID.ToLower().Contains(searchString.ToLower())
                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                            || ((emp.DateEmployed != null) && emp.DateEmployed.Value != null && emp.DateEmployed.Value.ToShortDateString().Contains(searchString))
                            || ((emp.DateDismissed != null) && emp.DateDismissed.Value != null && emp.DateDismissed.Value.ToString().Contains(searchString))
                            || ((emp.BirthDay != null) && emp.BirthDay.Value != null && emp.BirthDay.Value.ToString().Contains(searchString))
                            || ((emp.FullNameUk != null) && emp.FullNameUk.ToLower().Contains(searchString.ToLower()))
                            || ((emp.Position != null) && emp.Position.TitleEn.ToLower().Contains(searchString.ToLower()))
                            ||
                                  ((System.Web.Security.Membership.GetUser(emp.EID) != null)
                                  && System.Web.Security.Roles.GetRolesForUser(emp.EID) != null && String.Join(", ", System.Web.Security.Roles.GetRolesForUser(emp.EID)).ToLower().Contains(searchString.ToLower()))))
                    orderby emp.IsManager descending, emp.LastName
                    select new EmployeeViewModel(emp)).ToList();

            return data;

        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "VU")]
        public SelectList DepartmentsDropDownList()
        {
            var depL = from rep in repository.Departments
                       orderby rep.DepartmentName
                       select rep;

            return new SelectList(depL, "DepartmentID", "DepartmentName");
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "PU,VU,ADM")]
        public SelectList PositionsDropDownList()
        {
            var posList = from pos in repository.Positions
                          orderby pos.TitleEn
                          select pos;

            return new SelectList(posList, "PositionID", "TitleEn");
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "PU,VU,ADM")]
        public SelectList DropDownListWithSelectedDepartment(string selectedDepartment)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            foreach (Department department in repository.Departments)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = department.DepartmentName,
                    Value = department.DepartmentID.ToString(),
                    Selected = department.DepartmentName.ToString() == selectedDepartment ? true : false

                };

                selectListItems.Add(selectListItem);
            }

            var allDepartments = from rep in selectListItems.AsEnumerable().OrderBy(m => m.Text)
                                 orderby rep.Selected == true descending
                                 select rep;

            var id = from rep in selectListItems
                     where rep.Selected == true
                     select rep.Value;

            return new SelectList(allDepartments, "Value", "Text", id);
        }

        [Authorize(Roles = "VU")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExportEmployeesToExcelVU(string selectedDepartment)
        {
            IList<AjourBT.Domain.ViewModels.EmployeeViewModel> employeeData = repository.SearchUsersData(selectedDepartment);
            return File(xlsExporter.ExportEmployeesToExcelVU(employeeData), "application/vnd.ms-excel", "Employees.xls");
        }

#endregion
    }
}