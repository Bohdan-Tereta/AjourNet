using NUnit.Framework;
using AjourBT.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWrapper;
using AjourBT.Domain.Entities;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading;

namespace IntegrationTests.VUPage
{
         [TestFixture]
    public class VisasAndPermitsTab
    {
        #region VU_Page

        private StringBuilder verificationErrors;
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;

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
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickOnLink("Visas and Permits");
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
        public void CanLogInAsVU_VUPage_CorrectLinks()
        {
            //Arrange

            //Act

            //Assert
            Assert.IsTrue(Browser.HasElement("VU"));
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(Browser.Url, baseURL + "Home/VUView");
            Assert.AreEqual(Browser.Title, baseURL + "Home/VUView");
            Assert.AreNotEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[1]/a"), "ACC");
            Assert.AreNotEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[2]/a"), "ADM");
            Assert.AreEqual(Browser.GetText("/html/body/header/div/div[1]/nav/ul/li[1]/a"), "VU");

        }


        [Test]
        public void TestVUPageTabs_CorrectTabName()
        {
            //Arrange

            //Act

            //Assert
            Assert.IsTrue(Browser.HasElement("VU"));
            Assert.IsTrue(Browser.IsAt(baseURL + "Home/VUView"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[1]/a"), "BTs by Dates");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[2]/a"), "BTs by Quarters");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[3]/a"), "BTs in process");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[4]/a"), "Private Trips");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[5]/a"), "Calendar");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[6]/a"), "BTs by Units");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[7]/a"), "Visas and Permits");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[8]/a"), "Days From BT");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[9]/a"), "Employees");
        }

      
        #region VisasAndPermits


        [Test]
        public void VisasAndPermitsTabVU()
        {
            //Arrange

            //Act
            //Assert

            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/ul/li[7]/a"), "Visas and Permits");
            Assert.IsTrue(Browser.HasElement("Visas and Permits"));

        }


        [Test]
        public void VisasAndPermits_TableColumns()
        {
            //Arrange

            //Act
            //Assert
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[1]"), "EID");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[2]"), "Name");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[3]"), "Passport");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[4]"), "Type");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[5]"), "Visa From");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[6]"), "Visa To");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[7]"), "Entries");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[8]"), "Days");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[9]"), "Registration");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[10]"), "Num");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[11]"), "Permit From - To");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[12]"), "Last BT");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[13]"), "Status");
        }

        [Test]
        public void VisasAndPermits_TableRow()
        {
            //Arrange
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*20);
            Browser.ClickOnLink("Visas and Permits");
            //Act
            //Assert
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[1]"), "ncru");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[2]/a/b"), "Cruz Norma");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[4]"), "V_C07");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[5]/div"), new DateTime(2012, 02, 13).ToString("yyyy.MM.dd"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[6]/div"), DateTime.Now.ToLocalTimeAzure().AddDays(89).ToString("yyyy.MM.dd"));
            Assert.IsTrue(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[7]").Contains("4"));
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[8]"), "20");
            Assert.AreEqual(Browser.GetText("/html/body/div[1]/div/div[7]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]/td[9]"), DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy"));


        }

   
        [Test]
        public void ListEmployees_Count()
        {
            List<Employee> empsFromRepo = EmployeesTest.EmpsInRepositoryCount().Where(e => e.IsUserOnly == false).ToList();

            //Act
            Employee emp = empsFromRepo.FirstOrDefault();

            if (emp != null)
            {
                ReadOnlyCollection<IWebElement> empTable = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
                foreach (var element in empTable)
                {
                    if (element.Text.Contains(emp.FirstName))
                    {
                        Assert.IsTrue(element.Text.Contains(emp.LastName));
                        Assert.IsTrue(element.Text.Contains(emp.EID));
                    }
                }
                Assert.AreEqual(empsFromRepo.Count, empTable.Count());
            }
        }


        [Test]
        public void ExportToXls()
        {
            Browser.ClickByXPath("//*[@id='ExportVisasAndPermitsToXlsVU']");
        }




        #endregion

        #region SortingTable

        [Test]
        public void A_TableSorting_f_Days_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[8]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
         
                Assert.AreEqual(Browser.GetText(path), "Days");
                Assert.IsTrue(firstRow.Text.Contains("20"));
            }

        [Test]
        public void A_TableSorting_f_Days_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[8]";
            IWebElement firstRow;
                Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Days");
                Assert.IsTrue(firstRow.Text.Contains("180"));
            
            }



        [Test]
        public void A_TableSorting_h_Registration_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[9]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
        
                Assert.AreEqual(Browser.GetText(path), "Registration");
                Assert.IsTrue(firstRow.Text.Contains("01.01.2012"));
                Assert.IsTrue(firstRow.Text.Contains("Olson Wayne"));
                Assert.IsTrue(firstRow.Text.Contains("olsa"));
            }

        [Test]
        public void A_TableSorting_h_Registration_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[9]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Registration");
                Assert.IsTrue(firstRow.Text.Contains("Reid Linda"));
                Assert.IsTrue(firstRow.Text.Contains("mhan"));
                Assert.IsTrue(firstRow.Text.Contains("04.01.2016"));
          
            }

        [Test]
       public void A_TableSorting_i_Number_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[10]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
           
                Assert.AreEqual(Browser.GetText(path), "Num");
                Assert.IsTrue(firstRow.Text.Contains("01/2012"));
            }

        [Test]
        public void A_TableSorting_i_Number_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[10]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Num");
                Assert.IsTrue(firstRow.Text.Contains("mlan"));
                Assert.IsTrue(firstRow.Text.Contains("04/2015"));
                Assert.IsTrue(firstRow.Text.Contains("Rose Johnny"));
            }

        [Test]
         public void A_TableSorting_j_Permit_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[11]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual(Browser.GetText(path), "Permit From - To");
                Assert.IsTrue(firstRow.Text.Contains("01.01.2011 - 08.08.2011"));
                Assert.IsTrue(firstRow.Text.Contains("apat"));
                Assert.IsTrue(firstRow.Text.Contains("Fuller Jason")); ;
  
            }

        [Test]
        public void A_TableSorting_j_Permit_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[11]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Permit From - To");
                Assert.IsTrue(firstRow.Text.Contains("mlan"));
                Assert.IsTrue(firstRow.Text.Contains("Rose Johnny"));
                Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(100).ToString("dd.MM.yyyy") + " - " + DateTime.Now.AddDays(200).ToString("dd.MM.yyyy")));
            }

        [Test]
         public void A_TableSorting_k_LastBT_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[12]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
       
                Assert.AreEqual(Browser.GetText(path), "Last BT");
                Assert.IsTrue(firstRow.Text.Contains(""));
            }

            [Test]
         public void A_TableSorting_k_LastBT_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[12]";

            IWebElement firstRow;
                Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

                firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
      
                firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual(Browser.GetText(path), "Last BT");
            }
        [Test]
       public void A_TableSorting_l_ContactGow_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[13]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
      
                Assert.AreEqual(Browser.GetText(path), "Status");
                Assert.IsTrue(firstRow.Text.Contains("Contact Gov"));
            }

        [Test]
        public void A_TableSorting_l_ContactGow_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[13]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Status");
                Assert.IsTrue(firstRow.Text.Contains("lmor"));
                Assert.IsTrue(firstRow.Text.Contains("Moreno Lois"));
                Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(-15).ToString("dd.MM.yyyy")));

        }

        [Test]
       public void A_TableSorting_e_Entries_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[7]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
               
                Assert.AreEqual(Browser.GetText(path), "Entries");
                Assert.IsTrue(firstRow.Text.Contains("4"));
            }

        [Test]
        public void A_TableSorting_e_Entries_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[7]";

            IWebElement firstRow;
                Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

                firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
   
                Assert.AreEqual(Browser.GetText(path), "Entries");
                Assert.IsTrue(firstRow.Text.Contains("MULT"));
          
        }

        [Test]
        public void A_TableSorting_d_VisaFrom_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[5]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
           
                Assert.AreEqual(Browser.GetText(path), "Visa From");
                Assert.IsTrue(firstRow.Text.Contains("2012.02.13"));
            }


        [Test]
        public void A_TableSorting_d_VisaFrom_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[5]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Visa From");
                Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(300).ToString("yyyy.MM.dd")));
                Assert.IsTrue(firstRow.Text.Contains("mlan"));

            }

        [Test]
        public void A_TableSorting_c_Type_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[4]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[2]");

                Assert.AreEqual(Browser.GetText(path), "Type");
                Assert.IsTrue(firstRow.Text.Contains("V_C07"));
            }

        [Test]
        public void A_TableSorting_c_Type_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[4]";

            IWebElement firstRow;
                Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

                firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual(Browser.GetText(path), "Type");
                Assert.IsTrue(firstRow.Text.Contains("V_D08"));
            
        }

        [Test]
       public void A_TableSorting_b_Name_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
          
                Assert.AreEqual(Browser.GetText(path), "Name");
                Assert.IsTrue(firstRow.Text.Contains("Andrews Marie"));
                Assert.IsTrue(firstRow.Text.Contains("maad"));
            }

        [Test]
        public void A_TableSorting_b_Name_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Name");
                Assert.IsTrue(firstRow.Text.Contains("Reed Robert"));
                Assert.IsTrue(firstRow.Text.Contains("ender"));

        }

        [Test]
        public void A_TableSorting_a_EID_ascending()
        {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
  
                Assert.AreEqual(Browser.GetText(path), "EID");
                Assert.IsTrue(firstRow.Text.Contains("Lee Raymond"));
                Assert.IsTrue(firstRow.Text.Contains("aada"));
            }

        [Test]
        public void A_TableSorting_a_EID_descending()
            {
            string path = "//*[@id='visasViewVU_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]";

            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[7]/div[2]/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "EID");
                Assert.IsTrue(firstRow.Text.Contains("Simmons Teresa"));
                Assert.IsTrue(firstRow.Text.Contains("wsim"));
            }

        #endregion

        #endregion
    }
}
