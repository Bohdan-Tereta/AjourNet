using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using IntegrationTests.ABMPage;
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

namespace IntegrationTests.VUPage
{
    [TestFixture]
    public class CalendarTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string todayDate = DateTime.Now.ToString(String.Format("dd.MM.yyyy"));
        private string todayDatePlus14Days = DateTime.Now.AddDays(14).ToString(String.Format("dd.MM.yyyy"));

        private string todayDateMillisecondsFromEpoch = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToUniversalTime()
            - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
        private string todayDatePlus14DaysMillisecondsFromEpoch = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
            .AddDays(14).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();

        WebDriverWait timeout = Browser.wait(5);

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "ayou";
            password = "123456";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("VU");
            Thread.Sleep(Timings.Default_ms * 20);

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            Browser.ClickOnLink("Calendar");


        }

        public void LoginAsABM()
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
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }


        [Test]
        public void Default_AllDeptsts()
        {
            //Arrange

            string Calendar = Browser.FindElementByLinkText("Calendar").GetCssValue("color");

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual("rgba(225, 112, 9, 1)", Calendar);
        }

        [Test]
        public void Employees_DropdownListOfDepartments_DefaultDepartment()
        {
            //Arrange

            string expected = "All Departments";

            //Act
            //Browser.Refresh(); 
            //Browser.ClickOnLink("Calendar");
            //Thread.Sleep(Timings.Default_ms*20);
            string defaultDept = Browser.GetText("//*[@id='depDropList']/option[1]");
            int employees = Browser.Count("//*[contains(@id,'rowheader')]/span");

            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(expected, defaultDept);
        }

        [Test]
        public void Employees_CountInDefaultDepartment()
        {
            //Arrange
            string expected = "All Departments";

            //Act
            //Browser.Refresh();
            //Browser.ClickOnLink("Calendar");
            //Thread.Sleep(Timings.Default_ms*20);
            string defaultDept = Browser.GetText("//*[@id='depDropList']/option[1]");
            int employees = Browser.Count("//*[contains(@id,'rowheader')]/span") - 1;
            List<Employee> empsFromRepo = EmployeesTest.EmpsInRepositoryCount().AsEnumerable().Where(e => e.DateDismissed == null && e.IsUserOnly == false).ToList();
            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(expected, defaultDept);
            Assert.AreEqual(employees, empsFromRepo.Count());
        }


        [Test]
        public void Employees_DropdownListOfDepartments_SelectedDepartment()
        {
            //Arrange
            string expected = "BOARD";
            List<Employee> empsFromRepo = new List<Employee>();

            //Act
            //Browser.Refresh();
            //Thread.Sleep(Timings.Default_ms*10);
            //Browser.ClickOnLink("Calendar");
            //Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("/html/body/div[1]/div/div[5]/form/div[1]/select");
            Browser.ClickByXPath("/html/body/div[1]/div/div[5]/form/div[1]/select/option[2]");
            int employees = Browser.Count("//*[contains(@id,'rowheader')]/span");

            using (var db = new AjourBTForTestContext())
            {
                empsFromRepo = db.Employees
                                 .Where(emp => emp.Department.DepartmentName == "BOARD"
                                        && emp.DateDismissed == null).ToList();
            }


            //Assert
            Assert.AreEqual(expected, Browser.GetText("//*[@id='depDropList']/option[2]"));
            Assert.AreEqual(empsFromRepo.Count(), 10);

        }

        [Test]
        public void ListEmployees_TableData()
        {
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 20);
            List<Employee> empsFromRepo = EmployeesTest.EmpsInRepositoryCount().AsEnumerable().Where(e => e.DateDismissed == null && e.IsUserOnly == false).ToList();

            Employee employee = empsFromRepo.FirstOrDefault();

            if (employee != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[contains(@id,'rowheader')]/span");
                foreach (var element in empTable)
                {
                    if (element.Text.Contains(employee.LastName))
                    {
                        Assert.IsTrue(element.Text.Contains(employee.LastName));
                        Assert.IsTrue(element.Text.Contains(employee.FirstName));
                    }
                }
                Assert.AreEqual(empsFromRepo.Count, empTable.Count() - 1);
            }
        }


        [Test]
        public void Calendar_SelectMounthDatePicker()
        {
            //Browser.Refresh();
            //Thread.Sleep(Timings.Default_ms*10);
            //Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 20);
            Browser.ClickByXPath("//*[@id='wtrInput']/td[2]/img");
            Browser.ClickByXPath("//*[@id='ui-datepicker-div']/div/div/select[1]");
            Browser.SelectByValue("//*[@id='ui-datepicker-div']/div/div/select[1]", "10");
            Browser.ClickOnLink("1");
            Thread.Sleep(Timings.Default_ms * 20);

            Browser.ClickByXPath("//*[@id='wtrInput']/td[4]/img");
            Browser.ClickByXPath("//*[@id='ui-datepicker-div']/div/div/select[1]");
            Browser.SelectByValue("//*[@id='ui-datepicker-div']/div/div/select[1]", "11");
            Browser.ClickOnLink("1");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[contains(@class,'month')]/div");
            foreach (IWebElement e in empTable.AsEnumerable())
            {
                if (e.Text.Contains("October"))
                    Assert.IsTrue(e.Text.Contains("October"));
            }


        }
        [Test]
        public void Calendar_PrintButton()
        {
            //Browser.Refresh();
            //Thread.Sleep(Timings.Default_ms*10);
            //Browser.ClickOnLink("Calendar");
            //Thread.Sleep(Timings.Default_ms*20);
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='pdfPrintBtn']/span"));
            Assert.AreEqual(Browser.GetText("//*[@id='pdfPrintBtn']/span"), "Print");

        }

        [Test]
        public void PaidVacation_Added()
        {
            int paidVacationCountBeforeCreate = 0;
            int paidVacationCountAfterCreate = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);

            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");

            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, false);

            IWebElement calcWorkdays = Browser.FindElementByXPath("//label[@id='calcWorkdays']");
            IWebElement calcOverralDays = Browser.FindElementByXPath("//label[@id='calcOverralDays']");

            using (var db = new AjourBTForTestContext())
            {
                paidVacationCountBeforeCreate = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms * 10);
                paidVacationCountAfterCreate = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
            }

            Thread.Sleep(Timings.Default_ms * 30);

            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement paidVacationBar = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Assert.Less(paidVacationCountBeforeCreate, paidVacationCountAfterCreate);
            Assert.AreEqual("PaidVacation From: " + todayDate + " - To: " + todayDatePlus14Days + "", paidVacationBar.GetAttribute("desc"));
        }


        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void PaidVacation_Deleted()
        {
            int beforeDelete = 0;
            int afterDelete = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement paidVacation = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Browser.ClickOnWebElement(paidVacation);

            Thread.Sleep(Timings.Default_ms * 15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDelete = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms * 20);
                afterDelete = db.Vacations.Count(item => item.Type == VacationType.PaidVacation);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.Less(afterDelete, beforeDelete);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttBlue'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
        }


        [Test]
        public void UnpaidVacation_Added()
        {
            int beforeCreateUnpaid = 0;
            int afterCreateUnpaid = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

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
                Thread.Sleep(Timings.Default_ms * 10);
                afterCreateUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
            }

            Thread.Sleep(Timings.Default_ms * 30);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement paidVacationBar = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Assert.Less(beforeCreateUnpaid, afterCreateUnpaid);
            Assert.AreEqual("UnpaidVacation From: " + todayDate + " - To: " + todayDatePlus14Days + "", paidVacationBar.GetAttribute("desc"));
        }


        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void UnpaidVacation_Deleted()
        {
            int beforeDeleteUnpaid = 0;
            int afterDeleteUnpaid = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement unpaidVacation = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Browser.ClickOnWebElement(unpaidVacation);

            Thread.Sleep(Timings.Default_ms * 15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms * 20);
                afterDeleteUnpaid = db.Vacations.Count(item => item.Type == VacationType.UnpaidVacation);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.Less(afterDeleteUnpaid, beforeDeleteUnpaid);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttRed'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
        }


        [Test]
        public void SickAbsence_Added()
        {
            int beforeCreateSick = 0;
            int afterCreateSick = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

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
                Thread.Sleep(Timings.Default_ms * 10);
                afterCreateSick = db.Sicknesses.Count();
            }

            Thread.Sleep(Timings.Default_ms * 30);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement sickAbsenceBar = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Assert.Less(beforeCreateSick, afterCreateSick);
            Assert.AreEqual("SickAbsence From: " + todayDate + " - To: " + todayDatePlus14Days + "", sickAbsenceBar.GetAttribute("desc"));
            Assert.AreEqual("Test Sickness", sickAbsenceBar.GetAttribute("sick_desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void SickAbsence_Deleted()
        {
            int beforeDeleteSick = 0;
            int afterDeleteSick = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement sickAbsence = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Browser.ClickOnWebElement(sickAbsence);

            Thread.Sleep(Timings.Default_ms * 15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteSick = db.Sicknesses.Count();
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms * 20);
                afterDeleteSick = db.Sicknesses.Count();
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.Less(afterDeleteSick, beforeDeleteSick);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttViolet'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
        }

        [Test]
        public void OvertimeForReclaim_Added()
        {
            int beforeCreateOvertimeForReclaim = 0;
            int afterCreateOvertimeForReclaim = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

            IWebElement selectEvent = Browser.FindElementByXPath("//select[@id='event']");
            IWebElement dateVacationFrom = Browser.FindElementByXPath("//input[@id='dateVacationFrom']");
            IWebElement dateVacationTo = Browser.FindElementByXPath("//input[@id='dateVacationTo']");
            IWebElement CreateEvent = Browser.FindElementByXPath("//a[@id='CreateEvent']");

            Thread.Sleep(Timings.Default_ms * 20);
            Browser.SelectOption("event", 3);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationFrom').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('dateVacationTo').removeAttribute('readonly')");
            Browser.SendKeysTo(dateVacationFrom, todayDatePlus14Days, true);
            Browser.SendKeysTo(dateVacationTo, todayDatePlus14Days, true);

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms * 10);
                afterCreateOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement overtimeForReclaimBar = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Assert.Less(beforeCreateOvertimeForReclaim, afterCreateOvertimeForReclaim);
            Assert.AreEqual("OvertimeForReclaim From: " + todayDatePlus14Days + " - To: " + todayDatePlus14Days + "", overtimeForReclaimBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void OvertimesForReclaim_Deleted()
        {
            int beforeDeleteOvertimeForReclaim = 0;
            int afterdeleteOvertimeForReclaim = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);

            IWebElement overtimeForReclaim = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "']");
            Browser.ClickOnWebElement(overtimeForReclaim);

            Thread.Sleep(Timings.Default_ms * 20);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnLink("Delete");
                Thread.Sleep(Timings.Default_ms * 20);
                Browser.Refresh();
                Thread.Sleep(Timings.Default_ms * 30);
                afterdeleteOvertimeForReclaim = db.Overtimes.Count(o => o.Type == OvertimeType.Paid);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Thread.Sleep(Timings.Default_ms * 20);
            Setup();
            Thread.Sleep(Timings.Default_ms * 30);
            Assert.Less(afterdeleteOvertimeForReclaim, beforeDeleteOvertimeForReclaim);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttMagenta'][@dayfrom='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@dayto='" + todayDatePlus14DaysMillisecondsFromEpoch + "'][@data_id='45']");
        }

        [Test]
        public void OvertimeReclaimedOvertime_Added()
        {
            int beforeCreateReclaimedOvertime = 0;
            int afterCreatereclaimedOvertime = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
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
            Browser.SendKeysTo(dateVacationFrom, todayDate, true);
            Browser.SendKeysTo(dateVacationTo, todayDate, true);

            using (var db = new AjourBTForTestContext())
            {
                beforeCreateReclaimedOvertime = db.CalendarItems.Count();
                Browser.ClickOnWebElement(CreateEvent);
                Thread.Sleep(Timings.Default_ms * 20);
                afterCreatereclaimedOvertime = db.CalendarItems.Count();
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement reclaimedOvertimeBar = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "']");
            Assert.Less(beforeCreateReclaimedOvertime, afterCreatereclaimedOvertime);
            Assert.AreEqual("ReclaimedOvertime From: " + todayDate + " - To: " + todayDate + "", reclaimedOvertimeBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void OvertimeReclaimedOvertime_Deleted()
        {
            int beforeDeleteReclaimedOvertime = 0;
            int afterDeleteReclaimedOvertime = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement reclaimedOvertime = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "']");
            Browser.ClickOnWebElement(reclaimedOvertime);

            Thread.Sleep(Timings.Default_ms * 20);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteReclaimedOvertime = db.CalendarItems.Count();
                IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms * 20);
                afterDeleteReclaimedOvertime = db.CalendarItems.Count();
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);

            Assert.Less(afterDeleteReclaimedOvertime, beforeDeleteReclaimedOvertime);
            IWebElement deletedElem = Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "']");
        }

        [Test]
        public void PrivateMinus_Added()
        {
            int beforeCreatePrivateMinus = 0;
            int afterCreatePrivateMinus = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement calendarField = Browser.FindElementByXPath("//div[@class='calendar']");
            Browser.ClickOnWebElement(calendarField);
            Thread.Sleep(Timings.Default_ms * 20);

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
                Thread.Sleep(Timings.Default_ms * 10);
                afterCreatePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement privateMinusBar;
            try
            {
                privateMinusBar = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
            }
            catch (Exception)
            {
                privateMinusBar = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='38']"); 
            }
            Assert.Less(beforeCreatePrivateMinus, afterCreatePrivateMinus);
            Assert.AreEqual("PrivateMinus From: " + todayDate + " - To: " + todayDate + "", privateMinusBar.GetAttribute("desc"));
        }

        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void PrivateMinus_Deleted()
        {
            int beforeDeletePrivateMinus = 0;
            int afterDeletePrivateMinus = 0;
            TeardownTest();
            LoginAsABM();
            Browser.ClickOnLink("Calendar");
            Thread.Sleep(Timings.Default_ms * 30);
            IWebElement privateMinus;
            try
            {
                privateMinus = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
            }
            catch (Exception)
            {
                privateMinus = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='38']");
            }
            Browser.ClickOnWebElement(privateMinus);

            Thread.Sleep(Timings.Default_ms * 20);
            IWebElement Delete = Browser.FindElementByXPath("//a[@id='Delete']");
            using (var db = new AjourBTForTestContext())
            {
                beforeDeletePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
                Browser.ClickOnWebElement(Delete);
                Thread.Sleep(Timings.Default_ms * 20);
                afterDeletePrivateMinus = db.Overtimes.Count(o => o.Type == OvertimeType.Private);
            }
            Thread.Sleep(Timings.Default_ms * 20);
            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.Less(afterDeletePrivateMinus, beforeDeletePrivateMinus);
            try
            {
                privateMinus = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='49']");
            }
            catch (Exception)
            {
                privateMinus = Browser.FindElementByXPath("//div[@class='bar ganttYellow'][@dayfrom='" + todayDateMillisecondsFromEpoch + "'][@dayto='" + todayDateMillisecondsFromEpoch + "'][@data_id='38']");
            }
        }

        //TestcaseForIssue#639 Changing StartDate and OrderStartDate of FinishedBt as PUmakes junk journeys appear 
        //TestCaseForIssue#640 After deleting journey for which ReclaimedOvertime exists that overtime isn't deleted 
        [Test]
        public void ShortenFinishedBT_NoJunkJourneysAppear()
        {
            //Arrange 
            Thread.Sleep(Timings.Default_ms * 20);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('calendarFromDate').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('calendarToDate').removeAttribute('readonly')");
            IWebElement calendarFrom = Browser.FindElementByXPath("//*[@id='calendarFromDate']");
            IWebElement calendarTo = Browser.FindElementByXPath("//*[@id='calendarToDate']");

            Browser.SendKeysTo(calendarFrom, "01.01.2013", true);
            Browser.SendKeysTo(calendarTo, "10.02.2013", true);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("calendarAbsenceSubmitButton"), 2));
            Thread.Sleep(Timings.Default_ms * 35);
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttDarkGreen'][@dayfrom='1358546400000'][@dayto='1358546400000'][@data_id='73']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttDarkGreen'][@dayfrom='1359237600000'][@dayto='1359237600000'][@data_id='73']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttGreen'][@dayfrom='1358632800000'][@dayto='1359151200000'][@data_id='73']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='1359756000000'][@dayto='1359756000000'][@data_id='73']")); 

            Thread.Sleep(Timings.Default_ms * 10);
                Browser.ClickOnLink("Log off");
            string baseURL = "http://localhost:50616/";
        string username;
        string password;


            Browser.webDriver.Manage().Window.Maximize();
            username = "apat";
            password = "lokmop";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("PU");
            Thread.Sleep(Timings.Default_ms * 5); 
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms * 5);
            Browser.SelectOption("selectedYear", "2013");
            Thread.Sleep(Timings.Default_ms * 10); 

            //Act 
            Browser.ClickOnLink("tmas");
            Thread.Sleep(Timings.Default_ms * 20);
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDateFinishedBTPU').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateFinishedBTPU').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFinishedBTPU').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFinishedBTPU').removeAttribute('readonly')");

            IWebElement from = Browser.FindElementByXPath("//*[@id='editStartDateFinishedBTPU']");
            IWebElement to = Browser.FindElementByXPath("//*[@id='editEndDateFinishedBTPU']");
            IWebElement orderFrom = Browser.FindElementByXPath("//*[@id='orderStartDateFinishedBTPU']");
            IWebElement orderTo = Browser.FindElementByXPath("//*[@id='orderEndDateFinishedBTPU']");

            string startDate = "21.01.2013";
            string endDate = "26.01.2013";
            string orderStartDate = "20.01.2013";
            string orderEndDate = "27.01.2013";



            Browser.SendKeysTo(from, startDate, true);
            Browser.SendKeysTo(to, endDate, true);

            Browser.SendKeysTo(orderFrom, orderStartDate, true);
            Browser.SendKeysTo(orderTo, orderEndDate, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms * 20);

            TeardownTest();
            Setup();
            Thread.Sleep(Timings.Default_ms * 20);

            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('calendarFromDate').removeAttribute('readonly')");
            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('calendarToDate').removeAttribute('readonly')");
            calendarFrom = Browser.FindElementByXPath("//*[@id='calendarFromDate']");
            calendarTo = Browser.FindElementByXPath("//*[@id='calendarToDate']");

            Browser.SendKeysTo(calendarFrom, "01.01.2013", true);
            Browser.SendKeysTo(calendarTo, "10.02.2013", true);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("calendarAbsenceSubmitButton"), 2));
            Thread.Sleep(Timings.Default_ms * 35);

            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttDarkGreen'][@dayfrom='1358632800000'][@dayto='1358632800000'][@data_id='73']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttDarkGreen'][@dayfrom='1359237600000'][@dayto='1359237600000'][@data_id='73']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttGreen'][@dayfrom='1358719200000'][@dayto='1359151200000'][@data_id='73']"));
            int exceptionsCount = 0;
            try
            {
                Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttDarkGreen'][@dayfrom='1358546400000'][@dayto='1358546400000'][@data_id='73']"));
            }
            catch (Exception)
            {
                exceptionsCount++; 
            }
            try
            {
                Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttGreen'][@dayfrom='1358632800000'][@dayto='1359151200000'][@data_id='73']"));
            }
            catch (Exception)
            {
                exceptionsCount++;
            }
            try
            {
                Assert.IsNotNull(Browser.FindElementByXPath("//div[@class='bar ganttOrange'][@dayfrom='1359756000000'][@dayto='1359756000000'][@data_id='73']"));
            }
            catch (Exception)
            {
                exceptionsCount++;
            }
            TeardownTest();
            Setup();

            //Assert   
            Assert.AreEqual(3, exceptionsCount); 
            
        }

    }

}