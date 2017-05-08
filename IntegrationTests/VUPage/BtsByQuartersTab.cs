using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace IntegrationTests.VUPage
{
    [TestFixture]
    public class BtsByQuartersTab
    {
        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private AjourBTForTestContext db = new AjourBTForTestContext();
        private int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
        private string nothingFound = "No matching records found";

        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "ayou";
            password = "123456";
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("VU");
            Browser.ClickOnLink("BTs by Quarters");
            verificationErrors = new StringBuilder();
            Thread.Sleep(Timings.Default_ms*20);
            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {

            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");




        }

        [Test]
        [TestCase("//*[@id='vuByDatesSecondHeader']/th[1]", "EID", true)]
        [TestCase("//*[@id='vuByDatesSecondHeader']/th[2]", "Name", true)]
        [TestCase("//*[@id='vuByDatesSecondHeader']/th[1]", "EID", false)]
        [TestCase("//*[@id='vuByDatesSecondHeader']/th[2]", "Name", false)]
        public void BTs_SortingTable(string path, string column, bool ascending)
        {
            //Arrange
            //Act
            Browser.Refresh();
            Browser.ClickByXPath(path);
            if (!ascending)
                Browser.ClickByXPath(path);
            var firstEmploeeID = Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[1]");
            var firstEmploeeName = Browser.GetText("//*[@id='notUnderlineText']/a/m");
            Employee name = (from bt in db.BusinessTrips.AsEnumerable()
                             where bt.StartDate.Year == selectedYear && bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                     || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                                     || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)
                             orderby bt.BTof.LastName
                             select bt.BTof).FirstOrDefault();

            //Assert
            if (column == "EID")
                if (ascending)
                    Assert.True(firstEmploeeID.StartsWith("a"));
                else
                    Assert.IsFalse(firstEmploeeID.StartsWith("a"));
            else
                if (ascending)
                    Assert.AreEqual(firstEmploeeName, "Bishop Billy");
                else
                    Assert.IsFalse(firstEmploeeName.StartsWith(name.LastName));
        }

        [Test]
        public void ShowBTInformationWindowIsOpened_CorrectData()
        {
            //Arrange
            Browser.Refresh();
            Browser.ClickOnLink("BTs by Quarters");
            Thread.Sleep(Timings.Default_ms*20);

            //Act
            Browser.ClickByXPath("//*[@id='ShowBTInformation']");
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.AreEqual("Billy Bishop (smey)", Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[1]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("HO/LR", Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[2]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().AddDays(10).ToString("dd.MM.yyyy"), Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[3]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().AddDays(20).ToString("dd.MM.yyyy"), Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[4]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);

        }

        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Browser.Refresh();
            Browser.ClickOnLink("BTs by Quarters");
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchForQuarterVU']");

            Thread.Sleep(Timings.Default_ms * 20);
            Browser.SendKeysTo(searchInput, serchString, true);
            Browser.SendEnter(searchInput);
            Browser.Wait(5);
            Thread.Sleep(Timings.Default_ms*20);

            if (!isFound)
            {
                Thread.Sleep(Timings.Default_ms * 20);
                Assert.AreEqual(Browser.GetText("//td[@class='dataTables_empty']"), nothingFound);

            }
            else
            {
                Thread.Sleep(Timings.Default_ms * 20);
                //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[2]/div/div[1]/div/div[2]/div[2]/table/tbody/tr/td[1]"), "rkni");
                Assert.AreEqual(Browser.GetText("//table[@id='BTsInQuarterForViewerexample']/tbody/tr/td"), "rkni");
            }
        }

        [Test]
        public void YearDropDownList_DefaultYear()
        {
            //Arrange
            string expected = Browser.GetText("/html/body/div[1]/div/div[2]/form/div/select[1]/option[5]");
            string eidFirst = Browser.GetText("//table[@id='BTsInQuarterForViewerexample']/tbody/tr/td");

            //Act

            //Assert
            Assert.AreEqual("2015", expected);
            Assert.AreEqual("tmas", eidFirst);
        }

        public void YearDropDownList_CurrentMonth()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string monthNow = DateTime.Now.ToLocalTimeAzure().Month.ToString();

            //Act
            string resultMonth = (helper.CustomDisplayMonthInEnglishHelp(monthNow)).ToString();

            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectOption("selectedKey", 0);
            string expected = Browser.GetText("/html/body/div[1]/div/div[2]/form/div/select[1]/option[5]");
            Thread.Sleep(Timings.Default_ms*20);
            string eidFirst = Browser.GetText("/html/body/div[1]/div/div[2]/div/div[1]/div/div[2]/div[2]/table/tbody/tr/td[1]");
            Thread.Sleep(Timings.Default_ms*20);
            string month = Browser.GetText("/html/body/div[1]/div/div[2]/div/div[1]/div/div[2]/div[1]/div/table/thead/tr/th[3]");
            Thread.Sleep(Timings.Default_ms*20);

            //Act     

            //Assert
            Assert.AreEqual("current month", expected);
            Assert.AreEqual(month, resultMonth);
            Assert.AreEqual("tmas", eidFirst);
        }

        [Test]
        public void YearDropDownList_selectYear()
        {
            //Act
            Browser.SelectOption("selectedKey", (selectedYear + 1).ToString());
            Thread.Sleep(Timings.Default_ms*20);
            string expected = "2015";
            Thread.Sleep(Timings.Default_ms * 20);
            string eidFirst = Browser.GetText("//table[@id='BTsInQuarterForViewerexample']/tbody/tr/td[1]");

            //Assert
            Assert.AreEqual(selectedYear.ToString(), expected);
            Assert.AreEqual("tmas", eidFirst);
        }

        [Test]
        public void TableData()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickOnLink("BTs by Quarters");
            Thread.Sleep(Timings.Default_ms*20);
            int year = DateTime.Now.ToLocalTimeAzure().Year;

            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                List<Employee> employeeBts = (from e in db.Employees.AsEnumerable()
                                              where e.IsUserOnly == false
                                              select e).ToList();

                Employee empBT = employeeBts.FirstOrDefault();
                if (empBT != null)
                {
                    ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='BTsInQuarterForViewerexample']/tbody/tr");
                    Thread.Sleep(Timings.Default_ms*20);

                    foreach (var element in empTable)
                    {
                        if (element.Text.Contains(empBT.EID))
                        {
                            Assert.IsTrue(element.Text.Contains(empBT.FirstName));
                            Assert.IsTrue(element.Text.Contains(empBT.LastName));
                        }
                    }
                    Assert.AreEqual(employeeBts.Count, empTable.Count());
                }
            }
        }

    }
}
