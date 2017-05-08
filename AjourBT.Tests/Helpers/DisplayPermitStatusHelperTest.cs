using System;
using NUnit.Framework;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Web.Mvc;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using System.Linq;
using AjourBT.Helpers;
using AjourBT.Domain.Infrastructure;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class DisplayPermitStatusHelperTest
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
            mock = Mock_Repository.CreateMock();

        }

        [Test]
        public void GetLastBTForEmployee_EmployeeNull_null()
        {
            //Arrange
            Employee employee = null;

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeBusinessTripsNull_null()
        {
            //Arrange
            Employee employee = new Employee { BusinessTrips = null };

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeePermitNull_null()
        {
            //Arrange
            Employee employee = new Employee { Permit = null };

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeePermitStartDateNull_null()
        {
            //Arrange
            Employee employee = new Employee { Permit = new Permit { StartDate = null } };

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeHasLastBT_ValidDate()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(new DateTime (2013, 10, 05), lastBTEndDate.Value.Date);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeHasNoLastBTWrongDateEqualToDateTimeNow_null()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault().EndDate = DateTime.Now.ToLocalTimeAzure();
            BusinessTrip lastBTwithWrongStatus = employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault();
            employee.BusinessTrips.Clear();
            employee.BusinessTrips.Add(lastBTwithWrongStatus);
            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeHasNoLastBTWrongDateLesserThanLastPermitStartDate_null()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault().StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1200);
            employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault().EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1000);
            BusinessTrip lastBTwithWrongStatus = employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault();
            employee.BusinessTrips.Clear();
            employee.BusinessTrips.Add(lastBTwithWrongStatus);
            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeHasNoLastBTWrongBTStatus_null()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault().Status = BTStatus.Cancelled;
            BusinessTrip lastBTwithWrongStatus = employee.BusinessTrips.Where(b => b.BusinessTripID == 27).FirstOrDefault();
            employee.BusinessTrips.Clear();
            employee.BusinessTrips.Add(lastBTwithWrongStatus);
            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetLastBTForEmployee_EmployeeHasNoLastBT_null()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 6).FirstOrDefault();

            //Act
            DateTime? lastBTEndDate = DisplayPermitStatusHelper.GetLastBTForEmployee(employee);

            //Assert
            Assert.AreEqual(null, lastBTEndDate);
        }

        [Test]
        public void GetStartingDateTimePointForPermitExpiration_GetLastBTForEmployeeNull_PermitNull_DateTimeNow()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 6).FirstOrDefault();

            //Act
            DateTime startingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(employee);

            //Assert
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, startingPoint.Date);
        }

        [Test]
        public void GetStartingDateTimePointForPermitExpiration_GetLastBTForEmployeeNull_PermitNotNullPermitStartDateNull()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 6).FirstOrDefault();
            Permit permit = new Permit { StartDate = null, EmployeeID = 6 };
            employee.Permit = permit;

            //Act
            DateTime startingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(employee);

            //Assert
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, startingPoint.Date);
        }

        [Test]
        public void GetStartingDateTimePointForPermitExpiration_GetLastBTForEmployeeNull_PermitNotNullPermitStartDateNul()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 6).FirstOrDefault();
            Permit permit = new Permit { StartDate = null, EmployeeID = 6 };
            employee.Permit = permit;

            //Act
            DateTime startingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(employee);

            //Assert
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, startingPoint.Date);
        }

        [Test]
        public void GetStartingDateTimePointForPermitExpiration_EmployeeNull_DateTimeNow()
        {
            //Arrange
            Employee employee = null;

            //Act
            DateTime startingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(employee);

            //Assert
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, startingPoint.Date);
        }

        [Test]
        public void GetStartingDateTimePointForPermitExpiration_GetLastBTForEmployeeNotNull_LastBTDate()
        {
            //Arrange
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();

            //Act
            DateTime startingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(employee);

            //Assert
            Assert.AreEqual(new DateTime(2013, 10, 05), startingPoint.Date);
        }

        [Test]
        [TestCase(59, Result = true)]
        [TestCase(60, Result = false)]
        [TestCase(61, Result = false)]
        public bool IsLessThan60Days_ValidDates_Validresult(int DaysToAdd)
        {
            //Arrange
            DateTime dateNow = DateTime.Now.ToLocalTimeAzure();

            //Act
            bool result = DisplayPermitStatusHelper.IsLessThan60Days(dateNow.AddDays(-DaysToAdd));

            //Assert
            return result;
        }

        [Test]
        [TestCase(59, Result = false)]
        [TestCase(60, Result = true)]
        [TestCase(61, Result = true)]
        [TestCase(89, Result = true)]
        [TestCase(90, Result = false)]
        [TestCase(91, Result = false)]
        public bool IsToLessThan90Days_ValidDates_Validresult(int DaysToAdd)
        {
            //Arrange
            DateTime dateNow = DateTime.Now.ToLocalTimeAzure();

            //Act
            bool result = DisplayPermitStatusHelper.Is60ToLessThan90Days(dateNow.AddDays(-DaysToAdd));

            //Assert
            return result;
        }

        [Test]
        [TestCase(89, Result = false)]
        [TestCase(90, Result = true)]
        [TestCase(91, Result = true)]
        public bool Is90DaysAndAbove_ValidDates_Validresult(int DaysToAdd)
        {
            //Arrange
            DateTime dateNow = DateTime.Now.ToLocalTimeAzure();

            //Act
            bool result = DisplayPermitStatusHelper.Is90DaysAndAbove(dateNow.AddDays(-DaysToAdd));

            //Assert
            return result;
        }

        [Test]
        public void CustomDisplayPermitStatus_EmployeeNull_EmptyMVCHtmlString()
        {
            //Arrange
            Employee employee = null;
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual(new MvcHtmlString("").ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_PermitNull_EmptyMVCHtmlString()
        {
            //Arrange
            Employee employee = new Employee();
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual(new MvcHtmlString("").ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_PermitEndDateNull_EmptyMVCHtmlString()
        {
            //Arrange
            Employee employee = new Employee { Permit = new Permit() };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual(new MvcHtmlString("").ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_CancelRequestDateNotNull_ProperMVCHtmlStringColorRed()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40), CancelRequestDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40) } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual(String.Format("<span title=\"Requested to be cancelled\" style=\"color: red\">{0:dd'.'MM'.'yyyy}</span>", employee.Permit.CancelRequestDate).ToString(), result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_ProlongRequestDateNotNull_ProperMVCHtmlStringColorGreen()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40), ProlongRequestDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40) } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual(String.Format("<span title=\"Requested to be prolonged\" style=\"color: green\">{0:dd'.'MM'.'yyyy}</span>", employee.Permit.ProlongRequestDate), result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_60DaysHaventPassed_EmptyMVCHtmlString()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40) } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("", result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_From60To89DaysHavePassed_ProperMVCHtmlStringBackgroundOrange()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-89), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-145), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-140) } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("<span title=\"Last BT more than 60 days ago\" style=\"background-color: orange\">Contact Gov</span>", result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_MoreThan90DaysHavePassed_ProperMVCHtmlStringBackgroundRed()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-145), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40)} };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("<span title=\"Last BT more than 90 days ago\" style=\"background-color: red; color: white\">Contact Gov</span>", result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_60DaysHaventPassedDueToBT_EmptyMVCHtmlString()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-145), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40)} };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), Status = BTStatus.Confirmed | BTStatus.Reported } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("", result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_From60To89DaysHavePassedDueToBT_ProperMVCHtmlStringBackgroundOrange()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-189), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-189), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-89), Status = BTStatus.Confirmed | BTStatus.Reported } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("<span title=\"Last BT more than 60 days ago\" style=\"background-color: orange\">Contact Gov</span>", result.ToString());
        }

        [Test]
        public void CustomDisplayPermitStatus_MoreThan90DaysHavePassedDueToBT_ProperMVCHtmlStringBackgroundRed()
        {
            //Arrange
            Employee employee = new Employee { EmployeeID = 1, Permit = new Permit { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-145), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(40) } };
            employee.BusinessTrips = new List<BusinessTrip> { new BusinessTrip { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-145), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-140) } };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            //Act
            MvcHtmlString result = helper.CustomDisplayPermitStatus(employee);

            //Assert
            Assert.AreEqual("<span title=\"Last BT more than 90 days ago\" style=\"background-color: red; color: white\">Contact Gov</span>", result.ToString());
        }
    }
}

