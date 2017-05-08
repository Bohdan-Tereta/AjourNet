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
    class LocationTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string warningMsg = "Can't Delete this location";
        private string alreadyExistMsg = "Location with same Title already exists";
        private string emptyFieldCountryMsg = "The CountryID field is required.";
        private string emptyFieldTitleMsg = "The Title field is required.";
        private string emptyFieldAddressMsg = "The Address field is required.";

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

            Browser.ClickOnLink("Locations");

            string Employees = Browser.FindElementByLinkText("Employees").GetCssValue("color");
            Assert.AreEqual("rgba(46, 110, 158, 1)", Employees);
        }

        [Test]
        public void CreateLocation()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateLocationButton']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement address = Browser.FindElementByXPath("//input[@id='Address']");
            IWebElement responsible = Browser.FindElementByXPath("//input[@id='ResponsibleForLoc']");
            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.SelectOption("CountryID", 4);
            Browser.SendKeysTo(title, "XX/XX", true);
            Browser.SendKeysTo(address, "Test Address",true);
            Browser.SendKeysTo(responsible, "rkni", true);

            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            Assert.Less(departmentsCount.Count, departmentsAfterCreateCount.Count);
        }

        [Test]
        public void DeleteLocation()
        {
            ReadOnlyCollection<IWebElement> departmentsCountBeforeDelete = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            IWebElement testDepartment = Browser.FindElementsByXPath("//a[@class='locEditDialog']").LastOrDefault();
            Browser.ClickOnWebElement(testDepartment);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement deleteButton = Browser.FindElementByXPath("//button[@id='btnDeleteLocation']");
            Browser.ClickOnWebElement(deleteButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement deleteConfirmButton = Browser.FindElementByXPath("//div[@class='ui-dialog-buttonset']/button");
            Browser.ClickOnWebElement(deleteConfirmButton);
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> departmentsCountAfterDelete = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            Assert.Less(departmentsCountAfterDelete.Count, departmentsCountBeforeDelete.Count);
        }

        [Test]
        public void CannotDeleteLocation_Validation()
        {
            IWebElement departmentToDelete = Browser.FindElementsByXPath("//a[@class='locEditDialog']").FirstOrDefault();
            Browser.ClickOnWebElement(departmentToDelete);
            Thread.Sleep(Timings.Default_ms * 15);

            IWebElement deleteButton = Browser.FindElementByXPath("//button[@id='btnDeleteLocation']");
            Browser.ClickOnWebElement(deleteButton);
            Thread.Sleep(Timings.Default_ms * 15);

            IWebElement cantDeleteMsg = Browser.FindElementByXPath("//div[@id='deleteLocation-Confirm']/h4");
            IWebElement OKButton = Browser.FindElementByXPath("//div[@class='ui-dialog-buttonset']/button");

            Assert.AreEqual(warningMsg, cantDeleteMsg.Text);

            Browser.ClickOnWebElement(OKButton);
        }

        [Test]
        public void CannotCreateLocation_AlreadyExistValidation()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateLocationButton']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement address = Browser.FindElementByXPath("//input[@id='Address']");
            IWebElement responsible = Browser.FindElementByXPath("//input[@id='ResponsibleForLoc']");
            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.SelectOption("CountryID", 2);
            Browser.SendKeysTo(title, "AT/BW", true);
            Browser.SendKeysTo(address, "Test Address", true);
            Browser.SendKeysTo(responsible, "rkni", true);

            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement validationMsg = Browser.FindElementByXPath("//span[@class='field-validation-error']");
            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            Assert.AreEqual(alreadyExistMsg, validationMsg.Text);
            Assert.LessOrEqual(departmentsCount.Count, departmentsAfterCreateCount.Count);

            Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void CannotCreateLocation_RequiredFieldEmpty()
        {
            ReadOnlyCollection<IWebElement> departmentsCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            IWebElement createDepButton = Browser.FindElementByXPath("//a[@id='CreateLocationButton']");
            Browser.ClickOnWebElement(createDepButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement saveButton = Browser.FindElementByXPath("//button[@type='button']");

            Browser.ClickOnWebElement(saveButton);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement validationMsgCountry = Browser.FindElementByXPath("//span[@for='CountryID']");
            IWebElement validationMsgTitle = Browser.FindElementByXPath("//span[@for='Title']");
            IWebElement validationMsgAddress = Browser.FindElementByXPath("//span[@for='Address']");

            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");

            ReadOnlyCollection<IWebElement> departmentsAfterCreateCount = Browser.FindElementsByXPath("//a[@class='locEditDialog']");

            Assert.AreEqual(emptyFieldCountryMsg, validationMsgCountry.Text);
            Assert.AreEqual(emptyFieldTitleMsg, validationMsgTitle.Text);
            Assert.AreEqual(emptyFieldAddressMsg, validationMsgAddress.Text);

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
