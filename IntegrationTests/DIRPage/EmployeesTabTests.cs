using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.DIRPage
{
    [TestFixture]
    class EmployeesTabTests
    {
        #region Employees

        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private string nothingFound = "No matching records found";
        private AjourBTForTestContext db = new AjourBTForTestContext();


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "carn";
            password = "gredsa";
            Browser.Goto(baseURL);
            Browser.Wait(15);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Browser.Wait(10);
            Browser.ClickOnLink("DIR");
            Browser.ClickOnLink("Employees");
            Thread.Sleep(Timings.Default_ms * 10); 
            verificationErrors = new StringBuilder();

            //Assert.That(Browser.IsAt(baseURL + "Home/DIRView"));
        }


        [Test]
        public void CheckThatEducationPresentsInTable()
        {
            //Arrange
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.SelectOption("depDropList", "All Departments");
            Thread.Sleep(Timings.Default_ms * 10);

            //Act
            IWebElement table = Browser.FindElement(By.Id("employeeViewexample"), 2);

            //Assert  

            Assert.IsTrue(table.Text.Contains("базова вища, 03.01.2012"));
            Assert.IsTrue(table.Text.Contains("базова вища, 05.01." + DateTime.Now.AddYears(2).Year));
            Assert.IsTrue(table.Text.Contains("повна загальна середня, 01.05.2001; базова вища, 03.03." + DateTime.Now.AddYears(2).Year));
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        #endregion

    }
}
