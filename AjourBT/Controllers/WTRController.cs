using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
//using AjourBT.Helpers;
using AjourBT.Domain.ViewModels;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Infrastructure;

namespace AjourBT.Controllers
{
    public class WTRController : Controller
    {
        private WTRController()
        {
            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;

        }

        private IRepository repository;
        private IXLSExporter xlsExporter; 

        public WTRController(IRepository repo, IXLSExporter xlsExporter)
            : this()
        {

            this.repository = repo; 
            this.xlsExporter = xlsExporter;
        }

        [Authorize(Roles = "ABM")]
        public ActionResult GetWTR()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.FromText = new DateTime(currYear, currMonth, 01);
            ViewBag.ToText = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));

            return View();
        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetAbsencePerEMP()
        {
            return View();
        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetWTRPerEMP()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.FromText = new DateTime(currYear, currMonth, 01);
            ViewBag.ToText = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));

            return View();
        }

        [Authorize(Roles = "ABM")]
        public PartialViewResult GetWTRData(string From = "", string To = "", string searchString = "")
        {
            DateTime fromParsed = DateTime.Now;
            DateTime toParse = DateTime.Now;
            int FromYear = DateTime.Now.Year;
            int ToYear = DateTime.Now.Year;

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            if (From != "" && To != "")
            {
                try
                {
                    fromParsed = DateTime.ParseExact(From, "dd.MM.yyyy", null);
                    toParse = DateTime.ParseExact(To, "dd.MM.yyyy", null);
                    FromYear = fromParsed.Year;
                    ToYear = toParse.Year;
                }
                catch (System.FormatException)
                {
                    return PartialView("GetWTRDataEmpty");
                }
            }
            else
            {
                return PartialView("GetWTRDataEmpty");
            }
            if (fromParsed > toParse)
            {
                return PartialView("GetWTRDataEmpty");
            }

            searchString = searchString != "" ? searchString.Trim() : "";
            IList<WTRViewModel> wtrDataList = repository.SearchWTRData(fromParsed, toParse, searchString);             

            ViewBag.FromWeek = cal.GetWeekOfYear(fromParsed, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.ToWeek = cal.GetWeekOfYear(toParse, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;

            ViewBag.fromDate = From;
            ViewBag.toDate = To;

            return PartialView(wtrDataList);
        }

        [Authorize(Roles = "EMP")]
         public PartialViewResult GetWTRDataPerEMP(string From = "", string To = "", string userName = "")
        {
            DateTime fromParsed = DateTime.Now;
            DateTime toParse = DateTime.Now;
            int FromYear = DateTime.Now.Year;
            int ToYear = DateTime.Now.Year;

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            if (From != "" && To != "")
            {
                try
                {
                    fromParsed = DateTime.ParseExact(From, "dd.MM.yyyy", null);
                    toParse = DateTime.ParseExact(To, "dd.MM.yyyy", null);
                    FromYear = fromParsed.Year;
                    ToYear = toParse.Year;
                }
                catch (System.FormatException)
                {
                    return PartialView("GetWTRDataEmpty");
                }
            }
            else
            {
                return PartialView("GetWTRDataEmpty");
            }
            if (fromParsed > toParse)
            {
                return PartialView("GetWTRDataEmpty");
            }

            Employee employee = repository.Employees.Where(e => e.EID == userName).FirstOrDefault();

            if (employee == null)
            {
                return PartialView("NoData");
            }

            IList<WTRViewModel> wtrDataList = repository.SearchWTRDataPerEMP(fromParsed, toParse, employee); 

            ViewBag.FromWeek = cal.GetWeekOfYear(fromParsed, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.ToWeek = cal.GetWeekOfYear(toParse, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;
            ViewBag.UserName = userName;
            ViewBag.fromDate = From;
            ViewBag.toDate = To;

            return PartialView("GetWTRDataPerEMP", wtrDataList);
        }

        [Authorize(Roles = "ABM")]
        public ActionResult ExportWTR(string searchString, DateTime fromDate, DateTime toDate)
        {
            IList<WTRViewModel> wtrDataList = repository.SearchWTRData(fromDate, toDate, searchString);

            return File(xlsExporter.ExportWTR(fromDate, toDate, wtrDataList), "application/vnd.ms-excel", "WTR.xls");
        }

        [Authorize(Roles = "EMP")]
        public ActionResult ExportWTRForEMP(string searchString, DateTime fromDate, DateTime toDate)
                    {
            Employee employee = repository.Employees.Where(e => e.EID == searchString).FirstOrDefault();
            IList<WTRViewModel> wtrDataList = repository.SearchWTRDataPerEMP(fromDate, toDate, employee);
            return File(xlsExporter.ExportWTR(fromDate, toDate, wtrDataList), "application/vnd.ms-excel", "WTR.xls");
                    }

    }
}
