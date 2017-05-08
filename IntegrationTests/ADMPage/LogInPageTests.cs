using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWrapper;

namespace IntegrationTests.ADMPage
{
    [TestFixture]
    class LogInPageTests
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
            Assert.AreEqual(Browser.Title, "Log in");
            Assert.AreEqual(Browser.GetText("/html/body/header/div/div[2]/section/ul/li/a").ToString(), "Log in");
        }


        [Test]
        [TestCase("", "", false)]
        [TestCase("lgon", "corsad", true)]
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
                Assert.That(Browser.IsAt(baseURL));

            }
            else
            {
                Assert.That(Browser.Url, Is.EqualTo(baseURL + "Account/Login?ReturnUrl=%2f"));
                //Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/div/ul/li"), Is.EqualTo("The user name or password provided is incorrect."));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[1]/span"), Is.EqualTo("The User name field is required."));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/fieldset/ol/li[2]/span"), Is.EqualTo("The Password field is required."));
                Assert.AreEqual(Browser.Title, "Log in");
            }
        }


        [Test]
        [TestCase("lgon", "123456", true, false)]
        [TestCase("abcd", "corsad", false, true)]
        [TestCase("abcd", "123456", false, false)]
        [TestCase("lgon", "corsad", true, true)]
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
                Assert.That(Browser.IsAt(baseURL));
            }
            else
            {
                Assert.That(Browser.Url, Is.EqualTo(baseURL + "Account/Login"));
                Assert.That(Browser.GetText("/html/body/div[1]/section/section/form/div/ul/li"), Is.EqualTo("The user name or password provided is incorrect."));
                Assert.AreEqual(Browser.Title, "Log in");
            }
        }
        #endregion

    }
}
