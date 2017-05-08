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
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace IntegrationTests.VUPage
{
     [TestFixture]
     public class BTsInProcessTest
    {
        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private AjourBTForTestContext db = new AjourBTForTestContext();
        private string nothingFound = "No matching records found";


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "ayou";
            password = "123456";
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Thread.Sleep(Timings.Default_ms*20);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("VU");
            Browser.ClickOnLink("BTs in process");
            verificationErrors = new StringBuilder();
            Thread.Sleep(Timings.Default_ms*20);
            Thread.Sleep(Timings.Default_ms*20);
            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }


        [Test]
        public void ShowBTInformationWindowIsOpened_CorrectData()
        {
            //Arrange
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("BTs in process");
            //Act
            Browser.ClickByXPath("//table[@id='prepBTDataVU']/tbody/tr[6]/td[3]/a");
            Thread.Sleep(Timings.Default_ms * 30);

            //Assert
            Assert.AreEqual("Melissa Calak (carn)", Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[1]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("EC/BV", Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[2]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(1).Date.ToString("dd.MM.yyyy"), Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[3]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(32).Date.ToString("dd.MM.yyyy"), Browser.GetText("//*[@id='Show BT Data']/fieldset/table/tbody/tr[4]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20);
        }

  
        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("BTs in process");
            Browser.Wait(5);

            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchPrepBTsDataVU']");
            Browser.SendKeysTo(searchInput, serchString, true);
            Browser.SendEnter(Browser.FindElementByXPath("//input[@id='searchPrepBTsDataVU']"));
            Thread.Sleep(Timings.Default_ms * 20);

            if (!isFound)
            {
                Browser.ClickByXPath("//*[@id='prepBTDataVU']/tbody/tr/td");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='prepBTDataVU']/tbody/tr/td"), nothingFound);
            }
            else
            {
                Browser.ClickByXPath("//*[@id='prepBTDataVU']/tbody/tr[1]/td[1]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='prepBTDataVU']/tbody/tr[1]/td[1]"), "rkni");
            }

            Browser.SendKeysTo(searchInput, "", true);
            Browser.SendEnter(searchInput);
        }

         
         [Test]
        public void TableHeaders_CorrectData()
        {
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("BTs in process");

            //Assert
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("EID", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[1]"));
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("Name", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[2]"));
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("Registered", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[3]"));
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("Confirmed", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[4]"));
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("Reported", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[5]"));
            Thread.Sleep(Timings.Default_ms * 20);
            Assert.AreEqual("Current", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/thead/tr/th[6]"));
            Thread.Sleep(Timings.Default_ms * 20);
        }


         [Test]
         public void TableRows_CorrectData()
         {
             //Act
             Browser.Refresh();
             Browser.ClickOnLink("BTs in process");

             //Assert
             Thread.Sleep(Timings.Default_ms * 20);
             Assert.AreEqual("ppez", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/tbody/tr[2]/td[1]"));
             Thread.Sleep(Timings.Default_ms * 20);
             Assert.AreEqual("Austin Jonathan", Browser.GetText("//table[@class='prepBusinessTripDataVU dataTable']/tbody/tr[2]/td[2]"));
             Thread.Sleep(Timings.Default_ms * 20);
         }

         [Test]
         public void EmployeeInTableDataCount()
         {
             //Act
             Browser.Refresh();
             Browser.ClickOnLink("BTs in process");

             //Assert
             Thread.Sleep(Timings.Default_ms*20);
             List<Employee> list = (from e in db.Employees.AsEnumerable()
                                    where e.IsUserOnly == false && e.BusinessTrips.Count > 0
                                    orderby e.LastName
                                    select e).ToList();
              List<Employee> listWithBts = new List<Employee>();
                 foreach(Employee e in list.AsEnumerable() )
             {
                 if (e.BusinessTrips.Where(b => (b.Status != BTStatus.Planned)
                                            && b.Status != (BTStatus.Planned | BTStatus.Modified)
                                            && b.Status != (BTStatus.Planned | BTStatus.Cancelled)).FirstOrDefault()!=null)
                     listWithBts.Add(e);
             }

                 Employee emp = listWithBts.FirstOrDefault();
             ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='prepBTDataVU']/tbody/tr");
             if (emp != null)
             {
                 foreach (IWebElement e in empTable.AsEnumerable())
                 {
                     if (e.Text.Contains(emp.EID))
                         Assert.IsTrue(e.Text.Contains(emp.LastName));
                 }
             }
             Assert.AreEqual(listWithBts.Count, empTable.Count);
         }
       
    }
}
