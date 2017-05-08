using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    [Authorize(Roles = "ABM")]
    public class HolidayController : Controller
    {
        private IRepository repository;
        public HolidayController(IRepository repo)
        {
            this.repository = repo;
        }

        private List<Holiday> GetHolidaysList(int Year, int countryID)
        {
            List<Holiday> holList = (from holiday in repository.Holidays
                                     where (holiday.HolidayDate.Year == Year && holiday.CountryID == countryID)
                                     orderby holiday.HolidayDate
                                     select holiday).ToList();
            return holList;
        }
        
        public ActionResult GetHoliday()
        {        
            ViewBag.YearDropdownList = YearDropDownList();
            ViewBag.CountryDropdownList = CountryDropDownList();
                 
            var year = (from hol in repository.Holidays
                        orderby hol.HolidayDate descending
                        select hol.HolidayDate.Year).FirstOrDefault();
            if (repository.Holidays.Any(h => h.HolidayDate.Year == DateTime.Now.Year))
            {
                ViewBag.DefaultYear = DateTime.Now.Year;
            }
            else
            {
                ViewBag.DefaultYear = year;
            }
            ViewBag.DefaultCountry = SelectDefaultCountryID();
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return View();
        }

        public SelectList CountryDropDownList()
        {
            var countryList = (from countries in repository.Countries
                               select countries).ToList();
            int countryID = SelectDefaultCountryID();
            return new SelectList(countryList, "CountryID", "CountryName", countryID);
        }

        public int SelectDefaultCountryID()
        {
            return 1;
            //return (from country in repository.Countries
            //        where country.CountryName == "Ukraine"
            //        select country.CountryID).First();

        }

        public SelectList YearDropDownList()
        {
            var yearList = (from hol in repository.Holidays
                            select hol.HolidayDate.Date.Year).Distinct().ToList();

            var year = (from hol in repository.Holidays
                        orderby hol.HolidayDate descending
                        select hol.HolidayDate.Year).FirstOrDefault();
                       
            
              if (repository.Holidays.Any(h => h.HolidayDate.Year == DateTime.Now.Year))
              {
                  return new SelectList(yearList, DateTime.Now.Year );
              }
              else
                      return new SelectList(yearList, year);
            
        }

        public PartialViewResult GetHolidayData(string selectedYear, string selectedCountryID)
        {
            List<Holiday> holList = (from holiday in repository.Holidays
                                     where (holiday.HolidayDate.Year.ToString() == selectedYear && holiday.CountryID == Int32.Parse(selectedCountryID))
                                     orderby holiday.HolidayDate
                                     select holiday).ToList();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(holList);
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Holiday holiday = (from h in repository.Holidays where h.HolidayID == id select h).FirstOrDefault();

            if (holiday == null)
            {
                return HttpNotFound();
            }

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return View(holiday);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(Holiday holiday)
        {
            string modelError = "";
            try
            {
                Holiday holTitle = repository.Holidays.Where(h => h.Title == holiday.Title && h.CountryID == holiday.CountryID && h.HolidayDate.Year == holiday.HolidayDate.Year).FirstOrDefault();
                if( holTitle != null && holTitle.HolidayID != holiday.HolidayID)
                {
                    ModelState.AddModelError("Title", "Holiday with same Title already exist");
                }

                Holiday holDate = repository.Holidays.Where(h => h.HolidayDate == holiday.HolidayDate && h.CountryID == holiday.CountryID).FirstOrDefault();
                if (holDate != null && holDate.HolidayID != holiday.HolidayID)
                {
                    ModelState.AddModelError("HolidayDate", "Holiday with same Date already exist");
                }

                if (ModelState.IsValid)
                {
                    repository.SaveHoliday(holiday);
                }
                else
                {
                    return View(holiday);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                     + "was modified by another user after you got the original value. The "
                                     + "edit operation was canceled.";

                return Json(new { error = modelError });
            }

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            List<Holiday> holidays = GetHolidaysList(holiday.HolidayDate.Year, holiday.CountryID);
            return View("GetHolidayData", holidays);
        }

        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            Holiday holiday = (from h in repository.Holidays where h.HolidayID == id select h).FirstOrDefault();
            if (holiday == null)
            {
                return HttpNotFound();
            }

            return View(holiday);
        }

        [ValidateAntiForgeryToken]
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Holiday holiday = repository.Holidays.Where(h => h.HolidayID == id).FirstOrDefault();

            try
            {
                repository.DeleteHoliday(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }
            
            List<Holiday> holidays = GetHolidaysList(holiday.HolidayDate.Year, holiday.CountryID);
            return View("GetHolidayData", holidays);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.CountryList = CountryDropDownList();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(Holiday holiday)
        {
            if (repository.Holidays.Where(h => h.Title == holiday.Title && h.CountryID == holiday.CountryID && h.HolidayDate.Year == holiday.HolidayDate.Year).FirstOrDefault() != null)
            {
                ModelState.AddModelError("Title", "Holiday with same Title already exist");
            }
            if (repository.Holidays.Where(h => h.HolidayDate == holiday.HolidayDate && h.CountryID == holiday.CountryID).FirstOrDefault() != null)
            {
                ModelState.AddModelError("HolidayDate", "Holiday with same Date already exist");
            }

            if (ModelState.IsValid)
            {
                repository.SaveHoliday(holiday);
            }
            else
            {
                ViewBag.CountryList = CountryDropDownList();
                return View(holiday);
            }
            ViewBag.CountryList = CountryDropDownList();

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            List<Holiday> holidays = GetHolidaysList(holiday.HolidayDate.Year, holiday.CountryID);
            return View("GetHolidayData",holidays);
        }
    }
}
