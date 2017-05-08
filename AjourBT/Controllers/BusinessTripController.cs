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

    [Authorize(Roles = "ADM, DIR, BTM, ACC")]
    public class BusinessTripController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        public BusinessTripController(IRepository repository, IMessenger messenger)
        {
            this.repository = repository;
            this.messenger = messenger;
        }

        //TODO: duplicated in ACCController and ADMController
        private SelectList LocationsDropDownList(int selectedLocationID = 0)
        {
            var locationList = from loc in repository.Locations
                               orderby loc.Title
                               select loc;

            return new SelectList(locationList, "LocationID", "Title", selectedLocationID);
        }
    
        #region ADM, BTM, ACC

        //TODO: have analogical ShowBTInformation in VUController
        [Authorize(Roles = "ADM, BTM, ACC")]
        public ActionResult ShowBTData(int id = 0, string selectedDepartment = null)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();
            if (businessTrip == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedDepartment = selectedDepartment;
            BusinessTripViewModel businessTripModel = new BusinessTripViewModel(businessTrip);

            return View(businessTripModel);
        }

 

        #endregion

    }
}