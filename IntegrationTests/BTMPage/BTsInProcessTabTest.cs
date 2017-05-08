using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.BTMPage
{
    [TestFixture]
    class BTsInProcessTabTest
    {
        private string baseURL = "http://localhost:50616/";
        private string username;
        private string password;

        private int positionInRow;
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
            Browser.ClickOnLink("BTs in process");

            string BTsInProcess = Browser.FindElementByLinkText("BTs in process").GetCssValue("color");
            Assert.AreEqual("rgba(225, 112, 9, 1)", BTsInProcess);
        }

        #region BTs in process Tab

        [Test]
        public void SearchField_BadString_NoResult()
        {
            Thread.Sleep(Timings.Default_ms*20);
            IWebElement seachInputBT = Browser.FindElementByXPath("//input[@id='seachInputBT']");
            Browser.SendKeysTo(seachInputBT, "Bad SearchText", true);
            Browser.SendKeys(seachInputBT, Keys.Enter);

            Thread.Sleep(Timings.Default_ms*20);

            Assert.IsTrue(Browser.FindElementByClassName("dataTables_empty").Text.Contains(searchResultFail));
        }

        [Test]
        public void SearchField_ByName_NotEmpty()
        {
            IWebElement seachInputBT = Browser.FindElementByXPath("//input[@id='seachInputBT']");
            Browser.SendKeysTo(seachInputBT, "Cruz", true);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeys(seachInputBT, Keys.Enter);

            Thread.Sleep(Timings.Default_ms*30);
            empList = Tools.GetAllTableData("//tbody[@id='tbodyBts']/tr[contains(@class, 'zebra')]", Browser.webDriver);

            Browser.Clear(seachInputBT);
            Browser.SendKeys(seachInputBT, Keys.Enter);

            Assert.AreEqual(1, empList.Count);
        }

        [Test]
        public void SearchField_BySurnamePart_NotEmpty()
        {
            IWebElement seachInputBT = Browser.FindElementByXPath("//input[@id='seachInputBT']");
            Browser.SendKeysTo(seachInputBT, "ph", true);
            Thread.Sleep(Timings.Default_ms*20);
            Browser.SendKeys(seachInputBT, Keys.Enter);

            Thread.Sleep(Timings.Default_ms*20);
            empList = Tools.GetAllTableData("//tbody[@id='tbodyBts']/tr[contains(@id, 'Data')]", Browser.webDriver);
            Browser.Clear(seachInputBT);
            Browser.SendKeys(seachInputBT, Keys.Enter);

            Assert.AreEqual(2, empList.Count);
        }

        [Test]
        public void TestArrangeBT()
        {
            Thread.Sleep(Timings.Default_ms*15);

            IWebElement Habitation = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-15']");
            Browser.ClickOnWebElement(Habitation);
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Habitation-create']");

            //find elements in modal window
            ReadOnlyCollection<IWebElement> checkboxes = Tools.FindAllCheckboxes(modalWindow);
            ReadOnlyCollection<IWebElement> textareas = Tools.FindAllTextAreas(modalWindow);
            IWebElement btnSave = modalWindow.FindElement(By.XPath("//button[@id='btnSave']"));

            //set text in all textareas
            foreach (IWebElement textareaToFill in textareas)
            {
                if (textareaToFill.GetAttribute("readonly") == null)
                {
                    textareaToFill.Clear();
                    textareaToFill.SendKeys("Test Text");
                }
            }

            //make checkboxes checked
            foreach (IWebElement checkboxToClick in checkboxes)
            {
                if (!(checkboxToClick.GetAttribute("checked") == "true"))
                    Browser.ClickOnWebElement(checkboxToClick);
            }
            Browser.ClickOnWebElement(btnSave);

            //check changes

            //update our table
            Thread.Sleep(Timings.Default_ms*15);
            empList = Tools.GetAllTableData("//tr[contains(@class, 'zebra')]", Browser.webDriver);
            foreach (IWebElement item in empList)
            {
                if (item.Text.Contains("aada"))
                {
                   IWebElement element = item;

                   Assert.IsTrue(!(element.Text.Contains("+ Habitation")));
                   Assert.IsTrue(!(element.Text.Contains("+ Flights")));
                   Assert.IsTrue(!(element.Text.Contains("+ Invitation")));
                }
            }
        }

        [Test]
        public void TestArrangeBT_RemoveHabitationFlightsInvitation()
        {
            IWebElement Habitation = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-15']");
            Browser.ClickOnWebElement(Habitation);
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Habitation-create']");

            //find elements in modal window
            ReadOnlyCollection<IWebElement> checkboxes = Tools.FindAllCheckboxes(modalWindow);
            ReadOnlyCollection<IWebElement> textareas = Tools.FindAllTextAreas(modalWindow);

            IWebElement btnSave = modalWindow.FindElement(By.XPath("//button[@id='btnSave']"));

            foreach (IWebElement textareaToFill in textareas)
            {
                if (textareaToFill.GetAttribute("readonly") == null)
                {
                    Browser.Clear(textareaToFill);
                }
            }

            //make checkboxes checked
            foreach (IWebElement checkboxToClick in checkboxes)
            {
                if ((checkboxToClick.GetAttribute("checked") == "true"))
                    Browser.ClickOnWebElement(checkboxToClick);
            }

            Browser.ClickOnWebElement(btnSave);

            //check changes

            //update our table
            Thread.Sleep(Timings.Default_ms*15);
            empList = Tools.GetAllTableData("//tr[contains(@class, 'zebra')]", Browser.webDriver);
            foreach (IWebElement item in empList)
            {
                if (item.Text.Contains("aada"))
                {
                    IWebElement element = item;

                    Assert.IsTrue(element.Text.Contains("+ Habitation"));
                    Assert.IsTrue(element.Text.Contains("+ Flights"));
                    Assert.IsTrue(element.Text.Contains("+ Invitation"));
                }
            }
        }

        [Test]
        public void TestArrangeBT_TextConfirmHabitation()
        {
            IWebElement Habitation = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-15']");
            Browser.ClickOnWebElement(Habitation);
            Thread.Sleep(Timings.Default_ms*20);

            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Habitation-create']");

            //find elements in modal window
            ReadOnlyCollection<IWebElement> checkboxes = Tools.FindAllCheckboxes(modalWindow);
            ReadOnlyCollection<IWebElement> textareas = Tools.FindAllTextAreas(modalWindow);

            IWebElement btnSave = modalWindow.FindElement(By.XPath("//button[@id='btnSave']"));

            foreach (IWebElement textareaToFill in textareas)
            {
                if (textareaToFill.GetAttribute("readonly") == null)
                {
                Browser.SendKeysTo(textareaToFill, "Test Text", true); 
                }
            }

            //make checkboxes checked
            foreach (IWebElement checkboxToClick in checkboxes)
            {
                if ((checkboxToClick.GetAttribute("checked") == "true"))
                    Browser.ClickOnWebElement(checkboxToClick);
            }
            Browser.ClickOnWebElement(btnSave);

            //check changes

            //update our table
            Thread.Sleep(Timings.Default_ms*15);
            empList = Tools.GetAllTableData("//tr[contains(@class, 'zebra')]", Browser.webDriver);
            foreach (IWebElement item in empList)
            {
                if (item.Text.Contains("aada"))
                {
                    IWebElement element = item;

                    Assert.IsTrue(element.Text.Contains("Confirm Habitation"));
                    Assert.IsTrue(element.Text.Contains("Confirm Flights"));
                    Assert.IsTrue(element.Text.Contains("+ Invitation"));
                }
            }
        }

        [Test]
        public void TestArrangeBt_x_RejectBt()
        {
            IWebElement Habitation = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-15']");
            Browser.ClickOnWebElement(Habitation);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Habitation-create']");

            IWebElement RejectBTBTM = modalWindow.FindElement(By.XPath("//a[@id='RejectBTBTM']"));
            Browser.ClickOnWebElement(RejectBTBTM);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement RejectComment = Browser.FindElementByXPath("//textarea[@id='RejectComment']");
            Browser.SendKeysTo(RejectComment, "Test Comment", false);
            IWebElement RejectBTButton = Browser.FindElementByXPath("//button[@id='RejectBTButton']");
            Browser.ClickOnWebElement(RejectBTButton);

            Thread.Sleep(Timings.Default_ms*15);
            empList = Tools.GetAllTableData("//tr[contains(@class, 'zebra')]",Browser.webDriver);
            foreach (IWebElement item in empList)
            {
                if (item.Text.Contains("aada"))
                {
                    IWebElement element = item;

                    Assert.IsTrue(element.Text.Contains("?"));
                }
            }
        }


        [Test]
        [ExpectedException(typeof(NoSuchElementException))]
        public void TestDeleteBt_fcar()
        {
            IWebElement HabitationDelete = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-68']");
            Browser.ClickOnWebElement(HabitationDelete);

            Thread.Sleep(Timings.Default_ms*20);

            IWebElement DeleteBTBTM = Browser.FindElementByXPath("//a[@id='DeleteBTBTM']");
            Browser.ClickOnWebElement(DeleteBTBTM);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement modalWindowDeleteConfirm = Browser.FindElementByXPath("//div[@id='DeleteBTBTM-BTM']");
            Thread.Sleep(Timings.Default_ms*30);
            IWebElement DeleteBTButton = modalWindowDeleteConfirm.FindElement(By.XPath("//button[@id='DeleteBTButton']"));
            Browser.ClickOnWebElement(DeleteBTButton);
            Thread.Sleep(Timings.Default_ms*30);

            IWebElement checkDelete = Browser.FindElementByXPath("//tr[@id='Data-68']");
        }


        [Test]
        public void TestConfirmReportedBT_wens()
        {
            Thread.Sleep(Timings.Default_ms * 30);

            IWebElement confirmReportedBt = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-12']");
            Browser.ClickOnWebElement(confirmReportedBt);

            Thread.Sleep(Timings.Default_ms*20);
            IWebElement modalWindow = Browser.FindElementByXPath("//div[@id='Habitation-create']");

            ReadOnlyCollection<IWebElement> checkboxes = Tools.FindAllCheckboxes(modalWindow);
            IWebElement btnSave = modalWindow.FindElement(By.XPath("//button[@id='btnSave']"));

            foreach(IWebElement toCheck in checkboxes)
            {
                if (!(toCheck.GetAttribute("checked") == "true"))
                    Browser.ClickOnWebElement(toCheck);
                Thread.Sleep(Timings.Default_ms * 10); 
            }
            Browser.ClickOnWebElement(btnSave);

            Thread.Sleep(Timings.Default_ms*30);

            IWebElement checkReportedBt = Browser.FindElementByXPath("//a[@id='Habitation'][@data-updateid='Data-8']");
            Thread.Sleep(Timings.Default_ms * 30);

            IWebElement checkboxToReport = Browser.FindElementByXPath("//input[@id='selectedConfirmedBTs'][@type='checkbox']");

            Assert.IsTrue(!(checkboxToReport.GetAttribute("checked") == "true"));
            Browser.ClickOnWebElement(checkboxToReport);
            Thread.Sleep(Timings.Default_ms * 10);

            IWebElement globalSaveBtn = Browser.FindElementByXPath("//a[@id='ReportBT']");
            Browser.ClickOnWebElement(globalSaveBtn);

            Thread.Sleep(Timings.Default_ms*30);

            //check if bt reported

            IWebElement reported = Browser.FindElementByXPath("//tbody[@id='tbodyBts']/tr[@id='Data-8']");
            Thread.Sleep(Timings.Default_ms * 30);

            ReadOnlyCollection<IWebElement> cells = Browser.FindElementsByXPath("//tbody[@id='tbodyBts']/tr[@id='Data-12']/td");
            Thread.Sleep(Timings.Default_ms * 30);

            Assert.AreEqual(cells[3].Text, "");
            Thread.Sleep(Timings.Default_ms * 30);

            Assert.AreNotEqual(cells[4].Text,  "");
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
