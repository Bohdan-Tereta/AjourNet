using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayLastCRUDHelperTest
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
        public void DisplayLastCRUD_EID_equals_to_null_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.DisplayLastCRUD(null, new DateTime()).ToString();

            //Assert   
            Assert.AreEqual(result, "");            
        }

        [Test]
        public void DisplayLastCRUD_EID_is_valid_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.DisplayLastCRUD("ncru", new DateTime(2013,10,30)).ToString();

            //Assert   
            Assert.AreEqual("Last modified by ncru on 30.10.2013 at 00:00:00", result);
        }
    }
}
