using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.BTMPage
{

    [TestFixture]
    class PrivateTripsTabTest
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
            username = "wsim";
            password = "1234rt";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("BTM");

            Assert.That(Browser.IsAt(baseURL + "Home/BTMView"));
            Browser.ClickOnLink("Private Trips");

            string PrivateTrips = Browser.FindElementByLinkText("Private Trips").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", PrivateTrips);
        }

        #region PrivateTrips Tab

        [Test]
        public void PrivateTripAdd()
        {
            int beforeAddPrivateTrip = 0;
            int afterAddPrivateTrip = 0;

            Thread.Sleep(Timings.Default_ms*30);
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'zebra')]");

            IWebElement addBtn = empList[0].FindElement(By.XPath("//a[@id='AddPrivateTrip']"));
            Browser.ClickOnWebElement(addBtn);

            Thread.Sleep(Timings.Default_ms*20);
            Tools.ClickOnTwoDatePickers(Browser.webDriver, Browser.wait(10));

            IWebElement saveBtn = Browser.FindElementByXPath("//button[@id='btnSavePrivateTrip']");

            using (var db = new AjourBTForTestContext())
            {
                beforeAddPrivateTrip = db.PrivateTrips.Count();
                Browser.ClickOnWebElement(saveBtn);
                Thread.Sleep(Timings.Default_ms*30);
                afterAddPrivateTrip = db.PrivateTrips.Count();
            }

            empList = Browser.FindElementsByXPath("//tbody[@id='PTViewBody']/tr[contains(@class,'zebra')]");

            Assert.Less(beforeAddPrivateTrip, afterAddPrivateTrip);
            Assert.IsTrue(empList[0].Text.Contains(Tools.TestDate));
        }

        [Test]
        public void PrivateTripEdit()
        {
            int beforeEditPrivateTrip = 0;
            int afterEditPrivateTrip = 0;

            IWebElement elemToEdit = Browser.FindElementByXPath("//tbody[@id='PTViewBody']/tr").FindElement(By.CssSelector("[href*='/BTM/PrivateTripEdit/8?searchString=']")); 

            Browser.ClickOnWebElement(elemToEdit);
            Thread.Sleep(Timings.Default_ms*20);

            using (var db = new AjourBTForTestContext())
            {
                beforeEditPrivateTrip = db.PrivateTrips.Count();                
            }

            IWebElement editStatDatePT = Browser.FindElementByXPath("//*[@id='editStartDatePT'] ");
            IWebElement editEndDatePT = Browser.FindElementByXPath("//*[@id='editEndDatePT'] ");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDatePT').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDatePT').removeAttribute('readonly')");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo(editStatDatePT, "28.07.2014", true);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo(editEndDatePT, "10.01.2000", true);
            Thread.Sleep(Timings.Default_ms*20);
            IWebElement dateValidationError = Browser.FindElementByXPath("//*[@id='EditPrivateTripForm']/fieldset/table/tbody/tr[2]/td[2]/span/span");
            Browser.SendKeysTo(editEndDatePT, "30.07.2014", true); 
            Thread.Sleep(Timings.Default_ms*20);
            IWebElement btnSavePrivateTrip = Browser.FindElementByXPath("//*[@id='btnSavePrivateTrip']/span");
            Browser.ClickOnWebElement(btnSavePrivateTrip);
            Thread.Sleep(Timings.Default_ms*20);

            using (var db = new AjourBTForTestContext())
            {
                afterEditPrivateTrip = db.PrivateTrips.Count();
                PrivateTrip privateTrip = db.PrivateTrips.Where(p => p.PrivateTripID == 8).FirstOrDefault(); 
                Assert.AreEqual(3, (privateTrip.EndDate - privateTrip.StartDate).Days + 1);
            }

            empList = Tools.GetAllTableData("//tr[contains(@class,'zebra')]", Browser.webDriver);

            Assert.AreEqual (afterEditPrivateTrip, beforeEditPrivateTrip);
            Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PTViewBody']/tr[1]/td[4]").Text.Contains("9"))         ;

        }

        [Test]
        public void PrivateTripDelete()
        {
            int beforeDeletePrivateTrip = 0;
            int afterDeletePrivateTrip = 0;

            IWebElement elemToDelete = Browser.FindElementByXPath("//tbody[@id='PTViewBody']/tr").FindElements(By.XPath("//a[@id='editPrivateTrip']"))
                                        .Where(e => e.Text.Contains(Tools.FirstDayOfMonth) && e.Text.Contains(Tools.TodayDate)).FirstOrDefault();

            Browser.ClickOnWebElement(elemToDelete);
            Thread.Sleep(Timings.Default_ms*20);
            IWebElement btnDeletePrivateTrip = Browser.FindElementByXPath("//button[@id='btnDeletePrivateTrip']");
            Browser.ClickOnWebElement(btnDeletePrivateTrip);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement btnOKDeletePrivateTrip = Browser.FindElementByXPath("//button[@id='btnOKDeletePrivateTrip']");

            using (var db = new AjourBTForTestContext())
            {
                beforeDeletePrivateTrip = db.PrivateTrips.Count();
                Browser.ClickOnWebElement(btnOKDeletePrivateTrip);
                Thread.Sleep(Timings.Default_ms*30);
                afterDeletePrivateTrip = db.PrivateTrips.Count();
            }

            empList = Tools.GetAllTableData("//tr[contains(@class,'zebra')]",Browser.webDriver);

            Assert.Less(afterDeletePrivateTrip, beforeDeletePrivateTrip);
            Assert.IsFalse(empList[0].Text.Contains(Tools.TestDate));

        }

        [Test]
        public void SearchFIeld_BadString_Empty()
        {
            IWebElement searchField = Browser.FindElementByXPath("//input[@id='seachInputVU']");
            Browser.SendKeysTo(searchField, "sdfsdfsd", true);
            Browser.SendKeysTo(searchField, Keys.Enter, false);

            Thread.Sleep(Timings.Default_ms*20);

            Assert.IsTrue(Browser.FindElementByClassName("dataTables_empty").Text.Contains(searchResultFail));
            Browser.SendKeysTo(searchField, Keys.Enter, true);
        }

        [Test]
        public void SearchField_Cruz_NotEmpty()
        {
            IWebElement searchField = Browser.FindElementByXPath("//input[@id='seachInputVU']");
            Thread.Sleep(Timings.Default_ms*15);
            Browser.SendKeysTo(searchField, "Cruz", true);
            Thread.Sleep(Timings.Default_ms*15);
            Browser.SendKeys(searchField, Keys.Enter);

            Thread.Sleep(Timings.Default_ms*15);

            empList = Tools.GetAllTableData("//tbody[@id='PTViewBody']/tr[contains(@class, zebra)]", Browser.webDriver);
            Assert.AreEqual(1, empList.Count);

            Browser.SendKeysTo(searchField, Keys.Enter, true);
        }

        [Test]
        public void SaveSortAfterUpdate()
        {
            List<string> eid = new List<string>();
            List<string> eidAfterUpdate = new List<string>();

            ReadOnlyCollection<IWebElement> columnToSort = Browser.FindElementsByXPath("//th[@class='sorting']");
            Browser.ClickOnWebElement(columnToSort[1]);
            ReadOnlyCollection<IWebElement> BTMprivateTripsTable = Browser.FindElementsByXPath("//tbody[@id='PTViewBody']/tr");
            foreach (IWebElement item in BTMprivateTripsTable)
            {
                string[] td = item.Text.Split(' ');
                eid.Add(td[1]);
            }

            IWebElement privateTripLink = Browser.FindElementByXPath("//a[@id='editPrivateTrip']");
            Browser.ClickOnWebElement(privateTripLink);
            Thread.Sleep(Timings.Default_ms*15);

            //IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            IWebElement saveBtn = Browser.FindElementByXPath("//button[@id='btnSavePrivateTrip']");
            Browser.ClickOnWebElement(saveBtn);
            Thread.Sleep(Timings.Default_ms*15);

            BTMprivateTripsTable = Browser.FindElementsByXPath("//tbody[@id='PTViewBody']/tr");
            foreach (IWebElement item in BTMprivateTripsTable)
            {
                string[] updatedTd = item.Text.Split(' ');
                eidAfterUpdate.Add(updatedTd[1]);
            }

            Assert.AreEqual(eid.Count, eidAfterUpdate.Count);
            Assert.IsTrue(eid.SequenceEqual(eidAfterUpdate));
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
