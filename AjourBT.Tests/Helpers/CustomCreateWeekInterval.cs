using AjourBT.Domain.Abstract;
using AjourBT.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomCreateWeekInterval
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
        public void CustomWeekInterval_from2000_to2013()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            List<Interval> result = helper.CustomCreateWeekInterval(2000, 2013, 12, 46);

            //Assert
            Assert.AreEqual(14, result.Count());
            Assert.AreEqual(12, result.ToArray()[0].weekFrom);
            Assert.AreEqual(52, result.ToArray()[0].weekTo);
            Assert.AreEqual(2000, result.ToArray()[0].year);
            Assert.AreEqual(1, result.ToArray()[6].weekFrom);
            Assert.AreEqual(52, result.ToArray()[6].weekTo);
            Assert.AreEqual(2006, result.ToArray()[6].year);
            Assert.AreEqual(1, result.ToArray()[13].weekFrom);
            Assert.AreEqual(46, result.ToArray()[13].weekTo);
            Assert.AreEqual(2013, result.ToArray()[13].year);
        }

        [Test]
        public void CustomInterval_from2013_to2013()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            List<Interval> result = helper.CustomCreateWeekInterval(2013, 2013, 12, 46);

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(12, result.ToArray()[0].weekFrom);
            Assert.AreEqual(46, result.ToArray()[0].weekTo);
            Assert.AreEqual(2013, result.ToArray()[0].year);
            
        }
    }
}
