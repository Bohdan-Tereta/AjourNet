using AjourBT.Domain.Entities;
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

namespace IntegrationTests.VUPage
{
    [TestFixture]
    class DaysFromBTsTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchResultFail = "No matching records found";

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

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            Browser.ClickOnLink("Days From BT");

            string DaysFromBT = Browser.FindElementByLinkText("Days From BT").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", DaysFromBT);
        }

        //[Test]
        //public void CheckJourneyLinkPopup_ColemanJohn()
        //{
        //    IReadOnlyCollection<IWebElement> tbodyJourneys = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");

        //    IWebElement firstElem = tbodyJourneys.FirstOrDefault();
        //    IWebElement journey = firstElem.FindElement(By.XPath("//a[@id='journeyLink']/redtext"));

        //    Assert.IsTrue(firstElem.Text.Contains("Coleman John"));
        //    Assert.IsTrue(firstElem.Text.Contains("03.03.2013"));
        //    Assert.AreEqual("31.10.2013" ,journey.GetAttribute("title"));
        //}

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        [Test]
        public void CheckOvertimeLink()
        {
            IReadOnlyCollection<IWebElement> tbodyJourneys = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");

            IWebElement secondElem = tbodyJourneys.ElementAt(1);
            IWebElement overtime = secondElem.FindElement(By.XPath("//a[@id='overtimeLink']/redtext"));

            Assert.IsTrue(secondElem.Text.Contains("George Anatoliy"));
            Assert.IsTrue(secondElem.Text.Contains("03.01.2013"));
            Assert.AreEqual("10.03.2013", overtime.GetAttribute("title"));
        }

        [Test]
        public void Search_BadString()
        {
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchInput']");
            Browser.SendKeysTo(searchInput, "Bad Text", true);
            Browser.SendKeys(searchInput, Keys.Enter);
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement emptyTable = Browser.FindElementByXPath("//td[@class='dataTables_empty']");
            Assert.AreEqual(searchResultFail, emptyTable.Text);
        }

        [Test]
        public void Search_co_NotEmpty()
        {
            IWebElement searchInput = Browser.FindElementByXPath("//input[@id='searchInput']");
            Browser.SendKeysTo(searchInput, "co", true);
            Browser.SendKeys(searchInput, Keys.Enter);
            Thread.Sleep(Timings.Default_ms*20);

            ReadOnlyCollection<IWebElement> searchResult = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
            Assert.AreEqual(4, searchResult.Count);

            Browser.SendKeysTo(searchInput, Keys.Enter, true);
        }

        [Test]
        public void CheckJourneys()
        {
            Thread.Sleep(Timings.Default_ms*20);

           ReadOnlyCollection<IWebElement> daysFromBtTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
           Thread.Sleep(Timings.Default_ms*30);

            using (var db = new AjourBTForTestContext())
            {
                List<BusinessTrip> btList = (from bt in db.BusinessTrips where bt.Journeys.Count > 0 select bt).ToList();

                foreach (BusinessTrip item in btList)
                {
                  
                        Employee employee = (from emp in db.Employees.AsEnumerable() where emp.EmployeeID == item.EmployeeID select emp).FirstOrDefault();
                     List<Journey> journeyList =  (from bts in db.BusinessTrips where
                                                      bts.EmployeeID == employee.EmployeeID && bts.Journeys.Count > 0
                                                  from journ in bts.Journeys
                                                  where journ.DayOff == true
                                                  select journ).ToList();
                     if (journeyList.Count != 0)
                     {
                        
                            foreach (Journey j in journeyList)
                             {
                                 Console.WriteLine(j.Date.ToString(String.Format("dd.MM.yyyy")));
                             }
                         
                     }
                        
                    
                }
            }
        }

        [Test]
        public void CheckOvertimes()
        {
            ReadOnlyCollection<IWebElement> daysFromBtTable = Browser.FindElementsByXPath("//tbody[@id='tbodyJourneys']/tr");
            Thread.Sleep(Timings.Default_ms*30);

            using (var db = new AjourBTForTestContext())
            {
                List<Overtime> overtimeList = (from selectedOver in db.Overtimes where selectedOver.Type == OvertimeType.Paid select selectedOver).ToList();

                foreach (Overtime over in overtimeList)
                {
                    Employee employee = (from emp in db.Employees where emp.EmployeeID == over.EmployeeID select emp).FirstOrDefault();

                    foreach (IWebElement tableElement in daysFromBtTable.AsEnumerable())
                    {
                        if (tableElement.Text.Contains(employee.FirstName) && tableElement.Text.Contains(employee.LastName))
                        {
                            Assert.IsTrue(tableElement.Text.Contains(over.Date.ToString(String.Format("dd.MM.yyyy"))));                            
                        }
                    }
                }
            }
        }

        [Test]
        public void VUDays_FirstRowData()
        {
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("BOARD", Browser.GetText("//*[@id='tbodyJourneys']/tr[1]/td[1]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual( "Coleman John", Browser.GetText("//*[@id='tbodyJourneys']/tr[1]/td[2]"));
            Thread.Sleep(Timings.Default_ms*20); 
            Assert.AreEqual("jton", Browser.GetText("//*[@id='tbodyJourneys']/tr[1]/td[3]"));
            Thread.Sleep(Timings.Default_ms*20); 
            Assert.AreEqual("15.02.2013", Browser.GetText("//*[@id='overtimeLink']"));
            Thread.Sleep(Timings.Default_ms*20); Assert.AreEqual("03.01.2013", Browser.GetText("//*[@id='overtimeLink']/redtext"));
            Thread.Sleep(Timings.Default_ms*20);
        }

        [Test]
        public void VUDays_SecondRowData()
        {
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("BOARD", Browser.GetText("//*[@id='tbodyJourneys']/tr[2]/td[1]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("George Anatoliy", Browser.GetText("//*[@id='tbodyJourneys']/tr[2]/td[2]") );
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("sfis", Browser.GetText("//*[@id='tbodyJourneys']/tr[2]/td[3]"));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual("03.01.2013", Browser.GetText("//*[@id='overtimeLink']/redtext"));
            Thread.Sleep(Timings.Default_ms*20);
        }

        [Test]
        public void ListEmployeesInTableDataCount()
        {
            Thread.Sleep(Timings.Default_ms*20);
            List<Employee> empsFromRepo = EmployeesTest.EmpsInRepositoryCount().AsEnumerable().Where(e=>e.IsUserOnly==false).ToList();

            Employee employee = empsFromRepo.FirstOrDefault();

            if (employee != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='tbodyJourneys']/tr");
                foreach (var element in empTable)
                {
                    if (element.Text.Contains(employee.LastName))
                    {
                        Assert.IsTrue(element.Text.Contains(employee.LastName));
                        Assert.IsTrue(element.Text.Contains(employee.FirstName));
                    }
                }
                Assert.AreEqual(empsFromRepo.Count, empTable.Count());
            }
        }
    }
}