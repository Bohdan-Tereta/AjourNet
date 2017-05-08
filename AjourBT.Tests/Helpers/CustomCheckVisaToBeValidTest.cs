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
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomCheckVisaToBeValidTest
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

            List<Visa> visas = new List<Visa>
            {
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(10).Date, DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(50).Date, Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 },
                new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },
                new Visa { EmployeeID = 4, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50).Date, DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(50).Date, Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },
                new Visa { EmployeeID = 10, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },
                new Visa { EmployeeID = 12, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },
                new Visa { EmployeeID = 13, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 },
                new Visa { EmployeeID = 14, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 }  
            };

            mock.Setup(m => m.Visas).Returns(visas);
        }

        [Test]
        public void CheckVisaToBeValid_NullVisa_NoVisa()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            bool result = helper.CheckVisaToBeValid(null);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckVisaToBeValid_FutureVisaDates_False()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();

            //Act
            bool result = helper.CheckVisaToBeValid(visa);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckVisaToBeValid_OldVisaDates_False()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 2).FirstOrDefault();

            //Act
            bool result = helper.CheckVisaToBeValid(visa);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckVisaToBeValid_validVisa_True()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 4).FirstOrDefault();

            //Act
            bool result = helper.CheckVisaToBeValid(visa);

            //Assert
            Assert.IsTrue(result);
        }


        [Test]
        public void CheckVisaToBeValid_empVisaNull()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = new Employee { EmployeeID = 1};
            string resString = String.Format("<div class=\"littleSrift\">No Visa</div>");

            //Act
            string result = helper.CheckVisaToBeValid(emp, "").ToString();

            //Assert
            Assert.AreEqual(resString, result);
        }

        [Test]
        public void CheckVisaToBeValid_empVisaNotNull_VisaExpired()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = new Employee { Visa = new Visa { DueDate = new DateTime(2014, 01, 01) } };
            string resString = String.Format("<div class=\"littleSrift\">Visa Expired</div>");

            //Act
            string result = helper.CheckVisaToBeValid(emp, "").ToString();

            //Assert
            Assert.AreEqual(resString, result);
        }

        [Test]
        public void CheckVisaToBeValid_empVisaNotNull_VisaNotExpired()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = new Employee { EmployeeID = 1, Visa = new Visa { DueDate = DateTime.Now.AddDays(1)} };
            string searchString = "";
            string resString = String.Format("<a class=\"ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" id=\"AddPrivateTrip\"  data-date-format=\"dd.mm.yy\" href=\"{0}{1}?searchString={2}\"> Add </a>", "/BTM/PrivateTripCreate/", emp.EmployeeID, searchString);

            //Act
            string result = helper.CheckVisaToBeValid(emp, searchString).ToString();

            //Assert
            Assert.AreEqual(resString, result);
        }
    }
}
