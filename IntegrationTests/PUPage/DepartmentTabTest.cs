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
    class DepartmentTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string warningMsg = "Can't Delete this department";
        private string alreadyExistMsg = "Department with same Name already exists";
        private string emptyFieldMsg = "Please enter department name";

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

            Browser.ClickOnLink("Departments");

            string Employees = Browser.FindElementByLinkText("Employees").GetCssValue("color");
            Assert.AreEqual("rgba(46, 110, 158, 1)", Employees);
        }

        [Test]
        public void CreateDepartment()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateDepartment']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement departmentName = Browser.FindElementByXPath("//input[@id='DepartmentName']");
            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.SendKeysTo(departmentName, "Test Department", true);
            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            Assert.Less(departmentsCount.Count, departmentsAfterCreateCount.Count);
        }

        [Test]
        public void DeleteDepartment()
        {
            ReadOnlyCollection<IWebElement> departmentsCountBeforeDelete = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            IWebElement testDepartment = Browser.FindElementsByXPath("//a[@class='depEditDialog']").LastOrDefault();
            Browser.ClickOnWebElement(testDepartment);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement deleteButton = Browser.FindElementByXPath("//button[@id='btnDeleteDepartment']");
            Browser.ClickOnWebElement(deleteButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement deleteConfirmButton = Browser.FindElementByXPath("//div[@class='ui-dialog-buttonset']/button");
            Browser.ClickOnWebElement(deleteConfirmButton);
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> departmentsCountAfterDelete = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            Assert.Less(departmentsCountAfterDelete.Count, departmentsCountBeforeDelete.Count);
        }

        [Test]
        public void CannotDeleteDepartment_Validation()
        {
            IWebElement departmentToDelete = Browser.FindElementsByXPath("//a[@class='depEditDialog']").FirstOrDefault();
            Browser.ClickOnWebElement(departmentToDelete);
            Thread.Sleep(Timings.Default_ms * 15);

            IWebElement deleteButton = Browser.FindElementByXPath("//button[@id='btnDeleteDepartment']");
            Browser.ClickOnWebElement(deleteButton);
            Thread.Sleep(Timings.Default_ms * 15);

            IWebElement cantDeleteMsg = Browser.FindElementByXPath("//div[@id='deleteDepartment-Confirm']/h4");
            IWebElement OKButton = Browser.FindElementByXPath("//div[@class='ui-dialog-buttonset']/button");

            Assert.AreEqual(warningMsg, cantDeleteMsg.Text);

            Browser.ClickOnWebElement(OKButton);            
        }

        [Test]
        public void CannotCreate_AlreadyExistValidation()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateDepartment']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement departmentName = Browser.FindElementByXPath("//input[@id='DepartmentName']");
            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.SendKeysTo(departmentName, "DEPT1", true);
            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement validationMsg = Browser.FindElementByXPath("//span[@class='field-validation-error']");
            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            Assert.AreEqual(alreadyExistMsg, validationMsg.Text);
            Assert.LessOrEqual(departmentsCount.Count, departmentsAfterCreateCount.Count);

            Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void CannotCreate_RequiredFieldEmpty()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateDepartment']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement departmentName = Browser.FindElementByXPath("//input[@id='DepartmentName']");
            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.SendKeysTo(departmentName, "", true);
            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement validationMsg = Browser.FindElementByXPath("//span[@for='DepartmentName']");
            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='depEditDialog']");

            Assert.AreEqual(emptyFieldMsg, validationMsg.Text);
            Assert.LessOrEqual(departmentsCount.Count, departmentsAfterCreateCount.Count);

            Browser.ClickOnWebElement(closeBtn);
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
    }
}

