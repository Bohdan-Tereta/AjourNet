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

namespace IntegrationTests.VUPage
{  
    [TestFixture]
    public class BTsByDates
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
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }
        [Test]
        public void BTsByDatesTab()
        {
            //Arrange
            Browser.Refresh();
            Browser.ClickOnLink("VU");

            //Act
            //Assert
            Assert.NotNull(Browser.HasElement("VU"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[1]/a"), "BTs by Dates");
        }


        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Browser.Refresh();
            Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='replace']/input"), serchString, true);
            Browser.SendEnter(Browser.FindElementByXPath("//*[@id='replace']/input"));
            Browser.Wait(5);
            Thread.Sleep(Timings.Default_ms*20);

            if (!isFound)
            {
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDates_wrapper']/div[2]/div[2]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='exampleBtsViewByDates_wrapper']/div[2]/div[2]"), nothingFound);
            }
            else
            {
                Browser.ClickByXPath("//*[@id='exampleBtsViewByDates']/tbody/tr[1]/td[1]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='exampleBtsViewByDates']/tbody/tr[1]/td[1]"), "rkni");
            }
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
         public void YearDropDownList_defaultYear()
        {
            //Arrange
            Browser.Refresh();
            string expected = Browser.GetText("//*[@id='selectedYear']/option[1]");
             string data = Browser.GetText("//*[@id='exampleBtsViewByDates']/tbody/tr[1]/td[4]");

            //Act
            Browser.Refresh();
           
            //Assert
            Assert.AreEqual("2015", expected);
            Assert.IsTrue(data.Contains(selectedYear.ToString()));
        }

         [Test]
         public void YearDropDownList_selectYear()
         {
             //Act
             Browser.Refresh();
             Thread.Sleep(Timings.Default_ms * 20);

             string expected = "2015";
             string data = Browser.GetText("//*[@id='exampleBtsViewByDates']/tbody/tr[1]/td[4]");

             //Assert
             Assert.AreEqual("2015", expected);
             Assert.IsTrue(data.Contains((expected).ToString()));
         }

         [Test]
         public void LocationDropDownList_defaultLoc()
         {
             //Arrange
             Browser.Refresh();
             string expected = Browser.GetText("//*[@id='vuByDatesFirstHeader']/td[3]/span/select/option[1]");
             string data = Browser.GetText("//*[@id='exampleBtsViewByDates']/tbody/tr[1]/td[3]");
             IEnumerable<string> locations = (from l in db.Locations.AsEnumerable()
                                  select l.Title).ToList();
             //Act
             Browser.Refresh();

             //Assert
             Assert.AreEqual("All", expected);
             Assert.Contains(data, locations.ToList());
         }

         [Test]
         public void TableData()
         {
             int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
             Browser.Refresh();
             Thread.Sleep(Timings.Default_ms*10);
             Browser.ClickOnLink("BTs by Dates");
             Thread.Sleep(Timings.Default_ms*20);
             int year = DateTime.Now.ToLocalTimeAzure().Year;

                 List<Employee> employeeBts = (from bt in db.BusinessTrips.AsEnumerable()
                                                   where (bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled) || bt.Status == (BTStatus.Confirmed | BTStatus.Modified) || bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && bt.StartDate.Year == selectedYear
                                                   select bt.BTof).ToList();

                 Employee emp = employeeBts.FirstOrDefault();
                 if (emp != null)
                 {
                    ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='exampleBtsViewByDates']/tbody/tr");
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
    }
}
