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

namespace IntegrationTests.PUPage
{

        [TestFixture]
        class PositionTabTest
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
                username = "apat";
                password = "lokmop";
                Browser.Goto(baseURL);

                // Act
                Browser.SendKeysTo("UserName", username, true);
                Browser.SendKeysTo("Password", password, true);
                Browser.ClickByXPath("//input[@type='submit']");
                Browser.ClickOnLink("PU");

                Browser.ClickOnLink("Positions");

                string Positions = Browser.FindElementByLinkText("Positions").GetCssValue("color");
                Assert.AreEqual("rgba(225, 112, 9, 1)", Positions);
            }

            #region Positions Tab

            [Test]
            public void TestPositionsPage()
            {
                int unitsCount; 
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCount = dbContext.Positions.Count(); 
                }
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[1]/th[1]").Text); 
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[1]/th[2]").Text);
                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCount +1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("Add Position", Browser.FindElementByXPath("//*[@id='CreatePosition']/span").Text);
                Assert.AreEqual("Employee", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[2]/td[1]/a").Text);
                Assert.AreEqual("Розробник програмного забезпечення", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[3]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[2]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/1"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[3]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/2"));
            }

            [Test]
            public void AddPosition()
            {
                int unitsCountBefore, unitsCountAfter; 
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='CreatePosition']/span");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Create Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Create Position']").Text);
                Assert.AreEqual("Add new Position", Browser.FindElementByXPath("//*[@id='Create Position']/h4").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[1]/label").Text); 
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='TitleEn']"), "TitleEn");
                Thread.Sleep(Timings.Default_ms * 5); 
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='TitleUk']"), "TitleUk");
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 2, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(1, unitsCountAfter - unitsCountBefore); 
            }

            [Test]
            public void AddPositionAgainWithsameValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='CreatePosition']/span");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Create Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Create Position']").Text);
                Assert.AreEqual("Add new Position", Browser.FindElementByXPath("//*[@id='Create Position']/h4").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='TitleEn']"), "TitleEn");
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='TitleUk']"), "TitleUk");
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);


                Assert.AreEqual("Position with same TitleEn already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[2]/span").Text);
                Assert.AreEqual("Position with same TitleUk already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[2]/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
                Thread.Sleep(Timings.Default_ms * 10); 

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditPosition()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleEn']"), "1", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleUk']"), "2", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSavePosition']");
                Thread.Sleep(Timings.Default_ms * 10);
                             

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            } 
            
            [Test]
            public void EditPosition_NothingChanged()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("//*[@id='btnSavePosition']");
                Thread.Sleep(Timings.Default_ms * 10);


                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditPosition_ExistingValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleEn']"), "Employee", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleUk']"), "Працівник", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSavePosition']");
                Thread.Sleep(Timings.Default_ms * 10); 
                Assert.AreEqual("Position with same TitleEn already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/table/tbody/tr[1]/td[2]/span").Text);
                Assert.AreEqual("Position with same TitleUk already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/table/tbody/tr[2]/td[2]/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);             

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditPosition_EmptyValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleEn']"), "", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='TitleUk']"), "", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSavePosition']");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.AreEqual("Please enter TitleEn", Browser.FindElementByXPath("//*[@id='editPositionForm']/fieldset/table/tbody/tr[1]/td[2]/span/span").Text);
                Assert.AreEqual("Please enter TitleUk", Browser.FindElementByXPath("//*[@id='editPositionForm']/fieldset/table/tbody/tr[2]/td[2]/span/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/PositionEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void TryDeletePosition_Cancelled()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[2]");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore +1, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            } 

            [Test]
            public void TryDeletePosition_Deleted()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[1]");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter + 1, unitsCountBefore);
            }

            [Test]
            public void TryDeletePositionCanNotDelete()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Positions.Count();
                }

                Browser.ClickByXPath("//*[@id='PositionData']/table/tbody/tr[3]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Position", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Position']").Text);
                Assert.AreEqual("TitleEn", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("TitleUk", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Assert.AreEqual("Can't Delete this position", Browser.FindElementByXPath("//*[@id='deletePosition-Confirm']/h4").Text);
                Assert.AreEqual("Please delete all associated data first", Browser.FindElementByXPath("//*[@id='deletePosition-Confirm']/p").Text);

                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[1]/span");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='PositionData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Positions.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
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
