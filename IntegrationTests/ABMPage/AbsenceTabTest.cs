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
    class AbsenceTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchErrorText = "No absence data for this period";

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
            Browser.ClickOnLink("Absence");

            string Absence = Browser.FindElementByLinkText("Absence").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Absence);
        }

        #region Absence Tab

        [Test]
        public void GetAbsenseData_From01Oct2014To01Nov2014()
        {
            DateTime fromDateTime = new DateTime(2014,10,01);
            DateTime toDateTime = new DateTime(2014,11,01);

            IWebElement fromDate = Browser.FindElementByXPath("//input[@id='fromDate']");
            IWebElement toDate = Browser.FindElementByXPath("//input[@id='toDate']");
            IWebElement absenceButton = Browser.FindElementByXPath("//a[@id='absenceButton']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('fromDate').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('toDate').removeAttribute('readonly')");

            Browser.SendKeysTo(fromDate, "01.10.2014", true);
            Browser.SendKeysTo(toDate, "01.11.2014", true);
            Browser.ClickOnWebElement(absenceButton);

            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> absenceTable = Browser.FindElementsByXPath("//tbody[@id='absenceView']/tr");
            foreach(IWebElement item in absenceTable)
            {
                ReadOnlyCollection<IWebElement> absenceTableTD = item.FindElements(By.CssSelector("td"));
                Dictionary<CalendarItemType, string> dataFromColumns = new Dictionary<CalendarItemType, string>();
                string empEID = absenceTableTD[2].Text;
                

                dataFromColumns.Add(CalendarItemType.Journey, absenceTableTD[3].Text);
                dataFromColumns.Add(CalendarItemType.BT,absenceTableTD[4].Text);
                dataFromColumns.Add(CalendarItemType.ReclaimedOvertime, absenceTableTD[5].Text);
                dataFromColumns.Add(CalendarItemType.SickAbsence, absenceTableTD[6].Text);
                dataFromColumns.Add(CalendarItemType.PaidVacation, absenceTableTD[7].Text);

                using (var db = new AjourBTForTestContext())
                {
                    Employee employee = (from emp in db.Employees where emp.EID == empEID select emp).FirstOrDefault();
                    foreach (var itemFromTable in dataFromColumns)
                    {
                        if (!(String.IsNullOrEmpty(itemFromTable.Value)))
                        {
                            CalendarItem result = Tools.SearchCalendarItem(employee.EmployeeID, db, itemFromTable.Key, fromDateTime, toDateTime);

                            Assert.IsTrue(itemFromTable.Value.Contains(result.From.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(itemFromTable.Value.Contains(result.To.ToString(String.Format("dd.MM.yyyy"))));
                        }
                    }                    
                }
            }
        }

        [Test]
        public void Search_BadText()
        {
            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='absenceSearchInput']");
            Browser.SendKeysTo(absenceSearchInput, "Bad Text", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement searchResult = Browser.FindElementByXPath("//div[@id='absenceView']/div/p/b");
            Assert.IsTrue(searchResult.Text.Contains(searchErrorText));
        }

        [Test]
        public void Search_acox()
        {
            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='absenceSearchInput']");
            Browser.SendKeysTo(absenceSearchInput, "ealv", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> searchResultTable = Browser.FindElementsByXPath("//tbody[@id='absenceView']/tr");
            Assert.AreEqual(1, searchResultTable.Count);
        }

        #endregion

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
    }
}
