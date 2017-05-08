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
    class AbsenceTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string validationErrorMsg = "The From Date must be less than To date";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "ealv";
            password = "654321";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("EMP");

            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Absence");

            string Absence = Browser.FindElementByLinkText("Absence").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Absence);
        }

        #region Absence Tab

        [Test]
        public void CheckValidation_ErrorMsg()
        {
            IWebElement From = Browser.FindElementByXPath("//input[@id='From']");
            IWebElement To = Browser.FindElementByXPath("//input[@id='To']");
            IWebElement buttonSubmit = Browser.FindElementByXPath("//a[@id='buttonSubmit']");
            IWebElement errorTo = Browser.FindElementByXPath("//p[@id='errorFrom']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('From').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('To').removeAttribute('readonly')");

            Browser.SendKeysTo(From, "01.03.2014", true);
            Browser.SendKeysTo(To, "01.01.2014", true);
            Browser.ClickOnWebElement(buttonSubmit);

            Thread.Sleep(Timings.Default_ms*15);

            Assert.IsTrue(errorTo.Text.Contains(validationErrorMsg));
        }


        [Test]
        public void GetAbsenceDataForEmp_ealv()
        {
            DateTime fromDate = new DateTime(2015,01,01);
            DateTime toDate = new DateTime(2015,01,04);

            IWebElement From = Browser.FindElementByXPath("//input[@id='From']");
            IWebElement To = Browser.FindElementByXPath("//input[@id='To']");
            IWebElement buttonSubmit = Browser.FindElementByXPath("//a[@id='buttonSubmit']");

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('From').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('To').removeAttribute('readonly')");

            Browser.SendKeysTo(From, "01.01.2015", true);
            Browser.SendKeysTo(To, "04.01.2015", true);
            Browser.ClickOnWebElement(buttonSubmit);

            Thread.Sleep(Timings.Default_ms*15);


            using (var db = new AjourBTForTestContext())
            {

                Employee emp = db.Employees.Where(e => e.EID == "ealv").FirstOrDefault();

                CalendarItem absences = Tools.SearchCalendarItem(emp.EmployeeID, db, CalendarItemType.BT, fromDate, toDate);
                if (absences != null)
                {
                    IWebElement tableAbsenceForEmp = Browser.FindElementByXPath("//table[@class='weekTable']");

                    Assert.IsTrue(tableAbsenceForEmp.Text.Contains(absences.Location));
                    Assert.IsTrue(tableAbsenceForEmp.Text.Contains(absences.From.ToString(String.Format("dd.MM.yyyy"))));
                    if (absences.To > toDate)
                    {
                        Assert.IsTrue(tableAbsenceForEmp.Text.Contains(toDate.ToString(String.Format("dd.MM.yyyy"))));
                    }
                    else
                    {
                        Assert.IsTrue(tableAbsenceForEmp.Text.Contains(absences.To.ToString(String.Format("dd.MM.yyyy"))));
                    }
                }
                else {
                    IWebElement tableAbsenceForEmp = Browser.FindElementByXPath("//*[@id='wtrDataId']/tbody/tr/td/table[1]/tbody/tr");
                    Assert.AreEqual(Browser.GetText("//*[@id='wtrDataId']/tbody/tr/td/table[1]/tbody/tr/td"), "No absence data for this week");
                }
            }
        }

        #endregion
    }
}
