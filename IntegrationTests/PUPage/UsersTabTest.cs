using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.PUPage
{
    [TestFixture]
    class UsersTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string searchResultFail = "No matching records found";


        private void AddSomeUsers()
        {
            List<Employee> usersToAdd = new List<Employee> 
            {
                new Employee {EmployeeID = 86, FirstName = "Gerasum", LastName = "Mumu", DepartmentID = 2, EID = "gemu", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 87, FirstName = "Nestor", LastName = "Shyfruch", DepartmentID = 3, EID = "shne", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 88, FirstName = "George", LastName = "Dao", DepartmentID = 4, EID = "dago", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 89, FirstName = "Onyfriy", LastName = "Boo", DepartmentID = 5, EID = "boon", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 90, FirstName = "Kristy", LastName = "Fenshyi", DepartmentID = 6, EID = "fekr", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 91, FirstName = "Li", LastName = "Moon", DepartmentID = 7, EID = "moli", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 92, FirstName = "Lily", LastName = "Anderson", DepartmentID = 9, EID = "lian", IsUserOnly = true , IsGreetingMessageAllow = true}                
            };

            using (var db = new AjourBTForTestContext())
            {
                foreach (var item in usersToAdd)
                {
                    db.Employees.Add(item);
                }
                db.SaveChanges();
            }
        }

        private void DeleteSomeUsers(int startId, int count)
        {
            using (var db = new AjourBTForTestContext())
            {
                for (int id = 0; id < count; id++)
                {
                    Employee emp = db.Employees.ToList().LastOrDefault();
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                }
            }
        }


        [TestFixtureSetUp]
        public void Login()
        {
            //Arrange
            AddSomeUsers();            
        }

        [SetUp]
        public void SetUp()
        {
            Browser.webDriver.Manage().Window.Maximize();
            username = "apat";
            password = "lokmop";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("PU");

            Browser.ClickOnLink("Users");

            string Users = Browser.FindElementByLinkText("Users").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Users); 

        }

        [TestFixtureTearDown]
        public void TeardownTest()
        {
            DeleteSomeUsers(86, 7);
        }

        [TearDown]
        public void TearDown()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        } 

        #region Users Tab

        [Test]
        public void CheckUsersData()
        {
            using (var db = new  AjourBTForTestContext())
            {
                List<Employee> users = db.Employees.Where(e => e.IsUserOnly).ToList();
                VerifyUsersTable(users); 

            }
        }

        [Test]
        public void Search_BadText()
        {
            IWebElement userSearchInput = Browser.FindElementByXPath("//input[@id='userSearchInput']");
            Browser.SendKeysTo(userSearchInput, "Bad Text", true);
            Browser.SendEnter(userSearchInput);

            Thread.Sleep(Timings.Default_ms*15);

            IWebElement errorText = Browser.FindElementByXPath("//td[@class='dataTables_empty']");
            Assert.IsTrue(errorText.Text.Contains(searchResultFail));

            Browser.SendKeysTo(userSearchInput, Keys.Enter, true);
        }

        [Test]
        public void Search_user()
        {
            IWebElement userSearchInput = Browser.FindElementByXPath("//input[@id='userSearchInput']");
            Browser.SendKeysTo(userSearchInput, "user", true);
            Browser.SendEnter(userSearchInput);

            Thread.Sleep(Timings.Default_ms*30);

            ReadOnlyCollection<IWebElement> resultTable = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");

            Assert.AreEqual(5, resultTable.Count);

            Browser.SendKeysTo(userSearchInput, Keys.Enter, true);
        }

        [Test]
        public void TestDropDownList()
        {
            Dictionary<int, string> magicDictonary = new Dictionary<int, string>(); //<option number in DropDownList, EID>
            magicDictonary.Add(3, "gemu");
            magicDictonary.Add(4, "shne");
            magicDictonary.Add(5, "dago");
            magicDictonary.Add(6, "boon");
            magicDictonary.Add(7, "fekr");
            magicDictonary.Add(8, "moli");
            magicDictonary.Add(9, "lian");

            IWebElement dropDownList = Browser.FindElementByXPath("//select[@id='userDepDropList']");
            Thread.Sleep(Timings.Default_ms*10);

            using (var db = new AjourBTForTestContext())
            {
                foreach (var item in magicDictonary)
                {
                    Employee emp = db.Employees.Where(e => e.EID == item.Value).FirstOrDefault();
                    Browser.SelectOption("userDepDropList", item.Key);
                    Thread.Sleep(Timings.Default_ms*20);

                    IWebElement table = Browser.FindElementByXPath("//tbody[@id='userTableBody']/tr");

                    Assert.IsTrue(table.Text.Contains(emp.EID));
                    Assert.IsTrue(table.Text.Contains(emp.FirstName));
                    Assert.IsTrue(table.Text.Contains(emp.LastName));
                }
            }
        }


        #region SaveSortAfterTableRefreshDetails
        [Test]
        public void SaveSortAfterTableRefresh_EIDAscSortDetailsView_SortPreserved()
        {
            IWebElement sortByEID;
            List<Employee> users;

            sortByEID = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]");
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderBy(e => e.EID).ToList();
            }
            VerifyUsersTable(users); 
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20); 
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-dialog-titlebar-close")); 
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);
        }

        [Test]
        public void SaveSortAfterTableRefresh_EIDDescSortDetailsView_SortPreserved()
        {
            IWebElement sortByEID;
            List<Employee> users; 

            sortByEID = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]");
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderByDescending(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-dialog-titlebar-close"));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);

        }

        [Test]
        public void SaveSortAfterTableRefresh_NameAscSortDetailsView_SortPreserved()
        {
            IWebElement sortByName;
            List<Employee> users;

            sortByName = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]");
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderBy(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-dialog-titlebar-close"));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);

           
        }

        [Test]
        public void SaveSortAfterTableRefresh_NameDescSortDetailsView_SortPreserved()
        {
            IWebElement sortByName;
            List<Employee> users;

            sortByName = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]");
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderByDescending(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElementByClassName("ui-dialog-titlebar-close"));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);

           
        }
        #endregion 

        #region SaveSortAfterTableRefreshEdit
        [Test]
        public void SaveSortAfterTableRefresh_EIDAscSortEditView_SortPreserved()
        {
            IWebElement sortByEID;
            List<Employee> users;

            sortByEID = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]");
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderBy(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveEmployee"), 2));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);
        }

        [Test]
        public void SaveSortAfterTableRefresh_EIDDescSortEditView_SortPreserved()
        {
            IWebElement sortByEID;
            List<Employee> users;

            sortByEID = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]");
            Browser.ClickOnWebElement(sortByEID);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderByDescending(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveEmployee"), 2));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);

        }

        [Test]
        public void SaveSortAfterTableRefresh_NameAscSortEditView_SortPreserved()
        {
            IWebElement sortByName;
            List<Employee> users;

            sortByName = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]");
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderBy(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveEmployee"), 2));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);


        }

        [Test]
        public void SaveSortAfterTableRefresh_NameDescSortEditView_SortPreserved()
        {
            IWebElement sortByName;
            List<Employee> users;

            sortByName = Browser.FindElementByXPath("//*[@id='PuUsersTable_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]");
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(sortByName);
            Thread.Sleep(Timings.Default_ms*20);
            using (var db = new AjourBTForTestContext())
            {
                users = db.Employees.Where(e => e.IsUserOnly).OrderByDescending(e => e.EID).ToList();
            }
            VerifyUsersTable(users);
            Browser.ClickByXPath("//*[@id='userTableBody']/tr[1]/td[2]/a ");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("btnSaveEmployee"), 2));
            Thread.Sleep(Timings.Default_ms*20);
            VerifyUsersTable(users);


        }
        #endregion

        private static void VerifyUsersTable(List<Employee> users)
        {
            ReadOnlyCollection<IWebElement> usersTableRows = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");
            Assert.AreEqual(usersTableRows.Count, users.Count);

                for(int i=0; i< users.Count(); i++)
                {
                    ReadOnlyCollection<IWebElement> usersTableTD = usersTableRows[i].FindElements(By.CssSelector("td"));
                    if (usersTableTD[0].Text.Contains(">" + users[i].EID + "<"))
                    {
                        Console.WriteLine(users[i].FirstName + " - " + usersTableTD[ i ].Text);
                        Assert.IsTrue(usersTableTD[1].Text.Contains(users[i].FirstName));
                        Assert.IsTrue(usersTableTD[1].Text.Contains(users[i].LastName));
                    }
                }
        }

        #endregion

        #region AddUser button

        [TestFixture]
        class AddUsers
        {
            private string baseURL = "http://localhost:50616/";
            private string username;
            private string password;
            private string searchResultFail = "No matching records found";

            [TestFixtureSetUp]
            public void SetUp()
            {
                Browser.webDriver.Manage().Window.Maximize();
                username = "apat";
                password = "lokmop";
                Browser.Goto(baseURL);

                // Act
                Browser.SendKeysTo("UserName", username, true);
                Browser.SendKeysTo("Password", password, true);
                Browser.ClickByXPath("//input[@type='submit']");
                Browser.ClickOnLink("PU");

                Browser.ClickOnLink("Users");

                string Users = Browser.FindElementByLinkText("Users").GetCssValue("color");
                Assert.AreEqual("rgba(225, 112, 9, 1)", Users);

            }

            [TestFixtureTearDown]
            public void TearDown()
            {
                if (Browser.HasElement("Log off"))
                    Browser.ClickOnLink("Log off");
            }


            [Test]
            public void AddUserButton()
            {
                string buttonsPath = "//*[@id='CreateUser']";
                IWebElement buttonAdd = Browser.FindElementByXPath(buttonsPath);

                Assert.AreEqual("Add User", Browser.GetText(buttonsPath));
                Assert.IsTrue(buttonAdd.Text == "Add User");

            }

            [Test]

            public void UserWithAllRolesCanBeAdded()
            {
                string buttonAddPath = "//*[@id='CreateUser']";
                string buttonSavePath = "/html/body/div[3]/div[2]/form/div/button";
                ReadOnlyCollection<IWebElement> tableBeforeAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");

                IWebElement buttonAdd = Browser.FindElementByXPath(buttonAddPath);
                Browser.ClickByXPath(buttonAddPath);
                Thread.Sleep(Timings.Default_ms*20);

                Browser.SendKeysTo("FirstName", "firstName", false);
                Browser.SendKeysTo("LastName", "lastName", false);
                Browser.SendKeysTo("EID", "eeid", false);
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[1]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[2]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[3]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[4]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[5]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[6]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[7]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[8]");
                Browser.ClickByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[4]/td[2]/input[9]");
                Browser.SendKeysTo("Comment", "comment", false);

                Browser.ClickByXPath(buttonSavePath);
                Thread.Sleep(Timings.Default_ms*15);

                ReadOnlyCollection<IWebElement> tableAfterAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");
                IWebElement addedUser = Browser.FindElementByXPath("/html/body/div[1]/div/div[5]/div/div/div[1]/div[2]/table/tbody/tr[1]");
                Thread.Sleep(Timings.Default_ms*15);
                Assert.IsTrue(addedUser.Text.Contains("eeid"));
                Assert.IsTrue(addedUser.Text.Contains("firstName"));
                Assert.IsTrue(addedUser.Text.Contains("lastName"));
                Assert.Greater(tableAfterAdd.Count(), tableBeforeAdd.Count());
            }


            [Test]
            public void UserWithoutRolesCanBeAdded()
            {
                string buttonAddPath = "//*[@id='CreateUser']";
                string buttonSavePath = "/html/body/div[3]/div[2]/form/div/button";
                ReadOnlyCollection<IWebElement> tableBeforeAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");

                IWebElement buttonAdd = Browser.FindElementByXPath(buttonAddPath);
                Browser.ClickByXPath(buttonAddPath);
                Thread.Sleep(Timings.Default_ms*20);

                Browser.SendKeysTo("FirstName", "Jonny", false);
                Browser.SendKeysTo("LastName", "Rey", false);
                Browser.SendKeysTo("EID", "aore", false);
                Browser.SendKeysTo("Comment", "comment", false);

                Browser.ClickByXPath(buttonSavePath);
                Thread.Sleep(Timings.Default_ms*20);

                ReadOnlyCollection<IWebElement> tableAfterAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");
                IWebElement addedUser = Browser.FindElementByXPath("/html/body/div[1]/div/div[5]/div/div/div[1]/div[2]/table/tbody/tr[1]");
                Thread.Sleep(Timings.Default_ms*10);
                Assert.IsTrue(addedUser.Text.Contains("Jonny"));
                Assert.IsTrue(addedUser.Text.Contains("aore"));
                Assert.IsTrue(addedUser.Text.Contains("Rey"));
                Assert.Greater(tableAfterAdd.Count(), tableBeforeAdd.Count());
            }

            [Test]
            public void UserWithSameEID_CannotBeAddedValidationError()
            {
                string buttonAddPath = "//*[@id='CreateUser']";
                string buttonSavePath = "/html/body/div[3]/div[2]/form/div/button";
                ReadOnlyCollection<IWebElement> tableBeforeAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");

                IWebElement buttonAdd = Browser.FindElementByXPath(buttonAddPath);
                Browser.ClickByXPath(buttonAddPath);
                Thread.Sleep(Timings.Default_ms*20);

                Browser.SendKeysTo("FirstName", "Jonny", false);
                Browser.SendKeysTo("LastName", "Rey", false);
                Browser.SendKeysTo("EID", "aore", false);
                Browser.SendKeysTo("Comment", "comment", false);

                Browser.ClickByXPath(buttonSavePath);
                Thread.Sleep(Timings.Default_ms*10);

                ReadOnlyCollection<IWebElement> tableAfterAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");
                IWebElement addedUser = Browser.FindElementByXPath("/html/body/div[1]/div/div[5]/div/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual("Employee with EID aore already exists", Browser.GetText("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[3]/td[2]/div"));
                Assert.AreEqual(tableAfterAdd.Count(), tableBeforeAdd.Count());
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms*15);

            }

            [Test]
            public void UserWithEmptyFields_CannotBeAddedValidationErrors()
            {
                string buttonAddPath = "//*[@id='CreateUser']";
                string buttonSavePath = "/html/body/div[3]/div[2]/form/div/button";
                ReadOnlyCollection<IWebElement> tableBeforeAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");

                IWebElement buttonAdd = Browser.FindElementByXPath(buttonAddPath);
                Browser.ClickByXPath(buttonAddPath);
                Thread.Sleep(Timings.Default_ms*20);

                Browser.ClickByXPath(buttonSavePath);
                Thread.Sleep(Timings.Default_ms*20);

                ReadOnlyCollection<IWebElement> tableAfterAdd = Browser.FindElementsByXPath("//tbody[@id='userTableBody']/tr");
                Assert.AreEqual("Field First Name is required", Browser.GetText("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[2]/span/span"));
                Assert.AreEqual("Field Last Name is required", Browser.GetText("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[2]/span/span"));
                Assert.AreEqual("Field EID is required", Browser.GetText("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[3]/td[2]/span/span"));

                Assert.AreEqual(tableAfterAdd.Count(), tableBeforeAdd.Count());
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms*15);


            }
        }
        #endregion
    }
}
