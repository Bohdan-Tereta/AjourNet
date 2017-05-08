using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    class DisplayInsuranceEndDateTest
    { 
 
        Mock<IRepository> mock;
       
        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Insurance> insurances = new List<Insurance>
            {
                new Insurance { EmployeeID = 1, StartDate = new DateTime(2012,08,01), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 90},
                new Insurance { EmployeeID = 2, StartDate = new DateTime(2012,02,14), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(15), Days = 90 },
                new Insurance { EmployeeID = 3, StartDate = new DateTime(2012,08,01), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1), Days = 90},
                new Insurance { EmployeeID = 4, StartDate = new DateTime(2012,02,13), EndDate = DateTime.Now.ToLocalTimeAzure(), Days = 20},
                new Insurance { EmployeeID = 5, StartDate = new DateTime(2012,08,01), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(14), Days = 90},
                new Insurance { EmployeeID = 6, StartDate = new DateTime(2012,02,13), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(91), Days = 20},
            };

            mock.Setup(m => m.Insurances).Returns(insurances);

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
       
        #region DisplayInsuranceEndDate
        [Test]
        public void DisplayInsuranceEndDate_InsuranceIsNull_EmptyStrign()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(null,"");
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayInsuranceEndDate_EndDateNow_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(mock.Object.Insurances.Where(v => v.EmployeeID == 4).FirstOrDefault(), "/Insurance/Edit/4?searchString= ");
            string endDate = DateTime.Now.ToLocalTimeAzure().ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"/Insurance/Edit/4?searchString= \"style=\"color: red;\">"+ DateTime.Now.ToLocalTimeAzure().ToString("dd'.'MM'.'yyyy") +" (20)"+ "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayInsuranceEndDate_EndDateNowPlus14_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(mock.Object.Insurances.Where(v => v.EmployeeID == 5).FirstOrDefault(), "/Insurance/Edit/5?searchString= ");
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(14).ToString("dd'.'MM'.'yyyy");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"/Insurance/Edit/5?searchString= \"style=\"color: red;\">" + endDate +" (90)"+"</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayInsuranceEndDate_EndDateDateNowPlus15_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(mock.Object.Insurances.Where(v => v.EmployeeID == 2).FirstOrDefault(), "/Insurance/Edit/2?searchString= ");
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(15).ToString("dd'.'MM'.'yyyy");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"/Insurance/Edit/2?searchString= \">" + endDate + " (90)" + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
  

        [Test]
        public void DisplayInsuranceEndDate_EndDateNowMinus1_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(mock.Object.Insurances.Where(v => v.EmployeeID == 3).FirstOrDefault(), "/Insurance/Edit/3?searchString= ");
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1).ToString("dd'.'MM'.'yyyy");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"/Insurance/Edit/3?searchString= \"style=\"color: red;\">" + endDate + " (90)" + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayInsuranceEndDate_EndDateNowPlus91_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayInsuranceEndDate(mock.Object.Insurances.Where(v => v.EmployeeID == 6).FirstOrDefault(), "/Insurance/Edit/6?searchString= ");
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(91).ToString("dd'.'MM'.'yyyy");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"insuranceEditDialog\"data-date-format=\"dd.mm.yy\" href=\"/Insurance/Edit/6?searchString= \">" + endDate + " (20)" + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        #endregion
    }
}
