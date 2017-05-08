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
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class StatusHintHelperForVUPrepsTest
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
        //Mock<IMessenger> messengerMock;
        //Mock<ControllerContext> controllerContext;
        //BusinessTripController controller;
   



        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();

            //List<Department> departments = new List<Department>{
            //         new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            //List<Employee> employees = new List<Employee>
            // {
            //    new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>()},          
            //    new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true, BusinessTrips = new List<BusinessTrip>() }
            // };

            //List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            //{ 
            //    new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 },
            //    new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null,  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },
            //    new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="meeting", Manager = "xopu", Responsible = "xopu",  Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 },

            //     new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment="Visa Expired",  Status = BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 2, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Modified, EmployeeID = 1, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="", RejectComment="", Status = BTStatus.Reported, EmployeeID = 1, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager = null, Responsible = null, RejectComment=null,  Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Confirmed, EmployeeID = 1, LocationID = 1 },
            //     new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate= new DateTime(2013, 11, 30), OrderEndDate=  new DateTime(2013, 12,11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose="", Manager="", Responsible="",  RejectComment="Visa Expired", Status = BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 },
            //};

            //List<Location> locations = new List<Location>
            // { 
            //    new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>()}, 
            //    new Location {LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>()}
                             
            // };

            //mock.Setup(m => m.Departments).Returns(departments.AsQueryable());
            //mock.Setup(m => m.Employees).Returns(employees.AsQueryable());
            //mock.Setup(m => m.Locations).Returns(locations.AsQueryable());
            //mock.Setup(m => m.BusinessTrips).Returns(businessTrips.AsQueryable());

            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 1));
            //departments.Find(d => d.DepartmentID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 2));
            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 3));
            //departments.Find(d => d.DepartmentID == 4).Employees.Add(employees.Find(e => e.EmployeeID == 4));
            //departments.Find(d => d.DepartmentID == 6).Employees.Add(employees.Find(e => e.EmployeeID == 5));
            //departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 6));
            //departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 7));
            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 8));

            //employees.Find(e => e.EmployeeID == 1).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 2).Department = departments.Find(v => v.DepartmentID == 2);
            //employees.Find(e => e.EmployeeID == 3).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 4).Department = departments.Find(v => v.DepartmentID == 4);
            //employees.Find(e => e.EmployeeID == 5).Department = departments.Find(v => v.DepartmentID == 6);
            //employees.Find(e => e.EmployeeID == 6).Department = departments.Find(v => v.DepartmentID == 5);
            //employees.Find(e => e.EmployeeID == 7).Department = departments.Find(v => v.DepartmentID == 5);
            //employees.Find(e => e.EmployeeID == 8).Department = departments.Find(v => v.DepartmentID == 1);

            //employees.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 1));
            //employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 2));
            //employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 3));
            //employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 4));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 5));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 6));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 7));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 8));
            //employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 9));
            //employees.Find(e => e.EmployeeID == 6).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 10));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 11));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 12));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 13));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 14));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 15));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 16));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 17));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 18));
            //employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 19));
            //employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 20));
            //employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 21));
            //employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 22));
            //employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 23));
            //employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 24));
            //employees.Find(e => e.EmployeeID == 8).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 25));
            //employees.Find(e => e.EmployeeID == 8).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 26));
            //employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 27));
            //employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 28));
            //employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 29));

        }

        

        [Test]
        public void CustomStatusHint_PlannedBTEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_NullFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }



        [Test]
        public void CustomStatusHint_PlannedBTEmptyPurpose_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "", Manager = "xopu", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_EmptyManager_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBT_EmptyResponsible_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = "", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBTNullablePurpose_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = null, Manager = "xopu", Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }

        [Test]
        public void CustomStatusHint_PlannedBT_NullableManager_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager =null, Responsible = "xopu", Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBT_NullableResponsible_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 10), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2013, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Purpose = "meeting", Manager = "", Responsible = null, Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1 };

            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. ", result);
        }


        [Test]
        public void CustomStatusHint_PlannedBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("", result);
        }



        [Test]
        public void CustomStatusHint_ModifiedBTRejectCommentEmptyFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("BT was Modified!\r\n", result);
        }

        [Test]
        public void CustomStatusHint_ModifiedBTRegectCommentNullFields_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("BT was Modified!\r\n", result);
        }



        [Test]
        public void CustomStatusHint_ModifiedBTRegectCommentIsNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("Not enough info for registration. BT is Rejected!", result);
        }

        [Test]
        public void CustomStatusHint_ReportedBTIsNotNull_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 31).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("BT is Reported!", result);
        }

        [Test]
        public void CustomStatusHint_RegisteredBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 8).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void CustomStatusHint_CancelledBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("BT was cancelled!\r\n", result);
        }


        [Test]
        public void CustomStatusHint_ModifiedCancelledBTIsNotNull_DefaultString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 19).FirstOrDefault();
            //Act
            string result = helper.CustomStatusHintForVUPreps(bt);

            //Assert
            Assert.AreEqual("BT was first Modified then cancelled!\r\n", result);
        }
    }
}
