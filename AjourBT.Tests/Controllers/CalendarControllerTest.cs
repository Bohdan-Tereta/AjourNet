using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Domain.Abstract;
using AjourBT.Tests.MockRepository;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using System.Web;
using System.Web.Routing;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class CalendarControllerTest
    {
        Mock<IRepository> mock;
        CalendarController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://localhost/Home/ABMView"));
            context.Setup(c => c.Request).Returns(request.Object);
            controller = new CalendarController(mock.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }

        [SetUp]
        public void SetUpVU()
        {
            mock = Mock_Repository.CreateMock();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://localhost/Home/VUView"));
            context.Setup(c => c.Request).Returns(request.Object);
            controller = new CalendarController(mock.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }

        [Test]
        public void DepartmentDropDownList_Default_AllDepartments()
        {
            //Arrange

            //Act
            SelectList result = controller.DepartmentDropDownList();

            //Assert
            Assert.AreEqual(7, result.Count());
            Assert.AreEqual("RAAA1", result.First().Text);
            Assert.AreEqual("TAAAA", result.Last().Text);

        }

        [Test]
        public void GetHolidaysData_Default_AllUAHolidays()
        {
            //Arrange
            List<DateTime> exHolidaysList = (from holiday in mock.Object.Holidays where (holiday.CountryID == 1) orderby holiday.HolidayDate select holiday.HolidayDate.ToUniversalTime()).ToList();

            //Act
            List<DateTime> actHolidaysList = controller.GetHolidaysData();
            //Assert
            Assert.AreEqual(exHolidaysList, actHolidaysList);
        }

        [Test]
        public void GetPostponedHolidaysData_Default_AllUAPostponedHolidays()
        {
            //Arrange
            List<DateTime> exHolidaysList = (from holiday in mock.Object.Holidays where (holiday.CountryID == 1 && holiday.IsPostponed) orderby holiday.HolidayDate select holiday.HolidayDate.ToUniversalTime()).ToList();

            //Act
            List<DateTime> actHolidaysList = controller.GetPostponedHolidaysData();
            //Assert
            Assert.AreEqual(exHolidaysList, actHolidaysList);
        }

        [Test]
        public void GetCalendar_Null_SelectedDepartmentNull()
        {
            SelectList expDepartmentDropDownList = controller.DepartmentDropDownList();
            //Act 
            var result = controller.GetCalendar(null) as ViewResult;
            SelectList actDepartmentDropDownList = ((ViewResult)result).ViewBag.DepartmentDropDownList as SelectList;

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(expDepartmentDropDownList.ToArray().Length, actDepartmentDropDownList.ToArray().Length);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[0].Text, actDepartmentDropDownList.ToArray()[0].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[3].Text, actDepartmentDropDownList.ToArray()[3].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[6].Text, actDepartmentDropDownList.ToArray()[6].Text);
            Assert.AreEqual(null, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetCalendar_StringEmpty_SelectedDepartmentStringEmpty()
        {
            SelectList expDepartmentDropDownList = controller.DepartmentDropDownList();
            //Act 
            var result = controller.GetCalendar("") as ViewResult;
            SelectList actDepartmentDropDownList = ((ViewResult)result).ViewBag.DepartmentDropDownList as SelectList;

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(expDepartmentDropDownList.ToArray().Length, actDepartmentDropDownList.ToArray().Length);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[1].Text, actDepartmentDropDownList.ToArray()[1].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[2].Text, actDepartmentDropDownList.ToArray()[2].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[5].Text, actDepartmentDropDownList.ToArray()[5].Text);
            Assert.AreEqual("", result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetCalendar_SDDDA_SelectedDepartmentSDDDA()
        {
            SelectList expDepartmentDropDownList = controller.DepartmentDropDownList();
            //Act 
            var result = controller.GetCalendar("SDDDA") as ViewResult;
            SelectList actDepartmentDropDownList = ((ViewResult)result).ViewBag.DepartmentDropDownList as SelectList;

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(expDepartmentDropDownList.ToArray().Length, actDepartmentDropDownList.ToArray().Length);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[1].Text, actDepartmentDropDownList.ToArray()[1].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[2].Text, actDepartmentDropDownList.ToArray()[2].Text);
            Assert.AreEqual(expDepartmentDropDownList.ToArray()[5].Text, actDepartmentDropDownList.ToArray()[5].Text);
            Assert.AreEqual("SDDDA", result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetCalendarData_selectedDepartmentNull_AllEmployeesAbsences()
        {
            //Arrange
            List<DateTime> expHolidaysList = controller.GetHolidaysData();
            string fromDate = "01.01.2010";
            string toDate = "31.12.2016";

            //Act
            var result = controller.GetCalendarData(fromDate, toDate, null) as PartialViewResult;

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsInstanceOf(typeof(List<CalendarRowViewModel>), result.Model);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(List<DateTime>), result.ViewBag.Holidays);
            Assert.AreEqual(expHolidaysList, result.ViewBag.Holidays);

        }

        [Test]
        public void GetCalendarData_selectedDepartmentNullBadDates_AllEmployeesAbsences()
        {
            //Arrange
            List<DateTime> expHolidaysList = controller.GetHolidaysData();
            string fromDate = "41.01.2010";
            string toDate = "50.12.2016";

            //Act
            var result = controller.GetCalendarData(fromDate, toDate, null) as PartialViewResult;

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsInstanceOf(typeof(List<CalendarRowViewModel>), result.Model);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(List<DateTime>), result.ViewBag.Holidays);
            Assert.AreEqual(expHolidaysList, result.ViewBag.Holidays);

        }

        [Test]
        public void GetCalendarData_selectedDepartmentStringEmpty_AllEmployeesAbsences()
        {
            //Arrange
            List<DateTime> expHolidaysList = controller.GetHolidaysData();
            string fromDate = "01.01.2010";
            string toDate = "31.12.2016";
            //Act
            var result = controller.GetCalendarData(fromDate, toDate, "") as PartialViewResult;
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsInstanceOf(typeof(List<CalendarRowViewModel>), result.Model);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(List<DateTime>), result.ViewBag.Holidays);
            Assert.AreEqual(expHolidaysList, result.ViewBag.Holidays);

        }

        [Test]
        public void GetCalendarData_selectedDepartmentSDDDA_SDDDAEmployeesAbsences()
        {
            //Arrange
            List<DateTime> expHolidaysList = controller.GetHolidaysData();
            string fromDate = "01.01.2010";
            string toDate = "31.12.2016";
            //Act
            var result = controller.GetCalendarData(fromDate, toDate, "SDDDA") as PartialViewResult;
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsInstanceOf(typeof(List<CalendarRowViewModel>), result.Model);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(List<DateTime>), result.ViewBag.Holidays);
            Assert.AreEqual(expHolidaysList, result.ViewBag.Holidays);

        }

        [Test]
        public void GetCalendarData_UrlReffererreVUView_GetCalendarDataVU()
        {
            //Arrange
            SetUpVU();
            string fromDate = "01.01.2010";
            string toDate = "31.12.2016";

            //Act
            var result = controller.GetCalendarData(fromDate, toDate, "") as PartialViewResult;

            //Assert
            Assert.AreEqual("GetCalendarDataVU", result.ViewName);
        }

        #region SearchEmployeeData
        [Test]
        public void SearchEmployeeData_SelectedDepartmentNull_AllEmployeesWithCalendarItems()
        {
            //Arrange

            //Act
            List<Employee> searchList = controller.SearchEmployeeData(null);
            //Assert
            Assert.AreEqual(23, searchList.Count);
            Assert.AreEqual("Daolson", searchList.First().LastName);
            Assert.AreEqual("Zamorrison", searchList.Last().LastName);

        }
        [Test]
        public void SearchEmployeeData_SelectedDepartmentStringEmpty_AllEmployeesWithCalendarItems()
        {
            //Arrange

            //Act
            List<Employee> searchList = controller.SearchEmployeeData("");
            //Assert
            Assert.AreEqual(23, searchList.Count);
            Assert.AreEqual("Daolson", searchList.First().LastName);
            Assert.AreEqual("Zamorrison", searchList.Last().LastName);

        }
        [Test]
        public void SearchEmployeeData_SelectedDepartmentSDDDA_SDDDAEmployees()
        {
            //Arrange

            //Act
            List<Employee> searchList = controller.SearchEmployeeData("SDDDA");
            //Assert
            Assert.AreEqual(4, searchList.Count);
            Assert.AreEqual("Kowwood", searchList.First().LastName);
            Assert.AreEqual("Test", searchList.Last().LastName);


        }
        #endregion

        #region GetCalendarRowData
        [Test]
        public void GetCalendarRowData_AllEmployees_NotEmptyListCalendarRowData()
        {
            //Arrange
            List<Employee> empList = controller.SearchEmployeeData("");
            DateTime fromDate = new DateTime(2010, 01, 01);
            DateTime toDate = new DateTime(2016, 12, 31);

            //Act
            List<CalendarRowViewModel> calendarDataList = controller.GetCalendarRowData(empList, fromDate, toDate);

            //Assert
            Assert.AreEqual(24, calendarDataList.Count);
            Assert.AreEqual("5", calendarDataList.First().id);
            Assert.AreEqual(new DateTime(2014, 05, 09).ToUniversalTime(), calendarDataList.First().values[0].from);
            //Assert.AreEqual(new DateTime(2014, 05, 09), calendarDataList.First().values[0].from);
            Assert.AreEqual(new DateTime(2014, 05, 09).ToUniversalTime(), calendarDataList.First().values[0].to); 
            //Assert.AreEqual(new DateTime(2014, 05, 09), calendarDataList.First().values[0].to);

            Assert.AreEqual("fake_row", calendarDataList.Last().id);
            Assert.AreEqual(fromDate.ToUniversalTime(), calendarDataList.Last().values[0].from);
            Assert.AreEqual(fromDate.ToUniversalTime().ToUniversalTime(), calendarDataList.Last().values[0].to);   

        }

        [Test]
        public void GetCalendarRowData_RAAA1Employees_NotEmptyListCalendarRowData()
        {
            //Arrange
            List<Employee> empList = controller.SearchEmployeeData("RAAA4");
            DateTime fromDate = new DateTime(2010, 01, 01);
            DateTime toDate = new DateTime(2016, 12, 31);
            //Act
            List<CalendarRowViewModel> calendarDataList = controller.GetCalendarRowData(empList, fromDate, toDate);
            //Assert
            Assert.AreEqual(2, calendarDataList.Count);
            Assert.AreEqual("5", calendarDataList.First().id);

        }

        [Test]
        public void GetCalendarRowData_RAAA1EmployeesWithoutCalendarItems_EmptyListCalendarRowData()
        {
            //Arrange
            List<Employee> empList = controller.SearchEmployeeData("RAAA5");
            DateTime fromDate = new DateTime(2010, 01, 01);
            DateTime toDate = new DateTime(2016, 12, 31);
            //Act
            List<CalendarRowViewModel> calendarDataList = controller.GetCalendarRowData(empList, fromDate, toDate);
            //Assert
            Assert.AreEqual(1, calendarDataList.Count);

        }

        [Test]
        public void GetCalendarRowData_CalendarFromDateLessThanFromDate_CuttedCalendarItem()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);

            //Act
            List<CalendarRowViewModel> result = controller.GetCalendarRowData(empList, emp.CalendarItems.FirstOrDefault().From.AddDays(1), emp.CalendarItems.FirstOrDefault().To);

            //Assert
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].from, emp.CalendarItems.FirstOrDefault().From.AddDays(1).ToUniversalTime());
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].to, emp.CalendarItems.FirstOrDefault().To.ToUniversalTime());

        }

        [Test]
        public void GetCalendarRowData_CalendarToDateGreaterThanFromDate_CuttedCalendarItem()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);

            //Act
            List<CalendarRowViewModel> result = controller.GetCalendarRowData(empList, emp.CalendarItems.FirstOrDefault().From, emp.CalendarItems.FirstOrDefault().To.AddDays(-1));

            //Assert
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].from, emp.CalendarItems.FirstOrDefault().From.ToUniversalTime());
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].to, emp.CalendarItems.FirstOrDefault().To.AddDays(-1).ToUniversalTime());

        }

        [Test]
        public void GetCalendarRowData_CalendarFromAndToDateDifferentThanFromAndToDates_CuttedCalendarItem()
        {
            //Arrange
            Employee emp = mock.Object.Employees.FirstOrDefault();
            List<Employee> empList = new List<Employee>();
            empList.Add(emp);

            //Act
            List<CalendarRowViewModel> result = controller.GetCalendarRowData(empList, emp.CalendarItems.FirstOrDefault().From.AddDays(1), emp.CalendarItems.FirstOrDefault().To.AddDays(-1));

            //Assert
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].from, emp.CalendarItems.FirstOrDefault().From.AddDays(1).ToUniversalTime());
            Assert.AreEqual(result.ToArray()[0].values.ToArray()[0].to, emp.CalendarItems.FirstOrDefault().To.AddDays(-1).ToUniversalTime());

        }

        #endregion

        #region InsertFakeEmployee

        [Test]
        public void InsertFakeEmployee_EmptyList_FakeEmployee()
        {
            //Arrange
            List<CalendarRowViewModel> testList = new List<CalendarRowViewModel>();
            //Act
            var result = controller.InsertFakeEmployee(testList, new DateTime(), new DateTime());
            //Assert
            Assert.AreEqual(result.Count, 1);
            Assert.IsInstanceOf(typeof(List<CalendarRowViewModel>), result);
        }

        [Test]
        public void InsertFakeEmployee_NotEmptyList_ListCountIncreased()
        {
            //Arrange
            List<Employee> empList = controller.SearchEmployeeData("RAAA5");
            DateTime fromDate = new DateTime(2010, 01, 01);
            DateTime toDate = new DateTime(2016, 12, 31);
            //Act
            var dataList = controller.GetCalendarRowData(empList, fromDate, toDate);
            var result = controller.InsertFakeEmployee(dataList, new DateTime(), new DateTime());
            //Assert
            Assert.AreEqual("fake_row", result.Last().id);
            Assert.AreEqual(" ", result.Last().name);
            Assert.AreEqual(" ", result.Last().desc);
            Assert.AreEqual(2, result.Last().values.Count);
            Assert.AreEqual(dataList.Count, result.Count);

        }

        #endregion

        #region printCalendarToPdf

        [Test]
        public void PrintCalendarToPdf_EmptyDates_FileReturned()
        {
            //Arrange
            string from = "";
            string to = "";

            //Act
            var result = controller.printCalendarToPdf(from, to, "");

            //Assert        
            Assert.AreEqual(typeof(FileContentResult), result.GetType());
        }

        [Test]
        public void PrintCalendarToPdf_ProperDates_FileReturned()
        {
            //Arrange
            string from = "01.01.2012";
            string to = "01.02.2012";

            //Act
            var result = controller.printCalendarToPdf(from, to, ""); 

            //Assert        
            Assert.AreEqual(typeof(FileContentResult), result.GetType());
        }

        [Test]
        public void PrintCalendarToPdf_WrongFromDate_FileReturned()
        {
            //Arrange
            string from = "01a.01.2012";
            string to = "01.02.2012";

            //Act
            var result = controller.printCalendarToPdf(from, to, "");

            //Assert        
            Assert.AreEqual(typeof(FileContentResult), result.GetType());
        }

        [Test]
        public void PrintCalendarToPdf_WrongToDate_FileReturned()
        {
            //Arrange
            string from = "01.01.2012";
            string to = "01a.02.2012";

            //Act
            var result = controller.printCalendarToPdf(from, to, "");

            //Assert        
            Assert.AreEqual(typeof(FileContentResult), result.GetType());
        }

        [Test]
        public void PrintCalendarToPdf_FromIsGreater_FileReturned()
        {
            //Arrange
            string from = "02.02.2012";
            string to = "01.02.2012";

            //Act
            var result = controller.printCalendarToPdf(from, to, "");

            //Assert        
            Assert.AreEqual(typeof(FileContentResult), result.GetType());
        }

        #endregion
    }
}
