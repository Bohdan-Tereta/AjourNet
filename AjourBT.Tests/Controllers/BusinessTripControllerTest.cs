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
    public class BusinessTripControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;
        BusinessTripController controller;

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

            controller = new BusinessTripController(mock.Object, messengerMock.Object);
            controller.ControllerContext = controllerContext.Object;

        }

        #region ADM,BTM,ACC

        #region ShowBTData

        [Test]
        public void ShowBTData_NotExistingBT_HttpNotFound()
        {
            //Arrange

            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowBTData(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowBTData_ExistingBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.ShowBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowBTData_ExistingBusinessTripID_ShowBTDataView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = (ViewResult)controller.ShowBTData(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(12, resultModel.BusinessTripID);
        }

        [Test]
        public void ShowBTData_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowBTData(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }
        #endregion

        #endregion

       

        

    }
}