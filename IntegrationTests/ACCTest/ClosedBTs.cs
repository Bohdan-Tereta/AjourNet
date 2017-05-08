using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.ACCTest
{
    [TestFixture]
    class ClosedBTs
    {
        private const int defaultTimeSpan_ms = 300;

        private string baseURL = "http://localhost:50616/";
        private string username = "lark";
        private string password = "12345t";
        private string logIn = "/html/body/div[1]/section/section/form/fieldset/div/input";
        private string searchErrorText = "No matching records found";
        private List<BusinessTrip> btList = new List<BusinessTrip>();

        private void BTSetup()
        {
            btList = new List<BusinessTrip>
            {
                new BusinessTrip{AccComment = "TestComment1",BusinessTripID = 100, EmployeeID = 12, EndDate = new DateTime(2014,06,21),Manager = "ncru",  StartDate = new DateTime(2014,05,06), Status = BTStatus.Confirmed | BTStatus.Reported, Responsible = "abcd", LastCRUDTimestamp = new DateTime(2014,01,01),LocationID = 1, UnitID =1 },
                new BusinessTrip{AccComment = "TestComment2",BusinessTripID = 101, EmployeeID = 13, EndDate = new DateTime(2016,01,01),Manager = "ncru",  StartDate = new DateTime(2015,06,07), Status = BTStatus.Confirmed | BTStatus.Reported, Responsible = "abcd",LastCRUDTimestamp = new DateTime(2014,01,01), LocationID = 2, UnitID =1},
                
            };

            using (var db = new AjourBTForTestContext())
            {
                foreach (BusinessTrip item in btList)
                {
                    db.BusinessTrips.Add(item);
                    db.SaveChanges();
                }
            }
        }


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath(logIn);
            Thread.Sleep(defaultTimeSpan_ms * 10);
            Browser.ClickOnLink("ACC");

            Assert.That(Browser.IsAt(baseURL + "Home/ACCView"));
            BTSetup();

            Browser.ClickOnLink("Closed BTs");
        }


        [Test]
        public void CheckEmployeesWithAccComment()
        {
            ReadOnlyCollection<IWebElement> tableData = Browser.FindElementsByXPath("//table[@id='closedBTsView']/tbody/tr");
            Assert.AreEqual(2, tableData.Count);

            Assert.IsTrue(tableData[0].Text.Contains("wens"));
            Assert.IsTrue(tableData[0].Text.Contains("06.05.2014 - 21.06.2014"));
            Assert.IsTrue(tableData[0].Text.Contains("AT/BW"));

            Assert.IsTrue(tableData[1].Text.Contains("milt"));
            Assert.IsTrue(tableData[1].Text.Contains("07.06.2015 - 01.01.2016"));
            Assert.IsTrue(tableData[1].Text.Contains("EC/BV"));
        }

        [Test]
        public void CheckModalWindow()
        {
            IWebElement showClosedBt = Browser.FindElementByXPath("//a[@id='ShowClosedBTACC']");
            Browser.ClickOnWebElement(showClosedBt);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement modalWindow = Browser.FindElementByXPath("//textarea[@class='textAreaAccComment']");
            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");

            Assert.IsTrue(modalWindow.Text.Contains("TestComment1"));
            Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void Search_BadText()
        {
            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='searchClosedBTs']");
            Browser.SendKeysTo(absenceSearchInput, "Bad Text", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement searchResult = Browser.FindElementByXPath("//td[@class='dataTables_empty']");
            Assert.IsTrue(searchResult.Text.Contains(searchErrorText));
        }

        [Test]
        public void Search_wens()
        {
            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='searchClosedBTs']");
            Browser.SendKeysTo(absenceSearchInput, "wens", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> searchResultTable = Browser.FindElementsByXPath("//table[@id='closedBTsView']/tbody[@id='tbodyBTACC']/tr");
            Assert.AreEqual(1, searchResultTable.Count);
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");


            string[] comments = { "TestComment1", "TestComment2" };
            using (var db = new AjourBTForTestContext())
            {
                foreach (var item in comments)
                {
                    BusinessTrip btToRemove = db.BusinessTrips.Where(bt => bt.AccComment == item).FirstOrDefault();
                    db.BusinessTrips.Remove(btToRemove);
                }
                db.SaveChanges();
            }
        }
    }
}
