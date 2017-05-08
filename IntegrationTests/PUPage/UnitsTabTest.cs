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
        class UnitTabTest
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

                Browser.ClickOnLink("Units");

                string Units = Browser.FindElementByLinkText("Units").GetCssValue("color");
                Assert.AreEqual("rgba(225, 112, 9, 1)", Units);
            }

            #region Units Tab

            [Test]
            public void TestUnitsPage()
            {
                int unitsCount; 
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCount = dbContext.Units.Count(); 
                }
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[1]/th[1]").Text); 
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[1]/th[2]").Text);
                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCount +1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("Add Unit", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/p/a/span/span").Text);
                Assert.AreEqual("-", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[2]/td[1]/a").Text);
                Assert.AreEqual("Finance Unit", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[6]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[2]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/1"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[6]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/5"));
            }

            [Test]
            public void AddUnit()
            {
                int unitsCountBefore, unitsCountAfter; 
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/p/a/span/span");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Create Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Create Unit']").Text);
                Assert.AreEqual("Add new Unit", Browser.FindElementByXPath("/html/body/div[3]/div[2]/h4").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[1]/label").Text); 
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='ShortTitle']"), "ShortTitle");
                Thread.Sleep(Timings.Default_ms * 5); 
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='Title']"), "Title");
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore + 2, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(1, unitsCountAfter - unitsCountBefore); 
            }

            [Test]
            public void AddUnitAgainWithsameValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/p/a/span/span");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Create Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Create Unit']").Text);
                Assert.AreEqual("Add new Unit", Browser.FindElementByXPath("/html/body/div[3]/div[2]/h4").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='ShortTitle']"), "ShortTitle");
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeys(Browser.FindElementByXPath("//*[@id='Title']"), "Title");
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);


                Assert.AreEqual("Unit with same ShortTitle already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[1]/td[2]/span").Text);
                Assert.AreEqual("Unit with same Title already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/table/tbody/tr[2]/td[2]/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
                Thread.Sleep(Timings.Default_ms * 10); 

                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditUnit()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='ShortTitle']"), "1", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='Title']"), "2", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSaveUnit']");
                Thread.Sleep(Timings.Default_ms * 10);
                             

                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            } 
            
            [Test]
            public void EditUnit_NothingChanged()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("//*[@id='btnSaveUnit']");
                Thread.Sleep(Timings.Default_ms * 10);


                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditUnit_ExistingValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='ShortTitle']"), "-", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='Title']"), "Unknown", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSaveUnit']");
                Thread.Sleep(Timings.Default_ms * 10); 
                Assert.AreEqual("Unit with same ShortTitle already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/table/tbody/tr[1]/td[2]/span").Text);
                Assert.AreEqual("Unit with same Title already exists", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/table/tbody/tr[2]/td[2]/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);             

                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/6"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }

            [Test]
            public void EditUnit_EmptyValues_ValidationError()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("//*[@id='UnitData']/table/tbody/tr[4]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='ShortTitle']"), "", true);
                Thread.Sleep(Timings.Default_ms * 5);
                Browser.SendKeysTo(Browser.FindElementByXPath("//*[@id='Title']"), "", true);
                Thread.Sleep(Timings.Default_ms * 5);

                Browser.ClickByXPath("//*[@id='btnSaveUnit']");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.AreEqual("Please enter ShortTitle", Browser.FindElementByXPath("//*[@id='editUnitForm']/fieldset/table/tbody/tr[1]/td[2]/span/span").Text);
                Assert.AreEqual("Please enter Title", Browser.FindElementByXPath("//*[@id='editUnitForm']/fieldset/table/tbody/tr[2]/td[2]/span/span").Text);
                Browser.ClickByXPath("/html/body/div[3]/div[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='UnitData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                Assert.AreEqual("1", Browser.FindElementByXPath("//*[@id='UnitData']/table/tbody/tr[7]/td[1]/a").Text);
                Assert.AreEqual("2", Browser.FindElementByXPath("//*[@id='UnitData']/table/tbody/tr[7]/td[2]/a").Text);
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='UnitData']/table/tbody/tr[4]/td[1]/a").GetAttribute("href").Contains("/PU/UnitEdit/3"));
                Assert.IsTrue(Browser.FindElementByXPath("//*[@id='UnitData']/table/tbody/tr[4]/td[2]/a").GetAttribute("href").Contains("/PU/UnitEdit/3"));
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            }       

            [Test]
            public void TryDeleteUnit_Cancelled()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[2]");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("/html/body/div[1]/div/div[6]/div/table");
                Assert.AreEqual(unitsCountBefore +1, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter, unitsCountBefore);
            } 

            [Test]
            public void TryDeleteUnit_Deleted()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("/html/body/div[1]/div/div[6]/div/table/tbody/tr[7]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[1]");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='UnitData']/table");
                Assert.AreEqual(unitsCountBefore, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
                }
                Assert.AreEqual(unitsCountAfter + 1, unitsCountBefore);
            }

            [Test]
            public void TryDeleteUnitCanNotDelete()
            {
                int unitsCountBefore, unitsCountAfter;
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountBefore = dbContext.Units.Count();
                }

                Browser.ClickByXPath("//*[@id='UnitData']/table/tbody/tr[2]/td[1]/a");
                Thread.Sleep(Timings.Default_ms * 10);

                Assert.AreEqual("Edit Unit", Browser.FindElementByXPath("//*[@id='ui-dialog-title-Edit Unit']").Text);
                Assert.AreEqual("ShortTitle", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[1]/td[1]/label").Text);
                Assert.AreEqual("Title", Browser.FindElementByXPath("/html/body/div[3]/div[2]/form[1]/fieldset/table/tbody/tr[2]/td[1]/label").Text);

                Browser.ClickByXPath("/html/body/div[3]/div[2]/form[2]/div/button");
                Thread.Sleep(Timings.Default_ms * 10);
                Assert.IsTrue(Browser.FindElementByXPath("/html/body/div[5]/div[1]/span").Text.Contains("Delete"));
                Assert.AreEqual("OK", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[1]").Text);
                Assert.AreEqual("Cancel", Browser.FindElementByXPath("/html/body/div[5]/div[3]/div/button[2]").Text);
                Assert.AreEqual("Can't Delete this unit", Browser.FindElementByXPath("//*[@id='deleteUnit-Confirm']/h4").Text);
                Assert.AreEqual("Please delete all associated data first", Browser.FindElementByXPath("//*[@id='deleteUnit-Confirm']/p").Text);

                Browser.ClickByXPath("/html/body/div[5]/div[3]/div/button[1]/span");
                Thread.Sleep(Timings.Default_ms * 10);

                IWebElement positionsTable = Browser.FindElementByXPath("//*[@id='UnitData']/table");
                Assert.AreEqual(unitsCountBefore + 1, positionsTable.FindElements(By.TagName("tr")).Count());
                using (AjourBTForTestContext dbContext = new AjourBTForTestContext())
                {
                    unitsCountAfter = dbContext.Units.Count();
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
