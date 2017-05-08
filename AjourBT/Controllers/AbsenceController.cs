using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using AjourBT.Domain.ViewModels;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class AbsenceController : Controller
    {
        private IRepository repository; 
        private IXLSExporter xlsExporter; 

        public AbsenceController(IRepository repo, IXLSExporter xlsExporter)
        {
            repository = repo;
            this.xlsExporter = xlsExporter; 
        }

        public ActionResult GetAbsence(string searchString = "")
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.SearchString = searchString;
            ViewBag.FromValue = new DateTime(currYear, currMonth, 01);
            ViewBag.ToValue = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));
            return View();
        }

        public ActionResult GetAbsenceData(string fromDate, string toDate, string searchString = "")
        {
            IList<AbsenceViewModel> absenceData = SearchAbsenceData(fromDate, toDate, searchString);
            if (absenceData.Count == 0)
            {
                return PartialView("NoAbsenceData");
            }

            ViewBag.SearchString = searchString;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(absenceData);
        }

        public IList<AbsenceViewModel> SearchAbsenceData(string fromDate, string toDate, string searchString = "")
        {

            DateTime parseFromDate = new DateTime();
            DateTime parseToDate = new DateTime();
            searchString = searchString != "" ? searchString.Trim() : "";

            try
            {
                parseFromDate = DateTime.ParseExact(fromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(toDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)
            {
                return new List<AbsenceViewModel>();
            }

            return repository.SearchAbsenceData(parseFromDate, parseToDate, searchString); 
            }

        [Authorize(Roles = "EMP")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExportAbsenceToExcel(string from, string to, string searchString = "")
        {
            IList<AbsenceViewModel> absenceData = SearchAbsenceData(from, to, searchString);
            return File(xlsExporter.ExportAbsenceToExcel(absenceData), "application/vnd.ms-excel", "Absence.xls");
        }

    }
}
