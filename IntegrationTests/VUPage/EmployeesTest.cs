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
   public class EmployeesTest
    {
        #region Employees

        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private string nothingFound = "No matching records found";
        //private AjourBTForTestContext db = new AjourBTForTestContext();


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "ayou";
            password = "123456";
            Browser.Goto(baseURL);
            Browser.Wait(15);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Browser.Wait(10);
            Browser.ClickOnLink("VU");
            Browser.ClickOnLink("Employees");
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
        }

        public static List<Employee> EmpsInRepositoryCount(string dept = "")
        {
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                List<Employee> emps = new List<Employee>();

                if (dept != "")
                    emps = (from e in db.Employees.AsEnumerable()
                            where e.Department.DepartmentName == dept
                            select e).ToList();
                else
                    emps = (from e in db.Employees.AsEnumerable()
                            select e).ToList();

                return emps;
            }
        }
        [Test]
        public void EmployeesTab()
        {
            //Arrange
            Browser.ClickOnLink("Employees");

            //Act
            //Assert
            Assert.NotNull(Browser.HasElement("VU"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[9]/a"), "Employees");
        }

        [Test]
        [TestCase("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
        [TestCase("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
        [TestCase("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
        [TestCase("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
        public void Employees_SortingTable(string path, string column, bool ascending)
        {
            //Arrange
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            if (column != "EID")
                Browser.ClickByXPath(path);
            if (!ascending)
            {
                Browser.ClickByXPath(path);
            }

            var firstEmploeeID = Browser.GetText("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[2]/table/tbody/tr[1]/td[1]");
            var firstEmploeeName = Browser.GetText("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[2]/table/tbody/tr[1]/td[2]");

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

        [Test]
        public void Employees_DropdownListOfDepartments_DefaultDepartment()
        {
            //Arrange

            string expected = "All Departments";

            //Act
            Thread.Sleep(Timings.Default_ms*5);
            string defaultDept = Browser.GetText("/html/body/div[1]/div/div[9]/form/div/select/option[1]");
            int employees = Browser.Count("//tr[contains(@class,'zebra')]");

            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(expected, defaultDept);
        }

        [Test]
        public void Employees_CountInDefaultDepartment()
        {
            //Arrange
            string expected = "All Departments";

            //Act
            Thread.Sleep(Timings.Default_ms*20);
           
            string defaultDept = Browser.GetText("/html/body/div[1]/div/div[9]/form/div/select/option[1]");
            int employees = Browser.Count("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[2]/table/tbody/tr");
            List<Employee> empsFromRepo = EmpsInRepositoryCount();
            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(expected, defaultDept);
            Assert.AreEqual(employees, empsFromRepo.Count());
        }


        [Test]
        public void Employees_DropdownListOfDepartments_SelectedDepartment()
        {
            //Arrange
            string expected = "BOARD";
            List<Employee> empsFromRepo = new List<Employee>();

            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("/html/body/div[1]/div/div[9]/form/div/select");
            Browser.ClickByXPath("/html/body/div[1]/div/div[9]/form/div/select/option[2]");
            int employees = Browser.Count("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[2]/table/tbody/tr");
            using (var db = new AjourBTForTestContext())
            {
                empsFromRepo = db.Employees.Where(emp => emp.Department.DepartmentName == "BOARD").ToList();
            }

            //Assert
            Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[9]/form/div/select/option[2]"));
            Assert.AreEqual(empsFromRepo.Count(), 10);

        }


        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("seachInputEmployeesForADM", serchString, true);
            Browser.SendEnter("seachInputEmployeesForADM");
            Browser.Wait(5);
            Thread.Sleep(Timings.Default_ms*20);

            if (!isFound)
            {
                Browser.ClickByXPath("//*[@id='PUEmployee']/tr/td");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='PUEmployee']/tr/td"), nothingFound);
            }
            else
            {
                Browser.ClickByXPath("//*[@id='PUEmployee']/tr/td[1]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("//*[@id='PUEmployee']/tr/td[1]"), "rkni");
            }

            Browser.SendKeysTo("seachInputEmployeesForADM", " ", true);
        }

        [Test]
        public void ExportToXls()
        {
            Browser.ClickByXPath("/html/body/div[1]/div/div[9]/div/div/form/input[2]");
        }

        [Test]
        public void ListEmployees_Count()
        {
            List<Employee> employee = EmpsInRepositoryCount().Where(e => e.IsUserOnly == false).ToList();
            Employee emp = employee.FirstOrDefault();
            //Act

            if (emp != null)
                {
                    ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("/html/body/div[1]/div/div[9]/div/div/div[1]/div[1]/div[2]/table/tbody/tr");
                    foreach (var element in empTable)
                    {
                        if (element.Text.Contains(emp.FirstName))
                        {
                            Assert.IsTrue(element.Text.Contains(emp.LastName));
                            Assert.IsTrue(element.Text.Contains(emp.EID));
                        }
                    }
                    Assert.IsTrue(employee.Count == empTable.Count() - 5 || employee.Count == empTable.Count() - 7);
                }
            }
        


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }


      
        #endregion

    }
}
