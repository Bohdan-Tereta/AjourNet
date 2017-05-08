using AjourBT.Domain.Abstract;
using AjourBT.Domain.ViewModels;
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
    public class SortingFactorDatesTest
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
        }

        [Test]
        public void CustomSortingFactorDataByStartDate_EmptyList_EmptyList()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<FactorData> fData = new List<FactorData>();
            //Act

            var result = helper.CustomSortingFactorDataByStartDate(fData);
            //Assert
            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void CustomSortingFactorDataByStartDate_NotEmptyList_List()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<FactorData> fData = new List<FactorData>
            {
                new FactorData{ Factor = Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,12,31), To = new DateTime(2014,12,31),WeekNumber = 1},
                new FactorData{ Factor = Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,10), To = new DateTime(2014,02,12),WeekNumber = 1},
                new FactorData{ Factor = Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,09), To = new DateTime(2014,03,15),WeekNumber = 1},
                new FactorData{ Factor = Domain.Entities.CalendarItemType.SickAbsence, From = new DateTime(2014,01,02), To = new DateTime(2014,12,31),WeekNumber = 1},
                new FactorData{ Factor = Domain.Entities.CalendarItemType.UnpaidVacation, From = new DateTime(2014,01,01), To = new DateTime(2014,02,15),WeekNumber = 1},
            };
            //Act

            var result = helper.CustomSortingFactorDataByStartDate(fData);
            //Assert

            Assert.AreEqual(new DateTime(2014, 01, 01), result.ToArray()[0].From);
            Assert.AreEqual(new DateTime(2014, 12, 31), result.ToArray()[4].From);
        }
    }
}
