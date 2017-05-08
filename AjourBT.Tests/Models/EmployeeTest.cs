using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc; 
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Tests.Models
{
    [TestFixture]
    public class EmployeeTest
    {
        Mock<IRepository> mock;
        Employee employee;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            employee = new Employee();
        }

        [Test]
        public void prepareToXLSExportADM_DateDismissedNull_Stringified()
        {
            //Arrange 
            employee.EID = "abc"; 
            employee.DateDismissed = null;
            employee.LastName = "a";
            employee.FirstName = "b"; 
            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("a b", result[1]); 
        }

        [Test]
        public void prepareToXLSExportADM_DateDismissedNotNull_Stringified()
        {
            //Arrange 
            employee.EID = "abc";
            employee.DateDismissed = null;
            employee.LastName = "a";
            employee.FirstName = "b";
            employee.DateDismissed = new DateTime(); 
            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("a b\r\n" + new DateTime().ToShortDateString(), result[1]);
        }

        [Test]
        public void prepareToXLSExportADM_PassportNull_Stringified()
        {
            //Arrange 
            employee.Passport = null;

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("no", result[2]);
        }

        [Test]
        public void prepareToXLSExportADM_PassportNotNullEndDateNull_Stringified()
        {
            //Arrange 
            employee.Passport = new Passport();

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("yes", result[2]);
        }

        [Test]
        public void prepareToXLSExportADM_PassportNotNullEndDateNotNull_Stringified()
        {
            //Arrange 
            employee.Passport = new Passport();
            employee.Passport.EndDate = new DateTime();

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("till\r\n" + new DateTime().ToString(String.Format("dd.MM.yyyy")), result[2]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaNull_Stringified()
        {
            //Arrange 
            employee.Visa = null;

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[3]);
            Assert.AreEqual("No Visa", result[4]);
            Assert.AreEqual("No Visa", result[5]);
            Assert.AreEqual("", result[6]);
            Assert.AreEqual("", result[7]); 
        }

        [Test]
        public void prepareToXLSExportADM_VisaNotNUllEntriesZero_Stringified()
        {
            //Arrange 
            employee.Visa = new Visa
                {
                    Entries = 0, 
                    StartDate = new DateTime(2012, 01, 01),
                    DueDate = new DateTime(2014, 01, 01), 
                    Days = 1, 
                    DaysUsedInBT =1, 
                    DaysUsedInPrivateTrips =1, 
                    CorrectionForVisaDays =1, 
                    VisaType = "C0"

                };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("C0", result[3]);
            Assert.AreEqual(employee.Visa.StartDate.ToString("yyyy-MM-dd"), result[4]);
            Assert.AreEqual(employee.Visa.DueDate.ToString("yyyy-MM-dd"), result[5]);
            Assert.AreEqual("MULT", result[6]);
            Assert.AreEqual("1(3)", result[7]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaNotNUllEntriesNotZero_Stringified()
        {
            //Arrange 
            employee.Visa = new Visa
            {
                Entries = 1,
                StartDate = new DateTime(2012, 01, 01),
                DueDate = new DateTime(2014, 01, 01),
                Days = 1,
                DaysUsedInBT = 1,
                DaysUsedInPrivateTrips = 1,
                CorrectionForVisaDays = 1,
                VisaType = "C0", 
                EntriesUsedInBT = 1, 
                EntriesUsedInPrivateTrips = 1, 
                CorrectionForVisaEntries = 1

            };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("C0", result[3]);
            Assert.AreEqual(employee.Visa.StartDate.ToString("yyyy-MM-dd"), result[4]);
            Assert.AreEqual(employee.Visa.DueDate.ToString("yyyy-MM-dd"), result[5]);
            Assert.AreEqual("1(3)", result[6]);
            Assert.AreEqual("1(3)", result[7]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaRegistrationDateNUll_Stringified()
        {
            //Arrange 
            employee.VisaRegistrationDate = null; 

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[8]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaRegistrationDateRegistrationDateNUll_Stringified()
        {
            //Arrange 
            employee.VisaRegistrationDate = new VisaRegistrationDate { RegistrationDate = null };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[8]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaRegistrationDateRegistrationDateHasNoValue_Stringified()
        {
            //Arrange 
            employee.VisaRegistrationDate = new VisaRegistrationDate { RegistrationDate = new DateTime?() };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[8]);
        }

        [Test]
        public void prepareToXLSExportADM_VisaRegistrationDateRegistrationDateHasValue_Stringified()
        {
            //Arrange 
            employee.VisaRegistrationDate = new VisaRegistrationDate { RegistrationDate = new DateTime() };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual(employee.VisaRegistrationDate.RegistrationDate.Value.ToString(String.Format("dd.MM.yyyy")), result[8]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNull_Stringified()
        {
            //Arrange 
            employee.Permit = null;

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[9]);
            Assert.AreEqual("No Permit", result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNumberNullEndDateNullStartDateNull_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { Number = null, StartDate = null, EndDate = null };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[9]);
            Assert.AreEqual("", result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNumberNullEndDateNullStartDateNullKartaPolaka_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { Number = null, StartDate = null, EndDate = null, IsKartaPolaka = true };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[9]);
            Assert.AreEqual("Karta Polaka", result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNumberNotNullEndDateNullStartDateNullKartaPolaka_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { Number = "1", StartDate = null, EndDate = null, IsKartaPolaka = true };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("1", result[9]);
            Assert.AreEqual("Karta Polaka", result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNumberNullEndDateNotNullStartDateNotNullKartaPolaka_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { Number = null, StartDate = new DateTime(), EndDate = new DateTime(), IsKartaPolaka = true };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[9]);
            Assert.AreEqual("Karta Polaka" + "\r\n" + employee.Permit.StartDate.Value.ToString(String.Format("dd.MM.yyyy")) + " - " + 
                employee.Permit.EndDate.Value.ToString(String.Format("dd.MM.yyyy")), result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNumberNullEndDateNotNullStartDateNull_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { Number = null, StartDate = new DateTime(), EndDate = new DateTime(), IsKartaPolaka = false };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[9]);
            Assert.AreEqual(employee.Permit.StartDate.Value.ToShortDateString() + " - " + employee.Permit.EndDate.Value.ToShortDateString(), result[10]);
        }

        [Test]
        public void prepareToXLSExportADM_BusinessTripsNull_Stringified()
        {
            //Arrange 
            employee.BusinessTrips = null; 

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[11]);
        }

        [Test]
        public void prepareToXLSExportADM_BusinessTripsNotNullLastBTNotNull_Stringified()
        {
            //Arrange 
            employee.BusinessTrips = new List<BusinessTrip>();
            employee.BusinessTrips.Add(new BusinessTrip
            {
                EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10),
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10),
                Status = BTStatus.Confirmed | BTStatus.Reported,
                Location = new Location { Title = "abc" }
            }
                ); 

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("abc:" + employee.BusinessTrips[0].StartDate.ToString("dd.MM.yy") + " - " + 
                employee.BusinessTrips[0].EndDate.ToString("dd.MM.yy"), result[11]);
        }

        [Test]
        public void prepareToXLSExportADM_BusinessTripsNotNullLastBTNull_Stringified()
        {
            //Arrange 
            employee.BusinessTrips = new List<BusinessTrip>();
            

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[11]);
        }

        [Test]
        public void prepareToXLSExportADM_PermitNotNullPermitEnddateNotNullAbove90Days_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit { 
                Number = null,
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-100),
                EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-100), IsKartaPolaka = true };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("Contact Gov", result[12]);
            
        }

        [Test]
        public void prepareToXLSExportADM_PermitNotNullPermitEnddateNotNullAbove60Days_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit
            {
                Number = null,
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-65),
                EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-65),
                IsKartaPolaka = true
            };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("Contact Gov", result[12]);

        }

        [Test]
        public void prepareToXLSExportADM_PermitNotNullPermitEnddateNotNullProlong_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit
            {
                Number = null,
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(100),
                EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(100),
                IsKartaPolaka = true, 
                ProlongRequestDate = new DateTime()
            };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual(employee.Permit.ProlongRequestDate.Value.ToString(String.Format("dd.MM.yyyy")), result[12]);

        }

        [Test]
        public void prepareToXLSExportADM_PermitNotNullPermitEnddateNotNullCancel_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit
            {
                Number = null,
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(100),
                EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(100),
                IsKartaPolaka = true,
                CancelRequestDate = new DateTime()
            };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual(employee.Permit.CancelRequestDate.Value.ToString(String.Format("dd.MM.yyyy")), result[12]);

        }

        [Test]
        public void prepareToXLSExportADM_PermitNotNullPermitEnddateNullNothingSpecial_Stringified()
        {
            //Arrange 
            employee.Permit = new Permit
            {
                Number = null,
                StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(100),
                EndDate = null,
                IsKartaPolaka = true
            };

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[12]);

        }

        [Test]
        public void prepareToXLSExportADM_PermitNullNothingSpecial_Stringified()
        {
            //Arrange 
            employee.Permit = null; 

            //Act
            List<string> result = employee.PrepareToXLSExportVisasVU();

            //Assert 
            Assert.AreEqual(13, result.Count);
            Assert.AreEqual("", result[12]);

        }

    }
}
