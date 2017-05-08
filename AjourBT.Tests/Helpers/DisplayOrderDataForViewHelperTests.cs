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

namespace AjourBT.Tests.Helpers
{
        [TestFixture]
    public class DisplayOrderDataForViewHelperTests
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
        public void CustomDisplayOrderStartDate_StringIsNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderStartDate = null;
            //Act
            string result = helper.CustomDisplayOrderStartDate(orderStartDate).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayOrderStartDate_StringIsNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderStartDate = "";
            //Act
            string result = helper.CustomDisplayOrderStartDate(orderStartDate).ToString();

            //Assert
            Assert.AreEqual("<b>Order From</b> ", result);
        }

        [Test]
        public void CustomDisplayOrderStartDate_StringNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderStartDate = "01.01.2008";
            //Act
            string result = helper.CustomDisplayOrderStartDate(orderStartDate).ToString();

            //Assert
            Assert.AreEqual("<b>Order From</b> 01.01.2008", result);
        }


        [Test]
        public void CustomDisplayOrderEndDate_StringIsNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderEndDate = null;
            //Act
            string result = helper.CustomDisplayOrderEndDate(orderEndDate).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayOrderEndDate_StringIsNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderEndDate = "";
            //Act
            string result = helper.CustomDisplayOrderEndDate(orderEndDate).ToString();

            //Assert
            Assert.AreEqual("<b>Order To &nbsp &nbsp  </b>", result);
        }

        [Test]
        public void CustomDisplayOrderEndDate_StringNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string orderEndDate = "01.01.2008";
            //Act
            string result = helper.CustomDisplayOrderEndDate(orderEndDate).ToString();

            //Assert
            Assert.AreEqual("<b>Order To &nbsp &nbsp  </b>01.01.2008", result);
        }

        [Test]
        public void CustomDisplayOrderDates_IsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            int? orderDates = null;
            //Act
            string result = helper.CustomDisplayOrderDays(orderDates).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayOrderDates_DefaultValue_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            int? orderDates =0;
            //Act
            string result = helper.CustomDisplayOrderDays(orderDates).ToString();

            //Assert
            Assert.AreEqual("<b>Number Of Days</b> 0", result);
        }

        [Test]
        public void CustomDisplayOrderEndDate_CorrectValue_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            int? orderDates = 10;
            //Act
            string result = helper.CustomDisplayOrderDays(orderDates).ToString();

            //Assert
            Assert.AreEqual("<b>Number Of Days</b> 10", result);
        }
    }
}
