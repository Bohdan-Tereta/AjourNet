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
using AjourBT.Domain.Entities;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.ViewModels;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomDisplayNameOfEmployeeTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IRepository>();

            List<Employee> employees = new List<Employee>
            {
                new Employee{ EmployeeID = 1, DateDismissed = new DateTime(2014,01,01), FirstName = "Coco", LastName = "Jambo"},
                new Employee{ EmployeeID = 2, DateDismissed = null, IsManager = true, FirstName = "Hlopec", LastName = "Goroshok"},
                new Employee{ EmployeeID = 3, DateDismissed = null, IsManager = false, FirstName = "Yunak", LastName = "Andrievsky"}
            };

           mock.Setup(m => m.Employees).Returns(employees);
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
        public void CustomDisplayNameOfEmployee_nullEmployee_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());

            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployee(null);

            //Assert
            Assert.AreEqual("", result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployee_CorrectEmployee_DateDismissedNotNull_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault();

            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployee(emp);

            //Assert
            Assert.AreEqual(String.Format("<a><strike>{0} {1}</strike> </a> <br> <span id='i2'>{2}</span>", emp.LastName, emp.FirstName, emp.DateDismissed.Value.ToShortDateString()), result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployee_CorrectEmployee_DateDismissedNull_IsManager_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = mock.Object.Employees.Where(e => e.EmployeeID == 2).FirstOrDefault();

            //Act

            MvcHtmlString result = helper.CustomDisplayNameOfEmployee(emp);

            //Assert
            Assert.AreEqual(String.Format("<a><b>{0} {1}</b></a>", emp.LastName, emp.FirstName), result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployee_CorrectEmployee_DateDismissedNull_NotManager_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = mock.Object.Employees.Where(e => e.EmployeeID == 3).FirstOrDefault();
            
            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployee(emp);

            //Assert
            Assert.AreEqual(String.Format("<a><m>{0} {1}</m></a>", emp.LastName, emp.FirstName), result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployeeViewModelForVU_employeeModelNull_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            MvcHtmlString daysUsedInBT = new MvcHtmlString("");

            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployeeViewModelForVU(null, daysUsedInBT);

            //Assert
            Assert.AreEqual("", result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployeeViewModelForVU_DateDismissedNotNullAndNotEmpty_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            EmployeeViewModelForVU employeeModel = new EmployeeViewModelForVU{DateDismissed = "2014.01.01", LastName = "Gogol", FirstName = "Stepan"};
            MvcHtmlString hint = new MvcHtmlString("45");


            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployeeViewModelForVU(employeeModel, hint);

            //Assert
            Assert.AreEqual(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <strike>{1} {2}</strike></a> <br> <span id='i2'>{3}</span>", hint, employeeModel.LastName, employeeModel.FirstName, employeeModel.DateDismissed),result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployeeViewModelForVU_DateDismissedEmptyIsManager_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            EmployeeViewModelForVU employeeModel = new EmployeeViewModelForVU { DateDismissed = "", LastName = "Kokarda", FirstName = "Makar", IsManager = true };
            MvcHtmlString hint = new MvcHtmlString("180");

            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployeeViewModelForVU(employeeModel, hint);

            //Assert
            Assert.AreEqual(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <b>{1} {2}</b> </a>", hint, employeeModel.LastName, employeeModel.FirstName), result.ToString());
        }

        [Test]
        public void CustomDisplayNameOfEmployeeViewModelForVU_DateDismissedEmptyIsNotManager_CorrectString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            EmployeeViewModelForVU employeeModel = new EmployeeViewModelForVU { DateDismissed = "", LastName = "Goverla", FirstName = "Olena", IsManager = false };
            MvcHtmlString hint = new MvcHtmlString("100");

            //Act
            MvcHtmlString result = helper.CustomDisplayNameOfEmployeeViewModelForVU(employeeModel, hint);

            //Assert
            Assert.AreEqual(String.Format("<a id=\"NumberDaysInBt\" class=\"DaysInBt\" title=\"{0}\"> <msort>{1} {2}</msort> </a>", hint, employeeModel.LastName, employeeModel.FirstName), result.ToString());
        }
    }
}
