using IntegrationTests;
using AjourBT.Domain.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWrapper;
using AjourBT.Domain.Entities;
using IntegrationTests.VUPage;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using System.Threading;

namespace IntegrationTests.ADMPage
{
    [TestFixture]
    public class VisasAndPermitsTabTests
    {

        #region ADM_Page

            private StringBuilder verificationErrors;
            private string baseURL = "http://localhost:50616/";
            private string username;
            private string password;
            AjourBTForTestContext db = new AjourBTForTestContext();
        
            [TestFixtureSetUp]
            public void SetupTest()
            {
                //Arrange
                username = "lgon";
                password = "corsad";
                Browser.Goto(baseURL);

                // Act
                Browser.SendKeysTo("UserName", username, true);
                Browser.SendKeysTo("Password", password, true);
                Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
                Browser.Wait(10);
                Browser.ClickOnLink("ADM");
                verificationErrors = new StringBuilder();

                Assert.That(Browser.IsAt(baseURL + "Home/ADMView"));
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
            public void CanLogInAsADM_ADMPage_CorrectLinks()
            {
                //Arrange

                //Act
                Browser.ClickOnLink("ADM");

                //Assert
                Assert.IsTrue(Browser.HasElement("ADM"));
                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(Browser.Url, baseURL + "Home/ADMView");
                Assert.AreEqual(Browser.Title, baseURL + "Home/ADMView");
                Assert.AreNotEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[1]/a"), "ACC");
                Assert.AreEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[1]/a"), "ADM");
                Assert.AreNotEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[2]/a"), "BTM");

            }


            [Test]
            public void TestADMPageTabs_CorrectTabName()
            {
                //Arrange

                //Act

                //Assert
                Assert.IsTrue(Browser.HasElement("ADM"));
                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[1]/a"), "Visas and Permits");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[2]/a"), "BTs");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[3]/a"), "Employees");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[4]/a"), "Messages");

            }

         public static List<Employee> EmpsInRepositoryCount(string dept = "")
        {
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                List<Employee> emps = new List<Employee>();

                if (dept != "")
                    emps = (from e in db.Employees.AsEnumerable()
                            where e.Department.DepartmentName == dept && e.DateDismissed == null
                            select e).ToList();
                else
                    emps = (from e in db.Employees.AsEnumerable()
                            where e.DateDismissed == null
                            select e).ToList();

                return emps;
            }
            }

            [Test]
            public void ADMPage_DropdownListOfDepartments_DefaultDepartment()
            {
                //Arrange
                string expected = "DEPT4";
                //Act
                Browser.Goto(baseURL + "Home/ADMView");
                Browser.SelectByValue("//*[@id='departmentName']", "DEPT4");
                List<Employee> result = EmpsInRepositoryCount("DEPT4");
                int employees = Browser.Count("//tr[contains(@class,'indexBT')]");

                //Assert
                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[6]"));
                Assert.AreEqual(result.Count, employees);
            }


            [Test]
            public void ADMPage_DropdownListOfDepartments_AllDepartments()
            {
                //Arrange
                string expected = "All Departments";
                //Act
                Browser.Goto(baseURL + "Home/ADMView");
                Browser.SelectOption("departmentName", "All Departments");
                int employees = Browser.Count("//tr[contains(@class,'indexBT')]");
                List<Employee> result = EmpsInRepositoryCount();

                //Assert

                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[1]"));
                
            }

            [Test]
            public void ADMPage_DropdownListOfDepartments_SelectedDepartment()
            {
                //Arrange
                string expected = "BOARD";

                //Act
                Browser.SelectOption("departmentName", "BOARD");
                Thread.Sleep(Timings.Default_ms*20);
                int employees = Browser.Count("//tr[contains(@class,'indexBT')]");
                List<Employee> result = EmpsInRepositoryCount("BOARD");

                //Assert
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[2]"));
                Assert.AreEqual(result.Count, employees);

            }

            #region VisasAndPermits


            [Test]
            public void VisasAndPermitsTab()
            {
                //Arrange

                Browser.ClickOnLink("Visas and Permits");

                //Act
                //Assert

                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[1]/a"), "Visas and Permits");
                Assert.IsTrue(Browser.HasElement("Visas and Permits"));

            }


            [Test]
            public void VisasAndPermits_TableColumns()
            {
                //Arrange

                //Act

                //Assert
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]"), "EID");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]"), "Name");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[3]"), "Passport");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[4]"), "Type");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[5]"), "Visa From - To");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[6]"), "Entries");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[7]"), "Days");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[8]"), "Registration");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[9]"), "Num");
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[10]"), "Permit From - To");
            }

            [Test]
            public void VisasAndPermits_TableRow()
            {
                //Arrange
                Browser.Refresh();
                Browser.ClickOnLink("Visas and Permits");

                //Act
                //Assert
                Assert.AreEqual("rkni", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[1]"));
                Assert.AreEqual("Knight Robert", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[2]"));
                Assert.AreEqual("yes", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[3]"));
                Assert.AreEqual("V_C07", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[4]"));
                Assert.AreEqual(String.Format("{0:dd'.'MM'.'yyyy}", DateTime.Now.ToLocalTimeAzure().AddDays(-95).Date.ToString("dd.MM.yyyy") + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(95).Date.ToString("dd.MM.yyyy")), Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[5]"));
                Assert.AreEqual("4 (2)", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[6]"));
                Assert.AreEqual("180", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[7]"));
                Assert.AreEqual("", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[8]"));
                Assert.AreEqual("", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[9]"));
                Assert.AreEqual("Karta Polaka", Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[10]"));

            }

            [Test]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
            public void VisasAndPermits_SortingTable(string path, string column, bool ascending)
            {
                //Arrange

                //Act
                Browser.Refresh();
                Browser.ClickOnLink("Visas and Permits");
                Browser.ClickByXPath(path);
                if (!ascending)
                    Browser.ClickByXPath(path);
                var firstEmploeeID = Browser.GetText("//tr[contains(@class,'indexBT')]", 0);
                var firstEmploeeName = Browser.GetText("/html/body/div[1]/div/div[1]/div/div/div[1]/div[2]/table/tbody/tr[1]/td[2]");

                //Assert
                if (column == "EID")
                    if (ascending)
                        Assert.IsTrue(firstEmploeeID.StartsWith("a"));
                    else
                        Assert.IsFalse(firstEmploeeID.StartsWith("a"));
                else
                    if (ascending)
                        Assert.IsTrue(firstEmploeeName.StartsWith("A"));
                    else
                        Assert.IsFalse(firstEmploeeName.StartsWith("A"));
            }


          
            #endregion

                         
        #endregion

        
    }
}