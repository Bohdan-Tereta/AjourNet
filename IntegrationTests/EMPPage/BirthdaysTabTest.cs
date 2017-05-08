using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.EMPPage
{
    [TestFixture]
    class BirthdaysTabTest
    {
        private string baseURL = "http://localhost:50616/";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            Browser.ClickOnLink("EMP");

            Assert.That(Browser.IsAt(baseURL + "Home/EMPView"));
            Browser.ClickOnLink("Birthdays");

            string Birthdays = Browser.FindElementByLinkText("Birthdays").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Birthdays);
        }

        public DateTime TransformBirthDate(DateTime Birthdate, DateTime nowDate)
        {
            DateTime tempDate = new DateTime(nowDate.Year, Birthdate.Month, Birthdate.Day);
            DateTime transfDate = new DateTime();
            if (nowDate.Month == 12 && nowDate.Day >= 2 && Birthdate.Month == 1)
            {
                transfDate = tempDate.AddYears(1);
                return transfDate;
            }
            else return tempDate;
        }

        #region Birthdays Tab

        [Test]
        public void CheckBirthdays()
        {
            DateTime fromDate = DateTime.Now.Date;
            DateTime toDate = fromDate.AddDays(30).Date;
            List<Employee> birthList = new List<Employee>();

            using (var db = new AjourBTForTestContext())
            {                
                List<Employee> emp = (from e in db.Employees
                                      where
                                          e.DateDismissed == null &&
                                          e.BirthDay.HasValue
                                      orderby e.BirthDay.Value
                                      select e).ToList();

                DateTime date;
                foreach (Employee e in emp)
                {
                    date = TransformBirthDate(e.BirthDay.Value, DateTime.Now.Date);
                    if (date >= fromDate.AddDays(-10) && date <= toDate)
                    {
                        birthList.Add(e);
                    }
                }
            }

            ReadOnlyCollection<IWebElement> bdayList = Browser.FindElementsByXPath("//table[@id='bdayTable']/tbody/tr");

            Assert.AreEqual(birthList.Count, bdayList.Count);
        }

        #endregion

    }
}
