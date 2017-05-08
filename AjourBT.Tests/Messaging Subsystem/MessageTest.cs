using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Concrete;
using System.IO;
using AjourBT.Domain.Entities;
using System.Text.RegularExpressions;
using AjourBT.Domain.Infrastructure;
using System.Web.Configuration;

namespace AjourBT.Tests.Messaging_Subsystem
{
    [TestFixture]
    public class MessageTest
    {
        List<string> BTTemplates;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            BTTemplates = new List<string>();
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b>, SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2<b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>");
            BTTemplates.Add("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
                + "<br/>" + "<b>Habitation (not confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel");
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
            }
        }

        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Department> departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), FullNameUk ="Джонні Роус Олександрович"},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>()},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true }
             };

            List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            { 
                new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,10), Status= BTStatus.Planned, EmployeeID = 1, LocationID = 1,  Manager = "xtwe", Responsible = "wiza", Purpose = "Work in BD2" },
                new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2014,11,01), EndDate = new DateTime (2014,11,10), Status= BTStatus.Registered, EmployeeID = 2, LocationID = 1 }, 
                new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014,10,20), EndDate = new DateTime (2014,10,30), Status= BTStatus.Confirmed, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014,10,01), EndDate = new DateTime (2014,10,12), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,10,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013,08,01), EndDate = new DateTime (2013,08,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,25), Status= BTStatus.Confirmed, EmployeeID = 5, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned, EmployeeID = 6, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 11, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned, EmployeeID = 7, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 12, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 13, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee reg + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 14, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 15, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,26), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 17, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 7, LocationID = 1, Comment = "7 employee reg and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired"},
                new BusinessTrip { BusinessTripID = 18, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,25), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired" },
                new BusinessTrip { BusinessTripID = 19, StartDate = new DateTime(2014,09,01), EndDate = new DateTime (2014,09,27), Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and modified and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired" },
                new BusinessTrip { BusinessTripID = 20, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,27), Status= BTStatus.Planned, EmployeeID = 2, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting", RejectComment = "visa expired" },
                new BusinessTrip { BusinessTripID = 21, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013,10,01), EndDate = DateTime.Now.ToLocalTimeAzure(), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 23, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,10,03), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 24, StartDate = new DateTime(2013,10,01), EndDate = DateTime.Now.ToLocalTimeAzure(), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" }
            };

            List<Location> locations = new List<Location>
             { 
                new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>()}, 
                new Location {LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>()}
                             
             };

            List<VisaRegistrationDate> visaRegistrationDates = new List<VisaRegistrationDate>
            {
                new VisaRegistrationDate {EmployeeID=1, RegistrationDate=new DateTime(2013,01,01),VisaType="D08"},
                new VisaRegistrationDate {EmployeeID=2, RegistrationDate=new DateTime(2013,10,02),VisaType="C07"},   
                new VisaRegistrationDate {EmployeeID=3, RegistrationDate=new DateTime(2013,01,01),VisaType="C07"},
                new VisaRegistrationDate {EmployeeID=4, RegistrationDate=new DateTime(2013,01,04),VisaType="D08"}

            };

            List<Visa> visas = new List<Visa>
            {
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,11,02), Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 },
                new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 }
            };

            List<Permit> permits = new List<Permit>
            {
                new Permit { EmployeeID = 1, Number = "04/2012", StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},
                new Permit { EmployeeID = 2, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 2)},
                new Permit { EmployeeID = 3, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 3)}
            };

            mock.Setup(m => m.Departments).Returns(departments);
            mock.Setup(m => m.Employees).Returns(employees);
            mock.Setup(m => m.Locations).Returns(locations);
            mock.Setup(m => m.Visas).Returns(visas);
            mock.Setup(m => m.VisaRegistrationDates).Returns(visaRegistrationDates);
            mock.Setup(m => m.Permits).Returns(permits);
            mock.Setup(m => m.BusinessTrips).Returns(businessTrips);

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


            businessTrips.Find(b => b.BusinessTripID == 1).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 2).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 3).Location = (locations.Find(l => l.LocationID == 2));
            businessTrips.Find(b => b.BusinessTripID == 4).Location = (locations.Find(l => l.LocationID == 2));
            businessTrips.Find(b => b.BusinessTripID == 5).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 6).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 7).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 8).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 9).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 10).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 11).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 12).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 13).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 14).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 15).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 16).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 17).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 18).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 19).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 20).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 21).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 22).Location = (locations.Find(l => l.LocationID == 1));

            businessTrips.Find(b => b.BusinessTripID == 1).BTof = (employees.Find(l => l.EmployeeID == 1));
            businessTrips.Find(b => b.BusinessTripID == 2).BTof = (employees.Find(l => l.EmployeeID == 2));
            businessTrips.Find(b => b.BusinessTripID == 3).BTof = (employees.Find(l => l.EmployeeID == 3));
            businessTrips.Find(b => b.BusinessTripID == 4).BTof = (employees.Find(l => l.EmployeeID == 3));
            businessTrips.Find(b => b.BusinessTripID == 5).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 6).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 7).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 8).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 9).BTof = (employees.Find(l => l.EmployeeID == 5));
            businessTrips.Find(b => b.BusinessTripID == 10).BTof = (employees.Find(l => l.EmployeeID == 6));
            businessTrips.Find(b => b.BusinessTripID == 11).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 12).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 13).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 14).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 15).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 16).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 17).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 18).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 19).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 20).BTof = (employees.Find(l => l.EmployeeID == 2));
            businessTrips.Find(b => b.BusinessTripID == 21).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 22).BTof = (employees.Find(l => l.EmployeeID == 5));
            businessTrips.Find(b => b.BusinessTripID == 23).BTof = (employees.Find(l => l.EmployeeID == 5));
            businessTrips.Find(b => b.BusinessTripID == 24).BTof = (employees.Find(l => l.EmployeeID == 5));

            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 1));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 2));
            locations.Find(l => l.LocationID == 2).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 3));
            locations.Find(l => l.LocationID == 2).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 4));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 5));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 6));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 7));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 8));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 9));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 10));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 11));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 12));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 13));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 14));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 15));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 16));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 17));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 18));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 19));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 20));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 21));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 22));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 23));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 24));

            employees.Find(e => e.EmployeeID == 1).Visa = visas.Find(v => v.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).Visa = visas.Find(v => v.EmployeeID == 2);

            visas.Find(v => v.EmployeeID == 1).VisaOf = employees.Find(e => e.EmployeeID == 1);
            visas.Find(v => v.EmployeeID == 2).VisaOf = employees.Find(e => e.EmployeeID == 2);

            employees.Find(e => e.EmployeeID == 1).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 2);
            employees.Find(e => e.EmployeeID == 3).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 3);
            employees.Find(e => e.EmployeeID == 4).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 4);

            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 1).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 1);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 2).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 2);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 3).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 3);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 4).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 4);

            employees.Find(e => e.EmployeeID == 1).Permit = permits.Find(p => p.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).Permit = permits.Find(p => p.EmployeeID == 2);
            employees.Find(e => e.EmployeeID == 3).Permit = permits.Find(p => p.EmployeeID == 3);
        }

        [Test]
        public void MessageConstructor_NoParameters_EmptyMessage()
        {
            //Arrange

            //Act
            Message message = new Message();

            //Assert        
            Assert.AreEqual(null, message.Author);
            Assert.AreEqual(null, message.Body);
            Assert.AreEqual(null, message.BTList);
            Assert.AreEqual(null, message.Link);
            Assert.AreEqual(0, message.MessageID);
            Assert.IsNullOrEmpty(message.FullName);
            Assert.AreEqual(MessageType.UnknownType, message.messageType);
            Assert.AreEqual(null, message.ReplyTo);
            Assert.AreEqual(null, message.Role);
            Assert.AreEqual(null, message.Subject);
            Assert.AreEqual(new DateTime(), message.TimeStamp);
        }

        [Test]
        public void MessageConstructor_AllParametersAreSet_ProperMessage()
        {
            //Arrange

            //Act
            Employee author = mock.Object.Employees.FirstOrDefault();
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Take(2).ToList(), author);
            string messageBody =
                "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
                "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>" +
                "<b>Struz</b> <b>Anatoliy</b> (<b>ascr</b>), TAAAA, <b>LDF</b><b>, 01.11.2014 - </b><b>10.11.2014</b><br/><br/>";

            //Assert        
            Assert.AreEqual(mock.Object.Employees.FirstOrDefault(), message.Author);
            Assert.AreEqual(messageBody, message.Body);
            Assert.AreEqual(mock.Object.BusinessTrips.Take(2).ToList(), message.BTList);
            Assert.AreEqual(message.GetLink(), message.Link);
            Assert.AreEqual(0, message.MessageID);
            Assert.IsNullOrEmpty(message.FullName);
            Assert.AreEqual(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, message.messageType);
            Assert.AreEqual("andl@elegant.com", message.ReplyTo);
            Assert.AreEqual(message.GetRole(), message.Role);
            Assert.AreEqual(message.GetSubject(), message.Subject);
            Assert.AreNotEqual(message.TimeStamp, new DateTime());
            Assert.LessOrEqual(message.TimeStamp, DateTime.Now.ToLocalTimeAzure());
        }

        [Test]
        public void MessageConstructor_GreetingOverload_ProperGreetingMessage()
        {
            //Arrange

            //Act
            Employee employee = mock.Object.Employees.FirstOrDefault(); 

            string messageBody = "happyBirthday!";


            Message message = new Message(messageBody, "andl", employee, "Happy Birthday!"); 

            //Assert        
            Assert.AreEqual(null, message.Author);

            Assert.AreEqual("Header<br/><b>Джонні Роус</b><br/><br/><a href='http://andl.jpg'>Фото</a> <a href='http://?uid=andl'>Лінк на профіль</a><br/><br/>happyBirthday!<br/><br/>Footer", message.Body);
            Assert.AreEqual(null, message.BTList);
            Assert.AreEqual("", message.Link);
            Assert.AreEqual(0, message.MessageID);
            Assert.AreEqual("Anastasia Zarose", message.FullName);
            Assert.AreEqual(MessageType.Greeting, message.messageType);
            Assert.AreEqual("andl", message.ReplyTo);
            Assert.AreEqual("", message.Role);
            Assert.AreEqual("Happy Birthday, Anastasia!", message.Subject);
            Assert.LessOrEqual(message.TimeStamp, DateTime.Now.ToLocalTimeAzure());
            Assert.AreEqual(employee, message.employee); 
        }  

        [Test]
        public void MessageConstructor_ResetPasswordOverload_ProperGreetingMessage()
        {
            //Arrange

            //Act  
            Employee employee = mock.Object.Employees.FirstOrDefault();

            string subject = "Reset password";
            string messageBody = "Password Reseted!"; 


            Message message = new Message(subject, messageBody, employee);

            //Assert        
            Assert.AreEqual(null, message.Author);
            Assert.AreEqual("Password Reseted!", message.Body);    
            Assert.AreEqual(null, message.BTList);       
            Assert.AreEqual("", message.Link);
            Assert.AreEqual(0, message.MessageID);
            Assert.AreEqual("Anastasia Zarose", message.FullName);
            Assert.AreEqual(MessageType.ResetPassword, message.messageType);
            Assert.AreEqual(WebConfigurationManager.AppSettings["SystemNetMailReplyToDefaultValue"], message.ReplyTo);
            Assert.AreEqual("", message.Role);
            Assert.AreEqual("Reset password", message.Subject);
            Assert.LessOrEqual(message.TimeStamp, DateTime.Now.ToLocalTimeAzure());
            Assert.AreEqual(employee, message.employee);
        }  

        [Test]
        [TestCase(MessageType.UnknownType, Result = "Unknown Subject")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, Result = "For BTM: BT Confirmation")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, Result = "For DIR: BT Confirmation")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, Result = "For EMP: BT Confirmation")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToACC, Result = "For ACC: BT Confirmation")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, Result = "For BTM: BT Registration")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, Result = "For EMP: BT Registration")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, Result = "For ACC: BT Registration")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, Result = "For BTM: BT Replanning")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, Result = "For ACC: BT Replanning")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, Result = "For BTM: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, Result = "For EMP: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, Result = "For ACC: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, Result = "For BTM: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, Result = "For DIR: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, Result = "For EMP: BT Cancellation")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, Result = "For ACC: BT Cancellation")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, Result = "For EMP: BT Update")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC, Result = "For ACC: BT Update")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, Result = "For ACC: BT Report")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, Result = "For EMP: BT Report")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, Result = "For ACC: BT Report")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, Result = "For ADM: BT Rejection")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, Result = "For ACC: BT Rejection")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, Result = "For ADM: BT Rejection")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, Result = "For EMP: BT Rejection")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, Result = "For ACC: BT Rejection")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToADM, Result = "For ADM: BT Cancellation")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToBTM, Result = "For BTM: BT Cancellation")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToEMP, Result = "For EMP: BT Cancellation")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToADM, Result = "For ADM: BT Update")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToBTM, Result = "For BTM: BT Update")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToDIR, Result = "For DIR: BT Update")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToEMP, Result = "For EMP: BT Update")]
        [TestCase(MessageType.DIRRejectsConfirmedToADM, Result = "For ADM: BT Rejection")]
        [TestCase(MessageType.DIRRejectsConfirmedToEMP, Result = "For EMP: BT Rejection")]
        [TestCase(MessageType.DIRRejectsConfirmedToACC, Result = "For ACC: BT Rejection")]
        [TestCase(MessageType.BTMCancelsPermitToADM, Result = "For ADM: Permit Cancellation")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToBTM, Result = "For BTM: Planned Modified BT Cancellation")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToACC, Result = "For ACC: Planned Modified BT Cancellation")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, Result = "BT Confirmation")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, Result = "BT Cancellation")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToResponsible, Result = "BT Cancellation")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, Result = "BT Rejection")]
        [TestCase(MessageType.DIRRejectsConfirmedToResponsible, Result = "BT Rejection")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, Result = "BT Update")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, Result = "BT Report")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToResponsible, Result = "BT Update")]
        [TestCase(MessageType.BTMUpdateVisaRegistrationDateToEMP, Result = "Visa Registration Date Update")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToEMP, Result = "Visa Registration Date Creation")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToBTM, Result = "Visa Registration Date Creation")]
        [TestCase(MessageType.PUEditsFInishedBT, Result = "Finished BT Modification")]

        public String GetSubjectTest_MessageType_SubjectStringAccordingToType(MessageType messageType)
        {
            //Arrange
            Message message = new Message(messageType, null, null);

            //Act
            return message.GetSubject();

            //Assert             
        }

        [Test]
        [TestCase(MessageType.UnknownType, Result = "Unknown Role")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, Result = "DIR")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, Result = "EMP")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToACC, Result = "ACC")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, Result = "DIR")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, Result = "ADM")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, Result = "ADM")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, Result = "EMP")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToADM, Result = "ADM")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToBTM, Result = "BTM")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToEMP, Result = "EMP")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToADM, Result = "ADM")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToBTM, Result = "BTM")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToDIR, Result = "DIR")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToEMP, Result = "EMP")]
        [TestCase(MessageType.DIRRejectsConfirmedToADM, Result = "ADM")]
        [TestCase(MessageType.DIRRejectsConfirmedToEMP, Result = "EMP")]
        [TestCase(MessageType.DIRRejectsConfirmedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMCancelsPermitToADM, Result = "ADM")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToACC, Result = "ACC")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToEMP, Result = "EMP")]
        [TestCase(MessageType.BTMUpdateVisaRegistrationDateToEMP, Result = "EMP")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToBTM, Result = "BTM")]
        [TestCase(MessageType.BTMUpdateVisaRegistrationDateToBTM, Result = "BTM")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.DIRRejectsConfirmedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToResponsible, Result = "Unknown Role")]
        [TestCase(MessageType.PUEditsFInishedBT, Result = "Unknown Role")]
        public String GetRoleIDTest_MessageType_RoleAccordingToType(MessageType messageType)
        {
            //Arrange
            Message message = new Message(messageType, null, null);

            //Act
            return message.GetRole();

            //Assert             
        }

        [Test]
        [TestCase(MessageType.UnknownType, Result = "")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, Result = "<a href=\"http://localhost:50616/Home/DIRView/?tab=0\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, Result = "")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToACC, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        //[TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, Result = "")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, Result = "")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, Result = "")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, Result = "")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, Result = "")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, Result = "")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, Result = "")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, Result = "<a href=\"http://localhost:50616/Home/ACCView/?tab=0\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, Result = "<a href=\"http://localhost:50616/Home/EMPView/?tab=0\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, Result = "")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, Result = "")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, Result = "")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToEMP, Result = "")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToDIR, Result = "<a href=\"http://localhost:50616/Home/DIRView/?tab=0\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToEMP, Result = "")]
        [TestCase(MessageType.DIRRejectsConfirmedToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.DIRRejectsConfirmedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.DIRRejectsConfirmedToEMP, Result = "")]
        [TestCase(MessageType.DIRRejectsConfirmedToACC, Result = "")]
        [TestCase(MessageType.BTMCancelsPermitToADM, Result = "<a href=\"http://localhost:50616/Home/ADMView/?tab=0\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToACC, Result = "")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, Result = "")]
        [TestCase(MessageType.DIRRejectsConfirmedToResponsible, Result = "")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToResponsible, Result = "<a href=\"http://localhost:50616/Home/VUView/?tab=2\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToEMP, Result = "<a href=\"http://localhost:50616/Home/EMPView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMUpdateVisaRegistrationDateToEMP, Result = "<a href=\"http://localhost:50616/Home/EMPView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMCreateVisaRegistrationDateToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.BTMUpdateVisaRegistrationDateToBTM, Result = "<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>")]
        [TestCase(MessageType.PUEditsFInishedBT, Result = "")]
        public String GetLinkTest_MessageType_LinkAccordingToType(MessageType messageType)
        {
            //Arrange
            Message message = new Message(messageType, null, null);

            //Act
            return message.GetLink();

            //Assert             
        }

        #region GetBTTemplateTest
        [Test]
        public void GetBTTemplate_AllFieldsNoHabitation_ProperStringNoHabitation()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[0], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoLastName_ProperStringNoHabitationNoLastName()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.BTof.LastName = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[1], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoFirstName_ProperStringNoHabitationNoFirstName()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.BTof.FirstName = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[2], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoEID_ProperStringNoHabitationNoEID()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.BTof.EID = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[3], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoManager_ProperStringNoHabitationNoManager()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Manager = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[4], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoResponsible_ProperStringNoHabitationNoResponsible()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Responsible = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[5], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoLocationTitle_ProperStringNoHabitationNoResponsibleNoLocationTitle()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Location.Title = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[6], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsNoHabitationNoPurpose_ProperStringNoHabitationNoPurpose()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Purpose = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[7], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationNotConfirmed_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(BTTemplates[8], result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationConfirmed_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);
            string btTemplate = "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
                + "<br/>" + "<b>Habitation (confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel";

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            businessTrip.HabitationConfirmed = true;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(btTemplate, result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationConfirmedFlightsNotConfirmed_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);
            string btTemplate = "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
    + "<br/>" + "<b>Habitation (confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel<br/><b>Flights (not confirmed):</b> Kyiv - Warshawa";

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            businessTrip.HabitationConfirmed = true;
            businessTrip.Flights = "Kyiv - Warshawa";
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(btTemplate, result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationConfirmedFlightsConfirmed_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);
            string btTemplate = "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
    + "<br/>" + "<b>Habitation (confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel<br/><b>Flights (confirmed):</b> Kyiv - Warshawa";

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            businessTrip.HabitationConfirmed = true;
            businessTrip.Flights = "Kyiv - Warshawa";
            businessTrip.FlightsConfirmed = true;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(btTemplate, result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationConfirmedFlightsConfirmedInvitation_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);
            string btTemplate = "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
    + "<br/>" + "<b>Habitation (confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel<br/><b>Flights (confirmed):</b> Kyiv - Warshawa<br/><b>Invitation:</b> confirmed";

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            businessTrip.HabitationConfirmed = true;
            businessTrip.Flights = "Kyiv - Warshawa";
            businessTrip.FlightsConfirmed = true;
            businessTrip.Invitation = true;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(btTemplate, result);
        }

        [Test]
        public void GetBTTemplate_AllFieldsHabitationConfirmedFlightsConfirmedInvitationAllComments_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);
            string btTemplate = "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>"
    + "<br/>" + "<b>Habitation (confirmed):</b> Krakow, Poland, ul. Wroclawska 5a, Home&Travel<br/><b>Flights (confirmed):</b> Kyiv - Warshawa<br/><b>Invitation:</b> confirmed" +
    "<br/><b>Comment:</b> Comment added<br/>" +
    "<b>BTM comment:</b> BTM comment added<br/>" +
    "<b>Reject comment:</b> Reject comment added<br/>" +
       "<b>Cancel comment:</b> Cancel comment added";

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.Habitation = "Krakow, Poland, ul. Wroclawska 5a, Home&Travel";
            businessTrip.HabitationConfirmed = true;
            businessTrip.Flights = "Kyiv - Warshawa";
            businessTrip.FlightsConfirmed = true;
            businessTrip.Invitation = true;
            businessTrip.Comment = "Comment added";
            businessTrip.CancelComment = "Cancel comment added";
            businessTrip.RejectComment = "Reject comment added";
            businessTrip.BTMComment = "BTM comment added";
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual(btTemplate, result);
        }

        [Test]
        public void GetBTTemplate_BTAllFieldsAreNull_EmptyString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            string result = message.GetBTTemplate(new BusinessTrip());

            //Assert        
            Assert.AreEqual("<b>, 01.01.0001 - </b><b>01.01.0001</b>", result);
        }

        [Test]
        public void GetBTTemplate_BT_DepartmentNameIsNull_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.BTof.Department.DepartmentName = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>", result);
        }

        [Test]
        public void GetBTTemplate_BT_DepartmentIsNull_ProperString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            businessTrip.BTof.Department = null;
            string result = message.GetBTTemplate(businessTrip);

            //Assert        
            Assert.AreEqual("<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>", result);
        }

        [Test]
        public void GetBTTemplate_BTNull_EmptyString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();
            string result = message.GetBTTemplate(null);

            //Assert        
            Assert.AreEqual("", result);
        }

        #endregion

        [Test]
        [TestCase(MessageType.UnknownType, Result = "Unknown Message Type by Anastasia Zarose at ")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, Result = "<b>BT confirmation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToDIR, Result = "<b>BT confirmation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToEMP, Result = "<b>BT confirmation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToACC, Result = "<b>BT confirmation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM, Result = "<b>BT registration</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP, Result = "<b>BT registration</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC, Result = "<b>BT registration</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM, Result = "<b>BT replanning</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC, Result = "<b>BT replanning</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM, Result = "<b>BT cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP, Result = "<b>BT cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC, Result = "<b>BT cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, Result = "<b>BT(s) cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR, Result = "<b>BT(s) cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP, Result = "<b>BT(s) cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, Result = "<b>BT(s) cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP, Result = "<b>BT update</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC, Result = "<b>BT update</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC, Result = "<b>BT(s) report</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP, Result = "<b>BT(s) report</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToADM, Result = "<b>BT cancellation</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToBTM, Result = "<b>BT cancellation</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToEMP, Result = "<b>BT cancellation</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToADM, Result = "<b>BT modification</b> by ACC Anastasia Zarose at ")]
        //[TestCase(MessageType.ACCModifiesConfirmedReportedToBTM, Result = "<b>BT modification</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToDIR, Result = "<b>BT modification</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToEMP, Result = "<b>BT modification</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.DIRRejectsConfirmedToADM, Result = "<b>BT rejection</b> by DIR Anastasia Zarose at ")]
        [TestCase(MessageType.DIRRejectsConfirmedToEMP, Result = "<b>BT rejection</b> by DIR Anastasia Zarose at ")]
        [TestCase(MessageType.DIRRejectsConfirmedToBTM, Result = "<b>BT rejection</b> by DIR Anastasia Zarose at ")]
        [TestCase(MessageType.DIRRejectsConfirmedToACC, Result = "<b>BT rejection</b> by DIR Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToBTM, Result = "<b>Planned modified BT cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsPlannedModifiedToACC, Result = "<b>Planned modified BT cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMConfirmsPlannedOrRegisteredToResponsible, Result = "<b>BT confirmation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, Result = "<b>BT(s) cancellation</b> by ADM Anastasia Zarose at ")]
        [TestCase(MessageType.ACCCancelsConfirmedReportedToResponsible, Result = "<b>BT cancellation</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible, Result = "<b>BT update</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible, Result = "<b>BT(s) report</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.ACCModifiesConfirmedReportedToResponsible, Result = "<b>BT modification</b> by ACC Anastasia Zarose at ")]
        [TestCase(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible, Result = "<b>BT rejection</b> by BTM Anastasia Zarose at ")]
        [TestCase(MessageType.DIRRejectsConfirmedToResponsible, Result = "<b>BT rejection</b> by DIR Anastasia Zarose at ")]
        [TestCase(MessageType.PUEditsFInishedBT, Result = "<b>Finished BT Modification</b> by PU Anastasia Zarose at ")]
        public String GetMessageTemplate_MessageType_MessageTemplateAccordingToType(MessageType messageType)
        {
            //Arrange
            Message message = new Message(messageType, null, mock.Object.Employees.FirstOrDefault());

            //Act
            string messageTemplate = message.GetMessageTemplate().Remove(message.GetMessageTemplate().Length - 19, 19);

            //Assert              
            Assert.IsTrue(Regex.IsMatch(message.GetMessageTemplate().Substring(message.GetMessageTemplate().Length - 20), @"\d\d\.\d\d\.\d\d\d\d \d\d:\d\d:\d\d"));

            return messageTemplate;
        }



        [Test]
        public void GetBTTemplate_ACCModifiesConfirmedReportedToBTM_FormattedString()
        {
            //Arrange
            Message message = new Message(MessageType.ACCModifiesConfirmedReportedToBTM, null, mock.Object.Employees.FirstOrDefault());

            //Act
            string result = "<b>BT modification</b> by ACC Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "</br><b>Please check Order Dates!</b></br>If BT is composed of several parts to different locations their <b>Order Dates should be the same.</b>";

            //Assert        
            Assert.AreEqual(message.GetMessageTemplate(), result);
        }


        [Test]
        public void GetMessageTemplate_MessageTypeBTMCreateVisaRegistrationDateToEMP_CorrectMessageTemplate()
        {
            //Arrange
            Employee employeeForVisaRegDate = mock.Object.Employees.FirstOrDefault();
            Message message = new Message(MessageType.BTMCreateVisaRegistrationDateToEMP, null, mock.Object.Employees.FirstOrDefault(), employeeForVisaRegDate);

            string messageTemplate = string.Format("<b>Visa Registration Date Creation</b> by BTM {0} {1} at {2}", message.Author.FirstName, message.Author.LastName,
                           message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +"<br/>"
                            + string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}</br></br>"
                            + message.ReplaceURLWithHyperlink(WebConfigurationManager.AppSettings["VisaRegInfo"]), employeeForVisaRegDate.VisaRegistrationDate.VisaType, employeeForVisaRegDate.VisaRegistrationDate.RegistrationDate, employeeForVisaRegDate.VisaRegistrationDate.RegistrationTime, employeeForVisaRegDate.VisaRegistrationDate.City, employeeForVisaRegDate.VisaRegistrationDate.RegistrationNumber);

            //Act

            //Assert

            Assert.AreEqual(messageTemplate, message.GetMessageTemplate());
        }

        [Test]
        public void GetMessageTemplate_MessageTypeBTMUpdateVisaRegistrationDateToEMP_CorrectMessageTemplate()
        {
            //Arrange

            Employee employeeForVisaRegDate = mock.Object.Employees.FirstOrDefault();

            Message message = new Message(MessageType.BTMUpdateVisaRegistrationDateToEMP, null, mock.Object.Employees.FirstOrDefault(), employeeForVisaRegDate);

            string messageTemplate = string.Format("<b>Visa Registration Date Update</b> by BTM {0} {1} at {2}", message.Author.FirstName, message.Author.LastName,
                            message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                            "<br/>" + string.Format("Visa Type: {0} Date: {1:dd.MM.yyyy} Time: {2:dd.MM.yyyy} City: {3} Reg.Num: {4}</br></br>" 
                            + message.ReplaceURLWithHyperlink(WebConfigurationManager.AppSettings["VisaRegInfo"]), employeeForVisaRegDate.VisaRegistrationDate.VisaType, employeeForVisaRegDate.VisaRegistrationDate.RegistrationDate, employeeForVisaRegDate.VisaRegistrationDate.RegistrationTime, employeeForVisaRegDate.VisaRegistrationDate.City, employeeForVisaRegDate.VisaRegistrationDate.RegistrationNumber);

            //Act

            //Assert

            Assert.AreEqual(messageTemplate, message.GetMessageTemplate());
        }


        [Test]
        public void GetMessageTemplate_MessageTypeBTMCancellsPemitToADM_MessageTemplate()
        {
            //Arrange
            Employee employeeForPermit = mock.Object.Employees.FirstOrDefault();
            employeeForPermit.Permit.CancelRequestDate = DateTime.Now.ToLocalTimeAzure();
            Message message = new Message(MessageType.BTMCancelsPermitToADM, null, mock.Object.Employees.FirstOrDefault(), employeeForPermit);

            string messageTemplate = string.Format("<b>Permit cancellation</b> by BTM {0} {1} at {2}", message.Author.FirstName, message.Author.LastName, message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss")) +
                           "<br/>" +
                           string.Format("Cancel for permit of {0} {1} ({2}) with dates {3:dd.MM.yyyy} - {4:dd.MM.yyyy} requested at {5:dd.MM.yyyy}", employeeForPermit.FirstName, employeeForPermit.LastName, employeeForPermit.EID,
                           employeeForPermit.Permit.StartDate, employeeForPermit.Permit.EndDate, employeeForPermit.Permit.CancelRequestDate);

            //Act

            //Assert              
            Assert.AreEqual(messageTemplate, message.GetMessageTemplate());
        }


        [Test]
        public void GetMessageTemplate_authorIsNull_stringIsEmpty()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, null);

            //Act
            string result = message.GetMessageTemplate();

            //Assert           
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetMessageTemplate_authorFieldsAreNull_AuthorDataNotAddedToString()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, new Employee());

            //Act
            string result = message.GetMessageTemplate();

            //Assert           
            Assert.AreEqual("<b>BT confirmation</b> by ADM   at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"), result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListNull_MessageTemplateOnly()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, null, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
     "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss");
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListEmpty_MessageTemplateOnly()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip>(), mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
     "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBT_MessageBody()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Take(1).ToList(), mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsMoreThanOneBT_MessageBody()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Take(2).ToList(), mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
     "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
     "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>" +
     "<b>Struz</b> <b>Anatoliy</b> (<b>ascr</b>), TAAAA, <b>LDF</b><b>, 01.11.2014 - </b><b>10.11.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_AuthorIsNullBusinessTripListContainsMoreThanOneBT_BTTemplatesOnly()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Take(2).ToList(), null);
            string excpectedResult =
               "<br/><br/><b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>" +
                "<b>Struz</b> <b>Anatoliy</b> (<b>ascr</b>), TAAAA, <b>LDF</b><b>, 01.11.2014 - </b><b>10.11.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_AuthorFieldsAreNullBusinessTripListContainsMoreThanOneBT_MessageBodyWithoutAuthorData()
        {
            //Arrange
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, mock.Object.BusinessTrips.Take(2).ToList(), new Employee());
            string excpectedResult =
                "<b>BT confirmation</b> by ADM   at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
                "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>" +
                "<b>Struz</b> <b>Anatoliy</b> (<b>ascr</b>), TAAAA, <b>LDF</b><b>, 01.11.2014 - </b><b>10.11.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.Comment = "Comment added";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><b>Comment:</b> Comment added<br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTRejectCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.RejectComment = "Reject comment added";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><b>Reject comment:</b> Reject comment added<br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTCancelCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.CancelComment = "Cancel comment added";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><b>Cancel comment:</b> Cancel comment added<br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTOrderDatesAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.OrderStartDate = bTrip.StartDate.AddDays(-1);
            bTrip.OrderEndDate = bTrip.EndDate.AddDays(1);
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b>, Order dates: 30.11.2014 - 11.12.2014<br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTEmptyCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.Comment = "";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }
        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTEmptyRejectCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.RejectComment = "";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTEmptyCancelCommentAdded_MessageBody()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.CancelComment = "";
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTOrderStartDateOnlyWasAdded_MessageBodyWithoutOrderDates()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.OrderStartDate = bTrip.StartDate.AddDays(-1);
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        [Test]
        public void GetBodyTest_BusinessTripListContainsOneBTOrderEndDateOnlyWasAdded_MessageBodyWithoutOrderDates()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Take(1).FirstOrDefault();
            bTrip.OrderEndDate = bTrip.EndDate.AddDays(1);
            Message message = new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, new List<BusinessTrip> { bTrip }, mock.Object.Employees.FirstOrDefault());
            string excpectedResult =
      "<b>BT confirmation</b> by ADM Anastasia Zarose at " + message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss") + "<br/><br/>" +
      "<b>Zarose</b> <b>Anastasia</b> (<b>andl</b>), SDDDA, xtwe, wiza, Work in BD2, <b>LDF</b><b>, 01.12.2014 - </b><b>10.12.2014</b><br/><br/>";
            //Act
            string result = message.GetBody();

            //Assert  
            Assert.AreEqual(excpectedResult, result);
        }

        #region ReplaceURLWithHyperlink

        [Test]
        public void ReplaceURLWithHyperlink()
        { 
            //Arrange
            string inputString = "http://www.polandvisa-ukraine.com/bussiness_application.html   http://www.polandvisa-ukraine.com.html";
            string outputString = "<a href=\"http://www.polandvisa-ukraine.com/bussiness_application.html\">link</a>   <a href=\"http://www.polandvisa-ukraine.com.html\">link</a>";
           
            Message message = new Message();
            //Act
            string result = message.ReplaceURLWithHyperlink(inputString);
            
            //Assert
            Assert.AreEqual(outputString, result);
        }

        #endregion


        #region getUkName
        [Test]
        public void getUkName_FirstNameSurnameLastName_FirstNameSurname()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("Джонні Роус", result);
            
        }

        [Test]
        public void getUkName_Empty_EmptyString()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            emp.FullNameUk = "";
            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("", result);

        }

        [Test]
        public void getUkName_FirstName_FirstName()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            emp.FullNameUk = "Джонні";
            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("Джонні", result);

        }

        [Test]
        public void getUkName_FirstNameSurname_FirstNameSurname()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            emp.FullNameUk = "Джонні Роус";
            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("Джонні Роус", result);

        }

        [Test]
        public void getUkName_FullNameNull_EmptyString()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            emp.FullNameUk = null;
            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("", result);

        }

        [Test]
        public void getUkName_EmployeeNull_EmptyString()
        {
            //Arrange

            //Act

            Message msg = new Message();
            string result = msg.getUkName(null);

            //Assert        
            Assert.AreEqual("", result);

        }

        [Test]
        public void getUkName_FirstNameSurNameLastNameMoreThanOneWhiteSpaceInARow_EmptyString()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            emp.FullNameUk = "Джонні   Роус  Джонні    Роус";

            //Act

            Message msg = new Message();
            string result = msg.getUkName(emp);

            //Assert        
            Assert.AreEqual("Джонні Роус", result);

        } 

        #endregion

    }
}