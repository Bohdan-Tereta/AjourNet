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
    public class DIRControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;
        DIRController controller;

        string modelError = "The record you attempted to edit "
                              + "was modified by another user after you got the original value. The "
                              + "edit operation was canceled.";

        StringBuilder comment = new StringBuilder();
        string defaultAccComment;

        byte[] rowVersion = { 0, 0, 0, 0, 0, 0, 8, 40 };

        public DIRControllerTest()
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

            controller = new DIRController(mock.Object, messengerMock.Object);
            controller.ControllerContext = controllerContext.Object;

        }


        #region GetBusinessTripDIR
        [Test]
        public void GetBusinessTripDIR_Null_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = null;

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
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
        public void GetBusinessTripDIR_EmptyString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "";

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
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
        public void GetBusinessTripDIR_SDDDA_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "SDDDA";

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
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

        #region GetBusinessTripDataDIR
        [Test]
        public void GetBusinessTripDataDIR_DepEmptyStringSearchStringEmpty_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment, searchString).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Manowens", result.ToArray()[0].BTof.LastName);
            //Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Manowens");
            //Assert.AreEqual(result.ToArray()[2].BTof.LastName, "Pyorge");
            //Assert.AreEqual(result.ToArray()[2].BusinessTripID, 3);
            //Assert.AreEqual(result.ToArray()[3].BTof.LastName, "Pyorge");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataDIR_Null_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = null;
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment,searchString).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Manowens", result.ToArray()[0].BTof.LastName);
            //Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Manowens");
            //Assert.AreEqual(result.ToArray()[2].BTof.LastName, "Pyorge");
            //Assert.AreEqual(result.ToArray()[2].BusinessTripID, 3);
            //Assert.AreEqual(result.ToArray()[3].BTof.LastName, "Pyorge");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataDIR_SDDDA_EmptySearchString_BusinessTripsOfSDDDA()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment,searchString).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Manowens", result.ToArray()[0].BTof.LastName);
            //Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Pyorge");
            Assert.AreEqual(14, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataDIR_EmptyDepNotEmptySearchString_BusinessTrips()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "Manowens";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment, searchString).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment,searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Manowens", result.ToArray()[0].BTof.LastName);
            //Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Pyorge");
            Assert.AreEqual(14, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataDIR_SDDANotEmptySearchString_BusinessTripsSDDA()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "Manowens";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment, searchString).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment,searchString);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Manowens", result.ToArray()[0].BTof.LastName);
            //Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Pyorge");
            Assert.AreEqual(14, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }
        #endregion

        #region Reject_BT_DIRGet

        [Test]
        public void Reject_BT_DIRGet_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_DIR();
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_DIRGet_ExistingBT_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_DIR(3);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }



        [Test]
        public void Reject_BT_DIRGet_ExistingBTIdAndDefaultJsondataAndEmptyDepartment_CorrectViewLoads()
        {
            //Arrange
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            // Act
            var view = controller.Reject_BT_DIR(3, selectedDepartment: "");
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);

        }

        [Test]
        public void Reject_BT_DIRGet_ExistingBTAndDefaultJsondataAndSDDDA_CorrectViewLoads()
        {
            //Arrange
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 100, 40 } };

            // Act
            var view = controller.Reject_BT_DIR(3, selectedDepartment: "SDDDA");
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreNotEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
            Assert.AreEqual(rowVersion, businessTripAfterCall.RowVersion);
        }



        [Test]
        public void Reject_BT_DIR_JsonDataNotEmpty_ExistingBT()
        {
            //Arrange
            string department = "AA";
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_DIR(3, jsonData, department);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(department, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        [Test]
        public void Reject_BT_DIR_JsonDataContainsWhiteSpace_ExistingBT()
        {
            //Arrange
            string department = "AA";
            byte[] rowVersion = { 0, 0, 0, 0, 0, 2, 159, 230 };
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_DIR(3, jsonData.Replace("+", " "), department);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(department, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        #endregion

        #region Reject_BT_DIR_Confirm

        [Test]
        public void Reject_BT_DIR_Confirm_NotExistingBT_Error404()
        {
            //Arrange
            BusinessTrip businessTrip = null;


            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "");

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_EmptyComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            // Act

            var view = controller.Reject_BT_DIR_Confirm(businessTrip);
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_EmptyCommentSelectedDepartment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion, RejectComment = "", UnitID = 1, Unit = new Unit() };

            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidComment_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);
            businessTrip.DaysInBtForOrder = 9;

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip);

            //Assert 
            Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            Assert.AreEqual("DIRView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
            Assert.AreEqual(null, businessTrip.OrderStartDate);
            Assert.AreEqual(null, businessTrip.OrderEndDate);
            Assert.AreEqual(null, businessTrip.DaysInBtForOrder);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM)         && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Once);

        }

        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidCommentSelectedDepartment_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            Assert.AreEqual("DIRView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("SDDDA", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);


            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Once);

            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_CommentNullSelectedDepartment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 31).FirstOrDefault();
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(31, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidCommentConcurency_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            JsonResult result = (JsonResult)controller.Reject_BT_DIR_Confirm(businessTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);


            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        #endregion

        #region SearchBusinessTripDIR

        [Test]
        public void SearchBusinessTripDIR_BtList()
        {
            //Arrange
            string selectedDepartment = null;
            string searchString = "";

            //Act
            var result = controller.SearchBusinessTripDIR(mock.Object.BusinessTrips.ToList(), searchString, selectedDepartment);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(7, result.ToArray()[0].EmployeeID);
            //Assert.AreEqual(7, result.ToArray()[1].EmployeeID);
            Assert.AreEqual("LDF", result.ToArray()[0].Location.Title);
            //Assert.AreEqual("LDF", result.ToArray()[1].Location.Title);

        }

        [Test]
        public void SearchBusinessTripDIR_NotEmptyDep_BtList()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "";

            //Act
            var result = controller.SearchBusinessTripDIR(mock.Object.BusinessTrips.ToList(), searchString, selectedDepartment);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(7, result.ToArray()[0].EmployeeID);
            Assert.AreEqual("LDF", result.ToArray()[0].Location.Title);

        }

        [Test]
        public void SearchBusinessTripDIR_BadDEpartment_BtList()
        {
            //Arrange
            string selectedDepartment = "SDDDASSD";
            string searchString = "";

            //Act
            var result = controller.SearchBusinessTripDIR(mock.Object.BusinessTrips.ToList(), searchString, selectedDepartment);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SearchBusinessTripDIR_BadSearchstring_BtList()
        {
            //Arrange
            string selectedDepartment = null;
            string searchString = "SAFSDFDS";

            //Act
            var result = controller.SearchBusinessTripDIR(mock.Object.BusinessTrips.ToList(), searchString, selectedDepartment);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion

    }
}