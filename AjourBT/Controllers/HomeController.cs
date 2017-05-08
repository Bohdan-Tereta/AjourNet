using AjourBT.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        
        [Authorize(Roles="DIR")]
        public ActionResult DIRView(int tab = Tabs.DIR.BusinessTrips)
        {
            return View(tab);
        }
        
        [Authorize(Roles = "PU")]
        public ActionResult PUView(int tab = Tabs.PU.Departments, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "BTM")]
        public ActionResult BTMView(int tab = Tabs.BTM.VisasAndPermits, string searchString="")
        {
            //ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "ADM")]
        public ActionResult ADMView(int tab = Tabs.ADM.VisasAndPermits, string selectedDepartment=null)
        {
            ViewBag.SelectedDepartment = selectedDepartment; 
            return View(tab);
        }

        [Authorize(Roles = "ACC")]
        public ActionResult ACCView(int tab = Tabs.ACC.CurrentAndFutureBTs, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "VU")]
        public ActionResult VUView(int tab = Tabs.VU.BTsByDatesAndLocation)
        {
            return View(tab);
        }

        [Authorize(Roles = "EMP")]
        public ActionResult EMPView(int tab = Tabs.EMP.YourBTs)
        {
            return View(tab);
        }

        [Authorize(Roles = "ABM")]
        public ActionResult ABMView(int tab = Tabs.ABM.Countries)
        {
            return View(tab);
        }

        [Authorize(Roles = "BDM")]
        public ActionResult BDMView(int tab = Tabs.BDM.Greetings)
        {
            return View(tab);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRView(int tab = Tabs.HR.Foo)
        {
            return View(tab);
        }

        [Authorize]
        public ActionResult HelpView(int tab = Tabs.Help.Map)
        {
            return View(tab);
        }

        public ActionResult DataBaseDeleteError()
        {
            return View();
        }

    }
}