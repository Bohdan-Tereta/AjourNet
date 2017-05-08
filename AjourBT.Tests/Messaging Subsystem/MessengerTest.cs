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
using System.Web.Security;
using AjourBT.Domain.Infrastructure;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net;

namespace AjourBT.Tests.Messaging_Subsystem
{
    [TestFixture]
    public class MessengerTest
    {

        List<string> BTTemplates;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            BTTemplates = new List<string>();
            BTTemplates.Add("Zarose Anastasia (andl), xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Anastasia (andl), xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose (andl), xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia, xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia (andl), wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia (andl), xtwe, LDF, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia (andl), xtwe, wiza, Work in BD2, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia (andl), xtwe, wiza, LDF, 01.12.2014 - 10.12.2014");
            BTTemplates.Add("Zarose Anastasia (andl), xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014"
                + Environment.NewLine + "Habitation: Krakow, Poland, ul. Wroclawska 5a, Home&Travel");
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
            }
        }

        Mock<IRepository> mockRepository;
        Mock<IMessage> mockMessage;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new Mock<IRepository>();

            List<Department> departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            List<Greeting> greetings = new List<Greeting>
                {
                    new Greeting{GreetingId = 1,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 2,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 3,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 4,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 5,GreetingHeader = "Greeting 5", GreetingBody = "Celebrate your birthday today. Celebrate being Happy every day"}
                };

            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true , IsGreetingMessageAllow = true}
             };

            List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            { 
                new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,10), Status= BTStatus.Planned, EmployeeID = 1, LocationID = 1,  Manager = "xtwe", Responsible = "wiza", Purpose = "Work in BD2", LastCRUDedBy ="andl" },
                new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2014,11,01), EndDate = new DateTime (2014,11,10), Status= BTStatus.Registered, EmployeeID = 2, LocationID = 1, LastCRUDedBy ="tadk" }, 
                new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014,10,20), EndDate = new DateTime (2014,10,30), Status= BTStatus.Confirmed, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014,10,01), EndDate = new DateTime (2014,10,12), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,10,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1, LastCRUDedBy ="ascr" },
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013,08,01), EndDate = new DateTime (2013,08,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1, LastCRUDedBy ="ascr" },
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
                new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>(), ResponsibleForLoc = "andl, tebl;xnta...daol.daol"}, 
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
                new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },               
                new Visa { EmployeeID = 3, VisaType = "Test",StartDate = DateTime.Today, DueDate = DateTime.Today.AddDays(90), DaysUsedInBT = 1, Entries = 1, EntriesUsedInBT = 1}
            };

            List<Permit> permits = new List<Permit>
            {
                new Permit { EmployeeID = 1, Number = "04/2012", StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},
                new Permit { EmployeeID = 2, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 2)},
                new Permit { EmployeeID = 3, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 3)}
            };

            mockRepository.Setup(m => m.Departments).Returns(departments);
            mockRepository.Setup(m => m.Employees).Returns(employees);
            mockRepository.Setup(m => m.Locations).Returns(locations);
            mockRepository.Setup(m => m.Visas).Returns(visas);
            mockRepository.Setup(m => m.VisaRegistrationDates).Returns(visaRegistrationDates);
            mockRepository.Setup(m => m.Permits).Returns(permits);
            mockRepository.Setup(m => m.BusinessTrips).Returns(businessTrips);
            mockRepository.Setup(m => m.Greetings).Returns(greetings); 

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
            employees.Find(e => e.EmployeeID == 3).Visa = visas.Find(v => v.EmployeeID == 3);

            visas.Find(v => v.EmployeeID == 1).VisaOf = employees.Find(e => e.EmployeeID == 1);
            visas.Find(v => v.EmployeeID == 2).VisaOf = employees.Find(e => e.EmployeeID == 2);
            visas.Find(v => v.EmployeeID == 3).VisaOf = employees.Find(e => e.EmployeeID == 3);

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

            mockMessage = new Mock<IMessage>();

            mockMessage.Setup(m => m.MessageID).Returns(0);
            mockMessage.Setup(m => m.Role).Returns("BTM");
            mockMessage.Setup(m => m.Subject).Returns("BT Confirmation");
            mockMessage.Setup(m => m.Body).Returns("BT confirmation by ADM Anastasia Zarose\r\n" +
                "Zarose Anastasia (andl), xtwe, wiza, LDF, Work in BD2, 01.12.2014 - 10.12.2014\r\n" +
                "Struz Anatoliy (ascr), LDF, 01.11.2014 - 10.11.2014");
            mockMessage.Setup(m => m.Link).Returns("<a href=\"http://localhost:50616/Home/BTMView/?tab=1\"> Goto Ajour page </a>");

            //mockRoleProvider = new Mock<IRoleProvider>();
            //mockRoleProvide.Setup(m => m.GetUsersInRole("BTM")).Returns(() => new string[] { "abc@def.com", "xyz@def.com" });
            //mockRoleProvide.Setup(m => m.GetUsersInRole("ADM")).Returns(() => new string[] { "tedk", "xomi" });
            //mockRoleProvide = new Mock<RoleProvider>();

            //mockRoleProvide.Setup(m => m.AddUsersToRoles(new string[] { "tedk", "xomi" }, new string[] { "ADM" }));
            //mockRoleProvide.Setup(m => m.IsUserInRole("tedk", "ADM")).Returns(() => true);
            //mockRoleProvide.Setup(m => m.IsUserInRole("xomi", "ADM")).Returns(() => true);
        }

        [Test]
        public void StoreMessage_NoParametersNoSubjects_SaveMessageToRepository()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            mockMessage.Setup(m => m.Role).Returns("Unknown Role");

            //Act
            messenger.StoreMessage(mockMessage.Object);

            //Assert   
            mockRepository.Verify(m => m.SaveMessage(mockMessage.Object), Times.Once);

        }

        [Test]
        public void StoreMessage_NoParameters_SaveMessageToRepository()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            messenger.StoreMessage(mockMessage.Object);

            //Assert   
            mockRepository.Verify(m => m.SaveMessage(mockMessage.Object), Times.Once);

        }

        [Test]
        public void GetMailingListForRole_RoleBTM_ListOfAllBTMMails()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM, null, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "abc@elegant.com", "xyz@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleACC_ListOfAllACCMails()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC, null, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "edc@elegant.com", "rfv@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleDIR_ListOfAllDIRMails()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToDIR, null, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "tgb@elegant.com", "yhn@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_MessageNull_EmptyStringArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            string[] result = messenger.GetMailingListForRole(null);

            //Assert        
            Assert.AreEqual(new string[0] { }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleEMPDiffernetEMPs_TwoMailsInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(1).FirstOrDefault());
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToEMP, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com", "ascr@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleEMPSameEMPs_OneMailInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.Skip(5).FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(4).FirstOrDefault());
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToEMP, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "tadk@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleEMPNoEMPs_EmptyStringArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToEMP, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[0] { }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleEMPNoBT_OneMailInList1234()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee author = (from emp in mockRepository.Object.Employees where emp.EID == "andl" select emp).FirstOrDefault();
            Employee receiver = (from emp in mockRepository.Object.Employees where emp.VisaRegistrationDate != null select emp).FirstOrDefault();
            Message msg = new Message(MessageType.BTMCreateVisaRegistrationDateToEMP, null, author, receiver);

            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert
            Assert.AreEqual(new string[] { "andl@elegant.com" }, result);
        }

        [Test]
        public void GetMailingListForRole_RoleADMDiffernetADMs_TwoMailsInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(1).FirstOrDefault());
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToADM, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com", "tadk@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleADMSameADMs_OneMailInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.Skip(5).FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(4).FirstOrDefault());
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToADM, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "ascr@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleADMNoBT_OneMailInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee emp = (from e in mockRepository.Object.Employees where e.EID == "andl" select e).FirstOrDefault();
            Employee visa = (from e in mockRepository.Object.Employees where e.VisaRegistrationDate != null select e).FirstOrDefault();

            //Act
            Message msg = new Message(MessageType.BTMUpdateVisaRegistrationDateToEMP, null, emp, visa);
            msg.Role = "ADM";


            string[] result = messenger.GetMailingListForRole(msg);

            //Assert
            Assert.AreEqual(new string[] { "tedk" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleADMNoADMs_EmptyStringArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            Message msg = new Message(MessageType.ACCModifiesConfirmedReportedToADM, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[0] { }, result);

        }

        [Test]
        public void GetMailingListForRole_GreetingMessage_EmployeeFromMessage()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message("", "", mockRepository.Object.Employees.FirstOrDefault(), "");
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_ResetPasswordMessage_EmployeeFromMessage()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message("", "", mockRepository.Object.Employees.FirstOrDefault());
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_VisaWarningMessageEmpIsManager_EmployeeFromMessage()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message("xnta",mockRepository.Object.Employees.LastOrDefault(),"Visa Expiration Warning");
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "xomi@elegant.com" }, result);
        }

        [Test]
        public void GetMailingListForRole_VisaWarningMessageEmpIsNotManager_EmployeeFromMessageAndManager()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Message msg = new Message("xnta", mockRepository.Object.Employees.FirstOrDefault(), "Visa Expiration Warning");
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com","xomi@elegant.com" }, result);
        }

        [Test]
        public void Notify_NoParameters_SendAndStore()
        {
            //Arrange

            //Act
            Messenger messenger = new Messenger(mockRepository.Object);
            messenger.Notify(mockMessage.Object);

            //Assert        
            mockRepository.Verify(m => m.SaveMessage(mockMessage.Object), Times.Once);
        }

        [Test]
        public void GetMailingListForRole_RoleUnknown_NoResponsiblesInLocation_EmptyArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.Skip(2).FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(3).FirstOrDefault());
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleUnknown_NoBTsInBTList_EmptyArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleUnknown_NullBTList_EmptyArray()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, null, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleUnknown_ResponsiblesInLocationPresent_ProperMailingList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            btList.Add(mockRepository.Object.BusinessTrips.Skip(5).FirstOrDefault());
            btList.Add(mockRepository.Object.BusinessTrips.Skip(4).FirstOrDefault());
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "andl@elegant.com", "tebl@elegant.com", "xnta@elegant.com", "daol@elegant.com" }, result);

        }

        [Test]
        public void GetMailingListForRole_RoleUnknown_NoResponsiblesInLocationBTHasResponsibles_ProperMailingList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            BusinessTrip bt1 = mockRepository.Object.BusinessTrips.Skip(2).FirstOrDefault();
            BusinessTrip bt2 = mockRepository.Object.BusinessTrips.Skip(3).FirstOrDefault();
            BusinessTrip bt3 = new BusinessTrip(bt2);
            bt1.Responsible = "xkdf";
            bt2.Responsible = "fksd";
            bt3.Responsible = " fkld ";
            btList.Add(bt1);
            btList.Add(bt2);
            btList.Add(bt3);
            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { "xkdf@elegant.com", "fksd@elegant.com", "fkld@elegant.com" }, result);

        }


        [Test]
        public void GetMailingListForRole_RoleUnknown_NoResponsiblesInLocationBTHasResponsiblesNotAllowedCharactersUsed_EmptyMailingList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            List<BusinessTrip> btList = new List<BusinessTrip>();
            BusinessTrip bt1 = mockRepository.Object.BusinessTrips.Skip(2).FirstOrDefault();
            BusinessTrip bt2 = mockRepository.Object.BusinessTrips.Skip(3).FirstOrDefault();
            BusinessTrip bt3 = new BusinessTrip(bt2);
            bt1.Responsible = "xk df";
            bt2.Responsible = "a.";
            bt3.Responsible = "bk..bk";

            btList.Add(bt1);
            btList.Add(bt2);
            btList.Add(bt3);

            Message msg = new Message(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsible, btList, null);
            //Act
            string[] result = messenger.GetMailingListForRole(msg);

            //Assert        
            Assert.AreEqual(new string[] { }, result);

        }

        [Test]
        public void GetMailingListForRole_PUEditsFInishedBT_NoMailInList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee emp = (from e in mockRepository.Object.Employees where e.EID == "andl" select e).FirstOrDefault();
            Employee visa = (from e in mockRepository.Object.Employees where e.VisaRegistrationDate != null select e).FirstOrDefault();

            //Act
            Message msg = new Message(MessageType.PUEditsFInishedBT, null, emp, visa);
            msg.Role = "ADM";


            string[] result = messenger.GetMailingListForRole(msg);

            //Assert
            Assert.AreEqual(new string[0], result);

        }


        [Test]
        public void GetRandomGreetingBody_NoParameters_RandomNotEmptyBody()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            string result = "";
            string previousResult = "";
            int coincidences = 0;
            int trials = 20;

            //Act
            for (int i = 0; i < trials - 1; i++)
            {
                previousResult = result;
                result = messenger.GetRandomGreetingBody();
                if (result.Equals(previousResult))
                    coincidences++;
                if (result.Equals(String.Empty))
                    throw new ArgumentException(i.ToString());
            }

            //Assert    
            Assert.Greater(trials, coincidences);

        }

        [Test]
        public void GetRandomGreetingBody_NoParametersNoGreetingsInRepository_EmptyString()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            string result = "";
            string previousResult = "";
            int coincidences = 0;
            int trials = 20;
            mockRepository.Setup(m => m.Greetings).Returns(new List<Greeting>());

            //Act
            for (int i = 0; i < trials; i++)
            {
                previousResult = result;
                result = messenger.GetRandomGreetingBody();
                if (result.Equals(previousResult))
                    coincidences++;
            }

            //Assert        
            Assert.AreEqual("", result);
            Assert.AreEqual(20, coincidences);

        }

        [Test]
        public void GetGreetingMessages_NoBirthdaysToday_EmptyIMessageList()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            List<IMessage> messages = messenger.GetGreetingMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     
            Assert.AreEqual(0, messages.Count());

        }

        [Test]
        public void GetGreetingMessages_BirthdaysToday_IMessageList()
        {
            //Arrange
            mockRepository = new Mock<IRepository>();
            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),
                    BirthDay = new DateTime(1980, DateTime.Now.ToLocalTimeAzure().Month, DateTime.Now.ToLocalTimeAzure().Day)},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy1", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = new DateTime(1980, DateTime.Now.ToLocalTimeAzure().Month, DateTime.Now.ToLocalTimeAzure().Day)}, 
                new Employee {EmployeeID = 3, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(1)}, 
                new Employee {EmployeeID = 4, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = DateTime.Now.ToLocalTimeAzure().AddMonths(1)},  
                new Employee {EmployeeID = 5, FirstName = "Anastasia2", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = null, DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),
                    BirthDay = new DateTime(1980, DateTime.Now.ToLocalTimeAzure().Month, DateTime.Now.ToLocalTimeAzure().Day)},
             };

            mockRepository.Setup(m => m.Employees).Returns(employees);
            mockRepository.Setup(m => m.Greetings).Returns(new List<Greeting>());

            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            List<IMessage> messages = messenger.GetGreetingMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     
            Assert.AreEqual(2, messages.Count());
            Assert.AreEqual("andl", messages[0].ReplyTo);
            Assert.AreEqual("Happy Birthday, Anatoliy1!", messages[0].Subject);
            Assert.AreEqual("Happy Birthday, Anastasia2!", messages[1].Subject);
        }
        
        [Test]
        public void SendGreetingMessages_NoBirthdaysToday_NoExceptions()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            messenger.SendGreetingMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     

        }

        [Test]
        public void SendGreetingMessages_BirthdaysToday_NoExceptions()
        {
            //Arrange
            mockRepository = new Mock<IRepository>();
            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),
                    BirthDay = new DateTime(1980, DateTime.Now.ToLocalTimeAzure().Month, DateTime.Now.ToLocalTimeAzure().Day)},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = new DateTime(1980, DateTime.Now.ToLocalTimeAzure().Month, DateTime.Now.ToLocalTimeAzure().Day)}, 
                new Employee {EmployeeID = 3, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = DateTime.Now.ToLocalTimeAzure().AddMonths(1)}, 
                new Employee {EmployeeID = 4, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), 
                    BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(1)},  
             };
            mockRepository.Setup(m => m.Employees).Returns(employees);

            Messenger messenger = new Messenger(mockRepository.Object);
            mockRepository.Setup(m => m.Greetings).Returns(new List<Greeting>());
            //Act
            messenger.SendGreetingMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     

        }


        #region GetVisaWarningMessages

        [Test]
        public void GetVisaWarningMessages_NoVisaExpirationToday_EmptyIMessageList()
        {
            //Arrange
            mockRepository = new Mock<IRepository>();
            List<Visa> visas = new List<Visa>
             {
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,11,02), Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 },
             };

            mockRepository.Setup(m => m.Visas).Returns(visas);
            mockRepository.Setup(m => m.Employees).Returns(new List<Employee>());

            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            List<IMessage> messages = messenger.GetVisaWarningMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     
            Assert.AreEqual(0, messages.Count());
        }

        [Test]
        public void GetVisaWarningMessages_VisaExpirationToday_IMessageList()
        {
            //Arrange

            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            List<IMessage> messages = messenger.GetVisaWarningMessages(DateTime.Now.ToLocalTimeAzure());

            //Assert     
            Assert.AreEqual(1, messages.Count());
            Assert.AreEqual("andl", messages[0].ReplyTo);
            Assert.AreEqual("Visa Expiration Warning", messages[0].Subject);
        }

        #endregion

        #region SendVisaWarningMessage

        [Test]
        public void SendVisaWarningMessage_NoWarningsToday_NoExceptions()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            messenger.SendVisaWarningMessage(DateTime.Now.ToLocalTimeAzure());
        }

        [Test]
        public void SendVisaWarningMessages_VisaExpirationToday_NoExceptions()
        {
            //Arrange
            mockRepository = new Mock<IRepository>();
            List<Visa> employees = new List<Visa>
             {
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,11,02), Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 },
             };

            mockRepository.Setup(m => m.Visas).Returns(employees);
            mockRepository.Setup(m => m.Employees).Returns(new List<Employee>());

            Messenger messenger = new Messenger(mockRepository.Object);
            //Act
            messenger.SendVisaWarningMessage(DateTime.Now.ToLocalTimeAzure());
        }

        #endregion

        #region GetBlindCopyMailingList
        [Test]
        public void GetBlindCopyMailingList_ProperEmployee_AllEmployeesExceptForAGivenOne()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee employee = mockRepository.Object.Employees.FirstOrDefault();
            Message msg = new Message("", "", employee, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert        
            Assert.AreEqual(new string[] { "ascr@elegant.com", "tedk@elegant.com", "tadk@elegant.com", 
                                           "daol@elegant.com", "tebl@elegant.com", "xtwe@elegant.com", "xomi@elegant.com" },
                            result);

        }

        public void GetBlindCopyMailingList_ProperEmployee_AllEmployeesExceptForDismissed()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee employee = mockRepository.Object.Employees.Skip(1).FirstOrDefault();
            employee.DateDismissed = DateTime.Now; 
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert        
            Assert.AreEqual(new string[] { "tedk@elegant.com", "tadk@elegant.com", 
                                           "daol@elegant.com", "tebl@elegant.com", "xtwe@elegant.com", "xomi@elegant.com" },
                            result);

        }

        [Test]
        public void GetBlindCopyMailingList_MessageNotNullEmployeeNull_AllEmployees()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee employee = mockRepository.Object.Employees.FirstOrDefault();
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert        
            Assert.AreEqual(7, result.Length);

        }

        [Test]
        public void GetBlindCopyMailingList_MessageNull_NoEmployees()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            string[] result = messenger.GetBlindCopyMailingList(null);

            //Assert        
            Assert.AreEqual(0, result.Length);

        }

        [Test]
        public void GetBlindCopyMailingList_MessageNotNullEmployeeEIDNull_AllEmployees()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee employee = mockRepository.Object.Employees.FirstOrDefault();
            employee.EID = null;
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert        
            Assert.AreEqual(7, result.Length);

        }

        [Test]
        public void GetBlindCopyMailingList_MessageNotGreeting_NoEmployees()
        {
            //Arrange
            Messenger messenger = new Messenger(mockRepository.Object);
            Employee employee = mockRepository.Object.Employees.FirstOrDefault();
            employee.EID = null;
            Message msg = new Message("", "", null);
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert        
            Assert.AreEqual(0, result.Length);

        }

        [Test]
        public void GetBlindCopyMailingList_IsGreetingAllowedFalseForAllUsers_EmptyMailingList()
        {
            //Arrange
            mockRepository = new Mock<IRepository>();
            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true , IsGreetingMessageAllow = false}
             };
            mockRepository.Setup(m => m.Employees).Returns(employees);


            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void GetBlindCopyMailingList_IsGreetingAllowedTrueForFiveUsers_NotEmptyMailingList()
        {
            mockRepository = new Mock<IRepository>();
            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = false},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true , IsGreetingMessageAllow = true}
             };
            mockRepository.Setup(m => m.Employees).Returns(employees);


            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(new string[] {     "tedk@elegant.com", "daol@elegant.com", 
                                           "xtwe@elegant.com", "xomi@elegant.com" },
                            result);
        }

        [Test]
        public void GetBlindCopyMailingList_IsGreetingAllowedTrueForAllUsers_NotEmptyMailingList()
        {
            mockRepository = new Mock<IRepository>();
            List<Employee> employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(11/02/2011), IsManager = true , IsGreetingMessageAllow = true}
             };
            mockRepository.Setup(m => m.Employees).Returns(employees);


            Messenger messenger = new Messenger(mockRepository.Object);

            //Act
            Message msg = new Message("", "", null, "");
            string[] result = messenger.GetBlindCopyMailingList(msg);

            //Assert
            Assert.AreEqual(7, result.Length);
            Assert.AreEqual(new string[] { "ascr@elegant.com", "tedk@elegant.com",
                                           "tadk@elegant.com", "daol@elegant.com", "tebl@elegant.com",
                                           "xtwe@elegant.com", "xomi@elegant.com" },
                            result);
        }

        #endregion

        #region CreateSytemNetMailMessage 

        [Test]
        public void CreateSytemNetMailMessage_AllParametersAreNull_NoException()
        {
            //Arrange 
            string[] mailingList = null;
            string[] blindmailingList = null; 
            Message message = null; 

            //Act
            MailMessage msg = Messenger.CreateSytemNetMailMessage(mailingList, blindmailingList, message);

            //Assert        
            
        }

        [Test]
        public void CreateSytemNetMailMessage_AllParametersAreDefault_DefaultMessage()
        {
            //Arrange 
            string[] mailingList = new string[0];
            string[] blindmailingList = new string[0];
            Message message = new Message();

            //Act
            MailMessage msg = Messenger.CreateSytemNetMailMessage(mailingList, blindmailingList, message);

            //Assert        
            Assert.AreEqual(new MailAddress("abcd@elegant.com"), msg.From);
            Assert.AreEqual("", msg.Bcc);
            Assert.AreEqual("", msg.To);
            Assert.AreEqual("", msg.Subject);
            Assert.AreEqual("<br/>", msg.Body);
            Assert.AreEqual(true, msg.IsBodyHtml); 
        }

        [Test]
        public void CreateSytemNetMailMessage_AllParametersAreSet_ProperMessage()
        {
            //Arrange 
            string[] mailingList = {"abc@def.ghi", "123@def.ghi"};
            string[] blindmailingList = { "2@def.ghi", "1@def.ghi" };
            Message message = new Message();
            message.Body = "abc\ndef\n";
            message.Link = "ghijkl";
            message.Subject = "test subject"; 
            message.ReplyTo = "abc@def.ghi"; 

            //Act
            MailMessage msg = Messenger.CreateSytemNetMailMessage(mailingList, blindmailingList, message);

            //Assert        
            Assert.AreEqual(new MailAddress("abc@def.ghi"), msg.From.Address);
            Assert.AreEqual(2, msg.Bcc.Count());
            Assert.AreEqual("2@def.ghi", msg.Bcc[0].Address);
            Assert.AreEqual("1@def.ghi", msg.Bcc[1].Address);
            Assert.AreEqual(2, msg.To.Count());
            Assert.AreEqual("abc@def.ghi", msg.To[0].Address);
            Assert.AreEqual("123@def.ghi", msg.To[1].Address);
            Assert.AreEqual("test subject", msg.Subject);
            Assert.AreEqual("abc<br/>def<br/><br/>ghijkl", msg.Body);
            Assert.AreEqual(true, msg.IsBodyHtml);
        }

        #endregion 

        #region ConfigureSystemNetMailSMTPClient 
         //SmtpClient client = new SmtpClient();
         //   client.Host = WebConfigurationManager.AppSettings["SystemNetMailHost"];
         //   client.Port = Int32.Parse(WebConfigurationManager.AppSettings["SystemNetMailPort"]);
         //   if (WebConfigurationManager.AppSettings["SystemNetMailEnableSsl"].ToLower() == "true")
         //   {
         //       client.EnableSsl = true;
         //   }
         //   else
         //   {
         //       client.EnableSsl = false;
         //   }
         //   client.UseDefaultCredentials = false;
         //   client.Credentials = new NetworkCredential(
         //       WebConfigurationManager.AppSettings["SystemNetMailLogin"],
         //       WebConfigurationManager.AppSettings["SystemNetMailPassword"]);
         //   client.DeliveryMethod = SmtpDeliveryMethod.Network;
         //   return client;
        [Test]
        public void ConfigureSystemNetMailSMTPClient()
        {
             //Arrange 

             //Act
            SmtpClient smtpClient = Messenger.ConfigureSystemNetMailSMTPClient(); 

             //Assert
            Assert.AreEqual("ln1-lvz1.elegant.int", smtpClient.Host);
            Assert.AreEqual(25, smtpClient.Port);
            Assert.AreEqual(false, smtpClient.EnableSsl);
            Assert.AreEqual(false, smtpClient.UseDefaultCredentials);
            Assert.AreEqual("andl@elegant.com", (smtpClient.Credentials as NetworkCredential).UserName);
            Assert.AreEqual("abcds", (smtpClient.Credentials as NetworkCredential).Password);
            Assert.AreEqual(SmtpDeliveryMethod.Network, smtpClient.DeliveryMethod); 
        }

        #endregion

    }
}
