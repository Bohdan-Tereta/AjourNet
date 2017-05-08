using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure; 

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class LastBTGetter
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

        ViewContext viewContext;

        [SetUp]
        public void SetUpFixture()
        {
            viewContext = new ViewContext();
            viewContext.HttpContext = new FakeHttpContext();
        }

        [Test]
        public void GetLastBT_EmployeeHasNoBTsAtAll_null()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            Employee emp = new Employee(); 
            emp.BusinessTrips = new List<BusinessTrip>()    ;
 
            //Act
            BusinessTrip result = helper.GetLastBT(emp);             

            //Assert   
            Assert.IsNull(result);
        }

        [Test]
        public void GetLastBT_EmployeeBusinessTripsNull_null()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            Employee emp = new Employee();
            emp.BusinessTrips = null; 

            //Act
            BusinessTrip result = helper.GetLastBT(emp);

            //Assert   
            Assert.IsNull(result);
        }

        [Test]
        public void GetLastBT_EmployeeHasBTWhichNotEndedYet_Null ()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            Employee emp = new Employee();
            emp.BusinessTrips = new List<BusinessTrip>();
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure(), Status = BTStatus.Confirmed | BTStatus.Reported }); 

            //Act
            BusinessTrip result = helper.GetLastBT(emp);

            //Assert   
            Assert.IsNull(result);
        }

        //[Test]
        //public void GetLastBT_EmployeeHasBTsWithDifferentStatuses_Confirmedreported()
        //{
        //    //Arrange
        //    HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
        //    Employee emp = new Employee();
        //    emp.BusinessTrips = new List<BusinessTrip>();
        //    emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date, Status = BTStatus.Confirmed | BTStatus.Reported });

        //    //Act
        //    BusinessTrip result = helper.GetLastBT(emp);

        //    //Assert   
        //    Assert.IsNull(result);
        //}

        [Test]
        public void GetLastBT_EmployeeHasBTsWithDifferentStatuses_OnlyConfirmedReported()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            Employee emp = new Employee();
            emp.BusinessTrips = new List<BusinessTrip>();
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-1), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-2), Status = BTStatus.Confirmed });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), Status = BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4), Status = BTStatus.Cancelled });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5), Status = BTStatus.Modified });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-6), Status = BTStatus.Planned });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-7), Status = BTStatus.Registered });

            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-1), Status = BTStatus.Confirmed | BTStatus.Reported });
            //Act
            BusinessTrip result = helper.GetLastBT(emp);

            //Assert   
            Assert.IsNotNull(result);
            //Assert.IsInstanceOf(typeof(BusinessTrip), result.GetType());
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().AddDays(-1).Date, result.EndDate); 
        }

        [Test]
        public void GetLastBT_EmployeeHasBTs_BTWithHighestEndDate()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            Employee emp = new Employee();
            emp.BusinessTrips = new List<BusinessTrip>();
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-6), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-2), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-31), Status = BTStatus.Confirmed | BTStatus.Reported });
            emp.BusinessTrips.Add(new BusinessTrip() { EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-41), Status = BTStatus.Confirmed | BTStatus.Reported });
            //Act
            BusinessTrip result = helper.GetLastBT(emp);

            //Assert 
  
            Assert.IsNotNull(result);
            //Assert.IsInstanceOf(typeof(BusinessTrip), result.GetType());
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().AddDays(-2).Date, result.EndDate); 
        }


    }
}
