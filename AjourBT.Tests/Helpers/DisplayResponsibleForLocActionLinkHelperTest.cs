using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using AjourBT.Tests.MockRepository;
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
    public class DisplayResponsibleForLocActionLinkHelperTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
        }

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
        public void DisplayResponsibleForLocActionLink_LocationNull_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomResponsibleForLocActionLink(null);
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayResponsibleForLocActionLink_ResponsibleForLocNull_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Location location = mock.Object.Locations.Where(v => v.LocationID == 1).FirstOrDefault();
            
            //Act
            MvcHtmlString result = helper.CustomResponsibleForLocActionLink(location);
            MvcHtmlString expected = new MvcHtmlString("-");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayResponsibleForLocActionLink_ResponsibleForLocEmptyString_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Location location = mock.Object.Locations.Where(v => v.LocationID == 1).FirstOrDefault();
            location.ResponsibleForLoc = "";

            //Act
            MvcHtmlString result = helper.CustomResponsibleForLocActionLink(location);
            MvcHtmlString expected = new MvcHtmlString("-");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayResponsibleForLocActionLink_ResponsibleForLocNotEmptyString_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Location location = mock.Object.Locations.Where(v => v.LocationID == 1).FirstOrDefault();
            location.ResponsibleForLoc = "daol";

            //Act
            MvcHtmlString result = helper.CustomResponsibleForLocActionLink(location);
            MvcHtmlString expected = new MvcHtmlString("daol");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

    }
}
