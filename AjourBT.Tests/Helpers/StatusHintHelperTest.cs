using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AjourBT.Helpers;
using AjourBT.Domain.Entities;
using System.Web.Routing;
using Moq;
using System.Security.Principal;
using AjourBT.Controllers;
using AjourBT.Infrastructure;
using AjourBT.Domain.Abstract;
using System.IO;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class StatusHintHelperTest
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

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();            

            List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            { 
                new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null,  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="meeting", Manager = "xopu", Responsible = "xopu",  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment="Visa Expired",  Status = BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Reported, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment=null,  Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Confirmed, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 },
            };
           
            mock.Setup(m => m.BusinessTrips).Returns(businessTrips);

        }



        [Test]
        public void CustomStatusHint_PlannedBTEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_NullFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }



        [Test]
        public void CustomStatusHint_PlannedBTEmptyPurpose_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "", Manager = "xopu", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_EmptyManager_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBT_EmptyResponsible_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = "", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBTNullablePurpose_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = null, Manager = "xopu", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_NullableManager_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = null, Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBT_NullableResponsible_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = null, Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("Not enough info for Registration.", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("", result);
        }



        [Test]
        public void CustomStatusHint_RegisteredModifiedBTRejectCommentEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            bt.Status = bt.Status | BTStatus.Registered;

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was Replanned!\r\n", result);
        }

        [Test]
        public void CustomStatusHint_ConfirmedBTRejectCommentEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            bt.Status = bt.Status | BTStatus.Confirmed;

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT is Confirmed. Contact BTM to cancel BT", result);
        }

        [Test]
        public void CustomStatusHint_ConfirmedModifiedBTRejectCommentEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            bt.Status = bt.Status | BTStatus.Confirmed;

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT is Modified. Contact BTM to cancel BT", result);
        }

        [Test]
        public void CustomStatusHint_PlannedModifiedBTRegectCommentNullFieldsAndNotEnoughInfo_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            bt.Status = bt.Status | BTStatus.Planned;

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was Replanned!\r\nNot enough info for Registration.", result);
        }

        [Test]
        public void CustomStatusHint_PlannedModifiedBTRegectCommentNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            bt.Status = bt.Status | BTStatus.Planned;
            bt.Manager = "xomi";
            bt.Responsible = "xomi";
            bt.Purpose = "meeting";

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was Replanned!\r\n", result);
        }

        [Test]
        public void CustomStatusHint_ModifiedBTRegectCommentIsNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 6).FirstOrDefault();
            
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was Rejected!", result);
        }

        [Test]
        public void CustomStatusHint_ReportedBTIsNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 7).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT is Reported \n (Contact ACC to cancel BT)", result);
        }

        [Test]
        public void CustomStatusHint_RegisteredBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 8).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomStatusHint_CancelledBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was cancelled!\r\n", result);
        }


        [Test]
        public void CustomStatusHint_ModifiedCancelledBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("BT was first Modified then cancelled!\r\n", result);
        }

        [Test]
        public void CustomStatusHint_ModifiedBTRejectCommentEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();

            //Act
            string result = helper.CustomStatusHint(bt);

            //Assert
            Assert.AreEqual("", result);
        }
    }
}
