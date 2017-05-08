using System;
using AjourBT.Controllers;
using AjourBT.Domain.ViewModels;
using System.Collections.Generic;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Globalization;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using System.Text;
using ExcelLibrary.SpreadSheet;
using System.IO;
using AjourBT.Domain.Infrastructure;
using AjourBT.Infrastructure;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class VUControllerTest
    {
        Mock<IRepository> mock;
        Mock<IRepository> mock2;
        Mock<IXLSExporter> xlsExporterMock; 

        static List<Employee> employees2;
        static List<BusinessTrip> businessTrips2;
        static List<Location> locations2;
        static List<Department> departments2;

        VUController controller;
        VUController controller2;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            xlsExporterMock = new Mock<IXLSExporter>(); 

            controller = new VUController(mock.Object, xlsExporterMock.Object);
        }

        public void SetUp2()
        {
            mock2 = new Mock<IRepository>();
            controller2 = new VUController(mock2.Object, xlsExporterMock.Object);

            employees2 = new List<Employee>
            {
                 new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Andzzzz", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                 new Employee {EmployeeID = 2, FirstName = "And", LastName = "Zarose", DepartmentID = 2, EID = "kaaa", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            };

            departments2 = new List<Department>()
            {
                new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
            };

            locations2 = new List<Location>
            { 
                new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>()}, 
                new Location {LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>()}                        
            };

            DateTime currentDate = DateTime.Now.ToLocalTimeAzure().Date;

            businessTrips2 = new List<BusinessTrip>();
            {
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 1, StartDate = currentDate.AddMonths(-1).Date, EndDate = currentDate.AddMonths(-1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 2, StartDate = currentDate.AddMonths(-1).Date, EndDate = currentDate.AddMonths(-1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 3, StartDate = currentDate.AddMonths(-7).Date, EndDate = currentDate.AddMonths(-7).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 4, StartDate = currentDate.Date, EndDate = currentDate.Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 5, StartDate = currentDate.Date, EndDate = currentDate.Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 6, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 7, StartDate = currentDate.AddDays(1).Date, EndDate = currentDate.AddDays(1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 8, StartDate = currentDate.AddMonths(-3).Date, EndDate = currentDate.AddMonths(-3).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 9, StartDate = currentDate.AddMonths(-6).Date, EndDate = currentDate.AddMonths(-6).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 10, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 11, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled | BTStatus.Modified, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 12, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 13, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 14, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 15, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
            };

            mock2.Setup(m => m.BusinessTrips).Returns(businessTrips2);
            mock2.Setup(m => m.Employees).Returns(employees2);
            mock2.Setup(m => m.Departments).Returns(departments2);
            mock2.Setup(m => m.Locations).Returns(locations2);


            departments2.Find(d => d.DepartmentID == 1).Employees.Add(employees2.Find(e => e.EmployeeID == 1));
            departments2.Find(d => d.DepartmentID == 2).Employees.Add(employees2.Find(e => e.EmployeeID == 2));

            employees2.Find(e => e.EmployeeID == 1).Department = departments2.Find(v => v.DepartmentID == 1);
            employees2.Find(e => e.EmployeeID == 2).Department = departments2.Find(v => v.DepartmentID == 2);

            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 1));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 2));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 3));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 4));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 5));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 6));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 7));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 8));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 9));


            //businessTrips2.Find(b => b.BusinessTripID == 1).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 2).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 3).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 4).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 5).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 6).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 7).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 8).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 9).Location = (locations2.Find(l => l.LocationID == 1));

            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 1));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 2));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 3));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 4));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 5));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 6));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 7));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 8));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 9));

        }

        public static void AddBusinessTrip(BusinessTrip bt)
        {
            businessTrips2.Add(bt);
            SetBusinessTripRelations(bt);
        }

        private static void SetBusinessTripRelations(BusinessTrip bt)
        {
            employees2.Find(e => e.EmployeeID == bt.EmployeeID).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == bt.BusinessTripID));
            businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID).BTof = (employees2.Find(l => l.EmployeeID == bt.EmployeeID));
            businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID).Location = (locations2.Find(l => l.LocationID == bt.LocationID));
            locations2.Find(l => l.LocationID == bt.LocationID).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID));
        }
        
        #region ShowBTInformation

        [Test]
        public void ShowBTInformation_NotExistingBT_HttpNotFound()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowBTInformation(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowBTInformation_ExistingBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            var view = controller.ShowBTInformation(25);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(25, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }

        [Test]
        public void ShowBTInformation_ExistingReportedBTButCancelled_HttpNotFound()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 18).FirstOrDefault();
            var view = controller.ShowBTInformation(18);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(18, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }

        [Test]
        public void ShowBTInformation_ExistingConfirmedBTButNotReported_HttpNotFound()
        {
            //Arrange
            

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            var view = controller.ShowBTInformation(3);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }


        #endregion
        
        #region BTs by Dates/Location tab

        #region GetBusinessTripByDatesVU

        [Test]
        public void GetBusinessTripByDatesVU_Default_DefaultYear()
        {
            // Arrange

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU();

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_0Year_ListofYear()
        {
            // Arrange

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripByDatesVU_Current_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_2012_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = 2012;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_0_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_NotExistingYear_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = DateTime.Now.AddYears(10).Year;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        #endregion

        #region GetBusinessTripDataByDatesVU

        [Test]
        public void GetBusinessTripDataByDatesVU_0_ListOfYear()
        {
            // Arrange
            int selectedYear = 0;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_2013Year_BTs2013()
        {
            // Arrange
            int selectedYear = 2013;
            BusinessTrip btStartDateToBeChanged = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();
            btStartDateToBeChanged.StartDate = new DateTime(2018, 01, 01);

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(13, bts.Count());
            Assert.AreEqual(bts[0].BTof.EID, "daol");
            Assert.AreEqual(bts[6].BTof.EID, "tadk");
            Assert.AreEqual(bts[7].BTof.EID, "xomi");
            Assert.AreEqual(bts[8].BTof.EID, "xtwe");
            Assert.AreEqual("iwpe", bts[10].BTof.EID);
            Assert.AreEqual("tedk", bts[11].BTof.EID);

            Assert.AreEqual(23, bts[0].BusinessTripID);
            Assert.AreEqual(24, bts[1].BusinessTripID);
            Assert.AreEqual(25, bts[2].BusinessTripID);
            Assert.AreEqual(33, bts[3].BusinessTripID);
            Assert.AreEqual(26, bts[4].BusinessTripID);
            Assert.AreEqual(27, bts[5].BusinessTripID);
            Assert.AreEqual(18, bts[6].BusinessTripID);
            Assert.AreEqual(35, bts[7].BusinessTripID);
            Assert.AreEqual(32, bts[8].BusinessTripID);
            Assert.AreEqual(21, bts[9].BusinessTripID);
            Assert.AreEqual(39, bts[10].BusinessTripID);
            Assert.AreEqual(36, bts[11].BusinessTripID);

            Assert.AreEqual(2013, ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_2012Year_BTs2012()
        {
            // Arrange
            int selectedYear = 2012;
            var status = BTStatus.Confirmed | BTStatus.Reported;


            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual(employees[0].BTof.EID, "xomi");
            Assert.AreEqual(employees[1].BTof.EID, "tedk");
            Assert.AreEqual(employees[0].Status, status);
            Assert.AreEqual(employees[1].Status, status);
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesVU_2014Year_BTs2014()
        {
            // Arrange
            int selectedYear = 2014;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(4, bts.Count());
            Assert.AreEqual("xtwe", bts[0].BTof.EID);
            Assert.AreEqual("chap", bts[1].BTof.EID);
            Assert.AreEqual("tedk", bts[2].BTof.EID);
            Assert.AreEqual("tedk", bts[3].BTof.EID);

            Assert.AreEqual(15, bts[0].BusinessTripID);
            Assert.AreEqual(38, bts[1].BusinessTripID);
            Assert.AreEqual(37, bts[2].BusinessTripID);
            Assert.AreEqual(4, bts[3].BusinessTripID);
            Assert.AreEqual(2014, ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesVU_DefaultYear_NoBTs()
        {
            // Arrange

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU();
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU().Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_NotExistingYear_NoBTs()
        {
            // Arrange
            int selectedYear = DateTime.Now.AddYears(-10).Year;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IList<BusinessTripViewModel> result = (IList<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        #endregion



        #endregion

        #region BTs by Quarters tab

        #region GetListOfDepartmentsVU

        [Test]
        public void GetListOfDepartmentsVU_Default_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_DefaultYearAndSDDDA_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_DefaultYearAndNull_ListOfDepartments()
        {
            //Arrange

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetListOfDepartmentsVU_Year2012AndDefaultDep_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 2012;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_Year2012AndAndSDDDA_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_Year2012AndAndNull_ListOfDepartments()
        {
            //Arrange

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetListOfDepartmentsVU_0AndDefaultDep_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 0;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_0AndAndSDDDA_ListOfDepartments()
        {
            // Arrange
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 0;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_0AndAndNull_ListOfDepartments()
        {
            //Arrange

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }
         #endregion

        #region GetListOfYearsForQuarterVU

        [Test]
        public void GetListOfYearsForQuarterVU_YearNowEmptyString_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string searchString = "";
            var view = controller.GetListOfYearsForQuarterVU(selectedYear,searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);

        }

        [Test]
        public void GetListOfYearsForQuarterVU_2012SearchStringEmpty_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = 2012;
            string searchString = "";
            var view = controller.GetListOfYearsForQuarterVU(selectedYear,searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetListOfYearsForQuarterVU_0_ListOfYear()
        {
            // Arrange

            // Act
            int selectedYear = 0;
            string searchString = "";
            var view = controller.GetListOfYearsForQuarterVU(selectedYear,searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region GetBusinessTripDataInQuarterVU

        [Test]
        public void GetBusinessTripDataInQuarterVU_2011SDDDASearchStringEmpty_BTsIn2011()
        {
            // Arrange
            SetUp2();
            
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int currentDay = DateTime.Now.ToLocalTimeAzure().Day;
            int currentYear = DateTime.Now.ToLocalTimeAzure().Year;
            DateTime startDateOfBT = new DateTime(currentYear, currentMonth, currentDay);
            BusinessTrip btInCurrentMonth = new BusinessTrip { BusinessTripID = 1000, StartDate = startDateOfBT, EndDate = startDateOfBT.AddDays(1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 1, BTof = mock2.Object.Employees.Where(l => l.EmployeeID == 2).FirstOrDefault() };
            mock2.Object.BusinessTrips.ToList().Add(btInCurrentMonth);
            Employee employee = mock2.Object.Employees.Where(e => e.EmployeeID == 2).FirstOrDefault();
            employee.BusinessTrips.Add(btInCurrentMonth);


            // Act    
            int selectedKey = 2011;
            string selectedDepartment = "";
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);

            //CollectionAssert.IsEmpty((employees[0].BusinessTripsByMonth).Values);
            //CollectionAssert.IsEmpty((employees[1].BusinessTripsByMonth).Values);

            //Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            //Assert.AreEqual(1, employees[1].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());


            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2011, ((ViewResult)view).ViewBag.SelectedKey);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }
        
        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndDefaultDepartmentSearchStringEmpty_BTsIn2013()
        {
            // Arrange

            // Act    
            int selectedKey = 2013;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            int daysInDanisBT = ((DateTime.Now.ToLocalTimeAzure().AddDays(-3).Date - new DateTime(2013, 10, 10).Date).Days + 1) + 15;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual("daol", employees[0].EID);
            Assert.AreEqual("siol", employees[4].EID);
            Assert.AreEqual("ovol", employees[7].EID);
            Assert.AreEqual("Manowens", employees[3].LastName);

            Assert.AreEqual(daysInDanisBT, employees[0].DaysUsedInBt);
            Assert.AreEqual(22, employees[2].DaysUsedInBt);
            Assert.AreEqual(87, employees[3].DaysUsedInBt);
            Assert.AreEqual(0, employees[4].DaysUsedInBt);
            Assert.AreEqual(1, ((employees[2].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndSDDDASearchStringEmpty_BTsIn2013OfSDDDA()
        {
            // Arrange

            // Act    
            int selectedKey = 2013;
            string selectedDepartment = "SDDDA";
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(22, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(20, employees[2].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndNullSearchStringEmpty_NoBTs()
        {
            // Arrange

            // Act    
            int selectedKey = 2013;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012AndSDDDAEmptySearchString_BTsIn2012OfSDDDA()
        {
            // Arrange

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(employees[0].DaysUsedInBt, 91);
            Assert.AreEqual(91, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012YearAndDefaultDepartmentEmtySearchString_BTs2012()
        {
            // Arrange

            // Act    
            int selectedKey = 2012;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual(employees[0].EID, "daol");
            Assert.AreEqual("siol", employees[4].EID);
            Assert.AreEqual("ovol", employees[7].EID);
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(employees[2].DaysUsedInBt, 91);
            Assert.AreEqual(0, employees[4].DaysUsedInBt);
            Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012YearAndNullEmtpySearchString_NoBTs()
        {
            // Arrange

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndDefaultDepartmentEmptySearchString_NoBTs()
        {
            // Arrange

            // Act    
            int selectedKey = 2017;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual(employees[0].EID, "daol");
            Assert.AreEqual("siol", employees[4].EID);
            Assert.AreEqual("ovol", employees[7].EID);
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(0, employees[4].DaysUsedInBt);
            Assert.AreEqual(employees[7].DaysUsedInBt, 0);
            Assert.AreEqual(0, ((employees[4].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(((employees[7].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndSDDDAEmptySearchString_NoBTs()
        {
            // Arrange

            // Act    
            int selectedKey = 2017;
            string selectedDepartment = "SDDDA";
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            //Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(0, employees[2].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndNullEmptySearchString_NoBTs()
        {
            // Arrange

            // Act    
            int selectedKey = 2017;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment,searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndDefaultDepartmentEmptySearchString_BTsInCurrentMonth()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 0;
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };
            int daysUsedInBt = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 4 : 6;
            int btsByMonth = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 3 : 5;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(3, (employees[0].BusinessTripsByMonth.Values).Count());
            Assert.AreEqual(0, employees[1].DaysUsedInBt);

            Assert.AreEqual(4, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);

            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);
            //Assert.AreEqual(10, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[2].BusinessTripID);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndTAAAAEmptySearchString_NoBts()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 0;
            string selectedDepartment = "TAAAA";
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndNull_NoBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 0;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }
        
        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndDefaultDepartment_LastMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 1;
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };
            int daysUsedInBt = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 4 : 6;
            int btsByMonth = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 3 : 5;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(4, employees[0].DaysUsedInBt);
            Assert.AreEqual(3, employees[0].BusinessTripsByMonth.Values.Count);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);

            //previous month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);
            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray().Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            //Assert.AreEqual(6, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[1].BusinessTripID);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndTAAAA_NoLastMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 1;
            string selectedDepartment = "TAAAA";
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndNull_NoLastMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 1;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndDefaultDepartment_LastThreeMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 3;
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };
            int daysUsedInBt = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 4 : 6;
            int btsByMonth = (currentMonth == 2 || currentMonth == 4 || currentMonth == 6) ? 3 : 5;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(daysUsedInBt, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);
            Assert.AreEqual(3, (employees[0].BusinessTripsByMonth.Values).Count());
            //prevoius month
            Assert.AreEqual(3, (employees[0].BusinessTripsByMonth.Values).Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);


            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == secondPreviousMonth));
            //thirdprevoius month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[2].Value.ToArray().Count());
            Assert.AreEqual(8, employees[0].BusinessTripsByMonth.ToArray()[2].Value.ToArray()[0].BusinessTripID);
            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray().Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            //Assert.AreEqual(6, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[1].BusinessTripID);


        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndAndTAAAA_NoLastThreeMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 3;
            string selectedDepartment = "TAAAA";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndNull_NoLastThreeMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 3;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndDefaultDepartment_LastSixMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 6;
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixthPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(5, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);
            Assert.AreEqual(4, (employees[0].BusinessTripsByMonth.Values).Count());

            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).Count());
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == secondPreviousMonth));
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).Count());
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == fourthPreviousMonth));
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == fifthPrevoiusMonth));
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).Count());

            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value[0].BusinessTripID);
            Assert.AreEqual(6, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value[1].BusinessTripID);

            //previous month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).FirstOrDefault().Value.Count());
            //Assert.AreEqual(3, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).FirstOrDefault().Value[0].BusinessTripID);

            //thirdprevious month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(8, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).FirstOrDefault().Value[0].BusinessTripID);

            //sixprevoius month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(9, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).FirstOrDefault().Value[0].BusinessTripID);

        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndAndTAAAA_NoLastSixMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 6;
            string selectedDepartment = "TAAAA";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndNull_NoLastSixMonthBTs()
        {
            // Arrange
            SetUp2();

            // Act    
            int selectedKey = 6;
            string selectedDepartment = null;
            string searchString = "";
            var view = controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller2.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_emptySearchString()
        {
            // Arrange

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            string searchString = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(employees[0].DaysUsedInBt, 91);
            Assert.AreEqual(91, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);

        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_LongSearchString_empty()
        {
            // Arrange

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            string searchString = "ADJHAAS";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, searchString).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_SearchString()
        {

            // Arrange

            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            string searchString = "a";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, searchString);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, "a").Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(3, employees.Count());
            Assert.AreEqual(employees[0].EID, "chap");
            Assert.AreEqual(employees[1].EID, "ivte");
            Assert.AreEqual(employees[2].EID, "andl");
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }


        #endregion

        #endregion

        #region "BTs in preparation process" tab

        #region GetPrepBusinessTripDataVU

        [Test]
        public void GetPrepBusinessTripDataVU_Default_ListOfEmployee()
        {
            // Arrange

            // Act    
            var view = controller.GetPrepBusinessTripDataVU();
            Dictionary<Employee, List<BusinessTrip>> result = (Dictionary<Employee, List<BusinessTrip>>)controller.GetPrepBusinessTripDataVU().Model;
            Dictionary<Employee, List<BusinessTrip>> employees = result;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(10, employees.Count());
            Assert.AreEqual("daol", employees.Keys.ElementAt(0).EID);
            Assert.AreEqual("xomi", employees.Keys.ElementAt(2).EID);
            Assert.AreEqual("xtwe", employees.Keys.ElementAt(3).EID);
            Assert.AreEqual(7, employees.Values.ElementAt(0).Count);
            Assert.AreEqual(2, employees.Values.ElementAt(2).Count);
            Assert.AreEqual(6, employees.Values.ElementAt(3).Count);
        }

        #endregion

        #endregion

        #region Private Trips tab

        #region GetPrivateTripVU

        [Test]
        public void GetPrivateTripVU_EmptyString_searchString()
        {
            // Arrange
            string searchString = "";

            // Act

            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("", ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripVU_NullString_searchString()
        {
            // Arrange
            string searchString = null;

            // Act
            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripVU_dan_searchString()
        {
            // Arrange
            string searchString = "dan";

            // Act

            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("dan", ((ViewResult)view).Model);
        }

        #endregion

        #region GetPrivateTripDataVU

        [Test]
        public void GetPrivateTripDataVU_Default_AllEmployees()
        {
            // Arrange
            string searchString = "";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Kowwood");
            Assert.AreEqual(result.ToArray()[1].LastName, "Struz");
            Assert.AreEqual(23, result.ToArray().Length);
            Assert.AreEqual(result.ToArray()[2].LastName, "Daolson");
            Assert.AreEqual(result.ToArray()[3].LastName, "Kowood");
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataVU_FilterTep_EmployeesContain_Tep()
        {
            // Arrange - create the controller     
            string searchString = "ted";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[0].FirstName, "Tymur");
            Assert.AreEqual(result.ToArray()[0].EID, "tedk");
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataVU_Filteraa_EmployeesContain_aa()
        {
            // Arrange
            string searchString = "aa";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(0, result.ToArray().Length);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region SearchPrivateTripData

        [Test]
        public void SearchPrivateTripData_StringEmpty_AllEmpployees()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "");

            //Assert
            Assert.AreEqual(23, resultList.Count);
            Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
            Assert.AreEqual("PaidVac", resultList.ToArray()[6].FirstName);
            Assert.AreEqual("Only", resultList.ToArray()[6].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(resultList, typeof(Employee));

        }
        [Test]
        public void SearchPrivateTripData_StringABRAKADARA_NothingFound()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "ABRAKADARA");

            //Assert
            Assert.AreEqual(0, resultList.Count);


        }

        [Test]
        public void SearchPrivateTripData_StringAN_SelectedEmployees()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "AN");

            //Assert
            Assert.AreEqual(6, resultList.Count);
            Assert.AreEqual("Anatoliy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Struz", resultList.ToArray()[0].LastName);
            Assert.AreEqual("Ivan", resultList.ToArray()[1].FirstName);
            Assert.AreEqual("Daolson", resultList.ToArray()[1].LastName);


        }

        #endregion

        #endregion

        #region 'BTs by Units' tab

        #region GetBusinessTripByUnitsVU

        [Test]
        public void GetBusinessTripByUnitsVU()
        {
            //Arrange

            //Act
            var result = controller.GetBusinessTripByUnitsVU(2014);

            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(2014, ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByUnitsVU_SelectedYear_PartialView()
        {
            //Arrange
            int selectedYear = 2014; 
 
            //Act
            var result = controller.GetBusinessTripDataByUnitsVU(selectedYear);

            //Assert 
            mock.Verify(m => m.GetBusinessTripDataByUnits(selectedYear), Times.Once); 
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(2014, ((PartialViewResult)result).ViewBag.SelectedYear);
            Assert.AreEqual(MvcApplication.JSDatePattern, ((PartialViewResult)result).ViewBag.JSDatePattern);
            Assert.IsInstanceOf<List<BusinessTripViewModel>>(((PartialViewResult)result).Model);
        }

        [Test]
        public void GetBusinessTripDataByUnitsVU_NoSelectedYear_DefaultYearPartialView()
        {
            //Arrange

            //Act
            var result = controller.GetBusinessTripDataByUnitsVU();

            //Assert 
            mock.Verify(m => m.GetBusinessTripDataByUnits(0), Times.Once);
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(0, ((PartialViewResult)result).ViewBag.SelectedYear);
            Assert.AreEqual(MvcApplication.JSDatePattern, ((PartialViewResult)result).ViewBag.JSDatePattern);
            Assert.IsInstanceOf<List<BusinessTripViewModel>>(((PartialViewResult)result).Model);
        }

        #endregion 
        
        
        #region ExportBusinessTripByDatesToExcel

        [Test]
        public void ExportBusinessTripByUnitsToExcel_Year2014_FileResult()
        {
            //Arrange
            int selectedYear = 2014; 

            //Act 
            FileResult file = controller.ExportBusinessTripByUnitsToExcel(selectedYear) as FileResult;

            //Assert 
            mock.Verify(m => m.GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(selectedYear), Times.Once());
            xlsExporterMock.Verify(m => m.ExportBusinessTripsToExcelVU(It.IsAny<IList<BusinessTripViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }         

        #endregion

        #endregion

        #region Visas and Permits tab

        //TODO: duplicated in BTM, VU controller tests
        //#region SearchVisaData

        private string GetDateWithoutYear(int month, int day)
        {
            int DateWithSeparatorLength = 5;
            string tempDate = new DateTime(2014, month, day).ToShortDateString();
            string dateWithoutYear = "";
            bool endWith = tempDate.EndsWith("2014", StringComparison.CurrentCulture);
            bool startWith = tempDate.StartsWith("2014", StringComparison.CurrentCulture);

            if (endWith)
            {
                dateWithoutYear = tempDate.Substring(0, tempDate.Length - DateWithSeparatorLength);
            }
            if (startWith)
            {
                dateWithoutYear = tempDate.Substring(DateWithSeparatorLength, tempDate.Length);
            }

            return dateWithoutYear;
        }

        #region GetVisaVU

        [Test]
        public void GetVisaVU_AllEmployees()
        {
            //Arrange

            //Act
            var resultView = controller.GetVisaVU("");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaVU_EmployeesContain_Te()
        {
            //Arrange

            //Act
            var resultView = controller.GetVisaVU("Te");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("Te", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaVU_EmployeesContain_qq()
        {
            //Arrange

            //Act
            var resultView = controller.GetVisaVU("qq");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("qq", ((ViewResult)resultView).ViewBag.SearchString);
        }

        #endregion

        #region GetVisaDataVU

        [Test]
        public void GetVisaDataVU_Default_ProperView()
        {
            // Arrange - create the controller      

            // Act - call the action method
            var view = controller.GetVisaDataVU();
            IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model; 


            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            mock.Verify(m => m.SearchVisaData(""), Times.Once());
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result);
            Assert.AreEqual("", view.ViewBag.SearchString); 
        }

        [Test]
        public void GetVisaDataVU_EmptySearchString_ProperViewTrimmedSearchString()
        {
            // Arrange - create the controller     
            string searchString = "";

            // Act - call the action method
            var view = controller.GetVisaDataVU(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;


            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            mock.Verify(m => m.SearchVisaData(searchString.Trim()), Times.Once());
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result);
            Assert.AreEqual(searchString, view.ViewBag.SearchString.Trim());
        }   

        [Test]
        public void GetVisaDataVU_NotEmptySearchString_ProperViewTrimmedSearchString()
        {
            // Arrange - create the controller     
            string searchString = "abc";

            // Act - call the action method
            var view = controller.GetVisaDataVU(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;


            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            mock.Verify(m => m.SearchVisaData(searchString.Trim()), Times.Once());
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result);
            Assert.AreEqual(searchString, view.ViewBag.SearchString.Trim());
        }         

        #endregion

        #region Export VisasAndPermits into Excel File

        [Test]
        public void ExportVisasAndPermit_EmptySearchString_FileResult()
        {
            //Arrange 

            //Act 
            FileResult file = controller.ExportVisasAndPermits("") as FileResult;

            //Assert 
            mock.Verify(m => m.SearchVisaData(""), Times.Once());
            xlsExporterMock.Verify(m => m.ExportVisasAndPermitsVU(It.IsAny<IList<Employee>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void ExportVisasAndPermit_NotEmptySearchString_FileResult()
        {
            //Arrange 
            string searchString = "abc";

            //Act 
            FileResult file = controller.ExportVisasAndPermits(searchString) as FileResult;

            //Assert 
            mock.Verify(m => m.SearchVisaData(searchString), Times.Once());
            xlsExporterMock.Verify(m => m.ExportVisasAndPermitsVU(It.IsAny<IList<Employee>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }                

        #endregion
        
        #endregion

        #region Employees tab

        #region GetEmployeeReadOnlyVU
        [Test]
        public void GetEmployeeReadOnlyVU_Null_NullSelectedDepartment()
        {
            //Arrange

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployeeReadOnlyVU(null);
            string selectedDepartment = null;
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }
        #endregion

        #region GetEmployeeDataReadOnlyVU
        [Test]
        public void GetEmployeeDataReadOnlyVU_Null_AllEmployeesAndViewBag()
        {
            // Arrange
            
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;

            // Act
            //IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeDataReadOnlyVU(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(view.ViewName, "");
            Assert.IsNotNull(view.ViewBag.empsByPositions);

        }
        #endregion

        #region GetEmployeesByPositions
        [Test]
        public void GetEmployeesByPositions_Null_AllDepartmentsStatistics()
        {
            // Arrange
            string selectedDepartment = null;

            // Act
            Dictionary<string, int> result = controller.GetEmployeesByPositions(selectedDepartment);


            // Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(0, result["Employee"]);
            Assert.AreEqual(5, result["Software developer"]);

        }

        [Test]
        public void GetEmployeesByPositions_EmptyString_AllDepartmentsStatistics()
        {
            // Arrange
            
            string selectedDepartment = "";

            // Act
            Dictionary<string, int> result = controller.GetEmployeesByPositions(selectedDepartment);


            // Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(0, result["Employee"]);
            Assert.AreEqual(5, result["Software developer"]);

        }

        [Test]
        public void GetEmployeesByPositions_SDDDADeparetment_SDDDAStatistics()
        {
            // Arrange
            
            string selectedDepartment = "SDDDA";

            // Act
            Dictionary<string, int> result = controller.GetEmployeesByPositions(selectedDepartment);


            // Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(0, result["Employee"]);
            Assert.AreEqual(1, result["Software developer"]);

        }
        #endregion  

        #region ExportEmployeesToExcelVU
        [Test]
        public void ExportEmployeesToExcelVU_DefaultDepartment_FileResult()
        {
            //Arrange 
            //Act 
            FileResult file = controller.ExportEmployeesToExcelVU("") as FileResult;

            //Assert 
            mock.Verify(m => m.SearchUsersData("", ""), Times.Once());
            xlsExporterMock.Verify(m => m.ExportEmployeesToExcelVU(It.IsAny<IList<EmployeeViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void ExportEmployeesToExcelVU_Dept1_FileResult()
        {
            //Arrange 
            
            //Act 
            FileResult file = controller.ExportEmployeesToExcelVU("SDDDA") as FileResult;

            //Assert 
            mock.Verify(m => m.SearchUsersData("SDDDA", ""), Times.Once());
            xlsExporterMock.Verify(m => m.ExportEmployeesToExcelVU(It.IsAny<IList<EmployeeViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        #endregion

        #endregion

    }
}