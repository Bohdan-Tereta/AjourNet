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
    class QuestionnaireTabTest
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

            Assert.That(Browser.IsAt(baseURL + "Home/HRView"));
            Browser.ClickOnLink("Questionnaires");

            string Absence = Browser.FindElementByLinkText("Questionnaires").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", Absence);
        }

        #region Questionnaire

        [Test]
        public void XQuestionnairesView()
        {
            using (AjourBTForTestContext context = new AjourBTForTestContext())
            {
                List<Questionnaire> questionnaireList = context.Questionnaire.ToList();

                for (int i = 0; i < questionnaireList.Count; i++)
                {
                    IReadOnlyCollection<IWebElement> questionnairesCollection = Browser.FindElementsByXPath("//a[@class='questionTitle']");
                    Browser.ClickOnWebElement(questionnairesCollection.ToList()[i]);
                    Thread.Sleep(1000);

                    IReadOnlyCollection<IWebElement> ordrersValues = Browser.FindElementsByXPath("//input[@id='orderVal']");
                    IReadOnlyCollection<IWebElement> dropdowns = Browser.FindElementsByXPath("//select[@name='testDropDown']");
                    string[] tempData;
                    if (questionnaireList[i].QuestionSetId != null)
                    {
                        tempData = questionnaireList[i].QuestionSetId.Split(',');
                        Assert.AreEqual(ordrersValues.Count-1, tempData.Length);
                        Assert.AreEqual(dropdowns.Count-1, tempData.Length);

                        for (int j = 0; j < tempData.Length; j++)
                        {
                            string[] separatedValues = tempData[j].Split(':');
                            Assert.AreEqual(separatedValues[0], ordrersValues.ToList()[j].GetAttribute("value"));
                            //Assert.AreEqual(separatedValues[1], dropdowns.ToList()[j].Text);
                        }
                    }

                    IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
                    Browser.ClickOnWebElement(closeBtn);

                    Thread.Sleep(1000);
                }
            }
        }

        [Test]
        public void XQuestionnaireEdit_CorrectSave()
        {
            IWebElement questionnaire = Browser.FindElementByXPath("//a[@class='questionTitle']");
            Browser.ClickOnWebElement(questionnaire);

            Thread.Sleep(1000);

            IWebElement orderInput = Browser.FindElementByXPath("//input[@id='orderVal']");
            IWebElement dropdown = Browser.FindElementByXPath("//select[@name='testDropDown']");
            IWebElement addRowBtn = Browser.FindElementByXPath("//a[@id='questionSetToAdd']");
            IWebElement saveQuestionnaire = Browser.FindElementByXPath("//a[@id='questionnaireSaveBtn']");

            IWebElement lastOrderInput = Browser.FindElementsByXPath("//input[@id='orderVal']").LastOrDefault();
            IWebElement lastDropdown = Browser.FindElementsByXPath("//select[@name='testDropDown']").LastOrDefault();

            Browser.SendKeys(lastOrderInput, "6");
            Browser.SelectOption(lastDropdown, "SecondQuestionSet");

            Browser.ClickOnWebElement(addRowBtn);
            Thread.Sleep(1000);

            Browser.ClickOnWebElement(saveQuestionnaire);
            Thread.Sleep(1000);

            IWebElement questionnaireNew = Browser.FindElementByXPath("//a[@class='questionTitle']");
            Browser.ClickOnWebElement(questionnaireNew);

            Thread.Sleep(1000);

            IWebElement lastOrder = Browser.FindElementsByXPath("//input[@id='orderVal']").Skip(1).Take(1).First();
            IWebElement lastDropdownList = Browser.FindElementsByXPath("//select[@name='testDropDown'][1]").Skip(1).Take(1).First();

            Assert.AreEqual("6", lastOrder.GetAttribute("value"));
            //make assert for dropdown value

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);
            Thread.Sleep(1000);
        }

        [Test]
        public void XQuestionnaireEdit_CanDeleteRow()
        {
            IWebElement questionnaire = Browser.FindElementByXPath("//a[@class='questionTitle']");
            Browser.ClickOnWebElement(questionnaire);

            Thread.Sleep(1000);

            IReadOnlyCollection<IWebElement> rows = Browser.FindElementsByXPath("//tr[@id='questionDataRow']");
            IWebElement deleteBtn = Browser.FindElementByXPath("//a[@id='questionSetDelete']");
            Browser.ClickOnWebElement(deleteBtn);

            Thread.Sleep(1000);

            IReadOnlyCollection<IWebElement> rowsAfterDelete = Browser.FindElementsByXPath("//tr[@id='questionDataRow']");

            Assert.AreEqual(1, rowsAfterDelete.Count);

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);

            Thread.Sleep(1000);
        }

        [Test]
        public void XDeleteQuestionnaire_OK()
        {
            IReadOnlyCollection<IWebElement> questionnairesCollection = Browser.FindElementsByXPath("//a[@class='questionTitle']");
            int qCount = questionnairesCollection.Count;
            IWebElement el = questionnairesCollection.FirstOrDefault(); 
            Browser.ClickOnWebElement(el);

            Thread.Sleep(1500);

            IWebElement deleteBtn = Browser.FindElementByXPath("//a[@id='questionnaireDeleteBtn']");
            Browser.ClickOnWebElement(deleteBtn);
            Thread.Sleep(1500);
            Browser.ClickByXPath("//button[@id='okDeleteConfirm']");

            Thread.Sleep(1500);

            IReadOnlyCollection<IWebElement> questAfterDelete = Browser.FindElementsByXPath("//a[@class='questionTitle']");
            int countAfterDelete = questAfterDelete.Count;

            Assert.Less(countAfterDelete, qCount);
        }

        [Test]
        public void XDeleteQuestionnaire_Cancel()
        {
            IReadOnlyCollection<IWebElement> questionnairesCollection = Browser.FindElementsByXPath("//a[@class='questionTitle']");
            int qCount = questionnairesCollection.Count; 

            Thread.Sleep(1500); 

            Browser.ClickOnLink("Questionnaire 1");

            Thread.Sleep(1500);

            IWebElement deleteBtn = Browser.FindElementByXPath("//a[@id='questionnaireDeleteBtn']");
            Browser.ClickOnWebElement(deleteBtn);
            Thread.Sleep(1500);
            Browser.ClickByXPath("//button[@id='cancelDeleteConfirm']");

            Thread.Sleep(1500);

            IWebElement closeBtn = Browser.FindElementByXPath("//a[@class='ui-dialog-titlebar-close ui-corner-all']");
            Browser.ClickOnWebElement(closeBtn);

            Thread.Sleep(1000);

            IReadOnlyCollection<IWebElement> questAfterDelete = Browser.FindElementsByXPath("//a[@class='questionTitle']");
            int countAfterDelete = questAfterDelete.Count;

            Assert.AreEqual(countAfterDelete, qCount);
        }         

        [Test]
        public void QuestionnaireAddEmptyQuestionSet_Alert()
        {
            IWebElement questTitle = Browser.FindElementByXPath("//a[@class='questionTitle']");
            Browser.ClickOnWebElement(questTitle);

            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement addBtn = Browser.FindElementByXPath("//a[@id='questionSetToAdd']");
            IWebElement orderInput = Browser.FindElementsByXPath("//input[@id='orderVal']").LastOrDefault();
            IWebElement dropdownInput = Browser.FindElementsByXPath("//select[@name='testDropDown']").LastOrDefault();
            Browser.ClickOnWebElement(addBtn);

            Assert.AreEqual(" № and QuestionSet can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();

            Browser.SendKeysTo(orderInput, "1", true);
            Browser.ClickOnWebElement(addBtn);
            Assert.AreEqual(" № and QuestionSet can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();

            Thread.Sleep(Timings.Default_ms * 10);
            
            Browser.Clear(orderInput);
            Browser.SelectOption(dropdownInput, "FirstQuestionSet");
            Browser.ClickOnWebElement(addBtn);
            Assert.AreEqual(" № and QuestionSet can not be empty! ", Browser.webDriver.SwitchTo().Alert().Text);
            Browser.webDriver.SwitchTo().Alert().Accept();

            IWebElement closeBtn = Browser.FindElementByXPath("//span[@class='ui-icon ui-icon-closethick']");
            Browser.ClickOnWebElement(closeBtn);
        }


        [TestFixtureTearDown]
        public void TeardownTest()
        {
            if (Browser.HasElement("Log off"))
                Browser.ClickOnLink("Log off");
        }

        #endregion

        #region Add Questionary

        [Test]
        public void CreateQuestionnaire_AddCuestionnaire()
        {
            using (AjourBTForTestContext context = new AjourBTForTestContext())
            {
                List<Questionnaire> questionnaireList = context.Questionnaire.ToList();
                IReadOnlyCollection<IWebElement> questionnairesCollection = Browser.FindElementsByXPath("//a[@class='questionTitle']");

                Assert.AreEqual(questionnaireList.Count, questionnairesCollection.Count);

                Browser.ClickByXPath("//*[@id='addQuestionnaireButton']/span");
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[1]/td/input"), "New Questionnaire");
                Thread.Sleep(Timings.Default_ms * 10);
                Browser.ClickByXPath("//*[@id='addQuestionnaire']");
                Thread.Sleep(Timings.Default_ms * 20);

                using (AjourBTForTestContext context2 = new AjourBTForTestContext())
                {
                    List<Questionnaire> questionnaireListAfterAdd = context.Questionnaire.ToList();
                    IReadOnlyCollection<IWebElement> questionnairesCollectionAfterAdd = Browser.FindElementsByXPath("//a[@class='questionTitle']");

                    Assert.AreEqual(questionnaireListAfterAdd.Count, questionnairesCollectionAfterAdd.Count);
                    Assert.Greater(questionnaireListAfterAdd.Count, questionnaireList.Count);
                    Assert.Greater(questionnairesCollectionAfterAdd.Count, questionnairesCollection.Count);
                }
            }
        }

        #endregion

        #region Generate Questionary Preview


        [Test]
        public void Questionnaire_GenerateCuestionnaireEmptyQuestionnaire()
        {
            Browser.ClickOnLink("New Questionnaire");
            Browser.ClickByXPath("//*[@id='generateQuestButton']");
            Thread.Sleep(Timings.Default_ms * 10);
            Assert.NotNull(Browser.FindElementByXPath("//*[@id='ui-dialog-title-generateQ']"));
            Browser.ClickByXPath("/html/body/div[5]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
            Thread.Sleep(Timings.Default_ms * 10);
        }


        [Test]
        public void Questionnaire_GenerateCuestionnaire()
        {
            Browser.ClickOnLink("Questionnaire 1");
            Browser.ClickByXPath("//*[@id='generateQuestButton']");
            Thread.Sleep(Timings.Default_ms * 10);
            Assert.NotNull(Browser.FindElementByXPath("//*[@id='ui-dialog-title-generateQ']"));
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr/td[1]").Text, "5");
            Assert.IsTrue(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr/td[2]").Text.Contains("Question"));
            Browser.ClickByXPath("/html/body/div[5]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 10);
        }

        [Test]
        public void Questionnaire_GenerateCuestionnaire_AddSomeQuestions()
        {
            Browser.ClickOnLink("Questionnaire 1");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.SendKeys((Browser.FindElementsByXPath("//*[@id='orderVal']")[1]), "0");
            Thread.Sleep(Timings.Default_ms * 10);

            Browser.ClickByXPath("//*[@id='questionDataRow'][2]/td[1]/select");
            Browser.ClickByXPath("//*[@id='questionDataRow'][2]/td[1]/select/option[3]");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("//*[@id='questionSetToAdd']");
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("//*[@id='generateQuestButton']");

            Assert.NotNull(Browser.FindElementByXPath("//*[@id='ui-dialog-title-generateQ']"));
            Thread.Sleep(Timings.Default_ms * 10);

           // Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[1]/td[1]").Text, "5");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[2]/td[1]").Text, "5");
          //  Assert.IsTrue(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[1]/td[2]").Text.Contains("Question"));
            Assert.IsTrue(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[2]/td[2]").Text.Contains("Question"));
            Browser.ClickByXPath("/html/body/div[5]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 10);
        }

        [Test]
        public void Questionnaire_GenerateCuestionnaire_QuestionsAreOrderedByTitle()
        {
            Browser.ClickOnLink("Questionnaire 3");
            Browser.ClickByXPath("//*[@id='generateQuestButton']");
            Thread.Sleep(Timings.Default_ms * 10);
            Assert.NotNull(Browser.FindElementByXPath("//*[@id='ui-dialog-title-generateQ']"));
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[1]/td[1]").Text, "1");
            Assert.IsTrue(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[1]/td[2]").Text.Contains("Question"));
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[2]/td[1]").Text, "1.1");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[3]/td[1]").Text, "1a");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[4]/td[1]").Text, "1b");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[5]/td[1]").Text, "3");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[6]/td[1]").Text, "4");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[7]/td[1]").Text, "6");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[8]/td[1]").Text, "10");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='createQuestionnaireTable']/tbody/tr[9]/td[1]").Text, "11");


            Browser.ClickByXPath("/html/body/div[5]/div[1]/a");
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 20);
            Thread.Sleep(Timings.Default_ms * 10);
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 10);
        }
        #endregion
    }
}
