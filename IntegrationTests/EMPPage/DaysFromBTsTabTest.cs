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

namespace IntegrationTests.EMPPage
{
    [TestFixture]
    class DaysFromBTsTabTest
    {
        private string baseURL = "http://localhost:50616/";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.ClickOnLink("EMP");

            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Days From BTs");

            string DaysFromBTs = Browser.FindElementByLinkText("Days From BTs").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", DaysFromBTs);
        }

        #region Days From BTs Tab

        [Test]
        public void CheckDaysFromBTs_Journeys_ealv()
        {
            ReadOnlyCollection<IWebElement> journeysFromBtTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/a[@id='journeyLink']");

            using (var db = new AjourBTForTestContext())
            {
                Employee ealv = db.Employees.Where(e => e.EID == "ealv").FirstOrDefault();
                List<Journey> journeys = (from item in db.Journeys where
                                              item.DayOff == true && item.JourneyOf.EmployeeID == ealv.EmployeeID
                                          select item).ToList();

                Assert.AreEqual(journeysFromBtTable.Count, journeys.Count);
                if (journeys.Count > 0)
                {
                    IWebElement daysFromBtTable = Browser.FindElementByXPath("//tbody[@id='tbodyJourneys']");
                    foreach (Journey item in journeys)
                        Assert.IsTrue(daysFromBtTable.Text.Contains(item.Date.ToString(String.Format("dd.MM.yyyy"))));
                }
                else
                {
                    Thread.Sleep(Timings.Default_ms*10);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/b"), "You have no days for reclaim");
                }
            }
        }

        [Test]
        public void CheckDaysFromBTs_Overtimes_ealv()
        {
            ReadOnlyCollection<IWebElement> overtimesFromBtTable = Browser.FindElementsByXPath("//a[@id='overtimeLink']");

            using (var db = new AjourBTForTestContext())
            {
                Employee ealv = db.Employees.Where(e => e.EID == "ealv").FirstOrDefault();
                List<Overtime> overtimes = (from item in db.Overtimes where
                                              item.Type == OvertimeType.Paid && item.EmployeeID == ealv.EmployeeID
                                          select item).ToList();

                Assert.AreEqual(overtimesFromBtTable.Count, overtimes.Count);
                if (overtimes.Count > 0)
                {
                    IWebElement daysFromBtTable = Browser.FindElementByXPath("//table[@id='journeysViewABM']/tbody[@id='tbodyJourneys']");
                    foreach (Overtime item in overtimes)
                        Assert.IsTrue(daysFromBtTable.Text.Contains(item.Date.ToString(String.Format("dd.MM.yyyy"))));
                }
                else
                {
                    Thread.Sleep(Timings.Default_ms*10);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[4]/b"), "You have no days for reclaim");
                }
            }
        }

        #endregion
    }
}
