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

    [Authorize(Roles = "DIR")]
    public class DIRController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        //TODO: duplicated in ACCController and ADMController 
        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

        //TODO: duplicated in ACCController
        private StringBuilder comment = new StringBuilder();

        //TODO: duplicated in ACCController
        private string defaultAccComment;

        public DIRController(IRepository repository, IMessenger messenger)
        {
            this.repository = repository;
            this.messenger = messenger;

            //TODO: duplicated in ACCController
            this.comment = this.comment.Append("ВКО №   від   , cума:   UAH.");
            this.comment = this.comment.AppendLine();
            this.comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }

        [Authorize(Roles = "DIR")]
        public PartialViewResult GetBusinessTripDataDIR(string selectedDepartment = null, string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<BusinessTrip> selectedData = SearchBusinessTripDIR(repository.BusinessTrips.ToList(), searchString, selectedDepartment);

            ViewBag.SearchString = searchString;
            ViewBag.SelectedDepartment = selectedDepartment;

            return PartialView(selectedData);
        }

        [Authorize(Roles = "DIR")]
        public ActionResult GetBusinessTripDIR(string selectedDepartment = null)
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;

            return View();
        }

        public List<BusinessTrip> SearchBusinessTripDIR(List<BusinessTrip> btList, string searchString, string selectedDepartment)
        {
            DateTime StartDateToCompare = DateTime.Now.ToLocalTimeAzure().Date;
            List<BusinessTrip> query = (from bt in repository.BusinessTrips
                                        join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                                        join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                                        where (selectedDepartment == null || selectedDepartment == String.Empty || d.DepartmentName == selectedDepartment)
                                              && (e.DateDismissed == null
                                                  && e.EID.ToLower().Contains(searchString.ToLower())
                                                  || e.FirstName.ToLower().Contains(searchString.ToLower())
                                                  || e.LastName.ToLower().Contains(searchString.ToLower())
                                                  || bt.Location.Title.ToLower().Contains(searchString.ToLower()))
                                              && ((bt.Status == BTStatus.Confirmed || (bt.Status == (BTStatus.Confirmed | BTStatus.Modified))) && (bt.StartDate > StartDateToCompare))
                                        orderby e.IsManager descending, e.LastName, bt.StartDate
                                        select bt).ToList();
            return query;
        }


        //
        // GET: /DIR/Reject/
        [Authorize(Roles = "DIR")]
        public ActionResult Reject_BT_DIR(int id = 0, string jsonRowVersionData = "", string selectedDepartment = null)
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

            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel btripModel = new BusinessTripViewModel(businessTrip);

            return View(btripModel);
        }


        //
        // POST: /DIR/Reject/5

        [Authorize(Roles = "DIR")]
        [HttpPost, ActionName("Reject_BT_DIR")]
        [ValidateAntiForgeryToken]
        //public ActionResult Reject_BT_DIR_Confirm(int id = 0, string rejectComment = "", string selectedDepartment = null)
        public ActionResult Reject_BT_DIR_Confirm(BusinessTrip businessTrip, string selectedDepartment = null)
        {
            //BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

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
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToADM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToResponsible, selectedBusinessTripsList, author));
                    //messenger.Notify(new Message(MessageType.DIRRejectsConfirmedToACC, selectedBusinessTripsList, author));
                    return RedirectToAction("DIRView", "Home", new { tab = Tabs.DIR.BusinessTrips, selectedDepartment = selectedDepartment });
                }
                else
                {
                    ModelState.AddModelError("", "Please, enter reject comment.");
                    BusinessTripViewModel bTripModel = new BusinessTripViewModel(businessTrip);
                    ViewBag.SelectedDepartment = selectedDepartment;

                    return View(bTripModel);
                }
            }
        }


        //TODO: duplicated in ACC, DIR and BTMController
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

        //TODO: duplicated in ACC, DIR and BTMController
        public BusinessTrip RewriteBTsPropsFromRepositoryWhenReject(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();

            //all editable data for BTM
            //bTrip.RowVersion = businessTripFromRepository.RowVersion;

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

    }
}