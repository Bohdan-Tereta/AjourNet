using NUnit.Framework;
using AjourBT.Domain.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
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
    class CalendarTabTest
    {

        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string todayDate = DateTime.Now.ToString(String.Format("dd.MM.yyyy"));
        private string todayDatePlus14Days = DateTime.Now.AddDays(14).ToString(String.Format("dd.MM.yyyy"));

        private string todayDateMillisecondsFromEpoch = (new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,0,0,0).ToUniversalTime()
            - new DateTime(1970, 1, 1,0,0,0,DateTimeKind.Utc)).TotalMilliseconds.ToString();
        private string todayDatePlus14DaysMillisecondsFromEpoch = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
            .AddDays(14).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();

        WebDriverWait timeout = Browser.wait(5);

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
            Browser.ClickOnLink("Calendar");

            string Calendar = Browser.FindElementByLinkText("Calendar").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Calendar);
        }

        #region Paid Vacation

        [Test]
        public  void A_CreatePaidVacation_FromTodayToTodayPlus14_id49()
        {
            int paidVacationCountBeforeCreate = 0;
            int paidVacationCountAfterCreate = 0;

            Thread.Sleep(Timings.Default_ms*30);

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Browser.Wait(10);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");

            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SendKeysTo(dateVacationFrom, todayDate,true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, false);

            IWebElement calcWorkdays = Browser.FindElementByXPath("//label[@id='calcWorkdays']");
            IWebElement calcOverralDays = Browser.FindElementByXPath("//label[@id='calcOverralDays']");

            using (var db = new AjourBTForTestContext())
            {
                paidVacationCountBeforeCreate = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*10);
                paidVacationCountAfterCreate = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
            }

            Thread.Sleep(Timings.Default_ms*30);

            IWebElement paidVacationBar = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(paidVacationCountBeforeCreate, paidVacationCountAfterCreate);
            Assert.AreEqual("49", paidVacationBar.GetAttribute("data_id"));
            Assert.AreEqual("PaidVacation From: " + todayDate + " - To: " + todayDatePlus14Days + "", paidVacationBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void B_DeletePaidVacation_FromTodayToTodayPlus14_id49()
        {
            int beforeDelete = 0;
            int afterDelete = 0;
            Console.WriteLine(todayDateMillisecondsFromEpoch + " " + todayDatePlus14DaysMillisecondsFromEpoch);
            IWebElement paidVacation = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(paidVacation);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDelete = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterDelete = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
            }

            Assert.Less(afterDelete, beforeDelete);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
        }

        #endregion

        #region Unpaid Vacation

        [Test]
        public void C_CreateUnpaidVacation_FromTodayToTodayPlus14_id49()
        {
            int beforeCreateUnpaid = 0;
            int afterCreateUnpaid = 0;

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Browser.Wait(10);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SelectOption("event", 1);
            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, false);

            IWebElement calcWorkdays = Browser.FindElementByXPath("//label[@id='calcWorkdays']");
            IWebElement calcOverralDays = Browser.FindElementByXPath("//label[@id='calcOverralDays']");

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*10);
                afterCreateUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
            }

            Thread.Sleep(Timings.Default_ms*30);
            IWebElement paidVacationBar = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(beforeCreateUnpaid, afterCreateUnpaid);
            Assert.AreEqual("49", paidVacationBar.GetAttribute("data_id"));
            Assert.AreEqual("UnpaidVacation From: " + todayDate + " - To: " + todayDatePlus14Days + "", paidVacationBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void D_DeleteUnpaidVacation_FromTodayToTodayPlus14_id49()
        {
            int beforeDeleteUnpaid = 0;
            int afterDeleteUnpaid = 0;

            IWebElement unpaidVacation = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(unpaidVacation);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterDeleteUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
            }

            Assert.Less(afterDeleteUnpaid, beforeDeleteUnpaid);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
        }

        #endregion

        #region SickAbsence

        [Test]
        public void E_CreateSickAbsence_FromTodayToTodayPlus14_id49()
        {
            int beforeCreateSick = 0;
            int afterCreateSick = 0;

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Browser.Wait(10);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement sickText = Browser.FindElementByXPath("//input[@id='sickText']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SelectOption("event", 5);
            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, false);
            Browser.SendKeysTo(sickText, "Test Sickness", false);

            IWebElement calcWorkdays = Browser.FindElementByXPath("//label[@id='calcWorkdays']");
            IWebElement calcOverralDays = Browser.FindElementByXPath("//label[@id='calcOverralDays']");

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateSick = db.Sicknesses.Count();
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*10);
                afterCreateSick = db.Sicknesses.Count();
            }

            Thread.Sleep(Timings.Default_ms*30);
            IWebElement sickAbsenceBar = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(beforeCreateSick, afterCreateSick);
            Assert.AreEqual("49", sickAbsenceBar.GetAttribute("data_id"));
            Assert.AreEqual("SickAbsence From: " + todayDate + " - To: " + todayDatePlus14Days + "", sickAbsenceBar.GetAttribute("desc"));
            Assert.AreEqual("Test Sickness", sickAbsenceBar.GetAttribute("sick_desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void F_DeleteSickAbsence_FromTodayToTodayPlus14_id49()
        {
            int beforeDeleteSick = 0;
            int afterDeleteSick = 0;

            IWebElement sickAbsence = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(sickAbsence);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteSick = db.Sicknesses.Count();
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterDeleteSick = db.Sicknesses.Count();
            }
            Assert.Less(afterDeleteSick, beforeDeleteSick);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
        }

        #endregion

        #region OvertimeForReclaim

        [Test]
        public void G_CreateOvertimeForReclaim_TodayDate_id49()
        {
            int beforeCreateOvertimeForReclaim = 0;
            int afterCreateOvertimeForReclaim = 0;

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Browser.Wait(10);

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectOption("event", 3);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");
            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDate, true);

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*10);
                afterCreateOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
            }

            IWebElement overtimeForReclaimBar = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(beforeCreateOvertimeForReclaim, afterCreateOvertimeForReclaim);
            Assert.AreEqual("49", overtimeForReclaimBar.GetAttribute("data_id"));
            Assert.AreEqual("OvertimeForReclaim From: " + todayDate + " - To: " + todayDate + "", overtimeForReclaimBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void J_DeleteOvertimeForReclaim_TodayDate_id49()
        {
            int beforeDeleteOvertimeForReclaim = 0;
            int afterdeleteOvertimeForReclaim = 0;

            IWebElement overtimeForReclaim = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(overtimeForReclaim);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterdeleteOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
            }

            Assert.Less(afterdeleteOvertimeForReclaim, beforeDeleteOvertimeForReclaim);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
        }

        #endregion

        #region ReclaimedOvertime

        [Test]
        public void H_CreateReclaimedOvertime_TodayDatePlus14days_id49()
        {
            int beforeCreateReclaimedOvertime = 0;
            int afterCreatereclaimedOvertime = 0;

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SelectOption("event", 2);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");
            Browser.SendKeysTo(dateVacationFrom, todayDatePlus14Days, true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, true);

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateReclaimedOvertime = db.CalendarItems.Count();
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*20);
                afterCreatereclaimedOvertime = db.CalendarItems.Count();
            }

            IWebElement reclaimedOvertimeBar = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(beforeCreateReclaimedOvertime, afterCreatereclaimedOvertime);
            Assert.AreEqual("49", reclaimedOvertimeBar.GetAttribute("data_id"));
            Assert.AreEqual("ReclaimedOvertime From: " + todayDatePlus14Days + " - To: " + todayDatePlus14Days + "", reclaimedOvertimeBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void I_DeleteReclaimedOvertime_TodayDatePlus14days_id49()
        {
            int beforeDeleteReclaimedOvertime = 0;
            int afterDeleteReclaimedOvertime = 0;

            IWebElement reclaimedOvertime = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(reclaimedOvertime);

            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteReclaimedOvertime = db.CalendarItems.Count();
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterDeleteReclaimedOvertime = db.CalendarItems.Count();
            }
            Assert.Less(afterDeleteReclaimedOvertime, beforeDeleteReclaimedOvertime);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='49']");
        }

        #endregion

        #region PrivateMinus

        [Test]
        public void K_CreatePrivateMinus_TodayDate_id49()
        {
            int beforeCreatePrivateMinus = 0;
            int afterCreatePrivateMinus = 0;

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Browser.Wait(10);

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SelectOption("event", 4);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");
            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDate, true);

            using (var db = new AjourBTForTestContext())
            {
                beforeCreatePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms*10);
                afterCreatePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
            }

            IWebElement privateMinusBar = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");

            Assert.Less(beforeCreatePrivateMinus, afterCreatePrivateMinus);
            Assert.AreEqual("49", privateMinusBar.GetAttribute("data_id"));
            Assert.AreEqual("PrivateMinus From: " + todayDate + " - To: " + todayDate + "", privateMinusBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void L_DeletePrivateMinus_TodayDate_id49()
        {
            int beforeDeletePrivateMinus = 0;
            int afterDeletePrivateMinus = 0;

            IWebElement privateMinus = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
            Browser.ClickOnWebElement(privateMinus);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
            using (var db = new AjourBTForTestContext())
            {
                beforeDeletePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms*20);
                afterDeletePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
            }

            Assert.Less(afterDeletePrivateMinus, beforeDeletePrivateMinus);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
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
