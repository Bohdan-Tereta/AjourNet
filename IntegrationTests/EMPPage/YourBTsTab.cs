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

namespace IntegrationTests.EMPPage
{
    [TestFixture]
    class YourBTsTab
    {
        private string baseURL = "http://localhost:50616/";
        private string noBTs = "You have no BTs";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            Browser.ClickOnLink("EMP");

            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Your BTs");

            string YourBTs = Browser.FindElementByLinkText("Your BTs").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", YourBTs);
        }

        private IEnumerable<object> GetYears(AjourBTForTestContext context, string EID)
        {
            var selected = from bts in context.BusinessTrips.AsEnumerable()
                           where bts.BTof.EID == EID && bts.Status != BTStatus.Cancelled && (bts.Status == (BTStatus.Reported | BTStatus.Confirmed) ||
                                                  bts.Status == (BTStatus.Reported) ||
                                                  bts.Status == (BTStatus.Confirmed))
                           group bts by bts.StartDate.Year into yearGroup
                           orderby yearGroup.Key
                           select new { Year = yearGroup.Key };

            return selected;
        }

        private List<BusinessTrip> GetBtsInYear(AjourBTForTestContext context, int year, string EID)
        {
            List<BusinessTrip> btsInYear = (from bts in context.BusinessTrips.AsEnumerable() where
                                            ((bts.StartDate.Year == year) && (bts.BTof.EID == EID) && !bts.Status.HasFlag(BTStatus.Cancelled) &&
                                            (bts.Status.HasFlag(BTStatus.Registered) || bts.Status.HasFlag(BTStatus.Confirmed)))
                                            orderby bts.StartDate descending
                                            select new BusinessTrip(bts)).ToList();

            return btsInYear;
        }

        #region Your BTs

        [Test]
        public void CheckDropDownListValues_ealv()
        {
            IWebElement empTable;
            ReadOnlyCollection<IWebElement> dropDownListValues = Browser.FindElementsByXPath("//select[@id='selectedYear']/option");
            using (var db = new AjourBTForTestContext())
            {
                List<object> yearList = GetYears(db, "ealv").ToList();
                Assert.AreEqual(dropDownListValues.Count, yearList.ToList().Count);

                for(int i = 0; i < dropDownListValues.Count;i++)
                {
                    Browser.SelectOption("selectedYear", i);
                    int year = Convert.ToInt32(dropDownListValues[i].Text);
                    List<BusinessTrip> btForSelectedYear = GetBtsInYear(db, year, "ealv");

                    Thread.Sleep(Timings.Default_ms*20);

                    if (btForSelectedYear.Count == 0)
                    {
                        empTable = Browser.FindElementByXPath("//div[@id='btsByDatesEMP']/div/p/b");
                        Assert.IsTrue(empTable.Text.Contains(noBTs));
                    }
                    else
                    {
                        empTable = Browser.FindElementByXPath("//table[@id='reportedBTs']");
                        foreach (BusinessTrip bt in btForSelectedYear)
                        {
                            Assert.IsTrue(empTable.Text.Contains(bt.StartDate.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(empTable.Text.Contains(bt.EndDate.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(empTable.Text.Contains(bt.Location.Title));
                            Assert.IsTrue(empTable.Text.Contains(bt.OrderStartDate.Value.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(empTable.Text.Contains(bt.OrderEndDate.Value.ToString(String.Format("dd.MM.yyyy"))));
                            Assert.IsTrue(empTable.Text.Contains(bt.Manager));
                            Assert.IsTrue(empTable.Text.Contains(bt.Responsible));
                        }
                    }
                }
            }
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
