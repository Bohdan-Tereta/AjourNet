using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
   [Authorize]
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }       
    }
}
