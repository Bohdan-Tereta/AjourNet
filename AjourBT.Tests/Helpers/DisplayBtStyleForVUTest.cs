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
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Entities;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayBtStyleForVUTest
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

        ViewContext viewContext;

        [SetUp]
        public void SetUpFixture()
        {
            viewContext = new ViewContext();
            viewContext.HttpContext = new FakeHttpContext();
        }


        #region CustomBtModelStyle

        [Test]
        public void CustomBtModelStyle_CancelledModifiedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Modified, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_ModifiedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status =  BTStatus.Confirmed | BTStatus.Modified, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "color: blue;";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_CancelledRegisteredBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Registered, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_CancelledPlanneddBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Planned, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_CancelledConfirmedModifiedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Modified | BTStatus.Confirmed, UnitID = 1, Unit = new Unit()};
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_CancelledBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Confirmed, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "color: red; text-decoration: line-through;";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_ConfirmedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Confirmed, UnitID = 1, Unit = new Unit() };
            BusinessTripViewModel btModel = new BusinessTripViewModel(businessTrip);

            //Act
            string result = helper.CustomBtModelStyle(btModel);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtModelStyle_NullBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());

            //Act
            string result = helper.CustomBtModelStyle(null);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        #endregion



        #region CustomBtStyle

        [Test]
        public void CustomBtStyle_CancelledModifiedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Modified };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_CancelledRegisteredBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Registered };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_CancelledPlanneddBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Planned };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_CancelledConfirmedModifiedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Modified | BTStatus.Confirmed };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_CancelledBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Cancelled | BTStatus.Confirmed };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "color: red; text-decoration: line-through;";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_ConfirmedBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 1, Status = BTStatus.Confirmed };

            //Act
            string result = helper.CustomBtStyle(businessTrip);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CustomBtStyle_NullBt_RedAndStrikeStyle()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());

            //Act
            string result = helper.CustomBtStyle(null);
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        #endregion
    }
}
