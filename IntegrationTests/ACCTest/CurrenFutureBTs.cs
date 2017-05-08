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
    class CurrenFutureBTs
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
        const string tBodyBTACC = "/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody";
        const string departmentDropDownList = "/html/body/div[1]/div/div[1]/form/div/select";
        string searchFieldPath = "/html/body/div[1]/div/div[1]/form/div/input";
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
            Thread.Sleep(Timings.Default_ms*30);
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
        public void ACC_JqueryTabsAreOK()
        {
            //Arrange 

            //Act 

            //Assert
            Assert.NotNull(Browser.HasElement("ACC"));
            Assert.AreEqual(Browser.GetText(currentFutureBTsHref), "Current/Future BTs");
            Assert.AreEqual(Browser.GetText(accountableBTsHref), "Accountable BTs");
            Assert.AreEqual(Browser.GetText(messagesHref), "Messages");
            Assert.IsTrue(Browser.IsTabSelectedAndActive(currentFutureBTs));
            Assert.IsTrue(Browser.HasJQueryTab(tabsAcc));
        }

        [Test]
        public void ACC_CurrentFutureBTsTabElementsAreOK()
        {
            string[] text = new string[] 
            { "All Departments", "BOARD", "DEPT1", "DEPT2", "DEPT3", "DEPT4", "DEPT5", "DEPT6", "DEPT7", "DEPT8"
            };
            string[] values = new string[] 
            { "", "BOARD", "DEPT1", "DEPT2", "DEPT3", "DEPT4", "DEPT5", "DEPT6", "DEPT7", "DEPT8"
            };

            string id = "selectedDepartment";
            string selectedValue = null;

            Browser.ClickOnLink("Current/Future BTs");
            Assert.AreEqual("True", Browser.IsDropDownList(Browser.FindElementByXPath(departmentDropDownList), id, values, text, selectedValue));
            IWebElement searchField = Browser.FindElementByXPath(searchFieldPath);
            Assert.AreEqual("searchInputACC", searchField.GetAttribute("id"));
            Assert.AreEqual("search", searchField.GetAttribute("type"));
            Assert.AreEqual("Search...", searchField.GetAttribute("placeholder"));

            string btsViewDataTable = "/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[1]/div/table";
            IWebElement dataTable = Browser.FindElementByXPath(btsViewDataTable);
            Assert.AreEqual("btsView dataTable", dataTable.GetAttribute("class"));

            string dataTableHeadersPath = "/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th";
            string[] dataTableHeaders = { "EID", "Name", "From - To", "Location", "Habitation", "Flights" };
            for (int i = 1; i <= dataTableHeaders.Count(); i++)
            {
                IWebElement header = Browser.FindElementByXPath(dataTableHeadersPath + "[" + i + "]");
                Assert.AreEqual(dataTableHeaders[i - 1], header.Text);
                Assert.AreEqual("sorting", header.GetAttribute("class"));
            }
            int countOfBts;
            using (var db = new AjourBTForTestContext())
            {
                countOfBts = SelectCurrentFutureBTsACC(db, "", "", defaultAccComment).Count;
            }
            Assert.AreEqual((int)Math.Ceiling((double)countOfBts / 2), Browser.FindElementByXPath(tBodyBTACC).FindElements(By.ClassName("zebra")).Where(w => w.GetAttribute("class").Contains("odd")).Count());
            Assert.AreEqual((int)Math.Floor((double)countOfBts / 2), Browser.FindElementByXPath(tBodyBTACC).FindElements(By.ClassName("zebra")).Where(w => w.GetAttribute("class").Contains("even")).Count());
        }

        [Test]
        public void ACC_CheckCurrentFutureBTsTable()
        {
            using (var db = new AjourBTForTestContext())
            {
                Browser.ClickOnLink("Current/Future BTs");
                Thread.Sleep(Timings.Default_ms*12);
                string selectedDepartment = "";
                string searchString = "";

                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, selectedDepartment, searchString.Trim(), defaultAccComment);

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
                        Assert.IsTrue(rootElement.GetAttribute("href").Contains("/ACC/EditReportedBT/" + query[i].BusinessTripID + "?selectedDepartment=" + selectedDepartment));
                    }
                    else
                    {
                        rootElement = cells[2].FindElement(By.TagName("span"));
                    }
                    Assert.AreEqual(rootElement.GetAttribute("id"), "EditReportedBTACC");
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
                        Assert.AreEqual(query[i].StartDate.ToString("dd.MM.yyyy") + " - " + query[i].EndDate.ToString("dd.MM.yyyy"), rootElement.Text);
                    }

                    Assert.AreEqual(query[i].Location.Title ?? "", cells[3].Text);
                    Assert.AreEqual(query[i].Habitation ?? "", cells[4].Text);
                    Assert.AreEqual(query[i].Flights ?? "", cells[5].Text);
                }
            }

        }

        private static List<BusinessTrip> SelectCurrentFutureBTsACC(AjourBTForTestContext db, string selectedDepartment, string searchString, string defaultAccComment)
        {
            List<BusinessTrip> query = (from bt in db.BusinessTrips.ToList()
                                        join e in db.Employees on bt.EmployeeID equals e.EmployeeID
                                        join d in db.Departments on e.DepartmentID equals d.DepartmentID
                                        where (e.IsUserOnly == false && (selectedDepartment == null || selectedDepartment == String.Empty || d.DepartmentName == selectedDepartment)
                                              && (e.DateDismissed == null
                                                    && (e.EID.ToLower().Contains(searchString.ToLower())
                                                            || e.FirstName.ToLower().Contains(searchString.ToLower())
                                                            || e.LastName.ToLower().Contains(searchString.ToLower())))
                                              && ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Modified))
                                              && ((bt.EndDate.Date >= DateTime.Now.ToLocalTimeAzure().Date)
                                                    || (bt.AccComment == null || bt.AccComment == ""
                                                    || bt.AccComment == defaultAccComment))))
                                        orderby bt.StartDate, e.LastName
                                        select bt).ToList();
            return query;
        }




        [Test]
        public void ACC_CurrentFutureBTsDepartmentsDropDownList()
        {
            using (var db = new AjourBTForTestContext())
            {
                string searchString = "";
                string defaultAccComment = "ВКО №   від   , cума:   UAH." + Environment.NewLine + "ВКО №   від   , cума:   USD.";
                for (int department = 1; department < db.Departments.Count(); department++)
                {
                    Browser.SelectOption("selectedDepartment", department);
                    Thread.Sleep(Timings.Default_ms*12);

                    string selectedDepartment = db.Departments.OrderBy(dep => dep.DepartmentName).ToArray()[department - 1].DepartmentName;
                    List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, selectedDepartment, searchString.Trim(), defaultAccComment);
                    IWebElement currentFutureBTsTable = Browser.FindElement(By.XPath(tBodyBTACC), 10);
                    IWebElement[] rows = currentFutureBTsTable.FindElements(By.TagName("tr")).ToArray();
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (query != null && query.Count != 0)
                        {
                            IWebElement[] cells = rows[i].FindElements(By.TagName("td")).ToArray();
                            Assert.AreEqual(query[i].BTof.EID, cells[0].Text);
                            Assert.AreEqual(query[i].BTof.LastName + " " + query[i].BTof.FirstName, cells[1].Text);

                            IWebElement rootElement;
                            if (query[i].Status != (BTStatus.Confirmed | BTStatus.Modified))
                            {
                                rootElement = cells[2].FindElement(By.TagName("a"));
                                Assert.IsTrue(rootElement.GetAttribute("href").Contains("/ACC/EditReportedBT/" + query[i].BusinessTripID + "?selectedDepartment=" + selectedDepartment));
                            }
                            else
                            {
                                rootElement = cells[2].FindElement(By.TagName("span"));
                            }
                            Assert.AreEqual(rootElement.GetAttribute("id"), "EditReportedBTACC");
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
                                Assert.AreEqual(query[i].StartDate.ToString("dd.MM.yyyy") + " - " + query[i].EndDate.ToString("dd.MM.yyyy"), rootElement.Text);
                            }

                            Assert.AreEqual(query[i].Location.Title ?? "", cells[3].Text);
                            Assert.AreEqual(query[i].Habitation ?? "", cells[4].Text);
                            Assert.AreEqual(query[i].Flights ?? "", cells[5].Text);
                        }
                        else
                        {
                            IWebElement[] cell = rows[i].FindElements(By.TagName("td")).ToArray();
                            Assert.AreEqual(1, rows.Length);
                            Assert.AreEqual("No matching records found", cell[0].Text);
                        }
                    }
                }
                Browser.SelectOption("selectedDepartment", 0);
            }
        }

        [Test]
        public void ACC_CurrentFutureBTsSearch()
        {
            using (var db = new AjourBTForTestContext())
            {
                string selectedDepartment = "";
                string defaultAccComment = "ВКО №   від   , cума:   UAH." + Environment.NewLine + "ВКО №   від   , cума:   USD.";
                List<string> searchStrings = new List<string> { " dsto ", "dsto", " Rachel", " Stanley  ", "Stanley Rachel", ".2012", "RB/M", "Krakow", "500", "" };
                foreach (string searchString in searchStrings)
                {
                    Browser.SendKeysTo("searchInputACC", searchString, true);
                    Browser.SendEnter(Browser.FindElementByXPath(searchFieldPath));
                    Thread.Sleep(Timings.Default_ms*12);

                    List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, selectedDepartment, searchString.Trim(), defaultAccComment);
                    IWebElement currentFutureBTsTable = Browser.FindElement(By.XPath(tBodyBTACC), 10);
                    IWebElement[] rows = currentFutureBTsTable.FindElements(By.TagName("tr")).ToArray();
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (query != null && query.Count != 0)
                        {
                            IWebElement[] cells = rows[i].FindElements(By.TagName("td")).ToArray();
                            Assert.AreEqual(query[i].BTof.EID, cells[0].Text);
                            Assert.AreEqual(query[i].BTof.LastName + " " + query[i].BTof.FirstName, cells[1].Text);

                            IWebElement rootElement;
                            if (query[i].Status != (BTStatus.Confirmed | BTStatus.Modified))
                            {
                                rootElement = cells[2].FindElement(By.TagName("a"));
                                Assert.IsTrue(rootElement.GetAttribute("href").Contains("/ACC/EditReportedBT/" + query[i].BusinessTripID + "?selectedDepartment=" + selectedDepartment));
                            }
                            else
                            {
                                rootElement = cells[2].FindElement(By.TagName("span"));
                            }
                            Assert.AreEqual(rootElement.GetAttribute("id"), "EditReportedBTACC");
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
                        else
                        {
                            IWebElement[] cell = rows[i].FindElements(By.TagName("td")).ToArray();
                            Assert.AreEqual(1, rows.Length);
                            Assert.AreEqual("No matching records found", cell[0].Text);
                        }
                    }
                }
                Browser.SelectOption("selectedDepartment", 0);
            }
        }

        [Test]
        public void ACC_EditBTDialogWindow()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                int hrefsCount = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Count();
                Console.WriteLine(hrefsCount);
                for (int i = 31; i < 34; i++)
                { 
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    Console.WriteLine(href.Text);
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                    Assert.AreEqual("ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable", editBtDialogWindow.GetAttribute("class"));
                    IWebElement dialogWindowHeader = editBtDialogWindow.FindElement(By.ClassName("ui-dialog-title"));
                    Assert.AreEqual("ui-dialog-title-Update BT", dialogWindowHeader.GetAttribute("id"));
                    Assert.AreEqual("Edit BT", dialogWindowHeader.Text);

                    BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();

                        String h4 = bt.BTof.LastName + " " + bt.BTof.FirstName + " (" + bt.BTof.EID +
                            ") from " + bt.BTof.Department.DepartmentName;
                        Assert.AreEqual(h4, editBtDialogWindow.FindElement(By.TagName("h4")).Text);

                        if (bt.StartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            Assert.IsNotNull(editBtDialogWindow.FindElement(By.Id("editFutureBTForm")));
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            Assert.IsNotNull(orderTable);
                            Assert.AreEqual("From", orderTable.FindElements(By.Id("col1")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.StartDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("editStartDateFutureBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("editStartDateFutureBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Order From", orderTable.FindElements(By.Id("col2")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderStartDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderStartDateFutureBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderStartDateFutureBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("To", orderTable.FindElements(By.Id("col1")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.EndDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("editEndDateFutureBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("editEndDateFutureBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Order To", orderTable.FindElements(By.Id("col2")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderEndDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderEndDateFutureBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderEndDateFutureBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Location", orderTable.FindElements(By.Id("col1")).Skip(2).FirstOrDefault().Text);
                            string[] text = db.Locations.OrderBy(l => l.Title).Select(l => l.Title).ToArray();
                            string[] values = db.Locations.ToList().OrderBy(l => l.Title).Select(l => l.LocationID.ToString()).ToArray();
                            Assert.AreEqual("True", Browser.IsDropDownList(Browser.FindElement(By.Id("LocationID"), 10), "LocationID", values, text, bt.Location.Title));

                            Assert.AreEqual("Number Of Days", orderTable.FindElements(By.Id("col2")).Skip(2).FirstOrDefault().Text);
                            Assert.AreEqual(bt.DaysInBtForOrder.ToString(), orderTable.FindElement(By.Id("daysInBTForOrderFutureBTACC")).GetAttribute("value"));
                            Assert.AreEqual("Change BT", editBtDialogWindow.FindElement(By.Id("btnSaveFuture")).FindElement(By.ClassName("ui-button-text")).Text);
                            Assert.AreEqual("Cancel BT", editBtDialogWindow.FindElement(By.Id("CancelBTbyACC")).FindElement(By.ClassName("ui-button-text")).Text);
                        }

                        else if (bt.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            Assert.IsNotNull(editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")));
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")).FindElement(By.Id("orderTable"));
                            Assert.IsNotNull(orderTable);
                            Assert.AreEqual("From", orderTable.FindElements(By.Id("col1")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.StartDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("col11")).Text);
                            Assert.AreEqual(null, orderTable.FindElement(By.Id("col11")).GetAttribute("readonly"));

                            Assert.AreEqual("Order From", orderTable.FindElements(By.Id("col2")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderStartDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderStartDateFinishedBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderStartDateFinishedBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("To", orderTable.FindElements(By.Id("col1")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.EndDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("editEndDateACCFinished")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("editEndDateACCFinished")).GetAttribute("readonly"));

                            Assert.AreEqual("Order To", orderTable.FindElements(By.Id("col2")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderEndDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderEndDateFinishedBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderEndDateFinishedBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Location", orderTable.FindElements(By.Id("col1")).Skip(2).FirstOrDefault().Text);
                            Assert.AreEqual(bt.Location.Title, orderTable.FindElements(By.Id("col11")).Skip(2).FirstOrDefault().Text);

                            Assert.AreEqual("Number Of Days", orderTable.FindElements(By.Id("col2")).Skip(2).FirstOrDefault().Text);
                            Assert.AreEqual(bt.DaysInBtForOrder.ToString(), orderTable.FindElement(By.Id("daysInAccComment")).GetAttribute("value"));
                            Assert.AreEqual("Change BT", editBtDialogWindow.FindElement(By.Id("btnChangeFinished")).FindElement(By.ClassName("ui-button-text")).Text);
                        }

                        else
                        {
                            Assert.IsNotNull(editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")));
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")).FindElement(By.Id("orderTable"));
                            Assert.IsNotNull(orderTable);
                            Assert.AreEqual("From", orderTable.FindElements(By.Id("col1")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.StartDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("col11")).Text);
                            Assert.AreEqual(null, orderTable.FindElement(By.Id("col11")).GetAttribute("readonly"));

                            Assert.AreEqual("Order From", orderTable.FindElements(By.Id("col2")).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderStartDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderStartDateCurrentBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderStartDateCurrentBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("To", orderTable.FindElements(By.Id("col1")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.EndDate.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("editEndDateACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("editEndDateACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Order To", orderTable.FindElements(By.Id("col2")).Skip(1).FirstOrDefault().Text);
                            Assert.AreEqual(bt.OrderEndDate.Value.ToString("dd.MM.yyyy"), orderTable.FindElement(By.Id("orderEndDateCurrentBTACC")).GetAttribute("value"));
                            Assert.AreEqual("true", orderTable.FindElement(By.Id("orderEndDateCurrentBTACC")).GetAttribute("readonly"));

                            Assert.AreEqual("Location", orderTable.FindElements(By.Id("col1")).Skip(2).FirstOrDefault().Text);
                            Assert.AreEqual(bt.Location.Title, orderTable.FindElements(By.Id("col11")).Skip(2).FirstOrDefault().Text);

                            Assert.AreEqual("Number Of Days", orderTable.FindElements(By.Id("col2")).Skip(2).FirstOrDefault().Text);
                            Assert.AreEqual(bt.DaysInBtForOrder.ToString(), orderTable.FindElement(By.Id("daysInBTForOrderCurrentBTACC")).GetAttribute("value"));
                            Assert.AreEqual("Change BT", editBtDialogWindow.FindElement(By.Id("btnSaveCurrent")).FindElement(By.ClassName("ui-button-text")).Text);
                        }


                        Assert.AreEqual("Save ACC Comment", editBtDialogWindow.FindElement(By.Id("btnSaveAccComment")).FindElement(By.ClassName("ui-button-text")).Text);
                        if (bt.Comment != null)
                        {
                            Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Comment").FirstOrDefault().Text == (bt.Comment ?? ""));
                            Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Comment").FirstOrDefault().GetAttribute("readonly") == "true");
                        }
                        else
                        {
                            Assert.IsNull(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Comment").FirstOrDefault());
                        }
                        Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Habitation").FirstOrDefault().Text == (bt.Habitation ?? ""));
                        Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Habitation").FirstOrDefault().GetAttribute("readonly") == "true");
                        Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Flights").FirstOrDefault().Text == (bt.Flights ?? ""));
                        Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "Flights").FirstOrDefault().GetAttribute("readonly") == "true");
                        Assert.AreEqual(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "AccComment").FirstOrDefault().Text, bt.AccComment ?? defaultAccComment);
                        Assert.IsTrue(editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "AccComment").FirstOrDefault().GetAttribute("readonly") != "true");

                        Assert.AreEqual(editBtDialogWindow.FindElement(By.TagName("form")).FindElements(By.TagName("table"))[1].Text,
                            ("Purpose " + bt.Purpose).Trim() + Environment.NewLine + ("Manager " + bt.Manager).Trim() + Environment.NewLine + ("Responsible " + bt.Responsible).Trim());
                        Assert.AreEqual(editBtDialogWindow.FindElement(By.TagName("em")).Text,
                            String.Format("Last modified by {0} on {1:dd'.'MM'.'yyyy} at {1:HH':'mm':'ss}",
                            bt.LastCRUDedBy,
                            bt.LastCRUDTimestamp));
                        Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        Thread.Sleep(Timings.Default_ms * 12);
                    }
                }
            }

        }

        [Test]
        public void ACC_EditACCComment()
        {
            int btsCountBefore;
            string btNumber;
            int k = 0;
            using (var db = new AjourBTForTestContext())
            {
                int btsCount = SelectCurrentFutureBTsACC(db, "", "", "").Where(bt => bt.Status != (BTStatus.Confirmed | BTStatus.Modified)).Count();
                for (int i = 0; i < 3; i+=2)
                {
                    btsCountBefore = db.BusinessTrips.Count();

                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i - k).First();
                    btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                        Browser.ClickOnWebElement(href);
                        Thread.Sleep(Timings.Default_ms * 12);

                        IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        IWebElement saveAccCommentButton = editBtDialogWindow.FindElement(By.Id("btnSaveAccComment"));
                        IWebElement AccComment = editBtDialogWindow.FindElements(By.TagName("textarea")).Where(e => e.GetAttribute("id") == "AccComment").FirstOrDefault();
                        Console.WriteLine(btNumber);
                        Console.WriteLine(bt.BTof.LastName);
                        Console.WriteLine(bt.BTof.FirstName);
                        Console.WriteLine(bt.StartDate.ToString("dd-MM-yyyy") + " - " + bt.EndDate.ToString("dd-MM-yyyy"));
                        Browser.SendKeys(AccComment, "abc");
                        Thread.Sleep(Timings.Default_ms * 25);
                        Browser.ClickOnWebElement(saveAccCommentButton);
                        Thread.Sleep(Timings.Default_ms * 25);
                        if (bt.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            k++;
                        }
                        using (var db2 = new AjourBTForTestContext())
                        {
                            BusinessTrip bt2 = db2.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                            int btsCountAfter = db2.BusinessTrips.Count();
                            //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog")); 
                            Assert.IsTrue(bt2.AccComment.EndsWith("abc"));
                            Assert.AreEqual(btsCountBefore, btsCountAfter);
                        }
                    }
                }
            }
        }

        [Test]
        public void ACC_EditBTEndDateWhichIsLesserThanStartDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = 31; i < 34; i++)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime btEndDateBefore = bt.EndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.StartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateFutureBTACC"));
                            IWebElement startDate = orderTable.FindElement(By.Id("editStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(endDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("From Date is greater than To Date", orderTable.FindElement(By.ClassName("field-validation-error")).Text);

                        }
                        else if (bt.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateACCFinished"));
                            IWebElement startDate = orderTable.FindElement(By.Id("col11"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateACCFinished').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnChangeFinished"));

                            Browser.SendKeysTo(endDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("From Date is greater than To Date", orderTable.FindElement(By.ClassName("field-validation-error")).Text);
                        }
                        else
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateACC"));
                            IWebElement startDate = orderTable.FindElement(By.Id("orderStartDateCurrentBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveCurrent"));

                            Browser.SendKeysTo(endDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("From Date is greater than To Date", orderTable.FindElement(By.ClassName("field-validation-error")).Text);
                        }
                        Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        Thread.Sleep(Timings.Default_ms * 12);
                        int btsCountAfter = db.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(db, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(btEndDateBefore, db.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().EndDate);
                    }
                }
            }
        }

        [Test]
        public void ACC_EditBTOrderEndDateWhichIsLesserThanOrderOrderStartDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = 0; i < 3; i++)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");

                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime? btOrderEndDateBefore = bt.OrderEndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFutureBTACC"));
                            IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(OrderEndDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("Order From is greater than Order To", orderTable.FindElement(By.ClassName("field-validation-error")).Text);

                        }
                        else if (bt.OrderEndDate < DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFinishedBTACC"));
                            IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFinishedBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFinishedBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnChangeFinished"));

                            Browser.SendKeysTo(OrderEndDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("Order From is greater than Order To", orderTable.FindElement(By.ClassName("field-validation-error")).Text);
                        }
                        else
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateCurrentBTACC"));
                            IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateCurrentBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateCurrentBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveCurrent"));

                            Browser.SendKeysTo(OrderEndDate, "01.01.2000", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("Order From is greater than Order To", orderTable.FindElement(By.ClassName("field-validation-error")).Text);
                        }
                        Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        Thread.Sleep(Timings.Default_ms * 12);
                        int btsCountAfter = db.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(db, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(btOrderEndDateBefore, db.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().OrderEndDate);
                    }
                }
            }
        }

        [Test]
        public void ACC_EditFutureBTStartDateWhichIsGreaterThanEndDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                        Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime btEndDateBefore = bt.EndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.StartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateFutureBTACC"));
                            IWebElement startDate = orderTable.FindElement(By.Id("editStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(startDate, "01.01.2900", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("From Date is greater than To Date", orderTable.FindElement(By.ClassName("field-validation-error")).Text);

                        }
                        Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        Thread.Sleep(Timings.Default_ms * 12);
                        int btsCountAfter = db.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(db, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(btEndDateBefore, db.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().EndDate);
                    }
                }
            }
        }

        [Test]
        public void ACC_EditFutureBTOrderStartDateWhichIsGreaterThanOrderOrderEndDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime? btOrderEndDateBefore = bt.OrderEndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFutureBTACC"));
                            IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(OrderStartDate, "01.01.2900", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Assert.AreEqual("Order From is greater than Order To", orderTable.FindElement(By.ClassName("field-validation-error")).Text);

                        }
                        Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        Thread.Sleep(Timings.Default_ms * 12);
                        int btsCountAfter = db.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(db, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(btOrderEndDateBefore, db.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().OrderEndDate);
                    }
                }
            }
        }




        [Test]
        public void ACC_EditBTEndDateWhichIsGreaterThanStartDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = 0; i < 3; i++)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");

                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime btEndDateBefore = bt.EndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.StartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateFutureBTACC"));
                            IWebElement startDate = orderTable.FindElement(By.Id("editStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(endDate, "01.01.2900", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Thread.Sleep(Timings.Default_ms * 12);
                            //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                            //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));

                        }
                        else if (bt.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateACCFinished"));
                            IWebElement startDate = orderTable.FindElement(By.Id("col11"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateACCFinished').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnChangeFinished"));

                            Browser.SendKeysTo(endDate, "01.01.2900", true);
                            Thread.Sleep(Timings.Default_ms * 12);

                            Browser.ClickOnWebElement(btnChangeBT);
                            Thread.Sleep(Timings.Default_ms * 12);
                            //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                            //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));
                        }
                        else
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement endDate = orderTable.FindElement(By.Id("editEndDateACC"));
                            IWebElement startDate = orderTable.FindElement(By.Id("orderStartDateCurrentBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveCurrent"));

                            Browser.SendKeysTo(endDate, "01.01.2900", true);
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnChangeBT);
                            Thread.Sleep(Timings.Default_ms * 12);
                            //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                            //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));
                        }
                        //Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        //Thread.Sleep(Timings.Default_ms*30);
                        using (var dbSecond = new AjourBTForTestContext())
                        {
                            int btsCountAfter = dbSecond.BusinessTrips.Count();
                            int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                            //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                            Assert.AreEqual(btsCountBefore, btsCountAfter);
                            Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                            Assert.AreEqual("01.01.2900", dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().EndDate.ToString("dd.MM.yyyy"));
                        }
                    }
                }
            }
        }

        [Test]
        public void ACC_EditBTOrderEndDateWhichIsGreaterThanOrderOrderStartDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = 0; i < 3; i++)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();

                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                        Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");

                    BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                    DateTime? btOrderEndDateBefore = bt.OrderEndDate;
                    int btId = bt.BusinessTripID;
                    if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                    {
                        IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                        IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFutureBTACC"));
                        IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFutureBTACC"));
                        ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFutureBTACC').removeAttribute('readonly')");
                        IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                        Browser.SendKeysTo(OrderEndDate, bt.OrderStartDate.Value.AddDays(1).ToString("dd.MM.yyyy"), true);
                        Thread.Sleep(Timings.Default_ms*12);
                        Browser.ClickOnWebElement(btnChangeBT);
                        Thread.Sleep(Timings.Default_ms*12);
                        //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                        //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));

                    }
                    else if (bt.OrderEndDate < DateTime.Now.ToLocalTimeAzure().Date)
                    {
                        IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFinishedBTForm")).FindElement(By.Id("orderTable"));
                        IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFinishedBTACC"));
                        IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFinishedBTACC"));
                        ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFinishedBTACC').removeAttribute('readonly')");
                        IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnChangeFinished"));

                        Browser.SendKeysTo(OrderEndDate, bt.OrderStartDate.Value.AddDays(1).ToString("dd.MM.yyyy"), true);
                        Thread.Sleep(Timings.Default_ms*12);
                        Browser.ClickOnWebElement(btnChangeBT);
                        Thread.Sleep(Timings.Default_ms*12);
                        //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                        //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));
                    }
                    else
                    {
                        IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editCurrentBTForm")).FindElement(By.Id("orderTable"));
                        IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateCurrentBTACC"));
                        IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateCurrentBTACC"));
                        ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateCurrentBTACC').removeAttribute('readonly')");
                        IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveCurrent"));

                        Browser.SendKeysTo(OrderEndDate, bt.OrderStartDate.Value.AddDays(1).ToString("dd.MM.yyyy"), true);
                        Thread.Sleep(Timings.Default_ms*12);
                        Browser.ClickOnWebElement(btnChangeBT);
                        Thread.Sleep(Timings.Default_ms*12);
                        //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                        //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));
                    }
                    //Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                    //.Sleep(Timings.Default_ms*12);
                    using (var dbSecond = new AjourBTForTestContext())
                    {
                        int btsCountAfter = dbSecond.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(bt.OrderStartDate.Value.AddDays(1).ToString("dd.MM.yyyy"), dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().OrderEndDate.Value.ToString("dd.MM.yyyy"));
                    }
                    }
                }
            }
        }

        [Test]
        public void ACC_EditFutureBTStartDateWhichIsLesserThanEndDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();

                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                        Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");

                    BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                    DateTime btEndDateBefore = bt.EndDate;
                    int btId = bt.BusinessTripID;
                    if (bt.StartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.EndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                    {
                        IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                        IWebElement endDate = orderTable.FindElement(By.Id("editEndDateFutureBTACC"));
                        IWebElement startDate = orderTable.FindElement(By.Id("editStartDateFutureBTACC"));
                        ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDateFutureBTACC').removeAttribute('readonly')");
                        IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                        Browser.SendKeysTo(startDate, bt.EndDate.AddDays(-1).ToString("dd.MM.yyyy"), true);
                        Thread.Sleep(Timings.Default_ms*12);
                        Browser.ClickOnWebElement(btnChangeBT);
                        Thread.Sleep(Timings.Default_ms*12);
                        //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                        //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));

                    }
                    //Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                    //Thread.Sleep(Timings.Default_ms*12);
                    using (var dbSecond = new AjourBTForTestContext())
                    {
                        int btsCountAfter = dbSecond.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(bt.EndDate.AddDays(-1).ToString("dd.MM.yyyy"), dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().StartDate.ToString("dd.MM.yyyy"));
                    }
                    }
                }
            }
        }

        [Test]
        public void ACC_EditFutureBTOrderStartDateWhichIsLesserThanOrderOrderEndDate()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();

                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                        Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
 
                    BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                    DateTime? btOrderEndDateBefore = bt.OrderEndDate;
                    int btId = bt.BusinessTripID;
                    if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                    {
                        IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                        IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFutureBTACC"));
                        IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFutureBTACC"));
                        ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFutureBTACC').removeAttribute('readonly')");
                        IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                        Browser.SendKeysTo(OrderStartDate, bt.OrderEndDate.Value.AddDays(-1).ToString("dd.MM.yyyy"), true);
                        Thread.Sleep(Timings.Default_ms*12);

                        Browser.ClickOnWebElement(btnChangeBT);
                        Thread.Sleep(Timings.Default_ms*12);
                        //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                        //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));

                    }
                    //Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                    //Thread.Sleep(Timings.Default_ms*12);
                    using (var dbSecond = new AjourBTForTestContext())
                    {
                        int btsCountAfter = dbSecond.BusinessTrips.Count();
                        int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                        //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                        Assert.AreEqual(btsCountBefore, btsCountAfter);
                        Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                        Assert.AreEqual(bt.OrderEndDate.Value.AddDays(-1).ToString("dd.MM.yyyy"), dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().OrderStartDate.Value.ToString("dd.MM.yyyy"));
                    }
                    }
                }
            }
        }

        [Test]
        public void ACC_EditFutureBTWithoutModifying()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");
                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        DateTime? btOrderEndDateBefore = bt.OrderEndDate;
                        int btId = bt.BusinessTripID;
                        if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement orderTable = editBtDialogWindow.FindElement(By.Id("editFutureBTForm")).FindElement(By.Id("orderTable"));
                            IWebElement OrderEndDate = orderTable.FindElement(By.Id("orderEndDateFutureBTACC"));
                            IWebElement OrderStartDate = orderTable.FindElement(By.Id("orderStartDateFutureBTACC"));
                            ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFutureBTACC').removeAttribute('readonly')");
                            IWebElement btnChangeBT = editBtDialogWindow.FindElement(By.Id("btnSaveFuture"));

                            Browser.SendKeysTo(OrderStartDate, bt.OrderEndDate.Value.AddDays(-1).ToString("dd.MM.yyyy"), true);
                            Thread.Sleep(Timings.Default_ms * 12);

                            Browser.ClickOnWebElement(btnChangeBT);
                            Thread.Sleep(Timings.Default_ms * 12);
                            //if (Browser.FindElementByClassName("ui-dialog").Text != "")
                            //Assert.IsNull(Browser.FindElementByClassName("ui-dialog"));

                        }
                        //Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                        //Thread.Sleep(Timings.Default_ms*12);
                        using (var dbSecond = new AjourBTForTestContext())
                        {
                            int btsCountAfter = dbSecond.BusinessTrips.Count();
                            int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                            //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                            Assert.AreEqual(btsCountBefore, btsCountAfter);
                            Assert.AreEqual(btsOnPageBefore, btsOnPageAfter);
                            Assert.AreEqual(bt.OrderEndDate.Value.AddDays(-1).ToString("dd.MM.yyyy"), dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().OrderStartDate.Value.ToString("dd.MM.yyyy"));
                            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bt.Status);
                        }
                    }
                }
            }
        }

        [Test]
        public void ACC_CancelFutureBT()
        {
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> query = SelectCurrentFutureBTsACC(db, "", "", "");
                for (int i = query.Count() - 1; i < query.Count() - 4; i--)
                {
                    int btsCountBefore = db.BusinessTrips.Count();
                    int btsOnPageBefore = query.Count();
                    IWebElement href = Browser.FindElement(By.XPath(tBodyBTACC), 10).FindElements(By.TagName("a")).Where(w => w.GetAttribute("id") == "EditReportedBTACC").Skip(i).First();
                    string btNumber = href.GetAttribute("href").Split('?')[0].Split('/').LastOrDefault();
                    if (btNumber != "97")
                    {
                    Browser.ClickOnWebElement(href);
                    Thread.Sleep(Timings.Default_ms*12);

                    IWebElement editBtDialogWindow = Browser.FindElementByClassName("ui-dialog");

                        BusinessTrip bt = db.BusinessTrips.ToList().Where(b => b.BusinessTripID == Int32.Parse(btNumber)).FirstOrDefault();
                        int btId = bt.BusinessTripID;
                        if (bt.OrderStartDate > DateTime.Now.ToLocalTimeAzure().Date && bt.OrderEndDate >= DateTime.Now.ToLocalTimeAzure().Date)
                        {
                            IWebElement btnCancelBT = editBtDialogWindow.FindElement(By.Id("CancelBTbyACC"));
                            Browser.ClickOnWebElement(btnCancelBT);
                            Thread.Sleep(Timings.Default_ms * 12);

                            IWebElement cancelComment = Browser.FindElement(By.Id("CancelComment"), 10);
                            IWebElement btnCancelBTConfirm = Browser.FindElement(By.Id("cancelReportedBTAcc"), 10);
                            Browser.SendKeys(cancelComment, "abc");
                            Thread.Sleep(Timings.Default_ms * 12);
                            Browser.ClickOnWebElement(btnCancelBTConfirm);
                            Thread.Sleep(Timings.Default_ms * 30);
                            Assert.Throws<NoSuchElementException>(delegate { Browser.FindElementByClassName("ui-dialog"); });

                            using (var dbSecond = new AjourBTForTestContext())
                            {
                                int btsCountAfter = dbSecond.BusinessTrips.Count();
                                int btsOnPageAfter = SelectCurrentFutureBTsACC(dbSecond, "", "", "").Count();
                                //Assert.AreEqual(null, Browser.FindElementByClassName("ui-dialog"));
                                Assert.AreEqual(btsCountBefore, btsCountAfter);
                                Assert.AreEqual(btsOnPageBefore, btsOnPageAfter + 1);
                                Assert.IsTrue(dbSecond.BusinessTrips.Where(b => b.BusinessTripID == btId).FirstOrDefault().Status.HasFlag(BTStatus.Cancelled));
                            }
                        }
                        else
                        {
                            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-icon-closethick"));
                            Thread.Sleep(Timings.Default_ms * 12);
                        }
                    }
                }
            }
        }
    }
}
