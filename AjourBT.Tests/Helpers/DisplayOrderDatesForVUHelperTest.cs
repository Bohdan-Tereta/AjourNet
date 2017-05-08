using System;
using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using AjourBT.Domain.Abstract;
using System.Collections;
using System.Web.Mvc;
using AjourBT.Domain.Entities;
using Moq;
using System.Linq;
using AjourBT.Helpers;
using AjourBT.Domain.ViewModels;
using AjourBT.Infrastructure;
using AjourBT.Tests.MockRepository;


namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayOrderDatesForVUHelperTest
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
            mock = Mock_Repository.CreateMock();
        
        }

        [Test]
        public void CustomDisplayOrderDatesForVU_BTIsNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayOrderDatesForVU(null).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayOrderDatesForVU_BTIsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayOrderDatesForVU(null).ToString();

            //Assert
            Assert.AreEqual(String.Empty, result);
        }

        [Test]
        public void CustomDisplayOrderDatesForVU_CorrectBTNoOrderDates_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip();
            //Act
            string result = helper.CustomDisplayOrderDatesForVU(bt).ToString();

            //Assert
            Assert.AreEqual("Order:  - ", result);
        }

        [Test]
        public void CustomDisplayOrderDatesForVU_DefaultOrderDateBt_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            string result = helper.CustomDisplayOrderDatesForVU(bt).ToString();

            //Assert
            Assert.AreEqual("Order:  - ", result);
        }


        [Test]
        public void CustomDisplayOrderDatesForVU_CorrectOrderDatesBt_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
           
            //Act
            string result = helper.CustomDisplayOrderDatesForVU(bt).ToString();

            //Assert
            Assert.AreEqual("Order: 30.11.2014 - 11.12.2014", result);
        }

    }
}
