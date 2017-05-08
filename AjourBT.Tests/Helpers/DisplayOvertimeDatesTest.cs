using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using AjourBT.Domain.ViewModels;
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
    [TestFixture]
    public class DisplayOvertimeDatesTest
    {
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

        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Overtime> overtimes = new List<Overtime> 
            { 
               new Overtime{EmployeeID = 1, OvertimeID = 1, Date = new DateTime(2014,05,05), DayOff = true, Type = OvertimeType.Paid, ReclaimDate = new DateTime(2014,06,06)},
               new Overtime{EmployeeID = 2, OvertimeID = 2, Date = new DateTime(2014,03,03), DayOff = false,Type = OvertimeType.Private},
               new Overtime{EmployeeID = 3, OvertimeID = 3, Date = new DateTime(2014,11,11), DayOff = true, Type = OvertimeType.Overtime}
                
            };

            mock.Setup(m => m.Overtimes).Returns(overtimes);
        }

        [Test]
        public void DisplayOvertimeDates_OvertimeIsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            var result = helper.CustomDisplayOvertimeDate(null).ToString();

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void DisplayOvertimeDates_OvertimeNotNullDayOffTrueReclaimDateNotNull_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 1).FirstOrDefault();
            string searchString = "";
            string statusHint = String.Format("{0:dd.MM.yyyy}", overtime.ReclaimDate);
            string helperResult = String.Format("<a id=overtimeLink href=/Overtime/EditOvertime/{0}?searchString={1} <strike style=\"color:red\"><redText title=\"{2}\"> {3:dd'.'MM'.'yyyy} </redText></strike> </a>", overtime.OvertimeID, searchString, statusHint, overtime.Date);

            //Act
            var result = helper.CustomDisplayOvertimeDate(overtime).ToString();

            //Assert
            Assert.AreEqual(helperResult, result);
        }

        [Test]
        public void DisplayOvertimeDates_OvertimeNotNullFalseDayOff_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 2).FirstOrDefault();
            string searchString = "";
            string helperResult = String.Format("<a id=overtimeLink href=/Overtime/EditOvertime/{0}?searchString={1} </a> {2:dd'.'MM'.'yyyy}", overtime.OvertimeID, searchString, overtime.Date);

            //Act
            var result = helper.CustomDisplayOvertimeDate(overtime).ToString();

            //Assert
            Assert.AreEqual(helperResult, result);
        }

        [Test]
        public void DisplayOvertimeDates_OvretimeNotNullDayOffTrueReclaimDateNull_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault();
            string searchString = "";
            string helperResult = String.Format("<a id=overtimeLink href=/Overtime/EditOvertime/{0}?searchString={1} </a> {2:dd'.'MM'.'yyyy}", overtime.OvertimeID, searchString, overtime.Date);

            //Act
            var result = helper.CustomDisplayOvertimeDate(overtime).ToString();

            //Assert
            Assert.AreEqual(helperResult, result);
        }

        [Test]
        public void CustomDisplayAbsenceDateForEMP_AbsenceDataNull_emptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            string helperResult = String.Format("");

            //Act
            var result = helper.CustomDisplayAbsenceDateForEMP(null).ToString();

            //Assert
            Assert.AreEqual("",result);
        }

        [Test]
        public void CustomDisplayAbsenceDateForEMP_AbsenceDataNotNullReclaimDateNull_emptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            AbsenceFactorData data = new AbsenceFactorData { AbsenceFactorDataID = 1, From = new DateTime(2014, 01, 01), To = new DateTime(2014, 02, 02) };

            string helperResult = String.Format("{0:dd'.'MM'.'yyyy}", data.From);

            //Act
            var result = helper.CustomDisplayAbsenceDateForEMP(data).ToString();

            //Assert
            Assert.AreEqual(helperResult, result);
        }

        [Test]
        public void CustomDisplayAbsenceDateForEMP_AbsenceDataNotNullReclaimDateNotNull_emptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            AbsenceFactorData data = new AbsenceFactorData { AbsenceFactorDataID = 1, From = new DateTime(2014, 01, 01), To = new DateTime(2014, 02, 02) , ReclaimDate = new DateTime(2014,03,03)};
            string statusHint = String.Format("{0:dd.MM.yyyy}", data.ReclaimDate);

            string helperResult = String.Format("<a <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, data.From);

            //Act
            var result = helper.CustomDisplayAbsenceDateForEMP(data).ToString();

            //Assert
            Assert.AreEqual(helperResult, result);
        }

        #region CustomDisplayOvertimeDateForVU

        [Test]
        public void CustomDisplayOvertimeDateForVU_overtimeNull_EmptyString()
        {
            //Arrange
            Overtime overtime = null;
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayOvertimeDateForVU(overtime);
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayOvertimeDateForVU_DayOffFalse_MvcString()
        {
            //Arrange
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 2).FirstOrDefault();
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayOvertimeDateForVU(overtime);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<a id=overtimeLink </a> {0:dd'.'MM'.'yyyy}", overtime.Date));

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayOvertimeDateForVU_DayOffTrueReclaimDateNotNull_MvcString()
        {
            //Arrange
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 1).FirstOrDefault();
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            string statusHint = "06.06.2014";
            MvcHtmlString result = helper.CustomDisplayOvertimeDateForVU(overtime);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<a id=overtimeLink <strike style=\"color:red\"><redText title=\"{0}\"> {1:dd'.'MM'.'yyyy} </redText></strike> </a>", statusHint, overtime.Date));

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayOvertimeDateForVU_DayOffTrueReclaimDateNull_MvcString()
        {
            //Arrange
            Overtime overtime = mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault();
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayOvertimeDateForVU(overtime);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<a id=overtimeLink </a> {0:dd'.'MM'.'yyyy}", overtime.Date));

            //Assert
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        #endregion

    }
}
