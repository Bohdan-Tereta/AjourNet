using System;
using NUnit.Framework;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Controllers;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Domain.Concrete;
using System.Web;
using System.Security.Principal;
using System.Web.Routing;
using System.IO;
using System.Web.Mvc;
using PagedList;
using AjourBT.Infrastructure;
using AjourBT.Tests.MockRepository;


namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class MessageControllerTest
    {

        Mock<IRepository> mock;
        MessageController messageController;
        Mock<ControllerContext> controllerContext;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();

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

            messageController = new MessageController(mock.Object);
            messageController.ControllerContext = controllerContext.Object;

             IList<IMessage> messages = new List<IMessage>
                {
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault()),
                    new Message(MessageType.ACCModifiesConfirmedReportedToDIR,   mock.Object.BusinessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(),  mock.Object.Employees.FirstOrDefault())

                };
           
            mock.Setup(m => m.Messages).Returns(messages);
       
            }

        #region GetMessagesForRole

        [Test]
        public void GetMessagesForRole_AllParamsSet_AllNotNull()
        {
            //Arrange
            mock = new Mock<IRepository>();
            MessageController messageController = new MessageController(mock.Object);
            //Act
           ViewResult result = messageController.GetMessagesForRole(1,"BTM","BTMView","", 2) as ViewResult;
            //Assert
            Assert.IsTrue(result.ViewBag.SearchString == "");
            Assert.IsTrue(result.ViewBag.Role == "BTM");
            Assert.IsTrue(result.ViewBag.Tab == 1);
            Assert.IsTrue(result.ViewBag.ActionNameForPagination == "BTMView");
            Assert.IsTrue(result.ViewBag.Page == 2); 

        }
        [Test]
        public void GetMessagesForRole_NullableParamsNull_SomeParamsNull()
        {
            //Arrange
            mock = new Mock<IRepository>();
            MessageController messageController = new MessageController(mock.Object);
            //Act
            ViewResult result = messageController.GetMessagesForRole(1, null, null, "search") as ViewResult;
            //Assert
            Assert.IsTrue(result.ViewBag.SearchString == "search");
            Assert.IsTrue(result.ViewBag.Role == null);
            Assert.IsTrue(result.ViewBag.Tab == 1);
            Assert.IsTrue(result.ViewBag.ActionNameForPagination == null);
            Assert.IsTrue(result.ViewBag.Page == 1); 
         }


        #endregion

        #region GetMessagesDataForRole
        [Test]
        public void GetMessagesDataForRole_RoleBTMPage1SearchStringEmpty_ValidViewAllMessages()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(1,"BTM", "BTMView", "") as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("BTMView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(true, result.ViewBag.ShowLinks);
            Assert.AreEqual(1, result.ViewBag.TabID);
            Assert.AreEqual("BTM", result.ViewBag.Role);
            Assert.AreEqual(7, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
            
        }

        [Test]
        public void GetMessagesDataForRole_RolePUPage1SearchStringEmpty_ValidViewAllMessages()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = mock.Object.Messages.OrderByDescending(m => m.TimeStamp).ToList().ToPagedList(1, 4);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(4,"PU", "PUView","") as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual(onePageOfMessages, result.ViewBag.onePageOfMessages);
            Assert.AreEqual("PUView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(false, result.ViewBag.ShowLinks);
            Assert.AreEqual(4, result.ViewBag.TabID);
            Assert.AreEqual("PU", result.ViewBag.Role);
            Assert.AreEqual(8,((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);

        }

        [Test]
        public void GetMessagesDataForRole_RoleNullPage1SearchStringNotNull_HttpNotFound()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = mock.Object.Messages.OrderByDescending(m => m.TimeStamp).ToList().ToPagedList(1, 5);

            //Act
            HttpNotFoundResult result = messageController.GetMessagesDataForRole(1, null, null, "A") as HttpNotFoundResult;

            //Assert        
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual("Wrong Role", ((HttpNotFoundResult)result).StatusDescription);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void GetMessagesDataForRole_RoleBTMPage0SearchStringNotNull_HttpNotFound()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = mock.Object.Messages.OrderByDescending(m => m.TimeStamp).ToList().ToPagedList(1, 5);

            //Act
            HttpNotFoundResult result = messageController.GetMessagesDataForRole(3,"BTM", "BTMView", "B", 0) as HttpNotFoundResult;

            //Assert        
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual("Wrong Page", ((HttpNotFoundResult)result).StatusDescription);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void GetMessagesDataForRole_RoleNotExistsPage0SearchStringNotNull_HttpNotFound()
        {

            //Arrange
            //mock = new Mock<IRepository>();
            //MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = mock.Object.Messages.OrderByDescending(m => m.TimeStamp).ToList().ToPagedList(1, 5);

            //Act
            HttpNotFoundResult result = messageController.GetMessagesDataForRole(3,"", "BTMView", " c ", 0) as HttpNotFoundResult;

            //Assert        
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual("Wrong Role and Page", ((HttpNotFoundResult)result).StatusDescription);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void GetMessagesDataForRole_RoleBTMPageTooBigSearchStringNotNull_EmptyView()
        {

            //Arrange
            //mock = new Mock<IRepository>();
            //MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3,"BTM", "BTMView", " d ", 15555) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual(onePageOfMessages, result.ViewBag.onePageOfMessages);
            Assert.AreEqual("BTMView", result.ViewBag.actionNameForPagination);
        }

        [Test]
        public void GetMessagesDataForRole_RoleBTMPage1gSearchStringtNull_EmptyView()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3, "BTM", "BTMView", null, 1) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("BTMView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(true, result.ViewBag.ShowLinks);
            Assert.AreEqual(3, result.ViewBag.TabID);
            Assert.AreEqual("BTM", result.ViewBag.Role);
            Assert.AreEqual(7, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
        }

        [Test]
        public void GetMessagesDataForRole_RoleBTMPage1gSearchStringNotNull_MessagesForBTM()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3, "BTM", "BTMView", "ADMConfirms", 1) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("BTMView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(true, result.ViewBag.ShowLinks);
            Assert.AreEqual(3, result.ViewBag.TabID);
            Assert.AreEqual("BTM", result.ViewBag.Role);
            Assert.AreEqual(7, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
        }

        [Test]
        public void GetMessagesDataForRole_RolePUPage1gSearchStringNotNull_MessagesForPU()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3, "PU", "PUView", "ACCModifies", 1) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("PUView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(false, result.ViewBag.ShowLinks);
            Assert.AreEqual(3, result.ViewBag.TabID);
            Assert.AreEqual("PU", result.ViewBag.Role);
            Assert.AreEqual(1, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
        }

        [Test]
        public void GetMessagesDataForRole_RoleBTMPage1gSearchStringNotNull_EmptyView()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3, "BTM", "BTMView", "Akjhuhfnfdoewfnpor", 1) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("BTMView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(true, result.ViewBag.ShowLinks);
            Assert.AreEqual(3, result.ViewBag.TabID);
            Assert.AreEqual("BTM", result.ViewBag.Role);
            Assert.AreEqual(0, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
        }

        [Test]
        public void GetMessagesDataForRole_RolePUPage1gSearchStringNotNull_EmptyView()
        {

            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            PagedList.IPagedList<IMessage> onePageOfMessages = new List<IMessage>().ToPagedList(1, 5);

            //Act
            ViewResult result = messageController.GetMessagesDataForRole(3, "PU", "PUView", "jbhnpjigbhreuigfhreuihregfuh", 1) as ViewResult;

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual("PUView", result.ViewBag.actionNameForPagination);
            Assert.AreEqual(false, result.ViewBag.ShowLinks);
            Assert.AreEqual(3, result.ViewBag.TabID);
            Assert.AreEqual("PU", result.ViewBag.Role);
            Assert.AreEqual(0, ((PagedList.IPagedList<IMessage>)result.ViewBag.OnePageOfMessages).TotalItemCount);
        }

        #endregion

        #region SearchMessageData

        [Test]
        public void SearchPUMessageDataMessagesNullSearchStringNull_EmptyList()
        {
            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            IList<IMessage> messageList = new List<IMessage>();
            IList<IMessage> resList = new List<IMessage>();
            string searchString = null;

            //Act
            resList = messageController.SearchPUMessageData(messageList, searchString);

            //Assert
            Assert.IsEmpty(messageList);
            Assert.IsEmpty(resList);
        }

        [Test]
        public void SearchPUMessageDataMessagesNullsearchStringNotEmpty_EmptyList()
        {
            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            IList<IMessage> messageList = new List<IMessage>();
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "Johnny";

            //Act
            resList = messageController.SearchPUMessageData(messageList, searchString);

            //Assert
            Assert.IsEmpty(messageList);
            Assert.IsEmpty(resList);
        }

        [Test]
        public void SearchPUMessageDataMessageNotNullSearchStringEmpty_NotEmptyList()
        {
            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            IList<IMessage> messageList = new List<IMessage>(mock.Object.Messages);
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "";

            //Act
            resList = messageController.SearchPUMessageData(messageList, searchString);

            //Assert
            Assert.AreEqual(mock.Object.Messages.Count(), messageList.Count());
        }


        [Test]
        public void SearchPUmessageNotNullSearchStringNotEmpty_NotEmptyList()
        {
            //Arrange
            MessageController messageController = new MessageController(mock.Object);
            IList<IMessage> messageList = new List<IMessage>(mock.Object.Messages);
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "JOHNNY";

            //Act
            resList = messageController.SearchPUMessageData(messageList, searchString);

            //Assert
            Assert.AreEqual(8, messageList.Count());
        }


        [Test]
        public void SearchOtherRoleMessageListNullSearchStringNullRoleNull_EmptyList()
        {
            //Arrange
            MessageController msgController = new MessageController(mock.Object);
            IList<IMessage> msgList = new List<IMessage>();
            IList<IMessage> resList = new List<IMessage>();
            string searchString = null;
            string role = null;

            //Act
            resList = msgController.SearchOtherRoleMessageData(msgList, searchString,role);

            //Assert
            Assert.IsEmpty(msgList);
            Assert.IsEmpty(resList);
        }

        [Test]
        public void SearchOtherRoleMessageListNullSearchStringNotEmptyRoleNotEmpty_EmptyList()
        {
            //Arrange
            MessageController msgController = new MessageController(mock.Object);
            IList<IMessage> msgList = new List<IMessage>();
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "Johnny";
            string role = "ADM";

            //Act
            resList = msgController.SearchOtherRoleMessageData(msgList, searchString, role);

            //Assert

            Assert.IsEmpty(resList);
        }

        [Test]
        public void SearchOtherRoleMessageListNotNullSearchStringEmpty_NotEmptyList()
        {
            //Arrange
            MessageController msgController = new MessageController(mock.Object);
            IList<IMessage> msgList = new List<IMessage>(mock.Object.Messages);
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "";
            string role = "BTM";

            //Act
            resList = msgController.SearchOtherRoleMessageData(msgList, searchString, role);

            //Assert
            Assert.AreEqual(7, resList.Count());
        }

        [Test]
        public void SearchOtherRoleMessageListNotNullSearchStringNotEmpty_NotEmptyList()
        {
            //Arrange
            MessageController msgController = new MessageController(mock.Object);
            IList<IMessage> msgList = new List<IMessage>(mock.Object.Messages);
            IList<IMessage> resList = new List<IMessage>();
            string searchString = "john";
            string role = "BTM";

            //Act
            resList = msgController.SearchOtherRoleMessageData(msgList, searchString, role);

            //Assert
            Assert.AreEqual(0, resList.Count());
        }
        #endregion

    }
}
