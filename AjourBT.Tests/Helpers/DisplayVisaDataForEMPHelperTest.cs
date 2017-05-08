using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace AjourBT.Tests.Helpers
{
    class DisplayVisaDataForEMPHelperTest
    {
        Mock<IRepository> mock;
        StringBuilder builder;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();
            builder = new StringBuilder();

            List<Visa> visas = new List<Visa>
            {
                new Visa { EmployeeID = 3, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 19, DaysUsedInPrivateTrips = 1, CorrectionForVisaDays = 0, Entries = 5, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 4, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 10, DaysUsedInPrivateTrips = 11, CorrectionForVisaDays = 0, Entries = 2, EntriesUsedInBT = 1, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 5, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 12, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 2, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 6, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 40, DaysUsedInBT = 2, DaysUsedInPrivateTrips = 11, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 7, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 2, CorrectionForVisaDays = 0, Entries = 2, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 8, VisaType = "C07", StartDate = new DateTime(2012, 02, 13), DueDate = new DateTime(2013, 05, 13), Days = 20, DaysUsedInBT = 5, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4 , CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 9, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 5, CorrectionForVisaDays = 0, DaysUsedInPrivateTrips = 0, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 11, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 5, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 12, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 2, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 13, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 20, DaysUsedInBT = 5, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 180, DaysUsedInBT = 20, DaysUsedInPrivateTrips = 21, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 5, EntriesUsedInPrivateTrips = 1, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
                new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2014,12,20), DueDate = new DateTime(2014,12,20), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, PrivateTrips = new List<PrivateTrip>() },
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
        public void DisplayVisaDataForEMP_VisaNull_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayVisaForEMP(null, 10);
            MvcHtmlString expected = new MvcHtmlString("<tr><td><b>Visa</b></td><td>No Visa</td></tr> <br /> <tr> <td><br/></td> </tr>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DisplayVisaDataForEMP_VisaEntriesMult_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 5).FirstOrDefault();

            builder.AppendFormat("<tr><td rowspan=\"3\"><b>Visa</b></td> <td><customBlueItalic>Type:</customBlueItalic> {0}</td></tr>", visa.VisaType);
            builder.AppendFormat("<tr><td><customBlueItalic>Dates:</customBlueItalic> {0} - {1}</td></tr>", visa.StartDate.Date.ToShortDateString(), visa.DueDate.Date.ToShortDateString());
            builder.AppendFormat("<tr><td><customBlueItalic>Entries: </customBlueItalic> MULT, <customBlueItalic>Days:</customBlueItalic> 20(10)</td></tr> <br />");
            builder.Append(" <tr> <td><br/></td> </tr>");


            //Act
            MvcHtmlString result = helper.CustomDisplayVisaForEMP(visa, 10);
            MvcHtmlString expected = new MvcHtmlString(builder.ToString());

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }


        [Test]
        public void DisplayVisaDataForEMP_VisaEntriesNotMult_ProperString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault();

            builder.AppendFormat("<tr><td rowspan=\"3\"><b>Visa</b></td> <td><customBlueItalic>Type:</customBlueItalic> {0}</td></tr>", visa.VisaType);
            builder.AppendFormat("<tr><td><customBlueItalic>Dates:</customBlueItalic> {0} - {1}</td></tr>", visa.StartDate.Date.ToShortDateString(), visa.DueDate.Date.ToShortDateString());
            builder.AppendFormat("<tr><td><customBlueItalic>Entries: </customBlueItalic> 5(5), <customBlueItalic>Days:</customBlueItalic> 20(12)</td></tr> <br />");
            builder.Append(" <tr> <td><br/></td> </tr>");


            //Act
            MvcHtmlString result = helper.CustomDisplayVisaForEMP(visa, 12);
            MvcHtmlString expected = new MvcHtmlString(builder.ToString());

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
        }


        

    }
}
