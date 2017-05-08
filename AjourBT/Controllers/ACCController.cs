using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Abstract;
using System.Text;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Concrete;
using System.Data.Entity.Infrastructure;


namespace AjourBT.Controllers
{
    [Authorize(Roles = "ACC")]
    public class ACCController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;
        private StringBuilder comment = new StringBuilder();
        private string defaultAccComment;

        //TODO: duplicated in BusinessTripController
        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";
        private string btCreationError = "Absence already planned on this period for this user. "
                                      + "Please change OrderDates or if BT haven\'t OrderDates "
                                      + "change \'From\' or \'To\'";

        public ACCController(IRepository repository, IMessenger messenger)
        {
            this.repository = repository;
            this.messenger = messenger;
            this.comment = this.comment.Append("ВКО №   від   , cума:   UAH.");
            this.comment = this.comment.AppendLine();
            this.comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }


        [Authorize(Roles = "ACC")]
        public ActionResult GetBusinessTrip(string selectedDepartment = null, string searchString = "")
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View();
        }


        [Authorize(Roles = "ACC")]
        public ViewResult IndexForAccountableBTs(string searchString = "")//searchString
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<BusinessTrip> selectedData = SearchBusinessTripAccountableACC(repository.BusinessTrips.ToList(), searchString);

            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return View(selectedData);

        }

        [Authorize(Roles = "ACC")]
        public PartialViewResult GetBusinessTripACC(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView();
        }

        [Authorize(Roles = "ACC")]
        public PartialViewResult GetBusinessTripDataACC(string selectedDepartment = null, string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<BusinessTrip> selectedData = new List<BusinessTrip>();

            selectedData = SearchBusinessTripDataACC(repository.BusinessTrips.ToList(), selectedDepartment, searchString);

            ViewBag.SearchString = searchString;
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return PartialView(selectedData);
        }

        [Authorize(Roles = "ACC")]
        public PartialViewResult GetClosedBusinessTrip(string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            return PartialView();
        }

        [Authorize(Roles = "ACC")]
        public PartialViewResult GetClosedBusinessTripData(string searchString = "")
        {
            List<BusinessTrip> closedBTs = new List<BusinessTrip>();

            closedBTs = SearchClosedBusinessTripData(repository.BusinessTrips.ToList(), searchString);

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            return PartialView(closedBTs);
        }

        public List<BusinessTrip> SearchClosedBusinessTripData(List<BusinessTrip> closedBtList, string searchString)
        {
            List<BusinessTrip> result = (from bt in closedBtList
                                         join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                                         join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                                         where ((e.DateDismissed == null
                                                     && (e.EID.ToLower().Contains(searchString.ToLower())
                                                             || e.FirstName.ToLower().Contains(searchString.ToLower())
                                                             || e.LastName.ToLower().Contains(searchString.ToLower())))
                                               && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                                     || bt.Status == (BTStatus.Confirmed | BTStatus.Modified))
                                               && ((!String.IsNullOrEmpty(bt.AccComment) && bt.AccComment != defaultAccComment))))
                                         orderby bt.StartDate, e.LastName
                                         select bt).ToList();
            return result;
        }

        public List<BusinessTrip> SearchBusinessTripDataACC(List<BusinessTrip> btList, string selectedDepartment, string searchString)
        {
            List<BusinessTrip> query = (from bt in btList
                                        join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                                        join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                                        where ((selectedDepartment == null || selectedDepartment == String.Empty || d.DepartmentName == selectedDepartment)
                                              && (e.DateDismissed == null
                                                    && (e.EID.ToLower().Contains(searchString.ToLower())
                                                            || e.FirstName.ToLower().Contains(searchString.ToLower())
                                                            || e.LastName.ToLower().Contains(searchString.ToLower())))
                                              && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Modified))
                                              && ((bt.EndDate.Date >= DateTime.Now.ToLocalTimeAzure().Date)
                                                    || (bt.AccComment == null || bt.AccComment == ""
                                                    || bt.AccComment == defaultAccComment))))
                                        orderby bt.StartDate, e.LastName
                                        select bt).ToList();
            return query;
        }

        public List<BusinessTrip> SearchBusinessTripAccountableACC(List<BusinessTrip> btList, string searchString)
        {
            List<BusinessTrip> result = (from bt in repository.BusinessTrips as IEnumerable<BusinessTrip>
                                         join e in repository.Employees on bt.EmployeeID equals e.EmployeeID
                                         join d in repository.Departments on e.DepartmentID equals d.DepartmentID
                                         where (e.DateDismissed == null
                                               && (e.EID.ToLower().Contains(searchString.ToLower())
                                                  || e.FirstName.ToLower().Contains(searchString.ToLower())
                                                  || e.LastName.ToLower().Contains(searchString.ToLower())))
                                               && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported))
                                               && ((bt.OrderEndDate != null && bt.OrderEndDate.Value.Date <= DateTime.Now.ToLocalTimeAzure().Date) && (bt.OrderEndDate != null && bt.OrderEndDate.Value.Date >= (DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5)))))
                                         orderby bt.OrderEndDate.Value descending, e.LastName
                                         select bt).ToList();

            return result;
        }

        //TODO: duplicated in BusinessTripController
        private SelectList LocationsDropDownList(int selectedLocationID = 0)
        {
            var locationList = from loc in repository.Locations
                               orderby loc.Title
                               select loc;

            return new SelectList(locationList, "LocationID", "Title", selectedLocationID);
        }

        //TODO: duplicated in BTMController
        public int CountingDaysUsedInBT(BusinessTrip businessTrip)
        {
            //'+1' day for counting last day too
            return ((businessTrip.EndDate - businessTrip.StartDate).Days + 1);
        }

        //
        // GET: /ACC/EditReportedBTACC/5

        [Authorize(Roles = "ACC")]
        public ActionResult EditReportedBT(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            BusinessTrip businessT = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessT == null || (!businessT.Status.HasFlag(BTStatus.Confirmed | BTStatus.Reported)))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = businessT.BTof.LastName + " " + businessT.BTof.FirstName + " (" + businessT.BTof.EID + ") from " + businessT.BTof.Department.DepartmentName;
            }

            if (businessT.AccComment == null || businessT.AccComment == String.Empty)
            {
                businessT.AccComment = defaultAccComment;
            }

            BusinessTripViewModel businessTrip = new BusinessTripViewModel(businessT);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.LocationsList = LocationsDropDownList(businessT.LocationID);

            if (businessT.StartDate > DateTime.Now.ToLocalTimeAzure().Date && businessT.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFutureBT", businessTrip);
            }
            else if (businessT.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFinishedBT", businessTrip);
            }
            else
            {
                return View("EditReportedCurrentBT", businessTrip);
            }

        }

        //
        // POST: /ACC/EditReportedBTACC/

        [Authorize(Roles = "ACC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReportedBT(BusinessTrip businessTrip, string selectedDepartment = null)
        {
            BusinessTrip btFromRepository = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == businessTrip.BusinessTripID).FirstOrDefault());

            if (businessTrip == null || btFromRepository == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (ModelState.IsValid && businessTrip.Status.HasFlag(BTStatus.Reported))
                {
                    LocationsDropDownList(businessTrip.LocationID);

                    Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();

                    businessTrip.Status = (btFromRepository.Status | BTStatus.Modified) & ~BTStatus.Reported;
                    businessTrip.OldStartDate = btFromRepository.StartDate;
                    businessTrip.OldEndDate = btFromRepository.EndDate;
                    businessTrip.OldLocationID = btFromRepository.LocationID;
                    businessTrip.OldLocationTitle = btFromRepository.Location.Title;

                    int oldDaysUsedInBT = CountingDaysUsedInBT(btFromRepository);

                    //btFromRepository.StartDate = businessTrip.StartDate;
                    //btFromRepository.EndDate = businessTrip.EndDate;
                    //btFromRepository.LocationID = businessTrip.LocationID;
                    //btFromRepository.OrderStartDate = businessTrip.OrderStartDate;
                    //btFromRepository.OrderEndDate = businessTrip.OrderEndDate;
                    //btFromRepository.DaysInBtForOrder = businessTrip.DaysInBtForOrder;
                    //btFromRepository.RowVersion = businessTrip.RowVersion;

                    businessTrip.BTof = btFromRepository.BTof;
                    businessTrip.CancelComment = btFromRepository.CancelComment;
                    businessTrip.Comment = btFromRepository.Comment;
                    businessTrip.Flights = btFromRepository.Flights;
                    businessTrip.FlightsConfirmed = btFromRepository.FlightsConfirmed;
                    businessTrip.Habitation = btFromRepository.Habitation;
                    businessTrip.HabitationConfirmed = btFromRepository.HabitationConfirmed;
                    businessTrip.Invitation = btFromRepository.Invitation;
                    businessTrip.Manager = btFromRepository.Manager;
                    businessTrip.Purpose = btFromRepository.Purpose;
                    businessTrip.RejectComment = btFromRepository.RejectComment;
                    businessTrip.BTMComment = btFromRepository.BTMComment;
                    businessTrip.Responsible = btFromRepository.Responsible;
                    businessTrip.LastCRUDedBy = btFromRepository.LastCRUDedBy;
                    businessTrip.LastCRUDTimestamp = btFromRepository.LastCRUDTimestamp;
                    businessTrip.Location = btFromRepository.Location;
                    businessTrip.AccComment = btFromRepository.AccComment;



                    if (businessTrip.BTof.Visa != null)
                    {
                        businessTrip.BTof.Visa.DaysUsedInBT -= oldDaysUsedInBT;
                        businessTrip.BTof.Visa.EntriesUsedInBT--;
                    }

                    try
                    {
                        repository.SaveBusinessTrip(businessTrip);
                        selectedBusinessTripsList.Add(businessTrip);

                        messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToADM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToBTM, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToDIR, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToEMP, selectedBusinessTripsList, author));
                        messenger.Notify(new Message(MessageType.ACCModifiesConfirmedReportedToResponsible, selectedBusinessTripsList, author));
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
            LocationsDropDownList(businessTrip.LocationID);
            btTripModel.BTof = repository.BusinessTrips.Where(bt => bt.BusinessTripID == businessTrip.BusinessTripID).Select(b => b.BTof).FirstOrDefault();
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.LocationsList = LocationsDropDownList(btTripModel.LocationID);

            if (businessTrip.StartDate > DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFutureBT", btTripModel);
            }
            else if (businessTrip.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
            {
                return View("EditReportedFinishedBT", businessTrip);
            }
            else
            {
                return View("EditReportedCurrentBT", businessTrip);
            }
        }

        [Authorize(Roles = "ACC")]
        public ActionResult CancelReportedBT(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel btTripmodel = new BusinessTripViewModel(businessTrip);
            return View(btTripmodel);
        }

        [Authorize(Roles = "ACC")]
        [HttpPost, ActionName("CancelReportedBT")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelReportedBTConfirm(int id = 0, string cancelComment = "", string selectedDepartment = null)
        {
            BusinessTrip businessTrip = null;
            if (repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault() == null)
                return HttpNotFound();
            else
            {
                businessTrip = new BusinessTrip(repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault());
                businessTrip.CancelComment = cancelComment;

                if ((businessTrip.Status == (BTStatus.Confirmed | BTStatus.Reported))
                    && ((businessTrip.CancelComment != null) && (businessTrip.CancelComment != String.Empty)))
                {
                    Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
                    businessTrip.Status = BTStatus.Confirmed | BTStatus.Cancelled;

                    if (businessTrip.BTof.Visa != null)
                    {
                        businessTrip.BTof.Visa.DaysUsedInBT -= CountingDaysUsedInBT(businessTrip);
                        businessTrip.BTof.Visa.EntriesUsedInBT--;
                    }
                    try
                    {
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
                    selectedBusinessTripsList.Add(businessTrip);
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToADM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToBTM, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToEMP, selectedBusinessTripsList, author));
                    messenger.Notify(new Message(MessageType.ACCCancelsConfirmedReportedToResponsible, selectedBusinessTripsList, author));
                    return Json(new { success = "success" });

                }
                else
                {
                    ModelState.AddModelError("", "Please, enter cancel comment.");
                    BusinessTripViewModel bTripmodel = new BusinessTripViewModel(businessTrip);
                    ViewBag.SelectedDepartment = selectedDepartment;

                    return View(bTripmodel);
                }
            }
        }

        //TODO: duplicated in ACC, BTM, BusinessTripController
        public BusinessTrip RewriteBTsPropsFromRepositoryBTM(BusinessTrip bTrip)
        {
            BusinessTrip businessTripFromRepository = repository.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();
            //bTrip.BTMComment = businessTripFromRepository.BTMComment;
            //bTrip.Habitation = businessTripFromRepository.Habitation;
            //bTrip.HabitationConfirmed = businessTripFromRepository.HabitationConfirmed;
            //bTrip.Flights = businessTripFromRepository.Flights;
            //bTrip.FlightsConfirmed = businessTripFromRepository.FlightsConfirmed;
            //bTrip.Invitation = businessTripFromRepository.Invitation;
            //bTrip.RowVersion = businessTripFromRepository.RowVersion;

            //if ( !( (bTrip.Status == BTStatus.Confirmed || bTrip.Status == (BTStatus.Confirmed | BTStatus.Modified)) && (bTrip.Status != BTStatus.Cancelled) && (bTrip.Status != BTStatus.Reported)) )
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

        //TODO: duplicated in ACC, BTM, BusinessTripController
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


        [Authorize(Roles = "ACC")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveAccComment(BusinessTrip bTrip)
        {
            if (bTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (bTrip.Status == (BTStatus.Confirmed | BTStatus.Reported))
                {
                    bTrip = RewriteBTsPropsFromRepositoryBTM(bTrip);
                    bTrip = RewriteBTsPropsFromRepositoryWhenReject(bTrip);

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

                    //return RedirectToAction("ACCView", "Home", new { selectedDepartment = selectedDepartment });
                    return Json(new { success = "success" });
                }
            }
            return Json(new { error = "error" });
        }

        [Authorize(Roles = "ACC")]
        public ActionResult ShowAccountableBTData(int id = 0)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            BusinessTripViewModel businessTripModel = new BusinessTripViewModel(businessTrip);

            return View(businessTripModel);
        }

        [Authorize(Roles = "ACC")]
        public ActionResult ShowClosedBT(int id = 0, string searchString = "")
        {
            BusinessTrip bTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (bTrip == null)
            {
                return HttpNotFound();
            }

            BusinessTripViewModel btModel = new BusinessTripViewModel(bTrip);

            return View(btModel);
        }
    }
}
