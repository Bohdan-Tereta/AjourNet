using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Threading;
using OpenQA.Selenium.Chrome;
using TestWrapper;
using AjourBT.Domain.Entities;

namespace IntegrationTests.BTMPage
{
    [TestFixture]
    public class VisasAndPermitsTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private ReadOnlyCollection<IWebElement> empList;
        private string searchResultFail = "No matching records found";



        [TestFixtureSetUp]
        public void Login()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "wsim";
            password = "1234rt";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("BTM");

            Assert.That(Browser.IsAt(baseURL + "Home/BTMView"));
            Browser.ClickOnLink("Visas and Permits");

            string VisasAndPermits = Browser.FindElementByLinkText("Visas and Permits").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", VisasAndPermits);
        }

        #region Visas And Permit Tab

        [Test]
        public void A_tSearchStringField_SearchStringEmpty()
        {
            IWebElement search = Browser.FindElementByXPath("//input[@type='search']");
            Browser.SendKeysTo(search, "Bad text 42", false);
            Browser.SendEnter(search);

            Thread.Sleep(Timings.Default_ms*20);
            Assert.IsTrue(Browser.FindElementByClassName("dataTables_empty").Text.Contains(searchResultFail));

            Browser.SendKeysTo(search, Keys.Enter, true);
        }

        [Test]
        public void PassportCheck_TestShouldPass()
        {
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]/td/input[@id='HasPassport'][@checked='checked']");
            List<Employee> empsWithPassport = new List<Employee>();

            using (var db = new AjourBTForTestContext())
            {
                empsWithPassport = db.Employees.Where(e => e.DateDismissed == null && e.Passport != null).ToList();
            }

            Assert.AreEqual(empList.Count, empsWithPassport.Count);

            foreach (IWebElement elem in empList)
            {
                foreach (Employee emp in empsWithPassport)
                {
                    if (elem.Text.Contains(emp.EID))
                    {
                        Assert.IsTrue(elem.Text.Contains(emp.Passport.EndDate.ToString()));
                    }
                }
            }
        }

        [Test]
        public void VisaRegCheck()
        {
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]/a[@class='visaEditDialog']");


        }

        //[Test]
        //public void PermitFromTo()
        //{
        //    empList = firefoxDriver.FindElements(By.XPath("//tr[contains(@class,'indexBT')]"));

        //    for (int count = 0; count < empList.Count; count++)
        //    {
        //        try
        //        {
        //            IWebElement permitBtn = empList[count].FindElement(By.Id("CreatePermit"));
        //            Console.WriteLine(permitBtn.Text + ' ' + empList[count].GetAttribute("id"));
        //        }
        //        catch (NoSuchElementException)
        //        {
        //            Console.WriteLine("user have permit");
        //        }
        //    }
        //}

        //[Test]
        //public void IsKartaPolaka()
        //{
        //    empList = firefoxDriver.FindElements(By.XPath("//tr[contains(@class,'indexBT')]"));

        //    for (int count = 0; count < empList.Count; count++)
        //    {
        //        try
        //        {
        //            IWebElement isKarta = empList[count].FindElement(By.ClassName("permitEditDialog"));
        //            Console.WriteLine(isKarta.Text + ' ' + empList[count].GetAttribute("id"));
        //        }
        //        catch (NoSuchElementException)
        //        {
        //            Console.WriteLine("user havent permit");
        //        }
        //    }
        //}

        //[Test]
        //public void LastBT()
        //{
        //    empList = firefoxDriver.FindElements(By.XPath("//tr[contains(@class,'indexBT')]"));

        //    for (int count = 0; count < empList.Count; count++)
        //    {
        //        try
        //        {
        //            IWebElement lastBtDates = empList[count].FindElement(By.Id("ShowBTDataADM"));
        //            Console.WriteLine(lastBtDates.Text + ' ' + empList[count].GetAttribute("id"));
        //        }
        //        catch (NoSuchElementException)
        //        {
        //            Console.WriteLine("user havent lastBt");
        //        }
        //    }
        //}

        #region Passport

        [Test]
        public void A_tPassportAddBtn()
        {
            int beforeAddPassport = 0;
            int afterAddPassport = 0;
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms*30);
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement addPassportButton = empList[0].FindElement(By.XPath("//a[@id='AddDatePassport'][@role='button']"));
            Browser.ClickOnWebElement(addPassportButton);

            //fill ToDate field
            Thread.Sleep(Timings.Default_ms*20);
            //text input is readonly so need to simulate datepicker
            Browser.ClickOnDatePicker();
           
            Thread.Sleep(Timings.Default_ms*15);
            IWebElement buttonSave = Browser.FindElementByXPath("//button[@id='btnSavePassport'][@type='submit']");

            using (var db = new AjourBTForTestContext())
            {
                beforeAddPassport = db.Passports.Count(p => p.EndDate != null);
                Browser.ClickOnWebElement(buttonSave);
                //check changes
                Thread.Sleep(Timings.Default_ms*20);
                afterAddPassport = db.Passports.Count(p => p.EndDate != null);
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            Thread.Sleep(Timings.Default_ms*20);
            IWebElement isPassportDateAppear = empList[1];
            string dateToCompare = Tools.TodayDate;

            Assert.Less(beforeAddPassport, afterAddPassport);
            Assert.IsTrue(isPassportDateAppear.Text.Contains(dateToCompare));
        }

        [Test]
        public void A_UpdatePassport()
        {
         
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement updPassportLink = empList[0].FindElement(By.XPath("//a[@id='passportEdit']"));
            Browser.ClickOnWebElement(updPassportLink);

            //fill ToDate field
            Thread.Sleep(Timings.Default_ms*20);

            //text input is readonly so need to simulate datepicker
            IWebElement datepickerImg = Browser.FindElementByXPath("//img[@class='ui-datepicker-trigger']");
            Browser.ClickOnWebElement(datepickerImg);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.SelectDatePickerValue(DateTime.Now.Day.ToString(), 1);
            //find save button and click
            Thread.Sleep(Timings.Default_ms*15);
            IWebElement buttonSave = Browser.FindElementByXPath("//button[@id='btnSaveEditedPassport'][@type='submit']");
            Browser.ClickOnWebElement(buttonSave);
            //search for changes
            Thread.Sleep(Timings.Default_ms*20);
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement checkPassLink = empList[0].FindElement(By.XPath("//a[@id='passportEdit']"));
            string dateToCompare = Tools.FirstDayOfMonth;

            Assert.AreNotEqual(dateToCompare, checkPassLink.Text);
        }

        [Test]
        public void Z_DeletePassport()
        {
            int beforeDeletePassport = 0;
            int afterDeletePassport = 0;

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement checkboxValue = empList[0].FindElement(By.Id("HasPassport"));

            using (var db = new AjourBTForTestContext())
            {
                beforeDeletePassport = db.Passports.Count();
                Browser.ClickOnWebElement(checkboxValue);
                Thread.Sleep(Timings.Default_ms*30);
                afterDeletePassport = db.Passports.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement uncheckedVal = empList[0].FindElement(By.Id("HasPassport"));
            string result = uncheckedVal.GetAttribute("checked");

            Assert.Less(afterDeletePassport, beforeDeletePassport);
            Assert.AreNotEqual("checked", result);
        }

        #endregion

        #region Visa

        [Test]
        public void VisaAdd()
        {
            int beforeAddVisa = 0;
            int afterAddVisa = 0;

            IWebElement addVisaBtn = Browser.FindElementByXPath("//a[@id='CreateVisa']");
            Browser.ClickOnWebElement(addVisaBtn);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement visaTypeInput = Browser.FindElementByXPath("//input[@id='VisaType']");
            IWebElement createStartDateInput = Browser.FindElementByXPath("//input[@id='createStartDate']");
            IWebElement createDueDateInput = Browser.FindElementByXPath("//input[@id='createDueDate']");
            IWebElement entriesInput = Browser.FindElementByXPath("//input[@id='Entries']");
            IWebElement correctionInput = Browser.FindElementByXPath("//input[@id='CorrectionForVisaEntries']");
            IWebElement daysInput = Browser.FindElementByXPath("//input[@id='Days']");
            IWebElement correctionsForVisaInput = Browser.FindElementByXPath("//input[@id='CorrectionForVisaDays']");

            //fill form
            Browser.SendKeys(visaTypeInput, "V_C07");
            Browser.SendKeys(entriesInput, "4");
            Browser.SendKeys(correctionInput, "12");
            Browser.SendKeys(daysInput, "26");
            Browser.SendKeys(correctionsForVisaInput, "2");

            Browser.ClickByXPath("//*[@id='CreateVisaForm']/fieldset/table/tbody/tr[2]/td[2]/img");
            Browser.SelectDatePickerValue("1", 0);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickByXPath("//*[@id='CreateVisaForm']/fieldset/table/tbody/tr[3]/td[2]/img");
            Browser.SelectDatePickerValue(DateTime.Now.Day.ToString(), 0);
            Thread.Sleep(Timings.Default_ms*15);
            //find save btn and click
            Thread.Sleep(Timings.Default_ms*15);
            IWebElement sendBtn = Browser.FindElementByXPath("//button[@id='btnSave']");

            using (var db = new AjourBTForTestContext())
            {
                beforeAddVisa = db.Visas.Count();
                Browser.ClickOnWebElement(sendBtn);
                Thread.Sleep(Timings.Default_ms*20);
                afterAddVisa = db.Visas.Count();
            }

            //Get line with our changes

            IWebElement changedLine = Browser.FindElementByXPath("//td[contains(text(),'V_C07')]");

            Assert.Less(beforeAddVisa, afterAddVisa);
            Assert.AreEqual("V_C07", changedLine.Text);

        }

        //TODO: EditVisa

        [Test]
        public void VisaDelete()
        {
            int beforeVisaDelete = 0;
            int afterVisaDelete = 0;

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement deleteVisa = Browser.FindElementByXPath("//a[@class='visaEditDialog']");
            Browser.ClickOnWebElement(deleteVisa);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement deleteBtn = Browser.FindElementByXPath("//button[@id='btnDeleteVisa']");
            Browser.ClickOnWebElement(deleteBtn);

            Thread.Sleep(Timings.Default_ms*15);

            IWebElement deleteConfirmBtn = Browser.FindElementByXPath("//button[@id='OKDeleteVisa']");

            using (var db = new AjourBTForTestContext())
            {
                beforeVisaDelete = db.Visas.Count();
                Browser.ClickOnWebElement(deleteConfirmBtn);
                Thread.Sleep(Timings.Default_ms*30);
                afterVisaDelete = db.Visas.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement addVisaBtn = empList[0].FindElement(By.XPath("//a[@id='CreateVisa']"));

            Assert.Less(afterVisaDelete, beforeVisaDelete);
            Assert.AreEqual("Add Visa", addVisaBtn.Text);
        }

        #endregion

        #region Registration

        [Test]
        public void ZAddRegistration()
        {
            int beforeAddRegistration = 0;
            int afterAddRegistration = 0;

            IWebElement addRegistrationBtn = Browser.FindElementByXPath("//a[@id='CreateVisaRegistrationDate']");
            Browser.ClickOnWebElement(addRegistrationBtn);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement visaTypeInput = Browser.FindElementByXPath("//input[@id='VisaType']");
            IWebElement regDateInput = Browser.FindElementByXPath("//input[@id='RegistrationDate']");
            IWebElement visaRegTimeInput = Browser.FindElementByXPath("//input[@id='visaRegTime']");
            IWebElement PaymentDateInput = Browser.FindElementByXPath("//input[@id='PaymentDate']");
            IWebElement visaPaymentTimeInput = Browser.FindElementByXPath("//input[@id='visaPaymentTime']");
            IWebElement PaymentPINInput = Browser.FindElementByXPath("//input[@id='PaymentPIN']");

            //fill inputs

            Browser.SendKeys(visaTypeInput, "V_C07");
            Browser.SendKeys(visaRegTimeInput, "23:59");
            Browser.ClickOnDatePicker();

            //Find and click Save btn
            IWebElement saveBtn = Browser.FindElementByXPath("//button[@id='btnSaveRegDate']");

            using (var db = new AjourBTForTestContext())
            {
                beforeAddRegistration = db.VisaRegistrationDates.Count();
                Browser.ClickOnWebElement(saveBtn);
                Thread.Sleep(Timings.Default_ms*50);
                afterAddRegistration = db.VisaRegistrationDates.Count();
            }
            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement checkRegistrationAdded = empList[0].FindElement(By.XPath("//a[@class='visaRegistrationDateEdit']"));

            Assert.Less(beforeAddRegistration, afterAddRegistration);
            //Assert.AreEqual(Tools.TodayDate, checkRegistrationAdded.Text);
        }

        //TODO: edit registration

        [Test]
        public void ZDeleteRegistration()
        {
            int beforeVisaRegistrationDateDelete = 0;
            int afterVisaregistrationDatedelete = 0;

            IWebElement deleteRegistrationHref = Browser.FindElementByXPath("//a[@class='visaRegistrationDateEdit']");
            Browser.ClickOnWebElement(deleteRegistrationHref);

            Thread.Sleep(Timings.Default_ms*20);
            //Find Delete Btn
            IWebElement delRegistrationBtn = Browser.FindElementByXPath("//button[@id='btnDeleteVisaRegistrationDate']");

            using (var db = new AjourBTForTestContext())
            {
                beforeVisaRegistrationDateDelete = db.VisaRegistrationDates.Count();
                Browser.ClickOnWebElement(delRegistrationBtn);
                Thread.Sleep(Timings.Default_ms*50);
                afterVisaregistrationDatedelete = db.VisaRegistrationDates.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement addRegBtn = empList[0].FindElement(By.XPath("//a[@id='CreateVisaRegistrationDate']"));

            Assert.Less(afterVisaregistrationDatedelete, beforeVisaRegistrationDateDelete);
            Assert.AreEqual("Add", addRegBtn.Text);
        }

        #endregion

        #region Permit

        [Test]
        public void AddPermit()
        {
            int beforeAddPermit = 0;
            int afterAddPermit = 0;

            IWebElement permitBtn = Browser.FindElementByXPath("//a[@id='CreatePermit']");
            Browser.ClickOnWebElement(permitBtn);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement numPermit = Browser.FindElementByXPath("//input[@id='Number']");
            IWebElement startDatePermitCreate = Browser.FindElementByXPath("//input[@id='StartDatePermitCreate']");
            IWebElement endDatePermitCreate = Browser.FindElementByXPath("//input[@id='EndDatePermitCreate']");

            Browser.SendKeys(numPermit, "01/2014");
            ReadOnlyCollection<IWebElement> pictToClick = Browser.FindElementsByXPath("//img[@class='ui-datepicker-trigger']");
            Browser.ClickByXPath("//*[@id='CreatePermitForm']/fieldset/table/tbody/tr[3]/td[2]/img");
            Browser.SelectDatePickerValue("1", 0);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickByXPath("//*[@id='CreatePermitForm']/fieldset/table/tbody/tr[4]/td[2]/img");
            Browser.SelectDatePickerValue(DateTime.Now.Day.ToString(), 0);
            Thread.Sleep(Timings.Default_ms*15);
            IWebElement btnSave = Browser.FindElementByXPath("//button[@id='btnSave']");

            using (var db = new AjourBTForTestContext())
            {
                beforeAddPermit = db.Permits.Count();
                Browser.ClickOnWebElement(btnSave);
                Thread.Sleep(Timings.Default_ms*20);
                afterAddPermit = db.Permits.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement checkPermitAdded = empList[0].FindElement(By.XPath("//a[@class='permitEditDialog']"));

            string firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01).ToString(String.Format("dd.MM.yyyy"));
            string todayDate = DateTime.Now.ToString(String.Format("dd.MM.yyyy"));

            Assert.Less(beforeAddPermit, afterAddPermit);
            Assert.AreEqual(firstDayOfMonth + " - " + todayDate, checkPermitAdded.Text);
        }

        [Test]
        public void DeletePermit()
        {
            int beforeDeletePermit = 0;
            int afterDeletePermit = 0;

            IWebElement deletePermitHref = Browser.FindElementByXPath("//a[@class='permitEditDialog']");
            Browser.ClickOnWebElement(deletePermitHref);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement btnDeletePermit = Browser.FindElementByXPath("//button[@id='btnDeletePermit']");
            Browser.ClickOnWebElement(btnDeletePermit);

            Thread.Sleep(Timings.Default_ms*15);
            IWebElement deleteConfirmBtn = Browser.FindElementByXPath("//button[@id='OKDeletePermit']");
            Thread.Sleep(Timings.Default_ms*15);
            using (var db = new AjourBTForTestContext())
            {
                beforeDeletePermit = db.Permits.Count();
                Browser.ClickOnWebElement(deleteConfirmBtn);
                Thread.Sleep(Timings.Default_ms*15);
                afterDeletePermit = db.Permits.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement addRegBtn = empList[0].FindElement(By.XPath("//a[@id='CreatePermit']"));

            Assert.Less(afterDeletePermit, beforeDeletePermit);
            Assert.AreEqual("Add Permit", addRegBtn.Text);
                        
        }

        #endregion

        #region Insurance

        [Test]
        public void AddInsurance()
        {
            int beforeAddInsurance = 0;
            int afterAddInsurance = 0;

            IWebElement CreateInsurance = Browser.FindElementByXPath("//a[@id='CreateInsurance']");
            Browser.ClickOnWebElement(CreateInsurance);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement Days = Browser.FindElementByXPath("//input[@id='Days']");
            IWebElement btnSave = Browser.FindElementByXPath("//button[@id='btnSave']");
            Browser.ClickByXPath("//*[@id='CreateInsuranceForm']/fieldset/table/tbody/tr[1]/td[2]/img");
            Browser.SelectDatePickerValue("1", 0);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.ClickByXPath("//*[@id='CreateInsuranceForm']/fieldset/table/tbody/tr[2]/td[2]/img");
            Browser.SelectDatePickerValue(DateTime.Now.Day.ToString(), 0);
            Thread.Sleep(Timings.Default_ms*10);
            Browser.SendKeys(Days, "90");
            Thread.Sleep(Timings.Default_ms*15);

            using (var db = new AjourBTForTestContext())
            {
                beforeAddInsurance = db.Insurances.Count();
                Browser.ClickOnWebElement(btnSave);
                Thread.Sleep(Timings.Default_ms*20);
                afterAddInsurance = db.Insurances.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement checkInsuranceAdded = empList[0].FindElement(By.XPath("//a[@class='insuranceEditDialog']"));

            string firstDayOfMonth = new DateTime(2014, DateTime.Now.Month, 01).ToString(String.Format("dd.MM.yyyy"));
            string todayDate = DateTime.Now.ToString(String.Format("dd.MM.yyyy"));

            Assert.Less(beforeAddInsurance, afterAddInsurance);
            Assert.AreEqual(todayDate + " (90)", checkInsuranceAdded.Text);
        }

        [Test]
        public void DeleteInsurance()
        {
            int beforeDeleteInsurance = 0;
            int afterDeleteInsurance = 0;

            IWebElement insuranceEditDialog = Browser.FindElementByXPath("//a[@class='insuranceEditDialog']");
            Browser.ClickOnWebElement(insuranceEditDialog);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement btnDeleteInsurance = Browser.FindElementByXPath("//button[@id='btnDeleteInsurance']");
            Browser.ClickOnWebElement(btnDeleteInsurance);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement OKDeleteInsurance = Browser.FindElementByXPath("//button[@id='OKDeleteInsurance']");

            using (var db = new AjourBTForTestContext())
            {
                beforeDeleteInsurance = db.Insurances.Count();
                Browser.ClickOnWebElement(OKDeleteInsurance);
                Thread.Sleep(Timings.Default_ms*20);
                afterDeleteInsurance = db.Insurances.Count();
            }

            empList = Browser.FindElementsByXPath("//tr[contains(@class,'indexBT')]");
            IWebElement addRegBtn = empList[0].FindElement(By.XPath("//a[@id='CreateInsurance']"));

            Assert.Less(afterDeleteInsurance, beforeDeleteInsurance);
            Assert.AreEqual("Add Ins", addRegBtn.Text);
        }

        #endregion

        #endregion

        #region SortingTable 
     
        [Test]
        public void A_TableSorting_f_Days_ascending()
        {
            string path = "/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[7]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
           
                Assert.AreEqual(Browser.GetText(path), "Days");
                Assert.IsTrue(firstRow.Text.Contains("20"));
            }

        [Test]
        public void A_TableSorting_f_Days_descending()
            {
            string path = "/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[1]/div/table/thead/tr/th[7]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");


                Assert.AreEqual(Browser.GetText(path), "Days");
                Assert.IsTrue(firstRow.Text.Contains("180"));

            }

        [Test]
        public void A_TableSorting_g_Insurance_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[8]";
            IWebElement firstRow;
           Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Ins");
                 Assert.IsTrue(firstRow.Text.Contains("08.08.2011 (90)"));
                Assert.IsTrue(firstRow.Text.Contains("Perez Judy"));
                Assert.IsTrue(firstRow.Text.Contains("stho"));
            }

        [Test]
        public void A_TableSorting_g_Insurance_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[8]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual(Browser.GetText(path), "Ins");
                Assert.IsTrue(firstRow.Text.Contains("Hughes Mary"));
                Assert.IsTrue(firstRow.Text.Contains("sdea"));
            }

        [Test]
        public void A_TableSorting_h_Registration_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[9]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Reg");
                Assert.IsTrue(firstRow.Text.Contains("01.01.2012"));
                Assert.IsTrue(firstRow.Text.Contains("Olson Wayne"));
                Assert.IsTrue(firstRow.Text.Contains("olsa"));

            }

        [Test]
        public void A_TableSorting_h_Registration_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[9]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);
            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Reg");
                Assert.IsTrue(firstRow.Text.Contains("Reid Linda"));
                Assert.IsTrue(firstRow.Text.Contains("mhan"));
                Assert.IsTrue(firstRow.Text.Contains("04.01.2016"));
            }


        [Test]
        public void A_TableSorting_i_Number_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[10]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Num");
                Assert.IsTrue(firstRow.Text.Contains("01/2012"));
            }


        [Test]
        public void A_TableSorting_i_Number_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[10]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Num");
                Assert.IsTrue(firstRow.Text.Contains("mlan"));
                Assert.IsTrue(firstRow.Text.Contains("04/2015"));
                Assert.IsTrue(firstRow.Text.Contains("Rose Johnny"));


        }
        [Test]
        public void A_TableSorting_j_Permit_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[11]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Permit From - To");
                Assert.IsTrue(firstRow.Text.Contains("01.01.2011 - 08.08.2011"));
                Assert.IsTrue(firstRow.Text.Contains("apat"));
                Assert.IsTrue(firstRow.Text.Contains("Fuller Jason")); ;
            }

        [Test]
        public void A_TableSorting_j_Permit_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[11]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Permit From - To");
                Assert.IsTrue(firstRow.Text.Contains("mlan"));
                Assert.IsTrue(firstRow.Text.Contains("Rose Johnny"));
            Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(100).ToString("dd.MM.yyyy") + " - " + DateTime.Now.AddDays(200).ToString("dd.MM.yyyy")));
        }

        [Test]
        public void A_TableSorting_k_LastBT()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[12]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*35);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[2]");

                Assert.AreEqual(Browser.GetText(path), "Last BT");
                Console.WriteLine(firstRow.Text);
                Assert.IsTrue(firstRow.Text.Contains("HO/LR:02.02.13-05.02.13"));


        }

        [Test]
        public void A_TableSorting_l_ContactGow_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[13]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

            Assert.AreEqual(Browser.GetText(path), "Status");
            Assert.IsTrue(firstRow.Text.Contains("Contact Gov"));
        }

        [Test]
        public void A_TableSorting_l_ContactGow_descending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[13]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);

            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Status");
                Assert.IsTrue(firstRow.Text.Contains("lmor"));
                Assert.IsTrue(firstRow.Text.Contains("Moreno Lois"));
                Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(-15).ToString("dd.MM.yyyy")));


        }

        [Test]
        public void A_TableSorting_e_Entries_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[6]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
               
                Assert.AreEqual(Browser.GetText(path), "Entries");
                Assert.IsTrue(firstRow.Text.Contains("4"));
            }

        [Test]
        public void A_TableSorting_e_Entries_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[6]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Entries");
                Assert.IsTrue(firstRow.Text.Contains("MULT"));
            }

        [Test]
        public void A_TableSorting_d_VisaFromTo_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[5]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Visa From - To");
                Assert.IsTrue(firstRow.Text.Contains("13.02.2012"));
            }

        [Test]
        public void A_TableSorting_d_VisaFromTo_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[5]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Visa From - To");
            Assert.IsTrue(firstRow.Text.Contains(DateTime.Now.AddDays(300).ToString("dd.MM.yyyy") + " - " + DateTime.Now.AddDays(330).ToString("dd.MM.yyyy")));
                Assert.IsTrue(firstRow.Text.Contains("mlan"));
        }

        [Test]
        public void A_TableSorting_c_Type_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[4]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Type");
                Assert.IsTrue(firstRow.Text.Contains("V_C07"));

            }

        [Test]
        public void A_TableSorting_c_Type_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[4]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Type");
                Assert.IsTrue(firstRow.Text.Contains("V_D08"));

        }

        [Test]
        public void A_TableSorting_b_Name_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");
                Assert.AreEqual(Browser.GetText(path), "Name");
                Assert.IsTrue(firstRow.Text.Contains("Andrews Marie"));
                Assert.IsTrue(firstRow.Text.Contains("maad"));
            }

        [Test]
        public void A_TableSorting_b_Name_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[2]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*25);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "Name");
                Assert.IsTrue(firstRow.Text.Contains("Wood Harold"));
                Assert.IsTrue(firstRow.Text.Contains("dkim"));
            }

        [Test]
        public void A_TableSorting_a_EID_ascending()
        {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]";
            IWebElement firstRow;
            Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*30);

            firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "EID");
                Assert.IsTrue(firstRow.Text.Contains("Lee Raymond"));
                Assert.IsTrue(firstRow.Text.Contains("aada"));
            }


        [Test]
        public void A_TableSorting_a_EID_descending()
            {
            string path = "//*[@id='visasView_wrapper']/div[1]/div[1]/div/table/thead/tr/th[1]";

            IWebElement firstRow;
                Browser.ClickByXPath(path);
            Thread.Sleep(Timings.Default_ms*20);
                firstRow = Browser.FindElementByXPath("/html/body/div[1]/div/div[1]/div/div[1]/div/div[1]/div[2]/table/tbody/tr[1]");

                Assert.AreEqual(Browser.GetText(path), "EID");
                Assert.IsTrue(firstRow.Text.Contains("Simmons Teresa"));
                Assert.IsTrue(firstRow.Text.Contains("wsim"));

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
