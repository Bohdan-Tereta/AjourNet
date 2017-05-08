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
    public class GreetingControllerTest
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
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            ViewResult result = target.Index("BDMView");

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Index_Default_AllGreetings()
        {
            // Arrange - create the controller     
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            IEnumerable<Greeting> result = (IEnumerable<Greeting>)target.Index("BDMView").Model;
            List<Greeting> GreetingView = result.ToList<Greeting>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Greetings, GreetingView);
        }

        [Test]
        public void CanReturnView()
        {
            //Arrange
            var controller = new GreetingController(mock.Object);
            //Act 
            var result = controller.Index("BDMView");
            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void ShowRightView()
        {
            //Arrange
            GreetingController target = new GreetingController(mock.Object);
            //Act
            ViewResult result = (ViewResult)target.Index("BDMView");
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "");
        }


        [Test]
        public void ViewGreetingList()
        {
            //Arrange
            GreetingController target = new GreetingController(mock.Object);
            //Act
            IEnumerable<Greeting> result = (IEnumerable<Greeting>)target.Index("BDMView").Model;
            Greeting[] cArray = result.ToArray();
            //Assert
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Greeting));
            Assert.AreEqual(5, cArray.Length);
            Assert.IsTrue(cArray[0].GreetingHeader == "Greeting 1");
            Assert.IsTrue(cArray[1].GreetingHeader == "Greeting 2");
            Assert.IsTrue(cArray[2].GreetingHeader == "Greeting 3");

        }

        [Test]
        [Category("View names")]
        public void CreateGetView_True()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = target.Create() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreatePost_CanCreate_ValidGreeting()
        {
            // Arrange - create the controller                 
            GreetingController target = new GreetingController(mock.Object);
            Greeting Greeting = new Greeting { GreetingId = 6, GreetingHeader = "Greeting 1", GreetingBody = "Test Greeting" };

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.Create(Greeting);

            // Assert - check the result 
            mock.Verify(m => m.SaveGreeting(Greeting), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("BDMView", result.RouteValues["action"]);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidGreeting()
        {
            // Arrange - create the controller
            Greeting Greeting = new Greeting();
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(Greeting) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveGreeting(It.IsAny<Greeting>()), Times.Never);
            Assert.IsInstanceOf(typeof(Greeting), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        [Category("View names")]
        public void EditView_True()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditGet_CanEdit_ValidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            Greeting Greeting = mock.Object.Greetings.Where(m => m.GreetingId == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Greeting);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void EditPost_CanEdit_ValidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);
            Greeting Greeting = new Greeting { GreetingId = 6, GreetingHeader = "Greeting 1", GreetingBody = "Test Greeting" };


            // Act - call the action method 
            var result = (RedirectToRouteResult)target.Edit(Greeting);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("BDMView", result.RouteValues["action"]);
            mock.Verify(m => m.SaveGreeting(Greeting), Times.Once);
        }


        [Test]
        public void EditPost_CannotEdit_InvalidGreeting()
        {
            // Arrange - create the controller 
            Greeting Greeting = new Greeting { };
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(Greeting);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SaveGreeting(Greeting), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("", data);
            //Assert.IsInstanceOf(typeof(Greeting), result.ViewData.Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            GreetingController controller = new GreetingController(mock.Object);
            mock.Setup(m => m.SaveGreeting(It.IsAny<Greeting>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.Greetings.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveGreeting(It.IsAny<Greeting>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeleteGet_ValidGreeting_CanDelete()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(5) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Greeting), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }


        [Test]
        public void DeleteGet_InvalidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(15);
            Greeting Greeting = mock.Object.Greetings.Where(m => m.GreetingId == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(Greeting);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void DeletePost_CanDelete_ValidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteGreeting(1), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("BDMView", result.RouteValues["action"]);
            Assert.IsNull(result.RouteValues["tab"]);
            Assert.IsFalse(result.Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void DeletePost_CannotDelete_ValidGreeting()
        {
            // Arrange - create the controller 
            GreetingController target = new GreetingController(mock.Object);
            mock.Setup(x => x.DeleteGreeting(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteGreeting(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }
    }
}
