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
    class DisplayPermitDataForEMPHelperTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Permit> permits = new List<Permit>
            {
                new Permit{EmployeeID=1},
                new Permit{EmployeeID=2, CancelRequestDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=3, IsKartaPolaka = true},
                new Permit{EmployeeID=4, StartDate = new DateTime(2014,12,20), EndDate = new DateTime(2014,12,20), IsKartaPolaka = true, Number = "15/2014", CancelRequestDate = new DateTime(2014,12,20), ProlongRequestDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=5, EndDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=6, EndDate = new DateTime(2014,12,20), Number = "15/47"},
                new Permit{EmployeeID=7, StartDate = new DateTime(2014,12,20), EndDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=8, ProlongRequestDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=9, StartDate = new DateTime(2014,12,20)},
                new Permit{EmployeeID=10, Number = "15/86"},
                new Permit{EmployeeID=11, StartDate = new DateTime(2014,12,20), Number = "15/86"},
                new Permit{EmployeeID=12, StartDate = new DateTime(2014,12,20), EndDate = new DateTime(2014,12,20), IsKartaPolaka = false, Number = "15/2014", CancelRequestDate = new DateTime(2014,12,20), ProlongRequestDate = new DateTime(2014,12,20)},
            };

            mock.Setup(m => m.Permits).Returns(permits);
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
        public void DisplayPermitDataForEMP_PermitNull_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(null);
            MvcHtmlString expected = new MvcHtmlString("No Permit");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPermitDataForEMP_PermitOnlyWithKartaPolaka_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 3).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("Karta Polaka <br />");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitOnlyWithEndDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 5).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitOnlyWithStartDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 9).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitOnlyWithNumber_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 10).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitWithNumberAndStartDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 11).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitWithNumberAndEndDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 6).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitWithStartAndEndDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 7).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitWithCancelRequestDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 2).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayDisplayPermitDataForEMP_PermitWithProlongRequestDate_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(mock.Object.Permits.Where(v => v.EmployeeID == 8).FirstOrDefault());
            MvcHtmlString expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }


        [Test]
        public void DisplayPermitDataForEMP_PermitWithKartaPolakaAndDates_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Permit permit = mock.Object.Permits.Where(v => v.EmployeeID == 4).FirstOrDefault();

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(permit);
            MvcHtmlString expected = new MvcHtmlString(String.Format("Karta Polaka <br /><customBlueItalic>Dates:</customBlueItalic> 20.12.2014 - 20.12.2014"));

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayPermitDataForEMP_PermitWithAllDates_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Permit permit = mock.Object.Permits.Where(v => v.EmployeeID == 12).FirstOrDefault();

            //Act
            MvcHtmlString result = helper.CustomDisplayPermitDataForEMP(permit);
            MvcHtmlString expected = new MvcHtmlString(String.Format("<customBlueItalic>Dates:</customBlueItalic> 20.12.2014 - 20.12.2014"));

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }


    }
}
