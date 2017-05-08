using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using NUnit.Framework;
using System;
using AjourBT.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;
namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class SortingPrivateTripsHelperTest
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

        }

        [Test]
        public void CustomSortingPrivateTripsByStartdate_NullListPrivateTrips_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<PrivateTrip> emprivateTrips = null;
            //Act
            var result = helper.CustomSortingPrivateTripsByStartdate(emprivateTrips);

            //Assert
            Assert.AreEqual(null, result);
        }
       
        
        [Test]
        public void CustomSortingPrivateTripsByStartdate_DefaultListPrivateTrips_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<PrivateTrip> emprivateTrips = new List <PrivateTrip>();
            //Act
            var result = helper.CustomSortingPrivateTripsByStartdate(emprivateTrips);

            //Assert
            Assert.AreEqual(emprivateTrips, result);
            Assert.IsTrue(emprivateTrips.Count() == 0);
        }

        [Test]
        public void CustomSortingPrivateTripsByStartdate_ListPrivateTrips_SortedList()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            List<PrivateTrip> emprivateTrips = new List<PrivateTrip>
             {
                new PrivateTrip{ PrivateTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-190), EmployeeID = 1},
                new PrivateTrip{ PrivateTripID = 3, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-300), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-75), EmployeeID = 4 },
                new PrivateTrip{ PrivateTripID = 4, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-90), EmployeeID = 10 },
                new PrivateTrip{ PrivateTripID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-70), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-60), EmployeeID = 12 },
                new PrivateTrip{ PrivateTripID = 6, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EmployeeID = 13},
                new PrivateTrip{ PrivateTripID = 7, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-99), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-90), EmployeeID = 14 },
                new PrivateTrip{ PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-13), EmployeeID = 14 },
                };

            //Act
            var result = helper.CustomSortingPrivateTripsByStartdate(emprivateTrips).ToList();
              
            //Assert
            Assert.IsTrue(result.Count == 7);
            Assert.AreEqual(result[0].PrivateTripID, 3);
            Assert.AreEqual(result[1].PrivateTripID, 1);
            Assert.IsTrue(result[2].PrivateTripID == 4);
            Assert.IsTrue(result[3].PrivateTripID == 7);
            Assert.IsTrue(result[4].PrivateTripID == 5);
            Assert.IsTrue(result[5].PrivateTripID == 6);
            Assert.IsTrue(result[6].PrivateTripID == 8);
        }

        [Test]
        public void CustomSortingPrivateTripsByStartdate_SortedListPrivateTrips_SortedList()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            List<PrivateTrip> emprivateTrips = new List<PrivateTrip>
             {
                new PrivateTrip{ PrivateTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-190), EmployeeID = 1},
                new PrivateTrip{ PrivateTripID = 4, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-90), EmployeeID = 10 },
                new PrivateTrip{ PrivateTripID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-70), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-60), EmployeeID = 12 },
                new PrivateTrip{ PrivateTripID = 6, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EmployeeID = 13},
                new PrivateTrip{ PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-13), EmployeeID = 14 },
                };

            //Act
            var result = helper.CustomSortingPrivateTripsByStartdate(emprivateTrips).ToList();

            //Assert
            Assert.IsTrue(result.Count == 5);
            Assert.AreEqual(result[0].PrivateTripID, 1);
            Assert.AreEqual(result[1].PrivateTripID, 4);
            Assert.IsTrue(result[2].PrivateTripID == 5);
            Assert.IsTrue(result[3].PrivateTripID == 6);
            Assert.IsTrue(result[4].PrivateTripID == 8);
           
        }

       
    }
}
