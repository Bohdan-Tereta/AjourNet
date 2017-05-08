
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class CountryController : Controller
    {
        //
        // GET: /Country/
        private IRepository repository;
        private string modelError = "The record you attempted to edit "
                                     + "was modified by another user after you got the original value. The "
                                     + "edit operation was canceled.";
        public CountryController(IRepository repo)
        {
            this.repository = repo;
        }

      
        public ViewResult Index()
        {
            return View(repository.Countries.ToList());
        }
        

        ////
        //// GET: /Country/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        ////
        //// POST: /Country/Create

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(Country country)
        {
            if (repository.Countries.Where(c => c.CountryName == country.CountryName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("CountryName", "Country with same Name already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveCountry(country);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = modelError });
                }
            }
            else
            {
                return View(country);
            }

            List<Country> countries = repository.Countries.ToList();
            return View("Index", countries); 
        }

        ////
        //// GET: /Country/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Country country = (from c in repository.Countries where c.CountryID == id select c).FirstOrDefault();
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        ////
        //// POST: /Country/Edit/5

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(Country country)
        {
            string modelError = "";
            try
            {
                if (repository.Countries.Where(c => c.CountryName == country.CountryName && c.CountryID != country.CountryID).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("CountryName", "Country with same Name already exists");
                }
                if (ModelState.IsValid)
                {
                    repository.SaveCountry(country);
                }
                else
                {
                    return View(country);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

                return Json(new { error = modelError });
            }

            List<Country> countries = repository.Countries.ToList();
            return View("Index", countries);
        }

        ////
        //// GET: /Country/Delete/5
            
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            Country country = (from c in repository.Countries where c.CountryID == id select c).FirstOrDefault();
            if (country == null)
            {
                return HttpNotFound();
            }

            if (id == 1 || country.Locations.Count != 0 || country.Holidays.Count != 0)
            {
                return View("CannotDelete");
            }
            else
                return View(country);
        }

        ////
        //// POST: /Country/Delete/5

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteCountry(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Country> countries = repository.Countries.ToList();
            return View("Index", countries);
        }
    }
}
