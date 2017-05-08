using IntegrationTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Tests.IntegrationTests
{
    [TestFixture]
    public class IntegrationTestsForADM
    {
        
        #region LogInPage

        public static string baseURL = "http://localhost:50616/";
        private StringBuilder verificationErrors;

        [SetUp]
        public void SetupTest()
        {
            Browser.Goto(baseURL);
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        [Test]
        public void Can_Goto_LogInPage()
        {
            //Arrange

            //Act

            //Assert
            Assert.IsTrue(Browser.IsAt("Log in"));
            Assert.IsTrue(Browser.Url == baseURL);
            Assert.AreEqual(Browser.Title, "Log in");
            Assert.AreEqual(Browser.GetText("/html/body/header/div/div[2]/section/ul/li/a").ToString(), "Log in");
        }


        [Test]
        public void Can_Goto_HelpLink()
        {
            //Arrange

            //Act
            Browser.ClickOnLink("Help");

            //Assert
            Assert.That(Browser.Url, Is.EqualTo(baseURL + "Help/Map"));
            Assert.AreEqual(Browser.Title, baseURL + "Help/Map");

        }



        [Test]
        [TestCase("", "", false)]
        [TestCase("rkni", "feharo", true)]
        public void UserNameAndPasswordAreTypedIn_SignUpTest(string username, string password, bool isAccepted)
        {
            //Arrange
            Browser.Goto(baseURL);

            //Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");

            //Assert
            if (isAccepted)
            {
                Assert.That(Browser.IsAt(baseURL + "Home/VUView"));

            }
            else
            {
                Assert.That(Browser.Url, Is.EqualTo(baseURL));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/div/ul/li"), Is.EqualTo("The user name or password provided is incorrect."));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[1]/span"), Is.EqualTo("The User name field is required."));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[2]/span"), Is.EqualTo("The Password field is required."));
                Assert.AreEqual(Browser.Title, "Log in");
            }
        }

        [Test]
        [TestCase("rkni", "123456", true, false)]
        [TestCase("abcd", "feharo", false, true)]
        [TestCase("abcd", "123456", false, false)]
        [TestCase("rkni", "feharo", true, true)]
        public void IncorrectUserNameAndPassword_CannotSignUpTest(string username, string password, bool UserNameAccepted, bool PasswordAccepted)
        {
            //Arrange
            //Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");


            //Assert
            if (UserNameAccepted && PasswordAccepted)
            {
                Assert.That(Browser.IsAt(baseURL + "Home/VUView"));
            }
            else
            {
                Assert.That(Browser.Url, Is.EqualTo(baseURL));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/div/ul/li"), Is.EqualTo("The user name or password provided is incorrect."));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[1]/span"), Is.EqualTo(""));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[2]/span"), Is.EqualTo(""));
                Assert.AreEqual(Browser.Title, "Log in");
            }
        }
        #endregion


        #region ADM_Page
      
         
        [TestFixture]
        class ADMRoleTests
        {
            private StringBuilder verificationErrors;
            private string baseURL = "http://localhost:50616/";
            private string username;
            private string password;
            //private string nothingFound = "No matching records found";


            [TestFixtureSetUp]
            public void SetupTest()
            {
                //Arrange
                username = "rkni";
                password = "feharo";
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
                try
                {
                    Browser.Quit();
                }
                catch (Exception)
                {
                    // Ignore errors if unable to close the browser
                }
                Assert.AreEqual("", verificationErrors.ToString());
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
                Assert.AreEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[1]/a"), "ACC");
                Assert.AreEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[2]/a"), "ADM");
                Assert.AreEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[3]/a"), "BTM");

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

            [Test]
            public void ADMPage_DropdownListOfDepartments_DefaultDepartment()
            {
                //Arrange
                string expected = "DEPT4";
                //Act
                Browser.Goto(baseURL + "Home/ADMView");
                Browser.SelectOption("departmentName", 5);

                int employees = Browser.Count("//tr[contains(@class,'indexBT')]");

                //Assert
                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[6]"));
                Assert.AreEqual(12, employees);
            }


            [Test]
            public void ADMPage_DropdownListOfDepartments_AllDepartments()
            {
                //Arrange
                string expected = "All Departments";
                //Act
                Browser.Goto(baseURL + "Home/ADMView");
                Browser.SelectOption("departmentName", 0);
                int employees = Browser.Count("//tr[contains(@class,'indexBT')]");

                //Assert

                Assert.IsTrue(Browser.IsAt(baseURL + "Home/ADMView"));
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[1]"));
                Assert.AreEqual(12, employees);
            }

            [Test]
            public void ADMPage_DropdownListOfDepartments_SelectedDepartment()
            {
                //Arrange
                string expected = "BOARD";
                //Act
                Browser.SelectOption("departmentName", "BOARD");

                //Assert
                Assert.AreEqual(expected, Browser.GetText("/html/body/div[1]/div/div[1]/form/div/select/option[2]"));
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
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
            [TestCase("/html/body/div[1]/div/div[1]/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
            public void VisasAndPermits_SortingTable(string path, string column, bool ascending)
            {
                //Arrange

                //Act
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

            #region BTs

            //       [Test]
            //       public void BTs_Tab()
            //       {
            //           //Arrange
            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           //driver.FindElement(By.XPath("/html/body/div[1]/div/ul/li[2]/a")).Click();
            //           //Act
            //           //Assert
            //           Assert.NotNull(driver.FindElement(By.Id("tabsADM")));
            //           Assert.AreEqual(driver.FindElement(By.XPath("/html/body/div[1]/div/ul/li[2]/a")).Text, "BTs");
            //           Assert.NotNull(driver.FindElement(By.Id("tableBodyBusinessTrip")));

            //       }

            //       [Test]
            //       [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
            //       [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
            //       [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
            //       [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
            //       public void BTs_SortingTable(string path, string column, bool ascending)
            //       {
            //           //Arrange
            //           SetupTest();
            //           //Act
            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           driver.FindElement(By.XPath(path)).Click();
            //           if (!ascending)
            //               driver.FindElement(By.XPath(path)).Click();

            //           var firstEmploeeID = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"))[0];
            //           var firstEmploeeName = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"))[0].FindElements(By.TagName("td"))[1];

            //           //Assert
            //           if (column == "EID")
            //               if (ascending)
            //                   Assert.IsTrue(firstEmploeeID.Text.StartsWith("a"));
            //               else
            //                   Assert.IsFalse(firstEmploeeID.Text.StartsWith("a"));
            //           else
            //               if (ascending)
            //                   Assert.IsTrue(firstEmploeeName.Text.StartsWith("A"));
            //               else
            //                   Assert.IsFalse(firstEmploeeName.Text.StartsWith("A"));
            //       }

            //       [Test]
            //       public void BTs_VisaAndPermitsInformationInHints()
            //       {
            //           //Arrange
            //           string expected = "V: 05.06.2014 - 12.12.2014\r\nKarta Polaka";

            //           //Act
            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           var employees = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"));
            //           string[] hints = new string[employees.Count];
            //           for (int count = 0; count < employees.Count; count++)
            //           {
            //               IWebElement hint = employees[count].FindElement(By.TagName("a"));
            //               hints[count] = hint.GetAttribute("title");
            //           }

            //           //Assret
            //           Assert.AreEqual(expected, hints[0]);
            //       }

            //       public static void SelectDatePickerValue(IWebDriver driver, string date, int nextMounth)
            //       {

            //         if (nextMounth > 0)
            //         {
            //             for (int i = 0; i < nextMounth; i++)
            //             {
            //                 IWebElement next = driver.FindElement(By.XPath("//a[@data-handler='next']"));

            //             next.Click();
            //             i++;
            //             }
            //         }
            //         driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

            //         IWebElement dateWidget = driver.FindElement(By.Id("ui-datepicker-div"));

            //         List<IWebElement> rows = dateWidget.FindElements(By.TagName("tr")).ToList();  
            //         List<IWebElement> columns = dateWidget.FindElements(By.TagName("td")).ToList();  

            //         foreach (IWebElement cell in  columns)
            //         {  
            //          if (cell.Text ==date)
            //          {  
            //          cell.FindElement(By.LinkText(date)).Click();
            //          driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
            //          break;  
            //          }  
            //         }   
            //}  


            //       [Test]
            //       public void BTs_ValidInput_CanPlanBT()
            //       {
            //           //Arrange

            //           //Act
            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           var employees = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"));

            //           IWebElement e = employees[0].FindElement(By.Id("PlanForAdm"));
            //           e.Click();   
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            //               driver.FindElement(By.TagName("form"));

            //               var selectlist = driver.FindElement(By.Id("LocationID"));
            //               selectlist.FindElements(By.TagName("option"))[1].Click();
            //               selectlist = driver.FindElement(By.Id("UnitID"));
            //               selectlist.FindElements(By.TagName("option"))[1].Click();

            //               driver.FindElements(By.ClassName("ui-datepicker-trigger"))[0].Click();


            //               SelectDatePickerValue(driver, "25", 7);
            //               driver.FindElements(By.TagName("img"))[1].Click();
            //               driver.FindElements(By.ClassName("ui-datepicker-trigger"))[1].Click();
            //               driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));


            //               SelectDatePickerValue(driver, "27", 7);

            //               driver.FindElement(By.Id("planPurpose")).SendKeys("meeting");
            //               driver.FindElement(By.Id("Manager")).SendKeys("m");
            //               driver.FindElement(By.Id("Responsible")).SendKeys("resp");
            //               driver.FindElement(By.Id("Comment")).SendKeys("meeting");
            //               driver.FindElement(By.XPath("//button[@value='Save']")).Click();
            //               driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            //           var link = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[4]/a"));

            //           //Assert
            //           Assert.IsTrue(link.Displayed);
            //           Assert.IsTrue(link.GetAttribute("id") == "EditPlannedBT");
            //           Assert.IsTrue(link.FindElement(By.Id("selectedPlannedBTs")).GetAttribute("type") == "chekbox");
            //       }


            //       [Test]
            //       public void BTs_PlanSameBt_ValidationErrors()
            //       {
            //           //Arrange

            //           //Act

            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           var employees = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"));

            //           IWebElement e = employees[0].FindElement(By.Id("PlanForAdm"));
            //           e.Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            //           driver.FindElement(By.TagName("form"));

            //           var selectlist = driver.FindElement(By.Id("LocationID"));
            //           selectlist.FindElements(By.TagName("option"))[1].Click();
            //           selectlist = driver.FindElement(By.Id("UnitID"));
            //           selectlist.FindElements(By.TagName("option"))[1].Click();

            //           driver.FindElements(By.ClassName("ui-datepicker-trigger"))[0].Click();


            //           SelectDatePickerValue(driver, "25", 7);
            //           driver.FindElements(By.TagName("img"))[1].Click();
            //           driver.FindElements(By.ClassName("ui-datepicker-trigger"))[1].Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));


            //           SelectDatePickerValue(driver, "27", 7);

            //           driver.FindElement(By.Id("planPurpose")).SendKeys("meeting");
            //           driver.FindElement(By.Id("Manager")).SendKeys("m");
            //           driver.FindElement(By.Id("Responsible")).SendKeys("resp");
            //           driver.FindElement(By.Id("Comment")).SendKeys("meeting");
            //           driver.FindElement(By.XPath("//button[@value='Save']")).Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            //           var link = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[4]/a"));

            //           //Assert
            //           Assert.IsFalse(link.Displayed);

            //       }


            //       [Test]
            //       public void BTs_InvalidInput_ValidationErrors()
            //       {
            //           //Arrange

            //           //Act
            //           driver.FindElement(By.LinkText("BTs")).Click();
            //           var employees = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"));

            //           IWebElement e = employees[0].FindElement(By.Id("PlanForAdm"));
            //           e.Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            //           driver.FindElement(By.TagName("form"));

            //           //var selectlist = driver.FindElement(By.Id("LocationID"));
            //           //selectlist.FindElements(By.TagName("option"))[1].Click();
            //           //selectlist = driver.FindElement(By.Id("UnitID"));
            //           //selectlist.FindElements(By.TagName("option"))[1].Click();

            //           driver.FindElements(By.ClassName("ui-datepicker-trigger"))[0].Click();


            //           SelectDatePickerValue(driver, "25", 7);
            //           driver.FindElements(By.TagName("img"))[1].Click();
            //           driver.FindElements(By.ClassName("ui-datepicker-trigger"))[1].Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));


            //           SelectDatePickerValue(driver, "15", 6);

            //           driver.FindElement(By.Id("planPurpose")).SendKeys("meeting");
            //           driver.FindElement(By.Id("Manager")).SendKeys("m");
            //           driver.FindElement(By.Id("Responsible")).SendKeys("resp");
            //           driver.FindElement(By.Id("Comment")).SendKeys("meeting");
            //           driver.FindElement(By.XPath("//button[@value='Save']")).Click();
            //           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            //           //Assert
            //           Assert.IsTrue(driver.FindElement(By.XPath("//span [@for='LocationID']")).Displayed);
            //           Assert.IsTrue(driver.FindElement(By.XPath("//span [@for='UnitID']")).Displayed);
            //           Assert.IsTrue(driver.FindElement(By.XPath("//span [@for='planEndDateBTs']")).Displayed);

            //       }

            //       [Test]
            //       public void BTs_PlanButton()
            //       {
            //           //Arrange

            //          var employees = driver.FindElements(By.XPath("//tr[contains(@class,'zebra')]"));

            //          for (int count = 0; count < employees.Count; count++)
            //           {
            //               try
            //               {
            //                   IWebElement plan = employees[count].FindElement(By.Id("PlanForAdm"));
            //                  Console.WriteLine(plan.Text);
            //               }
            //               catch (NoSuchElementException)
            //               {
            //                  IWebElement plan = employees[count].FindElement(By.XPath("//a[contains(@class,'ui-button')]"));
            //                   string btnText = plan.Text;
            //                   Console.WriteLine(btnText +' '+ employees[count].GetAttribute("id"));
            //              }                
            //          }
            //      }



            #endregion

            //[Test]
            //[TestCase("not existsting", false)]
            //[TestCase("rkni", true)]
            //public void SearchString(string serchString, bool isFound)
            //{
            //    driver.FindElement(By.XPath("//input[@type='search']")).SendKeys(serchString);
            //    driver.FindElement(By.XPath("//input[@type='search']")).SendKeys(Keys.Enter);

            //    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            //    //WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 3));
            //    //wait.Until(e => e.FindElement(By.ClassName("dataTables_empty")));
            //    if (isFound)
            //    {
            //        Assert.IsTrue(driver.FindElement(By.XPath("//tr[contains(@class,'zebra ')]")).Text.Contains("rkni"));

            //    }
            //    else
            //    {
            //        Assert.IsTrue(driver.FindElement(By.ClassName("dataTables_empty")).Text.Contains(nothingFound));
            //    }
            //    driver.Navigate().GoToUrl(baseURL);

            //}

            #endregion

        }
    }
}



