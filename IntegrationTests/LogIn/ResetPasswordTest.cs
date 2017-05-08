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

namespace IntegrationTests.LogIn
{
    [TestFixture]
    class DaysFromBTsTabTest
    {
        private string baseURL = "http://localhost:50616/";

        [TestFixtureSetUp]
        public void Setup()
        {
            Browser.webDriver.Manage().Window.Maximize();
            Browser.Goto(baseURL);
        }

        #region Days From BTs Tab

        [Test]
        public void ResetPassword_UserExists_EmailSent()
        {
            Assert.AreEqual(Browser.FindElement(By.Id("forgotPassword"), 5).Text, "Forgot your password?");
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("forgotPassword"), 5)); 
            Thread.Sleep(Timings.Default_ms * 5);
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='ui-dialog-title-resetPwrd']").Text, "Reset password");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='resetPwrd']/h2 ").Text, "ForgotPassword");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='forgotPasswordForm']/fieldset/ol/li/label ").Text, "User Name");
            Browser.SendKeys(Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/ol/li/input"), "lark");
            Thread.Sleep(Timings.Default_ms * 5)  ;
            Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button"); 
            Thread.Sleep(Timings.Default_ms * 5);
            Assert.AreEqual(Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/ol/li/div").Text, "Email with password verification token has been sent.");
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span"); 
            Thread.Sleep(Timings.Default_ms * 5);
        }

        [Test]
        public void ResetPassword_UserDoesNotExist_Warning()
        {
            Assert.AreEqual(Browser.FindElement(By.Id("forgotPassword"), 5).Text, "Forgot your password?");
            Browser.ClickOnWebElement(Browser.FindElement(By.Id("forgotPassword"), 5));
            Thread.Sleep(Timings.Default_ms * 5);
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='ui-dialog-title-resetPwrd']").Text, "Reset password");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='resetPwrd']/h2 ").Text, "ForgotPassword");
            Assert.AreEqual(Browser.FindElementByXPath("//*[@id='forgotPasswordForm']/fieldset/ol/li/label ").Text, "User Name");
            Browser.SendKeys(Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/ol/li/input"), "larklark");
            Thread.Sleep(Timings.Default_ms * 5);
            Browser.ClickByXPath("/html/body/div[3]/div[3]/div/button");
            Thread.Sleep(Timings.Default_ms * 5);
            Assert.AreEqual(Browser.FindElementByXPath("/html/body/div[3]/div[2]/form/fieldset/ol/li/div").Text, "User does not exist.");
            Browser.ClickByXPath("/html/body/div[3]/div[1]/a/span");
            Thread.Sleep(Timings.Default_ms * 5);
        }
                
        #endregion
    }
}
