using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.ADMPage
{
    [TestFixture]
    class EmployeesTabTests
    {
        #region Employees

        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        private string nothingFound = "No matching records found";
        private AjourBTForTestContext db = new AjourBTForTestContext();


        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "lgon";
            password = "corsad";
            Browser.Goto(baseURL);
            Browser.Wait(15);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Browser.Wait(10);
            Browser.ClickOnLink("ADM");
            Browser.ClickOnLink("Employees");
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/ADMView"));
        }


        public List<Employee> EmpsInRepositoryCount(string dept ="")
        {
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                List<Employee> emps =  new List<Employee>();
              
                if (dept!="")
                    emps = (from e in db.Employees.AsEnumerable()
                            where e.Department.DepartmentName == dept
                            orderby e.IsManager descending, e.DateDismissed, e.LastName
                            select e).ToList();
                else
                    emps = (from e in db.Employees.AsEnumerable()
                            orderby e.IsManager descending, e.DateDismissed, e.LastName
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
            Assert.NotNull(Browser.HasElement("ADM"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[3]/a"), "Employees");
        }


        [Test]
        [TestCase("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
        [TestCase("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
        [TestCase("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
        [TestCase("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
        public void Employees_SortingTable(string path, string column, bool ascending)
        {
            //Arrange
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.ClickByXPath(path);
            if (!ascending)
            {
                Browser.ClickByXPath(path);
            }
         
            var firstEmploeeID = Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[1]");
            var firstEmploeeName = Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[2]/a");
    
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
         
            string expected = "DEPT4";
           
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.ClickByXPath("/html/body/div[1]/div/div[3]/form/div/select");
            string defaultDept = Browser.GetText("/html/body/div[1]/div/div[3]/form/div/select/option[6]");
            int employees = Browser.Count("//tr[contains(@class,'zebra')]");

            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
            Assert.AreEqual(expected, defaultDept);
        }


        [Test]
        public void Employees_CountInDefaultDepartment()
        {
            //Arrange
            string expected = "DEPT4";

            //Act
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.ClickByXPath("/html/body/div[1]/div/div[3]/form/div/select");
            string defaultDept = Browser.GetText("/html/body/div[1]/div/div[3]/form/div/select/option[6]");
            int employees = Browser.Count("//tr[@id='ADMEmployees']");
            List<Employee> empsFromRepo = EmpsInRepositoryCount("DEPT4");
            //Assert
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
            Assert.AreEqual(expected, defaultDept);
            Assert.AreEqual(empsFromRepo.Count(), employees);
        }
  

        [Test]
        public void Employees_DropdownListOfDepartments_SelectedDepartment()
        {
            //Arrange
            string expected = "BOARD";
           
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.SelectOption("depDropList", "BOARD");
            int employees = Browser.Count("//tr[@id='ADMEmployees']");
            List<Employee> empsFromRepo = EmpsInRepositoryCount("BOARD");

            //Assert
            Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[3]/form/div/select/option[2]"));
            Assert.AreEqual(empsFromRepo.Count(), 10);

        }


        [Test]
        [TestCase("notexisting", false)]
        [TestCase("rkni", true)]
        public void SearchString(string serchString, bool isFound)
        {
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.SendKeysTo("seachInputEmployeesForADM", serchString, true);
            Browser.SendEnter("seachInputEmployeesForADM");
            Browser.Wait(5);
            Thread.Sleep(Timings.Default_ms*20);

            if (!isFound)
            {
                Browser.ClickByXPath("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr/td");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr/td"), nothingFound);
            }
            else
            {
                Browser.ClickByXPath("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr/td[1]");
                Thread.Sleep(Timings.Default_ms*20);
                Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr/td[1]"), "rkni");
            }
        }


         [Test]
        public void ExportToXls()
        {
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.ClickByXPath("/html/body/div[1]/div/div[3]/div/form/input[2]");
        }


         [Test]
         public void Get_elegant_Mails()
         {
             //Arange
             string expected = "Mailing list";
            
             //Act
             Browser.ClickByXPath("//button[@id='btnGetMailAliasEMailsADM']");
             Browser.Wait(5);
            
             //Assert
             Assert.AreEqual(Browser.GetText("//span[@id='ui-dialog-title-EMailData']"), expected);
             Browser.ClickByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
         }


         [Test]
         public void Get_es_Mails()
         {   
             //Arange
             string expected = "Mailing list";
            
             //Act
             Browser.ClickByXPath("//button[@id='btnGetSecondMailAliasEMailsADM']");

             //Assert
             Assert.AreEqual(Browser.GetText("//span[@id='ui-dialog-title-EMailData']"), expected);
             Browser.ClickByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
         }


         [TestFixtureTearDown]
         public void TeardownTest()
         {
             if (Browser.HasElement("Log off"))
                 Browser.ClickOnLink("Log off");
         }


         [Test]
         public void BTs_SendButton()
         {
             //Arrange
             Browser.ClickOnLink("Employees");
             Browser.SelectOption("depDropList", "All Departments");

             int employees = Browser.Count("//tr[contains(@class,'zebra')]");

             for (int count = 0; count < employees; count++)
             {
                 if (Browser.HasElement("Send"))
                     Console.WriteLine(count);
                 else
                 {
                     string btnText = Browser.IWebElementToString("//a[contains(@class,'ui-button')]", count, "id");
                     Console.WriteLine(btnText);
                 }
             }
             Assert.IsTrue(Browser.HasElement("Send"));
         }


         [Test]
         public void BTs_SendButton_EmployeeWithoutEMail()
         {
             //Arrange

             //Act
             Browser.ClickOnLink("Employees");
             Browser.SelectOption("depDropList", "All Departments");

             int employees = Browser.Count("//tr[contains(@class,'zebra')]");

             for (int count = 0; count < employees; count++)
             {
                 if (Browser.HasElement("Send"))
                     Console.WriteLine(count);
             }
             Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[3]"), "");
             Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[1]"), "maad");
         }


        [Test]
         public void BTs_SendButton_MailToFirstEmpoloyeeAddedButtonSend()
         {
             //Arrange
       
             //Act
             TeardownTest();
             baseURL = "http://localhost:50616/";
             username = "apat";
             password = "lokmop";
             Browser.Goto(baseURL);
             Browser.Wait(15);

             // Act
             Browser.SendKeysTo("UserName", username, true);
             Browser.SendKeysTo("Password", password, true);
             Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
             Browser.Wait(10);
             Browser.ClickOnLink("PU");
             Browser.ClickOnLink("Employees");
             Browser.ClickByXPath("//*[@id='PUEmployee']/tr[2]/td[2]/a");
             Browser.SendKeysTo("EMail","abc@EMail.com", true);
             Browser.ClickByXPath("//*[@id='btnSaveEmployee']/span/span");
             Thread.Sleep(Timings.Default_ms*20);
             TeardownTest();
             SetupTest();
             Browser.ClickOnLink("ADM");
             Browser.ClickOnLink("Employees");
             //Browser.SelectOption("depDropList", "All Departments");
             int employees = Browser.Count("//tr[contains(@class,'zebra')]");

             for (int count = 0; count < employees; count++)
             {
                 if (Browser.HasElement("Send"))
                     Console.WriteLine(count);
             }

             Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[6]/td[3]/a"), "Send");
             Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr[6]/td[1]"), "apat");
          
         }
      

        [Test]
        public void ClickOnSendButton()
        {
            Employee empMessageAllowed = new Employee();
            using (var db = new AjourBTForTestContext())
            {
                empMessageAllowed = (from emp in db.Employees where !(String.IsNullOrEmpty(emp.EMail)) && emp.DateDismissed == null select emp).FirstOrDefault();
            }

            if (empMessageAllowed != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("/html/body/div[1]/div/div[3]/div/div/div/div[1]/div[2]/table/tbody/tr");
                foreach (var element in empTable)
                {
                    if (element.Text.Contains(empMessageAllowed.FirstName))
                    {
                        Assert.IsTrue(element.Text.Contains(empMessageAllowed.LastName));
                        Assert.IsTrue(element.Text.Contains(empMessageAllowed.EID));
                    }
                }
            }
        }


         [Test]
         public void Employees_EmployeeDetails()
         {
             //Arrange
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Browser.SelectOption("depDropList", "All Departments");
            Browser.ClickOnLink("Reyes Brandon");
            IWebElement empTable = Browser.FindElementByXPath("//*[@id='Edit Employee']/fieldset/table/tbody");
            Assert.IsTrue(Browser.HasElementByXPath("//*[@id='ui-dialog-title-Edit Employee']"));
            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Employee']"), "Employee Details");
            Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-Edit Employee']"), "Employee Details");
 
            Assert.IsTrue(empTable.Text.Contains("Reyes"));
            Assert.IsTrue(empTable.Text.Contains("Brandon"));
            Assert.IsTrue(empTable.Text.Contains("ayou"));
            Assert.IsTrue(empTable.Text.Contains("DEPT2"));
            Assert.IsTrue(empTable.Text.Contains("11.04.2011"));
            Assert.IsTrue(empTable.Text.Contains("Software developer"));
            Assert.IsTrue(empTable.Text.Contains("10.11.1987"));
            Assert.IsTrue(empTable.Text.Contains("Джоні Роус"));
        }


        [Test]
        public void ListEmployees_AllDepts_Count()
        {
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Thread.Sleep(Timings.Default_ms*25);
            Browser.ClickByXPath("/html/body/div[1]/div/div[3]/form/div/select");
            Thread.Sleep(Timings.Default_ms * 25);
            Browser.SelectByValue("/html/body/div[1]/div/div[3]/form/div/select", "");
            Thread.Sleep(Timings.Default_ms*35);
            List<Employee> employee = EmpsInRepositoryCount();
            Employee emp = employee.FirstOrDefault();
            
            //Act
            if (emp != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//tr[@id='ADMEmployees']");
                Thread.Sleep(Timings.Default_ms*4);
                foreach (var element in empTable)
                {
                    if (element.Text.Contains(emp.EID))
                    {
                        Assert.IsTrue(element.Text.Contains(emp.LastName));
                        Assert.IsTrue(element.Text.Contains(emp.FirstName));
                    }
                }
                Assert.AreEqual(employee.Count, empTable.Count());
            }
        }

        [Test]
        public void CheckThatEducationPresentsInTable()
        {
            //Arrange
            Browser.Refresh();
            Browser.ClickOnLink("Employees");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.SelectOption("depDropList", "All Departments");
            Thread.Sleep(Timings.Default_ms * 10);

            //Act
            IWebElement table = Browser.FindElement(By.Id("employeeViewexample"), 2);

            //Assert  

            Assert.IsTrue(table.Text.Contains("базова вища, 03.01.2012"));
            Assert.IsTrue(table.Text.Contains("базова вища, 05.01." + DateTime.Now.AddYears(2).Year));
            Assert.IsTrue(table.Text.Contains("повна загальна середня, 01.05.2001; базова вища, 03.03." + DateTime.Now.AddYears(2).Year));
        }

        #endregion

    }
}
