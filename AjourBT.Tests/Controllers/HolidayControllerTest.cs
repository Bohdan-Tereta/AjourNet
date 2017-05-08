using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Tests.MockRepository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class HolidayControllerTest
    {
        Mock<IRepository> mock;
        HolidayController controller;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            controller = new HolidayController(mock.Object);
        }
        [Test]
        public void GetHoliday_Default_ReturnView()
        {
            //Arrange
            SelectList expYearList = controller.YearDropDownList();
            SelectList expCountryList = controller.CountryDropDownList();
            int expCountryID = controller.SelectDefaultCountryID();
            //Act
            var result = controller.GetHoliday();
            SelectList actualYearList = ((ViewResult)result).ViewBag.YearDropdownList as SelectList;
            SelectList actualCountryList = ((ViewResult)result).ViewBag.CountryDropdownList as SelectList;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(expYearList.Items, actualYearList.Items);
            Assert.AreEqual(expCountryList.Items, actualCountryList.Items);
            Assert.AreEqual(expCountryID, ((ViewResult)result).ViewBag.DefaultCountry);

        }

        [Test]
        public void GetHoliday_DefaultCurrentYear_ReturnDefaultYear()
        {
            //Arrange
            SelectList expYearList = controller.YearDropDownList();
            SelectList expCountryList = controller.CountryDropDownList();
            int expCountryID = controller.SelectDefaultCountryID();
            int selectedYear = 2014;
            //Act
            var result = controller.GetHoliday();
            SelectList actualYearList = ((ViewResult)result).ViewBag.YearDropdownList as SelectList;
            SelectList actualCountryList = ((ViewResult)result).ViewBag.CountryDropdownList as SelectList;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(expYearList.Items, actualYearList.Items);
            Assert.AreEqual(expCountryList.Items, actualCountryList.Items);
            Assert.AreEqual(selectedYear, ((ViewResult)result).ViewBag.DefaultYear);
            Assert.AreEqual(expCountryID, ((ViewResult)result).ViewBag.DefaultCountry);

        }
        [Test]
        public void GetHoliday_DefaultLastYear_ReturnDefaultYear()
        {
            //Arrange
            SelectList expYearList = controller.YearDropDownList();
            SelectList expCountryList = controller.CountryDropDownList();
            int expCountryID = controller.SelectDefaultCountryID();
            //Act
            var result = controller.GetHoliday();
            SelectList actualYearList = ((ViewResult)result).ViewBag.YearDropdownList as SelectList;
            SelectList actualCountryList = ((ViewResult)result).ViewBag.CountryDropdownList as SelectList;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(expYearList.Items, actualYearList.Items);
            Assert.AreEqual(expCountryList.Items, actualCountryList.Items);
            Assert.AreEqual(2014, ((ViewResult)result).ViewBag.DefaultYear);
            Assert.AreEqual(expCountryID, ((ViewResult)result).ViewBag.DefaultCountry);

        }

        [Test]
        public void GetHoliday_Default_ReturnViewViewBagsAreNotNull()
        {
            //Arrange
            //Act
            ActionResult result = controller.GetHoliday();

            //Assert
            Assert.NotNull(((ViewResult)result).ViewBag.YearDropdownList);
            Assert.NotNull(((ViewResult)result).ViewBag.CountryDropdownList);
            Assert.NotNull(((ViewResult)result).ViewBag.DefaultYear);
            Assert.NotNull(((ViewResult)result).ViewBag.DefaultCountry);

        }


        [Test]
        public void CountryDropDownList_Default_ListofAllCountries()
        {
            //Arrange
            List<Country> countryList = (from countries in mock.Object.Countries
                                         select countries).ToList();

            //Act
            SelectList result = controller.CountryDropDownList();
            //Assert
            Assert.AreEqual(countryList, result.Items);
            Assert.AreEqual(5, result.Count());
            Assert.AreEqual(1, result.SelectedValue);
            Assert.AreEqual("Ukraine", result.First().Text);
            Assert.AreEqual("Zimbabve", result.Last().Text);

        }

        [Test]
        public void SelectDefaultCountryID_Default_UkraineID()
        {
            //Arrange
            var expCountryID = (from country in mock.Object.Countries
                                where country.CountryName == "Ukraine"
                                select country.CountryID).First();

            //Act
            int result = controller.SelectDefaultCountryID();
            //Assert
            Assert.AreEqual(expCountryID, result);


        }

        [Test]
        public void YearDropDownList_Default_AllYearList()
        {
            //Arrange
            List<int> yearList = (from holidays in mock.Object.Holidays
                                  select holidays.HolidayDate.Year).Distinct().ToList();

            //Act
            SelectList result = controller.YearDropDownList();
            //Assert
            Assert.AreEqual(yearList, result.Items);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2014, result.SelectedValue);
            Assert.AreEqual("2013", result.First().Text);
            Assert.AreEqual("2014", result.Last().Text);


        }


        [Test]
        public void GetHolidayData_2013Ukraine_HolidaysIn2013()
        {
            //Arrange
            string selectedYear = "2013";
            string selectedCountryID = "1";
            //Act
            var result = controller.GetHolidayData(selectedYear, selectedCountryID);
            List<Holiday> holList = (List<Holiday>)controller.GetHolidayData(selectedYear, selectedCountryID).Model;
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Holiday>), ((PartialViewResult)result).Model);
            Assert.AreEqual(10, holList.Count);

        }


        [Test]
        public void GetHolidayData_2014Ukraine_HolidaysIn2014()
        {
            //Arrange
            string selectedYear = "2014";
            string selectedCountryID = "1";
            //Act
            var result = controller.GetHolidayData(selectedYear, selectedCountryID);
            List<Holiday> holList = (List<Holiday>)controller.GetHolidayData(selectedYear, selectedCountryID).Model;
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Holiday>), ((PartialViewResult)result).Model);
            Assert.AreEqual(10, holList.Count);

        }
       
        [Test]
        public void GetHolidayData_2014Poland_HolidaysIn2014()
        {
            //Arrange
            string selectedYear = "2014";
            string selectedCountryID = "2";
            //Act
            var result = controller.GetHolidayData(selectedYear, selectedCountryID);
            List<Holiday> holList = (List<Holiday>)controller.GetHolidayData(selectedYear, selectedCountryID).Model;
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Holiday>), ((PartialViewResult)result).Model);
            Assert.AreEqual(3, holList.Count);

        }

        [Test]
        public void EditGet_CanEdit_ValidHoliday()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidHoliday()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(150);
            Holiday Holiday = mock.Object.Holidays.Where(m => m.HolidayID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Holiday);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void EditPost_CanEdit_ValidHolidayFalse()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);
            Holiday holiday = new Holiday { HolidayID = 1, CountryID = 1, HolidayComment = "Test Comment", HolidayDate = new DateTime(2014, 05, 05), Title = "New Year" };


            // Act - call the action method 
            var result = (ViewResult)target.Edit(holiday);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<Holiday>), result.ViewData.Model);
            mock.Verify(m => m.SaveHoliday(holiday), Times.Once);
        }

        [Test]
        public void EditPost_CanEdit_ValidHolidayTrue()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);
            Holiday holiday = new Holiday { HolidayID = 1, CountryID = 1, HolidayComment = "Test Comment", HolidayDate = new DateTime(2014, 05, 05), Title = "New Year",IsPostponed = true };


            // Act - call the action method 
            var result = (ViewResult)target.Edit(holiday);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<Holiday>), result.ViewData.Model);
            mock.Verify(m => m.SaveHoliday(holiday), Times.Once);
        }

        [Test]
        public void EditPost_CannotEdit_InvalidHolidayCheckboxFalse()
        {
            // Arrange - create the controller 
            Holiday holiday = new Holiday { HolidayDate = new DateTime(2014, 01, 01), IsPostponed = true, CountryID = 1, Title = "TEST" }; HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = (ViewResult)target.Edit(holiday);

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(holiday), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(Holiday), result.ViewData.Model);
        }

        [Test]
        public void EditPost_CannotEdit_InvalidHolidayCheckboxTrue()
        {
            // Arrange - create the controller 
            Holiday holiday = new Holiday { HolidayDate = new DateTime(2014, 04, 04), IsPostponed = true, CountryID = 1, Title = "TEST" };
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = (ViewResult)target.Edit(holiday);

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(holiday), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(Holiday), result.ViewData.Model);
        }
        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            HolidayController controller = new HolidayController(mock.Object);
            Holiday holiday = new Holiday { HolidayDate = new DateTime(2014, 05, 05), IsPostponed = false, CountryID = 1, Title = "TEST" };
            mock.Setup(m => m.SaveHoliday(It.IsAny<Holiday>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            var result = (JsonResult)controller.Edit(mock.Object.Holidays.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveHoliday(It.IsAny<Holiday>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeleteGet_ValidCountryWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(3) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Holiday), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void DeleteGet_InvalidHoliday()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(150);
            Holiday Holiday = mock.Object.Holidays.Where(m => m.HolidayID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Holiday);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void DeletePost_CanDelete_ValidHoliday()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            var result = (ViewResult)target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteHoliday(1), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<Holiday>), result.ViewData.Model);

        }

        [Test]
        public void DeletePost_CannotDelete_ValidHoliday()
        {
            // Arrange - create the controller 
            HolidayController target = new HolidayController(mock.Object);
            mock.Setup(x => x.DeleteHoliday(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteHoliday(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);
        }

        [Test]
        public void CreatePost_CanCreate_ValidHolidayCheckboxTrue()
        {
            // Arrange - create the controller                 
            HolidayController target = new HolidayController(mock.Object);
            Holiday Holiday = new Holiday { HolidayID = 1, Title = " Test", HolidayDate = new DateTime(2014,11,11), IsPostponed = true};

            // Act - call the action method 
            var result = (ViewResult)target.Create(Holiday);

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(Holiday), Times.Once);
            Assert.IsInstanceOf(typeof(List<Holiday>), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }


        [Test]
        public void CreatePost_CanCreate_ValidHolidayCheckboxFalse()
        {
            // Arrange - create the controller                 
            HolidayController target = new HolidayController(mock.Object);
            Holiday Holiday = new Holiday { HolidayID = 100, Title = "Test", HolidayDate = new DateTime(2014, 09, 09), IsPostponed = false };

            // Act - call the action method 
            var result = (ViewResult)target.Create(Holiday);

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(Holiday), Times.Once);
            Assert.IsInstanceOf(typeof(List<Holiday>), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidHolidayChecboxFalse()
        {
            // Arrange - create the controller
            Holiday Holiday = new Holiday { HolidayID = 1, Title = "", HolidayDate = new DateTime(2014, 01, 01), IsPostponed = false };
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(Holiday) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(It.IsAny<Holiday>()), Times.Never);
            Assert.IsInstanceOf(typeof(Holiday), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidHolidayChekboxTrue()
        {
            // Arrange - create the controller
            Holiday Holiday = new Holiday { HolidayID = 1, Title = "", HolidayDate = new DateTime(2014, 01, 01), IsPostponed = true };
            HolidayController target = new HolidayController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(Holiday) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveHoliday(It.IsAny<Holiday>()), Times.Never);
            Assert.IsInstanceOf(typeof(Holiday), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreateGet_ReturnView()
        {
            //Arrange
            HolidayController controller = new HolidayController(mock.Object);
            

            //Act
            var result = controller.Create() as ViewResult;
            SelectList countryList = result.ViewBag.CountryList as SelectList;

            //Assert
            Assert.IsInstanceOf(typeof(SelectList), result.ViewBag.CountryList);
            Assert.AreEqual(5, countryList.ToList().Count);
            Assert.AreEqual("Ukraine", countryList.ToArray()[0].Text);
            Assert.AreEqual("Zimbabve", countryList.ToArray()[4].Text);
            Assert.AreEqual("", result.ViewName);
        }
    }
}
