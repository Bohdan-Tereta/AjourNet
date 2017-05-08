using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
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

namespace AjourBT.Tests.Helpers
{
    public class DisplayPassportDataForEMPHelperTest
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
        public void DisplayPassportDataForEMP_PassportNull_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportDataForEMP(null);
            MvcHtmlString expected = new MvcHtmlString("<b><font color=\"Red\">non-available</font></b>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPassportDataForEMP_PassportOnlyWithID_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportDataForEMP(mock.Object.Passports.Where(v => v.EmployeeID == 1).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("valid");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPassportDataForEMP_PassportWithEndDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPassportDataForEMP(mock.Object.Passports.Where(v => v.EmployeeID == 5).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("valid till 20.12.2014");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

    }
}

