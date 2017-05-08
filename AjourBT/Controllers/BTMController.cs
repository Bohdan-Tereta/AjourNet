using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using System.Globalization;
using System.Threading;
using AjourBT.Domain.ViewModels;
using System.Data.Objects;
using AjourBT.Infrastructure;
using AjourBT.Domain.Entities;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Text;
using AjourBT.Exeptions;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Controllers // Add Items to CalendarItem (Employee)
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif

    [Authorize(Roles = "BTM")]
    public class BTMController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        //TODO: duplicated in ACCController and ADMController 
        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

        //TODO: duplicated in ADMController 
        private string btCreationError = "Absence already planned on this period for this user. "
                                      + "Please change OrderDates or if BT haven\'t OrderDates "
                                      + "change \'From\' or \'To\'";

        //TODO: duplicated in ACCController
        private StringBuilder comment = new StringBuilder();

        //TODO: duplicated in ACCController
        private string defaultAccComment;


        public BTMController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;

            //TODO: duplicated in ACCController
            this.comment = this.comment.Append("ВКО №   від   , cума:   UAH.");
            this.comment = this.comment.AppendLine();
            this.comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }

        #region "Visas and permits tab"

        [Authorize(Roles = "BTM")]
        public ActionResult GetVisaBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        [Authorize(Roles = "BTM")]
        public PartialViewResult GetVisaDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            IList<Employee> selected = repository.SearchVisaDataExcludingDismissed(searchString);

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return PartialView(selected);
        }

        #region Passport

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ModifyPassport(string id, string isChecked, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            int employeeID;
            bool AddPassport;
            AddPassport = isChecked != null ? true : false;

            Int32.TryParse(id, out employeeID);
            if (employeeID != 0)
            {
                Employee emp = (from e in repository.Employees where e.EmployeeID == employeeID select e).FirstOrDefault();
                if (emp == null)
                {
                    return HttpNotFound();
                }

                if (AddPassport)
                {
                    //if (emp.Passport == null)
                    //{
                    try
                    {
                        repository.SavePassport(new Passport { EmployeeID = employeeID });
                    }
                    catch (System.InvalidOperationException)
                    {
                        return Json(new { error = modelError });
                    }
                    //}
                }
                else
                {
                    if (emp.Passport != null)
                        repository.DeletePassport(employeeID);
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            return HttpNotFound();
        }

        //
        // GET: /BTM/PassportAddDate
        public ActionResult PassportAddDate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
            }

            Passport passport = repository.Passports.Where(p => p.EmployeeID == id).FirstOrDefault();
            PassportViewModel model = new PassportViewModel(passport);
            return View(model);
        }

        //
        // POST: /BTM/PassportAddDate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PassportAddDate(Passport passport, string searchString = "")
        {
            if (passport.EndDate == null)
            {
                ModelState.AddModelError("error", "error");
            }
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePassport(passport);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                Employee emp = repository.Employees.Where(e => e.EmployeeID == passport.EmployeeID).FirstOrDefault();
                ViewBag.SearchString = searchString;
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                // return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            PassportViewModel passportModel = new PassportViewModel(passport);
            ViewBag.SearchString = searchString;
            return View(passportModel);
        }

        // GET: /BTM/PassportEditDate/5
        public ActionResult PassportEditDate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Passport passport = (from p in repository.Passports where p.EmployeeID == id select p).FirstOrDefault();

            if (passport == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = passport.PassportOf.LastName + " " + passport.PassportOf.FirstName + " (" + passport.PassportOf.EID + ") from " + passport.PassportOf.Department.DepartmentName;

                PassportViewModel passportModel = new PassportViewModel(passport);

                return View(passportModel);
            }
        }

        //
        // POST: /BTM/PassportEditDate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PassportEditDate(Passport passport, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePassport(passport);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            PassportViewModel passportModel = new PassportViewModel(passport);
            ViewBag.SearchString = searchString;
            return View(passportModel);
        }

        #endregion

        #region Visa

        // GET: /BTM/VisaCreate
        public ActionResult VisaCreate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.EmployeeID = id;
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisaCreate(Visa visa, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisa(visa, visa.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }

                IList<Employee> empList = repository.SearchVisaData(searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            VisaViewModel visaModel = new VisaViewModel(visa);
            return View(visaModel);
        }

        //
        // GET: /BTM/VisaEdit/5
        public ActionResult VisaEdit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Visa visa = (from v in repository.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (visa == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = visa.VisaOf.LastName + " " + visa.VisaOf.FirstName + " (" + visa.VisaOf.EID + ") from " + visa.VisaOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;
                VisaViewModel visaModel = new VisaViewModel(visa);

                return View(visaModel);
            }
        }

        //
        // POST: /BTM/VisaEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisaEdit(Visa visa, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisa(visa, visa.EmployeeID);
                }

                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }

                IList<Employee> empList = repository.SearchVisaData(searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            VisaViewModel visaModel = new VisaViewModel(visa);
            ViewBag.SearchString = searchString;
            return View(visaModel);
        }

        //
        // GET: /BTM/VisaDelete/5
        public ActionResult VisaDelete(int id = 0, string searchString = "")
        {
            Visa visa = (from v in repository.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (visa == null)
            {
                return HttpNotFound();
            }
            ViewBag.SearchString = searchString;
            ViewBag.EmployeeID = id;
            return View(visa);
        }

        //
        // POST: /BTM/VisaDelete/5
        [HttpPost, ActionName("VisaDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult VisaDeleteConfirmed(int id, string searchString = "")
        {
            repository.DeleteVisa(id);
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            IList<Employee> empList = repository.SearchVisaData(searchString);
            return View("TableViewVisasAndPermitsBTM", empList);
        }
        #endregion

        #region VisaRegistrationDate
        //
        // GET: /BTM/VisaRegCreate

        public ActionResult VisaRegCreate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.EmployeeID = id;
            }

            return View();
        }

        //
        // POST: /BTM/VisaRegCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisaRegCreate(VisaRegistrationDate visaRegDate, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);

                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                Employee empWithVisa = (from emp in repository.Employees where emp.EmployeeID == visaRegDate.EmployeeID select emp).FirstOrDefault();
                messenger.Notify(new Message(MessageType.BTMCreateVisaRegistrationDateToEMP, null, author, empWithVisa));
                messenger.Notify(new Message(MessageType.BTMCreateVisaRegistrationDateToBTM, null, author, empWithVisa));

                return View("TableViewVisasAndPermitsBTM", empList);
            }

            RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);

            return View(visaRegistrationDate);
        }

        //
        // GET: /BTM/VisaRegEdit/5
        public ActionResult VisaRegEdit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            VisaRegistrationDate visaRegDate = (from v in repository.VisaRegistrationDates where v.EmployeeID == id select v).FirstOrDefault();

            if (visaRegDate == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = visaRegDate.VisaRegistrationDateOf.LastName + " " + visaRegDate.VisaRegistrationDateOf.FirstName + " (" + visaRegDate.VisaRegistrationDateOf.EID + ") from " + visaRegDate.VisaRegistrationDateOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;

                RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);

                return View(visaRegistrationDate);
            }
        }

        //
        // POST: /BTM/VisaRegEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VisaRegEdit(VisaRegistrationDate visaRegDate, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);

                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                Employee empWithVisa = (from emp in repository.Employees where emp.EmployeeID == visaRegDate.EmployeeID select emp).FirstOrDefault();
                messenger.Notify(new Message(MessageType.BTMUpdateVisaRegistrationDateToEMP, null, author, empWithVisa));
                messenger.Notify(new Message(MessageType.BTMUpdateVisaRegistrationDateToBTM, null, author, empWithVisa));

                return View("TableViewVisasAndPermitsBTM", empList);
            }

            RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);
            ViewBag.SearchString = searchString;
            return View(visaRegistrationDate);
        }

        //
        // POST: /BTM/VisaRegDelete/5

        [HttpPost, ActionName("VisaRegDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult VisaRegDeleteConfirmed(int id, string searchString = "")
        {
            VisaRegistrationDate visaRegistrationDate = (from v in repository.VisaRegistrationDates where v.EmployeeID == id select v).FirstOrDefault();
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (visaRegistrationDate == null)
            {
                return HttpNotFound();
            }
            else
            {
                repository.DeleteVisaRegistrationDate(id);
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            //return RedirectToAction("BTMView", "Home", new { searchString = searchString });

        }



        #endregion

        #region Permit
        //
        // GET: /BTM/PermitCreate

        public ActionResult PermitCreate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.EmployeeID = id;
            }

            return View();
        }

        //
        // POST: /BTM/PermitCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PermitCreate(Permit permit, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePermit(permit, permit.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            PermitViewModel permitModel = new PermitViewModel(permit);
            return View(permitModel);
        }

        //
        // GET: /BTM/PermitEdit/5

        public ActionResult PermitEdit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Permit permit = (from p in repository.Permits where p.EmployeeID == id select p).FirstOrDefault();

            if (permit == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = permit.PermitOf.LastName + " " + permit.PermitOf.FirstName + " (" + permit.PermitOf.EID + ") from " + permit.PermitOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;
                PermitViewModel permitModel = new PermitViewModel(permit);

                return View(permitModel);
            }
        }

        //
        // POST: /BTM/PermitEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PermitEdit(Permit permit, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePermit(permit, permit.EmployeeID);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }

                Employee emp = repository.Employees.Where(e => e.EmployeeID == permit.EmployeeID).FirstOrDefault();
                if (permit.CancelRequestDate != null)
                {
                    Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    Employee employeeForPermit = repository.Employees.Where(p => p.EmployeeID == permit.EmployeeID).FirstOrDefault();
                    messenger.Notify(new Message(MessageType.BTMCancelsPermitToADM, null, author, employeeForPermit));
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            PermitViewModel permitModel = new PermitViewModel(permit);
            return View(permitModel);
        }

        //
        // GET: /BTM/PermitDelete/5

        public ActionResult PermitDelete(int id = 0, string searchString = "")
        {
            Permit permit = (from p in repository.Permits where p.EmployeeID == id select p).FirstOrDefault();
            ViewBag.SearchString = searchString;

            if (permit == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = id;
            return View(permit);
        }

        //
        // POST: /BTM/PermitDelete/5

        [HttpPost, ActionName("PermitDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PermitDeleteConfirmed(int id, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            repository.DeletePermit(id);
            Employee emp = repository.Employees.Where(e => e.EmployeeID == id).FirstOrDefault();
            List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
            return View("TableViewVisasAndPermitsBTM", empList);
            //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
        }






        #endregion


        #region Insurance

        // GET: /BTM/InsuranceCreate
        public ActionResult InsuranceCreate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.EmployeeID = id;
            }

            return View();
        }

        //
        // POST: /BTM/InsuranceCreate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsuranceCreate(Insurance insurance, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveInsurance(insurance, insurance.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            InsuranceViewModel insuranceModel = new InsuranceViewModel(insurance);
            return View(insuranceModel);
        }

        //
        // GET: /BTM/InsuranceEdit/5

        public ActionResult InsuranceEdit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Insurance insurance = (from p in repository.Insurances where p.EmployeeID == id select p).FirstOrDefault();

            if (insurance == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = insurance.InsuranceOf.LastName + " " + insurance.InsuranceOf.FirstName + " (" + insurance.InsuranceOf.EID + ") from " + insurance.InsuranceOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;
                InsuranceViewModel insuranceModel = new InsuranceViewModel(insurance);

                return View(insuranceModel);
            }
        }

        //
        // POST: /BTM/InsuranceEdit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsuranceEdit(Insurance insurance, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveInsurance(insurance, insurance.EmployeeID);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            InsuranceViewModel insuranceModel = new InsuranceViewModel(insurance);
            ViewBag.SearchString = searchString;
            return View(insuranceModel);

        }

        //
        // GET: /BTM/PermitDelete/5

        public ActionResult InsuranceDelete(int id = 0, string searchString = "")
        {
            Insurance insurance = (from p in repository.Insurances where p.EmployeeID == id select p).FirstOrDefault();
            ViewBag.SearchString = searchString;

            if (insurance == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = id;
            return View(insurance);
        }

        //
        // POST: /BTM/PermitDelete/5

        [HttpPost, ActionName("InsuranceDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult InsuranceDeleteConfirmed(int id, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            repository.DeleteInsurance(id);
            Employee emp = repository.Employees.Where(e => e.EmployeeID == id).FirstOrDefault();
            List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
            return View("TableViewVisasAndPermitsBTM", empList);
            //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
        }
        #endregion

        #endregion

        #region "BTs in process" tab

        #region Commented methods for dropdownlist
        // ****** Important please do not delete when doing merge
        /* for right work of dropdownlist 
           must pass and take selectedDepartment as argument
           in methods which make POST request to save
        */

        //public PartialViewResult GetBusinessTripDataBTM(string selectedDepartment = "")
        //{
        //    var query = from e in repository.Employees
        //                join dep in repository.Departments on e.DepartmentID equals dep.DepartmentID
        //                where ((selectedDepartment == null||selectedDepartment == String.Empty || dep.DepartmentName == selectedDepartment) && (e.DateDismissed == null))
        //                orderby e.IsManager descending, e.LastName
        //                select e;

        //    ViewBag.JSDAtePattern = MvcApplication.JSDatePattern;
        //    ViewBag.SelectedDepartment = selectedDepartment;
        //    return PartialView(query.ToList());
        //}


        //public ActionResult GetBusinessTripBTM(string selectedDepartment = null)
        //{
        //    var selected = from dep in repository.Departments
        //                   orderby dep.DepartmentName
        //                   select dep;

        //    ViewBag.DepartmentsList = new SelectList(selected, "DepartmentName", "DepartmentName");
        //    ViewBag.SelectedDepartment = selectedDepartment;
        //    return View();
        //}
        #endregion

        public ActionResult GetBusinessTripBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        public PartialViewResult GetBusinessTripDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selectedEmpl = repository.Employees.ToList();

            selectedEmpl = SearchBusinessTripDataBTM(selectedEmpl, searchString);

            ViewBag.JSDAtePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            // ViewBag.SelectedDepartment = selectedDepartment;
            return PartialView(selectedEmpl);
        }

        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportConfirmedBTs(string[] selectedConfirmedBTs = null, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (selectedConfirmedBTs != null && selectedConfirmedBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedConfirmedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = null;
                        if (repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault() != null)
                            selectedBT = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault());
                        if (selectedBT != null
                             && (selectedBT.Status == (BTStatus.Confirmed)
                             || (selectedBT.Status == (BTStatus.Confirmed | BTStatus.Modified))))
                        {
                            selectedBT.Status = BTStatus.Confirmed | BTStatus.Reported;
                            if (selectedBT.BTof.Visa != null)
                            {
                                selectedBT.BTof.Visa.DaysUsedInBT += CountingDaysUsedInBT(selectedBT);
                                selectedBT.BTof.Visa.EntriesUsedInBT++;
                            }
                            try
                            {
                                repository.SaveBusinessTrip(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                }
            }
            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
        }

        [Authorize(Roles = "BTM")]
        public ActionResult BTMArrangeBT(int id = 0, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            BusinessTrip bt = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (bt == null)
            {
                return HttpNotFound();
            }

            if (bt.Status.HasFlag(BTStatus.Confirmed)
                && bt.OrderEndDate == null
                && bt.OrderStartDate == null)
            {
                bt.OrderStartDate = bt.StartDate.AddDays(-1);
                bt.OrderEndDate = bt.EndDate.AddDays(1);
                bt.DaysInBtForOrder = (bt.OrderEndDate.Value - bt.OrderStartDate.Value).Days + 1;
            }
            BusinessTripViewModel businesstrip = new BusinessTripViewModel(bt);

            return View(businesstrip);
        }

        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveArrangedBT(BusinessTrip bTrip, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            bTrip = RewriteBTsPropsFromRepositoryBTM(bTrip);

            if (ModelState.IsValid)
            {
                if (bTrip.Status == (BTStatus.Registered | BTStatus.Modified))
                {
                    bTrip.Status = bTrip.Status & ~BTStatus.Modified;
                }
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                try
                {
                    repository.SaveBusinessTrip(bTrip);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                catch (VacationAlreadyExistException)
                {
                    return Json(new { error = btCreationError });
                }
                selectedBusinessTripsList.Add(bTrip);
                if (bTrip.Status == (BTStatus.Confirmed | BTStatus.Modified) || bTrip.Status == BTStatus.Confirmed)
                {
                    messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author)); 
                    messenger.Notify(new Message(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                }
            }
            else
            {
                return View("BTMArrangeBT", new BusinessTripViewModel(bTrip));
            }

            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
            // return View("OneRowBTM", empList);
        }

        //GET: Delete BT by BTM
        [Authorize(Roles = "BTM")]
        public ActionResult DeleteBTBTM(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT == null || !selectedBT.Status.HasFlag(BTStatus.Cancelled))
            {
                return HttpNotFound();
            }
            return View(selectedBT);
        }

        //POST
        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("DeleteBTBTM")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBTBTMConfirmed(int id = 0, string searchString = "")
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            ViewBag.SearchString = searchString;

            if (selectedBT != null)
            {
                if (selectedBT.Status.HasFlag(BTStatus.Cancelled))
                {
                    repository.DeleteBusinessTrip(id);
                }
            }
            List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
            return View("TableViewBTM", empList);
        }

        //GET
        [Authorize(Roles = "BTM")]
        public ActionResult Reject_BT_BTM(int id = 0, string jsonRowVersionData = "", string searchString = "")
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            if (jsonRowVersionData != "")
            {

                businessTrip.RowVersion = JsonConvert.DeserializeObject<byte[]>(jsonRowVersionData.Replace(" ", "+"));
            }

            ViewBag.SearchString = searchString;
            BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);
            return View(btripModel);
        }

        //POST
        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("Reject_BT_BTM")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject_BT_BTM_Confirm(BusinessTrip businessTrip, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                businessTrip = RewriteBTsPropsFromRepositoryWhenReject(businessTrip);
                businessTrip = RewriteBTsPropsFromRepositoryBTM(businessTrip);

                if ((businessTrip.RejectComment != null) && (businessTrip.RejectComment != String.Empty))
                {
                    Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                    BTStatus oldBTStatus = businessTrip.Status;
                    businessTrip.Status = BTStatus.Planned | BTStatus.Modified;
                    businessTrip.OrderStartDate = null;
                    businessTrip.OrderEndDate = null;
                    businessTrip.DaysInBtForOrder = null;
                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                    selectedBusinessTripsList.Add(businessTrip);
                    if (oldBTStatus == (BTStatus.Registered | BTStatus.Modified) || (oldBTStatus == BTStatus.Registered))
                    {
                        messenger.Notify(new Message(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                    }
                    if (oldBTStatus == (BTStatus.Confirmed | BTStatus.Modified) || (oldBTStatus == BTStatus.Confirmed))
                    {
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    List<Employee> empList = SearchBusinessTripDataBTM(repository.Employees.ToList(), searchString);
                    return View("TableViewBTM", empList);
                }
                else
                {
                    ModelState.AddModelError("", "Please, enter reject comment.");
                    BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);
                    return View("Reject_BT_BTM", btripModel);
                }
            }
        }

        public List<Employee> SearchBusinessTripDataBTM(List<Employee> empList, string searchString)
        {
            List<Employee> selectedEmpl = (from emp in empList
                                           where emp.DateDismissed == null && emp.BusinessTrips.Count != 0 &&
                                           (emp.EID.ToLower().Contains(searchString.ToLower())
                                                || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                                || emp.LastName.ToLower().Contains(searchString.ToLower()))

                                           orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                                           select emp).ToList();

            return selectedEmpl;
        }

        //TODO: duplicated in ACCController and BTMController
        public BusinessTrip RewriteBTsPropsFromRepositoryBTM(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();

            if ((bTrip.Status == (BTStatus.Planned | BTStatus.Modified)) || (bTrip.Status.HasFlag(BTStatus.Registered)))
            {
                bTrip.OrderStartDate = businessTripFromRepository.OrderStartDate;
                bTrip.OrderEndDate = businessTripFromRepository.OrderEndDate;
                bTrip.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;
            }

            if (bTrip.RejectComment == null || bTrip.RejectComment == String.Empty)
            {
                bTrip.RejectComment = businessTripFromRepository.RejectComment;
            }


            if (bTrip.AccComment == null || bTrip.AccComment == String.Empty || bTrip.AccComment == defaultAccComment)
            {
                bTrip.AccComment = businessTripFromRepository.AccComment;
            }

            bTrip.CancelComment = businessTripFromRepository.CancelComment;
            bTrip.Comment = businessTripFromRepository.Comment;
            bTrip.EmployeeID = businessTripFromRepository.EmployeeID;
            bTrip.BTof = businessTripFromRepository.BTof;
            bTrip.EndDate = businessTripFromRepository.EndDate;
            bTrip.LocationID = businessTripFromRepository.LocationID;
            bTrip.Manager = businessTripFromRepository.Manager;
            bTrip.OldEndDate = businessTripFromRepository.OldEndDate;
            bTrip.OldLocationID = businessTripFromRepository.OldLocationID;
            bTrip.OldLocationTitle = businessTripFromRepository.OldLocationTitle;
            bTrip.OldStartDate = businessTripFromRepository.OldStartDate;
            bTrip.Purpose = businessTripFromRepository.Purpose;
            bTrip.Responsible = businessTripFromRepository.Responsible;
            bTrip.StartDate = businessTripFromRepository.StartDate;
            bTrip.Status = businessTripFromRepository.Status;
            bTrip.LastCRUDedBy = businessTripFromRepository.LastCRUDedBy;
            bTrip.LastCRUDTimestamp = businessTripFromRepository.LastCRUDTimestamp;
            bTrip.Location = businessTripFromRepository.Location;

            return bTrip;
        }

        //TODO: duplicated in ACCController and BTMController
        public BusinessTrip RewriteBTsPropsFromRepositoryWhenReject(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();

            //all editable data for BTM

            bTrip.BTMComment = businessTripFromRepository.BTMComment;
            bTrip.Habitation = businessTripFromRepository.Habitation;
            bTrip.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            bTrip.Flights = businessTripFromRepository.Flights;
            bTrip.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            bTrip.Invitation = businessTripFromRepository.Invitation;
            bTrip.OrderStartDate = businessTripFromRepository.OrderStartDate;
            bTrip.OrderEndDate = businessTripFromRepository.OrderEndDate;
            bTrip.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;

            return bTrip;
        }
        
        //TODO: duplicated in ACCController

        #endregion

        #region "Private Trips" tab
        [Authorize(Roles = "BTM")]
        public PartialViewResult GetPrivateTripDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selected = repository.Employees.ToList(); ;


            selected = SearchPrivateTripData(selected, searchString);

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return PartialView(selected);
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

        [Authorize(Roles = "BTM")]
        public ActionResult GetPrivateTripBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        // GET: /BTM/PrivateTripCreate
        [Authorize(Roles = "BTM")]
        public ActionResult PrivateTripCreate(int id = 0, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == id select e).FirstOrDefault();

            if (employee == null || employee.Visa == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.SearchString = searchString;
            }
            return View();
        }

        //
        // POST: /BTM/PrivateTripCreate
        [Authorize(Roles = "BTM")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PrivateTripCreate(PrivateTrip PTrip, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            PrivateTrip privateTrip = PTrip;

            if (ModelState.IsValid)
            {
                Visa visa = repository.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
                if (visa != null)
                {
                    visa.DaysUsedInPrivateTrips += CountingDaysUsedInPT(privateTrip);
                    visa.EntriesUsedInPrivateTrips++;

                    repository.SaveVisa(visa, visa.EmployeeID);
                }
                repository.SavePrivateTrip(privateTrip);
                List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
                return View("TableViewPTBTM", emplist);
            }

            PrivateTripViewModel pTripModel = new PrivateTripViewModel(privateTrip);

            return View(pTripModel);
        }

        //
        // GET: /BTM/PrivateTripEdit/5
        [Authorize(Roles = "BTM")]
        public ActionResult PrivateTripEdit(int id, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            PrivateTrip pTrip = repository.PrivateTrips.Where(p => p.PrivateTripID == id).FirstOrDefault();

            if (pTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                Employee employee = repository.Employees.Where(e => e.EmployeeID == pTrip.EmployeeID).FirstOrDefault();
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
            }

            PrivateTripViewModel privateTripModel = new PrivateTripViewModel(pTrip);
            return View(privateTripModel);
        }

        //POST: /BTM/PrivateTripEdit/5
        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateTripEdit(PrivateTrip privateTrip, string searchString = null)
        {

            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            string modelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    Visa visa = repository.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
                    if (visa != null)
                    {
                        if (visa.DaysUsedInPrivateTrips != null)
                        {
                            PrivateTrip oldPrivateTrip = repository.PrivateTrips.Where(p => p.PrivateTripID == privateTrip.PrivateTripID).FirstOrDefault();
                            if (oldPrivateTrip != null)
                            {
                                visa.DaysUsedInPrivateTrips -= CountingDaysUsedInPT(oldPrivateTrip);
                            }
                            visa.DaysUsedInPrivateTrips += CountingDaysUsedInPT(privateTrip);
                        }

                        repository.SaveVisa(visa, visa.EmployeeID);
                    }

                    repository.SavePrivateTrip(privateTrip);
                    List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
                    return View("TableViewPTBTM", emplist);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            }
            //PrivateTripViewModel privateTripModel = new PrivateTripViewModel(privateTrip);
            //return View(privateTripModel);
            return Json(new { error = modelError });
        }

        // GET: /BTM/PrivateTripDelete/5
        [Authorize(Roles = "BTM")]
        public ActionResult PrivateTripDelete(int id, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            PrivateTrip privateTrip = (from pt in repository.PrivateTrips where pt.PrivateTripID == id select pt).FirstOrDefault();

            if (privateTrip == null)
            {
                return HttpNotFound();
            }

            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == privateTrip.EmployeeID select e).FirstOrDefault();

            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.EmployeeInformation = "Delete Private trip of " + employee.LastName + " " + employee.FirstName;

            return View(privateTrip);
        }

        //
        // POST: /BTM/PrivateTripDelete/5
        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("PrivateTripDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PrivateTripDeleteConfirmed(int id, string searchString = "")
        {
            PrivateTrip privateTrip = (from pt in repository.PrivateTrips where pt.PrivateTripID == id select pt).FirstOrDefault();
            if (privateTrip != null)
            {
                Visa visa = (from v in repository.Visas where v.EmployeeID == privateTrip.EmployeeID select v).FirstOrDefault();
                if (visa != null)
                {
                    visa.DaysUsedInPrivateTrips -= CountingDaysUsedInPT(privateTrip);
                    visa.EntriesUsedInPrivateTrips--;
                    repository.SaveVisa(visa, visa.EmployeeID);
                }

                repository.DeletePrivateTrip(id);
            }
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
            return View("TableViewPTBTM", emplist);
        }






        #endregion

        public int CountingDaysUsedInBT(BusinessTrip businessTrip)
        {
            //'+1' day for counting last day too
            return ((businessTrip.EndDate - businessTrip.StartDate).Days + 1);
        }
        public int CountingDaysUsedInPT(PrivateTrip privateTrip)
        {
            //'+1' day for counting last day too
            return ((privateTrip.EndDate - privateTrip.StartDate).Days + 1);
        }

        public List<Employee> GetEmployeeData(List<Employee> empList, string searchString)
        {
            List<Employee> selected = (from emp in empList
                                       where emp.EID.ToLower().Contains(searchString.ToLower())
                                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                                 || (emp.Visa != null)
                                                 && (emp.Visa.VisaType.ToLower().Contains(searchString.ToLower())
                                      || emp.Visa.StartDate.ToString().Contains(searchString)
                                      || emp.Visa.DueDate.ToString().Contains(searchString)
                                                 || emp.Visa.Entries == 0 && searchString.ToLower().Contains("mult"))
                                 || (emp.VisaRegistrationDate != null
                                      && emp.VisaRegistrationDate.RegistrationDate.ToString().Contains(searchString))
                                 || (emp.Permit != null)
                                      && (emp.Permit.StartDate.ToString().Contains(searchString)
                                      || emp.Permit.EndDate.ToString().Contains(searchString))
                                          || (emp.Insurance != null)
                                      && (emp.Insurance.StartDate.ToString().Contains(searchString)
                                      || emp.Insurance.EndDate.ToString().Contains(searchString)
                                        || emp.Insurance.Days == 0)
                                       orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                                       select emp).ToList();
            return selected;
        }
    }
}