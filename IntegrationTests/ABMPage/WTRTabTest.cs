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
    class WTRTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchErrorText = "No absence data for this week";
        private string validationErrorMsg = "The From Date must be less than To date";

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
            Browser.ClickOnLink("WTR");

            string WTR = Browser.FindElementByLinkText("WTR").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", WTR);
        }

        [Test]
        public void GetWTRData_From02Dec2014To07Dec2014()
        {
            DateTime fromDateTime = new DateTime(2014, 12, 02);
            DateTime toDateTime = new DateTime(2014, 12, 07);

            IWebElement From = Browser.FindElementByXPath("//input[@id='From']");
            IWebElement To = Browser.FindElementByXPath("//input[@id='To']");
            IWebElement buttonSubmit = Browser.FindElementByXPath("//a[@id='buttonSubmit']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('From').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('To').removeAttribute('readonly')");

            Browser.SendKeysTo(From, "02.12.2014", true);
            Browser.SendKeysTo(To, "07.12.2014", true);
            Browser.ClickOnWebElement(buttonSubmit);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement wtrTable = Browser.FindElementByXPath("//table[@class='weekTable']");

            using(var db = new AjourBTForTestContext())
            {
                List<CalendarItem> result = Tools.SearchCalendarItemForWTR(db, fromDateTime, toDateTime);

                foreach (CalendarItem item in result)
                {
                    Employee emp = (from e in db.Employees where e.EmployeeID == item.EmployeeID select e).FirstOrDefault();
                    Assert.IsTrue(wtrTable.Text.Contains(emp.LastName + " " + emp.FirstName));
                    Assert.IsTrue(wtrTable.Text.Contains(emp.EID));
                    Assert.IsTrue(wtrTable.Text.Contains(item.Type.ToString()));

                    if (!(String.IsNullOrEmpty(item.Location)))
                    {
                        Assert.IsTrue(wtrTable.Text.Contains(item.Location));
                        Assert.IsTrue(((item.From <= fromDateTime && item.To >= fromDateTime)
                                    || (item.From >= fromDateTime && item.From <= toDateTime)));
                    }
                }
            }
        }

        [Test]
        public void Search_BadText()
        {
            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='searchInputABM']");
            Browser.SendKeysTo(absenceSearchInput, "Bad Text", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement searchResult = Browser.FindElementByXPath("//table[@class='no-data']/tbody/tr/td");
            Assert.IsTrue(searchResult.Text.Contains(searchErrorText));
        }

        [Test]
        public void Search_sban()
        {
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 20);
            Browser.ClickOnLink("WTR");

            IWebElement From = Browser.FindElementByXPath("//input[@id='From']");
            IWebElement To = Browser.FindElementByXPath("//input[@id='To']");
            IWebElement buttonSubmit = Browser.FindElementByXPath("//a[@id='buttonSubmit']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('From').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('To').removeAttribute('readonly')");

            Browser.SendKeysTo(From, "02.12.2014", true);
            Browser.SendKeysTo(To, "07.12.2014", true);
            Browser.ClickOnWebElement(buttonSubmit);

            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement absenceSearchInput = Browser.FindElementByXPath("//input[@id='searchInputABM']");
            Browser.SendKeysTo(absenceSearchInput, "sban", true);
            Browser.SendEnter(absenceSearchInput);

            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> wtrTable = Browser.FindElementsByXPath("//table[@class='weekTable']/tbody/tr");
            Assert.AreEqual(4, wtrTable.Count);
            Assert.IsTrue(wtrTable[1].Text.Contains("sban"));
        }

        [Test]
        public void CheckValidation_FromLessThanTo_Error()
        {
            IWebElement From = Browser.FindElementByXPath("//input[@id='From']");
            IWebElement To = Browser.FindElementByXPath("//input[@id='To']");
            IWebElement buttonSubmit = Browser.FindElementByXPath("//a[@id='buttonSubmit']");
            IWebElement errorMessage = Browser.FindElementByXPath("//p[@id='errorFrom']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('From').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('To').removeAttribute('readonly')");

            Browser.SendKeysTo(From, "12.10.2014", true);
            Browser.SendKeysTo(To, "06.10.2014", true);
            Browser.ClickOnWebElement(buttonSubmit);

            Assert.IsTrue(errorMessage.Text.Contains(validationErrorMsg));
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
    }
}
