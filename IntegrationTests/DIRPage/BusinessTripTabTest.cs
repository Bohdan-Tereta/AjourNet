using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
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

namespace IntegrationTests.DIRPage
{
    [TestFixture]
    class BusinessTripTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchErrorText = "No matching records found";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "carn";
            password = "gredsa";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("DIR");

            Browser.ClickOnLink("Business Trips");

            string BTs = Browser.FindElementByLinkText("Business Trips").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", BTs);
        }

        public List<BusinessTrip> GetBTsForDIR(AjourBTForTestContext context, string selectedDepartment = null)
        {
            List<BusinessTrip> bTrips = new List<BusinessTrip>();
            DateTime StartDateToCompare = DateTime.Now.ToLocalTimeAzure().Date;

            bTrips = (from item in context.BusinessTrips
                      join e in context.Employees on item.EmployeeID equals e.EmployeeID
                                             where
                                                 ((item.Status == BTStatus.Confirmed || (item.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                                                 && (item.StartDate > StartDateToCompare))
                                                 && e.DateDismissed == null
                                             select item).ToList();

            return bTrips;
        }


        #region BusinessTrip Tab

        [Test]
        public void GetBusinessTripsForDir()
        {
            Thread.Sleep(Timings.Default_ms*20);
            ReadOnlyCollection<IWebElement> dirTable = Browser.FindElementsByXPath("//table[@id='indexDIRexample']/tbody/tr");
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> btList = GetBTsForDIR(db);                

                foreach (IWebElement element in dirTable)
                {
                    ReadOnlyCollection<IWebElement> dirTableTD = element.FindElements(By.CssSelector("td"));
                    foreach (var bt in btList)
                    {
                        if (dirTableTD[0].Text.Contains(bt.BTof.EID))
                        {
                            Assert.IsTrue(dirTableTD[1].Text.Contains(bt.BTof.FirstName));
                            Assert.IsTrue(dirTableTD[1].Text.Contains(bt.BTof.LastName));
                            Assert.IsTrue(dirTableTD[2].Text.Contains(bt.StartDate.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(dirTableTD[2].Text.Contains(bt.EndDate.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(dirTableTD[3].Text.Contains(bt.Location.Title));
                            Assert.IsTrue(dirTableTD[4].Text.Contains(bt.Purpose));
                            Assert.IsTrue(dirTableTD[5].Text.Contains(bt.Manager));
                        }
                    }
                }
            }
        }

        [Test]
        public void RejectBT()
        {
            int beforeReject = 0;
            int afterReject = 0;

            string eidToCheckReject;

            ReadOnlyCollection<IWebElement> dirTable = Browser.FindElementsByXPath("//table[@id='indexDIRexample']/tbody/tr");
            ReadOnlyCollection<IWebElement> dirTableTD = Browser.FindElementsByXPath("//table[@id='indexDIRexample']/tbody/tr/td");
            eidToCheckReject = dirTableTD[0].Text;
            beforeReject = dirTable.Count;
            IWebElement rejectButton = Browser.FindElementByXPath("//a[@id='RejectBTbyDIR']");
            Browser.ClickOnWebElement(rejectButton);

            Thread.Sleep(Timings.Default_ms*15);

            IWebElement textArea = Browser.FindElementByXPath("//textarea[@id='RejectComment']");
            IWebElement rejectConfirmBtn = Browser.FindElementByXPath("//button[@id='RejectBtnDIR']");
            Browser.SendKeysTo(textArea, "Test Comment", true);
            Browser.ClickOnWebElement(rejectConfirmBtn);

            Thread.Sleep(Timings.Default_ms*15);

            dirTable = Browser.FindElementsByXPath("//table[@id='indexDIRexample']/tbody/tr");
            IWebElement dirAsElement = Browser.FindElementByXPath("//table[@id='indexDIRexample']");
            afterReject = dirTable.Count;
            
            Assert.Less(afterReject, beforeReject);
            Assert.IsTrue(!dirAsElement.Text.Contains(eidToCheckReject));
        }

        [Test]
        public void Search_BadText()
        {
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchBusinessTripsDIR']");
            Browser.SendKeysTo(searchInput, "Bad Text", true);
            Browser.SendEnter(searchInput);

            Thread.Sleep(Timings.Default_ms*15);

            IWebElement elementWithErrorText = Browser.FindElementByXPath("//td[@class='dataTables_empty']");

            Assert.IsTrue(elementWithErrorText.Text.Contains(searchErrorText));

            Browser.SendKeysTo(searchInput,"", true);
            Browser.SendEnter(searchInput);
        }

        [Test]
        public void Search_rlan()
        {
            string eidForSearch;
            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> recordsForDir = GetBTsForDIR(db);
                eidForSearch = recordsForDir[0].BTof.EID;
            }

            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchBusinessTripsDIR']");
            Browser.SendKeysTo(searchInput, eidForSearch, true);
            Browser.SendEnter(searchInput);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement searchResult = Browser.FindElementByXPath("//table[@id='indexDIRexample']/tbody");

            Assert.IsTrue(searchResult.Text.Contains(eidForSearch));

            Browser.SendKeysTo(searchInput, "", true);
            Browser.SendEnter(searchInput);
        }

        [Test]
        public void DropDownListTest()
        {
            Dictionary<int, string> dropDownListDict = new Dictionary<int, string>(); //<option number in DropDownList, EID>
            dropDownListDict.Add(2,"wens");
            dropDownListDict.Add(5,"rkni");
            dropDownListDict.Add(7,"rlan");

            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> recordsForDir = GetBTsForDIR(db);
                foreach(BusinessTrip bt in recordsForDir)
                {
                    if (bt.BTof.DepartmentID.HasValue)
                    {
                        int depId = (int)bt.BTof.DepartmentID;
                        int selectedOption = 0;

                        switch (depId)
                {
                            case 8:
                                selectedOption = 1;
                                break;
                            case 9:
                                selectedOption = 9;
                                break;
                            default:
                                selectedOption = depId + 1;
                                break;
                        }

                        Browser.SelectOption("selectedDepartment", selectedOption);
                        Thread.Sleep(Timings.Default_ms*20);

                        IWebElement table = Browser.FindElementByXPath("//table[@id='indexDIRexample']/tbody");
                        Assert.IsTrue(table.Text.Contains(bt.BTof.EID));
                        Assert.IsTrue(table.Text.Contains(bt.BTof.FirstName));
                        Assert.IsTrue(table.Text.Contains(bt.BTof.LastName));
                    }
                }
            }

            Browser.SelectOption("selectedDepartment", 0);
        }

        [Test]
        public void SaveSortAfterUpdate()
        {
            List<string> eid = new List<string>();
            List<string> eidAfterUpdate = new List<string>();

            IWebElement columnToSort = Browser.FindElementByXPath("//th[@class='sorting']");
            Browser.ClickOnWebElement(columnToSort);
            ReadOnlyCollection<IWebElement> dirTable = Browser.FindElementsByXPath("//tbody/tr");

            foreach (IWebElement item in dirTable)
            {
                string[] td = item.Text.Split(' ');
                eid.Add(td[0]);
            }

            IWebElement rejectBtn = Browser.FindElementByXPath("//a[@id='RejectBTbyDIR']");
            Browser.ClickOnWebElement(rejectBtn);
            Thread.Sleep(Timings.Default_ms*10);

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
            Thread.Sleep(Timings.Default_ms*10);

            dirTable = Browser.FindElementsByXPath("//tbody/tr");
            foreach (IWebElement item in dirTable)
            {
                string[] updatedTd = item.Text.Split(' ');
                eidAfterUpdate.Add(updatedTd[0]);
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
