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


namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayDaysInBTHelperTest
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
        public void CustomDisplayNumberOfDaysInBT_NullEmployee_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayNumberOfDaysInBT(null).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayNumberOfDaysInBT_NullEmployee_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayNumberOfDaysInBT(null).ToString();

            //Assert
            Assert.AreEqual(String.Empty, result);
        }

        [Test]
        public void CustomDisplayNumberOfDaysInBT_CorrectEmployeeNoDaysBt_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            EmployeeViewModelForVU emp = new EmployeeViewModelForVU();
            //Act
            string result = helper.CustomDisplayNumberOfDaysInBT(emp).ToString();

            //Assert
            Assert.AreEqual("No BT during this period", result);
        }

        [Test]
        public void CustomDisplayNumberOfDaysInBT_DefaultNumberDaysInBt_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            EmployeeViewModelForVU emp = new EmployeeViewModelForVU { DaysUsedInBt = 0};

            //Act
            string result = helper.CustomDisplayNumberOfDaysInBT(emp).ToString();

            //Assert
            Assert.AreEqual("No BT during this period", result);
        } 


        [Test]
        public void CustomDisplayNumberOfDaysInBT_CorrectNumberDaysInBt_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).LastOrDefault();
            EmployeeViewModelForVU emp = new EmployeeViewModelForVU { DaysUsedInBt = 3 };
          
            //Act
            string result = helper.CustomDisplayNumberOfDaysInBT(emp).ToString();

            //Assert
            Assert.AreEqual("3" + " days in BT", result);
        }
    }
}
