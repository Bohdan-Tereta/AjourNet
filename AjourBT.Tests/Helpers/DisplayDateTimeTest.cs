using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AjourBT.Helpers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayDateTimeTest
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
        public void DisplayDateTime_CorrectDate()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayDateTime(new DateTime(2014, 02, 06)).ToString();
            string resFormat = String.Format("{0:dd'.'MM'.'yyyy}", new DateTime(2014, 02, 06));

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void DisplayDateTime_nullDate()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            DateTime? date = null;

            //Act
            string result = helper.CustomDisplayDateTime(date).ToString();
            string resFormat = String.Empty;

            //Assert
            Assert.AreEqual(resFormat, result);
        }

        [Test]
        public void DisplayDateTime_notnullDate()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            DateTime? date = new DateTime(2014,02,06);

            //Act
            string result = helper.CustomDisplayDateTime(date).ToString();
            string resFormat = String.Format("{0:dd'.'MM'.'yyyy}", date);

            //Assert
            Assert.AreEqual(resFormat, result);
        }
    }
}
