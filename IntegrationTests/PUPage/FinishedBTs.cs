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

namespace IntegrationTests.PUPage
{
    [TestFixture]
    public class FinishedBTs
    {

        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private string nothingFound = "No matching records found";
        private AjourBTForTestContext db = new AjourBTForTestContext();
        private int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;

        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "apat";
            password = "lokmop";
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='PULink']/a");
            Thread.Sleep(Timings.Default_ms*20);
            verificationErrors = new StringBuilder();

            //Assert.That(Browser.IsAt("PUView"));

            //Browser.ClickOnLink("Finished BTs");
            //Thread.Sleep(Timings.Default_ms*20);
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
        [Test]
        public void FinishedBTsTest()
        {
            //Arrange
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);


            //Act
            //Assert
            Assert.NotNull(Browser.HasElement("PU"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[7]/a"), "Finished BTs");
        }


        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='replace']/input"), serchString, true);
            Browser.SendEnter(Browser.FindElementByXPath("//*[@id='replace']/input"));
            Browser.Wait(5);
            Thread.Sleep(Timings.Default_ms*20);

            if (!isFound)
            {
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU_wrapper']/div[2]/div[2]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='exampleBtsViewByDatesPU_wrapper']/div[2]/div[2]"), nothingFound);
            }
            else
            {
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a"), "rkni");
            }
        }

        [Test]
        [TestCase("//*[@id='puByDatesSecondHeader']/th[1]", "EID", true)]
        [TestCase("//*[@id='puByDatesSecondHeader']/th[2]", "Name", true)]
        [TestCase("//*[@id='puByDatesSecondHeader']/th[1]", "EID", false)]
        [TestCase("//*[@id='puByDatesSecondHeader']/th[2]", "Name", false)]
        public void BTs_SortingTable(string path, string column, bool ascending)
        {
            //Arrange
            //Act
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            Browser.ClickByXPath(path);
            if (!ascending)
                Browser.ClickByXPath(path);
            var firstEmploeeID = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");

            var firstEmploeeName = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[2]");
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
                    Assert.AreEqual(firstEmploeeName, "Austin Jonathan");
                else
                    Assert.IsFalse(firstEmploeeName.StartsWith(name.LastName));
        }

        [Test]
        public void YearDropDownList_defaultYear()
        {
            //Arrange
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            string expected = Browser.GetText("//*[@id='selectedYear']/option[1]");
            Thread.Sleep(Timings.Default_ms * 10);
            string data = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[4]");
            Thread.Sleep(Timings.Default_ms * 10);

            //Act
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.AreEqual("2015", expected);
            Assert.IsTrue(data.Contains(selectedYear.ToString()));
        }

        [Test]
        public void YearDropDownList_selectFutureYear()
        {
            //Act
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectOption("selectedYear", (selectedYear + 1).ToString());
            Thread.Sleep(Timings.Default_ms * 10);
            string expected = Browser.GetText("//*[@id='selectedYear']/option[2]");
            Thread.Sleep(Timings.Default_ms * 10);
            string data = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]");

            //Assert
            Assert.IsTrue(data.Contains("No matching records found"));
        }

        [Test]
        public void YearDropDownList_selectPreviousYear()
        {
            //Act
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectOption("selectedYear", (selectedYear - 1).ToString());
            Thread.Sleep(Timings.Default_ms * 10);
            string expected = Browser.GetText("//*[@id='selectedYear']/option[2]");
            Thread.Sleep(Timings.Default_ms * 10); 
            string data = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[4]");

            //Assert
            Assert.IsTrue(data.Contains((selectedYear - 1).ToString()));
        }

        [Test]
        public void LocationDropDownList_defaultLoc()
        {
            //Arrange
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            string expected = Browser.GetText("//*[@id='puByDatesFirstHeader']/td[3]/span/select/option[1]");
            string data = Browser.GetText("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[3]");
            IEnumerable<string> locations = (from l in db.Locations.AsEnumerable()
                                             select l.Title).ToList();
            //Act
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.AreEqual("All", expected);
            Assert.Contains(data, locations.ToList());
        }

        [Test]
        public void TableData()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);

            int year = DateTime.Now.ToLocalTimeAzure().Year;

            List<Employee> employeeBts = (from bt in db.BusinessTrips.AsEnumerable()
                                          where (bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date && bt.StartDate.Year == selectedYear)
                                          select bt.BTof).ToList();

            Employee emp = employeeBts.FirstOrDefault();
            if (emp != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
                Thread.Sleep(Timings.Default_ms*20);

                foreach (var element in empTable)
                {
                    if (element.Text.Contains(emp.FirstName))
                    {
                        Assert.IsTrue(element.Text.Contains(emp.FirstName));
                        Assert.IsTrue(element.Text.Contains(emp.LastName));
                    }
                }
                Assert.AreEqual(employeeBts.Count, empTable.Count());
            }
        }

        [Test]
        public void EditFinished_CheckLabels()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().AddYears(-1).Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);
            int year = DateTime.Now.ToLocalTimeAzure().AddYears(-1).Year;

            BusinessTrip employeeBt = (from bt in db.BusinessTrips.AsEnumerable()
                                       where (bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date.AddYears(-1) && bt.StartDate.Year == selectedYear&&bt.BTof.EID=="ppez")
                                       select bt).OrderBy(bt => bt.BTof.LastName).ThenBy(bt => bt.BTof.FirstName).FirstOrDefault();
   

            if (employeeBt != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
                Thread.Sleep(Timings.Default_ms*20);
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual("Edit BT", Browser.GetText("//*[@id='ui-dialog-title-Update BT']"));
                Assert.IsTrue(Browser.GetText("//*[@id='Update BT']/h4").Contains(employeeBt.BTof.FirstName));
                Assert.IsTrue(Browser.GetText("//*[@id='Update BT']/h4").Contains(employeeBt.BTof.LastName));
                Assert.IsTrue(Browser.GetText("//*[@id='Update BT']/h4").Contains(employeeBt.BTof.EID));
                Assert.IsTrue(Browser.GetText("//*[@id='Update BT']/h4").Contains(employeeBt.BTof.Department.DepartmentName));
                Assert.AreEqual("From", Browser.GetText("//*[@id='col1'][1]"));
                Assert.AreEqual("Order From", Browser.GetText("//*[@id='col2'][1]"));
                Assert.AreEqual("To", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[1]/tbody/tr[2]/td[1]/a"));
                Assert.AreEqual("Order To", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[1]/tbody/tr[2]/td[3]"));
                Assert.AreEqual("Location", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[1]/tbody/tr[4]/td[1]/a"));
                Assert.AreEqual("Number Of Days", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[1]/tbody/tr[4]/td[3]"));
                Assert.AreEqual("Unit", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[1]/tbody/tr[5]/td[1]/a"));
                Assert.AreEqual("Purpose", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[2]/tbody/tr[1]/td[1]/a"));
                Assert.AreEqual("Manager", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[2]/tbody/tr[2]/td[1]/a"));
                Assert.AreEqual("Responsible", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[2]/tbody/tr[3]/td[1]/a"));
                Assert.AreEqual("Comment", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[3]/tbody/tr[1]/td/b"));
                Assert.AreEqual("Invitation", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[3]/tbody/tr[2]/td/label"));
                Assert.AreEqual("Habitation", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[3]/tbody/tr[3]/td/b"));
                Assert.AreEqual("Flights", Browser.GetText("/html/body/div[4]/div[2]/form/fieldset/table[3]/tbody/tr[4]/td/b"));
                Assert.IsTrue(Browser.GetText("//*[@id='Update BT']/p/em").Contains("Last modified by "));
                Browser.ClickByXPath("/html/body/div[4]/div[1]/a/span");
                Thread.Sleep(Timings.Default_ms*10);
            }
        }

        [Test]
        public void EditFinished_EditDates()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);
          
            int year = DateTime.Now.ToLocalTimeAzure().Year;

            BusinessTrip employeeBt = (from bt in db.BusinessTrips.AsEnumerable()
                                       where (bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date && bt.StartDate.Year == selectedYear)
                                       select bt).OrderBy(bt => bt.BTof.LastName).ThenBy(bt => bt.BTof.FirstName).FirstOrDefault();
   

            if (employeeBt != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
                Thread.Sleep(Timings.Default_ms*20);
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
                Thread.Sleep(Timings.Default_ms*20);
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDateFinishedBTPU').removeAttribute('readonly')");
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateFinishedBTPU').removeAttribute('readonly')");
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFinishedBTPU').removeAttribute('readonly')");
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFinishedBTPU').removeAttribute('readonly')");

                IWebElement from = Browser.FindElementByXPath("//*[@id='editStartDateFinishedBTPU']");
                IWebElement to = Browser.FindElementByXPath("//*[@id='editEndDateFinishedBTPU']");
                IWebElement orderFrom = Browser.FindElementByXPath("//*[@id='orderStartDateFinishedBTPU']");
                IWebElement orderTo = Browser.FindElementByXPath("//*[@id='orderEndDateFinishedBTPU']");

                string startDate = "19.06.2014";
                string endDate = "21.06.2014";
                string orderStartDate = "15.06.2014";
                string orderEndDate = "01.07.2014";



                Browser.SendKeysTo(from, startDate, true);
                Browser.SendKeysTo(to, endDate, true);

                Browser.SendKeysTo(orderFrom, orderStartDate, true);
                Browser.SendKeysTo(orderTo, orderEndDate, true);

                string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
                Browser.ClickByXPath(SaveBTButtonXPath);

                Thread.Sleep(Timings.Default_ms*20);
                IWebElement modifiedRow = Browser.FindElementByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]");
                Console.WriteLine(modifiedRow.Text);
                Assert.IsTrue(modifiedRow.Text.Contains(startDate));
                Assert.IsTrue(modifiedRow.Text.Contains(endDate));
                Assert.IsTrue(modifiedRow.Text.Contains(orderStartDate));
                Assert.IsTrue(modifiedRow.Text.Contains(orderEndDate));
            }
        }

        [Test]
        public void EditFinished_EditDropDowns()
        {
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms * 20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);
            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            Browser.SelectOption("LocationID", "AT/BW");
            Thread.Sleep(Timings.Default_ms * 10); 
            Browser.SelectOption("UnitID", "B");
            Thread.Sleep(Timings.Default_ms * 10);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement modifiedRow = Browser.FindElementByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]");
            Console.WriteLine(modifiedRow.Text);
            Assert.IsTrue(modifiedRow.Text.Contains("AT/BW"));
            Assert.IsTrue(modifiedRow.Text.Contains("B"));
        }

        [Test]
        public void EditFinished_EditPurpose()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement purpose = Browser.FindElementByXPath("//*[@id='Purpose']");

            string purposeValue = "new purpose";
            Browser.SendKeysTo(purpose, purposeValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(purposeValue, bt.Purpose);
        }

        [Test]
        public void EditFinished_EditManager()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement manager = Browser.FindElementByXPath("//*[@id='Manager']");

            string managerValue = "new manager";
            Browser.SendKeysTo(manager, managerValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(managerValue, bt.Manager);
        }

        [Test]
        public void EditFinished_EditResponsible()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement responsible = Browser.FindElementByXPath("//*[@id='Responsible']");

            string responsibleValue = "new responsible";
            Browser.SendKeysTo(responsible, responsibleValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(responsibleValue, bt.Responsible);
        }

        [Test]
        public void EditFinished_EditComment()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement comment = Browser.FindElementByXPath("//*[@id='Comment']");

            string commentValue = "new comment";
            Browser.SendKeysTo(comment, commentValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(commentValue, bt.Comment);
        }

        [Test]
        public void EditFinished_EditHabitation()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement habitation = Browser.FindElementByXPath("//*[@id='Habitation']");

            string habitationValue = "new habitation";
            Browser.SendKeysTo(habitation, habitationValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(habitationValue, bt.Habitation);
        }

        [Test]
        public void EditFinished_EditFlights()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement flights = Browser.FindElementByXPath("//*[@id='Flights']");

            string flightsValue = "new flights";
            Browser.SendKeysTo(flights, flightsValue, true);

            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(flightsValue, bt.Flights);
        }

        [Test]
        public void EditFinished_EditInvitation()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            Browser.ClickByXPath("//*[@id='Invitation']"); 


            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(true, bt.Invitation);
        }

        [Test]
        public void EditFinished_EditHabitationConfirmed()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            Browser.ClickByXPath("//*[@id='HabitationConfirmed']");


            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(true, bt.HabitationConfirmed);
        }

        [Test]
        public void EditFinished_EditFlightsConfirmed()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
            Thread.Sleep(Timings.Default_ms*20);

            Browser.ClickByXPath("//*[@id='FlightsConfirmed']");


            string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
            Browser.ClickByXPath(SaveBTButtonXPath);

            Thread.Sleep(Timings.Default_ms*20);
            BusinessTrip bt;
            using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
            {
                bt = dbContext.BusinessTrips.Where(b => b.BusinessTripID == 94).FirstOrDefault();
            }
            Assert.IsNotNull(bt);
            Assert.AreEqual(true, bt.FlightsConfirmed);
        }

        [Test]
        public void EditFinished_EditDatesStartGreaterThanEnd()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            int year = DateTime.Now.ToLocalTimeAzure().Year;

            BusinessTrip employeeBt = (from bt in db.BusinessTrips.AsEnumerable()
                                       where (bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date && bt.StartDate.Year == selectedYear)
                                       select bt).OrderBy(bt => bt.BTof.LastName).ThenBy(bt => bt.BTof.FirstName).FirstOrDefault();

            if (employeeBt != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
                Thread.Sleep(Timings.Default_ms*20);
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
                Thread.Sleep(Timings.Default_ms*20);
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editStartDateFinishedBTPU').removeAttribute('readonly')");
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('editEndDateFinishedBTPU').removeAttribute('readonly')");
     
                IWebElement from = Browser.FindElementByXPath("//*[@id='editStartDateFinishedBTPU']");
                IWebElement to = Browser.FindElementByXPath("//*[@id='editEndDateFinishedBTPU']");

                string startDate = "21.06.2014";
                string endDate = "19.06.2014";

                Browser.SendKeysTo(from, startDate, true);
                Browser.SendKeysTo(to, endDate, true);

                string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
                Browser.ClickByXPath(SaveBTButtonXPath);

                Thread.Sleep(Timings.Default_ms*20);
                IWebElement error = Browser.FindElementByXPath("//*[@id='orderTable']/tbody/tr[3]/td[1]/span/span");
                Assert.AreEqual("From Date is greater than To Date", error.Text);
                Browser.ClickByXPath("/html/body/div[4]/div[1]/a/span");
                Thread.Sleep(Timings.Default_ms*20);
            }
        }

        [Test]
        public void EditFinished_EditOrderDatesStartGreaterThanEnd()
        {
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Finished BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedYear']");
            Browser.ClickByXPath("//*[@id='selectedYear']/option[4]");
            Thread.Sleep(Timings.Default_ms * 20);

            int year = DateTime.Now.ToLocalTimeAzure().Year;

            BusinessTrip employeeBt = (from bt in db.BusinessTrips.AsEnumerable()
                                       where (bt.EndDate.Date < DateTime.Now.ToLocalTimeAzure().Date && bt.StartDate.Year == selectedYear)
                                       select bt).OrderBy(bt => bt.BTof.LastName).ThenBy(bt => bt.BTof.FirstName).FirstOrDefault();

            if (employeeBt != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr");
                Thread.Sleep(Timings.Default_ms*20);
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDatesPU']/tbody/tr[1]/td[1]/a");
                Thread.Sleep(Timings.Default_ms*20);
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderStartDateFinishedBTPU').removeAttribute('readonly')");
                ((IJavaScriptExecutor)Browser.webDriver).ExecuteScript("document.getElementById('orderEndDateFinishedBTPU').removeAttribute('readonly')");

                IWebElement orderFrom = Browser.FindElementByXPath("//*[@id='orderStartDateFinishedBTPU']");
                IWebElement orderTo = Browser.FindElementByXPath("//*[@id='orderEndDateFinishedBTPU']");

                string startDate = "21.06.2014";
                string endDate = "19.06.2014";

                Browser.SendKeysTo(orderFrom, startDate, true);
                Browser.SendKeysTo(orderTo, endDate, true);

                string SaveBTButtonXPath = "/html/body/div[4]/div[2]/form/div/button";
                Browser.ClickByXPath(SaveBTButtonXPath);

                Thread.Sleep(Timings.Default_ms*20);
                IWebElement error = Browser.FindElementByXPath("//*[@id='orderTable']/tbody/tr[3]/td[2]/span/span");
                Assert.AreEqual("Order From is greater than Order To", error.Text);
                Browser.ClickByXPath("/html/body/div[4]/div[1]/a/span");
                Thread.Sleep(Timings.Default_ms*20);
            }
        }

    }
}
