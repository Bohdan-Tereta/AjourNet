using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AjourBT.Helpers;

namespace AjourBT.Tests.Helpers
{  
    [TestFixture]
    public class DisplayPassportsActionLinkTest
    {
        Mock<IRepository> mock;
        
        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Passport> passports = new List<Passport>
            {
                new Passport{EmployeeID=1},
                new Passport{EmployeeID=2},
                new Passport{EmployeeID=3},
                new Passport{EmployeeID=4, EndDate = new DateTime(2014,12,20)},
                new Passport{EmployeeID=5, EndDate = new DateTime(2014,12,20)},
                new Passport{EmployeeID=6, EndDate = new DateTime(2014,12,20)},
                new Passport{EmployeeID=7, EndDate = new DateTime(2014,12,20)}
            };

            mock.Setup(m => m.Passports).Returns(passports);
        }

        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }


        [Test]
        public void DisplayPassportsActionLink_PassportNullAndSearchStringNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportsActionLink(null, null);
            MvcHtmlString expected = new MvcHtmlString("");
            
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPassportsActionLink_EmptyPassportAndSearchStringDa_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportsActionLink(mock.Object.Passports.Where(v => v.EmployeeID == 1).FirstOrDefault(), "Da");
            MvcHtmlString expected = new MvcHtmlString("<a id=\"AddDatePassport\" data-date-format=\"dd.mm.yy\" href=\"/BTM/PassportAddDate/1?searchString=Da\">Date</a>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPassportsActionLink_PassportAndEmptySearchString_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportsActionLink(mock.Object.Passports.Where(v => v.EmployeeID == 5).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<a class=\"passportEditDialog\" id=\"passportEdit\" data-date-format=\"dd.mm.yy\" href=\"/BTM/PassportEditDate/5?searchString=\">20.12.2014</a>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
    }
}
