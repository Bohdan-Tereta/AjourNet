using AjourBT.Domain.Abstract;
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

namespace IntegrationTests.ADMPage
{
    [TestFixture]
    class BTsTabTests
    {
        #region BTs Table

        private StringBuilder verificationErrors;
        private string baseURL;
        private string username;
        private string password;
        AjourBTForTestContext db = new AjourBTForTestContext();

        [TestFixtureSetUp]
        public void SetupTest()
        {
            //Arrange
            baseURL = "http://localhost:50616/";
            username = "lgon";
            password = "corsad";
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
            Browser.Wait(5);
           
            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("/html/body/div[1]/section/section/form/fieldset/div/input");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("ADM");
            Browser.ClickOnLink("BTs");
            verificationErrors = new StringBuilder();

            Assert.That(Browser.IsAt(baseURL + "Home/ADMView"));
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }        


        public int BtsFromRepositoryCount()
        {
            using (AjourBTForTestContext db = new AjourBTForTestContext())
            {
                int bts = (from bt in db.BusinessTrips.AsEnumerable()
                           select bt).Count();
                return bts;
            }
        }

        [Test]
        public void BTs_BTsTab()
        {
            //Arrange

            //Act
            //Assert
            Assert.NotNull(Browser.HasElement("ADM"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[2]/a"), "BTs");
        }

        [Test]
        [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", true)]
        [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", true)]
        [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[1]", "EID", false)]
        [TestCase("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[1]/div/table/thead/tr/th[2]", "Name", false)]
        public void BTs_SortingTable(string path, string column, bool ascending)
        {
            //Arrange
            //Act
            Browser.Refresh();
            Browser.ClickOnLink("BTs");
            Browser.ClickByXPath(path);
            if (!ascending)
                Browser.ClickByXPath(path);
            var firstEmploeeID = Browser.GetText("//tr[contains(@class,'zebra')]", 0);
            var firstEmploeeName = Browser.GetText("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[1]/td[2]/a/u");

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

            Browser.Refresh();
            Browser.ClickOnLink("BTs");
        }

        [Test]
        public void BTs_VisaAndPermitsInformationInHints()
        {
            //Arrange
            string expected = "No Visa\r\nNo Permit";

            //Act
            int employees = Browser.Count("//tr[contains(@class,'zebra')]");
            List<string> hints = new List<string>();
            string hint;
            for (int count = 0; count < employees; count++)
            {
                hint = Browser.IWebElementToString("//tr[contains(@class,'zebra')]", count, "a", "title");
                hints.Add(hint);
            }
           
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.AreEqual(expected, hints[2]);
            Assert.IsTrue(employees == hints.Count());
        }

        #endregion

        #region Plan

        [Test]
        public void BTs_PlanButton()
        {
            //Arrange
            int employees = Browser.Count("//tr[contains(@class,'zebra')]");
            int buttons = 0;
            for (int count = 0; count < employees; count++)
            {
                if (Browser.HasElement("Plan"))
                {
                    Console.WriteLine(count);
                    buttons++;
                }
                else
                {
                    string btnText = Browser.IWebElementToString("//a[contains(@class,'ui-button')]", count, "id");
                    Console.WriteLine(btnText);
                }
            }

            Assert.IsTrue(employees == buttons);
        }


        [Test]
        public void BTs_ValidInput_CanPlanBT()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);
            int btsInRepoCount = BtsFromRepositoryCount();
           
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 6);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("25", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("27", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("planPurpose", "meeting", true);
            Browser.SendKeysTo("Manager", "m", true);
            Browser.SendKeysTo("Responsible", "resp", true);
            Browser.SendKeysTo("Comment", "meeting", true);
            Browser.ClickByXPath("//button[@id='btnPlanBT']");
            Thread.Sleep(Timings.Default_ms*20);

            int btsCountAfterPlan = BtsFromRepositoryCount();
         
            //Assert
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT)));
            Assert.AreEqual(btsInRepoCount + 1, btsCountAfterPlan);
        }


        [Test]
        public void BTs_WithSameDatesExists_ValidationErrors()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);
            string message = "BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.";
            int btsInRepoCount = BtsFromRepositoryCount();

            //Act
            Console.WriteLine("BTs_WithSameDatesExists_ValidationErrors");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 6);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("25", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("27", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("planPurpose", "meeting", true);
            Browser.SendKeysTo("Manager", "m", true);
            Browser.SendKeysTo("Responsible", "resp", true);
            Browser.SendKeysTo("Comment", "meeting", true);
            Browser.ClickByXPath("//*[@id='btnPlanBT']/span");
            int btsCountAfterPlan = BtsFromRepositoryCount();


            //Assert
            Assert.AreEqual(message, Browser.GetText("//div[@id='ModelError']"));
            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
            Thread.Sleep(Timings.Default_ms*20);
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT)));
            Thread.Sleep(Timings.Default_ms*20);
            Assert.AreEqual(btsInRepoCount, btsCountAfterPlan);
        }

        [Test]
        public void BTs_WrongInput_ValidationErrors()
        {
            //Arrange
            string messageForLocation = "The Location field is required.";
            string messageForUnit = "The Unit field is required.";
            string messageForDates = "From Date is greater than To Date";
            string mounthOfPlannedBT = Browser.SelectedMonth(4);
            string secondMonthOfPlannedBT = Browser.SelectedMonth(3);
            int btsInRepoCount = BtsFromRepositoryCount();


            //Act
            Thread.Sleep(Timings.Default_ms*30);
            Browser.ClickOnLink("Plan", 6);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("25", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("15", 6);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("planPurpose", "meeting", true);
            Browser.SendKeysTo("Manager", "m", true);
            Browser.SendKeysTo("Responsible", "resp", true);
            Browser.SendKeysTo("Comment", "meeting", true);
            Browser.ClickByXPath("//*[@id='btnPlanBT']/span");
            int btsCountAfterPlan = BtsFromRepositoryCount();

            //Assert
            Assert.AreEqual(messageForLocation, Browser.GetText("//*[@id='planBTForm']/fieldset/table/tbody/tr[5]/td[2]/span/span"));
            Assert.AreEqual(messageForUnit, Browser.GetText("//*[@id='planBTForm']/fieldset/table/tbody/tr[6]/td[2]/span/span"));
            Assert.AreEqual(messageForDates, Browser.GetText("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/span/span"));
            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
            Thread.Sleep(Timings.Default_ms*20);
            Assert.IsFalse(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-15." + secondMonthOfPlannedBT)));
            Assert.AreEqual(btsInRepoCount, btsCountAfterPlan);
        }
        #endregion

        #region Register, Confirm, Replan, Cancel butttons
        [Test]
        public void BTsValidInput_CanRegisterBT()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);

            //Act
            Browser.HasElement("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[7]/td[4]/input");
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[7]/td[4]/input");
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/table/tbody/tr/td[4]/div/button[1]");

            Thread.Sleep(Timings.Default_ms*20);
          
          
            //Assert
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT)));
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[7]/td[5]/a"), String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT));

        }

        [Test]
        public void BTsValidInput_CanReplanBT()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);

            //Act
            Browser.HasElementByXPath("//input[@id='selectedRegisteredBTs']");
            Browser.wait(5);
            Browser.ClickByXPath("//input[@id='selectedRegisteredBTs']");
            Browser.ClickByXPath("//button[@id='replanRegisterBt']");
            Thread.Sleep(Timings.Default_ms*20);

           
            //Assert
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT)));
           // Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[7]/td[4]/a"), String.Format("AT/BW:25."+ mounthOfPlannedBT + "-27." + mounthOfPlannedBT));
            
        }
     

        [Test]
        public void BTsValidInput_ConfirmRegisteredBT()
        {

            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);

            //Act
            Browser.HasElement("//*[@id='selectedPlannedBTs']");
            Browser.ClickByXPath("//*[@id='selectedPlannedBTs']");
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/table/tbody/tr/td[4]/div/button[2]");
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:25." + mounthOfPlannedBT + "-27." + mounthOfPlannedBT)));
            //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[7]/td[6]/a"), String.Format("AT/BW:25."+ mounthOfPlannedBT + "-27." + mounthOfPlannedBT));
            

        }

        [Test]
        public void BTsValidInput_ConfirmPlannedBT()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);

            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 7);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("15", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("17", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("planPurpose", "meeting", true);
            Browser.SendKeysTo("Manager", "m", true);
            Browser.SendKeysTo("Responsible", "resp", true);
            Browser.SendKeysTo("Comment", "meeting", true);
            Browser.ClickByXPath("//*[@id='btnPlanBT']/span");
            Browser.HasElement("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[8]/td[4]/input");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='selectedPlannedBTs']");
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/table/tbody/tr/td[4]/div/button[2]");
            Thread.Sleep(Timings.Default_ms*20);
            //Assert
       
            Assert.IsTrue(Browser.HasElement(String.Format("AT/BW:15." + mounthOfPlannedBT + "-17." + mounthOfPlannedBT)));
          //Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[8]/td[6]/a"), String.Format("AT/BW:15." + mounthOfPlannedBT + "-17." + mounthOfPlannedBT));
         
        }

        [Test]
        public void BTsValidInput_CanCancelRegisteredBT()
        {
            //Arrange
            string mounthOfPlannedBT = Browser.SelectedMonth(4);
            int btsCount = BtsFromRepositoryCount();

            //Act
            Browser.Refresh();
            Browser.ClickOnLink("BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 8);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("16", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("17", 7);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeysTo("planPurpose", "meeting", true);
            Browser.SendKeysTo("Manager", "m", true);
            Browser.SendKeysTo("Responsible", "resp", true);
            Browser.SendKeysTo("Comment", "meeting", true);
            Browser.ClickByXPath("//*[@id='btnPlanBT']/span");
            Thread.Sleep(Timings.Default_ms*20);
            int btsCountAfterPlan = BtsFromRepositoryCount();
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*25);
            Browser.ClickOnLink("BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Browser.HasElement("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[9]/td[4]/input");
            Thread.Sleep(Timings.Default_ms*30);
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[9]/td[4]/input");
            Thread.Sleep(Timings.Default_ms*25);
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/table/tbody/tr/td[4]/div/button[1]");
            Thread.Sleep(Timings.Default_ms*25);
            Browser.Refresh();
            Browser.ClickOnLink("BTs");
            Thread.Sleep(Timings.Default_ms*25);
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/div/div[1]/div[2]/table/tbody/tr[9]/td[5]/input");
            Thread.Sleep(Timings.Default_ms*25);
            Browser.ClickByXPath("/html/body/div[1]/div/div[2]/div/div/table/tbody/tr/td[5]/div/button[3]");
            Thread.Sleep(Timings.Default_ms*25);
            int btsCountAfterCancel = BtsFromRepositoryCount();

            //Assert
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("BTs");
            Thread.Sleep(Timings.Default_ms*20);
            Assert.IsFalse(Browser.HasElement(String.Format("AT/BW:16." + mounthOfPlannedBT + "-17." + mounthOfPlannedBT)));
            Assert.AreEqual(btsCount + 1, btsCountAfterPlan);
            Assert.AreEqual(btsCountAfterPlan, btsCountAfterCancel);
        }
        #endregion

        #region Calculate Visa Days

        [Test]
        public void BTs_PlanButton_NumberOfDays()
        {
            //Arrange
         
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 1);
         
            //Assert
            Assert.IsTrue(Browser.HasElementByXPath("//td[@id='numberOfDays']"));
            Assert.IsTrue(Browser.HasElementByXPath("//button[@id='btnCalculateDays']"));

             IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
             Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void BTs_PlanButton_Visa()
        {
            //Arrange
        
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 1);
            
            //Assert
            Assert.IsTrue(Browser.HasElementByXPath("//div[@title='Visa expires today']"));
            Assert.IsTrue(Browser.HasElementByXPath("//button[@id='btnCalculateDays']"));

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void BTs_PlanButton_CalculateWithSelectedDays()
        {
            //Arrange
         
            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 1);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[7]/td[2]/img");
            Browser.SelectDatePickerValue("6", 0);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickByXPath("//*[@id='planBTForm']/fieldset/table/tbody/tr[8]/td[2]/img");
            Browser.SelectDatePickerValue("7", 0);
            Thread.Sleep(Timings.Default_ms*20);


            Browser.ClickByXPath("//*[@id='btnCalculateDays']/span");
            Thread.Sleep(Timings.Default_ms*20);

            //Assert
            Assert.AreEqual(Browser.GetText("//td[@id='numberOfDays']"), "7");
            Assert.AreEqual(Browser.GetText("//td[@id='visaDays']"), "180");

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
        }

        [Test]
        public void BTs_PlanButton_CalculateWithoutSelectedDates()
        {
            //Arrange

            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Plan", 1);
            Browser.SelectOption("LocationID", 1);
            Browser.SelectOption("UnitID", 1);
            Browser.ClickByXPath("//*[@id='btnCalculateDays']/span");
            Thread.Sleep(Timings.Default_ms * 10);

            //Assert
            Assert.AreEqual(Browser.GetText("//td[@id='numberOfDays']"), "5");
            Assert.AreEqual(Browser.GetText("//td[@id='visaDays']"), "180");

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
        }

        #endregion

         [Test]
        public void BTs_ShowBtData()
        {
            //Arrange

            //Act
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("LO/JA:01.12-04.12");
            Thread.Sleep(Timings.Default_ms*10);

            //Assert
            Assert.AreEqual("Robert Knight (rkni)", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/h4"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("From", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[2]/td[1]/label"));
            Assert.AreEqual("To", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[3]/td[1]/label"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("01.12.2014", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[2]/td[2]"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("04.12.2014", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[3]/td[2]"));
            Assert.AreEqual("Order From", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[2]/td[3]/b"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("Number Of Days", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[1]/tbody/tr[4]/td[3]/b"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("Purpose", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[2]/tbody/tr[1]/td[1]/label"));
            Assert.AreEqual("meeting", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[2]/tbody/tr[1]/td[2]"));
            Thread.Sleep(Timings.Default_ms*10);
            Assert.AreEqual("Manager", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[2]/tbody/tr[2]/td[1]/label"));
            Assert.AreEqual("Responsible", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[2]/tbody/tr[3]/td[1]/label"));
            Assert.AreEqual("Comment", Browser.GetText("//*[@id='ShowBTData-ADM']/fieldset/table[2]/tbody/tr[4]/td[1]/label"));
            Thread.Sleep(Timings.Default_ms*10);

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
        }


         [Test]
         public void BTs_AllDepts_TableData()
         {
             int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
             Thread.Sleep(Timings.Default_ms*40);
             Browser.SelectOption("selectedDepartment", "All Departments");
             Thread.Sleep(Timings.Default_ms*20);
             int year = DateTime.Now.ToLocalTimeAzure().Year;
             List<Employee> employees = (from e in db.Employees.AsEnumerable()
                                         where e.DateDismissed == null && e.IsUserOnly == false
                                         orderby e.IsManager, e.LastName
                                         select e).ToList();
             ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//*[@id='tbodyBTADM']/tr");

             Employee emp = employees.FirstOrDefault();
             if (emp != null)
             {
                 foreach (var element in empTable)
                 {
                     if (element.Text.Contains(emp.EID))
                     {
                         Assert.IsTrue(element.Text.Contains(emp.EID));
                         Assert.IsTrue(element.Text.Contains(emp.LastName));
                     }
                 }
             }

             Browser.Refresh();
             Browser.ClickOnLink("BTs");
         }
             
         [Test]
         public void TestEditPlanedBT()
         {
             //Arrange

             //Act
             Thread.Sleep(Timings.Default_ms*20);
             Browser.ClickByXPath("//*[@id='EditPlannedBT']");

             //Assert
             Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-EditPlanedBT']"), "Edit Planned BT");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[1]/td[1]/label"), "Visa");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[2]/td/label"), "Days Used");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[5]/td[1]/label"), "Location");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[6]/td[1]/label"), "Unit");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[7]/td[1]/label"), "From");
             Assert.AreEqual(Browser.GetText("//*[@id='editPlanedBTForm']/fieldset/table[1]/tbody/tr[8]/td[1]/label"), "To");
             Assert.AreEqual(Browser.GetText("//*[@id='planEditedBT']/span"), "Plan");
             Assert.AreEqual(Browser.GetText("//*[@id='btnDeletePlannedBT']/span/span"), "Delete");
             Browser.ClickByXPath("//*[@id='planEditedBT']");
        }

         [Test]
         public void TestsDeletePlanedBT()
         {
             //Arrange

             //Act
             Thread.Sleep(Timings.Default_ms*20);
             Browser.ClickByXPath("//a[@id='EditPlannedBT']");
             Browser.ClickByXPath("//button[@id='btnDeletePlannedBT']");

             //Assert
             Assert.AreEqual(Browser.GetText("//*[@id='ui-dialog-title-DeletePlannedBT-ADM']"), "Delete Planned BT");
             Assert.AreEqual(Browser.GetText("//*[@id='DeletePlannedBT-ADM']/h4[2]"), "Details of BT:");
             Assert.AreEqual(Browser.GetText("//*[@id='deleteBTADMConfirmation']/span/span"), "Delete");
             Browser.ClickByXPath("//button[@id='deleteBTADMConfirmation']");
             Thread.Sleep(Timings.Default_ms*20);
         }
    }
}
