using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjourBT.Domain.Infrastructure;
using System.Web.Configuration;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using Newtonsoft.Json;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class ACCControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;
        ACCController controller;

        string modelError = "The record you attempted to edit "
                              + "was modified by another user after you got the original value. The "
                              + "edit operation was canceled.";

        StringBuilder comment = new StringBuilder();
        string defaultAccComment;

        byte[] rowVersion = { 0, 0, 0, 0, 0, 0, 8, 40 };

        public ACCControllerTest()
        {
            comment.Append("ВКО №   від   , cума:   UAH.");
            comment.AppendLine();
            comment.Append("ВКО №   від   , cума:   USD.");
            defaultAccComment = comment.ToString();
        }

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();

            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.UnknownType))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsPlannedModifiedToBTM))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsPlannedModifiedToACC))).Verifiable(); 
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToResponsible))).Verifiable();

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

            controller = new ACCController(mock.Object, messengerMock.Object);
            controller.ControllerContext = controllerContext.Object;

        }

        #region GetBusinessTrip

        [Test]
        public void GetBusinessTrip_EmptyString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "";

            // Act

            var view = controller.GetBusinessTrip(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTrip_NullString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = null;

            // Act

            var view = controller.GetBusinessTrip(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTrip_SDDDA_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "SDDDA";

            // Act

            var view = controller.GetBusinessTrip(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion
        
        #region IndexForAccountableBTs

        [Test]
        public void IndexACCforAccountableBTs()
        {
            //Arrange
            // Act

            var query = controller.IndexForAccountableBTs();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), query);
            Assert.AreEqual("", ((ViewResult)query).ViewName);
            Assert.AreEqual(1, ((List<BusinessTrip>)query.ViewData.Model).Count);
            Assert.AreEqual(17, ((List<BusinessTrip>)query.Model).ToArray()[0].EmployeeID);
            Assert.AreEqual("ncru", ((List<BusinessTrip>)query.Model).ToArray()[0].LastCRUDedBy);
            Assert.AreEqual(1, ((List<BusinessTrip>)query.Model).ToArray()[0].LocationID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), ((List<BusinessTrip>)query.Model).ToArray()[0].Status);
        }

        #endregion

        #region GetBusinessTripACC

        [Test]
        public void GetBusinessTripACC_EmptyString_EmptyString()
        {
            //Arrange
            string searchString = "";

            //Act
            var result = controller.GetBusinessTripACC(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).Model);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripACC_NotEmptyString_String()
        {
            //Arrange
            string searchString = "hello";

            //Act
            var result = controller.GetBusinessTripACC(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).Model);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripACC_NullString_EmptyString()
        {
            //Arrange
            string searchString = null;

            //Act
            var result = controller.GetBusinessTripACC(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).Model);
            Assert.AreEqual(null, result.ViewBag.SearchString);
        }

        #endregion

        #region GetBusinessTripDataACC
        [Test]
        public void GetBusinessTripDataACC_EmptyDepStringAndSearchstring_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment,searchString).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment,searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(15, result.ToArray().Length);

            Assert.AreEqual(31, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(34, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(35, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(25, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(37, result.ToArray()[11].BusinessTripID);

            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataACC_DepartmentNullAndSearchStringEmpty_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = null;
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment,searchString).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment,searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(15, result.ToArray().Length);

            Assert.AreEqual(31, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(34, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(35, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(25, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(37, result.ToArray()[11].BusinessTripID);

            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("", ((PartialViewResult)view).ViewBag.SearchString);
        }


        [Test]
        public void GetBusinessTripDataACC_RAAA3EmptySearchString_BusinessTripsOfRAAA3()
        {
            //Arrange
            string selectedDepartment = "RAAA3";


            BusinessTrip btrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            btrip.AccComment = defaultAccComment;

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(((PartialViewResult)view).Model, result.ToList());
            Assert.AreEqual(6, result.ToArray().Length);
            Assert.AreEqual("xtwe", result.ToArray()[0].BTof.EID);
            Assert.AreEqual("iwoo", result.ToArray()[1].BTof.EID);
            Assert.AreEqual("iwpe", result.ToArray()[2].BTof.EID);
            Assert.AreEqual("xtwe", result.ToArray()[3].BTof.EID);
            Assert.AreEqual("iwoo", result.ToArray()[4].BTof.EID);

            Assert.AreEqual(32, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(22, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(39, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(21, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(16, result.ToArray()[4].BusinessTripID);

            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("", ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataACC_DepartmentNull_SearchStringEmpty_BusinessTripsList()
        {
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(searchString).Model;
            var view = controller.GetBusinessTripDataACC(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(((PartialViewResult)view).Model, result.ToList());
            Assert.AreEqual(15, result.ToArray().Length);
            Assert.AreEqual("tedk", result.ToArray()[0].BTof.EID);
            Assert.AreEqual("xomi", result.ToArray()[1].BTof.EID);
            Assert.AreEqual("xomi", result.ToArray()[2].BTof.EID);
            Assert.AreEqual("daol", result.ToArray()[3].BTof.EID);
            Assert.AreEqual("daol", result.ToArray()[4].BTof.EID);

            Assert.AreEqual(31, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(34, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(35, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(25, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(33, result.ToArray()[4].BusinessTripID);

            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region GetClosedBusinessTrip

        [Test]
        public void GetClosedBusinessTrip_WithoutSearchString_ViewBagEmptySearchString()
        {
            //Arrange
            string searchString = "";
            //Act
            var result = controller.GetClosedBusinessTrip();

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);                        
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).Model);
        }

        [Test]
        public void GetClosedBusinessTrip_SearchStringTest_ViewBagTestSearchString()
        {
            //Arrange
            string searchString = "Test";
            //Act
            var result = controller.GetClosedBusinessTrip(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).Model);
        }

        #endregion

        #region GetClosedBusinessTripData

        [Test]
        public void GetClosedBusinessTripData_WithoutSearchString_ViewBagSearchStringEmpty()
        {
            //Arrange
            string searchString = "";

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            bTrip.AccComment = "TestComment";
            bTrip.EndDate = new DateTime(2014, 12, 12);

            BusinessTrip testBt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            bTrip.AccComment = "TestComment1";
            bTrip.EndDate = new DateTime(2015, 01, 01);
            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetClosedBusinessTripData().Model;
            var view = controller.GetClosedBusinessTripData();

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(4, result.ToArray().Length);

            Assert.AreEqual(32, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(23, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(27, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(15, result.ToArray()[3].BusinessTripID);

            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetClosedBusinessTripData_searchStringmeno_ViewBagSearchStringEmpty()
        {
            //Arrange
            string searchString = "mano";

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            bTrip.AccComment = "TestComment";
            bTrip.EndDate = new DateTime(2014, 12, 12);

            BusinessTrip testBt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            bTrip.AccComment = "TestComment1";
            bTrip.EndDate = new DateTime(2015, 01, 01);
            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetClosedBusinessTripData(searchString).Model;
            var view = controller.GetClosedBusinessTripData(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(2, result.ToArray().Length);

            Assert.AreEqual(32, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(15, result.ToArray()[1].BusinessTripID);

            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region SearchClosedBusinessTripData

        [Test]
        public void SearchClosedBusinessTripData_EmptysearchString_AllBTs()
        {
            //Arrange
            string searchString = "";

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            bTrip.AccComment = "TestComment";
            bTrip.EndDate = new DateTime(2014, 12, 12);

            BusinessTrip testBt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            bTrip.AccComment = "TestComment1";
            bTrip.EndDate = new DateTime(2015, 01, 01);

            //Act
            List<BusinessTrip> data = controller.SearchClosedBusinessTripData(mock.Object.BusinessTrips.ToList(), searchString);

            //Assert
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
            Assert.AreEqual("Manowens", data.Last().BTof.LastName);
            Assert.AreEqual(new DateTime(2014, 12, 15), data.Last().StartDate);
        }

        [Test]
        public void SearchClosedBusinessTripData_searchStringManowens_OneBT()
        {
            //Arrange
            string searchString = "Manowens";

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            bTrip.AccComment = "TestComment";
            bTrip.EndDate = new DateTime(2014, 12, 12);

            BusinessTrip testBt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            bTrip.AccComment = "TestComment1";
            bTrip.EndDate = new DateTime(2015, 01, 01);

            //Act
            List<BusinessTrip> data = controller.SearchClosedBusinessTripData(mock.Object.BusinessTrips.ToList(), searchString);

            //Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
            Assert.AreEqual("Manowens", data.Last().BTof.LastName);
            Assert.AreEqual(new DateTime(2014, 12, 15), data.Last().StartDate);
        }

        #endregion

        #region SearchSearchBusinessTripDataACC

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentNullSearchStringEmpty_AllBts()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "";

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            bTrip.AccComment = defaultAccComment;
            bTrip.EndDate = new DateTime(2014, 12, 12);

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            //Assert
            Assert.AreEqual(15, data.Count);
            Assert.AreEqual("Pyorge", data.First().BTof.LastName);
            Assert.AreEqual(new DateTime(2012, 04, 22), data.First().StartDate);
            Assert.AreEqual("Manowens", data.Last().BTof.LastName);
            Assert.AreEqual(DateTime.Now.AddDays(2).Date, data.Last().StartDate.Date);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentStringEmptySearchStringMan_()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "Man";

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            //Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(new DateTime(2013, 12, 25), data.First().StartDate);
            Assert.AreEqual("Manowens", data.Last().BTof.LastName);
            Assert.AreEqual(DateTime.Now.AddDays(2).Date, data.Last().StartDate.Date);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentNotExistsSearchStringAn_()
        {
            //Arrange
            string selectedDepartment = "AAAA";
            string searchString = "An";

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            //Assert
            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentRAAA3SearchA_()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "A";
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            bTrip.AccComment = defaultAccComment;

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);


            //Assert
            Assert.AreEqual(4 , data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(32, data.ToArray()[0].BusinessTripID);
            Assert.AreEqual(39, data.ToArray()[1].BusinessTripID);
            Assert.AreEqual(21, data.ToArray()[2].BusinessTripID);

            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_DefaultAccComment_()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "A";
            BusinessTrip btFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            btFromRepository.AccComment = defaultAccComment;

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            //Assert
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual(32, data.ToArray()[0].BusinessTripID);
            Assert.AreEqual(39, data.ToArray()[1].BusinessTripID);
            Assert.AreEqual(21, data.ToArray()[2].BusinessTripID);
            Assert.AreEqual(defaultAccComment, btFromRepository.AccComment);

            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
        }

        #endregion
                
        #region GetEditReportedBT
        [Test]
        public void GetEditReportedBT_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.EditReportedBT(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void GetEditReportedBT_ValidBT_NotValidStatus()
        {
            //Arrange

            // Act
            var view = controller.EditReportedBT(14);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 14).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(14, businessTrip.BusinessTripID);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void GetEditReportedBT_ValidBT_StartDateMoreThenDateNow_ValidStatus()
        {
            //Arrange
            
            // Act
            var viewResult = controller.EditReportedBT(16) as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.AreEqual(16, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(16, businessTripModel.BusinessTripID);
        }

        [Test]
        public void GetEditReportedBT_ValidBT_StartDateLessOrIsEqualToDateNow_ValidStatus()
        {
            //Arrange

            // Act
            var viewResult = controller.EditReportedBT(22) as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(22, businessTripModel.BusinessTripID);
        }

        [Test]
        public void GetEditReportedBT_ValidBTAccComment_StartDateLessToDateNow_View()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip();
            bTrip.AccComment = defaultAccComment;

            //Act
            var result = controller.EditReportedBT(25, null) as ViewResult;

            //Assert
            Assert.AreEqual("EditReportedFinishedBT", result.ViewName);
        }

        #endregion
        
        #region PostEditReportedBT

        //[Test]
        //public void PostEditReportedBT_NotExistingBT_HttpNotFound()
        //{
        //    //Arrange

        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

        //    // Act
        //    var view = controller.EditReportedBT(businessTrip, "");


        //    //Assert 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), view.GetType());
        //    Assert.IsNull(businessTrip);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        //}

        //[Test]
        //public void PostEditReportedBT_NullInputBT_HttpNotFound()
        //{
        //    //Arrange

        //    BusinessTrip businessTrip = null;

        //    // Act
        //    var view = controller.EditReportedBT(businessTrip, "");


        //    //Assert 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
        //    Assert.IsNull(businessTrip);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        //}

        [Test]
        public void PostEditReportedBT_NotValidPlannedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(5), Status = BTStatus.Planned, EmployeeID = 1, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);

            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidPlannedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(5), Status = BTStatus.Planned, EmployeeID = 1, UnitID = 1, Unit = new Unit() };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidRegisteredBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(1), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true, UnitID = 1, Journeys = new List<Journey>(), };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidRegisteredBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(1), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true, UnitID = 1, Journeys = new List<Journey>(), };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidConfirmedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate =  DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidConfirmedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, UnitID = 1, Unit = new Unit() };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidConfirmedModifiedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 4, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(14), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidConfirmedModifiedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 4, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(14), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2, UnitID = 1, Unit = new Unit() };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidReportedCurrentBT_NullSelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 29, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTrip), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidReportedCurrentBT_EmptySelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 29, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTrip), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidPlannedCurrentBT_SDDDASelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "SDDDA");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTrip), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBT_Viewresult()
        {

            //Arrange
            //messengerMock = new Mock<IMessenger>();
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, "SDDDA");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), bt.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bt.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBTEmptySelectedDep_Viewresult()
        {
            //Arrange
            //messengerMock = new Mock<IMessenger>();
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), bt.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bt.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBTNullSelectedDep_Viewresult()
        {
            //Arrange
            //messengerMock = new Mock<IMessenger>();
            //BusinessTrip bt =  new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure(), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), DaysInBtForOrder = ((DateTime.Now.ToLocalTimeAzure().Date.AddDays(2)).Date - (new DateTime(2013, 09, 30)).Date).Days + 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, null);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), businessTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), businessTrip.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_StartDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange

            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(String.Format("dd.MM.yyyy", DateTime.Now.AddDays(1)),String.Format("dd.MM.yyyy", bTrip.OldStartDate));
            Assert.AreEqual(new DateTime(2013, 10, 10), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 11, 30), bTrip.OrderStartDate);
            Assert.AreEqual(String.Format("dd.MM.yyyy",DateTime.Now.AddDays(5)),String.Format("dd.MM.yyyy", bTrip.OldEndDate));
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderStartDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 29), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 14, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 14).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(String.Format("dd.MM.yyyy",DateTime.Now.AddDays(1)),String.Format("dd.MM.yyyy", bTrip.OldStartDate));
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 11, 29), bTrip.OrderStartDate);
            Assert.AreEqual(String.Format("dd.MM.yyyy",DateTime.Now.AddDays(5)),String.Format("dd.MM.yyyy", bTrip.OldEndDate));
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderStartDateChangedToNullAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 26), OrderStartDate = null, OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(String.Format("dd.MM.yyyy", DateTime.Now.AddDays(1)),String.Format("dd.MM.yyyy", bTrip.OldStartDate));
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.StartDate);
            Assert.AreEqual(null, bTrip.OrderStartDate);
            Assert.AreEqual(String.Format("dd.MM.yyyy",DateTime.Now.AddDays(5)),String.Format("dd.MM.yyyy", bTrip.OldEndDate));
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_EndDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(2), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(2), bTrip.EndDate);
            //  Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), bTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderEndDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.EndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate.Value.Date);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), bTrip.OrderEndDate.Value.Date);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderEndDateChangedToNullAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = null, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.EndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate.Value.Date);
            Assert.AreEqual(null, bTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_LocatioIDChangedAndVisaNull_NullSelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 2, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(2, bTrip.LocationID);
            Assert.AreEqual(1, bTrip.OldLocationID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_FutureBTOrderDatesNullsAndVisaNotNull_EmptySelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = null, OrderEndDate = null, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(null, bTrip.OrderStartDate);
            Assert.AreEqual(null, bTrip.OrderEndDate);
            Assert.AreEqual(null, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_FutureBTDefaultOrderDatesAndVisaNotNull_NullSelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = default(DateTime), OrderEndDate = default(DateTime), DaysInBtForOrder = (default(DateTime).Date - default(DateTime).Date).Days + 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(default(DateTime), bTrip.OrderStartDate);
            Assert.AreEqual(default(DateTime), bTrip.OrderEndDate);
            Assert.AreEqual(1, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }


        [Test]
        public void PostEditReportedBT_FutureBTOrderDatesNotNullsAndVisaNotNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = new DateTime(2014, 12, 09), OrderEndDate = new DateTime(2014, 12, 13), DaysInBtForOrder = ((new DateTime(2014, 12, 13)).Date - (new DateTime(2014, 12, 09)).Date).Days + 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.AreEqual(22, bTripFromMock.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTripFromMock.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 12, 09), bTrip.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 13), bTrip.OrderEndDate);
            Assert.AreEqual(5, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_CurrentBTAndVisaNotNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTripFromMock.BTof = employee;
            bTripFromMock.BTof.Visa = visa;
            int? NewDaysUsedInBT = bTripFromMock.BTof.Visa.DaysUsedInBT - ((DateTime.Now.ToLocalTimeAzure().Date - new DateTime(2013, 10, 01).Date).Days + 1);

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Once);
            Assert.AreEqual(NewDaysUsedInBT, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTripFromMock.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
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
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion
        
        #region CancelReportedBT

        [Test]
        public void CancelReportedBTGet_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBT_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBTIdAndEmptyString_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3, "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBTAndSDDDA_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3, "SDDDA");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion

        #region CancelReportedBTConfirm

        [Test]
        public void CancelReportedBTConfirm_NullBusinessTrip_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(0, "", null);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTConfirm_NullBusinessTripEmptyDepartment_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(0, "too expensive", "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelComment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, null);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.IsNull(((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StringEmptyCancelComment_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, "", null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == ""
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.IsNull(((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelCommentAndEmptyDepartment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelCommentSelectedDepartment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, "RAAA4");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.AreEqual("RAAA4", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StringEmptyCancelCommentSelectedDepartment_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, "", "RAAA4");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == ""
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("RAAA4", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StatusOfBTisPlanned_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(1, "too expensive");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "too expensive")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndCommentIsNull_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(null, businessTrip.CancelComment);

        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndCommentIsEmpty_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, "", "");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNull_CanCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(22, "BT is too expensive");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Once);

            Assert.IsNull(businessTrip.BTof.Visa);

        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNullSelectedDept_CanCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(22, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Once);
            Assert.IsNull(businessTrip.BTof.Visa);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNull_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 

            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);

            //Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            //Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            //Assert.AreEqual("ACCView", ((RedirectToRouteResult)view).RouteValues["action"]);
            //Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            //Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            //Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            //Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);

            //Assert.AreEqual("BT is too expensive", bTrip.CancelComment);
            //Assert.AreEqual((BTStatus.Confirmed | BTStatus.Cancelled), bTrip.Status);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNullSelectedDept_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Once);

            //Assert.AreEqual("BT is too expensive", bTrip.CancelComment);
            //Assert.AreEqual((BTStatus.Confirmed | BTStatus.Cancelled), bTrip.Status);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNullSelectedDeptConcurrency_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);

            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsible))), Times.Never);

            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region SaveAccComment

        [Test]
        public void SaveAccComment_nullBT_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = null;

            //Act

            var result = controller.SaveAccComment(bTrip);

            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(bTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void SaveAccComment_PlannedBT_JsonError()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("error", data);
            Assert.AreEqual(BTStatus.Planned, bTripAfterMethodCalled.Status);
        }

        [Test]
        public void SaveAccComment_ReportedBT_JsonSuccess()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault();

            //Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "success")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("success", data);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTripAfterMethodCalled.Status);
            Assert.AreEqual("Test Comment", bTripAfterMethodCalled.AccComment);
        }

        [Test]
        public void SaveAccComment_ReportedBTConcurrency_JsonErrorReturned()
        {
            // Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 15, AccComment = "test2", Status = BTStatus.Confirmed | BTStatus.Reported };
            mock.Setup(m => m.SaveBusinessTrip(bTrip)).Throws(new DbUpdateConcurrencyException());

            // Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault();

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.AreEqual("Test Comment", bTripAfterMethodCalled.AccComment);
            Assert.AreEqual(modelError, data);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTripAfterMethodCalled.Status);
        }

        #endregion
        
        #region ShowAccountableBTData


        [Test]
        public void ShowAccountableBTData_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowAccountableBTData(777);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void ShowAccountableBTData_NotExistingBussinessTrip_HttpNotFound()
        {
            //Arrange

            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowAccountableBTData(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowAccountableBTData_ShowBTWithBussinessTripIDIs1_ShowAccountableBTDataView()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.ShowAccountableBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowAccountableBTData_ExistingBT_ShowAccountableBTDataView()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            var view = controller.ShowAccountableBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(10, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowAccountableBTData_ExistingBusinessTripID_ShowAccountableBTDataView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = (ViewResult)controller.ShowAccountableBTData(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(12, resultModel.BusinessTripID);
        }

        [Test]
        public void ShowAccountableBTData_NotExistingBusinessTrip_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowAccountableBTData(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }
        #endregion

        #region SearchBusinessTripAccountableACC

        [Test]
        public void SearchBusinessTripAccountableACC_SearchStringEmpty_NotEmptyResult()
        {
            //Arrange
            string searchString = "";

            //Act
            var result = controller.SearchBusinessTripAccountableACC(mock.Object.BusinessTrips.ToList(), searchString);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(17, result.ToArray()[0].EmployeeID);
            Assert.AreEqual("LDF", result.ToArray()[0].Location.Title);

        }

        [Test]
        public void SearchBusinessTripAccountableACC_chap_NotEmptyResult()
        {
            //Arrange

            string searchString = "chap";

            //Act
            var result = controller.SearchBusinessTripAccountableACC(mock.Object.BusinessTrips.ToList(), searchString);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(17, result.ToArray()[0].EmployeeID);
            Assert.AreEqual("LDF", result.ToArray()[0].Location.Title);
        }

        [Test]
        public void SearchBusinessTripAccountableACC_BadString_EmptyResult()
        {
            //Arrange

            string searchString = "BadString";

            //Act
            var result = controller.SearchBusinessTripAccountableACC(mock.Object.BusinessTrips.ToList(), searchString);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        //TODO: duplicated in ACC, BTM
        [Test]
        public void CountingDaysUsedInBT_ReportedBT_IncreasedDaysUsedInBT()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            int result1 = controller.CountingDaysUsedInBT(bTrip);

            //Assert
            Assert.AreEqual(20, result1);
        }


    }
}