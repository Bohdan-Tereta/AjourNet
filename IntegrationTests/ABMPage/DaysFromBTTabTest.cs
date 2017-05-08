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

namespace IntegrationTests.ABMPage
{
    [TestFixture]
    class DaysFromBTTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchResultFail = "No matching records found";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "tmas";
            password = "aserty";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("ABM");

            Assert.That(Browser.IsAt(baseURL + "Home/ABMView"));
            Browser.ClickOnLink("Days From BT");

            string DaysFromBT = Browser.FindElementByLinkText("Days From BT").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", DaysFromBT);
        }

        [Test]
        public void CheckJourneyLinkPopup_ColemanJohn()
        {
            IReadOnlyCollection<IWebElement> tbodyJourneys = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");

            IWebElement firstElem = tbodyJourneys.FirstOrDefault();
            IWebElement journey = firstElem.FindElement(By.XPath("//a[@id='journeyLink']/redtext"));

            Assert.IsTrue(firstElem.Text.Contains("Coleman John"));
            Assert.IsTrue(firstElem.Text.Contains("03.03.2013"));
            Assert.AreEqual("31.10.2013" ,journey.GetAttribute("title"));
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        [Test]
        public void CheckOvertimeLinkPopup_GeorgeAnatoliy()
        {
            IReadOnlyCollection<IWebElement> tbodyJourneys = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");

            IWebElement secondElem = tbodyJourneys.ElementAt(1);
            IWebElement overtime = secondElem.FindElement(By.XPath("//a[@id='overtimeLink']/redtext"));

            Assert.IsTrue(secondElem.Text.Contains("George Anatoliy"));
            Assert.IsTrue(secondElem.Text.Contains("03.01.2013"));
            Assert.AreEqual("10.03.2013", overtime.GetAttribute("title"));
        }

        [Test]
        public void Search_BadString()
        {
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchInput']");
            Browser.SendKeysTo(searchInput, "Bad Text", true);
            Browser.SendKeys(searchInput, Keys.Enter);
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement emptyTable = Browser.FindElementByXPath("//td[@class='dataTables_empty']");
            Assert.AreEqual(searchResultFail, emptyTable.Text);
        }

        [Test]
        public void Search_co_NotEmpty()
        {
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchInput']");
            Browser.SendKeysTo(searchInput, "co", true);
            Browser.SendKeys(searchInput, Keys.Enter);
            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> searchResult = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
            Assert.AreEqual(4, searchResult.Count);

            Browser.SendKeysTo(searchInput, Keys.Enter, true);
        }

        [Test]
        public void CheckJourneys()
        {
                ReadOnlyCollection<IWebElement> daysFromBtTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
                foreach (IWebElement tableElement in daysFromBtTable)
                {
                    ReadOnlyCollection<IWebElement> daysFromBtTableTD = tableElement.FindElements(By.CssSelector("td"));
                    string empFromTable = daysFromBtTableTD[2].Text;

                    using (var db = new AjourBTForTestContext())
                    {
                        Employee emp = db.Employees.Where(e => e.EID == empFromTable).FirstOrDefault();

                        List<Journey> journeys = (from bts in db.BusinessTrips where
                                                      bts.EmployeeID == emp.EmployeeID && bts.Journeys.Count > 0
                                                  from journ in bts.Journeys
                                                  where journ.DayOff == true
                                                  select journ).ToList();

                        if (journeys != null)
                        {
                            foreach (Journey j in journeys)
                            {
                                Assert.IsTrue(daysFromBtTableTD[3].Text.Contains(j.Date.ToString(String.Format("dd.MM.yyyy"))));
                            }
                        }
                    }
            }
       }

        [Test]
        public void CheckOvertimes()
        {

                ReadOnlyCollection<IWebElement> daysFromBtTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
                foreach (IWebElement tableElement in daysFromBtTable)
                {
                    ReadOnlyCollection<IWebElement> daysFromBtTableTD = tableElement.FindElements(By.CssSelector("td"));
                    string empFromTable = daysFromBtTableTD[2].Text;

                    using (var db = new AjourBTForTestContext())
                    {
                        Employee emp = db.Employees.Where(e => e.EID == empFromTable).FirstOrDefault();

                        List<Overtime> overtimes = (from over in db.Overtimes
                                                    where
                                                        over.Type == OvertimeType.Paid && over.EmployeeID == emp.EmployeeID
                                                    select over).ToList();

                        if (overtimes != null)
                        {
                            foreach (Overtime o in overtimes)
                                Assert.IsTrue(daysFromBtTableTD[3].Text.Contains(o.Date.ToString(String.Format("dd.MM.yyyy"))));
                        }
                    }
            }
        }

        [Test]
        public void SaveSortAfterUpdate()
        {
            List<string> eid = new List<string>();
            List<string> eidAfterUpdate = new List<string>();

            IWebElement columnToSort = Browser.FindElementByXPath("//th[@class='sorting']");
            Browser.ClickOnWebElement(columnToSort);
            ReadOnlyCollection<IWebElement> abmTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
            foreach (IWebElement item in abmTable)
            {
                string [] td = item.Text.Split(' ');
                eid.Add(td[2]);
            }

            IWebElement overtimeLink = Browser.FindElementByXPath("//a[@id='overtimeLink']");
            Browser.ClickOnWebElement(overtimeLink);
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
            Thread.Sleep(Timings.Default_ms*15);

            abmTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
            foreach (IWebElement item in abmTable)
            {
                string[] updatedTd = item.Text.Split(' ');
                eidAfterUpdate.Add(updatedTd[2]);
            }

            Assert.AreEqual(eid.Count, eidAfterUpdate.Count);
            Assert.IsTrue(eid.SequenceEqual(eidAfterUpdate));
        }
    }
}
