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

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomDisplayBTsDatesActionLink
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


        [Test]
        public void CustomDisplayBTsDatesActionLink_BTisNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayBTsDatesActionLink(null, "").ToString();

            //Assert
            Assert.AreEqual("", result);
        }


        [Test]
        public void CustomDisplayBTsDatesActionLink_BTsOrderStartDateAndOrderEndDate_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip {BusinessTripID = 1, OrderStartDate = new DateTime(2013, 01, 01), OrderEndDate = new DateTime(2014, 01, 01), DaysInBtForOrder = 100 };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"EditReportedBTACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/EditReportedBT/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(), bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, bTrip.DaysInBtForOrder);
            //Act
            string result = helper.CustomDisplayBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }


        [Test]
        public void CustomDisplayBTsDatesActionLink_BTsOrdersDateNull_Correct_String()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip {BusinessTripID = 1, StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 01, 01)};
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"EditReportedBTACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/ACC/EditReportedBT/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), dateFormat);
            //Act
            string result = helper.CustomDisplayBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }


        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTisNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(null, "").ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTsOrderStartDateAndOrderEndDate_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, OrderStartDate = new DateTime(2013, 01, 01), OrderEndDate = new DateTime(2014, 01, 01), DaysInBtForOrder = 100 };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/ShowAccountableBTData/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(), bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, bTrip.DaysInBtForOrder);
            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTsOrdersDateNull_Correct_String()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 01, 01) };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/ACC/ShowAccountableBTData/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), dateFormat);
            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTsOrderStartEndDate_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, OrderStartDate = new DateTime(2013, 01, 01), OrderEndDate = new DateTime(2014, 01, 01), DaysInBtForOrder = 100 };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/ShowAccountableBTData/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(), bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, bTrip.DaysInBtForOrder);
            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTsOrderEndDate_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, OrderStartDate = new DateTime(2013, 01, 01), OrderEndDate = new DateTime(2014, 01, 01), DaysInBtForOrder = 100 };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> <blue><b>{5} - {6}</b></blue> <orange>{8}</orange> &nbsp; &nbsp; {3} - {4} </a>", "/ACC/ShowAccountableBTData/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(), bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat, bTrip.DaysInBtForOrder);
            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BTsOrdersDateIsNull_Correct_String()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 01, 01) };
            string selectedDepartment = "";
            string dateFormat = MvcApplication.JSDatePattern;
            string resFormat = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/ACC/ShowAccountableBTData/", bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(), bTrip.EndDate.ToShortDateString(), dateFormat);
            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip, "").ToString();

            //Assert
            Assert.AreEqual(resFormat, result);
        }

    }
}
