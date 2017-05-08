using AjourBT.Domain.Abstract;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


namespace AjourBT.Controllers
{
    [Authorize(Roles = "PU")]
    public class PUController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";
        private string btCreationError = "Absence already planned on this period for this user. "
                              + "Please change OrderDates or if BT haven\'t OrderDates "
                              + "change \'From\' or \'To\'";

        public PUController(IRepository repository, IMessenger messenger)
        {
            this.repository = repository;
            this.messenger = messenger;
        }


        #region Unit
        //
        // GET: /PU/UnitIndex

        public ViewResult UnitIndex()
        {
            return View(repository.Units.ToList());
        }

        public ViewResult UnitCreate()
        {
            return View();
        }


        // POST: /PU/UnitCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnitCreate(Unit unit)
        {
            if (repository.Units.Where(u => u.Title == unit.Title).FirstOrDefault() != null)
            {
                ModelState.AddModelError("Title", "Unit with same Title already exists");
            }
            if (repository.Units.Where(u => u.ShortTitle == unit.ShortTitle).FirstOrDefault() != null)
            {
                ModelState.AddModelError("ShortTitle", "Unit with same ShortTitle already exists");
            }
            if (ModelState.IsValid)
            {
                repository.SaveUnit(unit);
            }
            else
            {
                return View(unit);
            }

            List<Unit> units = repository.Units.ToList();
            return View("UnitIndex", units);
        }

        //
        // GET: /PU/UnitEdit/5

        public ActionResult UnitEdit(int id = 0)
        {
            Unit unit = (from un in repository.Units where un.UnitID == id select un).FirstOrDefault();

            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        //
        // POST: /PU/UnitEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnitEdit(Unit unit)
        {
            string modelError = "";
            try
            {
                if (repository.Units.Where(u => u.Title == unit.Title && u.UnitID != unit.UnitID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("Title", "Unit with same Title already exists");
                }
                if (repository.Units.Where(u => u.ShortTitle == unit.ShortTitle && u.UnitID != unit.UnitID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("ShortTitle", "Unit with same ShortTitle already exists");
                }
                if (ModelState.IsValid)
                {
                    repository.SaveUnit(unit);
                }
                else
                {
                    return View(unit);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";

                return Json(new { error = modelError });
            }

            List<Unit> units = repository.Units.ToList();
            return View("UnitIndex", units);
        }

        //
        // GET: /PU/UnitDelete/5

        public ActionResult UnitDelete(int id = 0)
        {
            Unit unit = (from un in repository.Units where un.UnitID == id select un).FirstOrDefault();
            if (unit == null)
            {
                return HttpNotFound();
            }

            if (unit.BusinessTrips.Count != 0)
            {
                return View("UnitCannotDelete");
            }
            else
                return View(unit);
        }

        //
        // POST: /PU/UnitDelete/5

        [HttpPost, ActionName("UnitDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult UnitDeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteUnit(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Unit> units = repository.Units.ToList();
            return View("UnitIndex", units);
        }

        #endregion

        #region Department
        //
        // GET: /PU/DepartmentIndex

        public ViewResult DepartmentIndex()
        {
            return View(repository.Departments);
        }

        //
        // GET: /PU/DepartmentCreate

        public ActionResult DepartmentCreate()
        {
            return View();
        }

        //
        // POST: /PU/DepartmentCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentCreate(Department department)
        {
            if (repository.Departments.Where(d => d.DepartmentName == department.DepartmentName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("DepartmentName", "Department with same Name already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveDepartment(department);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }
            }
            else
            {
                return View(department);
            }

            List<Department> departments = repository.Departments.ToList();
            return View("DepartmentIndex", departments);
        }

        //
        // GET: /PU/DepartmentEdit/5

        public ActionResult DepartmentEdit(int id = 0)
        {
            Department department = repository.Departments.FirstOrDefault(d => d.DepartmentID == id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /PU/DepartmentEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentEdit(Department department)
        {
            string modelError = "";
            try
            {
                if (repository.Departments.Where(d => d.DepartmentName == department.DepartmentName && d.DepartmentID != department.DepartmentID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("DepartmentName", "Department with same Name already exists");
                }
                if (ModelState.IsValid)
                {
                    repository.SaveDepartment(department);
                }
                else
                {
                    return View(department);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

                return Json(new { error = modelError });
            }

            List<Department> departments = repository.Departments.ToList();
            return View("DepartmentIndex", departments);
        }

        //
        // GET: /PU/DepartmentDelete/5
        public ActionResult DepartmentDelete(int id = 0)
        {
            Department department = repository.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();

            if (department == null)
            {
                return HttpNotFound();
            }

            if (department.Employees.Count != 0)
            {
                return View("DepartmentCannotDelete");
            }
            else
                return View(department);
        }

        //
        // POST: /PU/DepartmentDelete/5

        [HttpPost, ActionName("DepartmentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmentDeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteDepartment(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Department> departments = repository.Departments.ToList();
            return View("DepartmentIndex", departments);
        }


        #endregion

        #region Position
        // GET: /PU/PositionIndex

        public ViewResult PositionIndex()
        {
            return View(repository.Positions.ToList());
        }

        //
        // GET: /PU/PositionCreate

        public ViewResult PositionCreate()
        {
            return View();
        }

        //
        // POST: /PU/PositionCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositionCreate(Position position)
        {
            if (repository.Positions.Where(p => p.TitleEn == position.TitleEn).FirstOrDefault() != null)
            {
                ModelState.AddModelError("TitleEn", "Position with same TitleEn already exists");
            }
            if (repository.Positions.Where(p => p.TitleUk == position.TitleUk).FirstOrDefault() != null)
            {
                ModelState.AddModelError("TitleUk", "Position with same TitleUk already exists");
            }
            if (ModelState.IsValid)
            {
                repository.SavePosition(position);
            }
            else
            {
                return View(position);
            }

            List<Position> positions = repository.Positions.ToList();

            return View("PositionIndex", positions);
        }


        //GET: /PU/PositionEdit/

        public ActionResult PositionEdit(int id = 0)
        {
            Position position = (from p in repository.Positions where p.PositionID == id select p).FirstOrDefault();
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        //
        // POST: /PU/PositionEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PositionEdit(Position position)
        {
            string modelError = "";
            try
            {
                if (repository.Positions.Where(p => p.TitleEn == position.TitleEn && p.PositionID != position.PositionID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("TitleEn", "Position with same TitleEn already exists");
                }
                if (repository.Positions.Where(p => p.TitleUk == position.TitleUk && p.PositionID != position.PositionID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("TitleUk", "Position with same TitleUk already exists");
                }
                if (ModelState.IsValid)
                {
                    repository.SavePosition(position);
                }
                else
                {
                    return View(position);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";

                return Json(new { error = modelError });
            }

            List<Position> positions = repository.Positions.ToList();
            return View("PositionIndex", positions);
        }

        //
        // GET: /PU/PositionDelete/

        public ActionResult PositionDelete(int id = 0)
        {
            Position position = (from p in repository.Positions where p.PositionID == id select p).FirstOrDefault();
            if (position == null)
            {
                return HttpNotFound();
            }

            if (position.Employees.Count != 0)
            {
                return View("PositionCannotDelete");
            }
            else
                return View(position);
        }

        //
        // POST: /PU/PositionDelete/5

        [HttpPost, ActionName("PositionDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PositionDeleteConfirmed(int id)
        {
            try
            {
                repository.DeletePosition(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Position> positions = repository.Positions.ToList();

            return View("PositionIndex", positions);
        }


        #endregion

        #region Employee
        [Authorize(Roles = "PU")]
        public ActionResult GetEmployee(string selectedDepartment = null, string searchString = "")
        {
            PrepareGetEmployeeViewBags(selectedDepartment, searchString);
            return View();
        }

        [Authorize(Roles = "PU")]
        public PartialViewResult GetEmployeeData(string selectedDepartment = null, string searchString = "")
        {
            List<EmployeeViewModel> data = PrepareEmployeeData(selectedDepartment, searchString);

            return PartialView(data.Where(emp => emp.IsUserOnly == false));
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        private List<EmployeeViewModel> PrepareEmployeeData(string selectedDepartment, string searchString = "")
        {
            searchString = (searchString != null && searchString != "") ? searchString.Trim() : "";
            List<EmployeeViewModel> data = SearchEmployeeData(repository.Users.ToList(), selectedDepartment, searchString);
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return data;
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
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
        public SelectList DepartmentsDropDownList()
        {
            var depL = from rep in repository.Departments
                       orderby rep.DepartmentName
                       select rep;

            return new SelectList(depL, "DepartmentID", "DepartmentName");
        }

        public SelectList EducationDropDownList()
        {
            var edu = Enum.GetValues(typeof(EducationType))
    .Cast<EducationType>()
    .Select(v => new {ID = v, Name = v.Description()})
    .ToList();

            return new SelectList(edu, "ID", "Name"); 
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        public SelectList PositionsDropDownList()
        {
            var posList = from pos in repository.Positions
                          orderby pos.TitleEn
                          select pos;

            return new SelectList(posList, "PositionID", "TitleEn");
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
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

        [Authorize(Roles = "PU")]
        public ViewResult EmployeeCreate(string selectedDepartment = null, string searchString = "")
        {

            ViewBag.PositionsList = PositionsDropDownList();
            ViewBag.DepartmentsList = DropDownListWithSelectedDepartment(selectedDepartment);
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.EducationDropDownList = EducationDropDownList();
            return View();
        }

        [Authorize(Roles = "PU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeCreate(Employee emp, string[] Roles = null, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (ModelState.IsValid)
            {
                if (repository.Users.Where(e => e.EID == emp.EID).FirstOrDefault() == null && System.Web.Security.Membership.GetUser(emp.EID) == null)
                {
                    repository.SaveEmployee(emp);
                    repository.SaveRolesForEmployee(emp.EID, Roles);
                    return RedirectToAction("PUView", "Home", new { tab = Tabs.PU.Employees, selectedDepartment = selectedDepartment, SearchString = searchString });
                }
                else
                {
                    return Json(new { error = "Employee with EID " + emp.EID + " already exists" });
                }
            }

            EmployeeViewModel employee = new EmployeeViewModel(emp);

            ViewBag.DepartmentsList = DepartmentsDropDownList();
            ViewBag.PositionsList = PositionsDropDownList();
            ViewBag.EducationDropDownList = EducationDropDownList();

            return View(employee);
        }

        [Authorize(Roles = "PU")]
        public ActionResult EmployeeEdit(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.DepartmentList = (from d in repository.Departments select d).ToList();
            ViewBag.PositionList = (from p in repository.Positions select p).ToList();

            ViewBag.EducationDropDownList = EducationDropDownList();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee emp = repository.Users.FirstOrDefault(e => e.EmployeeID == id);

            if (emp == null)
            {
                return HttpNotFound();
            }

            EmployeeViewModel employee = new EmployeeViewModel(emp);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString; 
            return View(employee);
        }

        [Authorize(Roles = "PU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeEdit(Employee emp, string[] Roles = null, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.DepartmentList = (from d in repository.Departments select d).ToList();
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.PositionList = (from p in repository.Positions select p).ToList();
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.EducationDropDownList = EducationDropDownList();
            ViewBag.SearchString = searchString;

            string modelError = "";

            try
            {
                if (ModelState.IsValid)
                {
                    repository.SaveEmployee(emp);
                    repository.SaveRolesForEmployee(emp.EID, Roles);
                    List<Employee> empl = repository.Users.ToList();
                    List<EmployeeViewModel> empList = SearchEmployeeData(empl, selectedDepartment, searchString);
                    return View("OneRowPU", empList);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            }
            //EmployeeViewModel employee = new EmployeeViewModel(emp);
            //return View(employee);
            return Json(new { error = modelError });
        }

        [Authorize(Roles = "PU")]
        public ActionResult EmployeeDelete(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            Employee employee = repository.Users.FirstOrDefault(e => e.EmployeeID == id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            if ((employee.BusinessTrips != null && employee.BusinessTrips.Count != 0) || employee.Visa != null || employee.Permit != null || employee.VisaRegistrationDate != null || employee.Passport != null)
            {
                ViewBag.SelectedDepartment = selectedDepartment;
                ViewBag.SearchString = searchString;
                ViewBag.EmployeeID = id;
                return View("EmployeeCannotDelete");
            }
            else
                ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(employee);
        }

        [Authorize(Roles = "PU")]
        [HttpPost, ActionName("EmployeeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeDeleteConfirmed(int id, string selectedDepartment = null, string searchString = "")
        {
            try
            {
                repository.DeleteUser(repository.Users.FirstOrDefault(e => e.EmployeeID == id).EID);
                repository.DeleteEmployee(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Employee> empl = repository.Users.ToList();
            List<EmployeeViewModel> empList = SearchEmployeeData(empl, selectedDepartment, searchString);
            return View("OneRowPU", empList);

        }

        [Authorize(Roles = "PU")]
        [HttpGet]
        public ViewResult ResetPassword(string EID, string[] Roles)
        {
            return View();
        }

        [Authorize(Roles = "PU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string ResetPasswordConfirmed(string EID, string[] Roles)
        {
            repository.SaveRolesForEmployee(EID, null);
            repository.SaveRolesForEmployee(EID, Roles);
            return "Password has been changed";
        }

        #endregion

        #region Location
        //
        // GET: /PU/LocationIndex

        public ViewResult LocationIndex()
        {
            return View(repository.Locations.ToList());
        }

        public SelectList CountriesDropDownList()
        {
            var countryList = from rep in repository.Countries
                              orderby rep.CountryName
                              select rep;

            return new SelectList(countryList, "CountryID", "CountryName");
        }

        //
        // GET: /PU/LocationCreate

        public ViewResult LocationCreate()
        {
            ViewBag.CountryList = CountriesDropDownList();
            return View();
        }

        //
        // POST: /PU/LocationCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocationCreate(Location location)
        {
            if (repository.Locations.Where(l => l.Title == location.Title).FirstOrDefault() != null)
            {
                ModelState.AddModelError("Title", "Location with same Title already exists");
            }
            bool isExistingID = IsExistingEID(location.ResponsibleForLoc);
            if (isExistingID == false)
            {
                ModelState.AddModelError("ResponsibleForLoc", "Not existing Employee's EID");
            }
            if (ModelState.IsValid)
            {
                repository.SaveLocation(location);
            }
            else
            {
                ViewBag.CountryList = CountriesDropDownList();
                return View(location);
            }

            ViewBag.CountryList = CountriesDropDownList();

            List<Location> locations = repository.Locations.ToList();
            return View("LocationIndex", locations);
        }

        public bool IsExistingEID(string responsibleForLoc)
        {
            if (String.IsNullOrEmpty(responsibleForLoc))
            {
                return true;
            }

            string[] ids = Regex.Split(responsibleForLoc, @"\W+");
            foreach (string id in ids)
            {
                if (id != "")
                {
                    var result = (from e in repository.Users where e.EID == id select e).FirstOrDefault();
                    if (result == null)
                        return false;
                }
            }
            return true;
        }



        //
        // GET: /PU/LocationEdit/5

        public ActionResult LocationEdit(int id = 0)
        {
            Location location = (from loc in repository.Locations where loc.LocationID == id select loc).FirstOrDefault();
            ViewBag.CountryList = (from c in repository.Countries select c).ToList();

            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        //
        // POST: /PU/LocationEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocationEdit(Location location)
        {
            ViewBag.CountryList = (from c in repository.Countries select c).ToList();

            string modelError = "";
            try
            {
                if (repository.Locations.Where(l => l.Title == location.Title && l.LocationID != location.LocationID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("Title", "Location with same Title already exists");
                }

                bool isExistingID = IsExistingEID(location.ResponsibleForLoc);
                if (isExistingID == false)
                {
                    ModelState.AddModelError("ResponsibleForLoc", "Not existing Employee's EID");
                }

                if (ModelState.IsValid)
                {
                    repository.SaveLocation(location);
                }
                else
                {
                    return View(location);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";

                return Json(new { error = modelError });

            }
            List<Location> locations = repository.Locations.ToList();
            return View("LocationIndex", locations);
        }

        // GET: /PU/LocationDelete/5

        public ActionResult LocationDelete(int id = 0)
        {
            Location location = (from loc in repository.Locations where loc.LocationID == id select loc).FirstOrDefault();
            if (location == null)
            {
                return HttpNotFound();
            }

            if (location.BusinessTrips.Count != 0)
            {
                return View("LocationCannotDelete");

            }
            else
                return View(location);
        }

        //
        // POST: /PU/LocationDelete/5

        [HttpPost, ActionName("LocationDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LocationDeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteLocation(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Location> locations = repository.Locations.ToList();
            return View("LocationIndex", locations);
        }


        #endregion

        #region Users

        [Authorize(Roles = "PU")]
        public ViewResult GetUsersOnly(string selectedDepartment = null, string searchString = "")
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return View();
        }

        [Authorize(Roles = "PU")]
        public PartialViewResult GetUsersOnlyData(string selectedDepartment = null, string searchString = "")
        {
            List<EmployeeViewModel> usersOnlyData = SearchUserOnlyData(repository.Users.ToList(), selectedDepartment, searchString);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return PartialView(usersOnlyData);
        }

        [Authorize(Roles = "PU")]
        public List<EmployeeViewModel> SearchUserOnlyData(List<Employee> empList, string selectedDepartment, string searchString)
        {
            List<EmployeeViewModel> data = new List<EmployeeViewModel>();
            data = (from emp in empList
                    where ((selectedDepartment == null || selectedDepartment == String.Empty || (emp.Department != null && emp.Department.DepartmentName == selectedDepartment))
                            && (emp.IsUserOnly)
                            && (emp.EID.ToLower().Contains(searchString.ToLower())
                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                            ||
                             ((System.Web.Security.Membership.GetUser(emp.EID) != null)
                                  && System.Web.Security.Roles.GetRolesForUser(emp.EID) != null && String.Join(", ", System.Web.Security.Roles.GetRolesForUser(emp.EID)).ToLower().Contains(searchString.ToLower()))))
                    orderby emp.IsManager descending, emp.LastName
                    select new EmployeeViewModel(emp)).ToList();

            return data;
        }

        [Authorize(Roles = "PU")]
        public ActionResult UserCreate(string selectedDepartment = null, string searchString = "")
        {
            ViewBag.PositionsList = PositionsDropDownList();
            ViewBag.DepartmentsList = DropDownListWithSelectedDepartment(selectedDepartment);
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View();
        }



        #endregion

        //TODO: this whole region is duplicated in ADM controller
        #region GetMails

        [Authorize(Roles = "PU")]
        public ActionResult GetMailAliasEMails(string selectedDepartment = "")
        {
            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "PU")]
        [HttpPost]
        public ActionResult GetMailToLinkWithBcc(string selectedDepartment = "")
        {
            string user = HttpContext.User.Identity.Name;
            ViewBag.User = user;

            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = ("PU"))]
        [HttpPost]
        public ActionResult GetSecondMailToLinkWithBcc(string selectedDepartment = "")
        {
            string user = HttpContext.User.Identity.Name;
            ViewBag.User = user;

            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "PU")]
        public ActionResult GetSecondMailAliasEMails(string selectedDepartment = "")
        {
            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }
        #endregion

        #region BTs by Dates/Location tab

        [Authorize(Roles = "PU")]
        public ViewResult GetBusinessTripByDatesPU(int selectedYear = 0)
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

            return View(selectedYear);
        }

        [Authorize(Roles = "PU")]
        public PartialViewResult GetBusinessTripDataByDatesPU(int selectedYear = 0)
        {
            //TODO: derived from VU controller, code shouldn't repeat
            var query = from bt in repository.BusinessTrips.AsEnumerable()
                        join emp in repository.Employees on bt.EmployeeID equals emp.EmployeeID
                        join loc in repository.Locations on bt.LocationID equals loc.LocationID
                        where (bt.StartDate.Year == selectedYear
                              && bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date)
                        orderby emp.DateDismissed, emp.LastName, bt.StartDate
                        select new BusinessTripViewModel(bt);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(query.ToList());
        }

        [Authorize(Roles = "PU")]
        public ActionResult EditFinishedBT(int id = 0, int selectedYear = 0)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            BusinessTrip businessT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessT == null || businessT.EndDate.Date >= DateTime.Now.ToLocalTimeAzure().Date)
            {
                return HttpNotFound();
            }

            ViewBag.EmployeeInformation = businessT.BTof.LastName + " " + businessT.BTof.FirstName + " (" + businessT.BTof.EID + ") from " + businessT.BTof.Department.DepartmentName;

            BusinessTripViewModel businessTrip = new BusinessTripViewModel(businessT);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.LocationsList = LocationsDropDownList(businessT.LocationID);
            ViewBag.UnitsList = UnitsDropDownList(businessT.UnitID);
            return View("EditFinishedBT", businessTrip);

        }

        [Authorize(Roles = "PU")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFinishedBT(BusinessTrip businessTrip, int selectedYear = 0)
        {

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            BusinessTrip btFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == businessTrip.BusinessTripID).FirstOrDefault();

            if (btFromRepository == null)
            {
                return HttpNotFound();
            }

            else
            {
                if (ModelState.IsValid)
                {
                    LocationsDropDownList(businessTrip.LocationID);

                    Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();

                    businessTrip.BTof = btFromRepository.BTof;
                    businessTrip.Status = btFromRepository.Status;
                    businessTrip.OldStartDate = btFromRepository.StartDate;
                    businessTrip.OldEndDate = btFromRepository.EndDate;
                    businessTrip.OldLocationID = btFromRepository.LocationID;
                    businessTrip.OldLocationTitle = btFromRepository.Location.Title;

                    int oldDaysUsedInBT = CountingDaysUsedInBT(btFromRepository);

                    businessTrip.LastCRUDedBy = btFromRepository.LastCRUDedBy;
                    businessTrip.LastCRUDTimestamp = btFromRepository.LastCRUDTimestamp;



                    if (businessTrip.BTof.Visa != null)
                    {
                        businessTrip.BTof.Visa.DaysUsedInBT -= oldDaysUsedInBT;
                        businessTrip.BTof.Visa.DaysUsedInBT += CountingDaysUsedInBT(businessTrip);
                    }

                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                        selectedBusinessTripsList.Add(businessTrip);

                        messenger.Notify(new Message(MessageType.PUEditsFInishedBT, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToBTM, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToDIR, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToEMP, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToResponsible, selectedBusinessTripsList, author));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                    catch (VacationAlreadyExistException)
                    {
                        return Json(new { error = btCreationError });
                    }

                    return Json(new { success = "success" });

                }
            }
            BusinessTripViewModel btTripModel = new BusinessTripViewModel(businessTrip);
            btTripModel.BTof = repository.BusinessTrips.Where(bt => bt.BusinessTripID == businessTrip.BusinessTripID).Select(b => b.BTof).FirstOrDefault();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.LocationsList = LocationsDropDownList(btTripModel.LocationID);
            ViewBag.UnitsList = UnitsDropDownList(btTripModel.LocationID);
            return View("EditReportedFinishedBT", businessTrip);
        }

        private SelectList LocationsDropDownList(int selectedLocationID = 0)
        {
            var locationList = from loc in repository.Locations
                               orderby loc.Title
                               select loc;

            return new SelectList(locationList, "LocationID", "Title", selectedLocationID);
        }

        private SelectList UnitsDropDownList(int selectedUnitID = 0)
        {
            var unitList = from unit in repository.Units
                           orderby unit.ShortTitle
                           select unit;

            return new SelectList(unitList, "UnitID", "ShortTitle", selectedUnitID);
        }

        public int CountingDaysUsedInBT(BusinessTrip businessTrip)
        {
            //'+1' day for counting last day too
            return ((businessTrip.EndDate - businessTrip.StartDate).Days + 1);
        }

        string defaultAccComment = "ВКО №   від   , cума:   UAH." + System.Environment.NewLine + "ВКО №   від   , cума:   USD.";

        #endregion
    }
}
