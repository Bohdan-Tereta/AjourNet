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
using System.Web.Routing;
using Moq;
using System.Security.Principal;
using AjourBT.Controllers;
using AjourBT.Infrastructure;
using AjourBT.Domain.Abstract;
using System.IO;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class JourneyDateStatusTest
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
            mock = new Mock<IRepository>();

            List<Journey> journeys = new List<Journey> 
            { 
                new Journey{JourneyID = 1, BusinessTripID = 1, Date = new DateTime(2014,01,14), DayOff = true },
                new Journey{JourneyID = 2, BusinessTripID = 1, Date = new DateTime(2014,01,14), DayOff = true, ReclaimDate = new DateTime(2014,01,20) },
                new Journey{JourneyID = 3, BusinessTripID = 1, Date = new DateTime(2014,01,14), DayOff = false}
            };

            mock.Setup(m => m.Journeys).Returns(journeys);

        }

        [Test]
        public void CustomDisplayJourneyDate_ReclaimDateNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Journey journey = mock.Object.Journeys.Where(j => j.JourneyID == 1).FirstOrDefault();
            //Act
            MvcHtmlString result = helper.CustomDisplayJourneyDate(journey);
            MvcHtmlString expected = new MvcHtmlString("<a id=journeyLink href=/Journey/EditJourney/1?searchString= </a> 14.01.2014 ");
            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayJourneyDate_ReclaimDateNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Journey journey = mock.Object.Journeys.Where(j => j.JourneyID == 2).FirstOrDefault();
            //Act
            MvcHtmlString result = helper.CustomDisplayJourneyDate(journey);
            MvcHtmlString expected = new MvcHtmlString("<a id=journeyLink href=/Journey/EditJourney/2?searchString= <strike style=\"color:red\"><redText title=\"20.01.2014\"> 14.01.2014 </redText></strike> </a>");
            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());

        }

      
       

        #region CustomDispalyJourneyDateForVU


        [Test]
        public void CustomDisplayJourneyDateForVU_ReclaimDateNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Journey journey = mock.Object.Journeys.Where(j => j.JourneyID == 3).FirstOrDefault();

            //Act
            MvcHtmlString result = helper.CustomDisplayJourneyDateForVU(journey);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<a id=journeyLink </a> 14.01.2014 "));

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayJourneyDateForVU_ReclaimDateNotNull_MvcString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Journey journey = mock.Object.Journeys.Where(j => j.JourneyID == 2).FirstOrDefault();

            //Act
            string statusHint = "20.01.2014";
            MvcHtmlString result = helper.CustomDisplayJourneyDateForVU(journey);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<a id=journeyLink <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, journey.Date));

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString()); 
        }

        #endregion


    }
}
