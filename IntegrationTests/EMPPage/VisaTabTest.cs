using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.EMPPage
{
    [TestFixture]
    class VisaTabTest
    {
        private string baseURL = "http://localhost:50616/";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            Browser.ClickOnLink("EMP");

            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Visa");

            string Visa = Browser.FindElementByLinkText("Visa").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Visa);
        }

        #region Visa Tab

        [Test]
        public void CheckVisaPassportAndPermit_ealv()
        {
            IWebElement tableVisaPerEmp = Browser.FindElementByXPath("//table[@id='visaDataPerEmp']");
         
            using (var db = new AjourBTForTestContext())
            {
                Employee emp = db.Employees.Where(e => e.EID == "ealv").FirstOrDefault();
                Passport passportForEmp = db.Passports.Where(p => p.EmployeeID == emp.EmployeeID).FirstOrDefault();
                Visa visaForEmp = db.Visas.Where(v => v.EmployeeID == emp.EmployeeID).FirstOrDefault();
                Permit permitForEmp = db.Permits.Where(p => p.EmployeeID == emp.EmployeeID).FirstOrDefault();

                if (passportForEmp != null && passportForEmp.EndDate.HasValue)
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("valid"));
                }
                
                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Passport"));
                }

                if (visaForEmp != null)
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.VisaType));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.Entries.ToString()));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.Days.ToString()));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.StartDate.ToString(String.Format("dd.MM.yyyy"))));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.DueDate.ToString(String.Format("dd.MM.yyyy"))));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(CalculateVisaDays(emp.EmployeeID).ToString( )));
                }
                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Visa"));

                }

                if (permitForEmp != null)
                {
                    if (permitForEmp.IsKartaPolaka)
                        Assert.IsTrue(tableVisaPerEmp.Text.Contains("Karta Polaka"));
                    else
                        Assert.IsTrue(tableVisaPerEmp.Text.Contains(permitForEmp.EndDate.Value.ToString("dd.MM.yyyy")));
                }
                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Permit"));
                }
            }
        }

        [Test]
        public void CheckVisaPassportAndPermit_rkni()
        {

            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off"); 
            Thread.Sleep(Timings.Default_ms * 5)  ;
            string username = "rkni";
            string password = "feharo";
            Thread.Sleep(Timings.Default_ms * 5);
            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("EMP");
            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Visa");

            IWebElement tableVisaPerEmp = Browser.FindElementByXPath("//table[@id='visaDataPerEmp']");

            using (var db = new AjourBTForTestContext())
            {
                Employee emp = db.Employees.Where(e => e.EID == "rkni").FirstOrDefault();
                Passport passportForEmp = db.Passports.Where(p => p.EmployeeID == emp.EmployeeID).FirstOrDefault();
                Visa visaForEmp = db.Visas.Where(v => v.EmployeeID == emp.EmployeeID).FirstOrDefault();
                Permit permitForEmp = db.Permits.Where(p => p.EmployeeID == emp.EmployeeID).FirstOrDefault();

                if (passportForEmp != null)
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("valid"));
                }

                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Passport"));
                }

                if (visaForEmp != null)
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.VisaType));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.Entries.ToString()));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.Days.ToString()));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.StartDate.ToString(String.Format("dd.MM.yyyy"))));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(visaForEmp.DueDate.ToString(String.Format("dd.MM.yyyy"))));
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains(CalculateVisaDays(emp.EmployeeID).ToString()));
                }
                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Visa"));

                }

                if (permitForEmp != null)
                {
                    if (permitForEmp.IsKartaPolaka)
                        Assert.IsTrue(tableVisaPerEmp.Text.Contains("Karta Polaka"));
                    else
                        Assert.IsTrue(tableVisaPerEmp.Text.Contains(permitForEmp.EndDate.Value.ToString("dd.MM.yyyy")));
                }
                else
                {
                    Assert.IsTrue(tableVisaPerEmp.Text.Contains("No Permit"));
                }
            }
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
            Thread.Sleep(Timings.Default_ms * 5);
            username = "ealv";
            password = "654321";
            Thread.Sleep(Timings.Default_ms * 5);
            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("EMP");
        }

        #endregion

        //[TestFixtureTearDown]
        //public void TeardownTest()
        //{
        //    if (Browser.HasElement("Log off"))
        //        Browser.ClickOnLink("Log off");
        //} 

        #region helpers
        //TODO: Duplicated in ADM Controller
        public int CalculateVisaDays(int employeeID)
        {
            DateTime endDateOfChekedPeriod = DateTime.Now.Date;

            DateTime startDateOfChekedPeriod = endDateOfChekedPeriod.AddDays(-180).Date;

            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> employeeBTs = (from b in db.BusinessTrips.AsEnumerable()
                                                  where b.BTof.EmployeeID == employeeID
                                                  && b.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                                  && b.OrderStartDate != null
                                                  && b.OrderEndDate != null
                                                  && (b.StartDate <= b.EndDate)
                                                  && (b.OrderStartDate <= endDateOfChekedPeriod && b.OrderEndDate >= startDateOfChekedPeriod)
                                                  select b).ToList();

                List<IGrouping<DateTime, BusinessTrip>> distinctBTs = employeeBTs.GroupBy(i => i.OrderStartDate.Value).ToList();

                List<PrivateTrip> employeePrivateTs = (from b in db.PrivateTrips.AsEnumerable()
                                                       where b.EmployeeID == employeeID
                                                       && (b.StartDate <= b.EndDate)
                                                       && (b.StartDate <= endDateOfChekedPeriod && b.EndDate >= startDateOfChekedPeriod)
                                                       select b).ToList();

                int spentDays = 0;
                foreach (IGrouping<DateTime, BusinessTrip> b in distinctBTs)
                {
                    spentDays += DaysSpentInTrip(startDateOfChekedPeriod, endDateOfChekedPeriod, b.FirstOrDefault().OrderStartDate, b.FirstOrDefault().OrderEndDate);
                }
                foreach (PrivateTrip pt in employeePrivateTs)
                {
                    spentDays += DaysSpentInTrip(startDateOfChekedPeriod, endDateOfChekedPeriod, pt.StartDate, pt.EndDate);
                }

                return spentDays;
            }
        }

        public int DaysSpentInTrip(DateTime startDateOfChekedPeriod, DateTime endDateOfChekedPeriod, DateTime? StartDate, DateTime? EndDate)
        {

            int days = 0;
            DateTime startDateForCalculations;
            DateTime endDateForCalculations;
            if (StartDate != null && EndDate != null
                && (StartDate <= EndDate)
                && (StartDate <= endDateOfChekedPeriod && EndDate >= startDateOfChekedPeriod))
            {
                startDateForCalculations = StartDate.Value > startDateOfChekedPeriod ? StartDate.Value : startDateOfChekedPeriod;
                endDateForCalculations = EndDate.Value < endDateOfChekedPeriod ? EndDate.Value : endDateOfChekedPeriod;
                days += (endDateForCalculations - startDateForCalculations).Days + 1;
            }

            return days;
        }
        #endregion

    }
}
