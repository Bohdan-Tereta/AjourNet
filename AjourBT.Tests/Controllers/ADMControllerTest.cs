using System;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Domain.ViewModels;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Concrete;
using System.Web;
using System.Web.Routing;
using System.Security.Principal;
using System.IO;
using System.Text;
using System.Collections;
using AjourBT.Infrastructure;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class ADMControllerTest
    {
        Mock<IRepository> mock; 
        Mock<IXLSExporter> xlsExporterMock;
        ADMController controller;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;
        string currentUser = "User";

        
        //TODO: duplicated
        private string modelError = "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";


        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();
            xlsExporterMock = new Mock<IXLSExporter>();

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

            controller = new ADMController(mock.Object, messengerMock.Object, xlsExporterMock.Object);
            controller.ControllerContext = controllerContext.Object;
        }

        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }

        #region GetVisaADM
        [Test]
        public void GetVisaADM_AllEmployeesInUserDepartment_TAAAA()
        {
            //Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            string userName = "ascr";
            //Act
            var resultView = controller.GetVisaADM(userName);
            //Assert        
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.IsInstanceOf(typeof(string), ((ViewResult)resultView).Model);
            Assert.AreEqual("TAAAA", ((ViewResult)resultView).Model);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)resultView).ViewBag.SelectedDepartment.Items);
        }

        [Test]
        public void GetVisaADM_AllEmployeesInUserDepartment_SDDDA()
        {
            //Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            string userName = "andl";
            //Act
            var resultView = controller.GetVisaADM(userName);
            //Assert        
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.IsInstanceOf(typeof(string), ((ViewResult)resultView).Model);
            Assert.AreEqual("SDDDA", ((ViewResult)resultView).Model);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)resultView).ViewBag.SelectedDepartment.Items);
        }

        #endregion

        #region GetVisaDataADM
        [Test]
        public void GetVisaDataADM_AllEmployeesInUserDepartment_TAAAA()
        {
            // Arrange      
            string departmentName = "";
            string userName = "ascr";
            string selectedUserDepartment = "TAAAA";

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM(departmentName, userName, selectedUserDepartment);
            string selectedDepartment = departmentName;

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "ascr", "TAAAA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual("Struz", employeeVisaView[0].LastName);
            Assert.AreEqual("Struz", employeeVisaView[0].LastName);

            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
        }

        [Test]
        public void GetVisaDataADM_AllEmployeesInUserDepartment_SDDDA()
        {
            // Arrange   

            // Act - call the action method

            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "zdul", "SDDDA");

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "zdul", "SDDDA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual("Kowwood", employeeVisaView[0].LastName);
        }

        [Test]
        public void GetVisaDataADM_AllEmployees()
        {
            // Arrange     

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "", "");

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "", "").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            //Assert.AreEqual("Zdvigkova", employeeVisaView[0].LastName);

            //Assert.AreEqual("Tymur", employeeVisaView[1].FirstName);
        }


        [Test]
        public void GetVisaDataADM_andzAllEmployeesInUserDepartment_SDDDA()
        {
            // Arrange 

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "andl", "SDDDA");

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "andl", "SDDDA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual("Petrenko", employeeVisaView[1].LastName);
            Assert.AreEqual("chap", employeeVisaView[1].EID);
            Assert.AreEqual("Chuck", employeeVisaView[1].FirstName);
        }
        #endregion

        #region GetBusinessTripADM

        [Test]
        public void GetBusinessTripADM_DefaultUserNameSelectedDepartmentNull_SelectedDepartmentNull()
        {
            // Arrange

            // Act
            string UserName = "";
            string selectedDepartment = null;
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.UserDepartment);

        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentNull_SelectDepartmentNull()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = null;
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;


            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.UserDepartment);
        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentStringEmpty_SelectDepartmentStringEmpty()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = "";
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;


            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.UserDepartment);
        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentRAAA1_SelectDepartmentRAAA1()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = "RAAA1";
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual("RAAA1", ((ViewResult)view).ViewBag.SelectedDepartment);

        }



        #endregion

        #region selectEmployees

        [Test]
        public void SelectEmployees_bothParametersAreNull_AllEmployees()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            IEnumerable<Employee> result = controller.SelectEmployees(null, null);

            //Assert
            Assert.AreEqual(23, result.Count());
        }

        [Test]
        public void SelectEmployees_OnlySelectedUseDepartmentIsNull_SelectedDepartmentEmployees()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            IEnumerable<Employee> result = controller.SelectEmployees("SDDDA", null);

            //Assert
            Assert.AreEqual(4, result.Count());
        }

        [Test]
        public void SelectEmployees_OnlySelectedDepartmentIsNull_SelectedUserDepartmentEmployees()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            IEnumerable<Employee> result = controller.SelectEmployees(null, "SDDDA");

            //Assert
            Assert.AreEqual(4, result.Count());
        }

        [Test]
        public void SelectEmployees_SelectedDepartmentEmpty_AllEmployees()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            IEnumerable<Employee> result = controller.SelectEmployees("", "SDDDA");

            //Assert
            Assert.AreEqual(23, result.Count());
        }

        [Test]
        public void SelectEmployeesBothAreNotNull_SelectedDepartmentEmployees()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            IEnumerable<Employee> result = controller.SelectEmployees("SDDDA", "TAAAA");

            //Assert
            Assert.AreEqual(4, result.Count());
        }



        #endregion

        #region GetBusinessTripDataADM

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentStringEmpty_AllEmployees()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "";
            string selectedUserDepartment = null;

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(employeeVisaView[1].EmployeeID, 2);
            Assert.AreEqual(employeeVisaView[2].EmployeeID, 5);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentNull_AllEmployeesInselectedUserDepartment()
        {
            // Arrange  

            // Act - call the action method
            string selectedDepartment = null;
            string selectedUserDepartment = "SDDDA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(17, employeeVisaView[1].EmployeeID);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentStringEmpty_AllEmployeesInselectedUserDepartment()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "";
            string selectedUserDepartment = "SDDDA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(employeeVisaView[1].EmployeeID, 2);
            Assert.AreEqual(employeeVisaView[2].EmployeeID, 5);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesInUserDepartment_RAAA3()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "RAAA3";
            string selectedUserDepartment = "TAAAA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(16, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 7);
            Assert.AreEqual(19, employeeVisaView[1].EmployeeID);
        }

        #endregion

        #region AddLastCRUDDataToBT
        [Test]
        public void AddLastCRUDDataToBT_BusinessTrip_CRUD_Data_Added()
        {
            //Arrange

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            string oldLastCRUDedBy = businessTrip.LastCRUDedBy;
            DateTime oldLastCRUDTimestamp = businessTrip.LastCRUDTimestamp;

            //Act
            controller.AddLastCRUDDataToBT(businessTrip);

            //Assert        
            Assert.AreNotEqual(oldLastCRUDedBy, businessTrip.LastCRUDedBy);
            Assert.AreEqual(businessTrip.LastCRUDedBy, "cbur");
            Assert.Greater(businessTrip.LastCRUDTimestamp, oldLastCRUDTimestamp);
            Assert.LessOrEqual(businessTrip.LastCRUDTimestamp, DateTime.Now.ToLocalTimeAzure());
        }
        //TODO: add to every test method which uses AddLastCRUDDataToBT mock.Verify.AddLastCRUDDataToBT(m => m.(bt), Times.Once);
        #endregion

        #region RegisterPlannedBTs

        [Test]
        public void RegisterPlannedBTs_SelectedTwoPlannedBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "10", "11" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Once);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "User");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_SelectedPlannedModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Once);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip1.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);
            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(null);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotPlannedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "2", "3" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_SelectedPlannedModifiedBTConcurrency_JsonErrorReturned()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region RegisterPlannedBT (Commented)

        //[Test]
        //public void RegisterPlannedBT_ValidBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;           
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);          
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void RegisterPlannedBT_NotValidBT_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //}

        //[Test]
        //public void RegisterPlannedBT_SelectedPlannedModifiedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;     
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //    Assert.IsNull(bTrip1.RejectComment);
        //}

        //[Test]
        //public void RegisterPlannedBT_NotValidModel_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act
        //    var result = (ViewResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
        //    Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
        //    Assert.AreEqual("EditPlannedBT", result.ViewName);
        //    Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
        //    Assert.IsInstanceOf(typeof(SelectList), result.ViewBag.LocationsList);


        //}


        #endregion

        #region ReplanRegisteredBTs
        [Test]
        public void ReplanRegisteredBTs_SelectedTwoRegisteredBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs, "RAAA1");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "User");
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs, "");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");

        }

        [Test]
        public void ReplanRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs, "SDDDA");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("SDDDA", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(null, "");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredModifiedBTConcurrency_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;


            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region CancelRegisteredBTs
        [Test]
        public void CancelRegisteredBTs_SelectedTwoRegisteredBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "User");
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");

        }

        [Test]
        public void CancelRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();


            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredModifiedBT_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region ConfirmPlannedBTs
        [Test]
        public void ConfirmPlannedBTs_SelectedTwoPlannedBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "10", "11" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed, bTrip2.Status);
            Assert.AreEqual(bTrip2.LastCRUDedBy, "User");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "User");
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
        }

        [Test]
        public void ConfirmPlannedBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs();

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedRejectedBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "20" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.IsNotNull(bTrip1.RejectComment);
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedModifiedBTConcurrency_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region ConfirmRegisteredBTs
        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredBT_StatusChanged()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.IsNull(bTrip1.RejectComment);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");

        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredBTAndRegisteredModified_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");

        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

        [Test]
        public void ConfirmRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }


        [Test]
        public void ConfirmRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(null, "SDDDA");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("SDDDA", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs, "RAAA1");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        public void ConfirmRegisteredBTs_SelectedRegisteredBTConcurrency_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "2" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void ConfirmRegisteredBTs_ValidModelConcurrency_ErrorReturned()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();

            string[] selectedIDsOfPlannedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(bTrip1)).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            // Act
            JsonResult result = (JsonResult)controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            Assert.IsNull(bTrip1.RejectComment);

        }

        #endregion

        #region CancelConfirmedBT
        [Test]
        public void CancelConfirmedBT_ExistingConfirmedBT_StatusChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(5, null);

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual(null, result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");
        }

        [Test]
        public void CancelConfirmedBT_ExistingConfirmedModifiedBT_StatusChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 14).FirstOrDefault();
            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(14, "");
            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "User");

        }

        [Test]
        public void CancelConfirmedBT_NotExistingBT_StatusChanged()
        {
            //Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();
            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(1000, "RAAA1");
            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA1", result.RouteValues["selectedDepartment"]);
            Assert.IsNull(bTrip1);
        }

        [Test]
        public void CancelConfirmedBT_ExistingReportedBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(22, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingPlannedBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(1, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingRegisteredBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(2, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingConfirmedBTConcurrency_JsonErrorResult()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            JsonResult result = (JsonResult)controller.CancelConfirmedBT(5, null);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert        
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region ConfirmPlannedBT (Commented)
        //[Test]
        //public void ConfirmPlannedBT_SelectedPlannedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;    
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ConfirmPlannedBT_SelectedPlannedModifiedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;    
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 28).FirstOrDefault();
        //    BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 28, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 25), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true };

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        //    Assert.AreEqual(businessTrip.LastCRUDedBy, "cbur");
        //    Assert.IsNull(businessTrip.RejectComment);
        //}

        //[Test]
        //public void ConfirmPlannedBT_SelectedNotPlannedBT_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //}

        //[Test]
        //public void ConfirmPlannedBT_NotValidModel_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act
        //    var result = (ViewResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))), Times.Never);
        //    Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
        //    Assert.AreEqual("EditPlannedBT", result.ViewName);
        //    Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
        //    Assert.IsInstanceOf(typeof(SelectList), result.ViewBag.LocationsList);


        //}
        #endregion

        #region DeletePlannedBT
        [Test]
        public void Get_DeletePlannedBT_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(10);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip, ((ViewResult)result).Model);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisPlannedModified_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(12);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip, ((ViewResult)result).Model);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisNotPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region DeletePlannedBTConfirmed
        [Test]
        public void DeletePlannedBTConfirmed_NotExistingBusinessTripID_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            var result = controller.DeletePlannedBTConfirmed(1000);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            mock.Verify(m => m.DeleteBusinessTrip(1000), Times.Never);
        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisPlanned_DeleteConfirmation()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();

            //Act
            var result = controller.DeletePlannedBTConfirmed(10);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(10), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip.LastCRUDedBy, "cbur");

        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisPlannedModified_DeleteConfirmation()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBTConfirmed(12);

            // Assert
            Assert.AreEqual(BTStatus.Planned | BTStatus.Cancelled, bTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip.LastCRUDedBy, "User");
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsPlannedModifiedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsPlannedModifiedToACC))), Times.Once);
        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisNotPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = controller.DeletePlannedBTConfirmed(2);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(2), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);

        }

        [Test]
        public void DeletePlannedBT_ValidBusinessTripID_BTisPlannedModified_Concurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            JsonResult result = (JsonResult)controller.DeletePlannedBTConfirmed(12);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region PlanGet
        [Test]
        public void GetPlanBT_ExistingEmployee_PlanBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
            var viewBagEmployeeVisa = employee.Visa;
            var viewBagLocationsList = from loc in mock.Object.Locations orderby loc.Title select loc;
            SelectList viewLocationsList = new SelectList(viewBagLocationsList, "LocationID", "Title");

            //Act
            var result = controller.Plan(1);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(viewBagEmployeeVisa, ((ViewResult)result).ViewBag.EmployeeVisa);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);

        }

        [Test]
        public void GetPlanBT_ExistingEmployeeHasNoVias_PlanBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 10 select e).FirstOrDefault();
            var viewBagLocationsList = from loc in mock.Object.Locations orderby loc.Title select loc;
            SelectList viewLocationsList = new SelectList(viewBagLocationsList, "LocationID", "Title");

            //Act
            var result = controller.Plan(10);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNull(((ViewResult)result).ViewBag.EmployeeVisa);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);

        }


        [Test]
        public void GetPlanBT_NonExistingEmployee_PlanBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1000 select e).FirstOrDefault();

            //Act
            var result = controller.Plan(1000);
            //Assert       
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);

        }

        #endregion

        #region PlanPost

        [Test]
        public void PostPlanBT_PlannedBT_PlanBTForm()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 0, StartDate = new DateTime(2015, 09, 01), EndDate = new DateTime(2015, 09, 27), Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting to Lodz" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }

        [Test]
        public void PostPlanBT_PlannedBTSelectedDepEmpty_PlanBTForm()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 0, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 27), Status = BTStatus.Planned, EmployeeID = 20, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting to Lodz" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt, "");

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }

        [Test]
        public void PostPlanBT_NotValidBTSelectedDepartment_PlanBTForm()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 0,
                StartDate = new DateTime(2013, 09, 01),
                EndDate = new DateTime(2013, 09, 27),
                OrderStartDate = new DateTime(2013, 09, 01),
                OrderEndDate = new DateTime(2013, 09, 27),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 28,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            controller.ModelState.AddModelError("error", "error");
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            var result = (ViewResult)controller.Plan(bt, "RAAA1");

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Plan", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostPlanBT_NotValidBT_PlanBTForm()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 0,
                StartDate = new DateTime(2013, 09, 01),
                EndDate = new DateTime(2013, 09, 27),
                OrderStartDate = new DateTime(2013, 09, 01),
                OrderEndDate = new DateTime(2013, 09, 27),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 27,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Plan", result.ViewName);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostPlanBT_TheSamePlannedModifiedBT_JsonErrorResult()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            bt.StartDate = new DateTime(2013, 08, 01);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt, "RAAA4");

            //Assert   

            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(20, bt.BusinessTripID);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Kyiv - Krakow", bt.Flights);
            Assert.AreEqual(1, bt.OldLocationID);
            Assert.AreEqual("cbur", bt.LastCRUDedBy);
            Assert.AreEqual(new DateTime(2013, 08, 01), bt.StartDate);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
        }

        [Test]
        public void PostPlanBT_PlannedModifiedBT_PlanTheSameBT()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            //Act
            var result = controller.Plan(bt, "RAAA4");
            //Assert 
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsNull(bt.RejectComment);
            Assert.AreEqual("Kyiv - Krakow", bt.Flights);
            Assert.AreEqual(1, bt.OldLocationID);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }

        [Test]
        public void PostPlanBT_PlannedModifiedBT_LocationChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, EmployeeID = 2, LocationID = 2 };
            var result = controller.Plan(bTrip, "RAAA4");

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsNull(bTrip.RejectComment);
            Assert.AreEqual("Kyiv - Krakow", bTrip.Flights);
            Assert.AreEqual(1, bTrip.OldLocationID);
            Assert.AreEqual("LDF", bTrip.OldLocationTitle);
            Assert.AreEqual(new DateTime(2013, 09, 01), bTrip.OldStartDate);
            Assert.AreEqual(bTrip.LastCRUDedBy, "cbur");

        }

        [Test]
        public void PostPlanBT_PlannedBTConcurrency_JsonErrorResult()
        {
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(bt)).Throws(new DbUpdateConcurrencyException());
            string selectedDep = "RAAA1";

            //Act
            var result = controller.Plan(bt, selectedDep);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void PostPlanBT_StartandEndDatesOverlayStartandEndDatesOfanotherBTSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 100,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_StartandEndDatesOverlayStartandEndDatesOfanotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "SSS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_StartDateOverlayEndDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_StartDatesOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 23),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLoo", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 24),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }

        [Test]
        public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);

        }

        [Test]
        public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {            
            //Arrange 

            mock.Object.BusinessTrips.Add(new BusinessTrip { BusinessTripID = 999, StartDate = new DateTime(2020, 12, 21), EndDate = new DateTime(2020, 12, 22), OrderStartDate = new DateTime(2020, 12, 20), OrderEndDate = new DateTime(2020, 12, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2020, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2020, 12, 18),
                EndDate = new DateTime(2020, 12, 21),
                OrderStartDate = new DateTime(2020, 12, 21),
                OrderEndDate = new DateTime(2020, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayStartDateAnotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange 

            mock.Object.BusinessTrips.Add(new BusinessTrip { BusinessTripID = 999, StartDate = new DateTime(2020, 12, 21), EndDate = new DateTime(2020, 12, 22), OrderStartDate = new DateTime(2020, 12, 20), OrderEndDate = new DateTime(2020, 12, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2020, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2020, 12, 18),
                EndDate = new DateTime(2020, 12, 21),
                OrderStartDate = new DateTime(2020, 12, 21),
                OrderEndDate = new DateTime(2020, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTPlanTheSameBTAnotherLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTPlanTheSameBTSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayTheSameBT_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 11, 06),
                EndDate = new DateTime(2014, 11, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_PlanBTOnSamePeriodAsVacation_JsonResult() //Vacation 12.02.2014  - 28.02.2014 empID = 1
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new VacationAlreadyExistException());
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = new DateTime(2014, 02, 12),
                EndDate = new DateTime(2014, 02, 28),
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("Absence already planned on this period for this user. "
                                      + "Please change OrderDates or if BT haven\'t OrderDates "
                                      + "change \'From\' or \'To\'", data);
        }
        #endregion

        #region PlanPostForEditPlanned

        [Test]
        public void PostPlanBTEdit_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 24),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }


        [Test]
        public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //  BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange

            mock.Object.BusinessTrips.Add(new BusinessTrip { BusinessTripID = 999, StartDate = new DateTime(2020, 12, 12), EndDate = new DateTime(2020, 12, 13), OrderStartDate = new DateTime(2020, 12, 11), OrderEndDate = new DateTime(2020, 12, 14), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2020, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2020, 12, 13),
                EndDate = new DateTime(2020, 12, 14),
                OrderStartDate = new DateTime(2020, 12, 21),
                OrderEndDate = new DateTime(2020, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);

        }


        [Test]
        public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            //BusinessTrip bt = new BusinessTrip
            //{
            //    BusinessTripID = 28,
            //    StartDate = new DateTime(2014, 12, 20),
            //    EndDate = new DateTime(2014, 12, 21),
            //    OrderStartDate = new DateTime(2014, 12, 21),
            //    OrderEndDate = new DateTime(2014, 12, 28),
            //    DaysInBtForOrder = 27,
            //    Status = BTStatus.Planned,
            //    EmployeeID = 7,
            //    LocationID = 1,
            //    Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
            //    Comment = "2 employee planned and rejected(with comment)",
            //    Manager = "xtwe",
            //    Purpose = "meeting",
            //    UnitID = 1,
            //    Unit = new Unit()
            //};

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 28).FirstOrDefault();


            //Act
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange

            mock.Object.BusinessTrips.Add(new BusinessTrip { BusinessTripID = 999, StartDate = new DateTime(2020, 12, 21), EndDate = new DateTime(2020, 12, 22), OrderStartDate = new DateTime(2020, 12, 20), OrderEndDate = new DateTime(2020, 12, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2020, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });

            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2020, 12, 18),
                EndDate = new DateTime(2020, 12, 21),
                OrderStartDate = new DateTime(2020, 12, 21),
                OrderEndDate = new DateTime(2020, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_EndDateOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange 

            mock.Object.BusinessTrips.Add(new BusinessTrip { BusinessTripID = 999, StartDate = new DateTime(2020, 12, 21), EndDate = new DateTime(2020, 12, 22), OrderStartDate = new DateTime(2020, 12, 20), OrderEndDate = new DateTime(2020, 12, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2020, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2020, 12, 18),
                EndDate = new DateTime(2020, 12, 21),
                OrderStartDate = new DateTime(2020, 12, 21),
                OrderEndDate = new DateTime(2020, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            // Assert.AreEqual("BT with same dates is already planned for this user. "
            // + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 1,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTPlanTheSameBTAnotherLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTedit_BTPlanTheSameBTSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayTheSameBT_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 11, 06),
                EndDate = new DateTime(2014, 11, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartandEndDatesOverlayStartandEndDatesOfanotherBTSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //  BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartandEndDatesOverlayStartandEndDatesOfanotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "SSS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayEndDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDatesOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 23),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLoo", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }
        //[Test]
        //public void PostPlanBTEdit_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 28,
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 27),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 28,
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 24),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 03),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert 
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());

        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 13),
        //        EndDate = new DateTime(2014, 12, 14),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert 
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());

        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 04),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTedit_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 03),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 13),
        //        EndDate = new DateTime(2014, 12, 14),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);

        //}


        //[Test]
        //public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 04),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 20),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 20),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 25),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 25),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBT_EndDateOverlayAnotherBTAnotherLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTPlanTheSameBTAnotherLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_BTPlanTheSameBTSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayTheSameBT_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 11, 06),
        //        EndDate = new DateTime(2014, 11, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}



        #endregion

        #region EditPlannedBT

        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_PlannedModified_EditPlannedBTView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            var viewBagEmployeeVisa = bTrip.BTof.Visa;

            //Act
            var result = (ViewResult)controller.EditPlannedBT(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("EditPlannedBT", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), resultModel);
            Assert.AreEqual(12, resultModel.BusinessTripID);
            Assert.IsTrue(resultModel.Status.HasFlag(BTStatus.Planned));
            Assert.AreEqual(viewBagEmployeeVisa, ((ViewResult)result).ViewBag.EmployeeVisa);
        }


        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_Planned_EditPlannedBTView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            var viewBagEmployeeVisa = bTrip.BTof.Visa;

            //Act
            var result = (ViewResult)controller.EditPlannedBT(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("EditPlannedBT", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), resultModel);
            Assert.AreEqual(12, resultModel.BusinessTripID);
            Assert.IsTrue(resultModel.Status.HasFlag(BTStatus.Planned));
            Assert.AreEqual(viewBagEmployeeVisa, ((ViewResult)result).ViewBag.EmployeeVisa);

        }




        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_NotPlanned_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = controller.EditPlannedBT(110);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void Get_EditPlannedBT_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            var result = controller.EditPlannedBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region ProcessCommands
        //[Test]
        //public void ProcessCommand_EditPlannedBTWithID_RedirectToEditPlannedBTMethod()
        //{
        //    //Arrange
        //    int id = 12;
        //    string commandName = "EditPlannedBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditPlannedBT", result.RouteValues["action"]);
        //    Assert.AreEqual(12, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_EditRegisteredBTWithID_RedirectToEditRegisteredBTMethod()
        //{
        //    //Arrange
        //    int id = 2;
        //    string commandName = "EditRegisteredBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditRegisteredBT", result.RouteValues["action"]);
        //    Assert.AreEqual(2, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_EditConfirmedBTWithID_RedirectToEditConfirmedBTMethod()
        //{
        //    //Arrange
        //    int id = 14;
        //    string commandName = "EditConfirmedBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditConfirmedBT", result.RouteValues["action"]);
        //    Assert.AreEqual(14, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Register_RedirectToRegisterPlannedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Register";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Confirm_RedirectToConfirmPlannedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Confirm";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);

        //}

        //[Test]
        //public void ProcessCommand_Plan__RedirectToPlanMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Plan ";
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();

        //    //Act
        //    var result = controller.ProcessCommand(businessTrip, 0, commandName, null);

        //    // Assert

        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Register__RedirectToRegisterPlannedBTMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Register ";
        //    BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    //Act
        //    var result = (ViewResult)controller.ProcessCommand(bt, 0, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");
        //    Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ProcessCommand_Confirm__RedirectToConfirmPlannedBTMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Confirm ";
        //    BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    //Act
        //    var result = controller.ProcessCommand(bt, 0, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");
        //    Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ProcessCommand_Confirm___ConfirmRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Confirm  ";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Replan_ReplanRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Replan";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Cancel_CancelRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Cancel";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Cancel__CancelConfirmedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Cancel ";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 5, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Default_RedirectToIndexMethod()
        //{
        //    //Arrange
        //    string commandName = "";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}
        #endregion

        #region RewriteBTsPropsAfterPlanningFromRepository

        [Test]
        public void RewriteBTsPropsAfterPlanningFromRepository_PlannedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, LocationID = 2, StartDate = new DateTime(2014, 11, 25), EndDate = new DateTime(2015, 10, 24), Comment = "123456" };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsAfterPlanningFromRepository(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("123456", bTrip.Comment);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        }

        [Test]
        public void RewriteBTsPropsAfterPlanningFromRepository_PlannedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, LocationID = 2, StartDate = new DateTime(2014, 11, 25), EndDate = new DateTime(2015, 10, 24), Comment = "123456" };
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();

            //Act
            BusinessTrip result = controller.RewriteBTsPropsAfterPlanningFromRepository(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.BusinessTripID);
            Assert.AreEqual(1, result.OldLocationID);
            Assert.AreEqual("visa expired", result.RejectComment);
            Assert.AreEqual("LDF", result.OldLocationTitle);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("123456", result.Comment);
            Assert.AreEqual(BTStatus.Modified | BTStatus.Planned, result.Status);
        }
        #endregion


        #region Employees tab

        #region GetEmployeeReadOnly
        [Test]
        public void GetEmployeeReadOnly_EstringEmpty_EmptySelectedDepartment()
        {
            //Arrange
            string selectedDepartment = String.Empty;
            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployeeReadOnly();
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

        #region GetEmployeeDataReadOnly

        [Test]
        public void GetEmployeeDataReadOnly_Null_AllEmployees()
        {
            // Arrange
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;

            // Act
            //IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeDataReadOnly(selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(view.ViewName, "");

        }

        [Test]
        public void GetEmployeeDataReadOnly_NotNull_AllEmployees()
        {
            // Arrange
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "BOARD";

            // Act
            //IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeDataReadOnly(selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(view.ViewName, "");

        }
        #endregion

        #region ExportEmployeesToExcel

        [Test]
        public void ExportEmployeesToExcelADM_DefaultDepartment_FileResult()
        {
            //Arrange 

            //Act 
            FileResult file = controller.ExportEmployeesToExcelADM("") as FileResult;

            //Assert 
            mock.Verify(m => m.SearchUsersData("", ""), Times.Once());

            xlsExporterMock.Verify(m => m.ExportEmployeesToExcelADM(It.IsAny<IList<EmployeeViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }


        [Test]
        public void ExportEmployeesToExcelADM_Dept1_FileResult()
        {
            //Arrange 

            //Act 
            FileResult file = controller.ExportEmployeesToExcelADM("SDDDA") as FileResult;

            //Assert 
            mock.Verify(m => m.SearchUsersData("SDDDA", ""), Times.Once());

            xlsExporterMock.Verify(m => m.ExportEmployeesToExcelADM(It.IsAny<IList<EmployeeViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }




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
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(result.ViewName, "");

        }

        #endregion

        #region GetSecondMailAliasEMails

        [Test]
        public void GetSecondMailAliasEMails_SDDDA_SDDDAEmployeesSecondMailsAndMailsIfSecondMailsNotExist()
        {
            //Arrange
            mock.Object.Employees.Where(e => e.EID == "andl").FirstOrDefault().EMail = "abc";
            mock.Object.Employees.Where(e => e.EID == "ivte").FirstOrDefault().EMail = "";
           
            //Act
            var result = (ViewResult)controller.GetSecondMailAliasEMails("SDDDA");
            
            //Assert
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
        #endregion

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

        #region EmployeeDetails
        [Test]
        public void EmployeeDetails_NonExistingEmployee()
        {
            //Arrange 

            //Act 
            var result = controller.EmployeeDetails(-1);

            //Assert        
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);

        }

        [Test]
        public void EmployeeDetails_ExistingEmployeeDefaultValues_EmployeeDetails()
        {
            //Arrange 
            int employeeId = 1;
            List<Department> departmentList = (from d in mock.Object.Departments select d).ToList();
            Employee emp = mock.Object.Users.FirstOrDefault(e => e.EmployeeID == employeeId);

            //Act 
            ViewResult result = (ViewResult)controller.EmployeeDetails(employeeId);

            //Assert        
            Assert.AreEqual(result.ViewName, "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), result.Model);
            Assert.AreEqual(emp.EmployeeID, ((EmployeeViewModel)result.Model).EmployeeID);
            Assert.AreEqual(result.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(result.ViewBag.Position, emp.Position.TitleEn);
            Assert.AreEqual(result.ViewBag.SelectedDepartment, null);
            Assert.AreEqual(result.ViewBag.SearchString, "");
            CollectionAssert.AreEqual(result.ViewBag.DepartmentList, departmentList);

        }

        [Test]
        public void EmployeeDetails_ExistingEmployeeDefaultValuesEmpPositionNull_EmployeeDetailsEmptyPosition()
        {
            //Arrange 
            int employeeId = 1;
            List<Department> departmentList = (from d in mock.Object.Departments select d).ToList();
            Employee emp = mock.Object.Users.FirstOrDefault(e => e.EmployeeID == employeeId);
            emp.Position = null;

            //Act 
            ViewResult result = (ViewResult)controller.EmployeeDetails(employeeId);

            //Assert        
            Assert.AreEqual(result.ViewName, "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), result.Model);
            Assert.AreEqual(emp.EmployeeID, ((EmployeeViewModel)result.Model).EmployeeID);
            Assert.AreEqual(result.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(result.ViewBag.Position, null);
            Assert.AreEqual(result.ViewBag.SelectedDepartment, null);
            Assert.AreEqual(result.ViewBag.SearchString, "");
            CollectionAssert.AreEqual(result.ViewBag.DepartmentList, departmentList);

        }

        [Test]
        public void EmployeeDetails_ExistingEmployeeDepartmentSearchString_EmployeeDetailsEmptyPosition()
        {
            //Arrange 
            int employeeId = 1;
            List<Department> departmentList = (from d in mock.Object.Departments select d).ToList();
            Employee emp = mock.Object.Users.FirstOrDefault(e => e.EmployeeID == employeeId);

            //Act 
            ViewResult result = (ViewResult)controller.EmployeeDetails(employeeId, "SDDDA", "abcdef");

            //Assert        
            Assert.AreEqual(result.ViewName, "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), result.Model);
            Assert.AreEqual(emp.EmployeeID, ((EmployeeViewModel)result.Model).EmployeeID);
            Assert.AreEqual(result.ViewBag.JSDatePattern, MvcApplication.JSDatePattern);
            Assert.AreEqual(result.ViewBag.Position, emp.Position.TitleEn);
            Assert.AreEqual(result.ViewBag.SelectedDepartment, "SDDDA");
            Assert.AreEqual(result.ViewBag.SearchString, "abcdef");
            CollectionAssert.AreEqual(result.ViewBag.DepartmentList, departmentList);

        }



        #endregion

        #endregion


        #region DropDownList

        [Test]
        public void LocationsDropDownList_Default_AllLocations()
        {
            // Arrange

            // Act          
            var result = controller.Plan(1);

            IEnumerable<Location> departmentsList = from l in mock.Object.Locations
                                                    orderby l.Title
                                                    select l;

            // Assert
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
        }


        [Test]
        public void UnitsDropDownList_Default_AllUnits()
        {
            // Arrange

            // Act          
            var result = controller.Plan(1);

            IEnumerable<Unit> unitsList = from l in mock.Object.Units
                                          orderby l.ShortTitle
                                          select l;

            // Assert
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.UnitsList);
        }

        #endregion

        [Test]
        public void CalculateVisaDays_ButtonPlan_CorrectNumber()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip
            {
                Status = BTStatus.Planned,
                EmployeeID = 1,
                Location = new Location(),
                Unit = new Unit()
            };

            int spentDays = 13;
         
            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 

            Assert.AreEqual(spentDays, result);
            Assert.IsInstanceOf(typeof(Int32), result);

        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_CorrectNumber()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            int spentDays = 21;
          
            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);

        }

        [Test]
        public void CalculateVisaDays_WithoutStartEndDates_TheSameresult()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip
            {
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            int spentDays = 13;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        }

        [Test]
        public void CalculateVisaDays_EmployeeWithoutVisa_ZeroDays()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip
            {
                Status = BTStatus.Planned,
                EmployeeID = 15,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            int spentDays = 0;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        }

        [Test]
        public void CalculateVisaDays_EmployeeWithoutBtsAndPrivatetrips_ZeroDays()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip
            {
                Status = BTStatus.Planned,
                EmployeeID = 18,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            int spentDays = 0;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTButNotConfirmedReported_CorrectNumber()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-9).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4).Date,
                OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10).Date,
                OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5).Date,
                Status = BTStatus.Confirmed,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            int spentDays = 21;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);

        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported_OrderStartDateNull_CorrectNumber()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-9).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4).Date,
                OrderStartDate = null,
                OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5).Date,
                Status = BTStatus.Confirmed | BTStatus.Reported,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            int spentDays = 21;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);

        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported_OrderEndDateNull_CorrectNumber()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-9).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4).Date,
                OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10).Date,
                OrderEndDate = null,
                Status = BTStatus.Confirmed | BTStatus.Reported,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            int spentDays = 21;

            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);

        }

        [TestCase(-5, -6, Result = 21)]
        [TestCase(1, 2, Result=21)]
        [TestCase(0, 1, Result = 22)]
        [TestCase(-1, 0, Result = 23)]
        [TestCase(-180, -179, Result = 23)]
        [TestCase(-181, -180, Result = 22)]
        [TestCase(-182, -181, Result = 21)]
        public int CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported(int orderStartDaysDelta, int orderEndDaysDelta)
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = bTrip.EndDate.AddDays(orderStartDaysDelta),
                EndDate = bTrip.EndDate.AddDays(orderEndDaysDelta),
                OrderStartDate = bTrip.EndDate.AddDays(orderStartDaysDelta),
                OrderEndDate = bTrip.EndDate.AddDays(orderEndDaysDelta),
                Status = BTStatus.Confirmed | BTStatus.Reported,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });

            //Act        
            var result = controller.CalculateVisaDays(bTrip); 

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
            return result; 

        }

        [TestCase(-5, -6, Result = 21)]
        [TestCase(1, 2, Result = 21)]
        [TestCase(0, 1, Result = 22)]
        [TestCase(-1, 0, Result = 23)]
        [TestCase(-180, -179, Result = 23)]
        [TestCase(-181, -180, Result = 22)]
        [TestCase(-182, -181, Result = 21)]
        public int CalculateVisaDays_NewPlanedBT_EmployeeHasPrivateTrip(int StartDateDelta, int EndDateDelta)
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();

            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            mock.Object.PrivateTrips.Add(new PrivateTrip
            {
                StartDate = bTrip.EndDate.Date.AddDays(StartDateDelta).Date,
                EndDate = bTrip.EndDate.Date.AddDays(EndDateDelta).Date,
                EmployeeID = 1
            });

            //Act        
            var result = controller.CalculateVisaDays(bTrip);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);

            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
            return result; 

        }

        [TestCase(-5, -6, Result = 0)]
        [TestCase(1, 2, Result = 0)]
        [TestCase(0, 1, Result = 1)]
        [TestCase(-1, 0, Result = 2)]
        [TestCase(-180, -179, Result = 2)]
        [TestCase(-181, -180, Result = 1)]
        [TestCase(-182, -181, Result = 0)]
        public int DaysSpentInTrip(int StartDateDelta, int EndDateDelta)
        {

            //Arrange
            DateTime EndDate = DateTime.Now.ToLocalTimeAzure();
            DateTime StartDate = EndDate.AddDays(-180);

            //Act        
            var result = controller.DaysSpentInTrip(StartDate, EndDate, EndDate.AddDays(StartDateDelta), EndDate.AddDays(EndDateDelta)); 

            //Assert 
            return result;

        }


        [Test]
        public void DaysSpentInTrip_StartDateNull_0Days()
        {
            //Arrange
            int spentDays = 0;
            DateTime startPeriod = DateTime.Now.ToLocalTimeAzure().AddDays(-180).Date;
            DateTime endPeriod = DateTime.Now.ToLocalTimeAzure();
            DateTime? startDate = null;
            DateTime? endDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10).Date;

            //Act        
            var result = controller.DaysSpentInTrip(startPeriod, endPeriod, startDate, endDate);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }

        [Test]
        public void DaysSpentInTrip_EndDateNull_0Days()
        {
            //Arrange
            int spentDays = 0;
            DateTime startPeriod = DateTime.Now.ToLocalTimeAzure().AddDays(-180).Date;
            DateTime endPeriod = DateTime.Now.ToLocalTimeAzure();
            DateTime? startDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10).Date;
            DateTime? endDate = null;

            //Act        
            var result = controller.DaysSpentInTrip(startPeriod, endPeriod, startDate, endDate);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }


    }
}
