using AjourBT.Domain.Abstract;
using AjourBT.Domain.Infrastructure;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;
using OpenQA.Selenium.Support.UI;
using AjourBT.Domain.Entities;
using System.Globalization;

namespace IntegrationTests.ACCTest
{
    [TestFixture]
    class AccountableBTs
    {

        private StringBuilder verificationErrors;
        private string baseURL = "http://localhost:50616/";
        private string username = "lark";
        private string password = "12345t";
        private string logIn = "/html/body/div[1]/section/section/form/fieldset/div/input";

        const string currentFutureBTs = "/html/body/div[1]/div/ul/li[1]";
        const string accountableBTs = "/html/body/div[1]/div/ul/li[2]";
        const string messages = "/html/body/div[1]/div/ul/li[3]";
        const string currentFutureBTsHref = currentFutureBTs + "/a";
        const string accountableBTsHref = accountableBTs + "/a";
        const string messagesHref = messages + "/a";
        const string tabsAcc = "tabsACC";
        const string tBodyBTACC = "//table[@id='btsViewExample']/tbody";//"/html/body/div[1]/div/div[2]/div[1]/div/div[2]/div[2]/table/tbody";
        const string departmentDropDownList = "/html/body/div[1]/div/div[1]/form/div/select";
        string searchFieldPath = "//form[@id='formForAcountableACC']/input[@id='searchInputACC']";//"/html/body/div[1]/div/div[2]/div[1]/div/div[1]/label/input";
        string defaultAccComment = "ВКО №   від   , cума:   UAH." + Environment.NewLine + "ВКО №   від   , cума:   USD.";

        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath(logIn);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("ACC");
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/ACCView"));
        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        [Test]
        public void ACC_AccountableBTsTabElementsAreOK()
        {
            Browser.ClickOnLink("Accountable BTs");
            IWebElement searchField = Browser.FindElementByXPath(searchFieldPath);
            Assert.AreEqual("search", searchField.GetAttribute("type"));
            Assert.AreEqual("Search...", searchField.GetAttribute("placeholder"));

            string btsViewDataTable = "//table[@id='btsViewExample']";
            IWebElement dataTable = Browser.FindElementByXPath(btsViewDataTable);
            Assert.AreEqual("btsView dataTable", dataTable.GetAttribute("class"));

            string dataTableHeadersPath = "/html/body/div[1]/div/div[2]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th";
            IReadOnlyCollection<IWebElement> headersPath = Browser.FindElementsByXPath(dataTableHeadersPath);

            string[] dataTableHeaders = { "EID", "Name", "From - To", "Location", "Habitation", "Flights" };
            for (int i=0; i < dataTableHeaders.Count() - 1; i++)
            {
                Assert.IsTrue(headersPath.ToArray()[i].Text.Contains(dataTableHeaders[i]));
                Assert.AreEqual("sorting", headersPath.ToArray()[i].GetAttribute("class"));
            }

            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(1, Browser.FindElementByXPath(tBodyBTACC).FindElements(By.ClassName("zebra")).Where(w => w.GetAttribute("class").Contains("odd")).Count());
            Assert.AreEqual(1, Browser.FindElementByXPath(tBodyBTACC).FindElements(By.ClassName("zebra")).Where(w => w.GetAttribute("class").Contains("even")).Count());
        }

        [Test]
        public void ACC_CheckAccountableBTsTable()
        {
            using (var db = new AjourBTForTestContext())
            {
                Browser.ClickOnLink("Accountable BTs");
                Thread.Sleep(Timings.Default_ms*2);
                string selectedDepartment = "";
                string searchString = "";

                List<BusinessTrip> query = IndexForAccountableBTs(db);

                IWebElement currentFutureBTsTable = Browser.FindElementByXPath(tBodyBTACC);
                Browser.WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                IWebElement[] rows = currentFutureBTsTable.FindElements(By.TagName("tr")).ToArray();
                Browser.WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));

                for (int i = 0; i < rows.Length; i++)
                {
                    IWebElement[] cells = rows[i].FindElements(By.TagName("td")).ToArray();
                    Browser.WebDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                    Assert.AreEqual(query[i].BTof.EID, cells[0].Text);

                    IWebElement rootElement;
                    if (query[i].Status != (BTStatus.Confirmed | BTStatus.Modified))
                    {
                        rootElement = cells[2].FindElement(By.TagName("a"));
                        Assert.IsTrue(rootElement.GetAttribute("href").Contains("/ACC/ShowAccountableBTData/" + query[i].BusinessTripID + "?selectedDepartment=" + selectedDepartment));
                    }
                    else
                    {
                        rootElement = cells[2].FindElement(By.TagName("span"));
                    }
                    Assert.AreEqual(rootElement.GetAttribute("id"), "ShowBTDataACC");
                    Assert.AreEqual(rootElement.GetAttribute("data-date-format"), "dd.mm.yy");

                    if (query[i].OrderEndDate.HasValue && query[i].OrderStartDate.HasValue)
                    {
                        IWebElement blueElement = rootElement.FindElement(By.TagName("blue"));
                        IWebElement bElement = blueElement.FindElement(By.TagName("b"));
                        Assert.AreEqual(bElement.Text, query[i].OrderStartDate.Value.ToString("dd.MM.yyyy") + " - " + query[i].OrderEndDate.Value.ToString("dd.MM.yyyy"));
                        IWebElement orangeElement = rootElement.FindElement(By.TagName("orange"));
                        Assert.AreEqual(orangeElement.Text, query[i].DaysInBtForOrder.ToString());
                        Assert.AreEqual(bElement.Text + " " + orangeElement.Text + "     " +
                            query[i].StartDate.ToString("dd.MM.yyyy") + " - " + query[i].EndDate.ToString("dd.MM.yyyy"), rootElement.Text);
                    }
                    else
                    {
                        Assert.AreEqual("     " + query[i].StartDate.ToString("dd.MM.yyyy") + " - " + query[i].EndDate.ToString("dd.MM.yyyy"), rootElement.Text);
                    }

                    Assert.AreEqual(query[i].Location.Title ?? "", cells[3].Text);
                    Assert.AreEqual(query[i].Habitation ?? "", cells[4].Text);
                    Assert.AreEqual(query[i].Flights ?? "", cells[5].Text);
                }
            }

        }

        List<BusinessTrip> IndexForAccountableBTs(AjourBTForTestContext db)
        {
            return (from bt in db.BusinessTrips as IEnumerable<BusinessTrip>
                    join e in db.Employees on bt.EmployeeID equals e.EmployeeID
                    join d in db.Departments on e.DepartmentID equals d.DepartmentID
                    where (e.DateDismissed == null
                          && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported))
                          && ((bt.OrderEndDate != null && bt.OrderEndDate.Value.Date <= DateTime.Now.ToLocalTimeAzure().Date) && (bt.OrderEndDate != null && bt.OrderEndDate.Value.Date >= (DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5))))))
                    orderby bt.OrderEndDate.Value descending, e.LastName
                    select bt).ToList();
        }


        [Test]
        public void ACC_AccountableBTsSearch()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> bts = IndexForAccountableBTs(db).ToList();
                Browser.ClickOnLink("Accountable BTs");
                Thread.Sleep(Timings.Default_ms*2);
                string defaultAccComment = "ВКО №   від   , cума:   UAH." + Environment.NewLine + "ВКО №   від   , cума:   USD.";
                List<string> searchStrings = new List<string>();
                List<int> counts = new List<int>();
                if (bts.Count > 0)
                {
                    searchStrings.Add(" " + bts.Last().BTof.EID + " ");
                    searchStrings.Add(" " + bts.Last().BTof.FirstName + " ");
                    searchStrings.Add(" " + bts.Last().BTof.LastName + " ");

                    counts.Add(bts.Where(b => b.BTof.EID == bts.Last().BTof.EID).Count());
                    counts.Add(bts.Where(b => b.BTof.FirstName == bts.Last().BTof.FirstName).Count());
                    counts.Add(bts.Where(b => b.BTof.LastName == bts.Last().BTof.LastName).Count());                   

                }
                int i = 0;
                foreach (string searchString in searchStrings)
                {
                    Browser.SendKeys(Browser.FindElementByXPath(searchFieldPath), searchString);
                    Browser.SendKeys(Browser.FindElementByXPath(searchFieldPath), Keys.Enter);
                    //Browser.SendEnter(Browser.FindElementByXPath(searchFieldPath));
                    Thread.Sleep(Timings.Default_ms*2);

                    List<BusinessTrip> query = IndexForAccountableBTs(db);
                    IWebElement currentFutureBTsTable = Browser.FindElement(By.XPath(tBodyBTACC), 10);
                    IWebElement[] rows = currentFutureBTsTable.FindElements(By.TagName("tr")).ToArray();
                    if (counts[i] != 0)
                    {
                        Assert.AreEqual(counts[i], rows.Count());
                    }
                    else
                    {

                        Assert.AreEqual(1, rows.Count());
                        Assert.AreEqual("No matching records found", rows[0].Text);

                    }

                    Browser.SendKeys(Browser.FindElementByXPath(searchFieldPath), String.Concat(Enumerable.Repeat(Keys.Backspace, searchString.Length)));
                    Thread.Sleep(Timings.Default_ms*2);

                    i++;

                }
            }

            Browser.SendKeysTo(Browser.FindElementByXPath(searchFieldPath), Keys.Enter, true);

        }

        [Test]
        public void ACC_EditBTDialogWindow()
        {
            Browser.ClickOnLink("Accountable BTs");
            Thread.Sleep(Timings.Default_ms*2);
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = IndexForAccountableBTs(db);
                int hrefsCount = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "ShowBTDataACC").Count();
                for (int i = 0; i < query.Count(); i++)
                {
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "ShowBTDataACC").Skip(i).First();
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*2);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("BT's Data"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("From"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("To"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Title"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Order From "));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Order To"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Number Of Days"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Purpose"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Manager"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Responsible"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Comment"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Invitation"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Habitation"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Flights"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("ACC's Comment"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains("Last modified by"));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].StartDate.ToString("dd.MM.yyyy")));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].EndDate.ToString("dd.MM.yyyy")));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].OrderStartDate.Value.ToString("dd.MM.yyyy")));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].OrderEndDate.Value.ToString("dd.MM.yyyy")));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Location.Title ?? ""));
                    //Assert.IsTrue(editBtDialogWindow.Text.Contains((query[i].DaysInBtForOrder.Value.ToString("dd.MM.yyyy"))));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Purpose ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Manager ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Responsible ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Comment ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Habitation ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].Flights ?? ""));
                    Assert.IsTrue(editBtDialogWindow.Text.Contains(query[i].AccComment ?? ""));
                    Browser.FindElementByClassName("ui-dialog").SendKeys(Keys.Escape);
                    Thread.Sleep(Timings.Default_ms*2);

                }
            }

        }
    }
}
