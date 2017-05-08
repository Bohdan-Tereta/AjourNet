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

namespace IntegrationTests.PUPage
{
    [TestFixture]
    class EmployeeTabTest
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
            username = "apat";
            password = "lokmop";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("PU");

            Browser.ClickOnLink("Employees");

            string Employees = Browser.FindElementByLinkText("Employees").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Employees);
        }

        #region Employees Tab

        [Test]
        public void CheckThatEducationPresentsInTable()
        {
            //Arrange

            //Act
            IWebElement table = Browser.FindElement(By.Id("employeeViewexample"), 2);

            //Assert  

            Assert.IsTrue(table.Text.Contains("базова вища, 03.01.2012"));
            Assert.IsTrue(table.Text.Contains("базова вища, 05.01." + DateTime.Now.AddYears(2).Year));
            Assert.IsTrue(table.Text.Contains("повна загальна середня, 01.05.2001; базова вища, 03.03." + DateTime.Now.AddYears(2).Year));
        }

        [Test]
        public void CheckThatEducationFieldsArePresentForEmployee()
        {
            //Arrange
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("CreateEmployee"), 2));


            //Act
            IWebElement form = Browser.FindElement(By.Id("createEmployeeForm"), 2);

            //Assert  

            Assert.IsTrue(form.Text.Contains("Education Acquired"));
            Assert.IsTrue(form.Text.Contains("Education Acquired Date"));
            Assert.IsTrue(form.Text.Contains("Education In Progress"));
            Assert.IsTrue(form.Text.Contains("Education In Progress Date"));
            Assert.IsNotNull(Browser.FindElement(By.Id("EducationAcquiredType"), 2));
            Assert.IsNotNull(Browser.FindElement(By.Id("createEducationAcquired"), 2));
            Assert.IsNotNull(Browser.FindElement(By.Id("EducationInProgressType"), 2));
            Assert.IsNotNull(Browser.FindElement(By.Id("createEducationInProgress"), 2));
            Assert.AreEqual("hasDatepicker", Browser.FindElement(By.Id("createEducationAcquired"), 2).GetAttribute("class"));
            Assert.AreEqual("hasDatepicker", Browser.FindElement(By.Id("createEducationInProgress"), 2).GetAttribute("class"));
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
            Thread.Sleep(Timings.Default_ms * 10);
        }


        [Test]
        public void CheckThatValidationForEducationFieldsMayBeTriggered()
        {
            //Arrange
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("CreateEmployee"), 2));


            //Act 
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveOnCreateEmployee"), 2));

            Thread.Sleep(Timings.Default_ms * 10);
            IWebElement form = Browser.FindElement(By.Id("createEmployeeForm"), 2);

            //Assert  
            Assert.IsTrue(form.Text.Contains("The Education Acquired field is required."));
            Assert.IsTrue(form.Text.Contains("The Education In Progress field is required."));
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
            Thread.Sleep(Timings.Default_ms * 10);
        }

        [Test]
        public void CheckThatValidationDoesNotTriggerIfFieldsAreNotEmpty()
        {
            //Arrange
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("CreateEmployee"), 2));
            Browser.SelectOption("EducationAcquiredType", "базова загальна середня");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.SelectOption("EducationInProgressType", "базова загальна середня");
            Thread.Sleep(Timings.Default_ms * 10);

            //Act 
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveOnCreateEmployee"), 2));

            Thread.Sleep(Timings.Default_ms * 10);
            IWebElement form = Browser.FindElement(By.Id("createEmployeeForm"), 2);

            //Assert 
            Assert.IsFalse(form.Text.Contains("The Education Acquired field is required."));
            Assert.IsFalse(form.Text.Contains("The Education In Progress field is required."));
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
            Thread.Sleep(Timings.Default_ms * 10);
        }

        [Test]
        public void EditEmployee_EducationSet_ChangesAreSaved()
        {
            //Arrange
            Browser.ClickOnLink("Lee Raymond");
            Browser.SelectOption("EducationAcquiredType", "базова загальна середня");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.SelectOption("EducationInProgressType", "базова вища");
            Thread.Sleep(Timings.Default_ms * 10);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEducationAcquired').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEducationInProgress').removeAttribute('readonly')");
            Browser.SendKeys(Browser.FindElement(By.Id("editEducationAcquired"), 2), new DateTime(2007, 07, 07).ToString("dd.MM.yyyy"));
            Browser.SendKeys(Browser.FindElement(By.Id("editEducationInProgress"), 2), new DateTime(2011, 11, 17).ToString("dd.MM.yyyy"));
            Browser.SendKeys(Browser.FindElement(By.Id("editEducationComment"), 2), "A comment");

            //Act 
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveEmployee"), 2));

            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement table = Browser.FindElement(By.Id("employeeViewexample"), 2);

            //Assert  
            Assert.IsTrue(table.Text.Contains("базова загальна середня, 07.07.2007; базова вища, 17.11.2011"));
            Assert.IsTrue(table.Text.Contains("A comment"));
        }

        [Test]
        public void ClickOnSendButton()
        {
            Employee empWithGreetingMessageAllow = new Employee();
            using (var db = new AjourBTForTestContext())
            {
                empWithGreetingMessageAllow = (from emp in db.Employees where !(String.IsNullOrEmpty(emp.EMail)) && emp.DateDismissed == null select emp).FirstOrDefault();
            }

            if (empWithGreetingMessageAllow != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//tbody[@id='PUEmployee']/tr");
                Console.WriteLine(empWithGreetingMessageAllow);
                foreach (var element in empTable)
                {
                    Console.WriteLine(element.Text);
                    if (element.Text.Contains(empWithGreetingMessageAllow.FirstName))
                    {
                        Console.WriteLine(empWithGreetingMessageAllow.FirstName + " - " + element.Text);
                        Assert.IsTrue(element.Text.Contains(empWithGreetingMessageAllow.LastName));
                        Assert.IsTrue(element.Text.Contains(empWithGreetingMessageAllow.EID));
                    }
                }
            }
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
