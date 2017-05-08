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
    public class DisplayVisaStartAndDueDatesTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Visa> visas = new List<Visa>
            {
                new Visa { EmployeeID = 1, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 2},
                new Visa { EmployeeID = 2, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(89), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 1, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 2},
                new Visa { EmployeeID = 3, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 10, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 2},
                new Visa { EmployeeID = 4, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 10, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 2},
                new Visa { EmployeeID = 5, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(1), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 90, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0},
                new Visa { EmployeeID = 6, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(91), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 10, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 1},
            };

            mock.Setup(m => m.Visas).Returns(visas);

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
        public void DisplayVisaStartAndDueDates_DueDateNow_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID==4).FirstOrDefault(), "/Visa/Edit/4?searchString= ");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/4?searchString= \" style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">" + "13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_DueDateNowPlus1_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 5).FirstOrDefault(), "/Visa/Edit/5?searchString=des");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/5?searchString=des\" style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 1 days\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(1).ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_DueDateNowPlus90_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 2).FirstOrDefault(), "/Visa/Edit/2?searchString=owef");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/2?searchString=owef\" style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 89 days\">" + "13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(89).ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_DueDateNowPlus89_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault(), "/Visa/Edit/1?searchString=edf");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/1?searchString=edf\" style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 90 days\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(90).ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_DueDateNowMinus1_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault(), "/Visa/Edit/3?searchString=ok");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/3?searchString=ok\" style=\"color: red; font-weight:bold;\" title=\"Visa expired\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(-1).ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_DueDateNowPlus91_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 6).FirstOrDefault(), "/Visa/Edit/6?searchString=ref");
            MvcHtmlString expected = new MvcHtmlString("<a class=\"visaEditDialog\" data-date-format=\"dd.mm.yy\" href=\"/Visa/Edit/6?searchString=ref\">" + "13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(91).ToString("dd'.'MM'.'yyyy") + "</a>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDates_VisaIsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(null, "/Visa/Edit/");
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        #region DisplayVisaStartAndDueDatesWithoutHiperLink
        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_VisaIsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(null);
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNowPlus91_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 6).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(91).ToString("dd'.'MM'.'yyyy"));
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNowMinus1_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(-1).ToString("dd'.'MM'.'yyyy") + "</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNowPlus89_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 90 days\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(90).ToString("dd'.'MM'.'yyyy") + "</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNowPlus90_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 2).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 89 days\">" + "13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(89).ToString("dd'.'MM'.'yyyy") + "</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNowPlus1_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 5).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 1 days\">" + "01.08.2012" + " - " + DateTime.Now.ToLocalTimeAzure().AddDays(1).ToString("dd'.'MM'.'yyyy") + "</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartAndDueDatesWithoutHiperLink_DueDateNow_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartAndDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 4).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">" + "13.02.2012" + " - " + DateTime.Now.ToLocalTimeAzure().ToString("dd'.'MM'.'yyyy") + "</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
        #endregion

        #region DisplayVisasStartDate

        [Test]
        public void DisplayVisaStartDate_VisaIsNull_EmptyStrign()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(null);
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNow_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 4).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">2012.02.13</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNowPlus1_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 5).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 1 days\">2012.08.01</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNowPlus90_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 2).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 89 days\">2012.02.13</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNowPlus89_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 90 days\">2012.08.01</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNowMinus1_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">2012.08.01</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaStartDate_DueDateNowPlus91_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaStartDate(mock.Object.Visas.Where(v => v.EmployeeID == 6).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("2012.02.13");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        #endregion

        #region DisplayVisasDueDate

        [Test]
        public void DisplayVisaDueDate_VisaIsNull_EmptyStrign()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(null);
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNow_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 4).FirstOrDefault());
            string endDate = DateTime.Now.ToLocalTimeAzure().ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires today\">"+endDate+"</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNowPlus1_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 5).FirstOrDefault());
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(1).ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 1 days\">"+endDate+"</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNowPlus90_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 2).FirstOrDefault());
            string dueDate = mock.Object.Visas.Where(e => e.EmployeeID == 2).FirstOrDefault().DueDate.ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 89 days\">"+dueDate+"</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNowPlus89_ProperStringColorOrangeWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault());
            string dueDate = mock.Object.Visas.Where(e => e.EmployeeID == 1).FirstOrDefault().DueDate.ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: orange; font-weight:bold;\" title=\"Visa expires in 90 days\">"+dueDate+"</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNowMinus1_ProperStringColorRedWithTitle()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault());
            string endDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1).ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString("<div style=\"color: red; font-weight:bold;\" title=\"Visa expired\">"+endDate+"</div>");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDueDate_DueDateNowPlus91_ProperStringColorBlack()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaDueDates(mock.Object.Visas.Where(v => v.EmployeeID == 6).FirstOrDefault());
            string dueDate = mock.Object.Visas.Where(e => e.EmployeeID == 6).FirstOrDefault().DueDate.ToString(String.Format("yyyy.MM.dd"));
            MvcHtmlString expected = new MvcHtmlString(dueDate);
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        #endregion
    }
}
