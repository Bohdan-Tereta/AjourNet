
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
using System.Text;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayVisaRegistrationDateTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<VisaRegistrationDate> visaRegDates = new List<VisaRegistrationDate>
            {
                new VisaRegistrationDate {EmployeeID=1, RegistrationDate=new DateTime(2013,01,01),VisaType=null},
                new VisaRegistrationDate {EmployeeID=2, RegistrationDate=new DateTime(2013,01,01),VisaType="V_C07", RegistrationTime = "12:00", City = "Kyiv", RegistrationNumber = "12c"},   
                new VisaRegistrationDate {EmployeeID=3, RegistrationDate=new DateTime(2013,01,01),VisaType="V_C07"},
                new VisaRegistrationDate {EmployeeID=4, RegistrationDate=new DateTime(2013,01,04),VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=5, RegistrationDate=new DateTime(2016,01,04),VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=6, RegistrationDate = default(DateTime)}
            };

            mock.Setup(m => m.VisaRegistrationDates).Returns(visaRegDates);

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

       #region
        [Test]
        public void CustomDisplayVisaRegistrationDateForEMP_Null_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaRegistrationDateForEMP(null);
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayVisaRegistrationDateForEMP_DateISNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(r => r.EmployeeID == 6).FirstOrDefault();
            MvcHtmlString result = helper.CustomDisplayVisaRegistrationDateForEMP(visaRegDate);
            MvcHtmlString expected = new MvcHtmlString("");
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayVisaRegistrationDateForEMP_VisaRegDateAndOtherFieldsNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaRegistrationDateForEMP((mock.Object.VisaRegistrationDates.Where(r => r.EmployeeID == 1)).FirstOrDefault());
            StringBuilder expectedString = new StringBuilder();
            expectedString.Append("<tr><td rowspan=\"4\"><b>Visa Registration Date</b></td> <td><customBlueItalic>Date: </customBlueItalic> 01.01.2013 </td></tr> <br />");
            expectedString.Append("<tr><td><customBlueItalic>Time:</customBlueItalic> - </td></tr>");
            expectedString.Append("<tr><td><customBlueItalic>City:</customBlueItalic> - </td></tr>");
            expectedString.Append("<tr><td><customBlueItalic>Registration Number:</customBlueItalic> - </td> </tr>");
            expectedString.Append("<tr> <td><br/></td> </tr>");

            MvcHtmlString expected = new MvcHtmlString(expectedString.ToString());
            
            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayVisaRegistrationDateForEMP_VisaRegDateAndOtherFieldsNOTNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaRegistrationDateForEMP((mock.Object.VisaRegistrationDates.Where(r => r.EmployeeID == 2)).FirstOrDefault());
            StringBuilder expectedString = new StringBuilder();
            expectedString.Append("<tr><td rowspan=\"4\"><b>Visa Registration Date</b></td> <td><customBlueItalic>Date: </customBlueItalic> 01.01.2013 </td></tr> <br />");
            expectedString.Append("<tr><td><customBlueItalic>Time:</customBlueItalic> 12:00</td></tr>");
            expectedString.Append("<tr><td><customBlueItalic>City:</customBlueItalic> Kyiv</td></tr>");
            expectedString.Append("<tr><td><customBlueItalic>Registration Number:</customBlueItalic> 12c</td> <td> <br /> </td> </tr>");
            expectedString.Append("<tr> <td><br/></td> </tr>");

            MvcHtmlString expected = new MvcHtmlString(expectedString.ToString());

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }


        #endregion
    }
}
