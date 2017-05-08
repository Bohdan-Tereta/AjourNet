using System;
using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Web.Mvc;
using AjourBT.Helpers;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayMontheHelprTest
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
        public void CustomDisplayMonthInEnglishHelp_EmptyString_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_Null_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = null;

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_1_Jan()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "1";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Jan";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_2_Feb()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "2";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Feb";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_3_Mar()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "3";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Mar";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_4_Apr()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "4";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Apr";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_5_May()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "5";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "May";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_6_Jun()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "6";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Jun";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_7_Jul()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "7";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Jul";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_8_Aug()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "8";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Aug";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_9_Sep()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "9";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Sep";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_10_Oct()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "10";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Oct";

            //Assert 
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDisplayMonthInEnglishHelp_11_Nov()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "11";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Nov";

            //Assert 
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CustomDisplayMonthInEnglishHelp_12_Dec()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string data = "12";

            //Act
            string result = (helper.CustomDisplayMonthInEnglishHelp(data)).ToString();
            string expected = "Dec";

            //Assert 
            Assert.AreEqual(expected, result);
        }

    }
}
