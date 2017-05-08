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
    public class CountryControllerTests
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
        }


        [Test]
        [Category("View names")]
        public void IndexView_True()
        {
            // Arrange - create the controller
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            ViewResult result = target.Index();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Index_Default_AllCountries()
        {
            // Arrange - create the controller     
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            IEnumerable<Country> result = (IEnumerable<Country>)target.Index().Model;
            List<Country> CountryView = result.ToList<Country>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Countries, CountryView);
        }

        [Test]
        public void CanReturnView()
        {
            //Arrange
            var controller = new CountryController(mock.Object);
            //Act 
            var result = controller.Index();
            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void ShowRightView()
        {
            //Arrange
            CountryController target = new CountryController(mock.Object);
            //Act
            ViewResult result = (ViewResult)target.Index();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "");
        }


        [Test]
        public void ViewCountryList()
        {
            //Arrange
            CountryController target = new CountryController(mock.Object);
            //Act
            IEnumerable<Country> result = (IEnumerable<Country>)target.Index().Model;
            Country[] cArray = result.ToArray();
            //Assert
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Country));
            Assert.AreEqual(5, cArray.Length);
            Assert.IsTrue(cArray[0].CountryName == "Ukraine");
            Assert.IsTrue(cArray[1].CountryName == "Poland");
            Assert.IsTrue(cArray[2].CountryName == "Sweden");

        }

        [Test]
        [Category("View names")]
        public void CreateGetView_True()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Create() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreatePost_CanCreate_ValidCountry()
        {
            // Arrange - create the controller                 
            CountryController target = new CountryController(mock.Object);
            Country Country = new Country { CountryID = 1, CountryName = "Netherlands", Comment = "Test Comment" };

            // Act - call the action method 
            ViewResult result = (ViewResult)target.Create(Country);

            // Assert - check the result 
            mock.Verify(m => m.SaveCountry(Country), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Index", result.ViewName);

        }

        [Test]
        public void CreatePost_CannotCreate_InvalidCountry()
        {
            // Arrange - create the controller
            Country Country = new Country();
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(Country) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveCountry(It.IsAny<Country>()), Times.Never);
            Assert.IsInstanceOf(typeof(Country), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        [Category("View names")]
        public void EditView_True()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditGet_CanEdit_ValidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            Country Country = mock.Object.Countries.Where(m => m.CountryID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Country);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void EditPost_CanEdit_ValidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);
            Country Country = new Country { CountryID = 1, CountryName = "Netherlands", Comment = "Test Comment" };


            // Act - call the action method 
            var result = (ViewResult)target.Edit(Country);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Index", result.ViewName);
            mock.Verify(m => m.SaveCountry(Country), Times.Once);
        }


        [Test]
        public void EditPost_CannotEdit_InvalidCountry()
        {
            // Arrange - create the controller 
            Country Country = new Country { };
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(Country);

            // Assert - check the result 
            mock.Verify(m => m.SaveCountry(Country), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(1, target.ViewData.ModelState.Count);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            CountryController controller = new CountryController(mock.Object);
            mock.Setup(m => m.SaveCountry(It.IsAny<Country>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.Countries.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveCountry(It.IsAny<Country>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeleteGet_ValidCountryWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("CannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void DeleteGet_DefaultCountry_CannotDelete()
        {
            // Arrange 
            List<Country> countries = new List<Country>
            {
                //ID of default country is 1
                new Country { CountryID = 1, CountryName = "Ukraine", Holidays = new List<Holiday>(), Locations = new List<Location>()}
            };
            
            mock.Setup(m => m.Countries).Returns(countries);
            
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(1) as ViewResult;  

            // Assert - check the result 
            Assert.AreEqual("CannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void DeleteGet_ValidCountryWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(5) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Country), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }


        [Test]
        public void DeleteGet_InvalidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(15);
            Country Country = mock.Object.Countries.Where(m => m.CountryID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Country);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void DeletePost_CanDelete_ValidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);

            // Act - call the action method 
            var result = (ViewResult)target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteCountry(1), Times.Once);
            Assert.AreEqual("Index", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void DeletePost_CannotDelete_ValidCountry()
        {
            // Arrange - create the controller 
            CountryController target = new CountryController(mock.Object);
            mock.Setup(x => x.DeleteCountry(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteCountry(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }
    }
}
