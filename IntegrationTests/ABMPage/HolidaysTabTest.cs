using AjourBT.Domain.Concrete;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.ABMPage
{
    [TestFixture]
    class HolidaysTabTest
    {
        #region Holidays

        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
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
            Browser.Wait(10);
            Browser.ClickOnLink("ABM");

            Assert.That(Browser.IsAt(baseURL + "Home/ABMView"));
            Browser.ClickOnLink("Holidays");

            string Holidays = Browser.FindElementByLinkText("Holidays").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Holidays);
        }

        #region Ukraine

        [Test]
        public void AddHoliday_UkraineTodayDate2015()
        {
            int beforeAddHolidayUkraine = 0;
            int afterAddHolidayUkraine = 0;
            Browser.Refresh();
            Browser.ClickOnLink("Holidays");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "1")
            {
                Browser.SelectOption("selectedCountryID", 0);
            }

            Browser.Wait(10);

            IWebElement CreateHoliday = Browser.FindElementByXPath("//a[@id='CreateHoliday']");
            Browser.ClickOnWebElement(CreateHoliday);
            Thread.Sleep(Timings.Default_ms*15);
            
            IWebElement Title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            IWebElement buttonSaveHoliday = Browser.FindElementByXPath("/html/body/div[4]/div[11]/div/button/span");

            Browser.SendKeysTo(Title, "Test Holiday", false);
            Browser.SendKeysTo(HolidayDate, Tools.TodayDate, false);
            Browser.SendKeysTo(HolidayComment, "Test Comment", false);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                beforeAddHolidayUkraine = db.Holidays.Count();
                Browser.ClickOnWebElement(buttonSaveHoliday);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
               // Browser.ClickOnLink("Holidays");
                Thread.Sleep(Timings.Default_ms*20);
                afterAddHolidayUkraine = db.Holidays.Count();
            }

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.Less(beforeAddHolidayUkraine, afterAddHolidayUkraine);
            Assert.IsTrue(HolidaysList.Last().Text.Contains(Tools.TodayDate));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void AddTheSameHolidayExists_CannotCreate()
        {
            int beforeAddHolidayUkraine = 0;
            int afterAddHolidayUkraine = 0;

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "1")
            {
                Browser.SelectOption("selectedCountryID", 0);
            }

            Browser.Wait(10);

            IWebElement CreateHoliday = Browser.FindElementByXPath("//a[@id='CreateHoliday']");
            Browser.ClickOnWebElement(CreateHoliday);
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement Title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            IWebElement buttonSaveHoliday = Browser.FindElementByXPath("/html/body/div[4]/div[11]/div/button/span");

            Browser.SendKeysTo(Title, "Test Holiday", false);
            Browser.SendKeysTo(HolidayDate, Tools.TodayDate, false);
            Browser.SendKeysTo(HolidayComment, "Test Comment", false);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                beforeAddHolidayUkraine = db.Holidays.Count();
                Browser.ClickOnWebElement(buttonSaveHoliday);
                Thread.Sleep(Timings.Default_ms*20);
              //  Browser.ClickOnLink("Holidays");
                Thread.Sleep(Timings.Default_ms*20);
                afterAddHolidayUkraine = db.Holidays.Count();
            }
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Holidays");

            Assert.AreEqual(beforeAddHolidayUkraine, afterAddHolidayUkraine);
        }

        [Test]
        public void AddTheSameHolidayExists_ValidationError()
        {
            string expectedForTitle = "Holiday with same Title already exist";
            string expectedForDates = "Holiday with same Date already exist";

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "1")
            {
                Browser.SelectOption("selectedCountryID", 0);
            }

            Browser.Wait(10);

            IWebElement CreateHoliday = Browser.FindElementByXPath("//a[@id='CreateHoliday']");
            Browser.ClickOnWebElement(CreateHoliday);
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement Title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            IWebElement buttonSaveHoliday = Browser.FindElementByXPath("/html/body/div[4]/div[11]/div/button/span");

            Browser.SendKeysTo(Title, "Test Holiday", false);
            Browser.SendKeysTo(HolidayDate, Tools.TodayDate, false);
            Browser.SendKeysTo(HolidayComment, "Test Comment", false);
           
            Browser.ClickOnWebElement(buttonSaveHoliday);
            Thread.Sleep(Timings.Default_ms * 10);
            Assert.AreEqual(expectedForTitle, Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table/tbody/tr[2]/td[2]/span"));
            Assert.AreEqual(expectedForDates, Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table/tbody/tr[3]/td[2]/span"));     

            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Holidays");
        }

        [Test]
        public void EditHoliday_Ukraine_LastDateOfYear2015()
        {
            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "1")
            {
                Browser.SelectOption("selectedCountryID", 0);
            }

            Browser.Wait(10);

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            Browser.ClickOnWebElement(HolidaysList.LastOrDefault());

            Browser.Wait(10);
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement btnSaveHoliday = Browser.FindElementByXPath("//button[@id='btnSaveHoliday']");

            Browser.SendKeysTo(HolidayComment, "Changed Test Comment", true);
            Browser.SendKeysTo(HolidayDate, "31.12.2015", true);
            Browser.ClickOnWebElement(btnSaveHoliday);
            Thread.Sleep(Timings.Default_ms*20);
            //should be removed after Issue #446 done
            //Browser.ClickOnLink("Holidays");
            Thread.Sleep(Timings.Default_ms*20);
            HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.IsTrue(HolidaysList.Last().Text.Contains("31.12.2015"));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void TestDeleteHoliday_Ukraine2015()
        {
            int beforeDeleteHolidayUkraine = 0;
            int afterDeleteHolidayUkraine = 0;

            Browser.SelectOption("selectedCountryID", 0);
            Thread.Sleep(Timings.Default_ms*30);

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int beforeDeleteCount = HolidaysList.Count;

            Browser.ClickOnWebElement(HolidaysList.Last());

            Thread.Sleep(Timings.Default_ms*15);            
            IWebElement btnDeleteHoliday = Browser.FindElementByXPath("//button[@id='btnDeleteHoliday']");
            Browser.ClickOnWebElement(btnDeleteHoliday);
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement OKDelete = Browser.FindElementByXPath("/html/body/div[6]/div[3]/div/button[1]/span");

            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteHolidayUkraine = db.Holidays.Count();
                Browser.ClickOnWebElement(OKDelete);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
           //     Browser.ClickOnLink("Holidays");
                Thread.Sleep(Timings.Default_ms*20);
                afterDeleteHolidayUkraine = db.Holidays.Count();
            }

            HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int afterDeleteCount = HolidaysList.Count;

            Assert.Less(afterDeleteHolidayUkraine, beforeDeleteHolidayUkraine);
            Assert.Less(afterDeleteCount, beforeDeleteCount);
        }
        #endregion

        #region Poland

        [Test]
        public void AddHoliday_PolandTodayDate2015()
        {
            int beforeAddHolidayPoland = 0;
            int afterAddHolidayPoland = 0;

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "2")
            {
                Browser.SelectOption("selectedCountryID", 1);
            }

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement CreateHoliday = Browser.FindElementByXPath("//a[@id='CreateHoliday']");


                Browser.ClickOnWebElement(CreateHoliday);


            IWebElement Title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            IWebElement buttonSaveHoliday = Browser.FindElementByXPath("/html/body/div[4]/div[11]/div/button/span");

            Browser.SelectOption("dropDownCountry", 2);
            Browser.SendKeysTo(Title, "Test Holiday", false);
            Browser.SendKeysTo(HolidayDate, Tools.TodayDate, false);
            Browser.SendKeysTo(HolidayComment, "Test Comment", false);
            using (var db = new AjourBTForTestContext())
            {
                beforeAddHolidayPoland = db.Holidays.Count();
                Browser.ClickOnWebElement(buttonSaveHoliday);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
              //  Browser.ClickOnLink("Holidays");
                Thread.Sleep(Timings.Default_ms*10);
                afterAddHolidayPoland = db.Holidays.Count();
            }

            Browser.SelectOption("selectedCountryID", 1);
            Thread.Sleep(Timings.Default_ms*20);
            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.Less(beforeAddHolidayPoland, afterAddHolidayPoland);
            Assert.IsTrue(HolidaysList.Last().Text.Contains(Tools.TodayDate));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void EditHoliday_Poland_LastDateOfYear2015()
        {
            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "2")
            {
                Browser.SelectOption("selectedCountryID", 1);
            }

            Thread.Sleep(Timings.Default_ms*20);

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            Browser.ClickOnWebElement(HolidaysList.LastOrDefault());

            Browser.Wait(10);
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement btnSaveHoliday = Browser.FindElementByXPath("//button[@id='btnSaveHoliday']");

            Browser.SendKeysTo(HolidayDate, "31.12.2015", true);
            Browser.ClickOnWebElement(btnSaveHoliday);
            Thread.Sleep(Timings.Default_ms*20);
            //should be removed after Issue #446 done
           // Browser.ClickOnLink("Holidays");
            Browser.SelectOption("selectedCountryID", 1);
            Thread.Sleep(Timings.Default_ms*20);
            HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.IsTrue(HolidaysList.Last().Text.Contains("31.12.2015"));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void TestDeleteHoliday_Poland2015()
        {
            int beforeDeleteHolidayPoland = 0;
            int afterDeleteHolidayPoland = 0;

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "2")
            {
                Browser.SelectOption("selectedCountryID", 1);
            }

            Browser.Wait(10);
            Thread.Sleep(Timings.Default_ms*20);
            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int beforeDeleteCount = HolidaysList.Count;

            Browser.ClickOnWebElement(HolidaysList.Last());

            Browser.Wait(10);
            IWebElement btnDeleteHoliday = Browser.FindElementByXPath("//button[@id='btnDeleteHoliday']");
            Browser.ClickOnWebElement(btnDeleteHoliday);
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement OKDelete = Browser.FindElementByXPath("/html/body/div[6]/div[3]/div/button[1]/span");
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteHolidayPoland = db.Holidays.Count();
                Browser.ClickOnWebElement(OKDelete);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
                //Browser.ClickOnLink("Holidays");
                //Thread.Sleep(Timings.Default_ms*10);
                afterDeleteHolidayPoland = db.Holidays.Count();
            }

            Browser.SelectOption("selectedCountryID", 1);
            Thread.Sleep(Timings.Default_ms*20);
            HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int afterDeleteCount = HolidaysList.Count;

            Assert.Less(afterDeleteHolidayPoland, beforeDeleteHolidayPoland);
            Assert.Less(afterDeleteCount, beforeDeleteCount);
        }

        #endregion

        #region Sweden

        [Test]
        public void AddHoliday_SwedenTodayDate2015()
        {
            int beforeAddHolidaySweden = 0;
            int afterAddHolidaySweden = 0;

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "3")
            {
                Browser.SelectOption("selectedCountryID", 2);
            }

            Browser.Wait(10);

            IWebElement CreateHoliday = Browser.FindElementByXPath("//a[@id='CreateHoliday']");
            Browser.ClickOnWebElement(CreateHoliday);

            Browser.Wait(10);
            IWebElement Title = Browser.FindElementByXPath("//input[@id='Title']");
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement HolidayComment = Browser.FindElementByXPath("//input[@id='HolidayComment']");
            IWebElement buttonSaveHoliday = Browser.FindElementByXPath("/html/body/div[4]/div[11]/div/button/span");

            Browser.SelectOption("dropDownCountry", 3);
            Browser.SendKeysTo(Title, "Test Holiday1", false);
            Browser.SendKeysTo(HolidayDate, Tools.TodayDate, false);
            Browser.SendKeysTo(HolidayComment, "Test Comment", false);

            using (var db = new AjourBTForTestContext())
            {
                beforeAddHolidaySweden = db.Holidays.Count(h => h.CountryID == 3);
                Browser.ClickOnWebElement(buttonSaveHoliday);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
               // Browser.ClickOnLink("Holidays");
                Browser.SelectOption("selectedCountryID", 2);
                Thread.Sleep(Timings.Default_ms*20);
                afterAddHolidaySweden = db.Holidays.Count(h => h.CountryID == 3);
            }

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.Less(beforeAddHolidaySweden, afterAddHolidaySweden);
            Assert.IsTrue(HolidaysList.Last().Text.Contains(Tools.TodayDate));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void EditHoliday_Sweden_LastDateOfYear2015()
        {
            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "3")
            {
                Browser.SelectOption("selectedCountryID", 2);
            }

            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            Browser.ClickOnWebElement(HolidaysList.LastOrDefault());

            Browser.Wait(10);
            IWebElement HolidayDate = Browser.FindElementByXPath("//input[@id='HolidayDate']");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('HolidayDate').removeAttribute('readonly')");
            IWebElement btnSaveHoliday = Browser.FindElementByXPath("//button[@id='btnSaveHoliday']");

            Browser.SendKeysTo(HolidayDate, "31.12.2015", true);
            Browser.ClickOnWebElement(btnSaveHoliday);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectOption("selectedCountryID", 2);
            Thread.Sleep(Timings.Default_ms*20);
            HolidaysList = Browser.FindElementsByXPath("//tbody[@id='HolidaysList']/tr");

            Assert.IsTrue(HolidaysList.Last().Text.Contains("31.12.2015"));
            Assert.IsTrue(HolidaysList.Last().Text.Contains("Test Holiday"));
        }

        [Test]
        public void TestDeleteHoliday_Sweden2015()
        {
            int beforeDeleteHolidaySweden = 0;
            int afterDeleteHolidaySweden = 0;

            IWebElement selectCountry = Browser.FindElementByXPath("//select[@id='selectedCountryID']/option[@selected='selected']");
            if (selectCountry.GetAttribute("value") != "3")
            {
                Browser.SelectOption("selectedCountryID", 2);
            }

            Thread.Sleep(Timings.Default_ms*20);

            IReadOnlyCollection<IWebElement> HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int beforeDeleteCount = HolidaysList.Count;
            Browser.ClickOnWebElement(HolidaysList.Last());

            IWebElement btnDeleteHoliday = Browser.FindElementByXPath("//button[@id='btnDeleteHoliday']");
            Browser.ClickOnWebElement(btnDeleteHoliday);

            IWebElement OKDelete = Browser.FindElementByXPath("/html/body/div[6]/div[3]/div/button[1]/span");
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteHolidaySweden = db.Holidays.Count();
                Browser.ClickOnWebElement(OKDelete);
                Thread.Sleep(Timings.Default_ms*20);
                //should be removed after Issue #446 done
                //Browser.ClickOnLink("Holidays");
                Thread.Sleep(Timings.Default_ms*10);
                afterDeleteHolidaySweden = db.Holidays.Count();
            }

            Browser.SelectOption("selectedCountryID", 2);
            Thread.Sleep(Timings.Default_ms*20);
            HolidaysList = Browser.FindElementsByXPath("//a[@class='holidayEditDialog']");
            int afterDeleteCount = HolidaysList.Count;

            Assert.Less(afterDeleteHolidaySweden, beforeDeleteHolidaySweden);
            Assert.Less(afterDeleteCount, beforeDeleteCount);
        }

        #endregion

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        #endregion
    }
}
