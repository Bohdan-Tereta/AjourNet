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
using AjourBT.Models;
using AjourBT.Infrastructure;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayBTsDatesActionLinkTest
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

            List<Department> departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "SDD1U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "TSS1U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "RAD1U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "RAD2U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "RAD3U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "RAD4U",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "RAD5U",Employees = new List<Employee>()}};

            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Andriy", LastName = "Zadorozhniy", DepartmentID = 1, EID = "andz", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 2, FirstName = "Anna", LastName = "Storoshenko", DepartmentID = 2, EID = "asto", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>()},          
                new Employee {EmployeeID = 3, FirstName = "Tetyana", LastName = "Pylat", DepartmentID = 1, EID = "tepy", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 4, FirstName = "Taras", LastName = "Kopchyshyn", DepartmentID = 4 , EID = "tako", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 5, FirstName = "Iryna", LastName = "Dankovska", DepartmentID = 6, EID = "dani", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 6, FirstName = "Bohdan", LastName = "Tereta", DepartmentID = 5, EID = "tebo", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 7, FirstName = "Taras", LastName = "Mandzak", DepartmentID = 5, EID = "xtma", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 8, FirstName = "Orest", LastName = "Kossak", DepartmentID = 1, EID = "xoko", DateEmployed = new DateTime(11/02/2011), IsManager = true, BusinessTrips = new List<BusinessTrip>() }
             };

            List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            { 
                new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null,  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="meeting", Manager = "xopu", Responsible = "xopu",  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment="Visa Expired",  Status = BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Reported, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment=null,  Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Confirmed, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 30, StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2013, 03,01), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Cancelled, EmployeeID = 1, LocationID = 1}
            };

            List<Location> locations = new List<Location>
             { 
                new Location {LocationID = 1, Title = "LVS", Address = "Lviv, Sholom St.", BusinessTrips = new List<BusinessTrip>()}, 
                new Location {LocationID = 2, Title = "LVG", Address = "Lviv, Gazova St.", BusinessTrips = new List<BusinessTrip>()}
                             
             };

            mock.Setup(m => m.Departments).Returns(departments.AsQueryable());
            mock.Setup(m => m.Employees).Returns(employees.AsQueryable());
            mock.Setup(m => m.Locations).Returns(locations.AsQueryable());
            mock.Setup(m => m.BusinessTrips).Returns(businessTrips.AsQueryable());

            departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 1));
            departments.Find(d => d.DepartmentID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 2));
            departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 3));
            departments.Find(d => d.DepartmentID == 4).Employees.Add(employees.Find(e => e.EmployeeID == 4));
            departments.Find(d => d.DepartmentID == 6).Employees.Add(employees.Find(e => e.EmployeeID == 5));
            departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 6));
            departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 7));
            departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 8));

            employees.Find(e => e.EmployeeID == 1).Department = departments.Find(v => v.DepartmentID == 1);
            employees.Find(e => e.EmployeeID == 2).Department = departments.Find(v => v.DepartmentID == 2);
            employees.Find(e => e.EmployeeID == 3).Department = departments.Find(v => v.DepartmentID == 1);
            employees.Find(e => e.EmployeeID == 4).Department = departments.Find(v => v.DepartmentID == 4);
            employees.Find(e => e.EmployeeID == 5).Department = departments.Find(v => v.DepartmentID == 6);
            employees.Find(e => e.EmployeeID == 6).Department = departments.Find(v => v.DepartmentID == 5);
            employees.Find(e => e.EmployeeID == 7).Department = departments.Find(v => v.DepartmentID == 5);
            employees.Find(e => e.EmployeeID == 8).Department = departments.Find(v => v.DepartmentID == 1);

            employees.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 1));
            employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 2));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 3));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 4));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 5));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 6));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 7));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 8));
            employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 9));
            employees.Find(e => e.EmployeeID == 6).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 10));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 11));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 12));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 13));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 14));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 15));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 16));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 17));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 18));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 19));
            employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 20));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 21));
            employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 22));
            employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 23));
            employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 24));
            employees.Find(e => e.EmployeeID == 8).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 25));
            employees.Find(e => e.EmployeeID == 8).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 26));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 27));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 28));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 29));
            employees.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 30));


        }


        [Test]
        public void CustomDisplayBTsDatesActionLink_BusinessTripIsNULL_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayBTsDatesActionLink(null).ToString();

            //Assert

            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayBTsDatesActionLink_BusinessTripHasOrderStartDateAndEndDate_NotEmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            string result = helper.CustomDisplayBTsDatesActionLink(bTrip).ToString();
            string dateFormat = MvcApplication.JSDatePattern;
            string selectedDepartment = "";
            string output = String.Format("<a id=\"EditReportedBTACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> {3} - {4}  &nbsp; &nbsp; &nbsp; <blue><b>{5} - {6}</b></blue></a>", "/BusinessTrip/EditReportedBT/",
                                          bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(),
                                          bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(),
                                          bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat);

            //Assert
            Assert.AreEqual(output, result);
        }

        [Test]
        public void CustomDisplayBTsDatesActionLink_BusinessTripNoOrderStartDateAndEndDate_NotEmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            string result = helper.CustomDisplayBTsDatesActionLink(bTrip).ToString();
            string dateFormat = MvcApplication.JSDatePattern;
            string selectedDepartment = "";
            string output = String.Format("<a id=\"EditReportedBTACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/BusinessTrip/EditReportedBT/",
                                          bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(),
                                          bTrip.EndDate.ToShortDateString(), dateFormat);

            //Assert
            Assert.AreEqual(output, result);
 
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BusinessTripIsNULL_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(null).ToString();

            //Assert

            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BusinessTripHasOrderStartDateAndEndDate_NotEmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip).ToString();
            string dateFormat = MvcApplication.JSDatePattern;
            string selectedDepartment = "";
            string output = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{7}\"> {3} - {4}  &nbsp; &nbsp; &nbsp; <blue><b>{5} - {6}</b></blue></a>", "/BusinessTrip/ShowBTData/",
                                          bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(),
                                          bTrip.EndDate.ToShortDateString(), bTrip.OrderStartDate.Value.ToShortDateString(),
                                          bTrip.OrderEndDate.Value.ToShortDateString(), dateFormat);

            //Assert
            Assert.AreEqual(output, result);
        }

        [Test]
        public void CustomDisplayAccountableBTsDatesActionLink_BusinessTripNoOrderStartDateAndEndDate_NotEmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            string result = helper.CustomDisplayAccountableBTsDatesActionLink(bTrip).ToString();
            string dateFormat = MvcApplication.JSDatePattern;
            string selectedDepartment = "";
            string output = String.Format("<a id=\"ShowBTDataACC\" href=\"{0}{1}?selectedDepartment={2}\" data-date-format=\"{5}\"> {3} - {4} </a>", "/BusinessTrip/ShowBTData/",
                                          bTrip.BusinessTripID, selectedDepartment, bTrip.StartDate.ToShortDateString(),
                                          bTrip.EndDate.ToShortDateString(), dateFormat);

            //Assert
            Assert.AreEqual(output, result);

        }

    }
}

