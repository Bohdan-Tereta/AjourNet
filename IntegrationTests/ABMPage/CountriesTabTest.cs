using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.ABMPage
{
    [TestFixture]
    class CountriesTabTest
    {
        #region Countries

        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string warningMsg = "This location has associated data or it's a default location";
        WebDriverWait timeout = Browser.wait(5);

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            username = "tmas";
            password = "aserty";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("ABM");

            Assert.That(Browser.IsAt(baseURL + "Home/ABMView"));
            string Countries = Browser.FindElementByLinkText("Countries").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Countries);
        }

        [Test]
        public void TestAddCountry()
        {
            int beforeAddCountry = 0;
            int afterAddCountry = 0;

            IWebElement addCountryBtn = Browser.FindElementByXPath("//a[@id='CreateCountry']");
            addCountryBtn.Click();

            timeout.Until(m => m.FindElement(By.XPath("//div[@id='Create Country']")));
            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Create Country']");

            IWebElement CountryName = modalWindow.FindElement(By.XPath("//input[@id='CountryName']"));
            IWebElement Comment = modalWindow.FindElement(By.XPath("//input[@id='Comment']"));

            IWebElement saveBtn = modalWindow.FindElement(By.XPath("//div[@class='ui-dialog-buttonset']"));
            Browser.SendKeysTo(CountryName, "Nepal", false);
            Browser.SendKeysTo(Comment, "Test Comment", false);
            Thread.Sleep(Timings.Default_ms*15);

            using (var db = new AjourBTForTestContext())
            {
                beforeAddCountry = db.Countries.Count();
                Browser.ClickOnWebElement(saveBtn);
                Thread.Sleep(Timings.Default_ms*10);
                afterAddCountry = db.Countries.Count();
            }

            string classValue = Browser.FindElementByLinkText("Nepal").GetAttribute("class");

            Assert.Less(beforeAddCountry, afterAddCountry);
            Assert.AreEqual("countryEditDialog", classValue);
        }

        [Test]
        public void TestAddThesameCountry_CannotAdd()
        {
            int beforeAddCountry = 0;
            int afterAddCountry = 0;

            IWebElement addCountryBtn = Browser.FindElementByXPath("//a[@id='CreateCountry']");
            addCountryBtn.Click();

            timeout.Until(m => m.FindElement(By.XPath("//div[@id='Create Country']")));
            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Create Country']");

            IWebElement CountryName = modalWindow.FindElement(By.XPath("//input[@id='CountryName']"));
            IWebElement Comment = modalWindow.FindElement(By.XPath("//input[@id='Comment']"));

            IWebElement saveBtn = modalWindow.FindElement(By.XPath("//div[@class='ui-dialog-buttonset']"));
            Browser.SendKeysTo(CountryName, "Nepal", false);
            Browser.SendKeysTo(Comment, "Test Comment", false);
            Thread.Sleep(Timings.Default_ms*10);

            using (var db = new AjourBTForTestContext())
            {
                beforeAddCountry = db.Countries.Count();
                Browser.ClickOnWebElement(saveBtn);
                afterAddCountry = db.Countries.Count();
            }
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(beforeAddCountry, afterAddCountry);
        }


        [Test]
        public void TestAddThesameCountry_ValidationError()
        {
            string expected = "Country with same Name already exists";

            IWebElement addCountryBtn = Browser.FindElementByXPath("//a[@id='CreateCountry']");
            addCountryBtn.Click();

            timeout.Until(m => m.FindElement(By.XPath("//div[@id='Create Country']")));
            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Create Country']");

            IWebElement CountryName = modalWindow.FindElement(By.XPath("//input[@id='CountryName']"));
            IWebElement Comment = modalWindow.FindElement(By.XPath("//input[@id='Comment']"));

            IWebElement saveBtn = modalWindow.FindElement(By.XPath("//div[@class='ui-dialog-buttonset']"));
            Browser.SendKeysTo(CountryName, "Nepal", false);
            Browser.SendKeysTo(Comment, "Test Comment", false);
            Thread.Sleep(Timings.Default_ms*10);
               
            Browser.ClickOnWebElement(saveBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            Assert.AreEqual(expected, Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table/tbody/tr[1]/td[2]/span"));        
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
        }


        [Test]
        public void TestDeleteCountry_WarningMsg()
        {
            IWebElement countryToDelete = Browser.FindElementByLinkText("Ukraine");
            Browser.ClickOnWebElement(countryToDelete);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Edit Country']");
            IWebElement btnDeleteCountry = modalWindow.FindElement(By.XPath("//button[@id='btnDeleteCountry']"));
            Browser.ClickOnWebElement(btnDeleteCountry);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement confirmModalWindow = Browser.FindElementByXPath("//div[@id='deleteCountry-Confirm']");
            IWebElement warningField = Browser.FindElementByXPath("//div[@id='deleteCountry-Confirm']/p");

            Assert.AreEqual(warningMsg, warningField.Text);

            IWebElement closeBtn = Browser.FindElementByXPath("/html/body/div[6]/div[1]/a/span");
            Browser.ClickOnWebElement(closeBtn);

            IWebElement closeMainBtn = Browser.FindElementByXPath("/html/body/div[4]/div[1]/a/span");
            Browser.ClickOnWebElement(closeMainBtn);

            Thread.Sleep(Timings.Default_ms*15);
            string classValue = Browser.FindElementByLinkText("Ukraine").GetAttribute("class");

            Assert.AreEqual("countryEditDialog", classValue);
        }

        [Test]
        public void TestDeleteCountry_OK()
        {
            int beforeDeleteCountry = 0;
            int afterDeleteCountry = 0;

            IReadOnlyCollection<IWebElement> elemCount = Browser.FindElementsByXPath("//div[@id='CountryData']/table/tbody/tr");
            int countBeforeDelete = elemCount.Count;

            IWebElement countryToDelete = Browser.FindElementByLinkText("Nepal");
            Browser.ClickOnWebElement(countryToDelete);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement btnDeleteCountry = Browser.FindElementByXPath("//button[@id='btnDeleteCountry']");
            Browser.ClickOnWebElement(btnDeleteCountry);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement OKDelete = Browser.FindElementByXPath("/html/body/div[6]/div[3]/div/button[1]");

            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteCountry = db.Countries.Count();
                Browser.ClickOnWebElement(OKDelete);
                Thread.Sleep(Timings.Default_ms*15);
                afterDeleteCountry = db.Countries.Count();
            }

            IReadOnlyCollection<IWebElement> elemAfterDelete = Browser.FindElementsByXPath("//div[@id='CountryData']/table/tbody/tr");
            int countAfterDelete = elemAfterDelete.Count;

            Assert.Less(afterDeleteCountry, beforeDeleteCountry);
            Assert.Less(countAfterDelete, countBeforeDelete);
        }

        [Test]
        public void TestEditCountryName()
        {
            Country editedCountry = new Country();

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement countryToEdit = Browser.FindElementByLinkText("Poland");
            Browser.ClickOnWebElement(countryToEdit);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement nameField = Browser.FindElementByXPath("//input[@id='CountryName']");
            Browser.SendKeysTo(nameField, "Polandium", true);
            IWebElement saveBtn = Browser.FindElementByXPath("//button[@id='btnSaveCountry']");
            Browser.ClickOnWebElement(saveBtn);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                editedCountry = db.Countries.Where(c => c.CountryName == "Polandium").FirstOrDefault();
            }

            string classValue = Browser.FindElementByLinkText("Polandium").GetAttribute("class");
            Assert.IsNotNull(editedCountry);
            Assert.AreEqual("countryEditDialog", classValue);
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
