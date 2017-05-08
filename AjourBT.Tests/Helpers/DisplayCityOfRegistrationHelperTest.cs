using System;
using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayCityOfRegistrationHelperTest
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
        public void CustomDisplayCityOfVisaRegistration_NullVisaRegDate_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
          
            //Act
            string result = helper.CustomDisplayCityOfVisaRegistration(null).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayCityOfVisaRegistration_ValidVisaRegDateCityisNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            VisaRegistrationDate visaRegistrationDate = new VisaRegistrationDate();

            //Act
            string result = helper.CustomDisplayCityOfVisaRegistration(visaRegistrationDate).ToString();

            //Assert
            Assert.AreEqual("City is not specified", result);
        }

       
        [Test]
        public void CustomDisplayCityOfVisaRegistration_DefaultCity_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            VisaRegistrationDate visaRegistrationDate = new VisaRegistrationDate() { City = "Kyiv"};

            //Act
            string result = helper.CustomDisplayCityOfVisaRegistration(visaRegistrationDate).ToString();

            //Assert
            Assert.AreEqual("Kyiv", result);
        }

        [Test]
        public void CustomDisplayCityOfVisaRegistration_CityIsNotSpecified_EmptyStrind()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            VisaRegistrationDate visaRegistrationDate = new VisaRegistrationDate() { City = ""};

            //Act
            string result = helper.CustomDisplayCityOfVisaRegistration(visaRegistrationDate).ToString();

            //Assert
            Assert.AreEqual("City is not specified", result);
        }


        [Test]
        public void CustomDisplayCityOfVisaRegistration_CityIsSpecified_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            VisaRegistrationDate visaRegistrationDate = new VisaRegistrationDate() { City = "Kyiv" };

            //Act
            string result = helper.CustomDisplayCityOfVisaRegistration(visaRegistrationDate).ToString();

            //Assert
            Assert.AreEqual("Kyiv", result);
        }
    }
}
