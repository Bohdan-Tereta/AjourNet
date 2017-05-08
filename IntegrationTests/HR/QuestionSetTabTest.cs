using AjourBT.Domain.Entities;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.HR
{
        [TestFixture]
    class QuestionSetTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;
        private string validationErrorMsg = "The From Date must be less than To date";

        [TestFixtureSetUp]
        public void Setup()
        {
            //Arrange
            Browser.webDriver.Manage().Window.Maximize();
            username = "mter";
            password = "654321";
            Browser.Goto(baseURL);

            // Act
            Browser.SendKeysTo("UserName", username, true);
            Browser.SendKeysTo("Password", password, true);
            Browser.ClickByXPath("//input[@type='submit']");
            Browser.ClickOnLink("HR");

            Browser.ClickOnLink("QuestionSets");

            string Absence = Browser.FindElementByLinkText("QuestionSets").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Absence);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        #region QuestionSetTab
        [Test]
        public void QuestionSetsTabVerify()
        {
            //Arrange

            //Act

            //Assert   
            //Assert.AreEqual("QuestionSets", Browser.FindElementByXPath("//*[@id='ui-tabs-1']/h2").Text);
            Assert.IsNotNull(Browser.FindElementByLinkText("FirstQuestionSet"));
            Assert.IsNotNull(Browser.FindElementByLinkText("SecondQuestionSet"));
            Assert.IsNotNull(Browser.FindElementByLinkText("ThirdQuestionSet"));
            Assert.IsNotNull(Browser.FindElementByLinkText("FourthQuestionSet"));
            Assert.AreEqual("Add QuestionSet", Browser.FindElementByXPath("//*[@id='createQuestionSetButton']").Text);
        }

        [Test]
        public void QuestionSetCreate_CheckIfElementsExist()
        {
            //Arrange
            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert   
            Assert.AreEqual(4, rowCount);
            Browser.ClickOnLink("Add QuestionSet");
            Assert.AreEqual("Create QuestionSet", Browser.FindElementByXPath("//*[starts-with(@id,'ui-dialog-title-')]").Text);
            Assert.AreEqual("Title", Browser.FindElementByXPath("//*[@id='questionSetTitleLabel']").Text);
            Assert.AreEqual("Questions", Browser.FindElementByXPath("//*[@id='questionSetQuestionsLabel']").Text);
            Assert.AreEqual("Add", Browser.FindElementByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Text);
            Assert.AreEqual("Save", Browser.FindElementByXPath("//*[@id='saveQuestionSet']").Text);
            Assert.IsNotNull(Browser.FindElementByXPath("//*[@id='questionSetTitleInput']"));
            Assert.IsNotNull(Browser.FindElementByXPath("//*[@class='questionInput']"));
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(4, rowCount);
        }

        [Test]
        public void QuestionSetCreate_AddEmptyQuestion_Alert()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(4, rowCount);
            Browser.ClickOnLink("Add QuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.ClickOnLink("Add");
            Thread.Sleep(Timings.Default_ms);
            Assert.AreEqual("Please enter the Question. It can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();

            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(4, rowCount);
        }

        [Test]
        public void QuestionSetCreate_EmptyTitleCreateQuestionSet_Alert()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(4, rowCount);
            Browser.ClickOnLink("Add QuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@id='saveQuestionSet']"));
            Thread.Sleep(Timings.Default_ms);
            Assert.AreEqual("Please enter the QuestionSet Title. It can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(4, rowCount);
        }

        [Test]
        public void QuestionSetCreate_Title_QuestionSetCreated()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(4, rowCount);
            Browser.ClickOnLink("Add QuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@id='questionSetTitleInput']"), "FifthQuestionSet");
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@id='saveQuestionSet']"));
            Thread.Sleep(Timings.Default_ms);
            Assert.IsNotNull(Browser.FindElementByLinkText("FifthQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(5, rowCount);
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickOnLink("QuestionSets");
            Thread.Sleep(Timings.Default_ms * 10); 
            Assert.IsNotNull(Browser.FindElementByLinkText("FifthQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(5, rowCount);
        }

        [Test]
        public void QuestionSetCreate_TitleAndTwoQuestions_QuestionSetCreated()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(5, rowCount);
            Browser.ClickOnLink("Add QuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@id='questionSetTitleInput']"), "SixthQuestionSet");
            Browser.SendKeys(Browser.FindElementByXPath("(//*[@class='questionInput'])[1]"), "1");
            Browser.ClickOnLink("Add");
            Browser.SendKeys(Browser.FindElementByXPath("(//*[@class='questionInput'])[2]"), "2");
            Browser.ClickOnLink("Add");
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@id='saveQuestionSet']"));
            Thread.Sleep(Timings.Default_ms);
            Assert.IsNotNull(Browser.FindElementByLinkText("SixthQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count();
            Assert.AreEqual(6, rowCount);
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickOnLink("QuestionSets");
            Thread.Sleep(Timings.Default_ms * 10); 
            Assert.IsNotNull(Browser.FindElementByLinkText("SixthQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count();
            Assert.AreEqual(6, rowCount);
            Browser.ClickOnLink("SixthQuestionSet");
            Assert.AreEqual("1", Browser.FindElementByXPath("(//*[@class='questionInput'])[1]").GetAttribute("value"));
            Assert.AreEqual("2", Browser.FindElementByXPath("(//*[@class='questionInput'])[2]").GetAttribute("value"));
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
        }

        [Test]
        public void QuestionSetEdit_QuestionSetModified()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(6, rowCount);
            Browser.ClickOnLink("SixthQuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='questionSetTitleInput']"), "SeventhQuestionSet", true);
            Browser.ClickOnWebElement(Browser.FindElementByXPath("(//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')])[1]"));
            Browser.SendKeys(Browser.FindElementByXPath("(//*[@class='questionInput'])[2]"), "3");
            Browser.ClickOnLink("Add");
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@id='saveQuestionSet']"));
            Thread.Sleep(Timings.Default_ms);
            Assert.IsNotNull(Browser.FindElementByLinkText("SeventhQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count();
            Assert.AreEqual(6, rowCount);
            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickOnLink("QuestionSets");
            Thread.Sleep(Timings.Default_ms * 10); 
            Assert.IsNotNull(Browser.FindElementByLinkText("SeventhQuestionSet"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count();
            Assert.AreEqual(6, rowCount);
            Browser.ClickOnLink("SeventhQuestionSet");
            Assert.AreEqual("2", Browser.FindElementByXPath("(//*[@class='questionInput'])[1]").GetAttribute("value"));
            Assert.AreEqual("3", Browser.FindElementByXPath("(//*[@class='questionInput'])[2]").GetAttribute("value"));
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
        }

        [Test]
        public void QuestionSetEditDelete_QuestionSetDelete()
        {
            //Arrange 

            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert  
            Assert.AreEqual(6, rowCount);
            Browser.ClickOnLink("SeventhQuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.ClickOnWebElement(Browser.FindElement(By.XPath("//*[@id='deleteQuestionSet']"), 6));
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement OKBtn = Browser.FindElementByXPath("//button[@id='okDeleteConfirm']");
            Browser.ClickOnWebElement(OKBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            bool seventhLinkPresents = true;
            try
            {
                Assert.IsNull(Browser.FindElementByLinkText("SeventhQuestionSet"));
            }
            catch (Exception)
            {
                seventhLinkPresents = false;
            }
            Assert.IsFalse(seventhLinkPresents); 
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count(); 
            Assert.AreEqual(5, rowCount); 

            Browser.Refresh();
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickOnLink("QuestionSets");
            Thread.Sleep(Timings.Default_ms * 10); 

            seventhLinkPresents = true;
            try
            {
                Assert.IsNull(Browser.FindElementByLinkText("SeventhQuestionSet"));
            }
            catch (Exception)
            {
                seventhLinkPresents = false;
            }
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 6).Count();
            Assert.AreEqual(5, rowCount); 
            Assert.IsFalse(seventhLinkPresents);
        }

        [Test]
        public void QuestionSetCreate_QuestionsAreAddedToKnockoutModelAndRemoved_TableRowsAreAdded()
        {
            //Arrange
            int rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();

            //Act

            //Assert        
            Assert.AreEqual(4, rowCount);

            Browser.ClickOnLink("Add QuestionSet");
            Thread.Sleep(Timings.Default_ms);
            Browser.SendKeys(Browser.FindElementByXPath("//*[@class='questionInput']"), "FifthQuestionSet");
            Browser.ClickOnLink("Add");
            Thread.Sleep(Timings.Default_ms);
            int deleteButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == true).Count();
            int addButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == true).Count();
            int deleteButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == false).Count();
            int addButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == false).Count();
            Assert.AreEqual(1, deleteButtonsCount);
            Assert.AreEqual(1, addButtonsCount);
            Assert.AreEqual(1, deleteButtonsInvisibleCount);
            Assert.AreEqual(1, addButtonsInvisibleCount);
            Browser.SendKeys(Browser.FindElementByXPath("(//*[@class='questionInput'])[2]"), "FifthQuestionSet");
            Browser.ClickOnLink("Add");
            Thread.Sleep(Timings.Default_ms);
            deleteButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == true).Count();
            addButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == true).Count();
            deleteButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == false).Count();
            addButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == false).Count();
            Assert.AreEqual(2, deleteButtonsCount);
            Assert.AreEqual(1, addButtonsCount);
            Assert.AreEqual(1, deleteButtonsInvisibleCount);
            Assert.AreEqual(2, addButtonsInvisibleCount);
            Browser.ClickOnLink("Delete");
            Thread.Sleep(Timings.Default_ms);
            deleteButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == true).Count();
            addButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == true).Count();
            deleteButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == false).Count();
            addButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == false).Count();
            Assert.AreEqual(1, deleteButtonsCount);
            Assert.AreEqual(1, addButtonsCount);
            Assert.AreEqual(1, deleteButtonsInvisibleCount);
            Assert.AreEqual(1, addButtonsInvisibleCount);
            Browser.ClickOnLink("Delete");
            Thread.Sleep(Timings.Default_ms);
            deleteButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == true).Count();
            addButtonsCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == true).Count();
            deleteButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionDelete ')]").Where(el => el.Displayed == false).Count();
            addButtonsInvisibleCount = Browser.FindElementsByXPath("//*[contains(concat(' ',normalize-space(@class),' '),' questionToAdd ')]").Where(el => el.Displayed == false).Count();
            Assert.AreEqual(0, deleteButtonsCount);
            Assert.AreEqual(1, addButtonsCount);
            Assert.AreEqual(1, deleteButtonsInvisibleCount);
            Assert.AreEqual(0, addButtonsInvisibleCount);
            Browser.ClickOnWebElement(Browser.FindElementByXPath("//*[@class='ui-icon ui-icon-closethick']"));
            rowCount = Browser.FindElements(By.XPath("//*[@id='getQuestionSets']/table/tbody/tr"), 5).Count();
            Assert.AreEqual(4, rowCount);
            
        }


        [ExpectedException(typeof(NoSuchElementException))]
        [Test]
        public void XDeleteQuestionSet_ConfirmDeleteWindowCancelPressed()
        {
            IWebElement firstQuestionSet = Browser.FindElementByXPath("//a[@class='questionSetTitle']");
            Browser.ClickOnWebElement(firstQuestionSet);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement deleteBtn = Browser.FindElementByXPath("//a[@id='deleteQuestionSet']");
            Browser.ClickOnWebElement(deleteBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement cancelBtn = Browser.FindElementByXPath("//button[@id='cancelDeleteConfirm']");
            IWebElement confirmWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-ConfirmQuestionSetDelete']");

            Browser.ClickOnWebElement(cancelBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement mainModalWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-1']");
            IWebElement retryConfirmWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-ConfirmQuestionSetDelete']");//raise ex
        }

        [ExpectedException(typeof(NoSuchElementException))]
        [Test]
        public void XDeleteQuestionSet_ConfirmDeleteWindowDeletePressed()
        {
            IWebElement deleteBtn = Browser.FindElementByXPath("//a[@id='deleteQuestionSet']");
            Browser.ClickOnWebElement(deleteBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement OKBtn = Browser.FindElementByXPath("//button[@id='okDeleteConfirm']");
            IWebElement confirmWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-ConfirmQuestionSetDelete']");

            Browser.ClickOnWebElement(OKBtn);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement mainModalWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-1']");//raise ex
            IWebElement retryConfirmWindow = Browser.FindElementByXPath("//span[@id='ui-dialog-title-ConfirmQuestionSetDelete']");//raise ex
        }


        [Test]
        public void EditQuestionField_FieldEmpty_validationAlert()
        {
            IWebElement firstQuestionSet = Browser.FindElementByXPath("//a[@class='questionSetTitle']");
            Browser.ClickOnWebElement(firstQuestionSet);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement firstQuestInput = Browser.FindElementByXPath("//textarea[@class='questionInput']");
            Browser.SendKeysTo(firstQuestInput, "", true);
            IWebElement saveBtn = Browser.FindElementByXPath("//a[@id='saveQuestionSet']");
            Browser.ClickOnWebElement(saveBtn);

            Assert.AreEqual("Please enter the Question. It can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();

            IWebElement closeModal = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");
            Browser.ClickOnWebElement(closeModal);
        }

        #endregion
    }
}
