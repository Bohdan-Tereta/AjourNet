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
using AjourBT.Domain.Infrastructure;

namespace IntegrationTests.VUPage
{
    [TestFixture]
    public class BTsByUnitsTabTests
    {
        

        private StringBuilder verificationErrors;
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        public  AjourBTForTestContext db = new AjourBTForTestContext();


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "ayou";
            password = "123456";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Browser.Wait(10);
            Browser.ClickOnLink("VU");
            Browser.ClickOnLink("BTs by Units");

            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            {
                if (Browser.HasElement("Log off"))
                    Browser.ClickOnLink("Log off");
            }
        }

        [Test]
        public void BtsByUnits_TableColumns()
        {
            //Arrange

            //Act

            //Assert
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[1]"), "ID");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[2]"), "EID");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[3]"), "Name");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[4]"), "Loc");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[5]"), "From");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[6]"), "To");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[7]"), "Unit");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[8]"), "Purpose");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[9]"), "Mgr");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[1]/div/table/thead/tr[2]/th[10]"), "Resp");

        }

        
        [Test]
        public void BtsByUnits_TableRows()
        {
            //Arrange
            using(AjourBTForTestContext db = new AjourBTForTestContext()){
            BusinessTrip employeeBt = (from bt in db.BusinessTrips.AsEnumerable()
                                              where (bt.Status == BTStatus.Cancelled || bt.Status == (BTStatus.Confirmed | BTStatus.Modified) || bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && bt.StartDate.Year == DateTime.Now.ToLocalTimeAzure().Year
                                              orderby bt.UnitID
                                              select bt).FirstOrDefault();
            
            //Act

            //Assert
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[1]"), employeeBt.UnitID.ToString());
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[2]"), employeeBt.BTof.EID);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[3]"), String.Format(employeeBt.BTof.LastName +" "+employeeBt.BTof.FirstName));
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[4]"), employeeBt.Location.Title);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[7]"), employeeBt.Unit.ShortTitle.ToString());
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[8]"), employeeBt.Purpose);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[9]"), employeeBt.Manager);
                    Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[10]"), employeeBt.Responsible);
                    }
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[1]/td[1]"), "1");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[2]"), "swhe");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[3]"), "Harper Earl");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[4]"), "RB/MA");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[7]"), "-");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[8]"), "Meeting");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[9]"), "ncru");
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[6]/div/div/div[2]/div[2]/table/tbody/tr[2]/td[10]"), "mter");
        }

        [Test]
        public void Default_AllDeptsts()
        {
            //Arrange
            string unit = Browser.FindElementByLinkText("BTs by Units").GetCssValue("color");

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual("rgba(225, 112, 9, 1)", unit);
        }


        public List<BusinessTrip> btsByUnitsFromRepository(int selectedYear)
        {
            List<BusinessTrip> employeeBts = new List<BusinessTrip>();
          
                 employeeBts = (from bt in db.BusinessTrips.AsEnumerable()
                                                  where (bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled) || bt.Status == (BTStatus.Confirmed | BTStatus.Modified) || bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && bt.StartDate.Year == selectedYear
                                                  select bt).ToList();
            
            return employeeBts;
        }


        [Test]
        public void DropdownListOfYears_DefaultYear()
        {
            //Arrange
            int year = DateTime.Now.ToLocalTimeAzure().Year;
         
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            string defaultYear = Browser.GetText("/html/body/div[1]/div/div[6]/form/div/select/option[1]");
            int rows = btsByUnitsFromRepository(year).Count();
            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year);
        
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
                foreach (var element in empTable)
                {
                    Assert.IsTrue(element.Text.Contains(year.ToString()));            
                }
           
            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual("2015", defaultYear);
            Assert.AreEqual(employeeBts.Count, rows);

        }


        [Test]
        public void DropdownListOfYears_SelectedYear()
        {
            //Arrange
            int year = DateTime.Now.ToLocalTimeAzure().Year - 1;

            //Act
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickOnLink("BTs by Units");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectByValue("/html/body/div[1]/div/div[6]/form/div/select", year.ToString());
            Thread.Sleep(Timings.Default_ms*10);
            int rows = btsByUnitsFromRepository(year).Count();
            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
            foreach (var element in empTable)
            {
                Assert.IsTrue(element.Text.Contains(year.ToString()));
            }
            //Assert
            Assert.AreEqual(employeeBts.Count, rows);
        }


        [Test]
        public void DropdownListOfUnits_DefaultUnit()
        {
            //Arrange
            string unit  = "All";
            int year = DateTime.Now.ToLocalTimeAzure().Year;
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            string defaultUnit = Browser.GetText("//*[@id='vuByUnitsFirstHeader']/td[7]/span/select/option[1]");
            int rows = btsByUnitsFromRepository(year).Count();
            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
            
            //Assert
            foreach (var element in empTable)
            {        
                Assert.IsTrue(element.Text.Contains("-")||element.Text.Contains("EPUA_B")||element.Text.Contains("BD"));
            }
            Assert.AreEqual(employeeBts.Count, rows);
            Assert.AreEqual(unit, defaultUnit);

        }


        [Test]
        public void DropdownListOfUnits_SelectedUnit()
        {
            //Arrange
            string unit = "-";
            int year = DateTime.Now.ToLocalTimeAzure().Year;
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectByValue("//*[@id='vuByUnitsFirstHeader']/td[7]/span/select", unit);
            Thread.Sleep(Timings.Default_ms*10);
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                int rows = (from u in btsByUnitsFromRepository(year).AsEnumerable()
                            where u.Unit.ShortTitle == unit
                            select u).ToList().Count();
                List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year).Where(b => b.Unit.ShortTitle == unit).ToList();

                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
                foreach (var element in empTable)
                {
                    Assert.IsTrue(element.Text.Contains("-"));
                }
                //Assert
                Assert.AreEqual(employeeBts.Count, rows);
                Assert.AreEqual(unit, Browser.GetText("//*[@id='vuByUnitsFirstHeader']/td[7]/span/select/option[2]"));
            }
        }


        [Test]
        public void DropdownListOfLocations_DefaultLocation()
        {
            //Arrange
            string unit = "All";
            int year = DateTime.Now.ToLocalTimeAzure().Year;
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            string defaultLoc = Browser.GetText("//*[@id='vuByUnitsFirstHeader']/td[4]/span/select/option[1]");
            int rows = btsByUnitsFromRepository(year).Count();
            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year);

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");

            //Assert
            foreach (var element in empTable)
            {
                Assert.IsTrue(element.Text.Contains("AT/BW") || element.Text.Contains("RB/MA") || element.Text.Contains("LO/JA")||
                    element.Text.Contains("EC/BV") || element.Text.Contains("HO/LR"));
            }
            Assert.AreEqual(employeeBts.Count, rows);
            Assert.AreEqual(unit, defaultLoc);

        }

        [Test]
        public void DropdownListOfLocations_SelectedLocations()
        {
            //Arrange
            string loc = "AT/BW";
            int year = DateTime.Now.ToLocalTimeAzure().Year;
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SelectByValue("//*[@id='vuByUnitsFirstHeader']/td[4]/span/select", loc);
            Thread.Sleep(Timings.Default_ms*10);
            int rows = btsByUnitsFromRepository(year).Where(l=>l.Location.Title==loc).ToList().Count();
            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year).Where(l=>l.Location.Title==loc).ToList();

            ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
            foreach (var element in empTable)
            {
                Assert.IsTrue(element.Text.Contains(loc));
            }
            //Assert
            Assert.AreEqual(employeeBts.Count, rows);
            Assert.AreEqual(loc, Browser.GetText("//*[@id='vuByUnitsFirstHeader']/td[4]/span/select/option[2]"));
        }


        [Test]
        public void ExportToXls()
        {
            Browser.ClickByXPath("//*[@id='ExportBusinessTripByUnitsToXls']");
        }

        [Test]
        public void TableData()
        {
            Browser.Refresh();
            Browser.ClickOnLink("BTs by Units");
            Thread.Sleep(Timings.Default_ms*20);
            int year = DateTime.Now.ToLocalTimeAzure().Year;

            List<BusinessTrip> employeeBts = btsByUnitsFromRepository(year);
            Employee bt = btsByUnitsFromRepository(year).Distinct().FirstOrDefault().BTof;
            if (bt != null)
                {
                    ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByUnits']/tbody/tr");
                    Thread.Sleep(Timings.Default_ms*20);

                    foreach (var element in empTable)
                    {
                        if (element.Text.Contains(bt.LastName))
                        {
                            Assert.IsTrue(element.Text.Contains(bt.FirstName));
                            Assert.IsTrue(element.Text.Contains(bt.LastName));

                        }
                    }
                    Assert.AreEqual(employeeBts.Count, empTable.Count());
                }
            }
        
    }
}
