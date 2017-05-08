using System;
using NUnit.Framework;
using Moq;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using System.Data.SqlClient;
using AjourBT.Domain.Infrastructure;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using AjourBT.Domain.ViewModels;
using System.Web.Routing;
using System.Web;
using System.Security.Principal;
using System.IO;
using AjourBT.Domain.Concrete;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class PUControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        PUController controller;
        Mock<ControllerContext> controllerContext;
        string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";
        string currentUser = "User";

        string defaultAccComment = "ВКО №   від   , cума:   UAH." + System.Environment.NewLine + "ВКО №   від   , cума:   USD.";
        private string btCreationError = "Absence already planned on this period for this user. "
                              + "Please change OrderDates or if BT haven\'t OrderDates "
                              + "change \'From\' or \'To\'";

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();
            controller = new PUController(mock.Object, messengerMock.Object);
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
        }

            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);
            controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            controller.ControllerContext = controllerContext.Object; 

        }

        #region Unit
        [Test]
        [Category("View names")]
        public void UnitIndexView_True()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = target.UnitIndex();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void UnitIndex_Default_AllUnits()
        {
            // Arrange - create the controller     
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            IEnumerable<Unit> result = (IEnumerable<Unit>)target.UnitIndex().Model;
            List<Unit> unitView = result.ToList<Unit>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Units, unitView);
        }

        
        [Test]
        [Category("View names")]
        public void UnitCreateGetView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.UnitCreate() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void UnitCreatePost_CanCreate_ValidUnit()
        {  
            // Arrange - create the controller                 
            PUController target = new PUController(mock.Object, messengerMock.Object);
              Unit unit = new Unit { UnitID = 15, ShortTitle = "Umknown1", Title = "Unknown2" };
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.UnitCreate(unit);

            // Assert - check the result 
            mock.Verify(m => m.SaveUnit(unit), Times.Once);
        }
          

        [Test]
           public void UnitCreatePost_CannotCreate_InvalidUnit()
        {
            // Arrange - create the controller
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Units).Returns(new Unit[]{
                new Unit{UnitID = 7}
                });
            Unit unit = new Unit();
            PUController target = new PUController(mRepository.Object, messengerMock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.UnitCreate(unit);

            // Assert - check the result 
            mRepository.Verify(m => m.SaveUnit(It.IsAny<Unit>()), Times.Never);
            Assert.AreEqual(false, ((ViewResult)result).ViewData.ModelState.IsValid);
           // Assert.IsInstanceOf(typeof(List<Unit>), ((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }
        
        [Test]
        [Category("View names")]
        public void UnitEditView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.UnitEdit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void UnitEditGet_CanEdit_ValidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.UnitEdit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void UnitEditGet_CannotEdit_InvalidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.UnitEdit(15);
            Unit unit = mock.Object.Units.Where(m => m.UnitID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(unit);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void UnitEditPost_CanEdit_ValidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Unit unit = new Unit { UnitID = 1, ShortTitle = "Unknown", Title = "Unknown" };


            // Act - call the action method 
            var result = (ViewResult)target.UnitEdit(unit);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            mock.Verify(m => m.SaveUnit(unit), Times.Once);
        }


        //[Test]
        //public void UnitEditPost_CannotEdit_InvalidUnit()
        //{
        //    // Arrange - create the controller 
        //    Unit unit = new Unit { };
        //    PUController target = new PUController(mock.Object, messengerMock.Object);

        //    // Act - call the action method 
        //    target.ModelState.AddModelError("error", "error");
        //    var result = target.UnitEdit(unit);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    // Assert - check the result 
        //    mock.Verify(m => m.SaveUnit(unit), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual("", data);
        //}

        [Test]
        public void UnitEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(m => m.SaveUnit(It.IsAny<Unit>())).Throws(new DbUpdateConcurrencyException());

            //Act
            JsonResult result = (JsonResult)controller.UnitEdit(mock.Object.Units.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveUnit(It.IsAny<Unit>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void UnitDeleteGet_ValidUnitWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.UnitDelete(1) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("UnitCannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void UnitDeleteGet_ValidUnitWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.UnitDelete(3) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Unit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }


        [Test]
        public void UnitDeleteGet_InvalidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.UnitDelete(15);
            Unit unit = mock.Object.Units.Where(m => m.UnitID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(unit);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void UnitDeletePost_CanDelete_ValidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.UnitDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteUnit(1), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void UnitDeletePost_CannotDelete_ValidUnit()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(x => x.DeleteUnit(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.UnitDeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteUnit(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }
        #endregion

        #region Department
        [Test]
        public void CanReturnView()
        {
            //Arrange
            var controller = new PUController(mock.Object, messengerMock.Object);
            //Act 
            var result = controller.DepartmentIndex();
            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void ShowRightView()
        {
            //Arrange
            PUController target = new PUController(mock.Object, messengerMock.Object);
            //Act
            ViewResult result = (ViewResult)target.DepartmentIndex();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "");
        }


        [Test]
        public void ViewDepartmentList()
        {
            //Arrange
            PUController target = new PUController(mock.Object, messengerMock.Object);
            //Act
            IEnumerable<Department> result = (IEnumerable<Department>)target.DepartmentIndex().Model;
            Department[] depArray = result.ToArray();
            //Assert
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Department));
            Assert.IsTrue(depArray.Length == 7);
            Assert.IsTrue(depArray[0].DepartmentName == "SDDDA");
            Assert.IsTrue(depArray[3].DepartmentName == "RAAA2");
            Assert.IsTrue(depArray[6].DepartmentName == "RAAA5");

        }

        [Test]
        public void CreateDepartment_ReturnView()
        {
            //Arrange
            PUController target = new PUController(mock.Object, messengerMock.Object);
            //Act
            ViewResult result = (ViewResult)target.DepartmentCreate();
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void AddDepertment_ValidModel_Added()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            });
            PUController target = new PUController(mRepository.Object, messengerMock.Object);
            Department department = new Department(){DepartmentID = 15, DepartmentName = "123", Employees = new List<Employee>()};
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);
            //Act
            var result = target.DepartmentCreate(department) as ViewResult;
            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Once());
        }

        [Test]
        [Category("View names")]
        public void DepartmentCreateGetView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.DepartmentCreate() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

      
        public void AddDepartment_InvalidModel_NotAdded()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            });
            PUController target = new PUController(mRepository.Object, messengerMock.Object);
            Department department = new Department();
            target.ModelState.AddModelError("error", "error");
            //Act
            ViewResult result = (ViewResult)target.DepartmentCreate(department);
            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void EditDepartmentGetMethodWithValidID()
        {
            //Arrange

            PUController target = new PUController(mock.Object, messengerMock.Object);
            //   Act
            ViewResult result = (ViewResult)target.DepartmentEdit(1);
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditDepartmentGetMethodWithInvalidID()
        {
            //Arrange

            PUController target = new PUController(mock.Object, messengerMock.Object);
            //   Act
            var result = target.DepartmentEdit(100);
            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
        }

        [Test]
        public void EditDepartment_ValidModel_Save()
        {
            //Arrange  
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);
            Department dept =  new Department(){DepartmentID = 1, DepartmentName = "EPUA", Employees = new List<Employee>()};

            // Act - call the action method 
            var result = target.DepartmentEdit(1) as ViewResult;

            // Assert - check the result
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            }

      
        [Test]
        public void EditDepartment_InValidModel_NotSave()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.DepartmentEdit(16);
            Department dept = mock.Object.Departments.Where(m => m.DepartmentID == 150).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(dept);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }
       
    
        [Test]
        public void GetDeleteDepartment_CorrectIDDepartmentWithoutEmployees_ConfirmDeleting()
        {
            //Arrange

            PUController target = new PUController(mock.Object, messengerMock.Object);
            //   Act
            ViewResult result = (ViewResult)target.DepartmentDelete(3);
            // Assert
            Assert.IsInstanceOf(typeof(Department), result.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void GetDeleteDepartment_CorrectIDDepartmentWithEmployees_CannotDelete()
        {
            //Arrange

            PUController target = new PUController(mock.Object, messengerMock.Object);
            //   Act
            ViewResult result = (ViewResult)target.DepartmentDelete(1);
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("DepartmentCannotDelete", result.ViewName);
        }



        [Test]
        public void GetDeleteDepartment_IncorrectID_Error()
        {
            //Arrange

            PUController target = new PUController(mock.Object, messengerMock.Object);
            //   Act
            HttpNotFoundResult result = (HttpNotFoundResult)target.DepartmentDelete(100);
            // Assert
            Assert.IsTrue(result.StatusCode == 404);
        }
        [Test]
        public void PostDeleteDepartment_CanDelete_Redirect_to_PUView()
        {
            //Arrange
            PUController target = new PUController(mock.Object, messengerMock.Object);
            //Act
            ViewResult result = (ViewResult)target.DepartmentDeleteConfirmed(6);

            //Assert
            mock.Verify(m => m.DeleteDepartment(6), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void PostDeleteDepartment_CannotDelete()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(x => x.DeleteDepartment(It.IsAny<int>()))
              .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });

            // Act - call the action method
            RedirectToRouteResult result = (RedirectToRouteResult)target.DepartmentDeleteConfirmed(2);

            // Assert - check the result
            mock.Verify(m => m.DeleteDepartment(2), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);
        }

        [Test]
        public void EditDepartment_ValidModelComcurrency_JsonErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SaveDepartment(It.IsAny<Department>())).Throws(new DbUpdateConcurrencyException());
            PUController target = new PUController(mock.Object, messengerMock.Object);

            //Act
            JsonResult result = (JsonResult)target.DepartmentEdit(mock.Object.Departments.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }


        #endregion

        #region Position
        [Test]
        [Category("View names")]
        public void PositionIndexView_True()
        {
            // Arrange - create the controller

            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = target.PositionIndex();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void PositionIndex_Default_Allpositions()
        {
            // Arrange - create the controller     
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            IEnumerable<Position> result = (IEnumerable<Position>)target.PositionIndex().Model;
            List<Position> positionView = result.ToList<Position>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Positions, positionView);
        }

        [Test]
        [Category("View names")]
        public void PositionCreateGetView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionCreate() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void PositionCreatePost_CanCreate_ValidPosition()
        {
            // Arrange - create the controller                 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Position position = new Position { PositionID = 5, TitleEn = "Accountant", TitleUk = "Бухгалтер", Employees = new List<Employee>() };
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.PositionCreate(position);

            // Assert - check the result 
            mock.Verify(m => m.SavePosition(position), Times.Once);
        }

        [Test]
        public void PositionCreatePost_CanNotCreate_TitleEnExists()
        {
            // Arrange - create the controller                 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Position position = new Position { PositionID = 5, TitleEn = "Director", TitleUk = "Бухгалтер", Employees = new List<Employee>() };
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.PositionCreate(position);

            // Assert - check the result 
            //mock.Verify(m => m.SavePosition(position), Times.Once);
            Assert.AreEqual("Position with same TitleEn already exists", result.ViewData.ModelState["TitleEn"].Errors[0].ErrorMessage);
        }

        [Test]
        public void PositionCreatePost_CanNotCreate_TitleUkExists()
        {
            // Arrange - create the controller                 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Position position = new Position { PositionID = 5, TitleEn = "aaa", TitleUk = "Лайн-менеджер", Employees = new List<Employee>() };
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.PositionCreate(position);

            // Assert - check the result 
            //mock.Verify(m => m.SavePosition(position), Times.Once);
            Assert.AreEqual("Position with same TitleUk already exists", result.ViewData.ModelState["TitleUk"].Errors[0].ErrorMessage);
        }

        [Test]
        public void PositionCreatePost_CannotCreate_InvalidPosition()
        {

            // Arrange - create the controller
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Positions).Returns(new Position[]{
                new Position{PositionID = 4}
                });
            Position position = new Position();
            PUController target = new PUController(mRepository.Object, messengerMock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.PositionCreate(position);

            // Assert - check the result 
            mRepository.Verify(m => m.SavePosition(It.IsAny<Position>()), Times.Never);
            Assert.AreEqual(false, ((ViewResult)result).ViewData.ModelState.IsValid);
           // Assert.IsInstanceOf(typeof(List<Position>), ((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        [Category("View names")]
        public void PositionEditView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionEdit(2);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }

        [Test]
        public void PositionEditGet_CanEdit_ValidPosition()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionEdit(2);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void PositionEditGet_CannotEdit_InvalidPosition()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionEdit(15);
            Position position = mock.Object.Positions.Where(m => m.PositionID == 150).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(position);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void PositionEditPost_CanNotEdit_TitleUkExists()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Position position = new Position { PositionID = 6, TitleEn = "cccSoftware developer", TitleUk = "Розробник програмного забезпечення" };

            // Act - call the action method 
            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result
            Assert.AreEqual("Position with same TitleUk already exists", result.ViewData.ModelState["TitleUk"].Errors[0].ErrorMessage); 

        }

        [Test]
        public void PositionEditPost_CanNotEdit_TitleEnExists()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Position position = new Position { PositionID = 7, TitleEn = "Software developer", TitleUk = "adaaРозробник програмного забезпечення" };

            // Act - call the action method 
            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result
            Assert.AreEqual("Position with same TitleEn already exists", result.ViewData.ModelState["TitleEn"].Errors[0].ErrorMessage);

        }

        [Test]
        public void PositionEditPost_CanEdit_TitleUkExistsInSelf()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);        
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);

            Position position = new Position { PositionID = 2, TitleEn = "cccSoftware developer", TitleUk = "Розробник програмного забезпечення" };

            // Act - call the action method 
            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result
            Assert.AreEqual("PositionIndex", result.ViewName);
            Assert.AreEqual(4, ((List<Position>)result.Model).Count);

        }

        [Test]
        public void PositionEditPost_CanEdit_TitleEnExistsInSelf()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);
            Position position = new Position { PositionID = 2, TitleEn = "Software developer", TitleUk = "adaaРозробник програмного забезпечення" };

            // Act - call the action method 
            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result
            Assert.AreEqual("PositionIndex", result.ViewName);
            Assert.AreEqual(4, ((List<Position>)result.Model).Count);

        }

        [Test]
        public void PositionEditPost_CanEdit_NoTitleExistsInDatabase()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var routes = new RouteCollection();
            target.ControllerContext = controllerContext.Object;
            target.Url = new UrlHelper(new RequestContext(controllerContext.Object.HttpContext, new RouteData()), routes);
            Position position = new Position { PositionID = 2, TitleEn = "aaSoftware developer", TitleUk = "adaaРозробник програмного забезпечення" };

            // Act - call the action method 
            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result
            Assert.AreEqual("PositionIndex", result.ViewName);
            Assert.AreEqual(4, ((List<Position>)result.Model).Count);

        }

        [Test]
        public void PositionEditPost_CannotEdit_InvalidPosition()
        {
            // Arrange - create the controller 
            Position position = new Position();
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");

            var result = (ViewResult)target.PositionEdit(position);

            // Assert - check the result 
            mock.Verify(m => m.SavePosition(position), Times.Never);
            //Assert.AreEqual(false, ((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual("error", result.ViewData.ModelState["error"].Errors[0].ErrorMessage); 
            //Assert.IsInstanceOf(typeof(Position), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void PositionEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                + "was modified by another user after you got the original value. The "
                + "edit operation was canceled.";
            Position position = mock.Object.Positions.Where(p => p.PositionID == 1).FirstOrDefault();
            mock.Setup(m => m.SavePosition(position)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.PositionEdit(position);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePosition(position), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void PositionDeleteGet_ValidPositionWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionDelete(4);

            // Assert - check the result 
            Assert.AreEqual("PositionCannotDelete", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void PositionDeleteGet_ValidPositionWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionDelete(1);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(Position), ((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
        }


        [Test]
        public void PositionDeleteGet_InvalidPosition()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionDelete(150);
            Position position = mock.Object.Positions.Where(m => m.PositionID == 150).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(position);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void PositionDeletePost_CanDelete_ValidPosition()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.PositionDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeletePosition(1), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void PositionDeletePost_CannotDelete_ValidPosition()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(x => x.DeletePosition(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            var result = target.PositionDeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeletePosition(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", ((RedirectToRouteResult)result).RouteValues["action"]);


        }


        #endregion

        #region Employee

        #region GetEmployee

        [Test]
        public void GetEmployee_Null_NullSelectedDepartment()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee(null);
            string selectedDepartment = null;
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployee_StringEmpty_StringEmptySelectedDepartment()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee("");
            string selectedDepartment = "";
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployee_SDDDA_SDDDASelectedDepartment()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee("SDDDA");
            string selectedDepartment = "SDDDA";
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        #endregion

        #region GetEmployeeData
        [Test]
        public void GetEmployeeData_Null_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_SDDDA_SDDDAEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "SDDDA";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);
            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(5, result.ToArray<EmployeeViewModel>().Length);
            Assert.IsTrue(result.ToArray<EmployeeViewModel>().Length == 5);
            Assert.AreEqual(5, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].LastName, "Kowwood");
            Assert.AreEqual("Chuck", result.ToArray<EmployeeViewModel>()[1].FirstName);
            Assert.AreEqual("Pyorge", result.ToArray<EmployeeViewModel>()[2].LastName);

            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployeeData_StringEmpty_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_NonExistingDepartment_NoResult()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "assdsa";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.IsTrue(result.ToArray<EmployeeViewModel>().Length == 0);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_SearchString_Null_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;
            string searchString = null;

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment, searchString).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_SearchString_Empty_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;
            string searchString = "";

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment, searchString).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_SearchString_NotEmpty_SomeEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;
            string searchString = "Oleksiy";

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment, searchString).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert 
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
        }

        #endregion 

        //TODO: duplicated in Employee, PU, VU, ADM conrollers
        #region DropDownListWithSelectedDepartment

        [Test]
        public void DropDownListWithSelectedDepartment_Default_ListOfAllDepartments()
        {
            //Arrange

            PUController controller = new PUController(mock.Object, messengerMock.Object);
            //Act
            string selectedDepartment = "";
            var result = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.IsTrue(result.ToArray().Length == 7);
            Assert.AreEqual("RAAA1", result.ToArray()[0].Text);
            Assert.AreEqual("RAAA4", result.ToArray()[3].Text);
            Assert.AreEqual("TAAAA", result.ToArray()[6].Text);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_SelectedDepartmentNull_ViewCreateSelectedDepartmentNull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_StringEmpty_ViewCreateSelectedDepartmentStringEmpty()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_RAAA1_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_True_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            SelectListItem selectListItem3 = new SelectListItem { Text = "RAAA1", Value = "3", Selected = true };
            SelectListItem selectListItem4 = new SelectListItem { Text = "RAAA2", Value = "4", Selected = false };
            SelectListItem selectListItem5 = new SelectListItem { Text = "RAAA3", Value = "5", Selected = false };
            SelectListItem selectListItem6 = new SelectListItem { Text = "RAAA4", Value = "6", Selected = false };
            SelectListItem selectListItem7 = new SelectListItem { Text = "RAAA5", Value = "7", Selected = false };
            SelectListItem selectListItem1 = new SelectListItem { Text = "SDDDA", Value = "1", Selected = false };
            SelectListItem selectListItem2 = new SelectListItem { Text = "TAAAA", Value = "2", Selected = false };
            selectListItems.Add(selectListItem3);
            selectListItems.Add(selectListItem4);
            selectListItems.Add(selectListItem5);
            selectListItems.Add(selectListItem6);
            selectListItems.Add(selectListItem7);
            selectListItems.Add(selectListItem1);
            selectListItems.Add(selectListItem2);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectListItems.Count());
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectListItems[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectListItems[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_SelectedFalse_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            SelectListItem selectListItem3 = new SelectListItem { Text = "RAAA1", Value = "3", Selected = true };
            SelectListItem selectListItem4 = new SelectListItem { Text = "RAAA2", Value = "4", Selected = false };
            SelectListItem selectListItem5 = new SelectListItem { Text = "RAAA3", Value = "5", Selected = false };
            SelectListItem selectListItem6 = new SelectListItem { Text = "RAAA4", Value = "6", Selected = false };
            SelectListItem selectListItem7 = new SelectListItem { Text = "RAAA5", Value = "7", Selected = false };
            SelectListItem selectListItem1 = new SelectListItem { Text = "SDDDA", Value = "1", Selected = false };
            SelectListItem selectListItem2 = new SelectListItem { Text = "TAAAA", Value = "2", Selected = false };
            selectListItems.Add(selectListItem3);
            selectListItems.Add(selectListItem4);
            selectListItems.Add(selectListItem5);
            selectListItems.Add(selectListItem6);
            selectListItems.Add(selectListItem7);
            selectListItems.Add(selectListItem1);
            selectListItems.Add(selectListItem2);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectListItems.Count());
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectListItems[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectListItems[6].Value);
        }

        #endregion

        #region EmployeeCreateGet
        [Test]
        public void GetEmployeeCreate_SelectedDepartmentNull_ViewCreateSelectedDepartmentNull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void GetEmployeeCreate_StringEmpty_ViewCreateSelectedDepartmentStringEmpty()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList selectList = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectList.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectList.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectList.ToArray()[6].Value);
        }

        [Test]
        public void GetEmployeeCreate_RAAA1_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.EmployeeCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }




        #endregion

        #region EmployeeCreatePost
        [Test]
        public void PostEmployeeCreate_NotValidModelSelectedDepartmentNull_ViewCreate()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            controller.ModelState.AddModelError("DepartmentID", "Field DepartmentID must be not null");
            Employee emp = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            EmployeeViewModel employee = new EmployeeViewModel(emp);
            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.EmployeeCreate(emp);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never());
            Assert.IsTrue(view != null);
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void PostEmployeeCreate_NotValidModelSelectedDepartmentRAAA1_ViewCreate()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            controller.ModelState.AddModelError("DepartmentID", "Field DepartmentID must be not null");
            Employee emp = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            EmployeeViewModel employee = new EmployeeViewModel(emp);

            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.EmployeeCreate(emp, null, "RAAA1");
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never());
            Assert.IsTrue(view != null);
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);

        }

        [Test]
        public void PostEmployeeCreate_ValidModelSelectedDepartmentNull_ViewAllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false, EID = "andm" };
            string selectedDepartment = null;

            // Act
            var view = controller.EmployeeCreate(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        [Test]
        public void PostEmployeeCreate_ValidModelSelectedDepartmentEmptyString_ViewAllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false, EID = "andm" };
            string selectedDepartment = "";
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            var view = controller.EmployeeCreate(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        [Test]
        public void PostEmployeeCreate_ValidModelSelectedDepartmentRAAA1_ViewRAAA1Employees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false, EID = "andm" };
            string selectedDepartment = "RAAA1";

            // Act
            var view = controller.EmployeeCreate(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("RAAA1", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        [Test]
        public void PostEmployeeCreate_EmployeeAlreadyExists_JsonError()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false, EID = "andl" };
            string selectedDepartment = "RAAA1";

            // Act
            JsonResult result = controller.EmployeeCreate(employee, null, selectedDepartment) as JsonResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Never);
            Assert.AreEqual("{ error = Employee with EID andl already exists }", result.Data.ToString());
        }

        #endregion

        #region EmployeeEditGet

        [Test]
        public void GetEmployeeEdit_ExistingEmployeeSelectedDepartmentNull_ViewEditSelectedDepartmentNull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.EmployeeEdit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEmployeeEdit_ExistingUserSelectedDepartmentNull_ViewEditSelectedDepartmentNull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.EmployeeEdit(25, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 25);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEmployeeEdit_ExistingEmployeeSelectedDepartmentStringEmpty_ViewEditSelectedDepartmentStringEmpty()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "";
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.EmployeeEdit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEmployeeEdit_ExistingEmployeeSelectedDepartmentTAAAA_ViewEditSelectedDepartmentTAAAA()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "TAAAA";
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.EmployeeEdit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEmployeeEdit_NotExistingEmployee_Error()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            HttpNotFoundResult result = controller.EmployeeEdit(1000) as HttpNotFoundResult;

            // Assert
            Assert.IsTrue(result.StatusCode == 404);
        }

        #endregion

        #region EmployeeEditPost
        [Test]
        public void PostEmployeeEdit_ValidModelSelectedDepartmentNull_ViewAllEmployee()
        {
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = null;
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.EmployeeEdit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());
        }

        [Test]
        public void PostEmployeeEdit_ValidModelUserSelectedDepartmentNull_ViewAllUsers()
        {
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = mock.Object.Users.Where(e => e.EmployeeID == 25).FirstOrDefault();
            string selectedDepartment = null;
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.EmployeeEdit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());
        }

        [Test]
        public void PostEmployeeEdit_ValidModelSelectedDepartmentStringEmpty_ViewAllEmployee()
        {
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.EmployeeEdit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());
        }

        [Test]
        public void PostEmployeeEdit_ValidModelSelectedDepartmentRAAA1_ViewAllEmployee()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "RAAA1";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.EmployeeEdit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());

        }

        [Test]
        public void PostEmployeeEdit_NotValidModelSelectedDepartmentNull_ViewEdit()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = null;
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.EmployeeEdit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEmployeeEdit_NotValidModelSelectedDepartmentStringEmpty_ViewEdit()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.EmployeeEdit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEmployeeEdit_NotValidModelSelectedDepartmentRAAA5_ViewEdit()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "RAAA5";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.EmployeeEdit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEmployeeEdit_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SaveEmployee(It.IsAny<Employee>())).Throws(new DbUpdateConcurrencyException());
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.EmployeeEdit(mock.Object.Employees.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveEmployee(It.IsAny<Employee>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region EmployeeDelete

        [Test]
        public void GetEmployeeDelete_EmpWithoutAssociatedDataDepartmentnull_ViewConfrimDeleteSelectedDepartmentnull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;

            // Act
            ViewResult result = controller.EmployeeDelete(15, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(Employee), result.Model);
            Assert.IsTrue(employee.EmployeeID == 15);
        }

        [Test]
        public void GetEmployeeDelete_User__WithoutAssociatedDataDepartmentnull_ViewConfrimDeleteSelectedDepartmentnull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;

            // Act
            ViewResult result = controller.EmployeeDelete(25, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(Employee), result.Model);
            Assert.IsTrue(employee.EmployeeID == 25);
        }

        [Test]
        public void GetEmployeeDelete_EmpWithBTDepartmentSDDDA_ViewCannotDeleteDelete()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "SDDDA";

            // Act
            ViewResult result = controller.EmployeeDelete(1, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "EmployeeCannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);

        }

        [Test]
        public void GetEmployeeDelete_WithVisaRegDateSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA3";

            // Act
            ViewResult result = controller.EmployeeDelete(6, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "EmployeeCannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetEmployeeDelete_WithVisaRegDateAndVisaAndBTSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "SDDDA";

            // Act
            ViewResult result = controller.EmployeeDelete(3, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "EmployeeCannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetEmployeeDelete_WithVisaAndBTSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA3";

            // Act
            ViewResult result = controller.EmployeeDelete(7, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "EmployeeCannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetEmployeeDelete_WithPermitSelectedDepartmentRAAA1_ViewCannotDeleteDelete()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA1";

            // Act
            ViewResult result = controller.EmployeeDelete(5, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "EmployeeCannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetEmployeeDelete_IncorrectId_404NotFound()
        {
            // Arrange 
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            // Act 
            HttpNotFoundResult result = (HttpNotFoundResult)controller.EmployeeDelete(1000);
            // Assert 
            Assert.IsTrue(result.StatusCode == 404);
        }
        #endregion

        #region EmployeeDeleteConfirmed
        [Test]
        public void EmployeeDeletePost_IdSelectedDepartmentNull_RedirectToPUView()
        {
            // Arrange
            //List<Employee> empList = mock.Object.Employees.ToList();
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;

            // Act
            var result = controller.EmployeeDeleteConfirmed(3, selectedDepartment);


            // Assert
            mock.Verify(m => m.DeleteEmployee(3), Times.Once);
            Assert.AreEqual(((ViewResult)result).GetType(), typeof(ViewResult));
            Assert.AreEqual(((ViewResult)result).ViewName, "OneRowPU");
        }

        [Test]
        public void EmployeeDeletePost_UserIdSelectedDepartmentNull_RedirectToPUView()
        {
            // Arrange
            //List<Employee> empList = mock.Object.Employees.ToList();
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;

            // Act
            var result = controller.EmployeeDeleteConfirmed(25, selectedDepartment);


            // Assert
            mock.Verify(m => m.DeleteEmployee(25), Times.Once);
            Assert.AreEqual(((ViewResult)result).GetType(), typeof(ViewResult));
            Assert.AreEqual(((ViewResult)result).ViewName, "OneRowPU");
        }

        [Test]
        public void EmployeeDeletePost_CannotDelete_DataBaseDeleteError()
        {
            // Arrange - create the controller
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(x => x.DeleteEmployee(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method
            RedirectToRouteResult result = (RedirectToRouteResult)controller.EmployeeDeleteConfirmed(2);

            // Assert - check the result
            mock.Verify(m => m.DeleteEmployee(2), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);

        }
        #endregion

        #region SearchEmployeeData

        [Test]
        public void SearchEmployeeDataEmployee_NotEmptyDepartmentEmptySearchStringEmpty_AllUsers()
        {
            //Arrange
            List<Employee> empList = mock.Object.Users.ToList();
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            //Act
            List<EmployeeViewModel> result = controller.SearchEmployeeData(empList, "", "");
            //Assert
            Assert.AreEqual(25, result.Count());
            Assert.AreEqual(8, result.First().EmployeeID);
            Assert.AreEqual(1, result.Last().EmployeeID);
        }

        #endregion

        #region ResetPassword
        [Test]
        public void ResetPasswordGET_AllParametersNotNull_View()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = "andl";
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void ResetPasswordGET_RolesIsNull_View()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = "andl";
            string[] Roles = null;
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void ResetPasswordGET_EIDIsNull_View()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = null;
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }


        [Test]
        public void ResetPasswordConfirmPost_AllParametersNotNull_FormattedString()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = "andl";
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee("andl", new string[] { "ADM", "PU" }), Times.Exactly(1));
            mock.Verify(m => m.SaveRolesForEmployee(EID, null), Times.Exactly(1));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }

        [Test]
        public void ResetPasswordConfirmPost_EIDIsNull_FormattedString()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = null;
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee(EID, Roles), Times.Exactly(1));
            mock.Verify(m => m.SaveRolesForEmployee(EID, null), Times.Exactly(1));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }

        [Test]
        public void ResetPasswordConfirmPost_RolesIsNull_FormattedString()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string EID = "andl";
            string[] Roles = null;
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee(EID, Roles), Times.Exactly(2));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }

        #endregion

        #region DepartmentsDropDownList
        [Test]
        public void DepartmentsDropDownList_Default_ListOfAllDepartments()
        {
            //Arrange

            PUController controller = new PUController(mock.Object, messengerMock.Object);
            //Act
            var result = controller.DepartmentsDropDownList();

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.IsTrue(result.ToArray().Length == 7);
            Assert.AreEqual("RAAA1", result.ToArray()[0].Text);
            Assert.AreEqual("RAAA4", result.ToArray()[3].Text);
            Assert.AreEqual("TAAAA", result.ToArray()[6].Text);
        }
        #endregion


        #endregion        

        #region Location

        #region LocationIndex
        [Test]
        [Category("View names")]
        public void LocationIndexView_True()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = target.LocationIndex();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void LocationIndex_Default_AllLocations()
        {
            // Arrange - create the controller     
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            IEnumerable<Location> result = (IEnumerable<Location>)target.LocationIndex().Model;
            List<Location> locationView = result.ToList<Location>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Locations, locationView);
        }


        [Test]
        public void CountriesDropDownList_Default_ListOfAllCountries()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            //Act
            var result = controller.CountriesDropDownList();
            var expected = result.ToArray();

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.AreEqual(5, expected.Length);
            Assert.AreEqual("Belarus", expected[0].Text);
            Assert.AreEqual("Poland", expected[1].Text);
            Assert.AreEqual("Sweden", expected[2].Text);
            Assert.AreEqual("Ukraine", expected[3].Text);
            Assert.AreEqual("Zimbabve", expected[4].Text);
        }
        #endregion

        #region LocationCreate

        [Test]
        [Category("View names")]
        public void LocationCreateGet_View_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            IEnumerable<Country> countriesList = from c in mock.Object.Countries
                                                 orderby c.CountryName
                                                 select c;

            // Act - call the action method 
            var result = target.LocationCreate() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual(countriesList.ToList(), ((ViewResult)result).ViewBag.CountryList.Items);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void LocationCreatePost_CanCreate_ValidLocationNullResponsibleID()
        {
            // Arrange - create the controller                 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St." };

            // Act - call the action method 
            ActionResult result = (ActionResult)target.LocationCreate(location);

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void LocationCreatePost_CannotCreate_InvalidLocation()
        {
            // Arrange - create the controller
            Location location = new Location();
            PUController target = new PUController(mock.Object, messengerMock.Object);
            IEnumerable<Country> countriesList = from c in mock.Object.Countries
                                                 orderby c.CountryName
                                                 select c;

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.LocationCreate(location) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.AreEqual(countriesList.ToList(), ((ViewResult)result).ViewBag.CountryList.Items);
            Assert.IsInstanceOf(typeof(Location), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }


        [Test]
        public void LocationCreatePost_CannotCreate_InvalidResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "xyza" };
            PUController target = new PUController(mock.Object, messengerMock.Object);
            target.ViewData.ModelState.Clear();

            // Act - call the action method 
            var result = target.LocationCreate(location);           

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.AreEqual(2, target.ViewData.ModelState.Count);
            Assert.IsInstanceOf(typeof(ActionResult), result);
        }


        [Test]
        public void LocationCreatePost_CanCreate_ValidLocationWithResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "ABCDEF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "daol" };
            PUController target = new PUController(mock.Object, messengerMock.Object);
            target.ViewData.ModelState.Clear();

            // Act - call the action method 
            ViewResult result = target.LocationCreate(location) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Once);
            Assert.IsInstanceOf(typeof(ActionResult), result);
        }

        [Test]
        public void LocationCreatePost_CanCreate_ValidLocationWithEmptyResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "" };
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = target.LocationCreate(location) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Never);
            Assert.IsInstanceOf(typeof(ActionResult), result);
        }

        #endregion

        #region IsExistingEID
        [Test]
        public void IsExistingEID_true_ExistingIds()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "tedk,daol,User";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingEID_ExistingIdsWithEmptySpacesInTheEndAndBegin_True()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "    tedk  ,     daol    ";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingEID_true_ExistingIds2()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "tedk, daol";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingEID_true_ExistingIds3()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "tedk;   daol";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingEID_false_NotExistingIds()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "tedk;;*&?$#!/-+=[]{}:''<>.,   dni";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsFalse(result);
        }

        [Test]
        public void IsExistingEID_false_NotExistingIds2()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "tepydani";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsFalse(result);
        }

        [Test]
        public void IsExistingEID_EmptyString_True()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = "";

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingEID_Null_True()
        {
            // Arrange - create the controller
            PUController target = new PUController(mock.Object, messengerMock.Object);
            string ids = null;

            // Act - call the action method          
            bool result = target.IsExistingEID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        #endregion

        #region LocationEditGet
        [Test]
        [Category("View names")]
        public void LocationEditView_True()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.LocationEdit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void LocationEditGet_CanEdit_ValidLocation()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.LocationEdit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void LocationEditGet_CannotEdit_InvalidLocation()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.LocationEdit(15);
            Location location = mock.Object.Locations.Where(m => m.LocationID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(location);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion

        #region LocationEditPost

        [Test]
        public void LocationEditPost_CanEdit_ValidLocationNullResponsibleID()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St." };


            // Act - call the action method 
            var result = (ViewResult)target.LocationEdit(location);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            mock.Verify(m => m.SaveLocation(location), Times.Once);
        }


        [Test]
        public void LocationEditPost_CanEdit_ValidLocationWithResponsibleID()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "daol" };


            // Act - call the action method 
            var result = (ViewResult)target.LocationEdit(location);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            mock.Verify(m => m.SaveLocation(location), Times.Once);
        }



        [Test]
        public void LocationEditPost_CannotEdit_InvalidLocation()
        {
            // Arrange - create the controller 
            Location location = new Location { };
            PUController target = new PUController(mock.Object, messengerMock.Object);
            target.ViewData.ModelState.Clear();

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.LocationEdit(location);            

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(1, target.ViewData.ModelState.Count);
        }

        [Test]
        public void LocationEditPost_CannotCreate_InvalidResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "xxxx" };
            PUController target = new PUController(mock.Object, messengerMock.Object);
            target.ViewData.ModelState.Clear();

            // Act - call the action method 
            var result = target.LocationEdit(location);
            
            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(1, target.ViewData.ModelState.Count);
        }

        [Test]
        public void LocationEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(m => m.SaveLocation(It.IsAny<Location>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.LocationEdit(mock.Object.Locations.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveLocation(It.IsAny<Location>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region LocationDeleteGet
        [Test]
        public void LocationDeleteGet_ValidLocationWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.LocationDelete(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("LocationCannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void LocationDeleteGet_ValidLocationWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.LocationDelete(3) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Location), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void LocationDeleteGet_InvalidLocation()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.LocationDelete(15);
            Location location = mock.Object.Locations.Where(m => m.LocationID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(location);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion

        #region LocationDeletePost
        [Test]
        public void LocationDeletePost_CanDelete_ValidLocation()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            ViewResult result = (ViewResult)target.LocationDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteLocation(1), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void LocationDeletePost_CannotDelete_ValidLocation()
        {
            // Arrange - create the controller 
            PUController target = new PUController(mock.Object, messengerMock.Object);
            mock.Setup(x => x.DeleteLocation(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.LocationDeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteLocation(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }

        #endregion

        #endregion

        #region Users

        #region UserCreate
        
        [Test]
        public void GetUserCreate_SelectedDepartmentNull_ViewCreateSelectedDeptNull()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = null;
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.UserCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void GetUseCreate_StringEmpty_ViewCreateSelectedDepartmentStringEmpty()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.UserCreate(selectedDepartment);
            SelectList selectList = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectList.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectList.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectList.ToArray()[6].Value);
        }

        [Test]
        public void GetUserCreate_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.UserCreate(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        #endregion


        #region GetUsersOnly

        [Test]
        public void GetUsersOnly_NullSearchStringAndNullSelectedDepartment_NullSelectedDepartment()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var result = controller.GetUsersOnly(null);
            string selectedDepartment = null;
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(null, ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)result).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetUsersOnly_EmptySearchStringAndSelectedDepartment_EmptyList()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string searchString = "";
            string selectedDepartment = "";

            //Act
            var result = controller.GetUsersOnly(searchString, selectedDepartment);
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(null, ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)result).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
 
        }

        [Test]
        public void GetUsersOnly_SDDDASelectedDepartment_NotEmptyList()
        {
            //Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "SDDDA";

            //Act
            var result = controller.GetUsersOnly(selectedDepartment);
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(null, ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)result).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        #endregion

        #region GetUsersOnlyData

        [Test]
        public void GetUsersOnlyData_Null_AllEmployees()
        {
            // Arrange

            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment).Model;
            var view = controller.GetUsersOnlyData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual("User", result.ToArray<EmployeeViewModel>()[0].FirstName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetUsersOnlyData_RAAA3_RAAA3Employees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment,"").Model;
            var view = controller.GetUsersOnlyData(selectedDepartment,"");
            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual("User",result.ToArray<EmployeeViewModel>()[0].LastName);

            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetUsersOnlyData_StringEmpty_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment).Model;
            var view = controller.GetUsersOnlyData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual("User", result.ToArray<EmployeeViewModel>()[0].FirstName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetUsersOnlyData_NonExistingDepartment_NoResult()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "assdsa";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment).Model;
            var view = controller.GetUsersOnlyData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.IsTrue(result.ToArray<EmployeeViewModel>().Length == 0);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        //[Test]
        //public void GetUsersOnlyData_SearchString_Null_AllEmployees()
        //{
        //    // Arrange
        //    PUController controller = new PUController(mock.Object, messengerMock.Object);
        //    MvcApplication.JSDatePattern = "dd.mm.yy";
        //    string selectedDepartment = null;
        //    string searchString = null;

        //    // Act
        //    IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment, searchString).Model;
        //    var view = controller.GetUsersOnlyData(selectedDepartment);


        //    // Assert
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
        //    Assert.AreEqual(25, result.ToArray<EmployeeViewModel>().Length);
        //    Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
        //    Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
        //    Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
        //    Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
        //    Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
        //    Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
        //    Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
        //    Assert.IsTrue(view.ViewName == "");
        //    Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        //    Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        //}

        [Test]
        public void GetUsersOnlyData_SearchString_Empty_AllEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;
            string searchString = "";

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment, searchString).Model;
            var view = controller.GetUsersOnlyData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual("User", result.ToArray<EmployeeViewModel>()[0].FirstName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetUsersOnlyData_SearchString_NotEmpty_SomeEmployees()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;
            string searchString = "User";

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetUsersOnlyData(selectedDepartment, searchString).Model;
            var view = controller.GetUsersOnlyData(selectedDepartment);


            // Assert 
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(1, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual("User", result.ToArray<EmployeeViewModel>()[0].FirstName);
        }

        #endregion

        #endregion

        #region Finished BTs tab

        #region GetBusinessTripByDatesPU

        [Test]
        public void GetBusinessTripByDatesPU_Default_DefaultYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesPU();

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesPU_0Year_ListofYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesPU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripByDatesPU_Current_ListOfYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            var view = controller.GetBusinessTripByDatesPU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesPU_2012_ListOfYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = 2012;
            var view = controller.GetBusinessTripByDatesPU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesPU_0_ListOfYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesPU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesPU_NotExistingYear_ListOfYear()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act
            int selectedYear = DateTime.Now.AddYears(10).Year;
            var view = controller.GetBusinessTripByDatesPU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        #endregion

        #endregion


        #region GetMailAliasEMails

        [Test]
        public void GetMailAliasEMails_SDDDA_SDDDAEmployeesMails()
        {
            //Arrange
            mock.Object.Employees.Where(e => e.EID == "andl").FirstOrDefault().EMail = "abc";
            mock.Object.Employees.Where(e => e.EID == "ivte").FirstOrDefault().EMail = "";
            mock.Object.Employees.Where(e => e.EID == "tedk").FirstOrDefault().EMail = null;
            //Act
            var result = (ViewResult)controller.GetMailAliasEMails("SDDDA");
            //Assert
            // Assert.AreEqual("<textarea rows=\"10\" cols=\"45\" name=\"text\">tedk@elegant.com, xomi@elegant.com, chap@elegant.com, ivte@elegant.com</textarea>", result.ToString());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(result.ViewName, "GetMailAliasEMails");

        }

        #endregion

        #region GetSecondMailAliasEMails

        [Test]
        public void GetSecondMailAliasEMails_SDDDA_SDDDAEmployeesSecondMailsAndMailsIfSecondMailsNotExist()
        {
            //Arrange
            mock.Object.Employees.Where(e => e.EID == "andl").FirstOrDefault().EMail = "abc";
            mock.Object.Employees.Where(e => e.EID == "ivte").FirstOrDefault().EMail = "";
            //mock.Object.Employees.Where(e => e.EID == "tedk").FirstOrDefault().EMail = null;
            //Act
            var result = (ViewResult)controller.GetSecondMailAliasEMails("SDDDA");
            //Assert
            // Assert.AreEqual("<textarea rows=\"10\" cols=\"45\" name=\"text\">abc, xomi@elegant.com, chap@elegant.com, ivte@elegant.com</textarea>", result.ToString());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(result.ViewName, "GetMailAliasEMails");
        }

        #endregion

        #region GetMailToLinkWithBcc

        [Test]
        public void GetMailAliasToLinkWithBcc_SDDDA_MailtoLink()
        {
            //Arrange
         
            //Act
            var result = (ViewResult)controller.GetMailToLinkWithBcc("SDDDA");
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(result.ViewName, "GetMailAliasEMails");
            Assert.AreEqual(currentUser, ((ViewResult)result).ViewBag.User);

        }
        #region GetBusinessTripDataByDatesPU

        [Test]
        public void GetBusinessTripDataByDatesPU_0_ListOfYear()
        {
            // Arrange
            int selectedYear = 0;
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesPU_2013Year_BTs2013()
        {
            // Arrange
            int selectedYear = 2013;
            BusinessTrip btStartDateToBeChanged = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();
            btStartDateToBeChanged.StartDate = new DateTime(2018, 01, 01);
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(16, bts.Count());
            Assert.AreEqual(bts[0].BTof.EID, "daol");
            Assert.AreEqual(bts[6].BTof.EID, "tadk");
            Assert.AreEqual(bts[7].BTof.EID, "tadk");
            Assert.AreEqual(bts[8].BTof.EID, "tadk");
            Assert.AreEqual("xtwe", bts[10].BTof.EID);

            Assert.AreEqual(9, bts[0].BusinessTripID);
            Assert.AreEqual(35, bts[9].BusinessTripID);
            Assert.AreEqual(32, bts[10].BusinessTripID);

            Assert.AreEqual(2013, ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesPU_2012Year_BTs2012()
        {
            // Arrange
            int selectedYear = 2012;
            var status = BTStatus.Confirmed | BTStatus.Reported;
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual(employees[0].BTof.EID, "xomi");
            Assert.AreEqual(employees[1].BTof.EID, "tedk");
            Assert.AreEqual(employees[0].Status, status);
            Assert.AreEqual(employees[1].Status, status);
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesPU_2014Year_BTs2014()
        {
            // Arrange
            int selectedYear = 2014;
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            //Assert.AreEqual(6, bts.Count());
            Assert.AreEqual("tadk", bts[0].BTof.EID);
            Assert.AreEqual("tadk", bts[1].BTof.EID);
            Assert.AreEqual("xtwe", bts[2].BTof.EID);

            Assert.AreEqual(19, bts[0].BusinessTripID);
            Assert.AreEqual(13, bts[4].BusinessTripID);
            Assert.AreEqual(15, bts[5].BusinessTripID);
            Assert.AreEqual(2014, ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesPU_DefaultYear_NoBTs()
        {
            // Arrange
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU();
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU().Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        [Test]
        public void GetBusinessTripDataByDatesPU_NotExistingYear_NoBTs()
        {
            // Arrange
            int selectedYear = DateTime.Now.AddYears(-10).Year;
            PUController controller = new PUController(mock.Object, messengerMock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesPU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesPU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        #endregion

        #region GetEditFinishedBT
        [Test]
        public void GetEditFinishedBT_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.EditFinishedBT(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
    }

        #region GetSecondMailToLinkWithBcc

        [Test]
        public void GetSecondMailToLinkWithBcc_SDDDA_MailToLink()
        {
            //Arrange

            //Act
            var result = (ViewResult)controller.GetSecondMailToLinkWithBcc("SDDDA");
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(result.ViewName, "GetMailAliasEMails");
            Assert.AreEqual(currentUser, ((ViewResult)result).ViewBag.User);
        }
        #endregion

        [Test]
        public void GetEditFinishedBT_NotValidBT_NotFinished()
        {
            //Arrange

            // Act
            var view = controller.EditFinishedBT(14);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 14).FirstOrDefault();
            businessTrip.StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10);
            businessTrip.EndDate = DateTime.Now.ToLocalTimeAzure();
            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
}

        [Test]
        public void GetEditFinishedBT_ValidBT_Finished_ValidStatus()
        {
            //Arrange

            // Act

            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            businessTrip.StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10);
            businessTrip.EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1);

            var viewResult = controller.EditFinishedBT(16) as ViewResult;
            
            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditFinishedBT", viewResult.ViewName);
            Assert.AreEqual(16, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(16, businessTripModel.BusinessTripID);
            Assert.AreEqual(viewResult.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(viewResult.ViewBag.SelectedYear, 0);
            Assert.IsNotNull(viewResult.ViewBag.UnitsList);
            Assert.IsNotNull(viewResult.ViewBag.LocationsList); 
        }

        [Test]
        public void GetEditFinishedBT_ValidBT_Finished_SelectedYear_ValidStatus()
        {
            //Arrange

            // Act

            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            businessTrip.StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10);
            businessTrip.EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1);

            var viewResult = controller.EditFinishedBT(16, 2011) as ViewResult;

            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditFinishedBT", viewResult.ViewName);
            Assert.AreEqual(16, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(16, businessTripModel.BusinessTripID);
            Assert.AreEqual(viewResult.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(viewResult.ViewBag.SelectedYear, 2011);
            Assert.IsNotNull(viewResult.ViewBag.UnitsList);
            Assert.IsNotNull(viewResult.ViewBag.LocationsList);

        }

        #endregion

        #region PostEditFinishedBT

        [Test]
        public void PostEditFinishedBT_NullBT_404()
        {
            //Arrange
            BusinessTrip bt = null;

            // Act
            var view = controller.EditFinishedBT(bt, 0);
            ViewResult viewResult = view as ViewResult;

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void PostEditFinishedBT_NonExistingBT_404()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 0 };

            // Act
            var view = controller.EditFinishedBT(bt, 0);
            ViewResult viewResult = view as ViewResult;

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void PostEditFinishedBT_ModelError_404()
        {
            //Arrange
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            // Act
            controller.ModelState.AddModelError("abc", "Exception"); 
            var view = controller.EditFinishedBT(bt, 2014);
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(viewResult.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(viewResult.ViewBag.SelectedYear, 2014);
            Assert.IsNotNull(viewResult.ViewBag.UnitsList);
            Assert.IsNotNull(viewResult.ViewBag.LocationsList);
        } 

        [Test]
        public void PostEditFinishedBT_BTOK_JSON()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { 
                BusinessTripID = 1,
                StartDate = new DateTime(2013, 10, 09),
                EndDate = new DateTime(2013, 10, 10),
                OrderStartDate = new DateTime(2013, 10, 08),
                OrderEndDate = new DateTime(2013, 10, 11), 
                LocationID = 1, 
                DaysInBtForOrder = 4, 
                UnitID = 1, 
                Purpose = "p", 
                Manager = "m", 
                Responsible = "r", 
                Comment = "c", 
                CancelComment ="cc", 
                RejectComment ="r", 
                AccComment ="a", 
                BTMComment ="b", 
                Invitation = true, 
                Habitation = "h", 
                Flights = "f", 
                HabitationConfirmed = true, 
                FlightsConfirmed = true
                };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditFinishedBT(bTrip, 0);
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            Assert.AreEqual(bTrip.StartDate, new DateTime(2013, 10, 09));
            Assert.AreEqual(bTrip.EndDate, new DateTime(2013, 10, 10));
            Assert.AreEqual(bTrip.OrderStartDate, new DateTime(2013, 10, 08));
            Assert.AreEqual(bTrip.OrderEndDate, new DateTime(2013, 10, 11));
            Assert.AreEqual(bTrip.LocationID, 1);
            Assert.AreEqual(bTrip.DaysInBtForOrder,4);
            Assert.AreEqual(bTrip.UnitID, 1);
            Assert.AreEqual(bTrip.Purpose, "p");
            Assert.AreEqual(bTrip.Manager, "m");
            Assert.AreEqual(bTrip.Responsible, "r");
            Assert.AreEqual(bTrip.Comment, "c");
            Assert.AreEqual(bTrip.CancelComment,"cc");

            Assert.AreEqual(bTrip.RejectComment, "r");
            Assert.AreEqual(bTrip.AccComment, "a");
            Assert.AreEqual(bTrip.BTMComment,"b");
            Assert.AreEqual(bTrip.Invitation, true);
            Assert.AreEqual(bTrip.Habitation, "h");
            Assert.AreEqual(bTrip.Flights,"f");
            Assert.AreEqual(bTrip.HabitationConfirmed, true);
            Assert.AreEqual(bTrip.FlightsConfirmed, true);
            Assert.AreEqual(12, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(5, bTrip.BTof.Visa.EntriesUsedInBT); 
            Assert.AreEqual(typeof(JsonResult), result.GetType()); 
            Assert.AreEqual("success", data);
        }

        [Test]
        public void PostEditReportedBT_StartDateChangedAndVisaNullConcurrency_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditFinishedBT(bTrip, 0);
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data); 
        }

        [Test]
        public void PostEditReportedBT_BTCreationERROR_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new VacationAlreadyExistException());
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditFinishedBT(bTrip, 0);
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(btCreationError, data);
        }

        #endregion

        #endregion
    }
}
 