using NUnit.Framework;
using OpenQA.Selenium;
using AjourBT.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Entities;

namespace IntegrationTests.VUPage
{
    [TestFixture]
    class PrivateTripsTabTests
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;

        private ReadOnlyCollection<IWebElement> empList;
        private string searchResultFail = "No matching records found";

        [TestFixtureSetUp]
        public void Login()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "ayou";
            password = "123456";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("VU");

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            Browser.ClickOnLink("Private Trips");

            string PrivateTrips = Browser.FindElementByLinkText("Private Trips").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", PrivateTrips);
        }


        [Test]
        public void SearchFIeld_BadString_Empty()
        {
            IWebElement searchField = Browser.FindElementByXPath("//input[@id='seachInput']");
            Browser.SendKeysTo(searchField, "sdfsdfsd", true);
            Browser.SendKeysTo(searchField, Keys.Enter, false);

            Thread.Sleep(Timings.Default_ms*20);

            Assert.IsTrue(Browser.FindElementByClassName("dataTables_empty").Text.Contains(searchResultFail));
            Browser.SendKeysTo(searchField, Keys.Enter, true);
        }

        [Test]
        public void SearchField_Cruz_NotEmpty()
        {
            IWebElement searchField = Browser.FindElementByXPath("//input[@id='seachInput']");
            Thread.Sleep(Timings.Default_ms*15);
            Browser.SendKeysTo(searchField, "Cruz", true);
            Thread.Sleep(Timings.Default_ms*15);
            Browser.SendKeys(searchField, Keys.Enter);

            Thread.Sleep(Timings.Default_ms*15);

            empList = Tools.GetAllTableData("/html/body/div[1]/div/div[4]/div/div/div[1]/div[2]/table/tbody/tr", Browser.webDriver);
            Assert.AreEqual(1, empList.Count);

            Browser.SendKeysTo(searchField, Keys.Enter, true);
        }


        [Test]
        public void PrivateTrips_TableColumns()
        {
            //Arrange
            Thread.Sleep(Timings.Default_ms*20);
            //Act

            //Assert
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]"), "EID");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]"), "Name");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[1]/div/table/thead/tr/th[3]"), "Total Days");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[1]/div/table/thead/tr/th[4]"), "Private Trips");

        }


        [Test]
        public void PrivateTrips_TableRows()
        {
            //Arrange
            Thread.Sleep(Timings.Default_ms*20);
            //Act

            //Assert
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[2]/table/tbody/tr[15]/td[1]"), "salz");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[2]/table/tbody/tr[15]/td[2]"), "Baker Kathryn");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[2]/table/tbody/tr[15]/td[3]"), "33");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/div/div/div[1]/div[2]/table/tbody/tr[15]/td[4]"), String.Format(DateTime.Now.ToLocalTimeAzure().AddDays(-100).Date.ToString("dd.MM.yyyy")+" - "+ DateTime.Now.ToLocalTimeAzure().AddDays(-90).Date.ToString("dd.MM.yyyy")+ " (11)       "+DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-70).Date.ToString("dd.MM.yyyy")+" - " +DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-60).Date.ToString("dd.MM.yyyy")+" (11)       "+DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-30).Date.ToString("dd.MM.yyyy")+" - " +DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20).Date.ToString("dd.MM.yyyy")+" (11)      "));

        }


        [Test]
        public void TableData()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickOnLink("Private Trips");
            Thread.Sleep(Timings.Default_ms*20);
            int year = DateTime.Now.ToLocalTimeAzure().Year;

            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                List<PrivateTrip> employeePts = (from bt in db.PrivateTrips.AsEnumerable()
                                                  select bt).ToList();
                List<Employee> employees= (from e in db.Employees.AsEnumerable()
                                           where e.IsUserOnly==false && e.DateDismissed==null
                                                 select e).ToList();

                Visa visa = employeePts.FirstOrDefault().PrivateTripOf;
                if (visa != null)
                {
                    ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='privateTripsBTMexample']/tbody/tr");
                    Thread.Sleep(Timings.Default_ms*20);

                    foreach (var element in empTable)
                    {
                        if (element.Text.Contains(visa.VisaOf.LastName))
                        {
                            Assert.IsTrue(element.Text.Contains(visa.VisaOf.FirstName));
                            Assert.IsTrue(element.Text.Contains(visa.VisaOf.LastName));

                        }
                    }
                    Assert.AreEqual(employees.Count, empTable.Count);
                }
            }
        }
        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
    }
}
