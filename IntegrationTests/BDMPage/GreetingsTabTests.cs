using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.BDMPage
{
    [TestFixture]
    public class GreetingsTabTests
    {
        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;

        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "sdea";
            password = "trew21";
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("BDM");
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/BDMView"));
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        [Test]
        public void GreetingsTab()
        {
           
            //Assert
            Assert.IsTrue(Browser.HasElement("BDM"));
            Assert.IsTrue(Browser.HasElement("Greetings"));
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='CreateGreeting']/span/span"));
            Assert.AreEqual(Browser.GetText("//*[@id='tabsBDM']/ul/li/a"), "Greetings");
            Assert.AreEqual(Browser.GetText("//*[@id='GreetingData']/table/tbody/tr[1]/th"), "GreetingHeader");
            Browser.Refresh();
        }
 
        [Test]
        public void AddGreeting_InValidInput_ValidationErrors()
        {

            List<Greeting> listBeforeAdd = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBeforeAdd = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Browser.ClickByXPath("//*[@id='CreateGreeting']/span/span");

            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Create Greeting']"), "Create Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");
         
            Browser.ClickByXPath("/html/body/div[3]/div[11]/div/button/span");

            List<Greeting> listAfterAdd = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
                listAfterAdd = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[1]/td[2]/span/span"));
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[1]/td[2]/span/span"), "The GreetingHeader field is required.");
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[2]/td[2]/span/span"));
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[2]/td[2]/span/span"), "The GreetingBody field is required.");
            Assert.AreEqual(listBeforeAdd.Count, listAfterAdd.Count);
        }

        [Test]
        public void AddGreeting_ValidInput_CanCreateGreeting()
        {
            List<Greeting> listBeforeAdd = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
               listBeforeAdd = (from d in db.Greetings.AsEnumerable()   select d).ToList();
            }
            Browser.Refresh();
            Browser.ClickByXPath("//*[@id='CreateGreeting']/span/span");

            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Create Greeting']"), "Create Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='createGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");
            Browser.SendKeysTo("GreetingHeader", "AddedGreeting", false);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@id='GreetingBody']"), "Be happy!");
            Browser.ClickByXPath("/html/body/div[3]/div[11]/div/button/span");

            List<Greeting> listAfterAdd = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
               listAfterAdd = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.IsTrue(Browser.HasElement("AddedGreeting"));
            Assert.AreEqual(listBeforeAdd.Count + 1, listAfterAdd.Count);
        }

        [Test]
        public void EditGreeting_ValidInput_CanEditGreeting()
        {
            List<Greeting> listBeforeAdd = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBeforeAdd = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Browser.Refresh();
            Browser.ClickByXPath("//*[@id='GreetingData']/table/tbody/tr[2]/td/a");
            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Greeting']"), "Edit Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");
            Browser.SendKeysTo("GreetingHeader", "EditedGreeting", true);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@id='GreetingBody']"), "Be brave and happy!");
            Browser.ClickByXPath("//*[@id='btnSaveGreeting']/span/span");

            List<Greeting> listAfterAdd = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
                listAfterAdd = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.IsTrue(Browser.HasElement("EditedGreeting"));
            Assert.AreEqual(listBeforeAdd.Count, listAfterAdd.Count);
        }

        [Test]
        public void EditGreeting_InValidInput_ValidationErrors()
        {

            List<Greeting> listBeforeAdd = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBeforeAdd = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Browser.Refresh();
            Browser.ClickByXPath("//*[@id='GreetingData']/table/tbody/tr[2]/td/a");

            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Greeting']"), "Edit Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");
            Browser.SendKeysTo("GreetingHeader", "", true);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@id='GreetingBody']"), "");
            Browser.ClickByXPath("//*[@id='btnSaveGreeting']/span/span");

            List<Greeting> listAfterAdd = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
                listAfterAdd = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[2]/span/span"));
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[2]/span/span"), "The GreetingHeader field is required.");
            Assert.AreEqual(listBeforeAdd.Count, listAfterAdd.Count);
        }

        [Test]
        public void EditGreetingForm_DeleteButton_DeleteCancel()
        {
            List<Greeting> listBeforeDelete = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBeforeDelete = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Browser.Refresh();
            Browser.ClickByXPath("//*[@id='GreetingData']/table/tbody/tr[2]/td/a");

            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Greeting']"), "Edit Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");
          
            Browser.ClickByXPath("//*[@id='btnDeleteGreeting']/span/span");
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='ui-dialog-title-deleteGreeting-Confirm']"));
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='btnCancelDeleteGreeting']/span/span"));
            Browser.ClickByXPath("//*[@id='btnCancelDeleteGreeting']/span/span");

            List<Greeting> listAfterDelete = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
                listAfterDelete = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.AreEqual(listBeforeDelete.Count, listAfterDelete.Count);
        }

        [Test]
        public void EditGreetingForm_DeleteButton_DeleteConfirm()
        {
            List<Greeting> listBeforeDelete = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBeforeDelete = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Browser.Refresh();
            Browser.ClickByXPath("//*[@id='GreetingData']/table/tbody/tr[2]/td/a");

            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Greeting']"), "Edit Greeting");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[1]/td[1]/label"), "GreetingHeader");
            Assert.AreEqual(Browser.GetText("//*[@id='editGreetingForm']/fieldset/table/tbody/tr[2]/td[1]/label"), "GreetingBody");

            Browser.ClickByXPath("//*[@id='btnDeleteGreeting']/span/span");
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='ui-dialog-title-deleteGreeting-Confirm']"));
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='btnCancelDeleteGreeting']/span/span"));
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='OKDelete']/span/span"));
            Browser.ClickByXPath("//*[@id='OKDelete']/span/span");
            
            List<Greeting> listAfterDelete = new List<Greeting>();
            using (AjourBTForTestContext data = new AjourBTForTestContext())
            {
                listAfterDelete = (from d in data.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.AreEqual(listBeforeDelete.Count - 1, listAfterDelete.Count);
        }

        [Test]
        public void TestPagination()
        {
            List<Greeting> listBefore = new List<Greeting>();
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                listBefore = (from d in db.Greetings.AsEnumerable() select d).ToList();
            }
            Assert.IsTrue(Browser.HasElement("1"));
            List<IWebElement> linksOnPage = Browser.FindElementsByXPath("//a[@class='greetingEditDialog']").ToList();
            //Browser.ClickByXPath("//*[@id='tabs-6']/div[2]/div/ul/li[3]/a");
            Browser.ClickByXPath("//a[@rel='next']");
            Thread.Sleep(Timings.Default_ms*20);
            linksOnPage.AddRange(Browser.FindElementsByXPath("//a[@class='greetingEditDialog']").ToList());
         
            Assert.AreEqual(listBefore.Count, linksOnPage.Count);
        }
    }
}
