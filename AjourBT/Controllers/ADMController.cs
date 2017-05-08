using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;
using System.Data.Entity.Infrastructure;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.ViewModels;
using ExcelLibrary.SpreadSheet;
using System.IO;
using System.Text;
using System.Web.Configuration;
using AjourBT.Infrastructure;
using System.Collections;
using AjourBT.Helpers;
using System.Web.Mvc.Html;
namespace AjourBT.Controllers
{
    [Authorize(Roles = "ADM, DIR")]
    public class ADMController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;
        private IXLSExporter xlsExporter;

        //TODO: duplicated in BusinessTripController and ACCController
        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";
        
        private string btDuplication = "BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.";
        
        private string btDatesOverlay = "BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'";
        //TODO: duplicated in BusinessTripController 
        private string btCreationError = "Absence already planned on this period for this user. "
                                      + "Please change OrderDates or if BT haven\'t OrderDates "
                                      + "change \'From\' or \'To\'";

        public ADMController(IRepository repository, IMessenger messenger, IXLSExporter xlsExporter)
        {
            this.repository = repository;
            this.messenger = messenger;
            this.xlsExporter = xlsExporter; 
        }

        #region Visas and Permits / BTs

        [Authorize(Roles = "ADM")]
        public ViewResult GetVisaADM(string userName = "")
        {

            var selectedUserDepartment = (from e in repository.Employees
                                          where e.EID == userName
                                          select e.Department.DepartmentName).FirstOrDefault();

            var allDepartments = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;
            
            ViewBag.SelectedDepartment = new SelectList(allDepartments, "DepartmentName", "DepartmentName", selectedUserDepartment);
            return View((object)selectedUserDepartment);
        }

        [Authorize(Roles = "ADM")]
        public PartialViewResult GetVisaDataADM(string departmentName = "", string userName = "", string selectedUserDepartment = "")
        {
            string selectedDepartment = departmentName;

            var selected = from emp in repository.Employees
                           join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                           where (((emp.Department.DepartmentName == selectedUserDepartment || (selectedDepartment == String.Empty && userName == String.Empty))
                                   || (dep.DepartmentName == selectedDepartment) || (selectedUserDepartment == String.Empty && selectedDepartment == String.Empty))
                                   && (emp.DateDismissed == null))
                           orderby emp.IsManager descending, emp.LastName
                           select emp;

            return PartialView(selected.ToList());
        }

        [Authorize(Roles = "ADM")]
        public ViewResult GetBusinessTripADM(string userName = "", string selectedDepartment = null)
        {

            var selectedUserDepartment = (from e in repository.Employees
                                          where e.EID == userName
                                          select e.Department.DepartmentName).FirstOrDefault();

            var allDepartments = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(allDepartments, "DepartmentName", "DepartmentName", selectedUserDepartment);
            ViewBag.UserDepartment = selectedUserDepartment;
            ViewBag.SelectedDepartment = selectedDepartment;
            return View();

        }
        
        public IEnumerable<Employee> SelectEmployees(string selectedDepartment, string selectedUserDepartment)
        {
            IEnumerable<Employee> data;
            if (selectedDepartment == null)
            {
                if (selectedUserDepartment == null)
                {
                    data = from emp in repository.Employees.AsEnumerable()
                           join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                           where (emp.DateDismissed == null)
                           orderby emp.IsManager descending, emp.LastName
                           select emp;
                }
                else
                {
                    data = from emp in repository.Employees.AsEnumerable()
                           join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                           where ((emp.Department.DepartmentName == selectedUserDepartment && (emp.DateDismissed == null)))
                           orderby emp.IsManager descending, emp.LastName
                           select emp;
                }


            }
            else
            {
                data = from emp in repository.Employees.AsEnumerable()
                       join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                       where ((emp.Department.DepartmentName == selectedDepartment || (selectedDepartment == String.Empty)
    || (selectedDepartment == null)) && (emp.DateDismissed == null))
                       orderby emp.IsManager descending, emp.LastName
                       select emp;



            }
            return data;
        }
        
        [Authorize(Roles = "ADM")]
        public PartialViewResult GetBusinessTripDataADM(string selectedDepartment = null, string selectedUserDepartment = null)
        {
            IEnumerable<Employee> data;
            data = SelectEmployees(selectedDepartment, selectedUserDepartment);

            ViewBag.SelectedDepartment = selectedUserDepartment == null ? selectedDepartment : selectedUserDepartment;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(data.ToList());
        }


        public void AddLastCRUDDataToBT(BusinessTrip selectedBT)
        {
            selectedBT.LastCRUDedBy = ControllerContext.HttpContext.User.Identity.Name;
            selectedBT.LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure();
        }

        // POST: /ADM/RegisterPlannedBTs

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPlannedBTs(string[] selectedPlannedBTs, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedPlannedBTs != null && selectedPlannedBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedPlannedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Planned) == BTStatus.Planned) && selectedBT.RejectComment == null)
                        {
                            selectedBT.Status = (selectedBT.Status | BTStatus.Registered) & ~BTStatus.Planned;
                            try
                            {
                                repository.SaveBusinessTrip(selectedBT);
                                AddLastCRUDDataToBT(selectedBT);
                                selectedBusinessTripsList.Add(selectedBT);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                return Json(new { error = modelError });
                            }
                            //catch (VacationAlreadyExistException)
                            //{
                            //    return Json(new { error = btCreationError });
                            //}
                        }
                    }
                }
                if (selectedBusinessTripsList.Count != 0)
                {
                    messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPlannedBTs(string[] selectedPlannedBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedPlannedBTs != null && selectedPlannedBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedPlannedBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Planned) == BTStatus.Planned) && selectedBT.RejectComment == null)
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Confirmed) & ~BTStatus.Planned;
                                AddLastCRUDDataToBT(selectedBT);
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
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if (((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered) && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Confirmed) & ~BTStatus.Registered;
                                AddLastCRUDDataToBT(selectedBT);
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
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMConfirmsPlannedOrRegisteredToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplanRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if ((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Planned | BTStatus.Modified) & ~BTStatus.Registered;
                                selectedBT.OldLocationID = selectedBT.LocationID;
                                selectedBT.OldLocationTitle = selectedBT.Location.Title;
                                selectedBT.OldStartDate = selectedBT.StartDate;
                                selectedBT.OldEndDate = selectedBT.EndDate;
                                AddLastCRUDDataToBT(selectedBT);
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
                    messenger.Notify(new Message(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelRegisteredBTs(string[] selectedRegisteredBTs = null, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;

            if (selectedRegisteredBTs != null && selectedRegisteredBTs.Length != 0)
            {
                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                foreach (string bt in selectedRegisteredBTs)
                {
                    int btID;
                    if (Int32.TryParse(bt, out btID))
                    {
                        BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == btID).FirstOrDefault();
                        if ((selectedBT.Status & BTStatus.Registered) == BTStatus.Registered && !selectedBT.Status.HasFlag(BTStatus.Cancelled))
                        {
                            try
                            {
                                selectedBT.Status = (selectedBT.Status | BTStatus.Cancelled) & ~BTStatus.Modified;
                                AddLastCRUDDataToBT(selectedBT);
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
                    messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, selectedBusinessTripsList, author));
                }
            }

            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT != null)
            {
                if ((selectedBT.Status & BTStatus.Confirmed) == BTStatus.Confirmed && !selectedBT.Status.HasFlag(BTStatus.Reported))
                {
                    try
                    {
                        Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                        List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                        selectedBT.Status = (selectedBT.Status | BTStatus.Cancelled);
                        AddLastCRUDDataToBT(selectedBT);
                        repository.SaveBusinessTrip(selectedBT);
                        selectedBusinessTripsList.Add(selectedBT);
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }
                }
            }
            return RedirectToAction("ADMView", "Home", new { tab = 1, selectedDepartment = selectedDepartment });
        }


        //GET: Delete Planned BT

        [Authorize(Roles = "ADM")]
        public ActionResult DeletePlannedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (selectedBT == null || (selectedBT.Status != (BTStatus.Planned) && selectedBT.Status != (BTStatus.Planned | BTStatus.Modified)))
            {
                return HttpNotFound();
            }
            ViewBag.SelectedDepartment = selectedDepartment;
            return View(selectedBT);
        }

        //POST: Delete Planned BT

        [Authorize(Roles = "ADM")]
        [HttpPost, ActionName("DeletePlannedBT")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePlannedBTConfirmed(int id = 0, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTrip selectedBT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (selectedBT != null)
            {
                if (selectedBT.Status == BTStatus.Planned)
                {
                    AddLastCRUDDataToBT(selectedBT);
                    repository.DeleteBusinessTrip(id);
                }
                else if (selectedBT.Status == (BTStatus.Planned | BTStatus.Modified))
                {
                    try
                    {
                        selectedBT.Status = (BTStatus.Planned | BTStatus.Cancelled);
                        AddLastCRUDDataToBT(selectedBT);
                        Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                        List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                        repository.SaveBusinessTrip(selectedBT);
                        selectedBusinessTripsList.Add(selectedBT);
                        messenger.Notify(new Message(MessageType.ADMCancelsPlannedModifiedToBTM, selectedBusinessTripsList, author));
                        //messenger.Notify(new Message(MessageType.ADMCancelsPlannedModifiedToACC, selectedBusinessTripsList, author));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new { error = modelError });
                    }

                }
            }
            IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
            return View("TableViewBTADM", empList.ToList());
        }

        //
        // GET: /ADM/Plan

        [Authorize(Roles = "ADM")]
        public ActionResult Plan(int id = 0, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == id select e).FirstOrDefault();

            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.LocationsList = LocationsDropDownList();
                ViewBag.UnitsList = UnitsDropDownList();
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

                if (employee.Visa != null)
                {
                    ViewBag.EmployeeVisa = employee.Visa;
                }
            }

            return View();
        }

        //
        // POST: /ADM/Plan

        [Authorize(Roles = "ADM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Plan(BusinessTrip BTrip, string selectedDepartment = null)
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.LocationsList = LocationsDropDownList(BTrip.LocationID);
            ViewBag.UnitsList = UnitsDropDownList(BTrip.UnitID);
            BusinessTrip businessTrip = BTrip;

            if (businessTrip.BusinessTripID == 0)
            {
                BusinessTrip BussinesTrip = (from e in repository.Employees
                                             where e.EmployeeID == businessTrip.EmployeeID
                                             from bts in e.BusinessTrips
                                             where
                                                 (businessTrip.StartDate == bts.StartDate ||
                                                 businessTrip.EndDate == bts.EndDate)
                                             select bts).FirstOrDefault();

                if (BussinesTrip != null && !BussinesTrip.Status.HasFlag(BTStatus.Cancelled))
                {
                    if (businessTrip.StartDate == businessTrip.EndDate)
                    {
                        if (BussinesTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                    else
                    {
                        if ((businessTrip.StartDate == BussinesTrip.StartDate && businessTrip.EndDate == BussinesTrip.EndDate) || BussinesTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                }

                foreach (BusinessTrip bt in repository.BusinessTrips.Where(bt => bt.EmployeeID == BTrip.EmployeeID))
                {
                    if (!bt.Status.HasFlag(BTStatus.Cancelled))
                    {

                        if ((BTrip.StartDate == bt.StartDate) && (BTrip.EndDate == bt.EndDate))
                        {
                            return Json(new { error = btDuplication });
                        }

                        if ((BTrip.EndDate == bt.StartDate) || (BTrip.StartDate == bt.EndDate) || (BTrip.StartDate == bt.StartDate) || (BTrip.EndDate == bt.EndDate))
                        {
                            if (BTrip.LocationID == bt.LocationID)
                            {
                                return Json(new { error = btDuplication });
                            }
                        }

                        if (isBetween(BTrip.StartDate, bt) || isBetween(BTrip.EndDate, bt) || isBetween(bt.StartDate, BTrip) || isBetween(bt.EndDate, BTrip))
                        {

                            return Json(new { error = btDatesOverlay });

                        }
                    }
                }
            }

            if (BTrip.BusinessTripID != 0)
            {
                BusinessTrip bsTrip = (from e in repository.Employees
                                       where e.EmployeeID == BTrip.EmployeeID
                                       from bt in e.BusinessTrips
                                       where
                                            (BTrip.StartDate == bt.StartDate ||
                                            BTrip.EndDate == bt.EndDate) //&& BTrip.LocationID == bt.LocationID
                                       select bt).FirstOrDefault();

                if (bsTrip != null && bsTrip.BusinessTripID != BTrip.BusinessTripID && !bsTrip.Status.HasFlag(BTStatus.Cancelled))
                {
                    if (businessTrip.StartDate == businessTrip.EndDate)
                    {
                        if (bsTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                    else
                    {
                        if ((businessTrip.StartDate == bsTrip.StartDate && businessTrip.EndDate == bsTrip.EndDate) || bsTrip.LocationID == businessTrip.LocationID)
                        {
                            return Json(new { error = btDuplication });
                        }
                    }
                }

                foreach (BusinessTrip bt in repository.BusinessTrips.Where(bt => (bt.EmployeeID == BTrip.EmployeeID && bt.BusinessTripID != BTrip.BusinessTripID)))
                {
                    if (!bt.Status.HasFlag(BTStatus.Cancelled))
                    {

                        if ((BTrip.StartDate == bt.StartDate) && (BTrip.EndDate == bt.EndDate))
                        {
                            return Json(new { error = btDuplication });
                        }

                        if ((BTrip.EndDate == bt.StartDate) || (BTrip.StartDate == bt.EndDate) || (BTrip.StartDate == bt.StartDate) || (BTrip.EndDate == bt.EndDate))
                        {
                            if (BTrip.LocationID == bt.LocationID)
                            {
                                return Json(new { error = btDuplication });
                            }
                        }

                        if (isBetween(BTrip.StartDate, bt) || isBetween(BTrip.EndDate, bt) || isBetween(bt.StartDate, BTrip) || isBetween(bt.EndDate, BTrip))
                        {
                            return Json(new { error = btDatesOverlay });
                        }
                    }
                }

                businessTrip = RewriteBTsPropsAfterPlanningFromRepository(BTrip);
                businessTrip.RejectComment = null;
            }


            if (ModelState.IsValid)
            {
                try
                {
                    AddLastCRUDDataToBT(businessTrip);
                    businessTrip.Location = repository.Locations.Where(l => l.LocationID == businessTrip.LocationID).FirstOrDefault();
                    businessTrip.Unit = repository.Units.Where(l => l.UnitID == businessTrip.UnitID).FirstOrDefault();

                    repository.SaveBusinessTrip(businessTrip);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = modelError });
                }
                catch (VacationAlreadyExistException)
                {
                    return Json(new { error = btCreationError });
                }

                IEnumerable<Employee> empList = SelectEmployees(selectedDepartment, null);
                return View("TableViewBTADM", empList.ToList());
            }

            BusinessTripViewModel bTripModel = new BusinessTripViewModel(businessTrip);
            return View("Plan", bTripModel);
        }


        public bool isBetween(DateTime date, BusinessTrip btFromRepository)
        {
            if ((date > btFromRepository.StartDate) && (date < btFromRepository.EndDate))
                return true;
            else
                return false;
        }


        [Authorize(Roles = "ADM")]
        public BusinessTrip RewriteBTsPropsAfterPlanningFromRepository(BusinessTrip businessTripModel)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == businessTripModel.BusinessTripID).FirstOrDefault();

            if (businessTripFromRepository.Status.HasFlag(BTStatus.Modified))
            {
                businessTripFromRepository.UnitID = businessTripFromRepository.UnitID;
                businessTripFromRepository.OldLocationID = businessTripFromRepository.LocationID;
                businessTripFromRepository.OldLocationTitle = businessTripFromRepository.Location.Title;
                businessTripFromRepository.OldStartDate = businessTripFromRepository.StartDate;
                businessTripFromRepository.OldEndDate = businessTripFromRepository.EndDate;
            }

            //businessTrip.LocationID = businessTripModel.LocationID;
            //businessTrip.Location = repository.Locations.Where(l => l.LocationID == businessTrip.LocationID).FirstOrDefault();
            //businessTrip.StartDate = businessTripModel.StartDate;
            //businessTrip.EndDate = businessTripModel.EndDate;
            //businessTrip.Purpose = businessTripModel.Purpose;
            //businessTrip.Manager = businessTripModel.Manager;
            //businessTrip.Responsible = businessTripModel.Responsible;
            //businessTrip.Comment = businessTripModel.Comment;
            //businessTrip.RowVersion = businessTripModel.RowVersion;

            businessTripFromRepository.UnitID = businessTripFromRepository.UnitID;
            businessTripModel.BTof = businessTripFromRepository.BTof;
            businessTripModel.CancelComment = businessTripFromRepository.CancelComment;
            businessTripModel.Flights = businessTripFromRepository.Flights;
            businessTripModel.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            businessTripModel.Habitation = businessTripFromRepository.Habitation;
            businessTripModel.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            businessTripModel.Invitation = businessTripFromRepository.Invitation;
            businessTripModel.OldEndDate = businessTripFromRepository.OldEndDate;
            businessTripModel.OldLocationID = businessTripFromRepository.OldLocationID;
            businessTripModel.OldLocationTitle = businessTripFromRepository.OldLocationTitle;
            businessTripModel.OldStartDate = businessTripFromRepository.OldStartDate;
            businessTripModel.RejectComment = businessTripFromRepository.RejectComment;
            businessTripModel.BTMComment = businessTripFromRepository.BTMComment;
            businessTripModel.Status = businessTripFromRepository.Status;
            businessTripModel.LastCRUDedBy = businessTripFromRepository.LastCRUDedBy;
            businessTripModel.LastCRUDTimestamp = businessTripFromRepository.LastCRUDTimestamp;
            businessTripModel.OrderStartDate = businessTripFromRepository.OrderStartDate;
            businessTripModel.OrderEndDate = businessTripFromRepository.OrderEndDate;
            businessTripModel.DaysInBtForOrder = businessTripFromRepository.DaysInBtForOrder;


            return businessTripModel;
        }

        //
        // GET: /ADM/Edit/5

        [Authorize(Roles = "ADM")]
        public ActionResult EditPlannedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (businessT == null || (!businessT.Status.HasFlag(BTStatus.Planned)))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = businessT.BTof.LastName + " " + businessT.BTof.FirstName + " (" + businessT.BTof.EID + ") from " + businessT.BTof.Department.DepartmentName;
                if (businessT.BTof.Visa != null)
                {
                    ViewBag.EmployeeVisa = businessT.BTof.Visa;
                }
            }

            BusinessTripViewModel businessTrip = new BusinessTripViewModel(businessT);
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.LocationsList = LocationsDropDownList(businessT.LocationID);
            ViewBag.UnitsList = UnitsDropDownList(businessT.UnitID);
            return View("EditPlannedBT", businessTrip);
        }
        #endregion

        #region Employees tab

        [Authorize(Roles = "ADM, DIR")]
        public ActionResult GetEmployeeReadOnly(string searchString = "")
        {
            string selectedDepartment;
            try
            {
                selectedDepartment = (from e in repository.Employees
                              where e.EID == System.Web.HttpContext.Current.User.Identity.Name
                                      orderby e.IsManager descending, e.DateDismissed, e.LastName
                              select e).FirstOrDefault().Department.DepartmentName;
            }
            catch
            {
                selectedDepartment = String.Empty;
            }
            PrepareGetEmployeeViewBags(selectedDepartment, searchString);
            return View();
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "ADM, DIR")]
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

        [Authorize(Roles = "ADM, DIR")]
        public PartialViewResult GetEmployeeDataReadOnly(string selectedDepartment = null, string searchString = "")
        {
            List<EmployeeViewModel> data = PrepareEmployeeData(selectedDepartment, searchString);

            return PartialView(data);
        }

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        [Authorize(Roles = "ADM, DIR")]
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
        //To be moved to repository after ViewModels are moved to Domain [tebo  ] 
        [Authorize(Roles = "ADM, DIR")]
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
                    orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                    select new EmployeeViewModel(emp)).ToList();

            return data;
        }

        [Authorize(Roles = "ADM, DIR")]
        public ActionResult EmployeeDetails(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.DepartmentList = (from d in repository.Departments select d).ToList();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee emp = repository.Users.FirstOrDefault(e => e.EmployeeID == id);

            if (emp == null)
            {
                return HttpNotFound();
            }

            if (emp.Position != null)
            {
                ViewBag.Position = emp.Position.TitleEn;
            }

            EmployeeViewModel employee = new EmployeeViewModel(emp);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(employee);
        }

        //TODO: this whole region is duplicated in PU controller
        #region GetMails

        [Authorize(Roles = "ADM, DIR")]
        public ActionResult GetMailAliasEMails(string selectedDepartment = "")
        {
            return View(repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "ADM, DIR")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GetMailToLinkWithBcc(string selectedDepartment = "")
        {
            string user = HttpContext.User.Identity.Name;
            ViewBag.User = user;

            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "ADM, DIR")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GetSecondMailToLinkWithBcc(string selectedDepartment = "")
        {
            string user = HttpContext.User.Identity.Name;
            ViewBag.User = user;

            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "ADM, DIR")]
        public ActionResult GetSecondMailAliasEMails(string selectedDepartment = "")
        {
            return View("GetMailAliasEMails", repository.GetCurrentlyEmployedEmployees(selectedDepartment));
        }


        [Authorize(Roles = "ADM, DIR")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExportEmployeesToExcelADM(string selectedDepartment)
        {
            //TODO [tebo] Replace full name with using namespace after moving all viewModels to domain
            IList<AjourBT.Domain.ViewModels.EmployeeViewModel> employeeData = repository.SearchUsersData(selectedDepartment);
            return File(xlsExporter.ExportEmployeesToExcelADM(employeeData), "application/vnd.ms-excel", "Employees.xls");
            }
        #endregion

        #endregion


        //TODO: duplicated in ACCController and BusinessTripController 
        [Authorize(Roles = "ADM, DIR")]
        private SelectList LocationsDropDownList(int selectedLocationID = 0)
        {
            var locationList = from loc in repository.Locations
                               orderby loc.Title
                               select loc;

            return new SelectList(locationList, "LocationID", "Title", selectedLocationID);
        }

        [Authorize(Roles = "ADM, DIR")]
        private SelectList UnitsDropDownList(int selectedUnitID = 0)
        {
            var unitList = from unit in repository.Units
                           orderby unit.ShortTitle
                           select unit;

            return new SelectList(unitList, "UnitID", "ShortTitle", selectedUnitID);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public int CalculateVisaDays(BusinessTrip bTrip)
        {
            DateTime endDateOfChekedPeriod = DateTime.Now.ToLocalTimeAzure().Date;
            if (bTrip.EndDate != default(DateTime))
                endDateOfChekedPeriod = bTrip.EndDate;
             
            DateTime startDateOfChekedPeriod = endDateOfChekedPeriod.AddDays(-180).Date;

            List<BusinessTrip> employeeBTs = (from b in repository.BusinessTrips.AsEnumerable()
                                             where b.BTof.EmployeeID == bTrip.EmployeeID 
                                             && b.Status == (BTStatus.Confirmed|BTStatus.Reported) 
                                             && b.OrderStartDate!=null 
                                             && b.OrderEndDate!=null 
                                             && (b.StartDate <= b.EndDate)
                                             && (b.OrderStartDate <= endDateOfChekedPeriod && b.OrderEndDate >= startDateOfChekedPeriod)
                                              select b).ToList();
         
            List<IGrouping<DateTime,BusinessTrip>> distinctBTs = employeeBTs.GroupBy(i => i.OrderStartDate.Value).ToList();
            
            List<PrivateTrip> employeePrivateTs = (from b in repository.PrivateTrips.AsEnumerable()
                                                   where b.EmployeeID == bTrip.EmployeeID
                                                   && (b.StartDate <= b.EndDate)
                                                   && (b.StartDate <= endDateOfChekedPeriod && b.EndDate >= startDateOfChekedPeriod)
                                              select b).ToList();
                     
            int spentDays = 0;
            if (bTrip.StartDate != default(DateTime) && bTrip.EndDate != default(DateTime))
            {
                spentDays += DaysSpentInTrip(startDateOfChekedPeriod, endDateOfChekedPeriod, bTrip.StartDate, bTrip.EndDate);
            }
            foreach (IGrouping<DateTime,BusinessTrip> b in distinctBTs)
            {
               spentDays += DaysSpentInTrip(startDateOfChekedPeriod, endDateOfChekedPeriod, b.FirstOrDefault().OrderStartDate, b.FirstOrDefault().OrderEndDate);
            }
            foreach (PrivateTrip pt in employeePrivateTs)
            {
                spentDays += DaysSpentInTrip(startDateOfChekedPeriod, endDateOfChekedPeriod, pt.StartDate, pt.EndDate);
            }

            return spentDays;
        }

        public int DaysSpentInTrip(DateTime startDateOfChekedPeriod, DateTime endDateOfChekedPeriod,  DateTime? StartDate, DateTime? EndDate) {
           
            int days = 0; 
            DateTime startDateForCalculations; 
            DateTime endDateForCalculations; 
            if (StartDate != null && EndDate != null 
                && (StartDate <= EndDate) 
                && (StartDate <= endDateOfChekedPeriod && EndDate >= startDateOfChekedPeriod))
            {
                startDateForCalculations =  StartDate.Value > startDateOfChekedPeriod ? StartDate.Value : startDateOfChekedPeriod; 
                endDateForCalculations =  EndDate.Value < endDateOfChekedPeriod ? EndDate.Value : endDateOfChekedPeriod;
                days += (endDateForCalculations - startDateForCalculations).Days + 1;
            }
            
            return days;
        }
    }
}
