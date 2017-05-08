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
    public class SelectEmployeeByWeekTest
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
        public void CustomSelectEmployeeByWeek_NullList_EmptyList()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel>();

            //Act
            var result = helper.CustomSelectEmployeeByWeek(emp, 1, 2014);
            //Assert
            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void CustomSelectEmployeeByWeek_NotEmptyListAndWrongWeek_EmptyList()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>()},

            };
            //Act
            var result = helper.CustomSelectEmployeeByWeek(emp, 5,2014);
            //Assert

            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void CustomSelectEmployeeByWeek_NotEmptyListAndWeek_List()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,01), To = new DateTime(2014,01,05), Hours = 0, WeekNumber = 1},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,08), To = new DateTime(2014,01,15), Hours = 0, WeekNumber = 2},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,07), Hours = 0, WeekNumber = 1}

                }
                },
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 2 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 3},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 4 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 5},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }}

            };

            //Act
            var result = helper.CustomSelectEmployeeByWeek(emp, 1,2014);
            //Assert
            Assert.AreEqual(3, result.ToList().Count);
        }
    }
}
