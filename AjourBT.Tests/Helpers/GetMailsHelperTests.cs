using AjourBT.Domain.Abstract;
using NUnit.Framework;
using System;
using AjourBT.Domain.Entities;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AjourBT.Helpers;
using AjourBT.Domain.Infrastructure;
using AjourBT.Tests.MockRepository;
using System.Web.Routing;
using System.Security.Principal;
using AjourBT.Controllers;
using System.IO;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class GetMailsHelperTests
    {
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
    
       Mock<IRepository> mock;
   
       Mock<IMessenger> messengerMock;
       Mock<ControllerContext> controllerContext;
     
        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();
           
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
              
        }

        #region GetSecondMailAliasEMails
        [Test]
        public void GetSecondMailAliasEMails_SDDDA_SDDDAEmployeesSecondMailsAndMailsIfSecondMailsNotExist()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            mock.Object.Employees.Where(e => e.EID == "andl").FirstOrDefault().EMail = "abc@gmail.com";
            List<Employee> emps =  new List<Employee>();
            emps.AddRange(mock.Object.Employees.Where(e=>e.DepartmentID==1).ToList());
            //Act
            var result = helper.GetSecondMailAliasEMailsHelper(emps);
            
            //Assert
            Assert.AreEqual("<textarea rows=\"10\" cols=\"45\" name=\"text\">abc@gmail.com, abc, xomi@elegant.com, chap@elegant.com, ivte@elegant.com</textarea>", result.ToString());
          }

   
        [Test]
        public void GetSecondMailAliasEMails_ForSDDDAEmployeesWithCurrentUser_SecondMails()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var mock = Mock_Repository.CreateMock();
            controllerContext = new Mock<ControllerContext>();
            messengerMock = new Mock<IMessenger>();
            messengerMock.Setup(m => m.Notify(It.IsAny<IMessage>())).Verifiable();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
        
            List<Employee> emps = new List<Employee>();
            emps.AddRange(mock.Object.Employees.Where(e => e.DepartmentID == 1).ToList());
       
            //Act
            var result = helper.GetSecondMailAliasEMailsHelper(emps, "rkni");
            
            //Assert
            Assert.AreEqual("mailto:rkni@elegant.com?bcc=Anl@E-mail.ua, abc, xomi@elegant.com, chap@elegant.com, ivte@elegant.com", result.ToString());  
        }

        [Test]
        public void GetSecondMailAliasEMails_ForOneEmployeeInDeptCurrentUser_SecondMails()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var mock = Mock_Repository.CreateMock();
            controllerContext = new Mock<ControllerContext>();
            messengerMock = new Mock<IMessenger>();
            messengerMock.Setup(m => m.Notify(It.IsAny<IMessage>())).Verifiable();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            List<Employee> emps = new List<Employee>();

            //Act
            var result = helper.GetSecondMailAliasEMailsHelper(emps, "rkni");

            //Assert
            Assert.AreEqual("mailto:rkni@elegant.com?bcc=", result.ToString());
        }
        #endregion


        #region GetMailAliasEMails
        [Test]
        public void GetMailAliasEMails_ForOneEmployeeInDeptCurrentUser()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var mock = Mock_Repository.CreateMock();
            controllerContext = new Mock<ControllerContext>();
            messengerMock = new Mock<IMessenger>();
            messengerMock.Setup(m => m.Notify(It.IsAny<IMessage>())).Verifiable();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            List<Employee> emps = new List<Employee>();

            //Act
            var result = helper.GetMailAliasEMailsHelper(emps, "rkni");

            //Assert
            Assert.AreEqual("mailto:rkni@elegant.com?bcc=", result.ToString());
        }

        [Test]
        public void GetMailAliasEMails_ForSDDDAEmployeesWithCurrentUser()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var mock = Mock_Repository.CreateMock();
            controllerContext = new Mock<ControllerContext>();
            messengerMock = new Mock<IMessenger>();
            messengerMock.Setup(m => m.Notify(It.IsAny<IMessage>())).Verifiable();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("rkni");
            controllerContext.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            List<Employee> emps = new List<Employee>();
            emps.AddRange(mock.Object.Employees.Where(e => e.DepartmentID == 1).ToList());

            //Act
            var result = helper.GetMailAliasEMailsHelper(emps, "rkni");

            //Assert
            Assert.AreEqual("mailto:rkni@elegant.com?bcc=andl@elegant.com, tedk@elegant.com, xomi@elegant.com, chap@elegant.com, ivte@elegant.com", result.ToString());
        }
   
        [Test]
        public void GetMailAliasEMails_SDDDA_SDDDAEmployeesMails()
        {
            //Arrange  
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            mock.Object.Employees.Where(e => e.EID == "andl").FirstOrDefault().EMail = "abc@gmail.com";
            List<Employee> emps = new List<Employee>();
            emps.AddRange(mock.Object.Employees.Where(e => e.DepartmentID == 1).ToList());
            //Act
            var result = helper.GetMailAliasEMailsHelper(emps);

            //Assert
            Assert.AreEqual("<textarea rows=\"10\" cols=\"45\" name=\"text\">andl@elegant.com, tedk@elegant.com, xomi@elegant.com, chap@elegant.com, ivte@elegant.com</textarea>", result.ToString());
        }

        #endregion

        #region RemoveEndingComa
        [Test]
        public void RemoveEndingComa_EmptyStringBuilder_EmptyStringBuilder()
        {
            //Arrange
            StringBuilder sb = new StringBuilder();
            //Act
            GetMailsHelper.RemoveEndingComa(sb);
            //Assert
            Assert.AreEqual(0, sb.Length);
        }

        [Test]
        public void RemoveEndingComa_NotEmptyStringBuilder_EmptyStringBuilderWithoutLastTwoSymbols()
        {
            //Arrange
            StringBuilder sb = new StringBuilder();
            sb.Append("asdasd, ");
            //Act
            GetMailsHelper.RemoveEndingComa(sb);
            //Assert
            Assert.AreEqual(6, sb.Length);
            Assert.AreEqual("asdasd", sb.ToString());
        }
        #endregion
    }
}
