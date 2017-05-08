using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using AjourBT.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Tests.MockRepository;
using AjourBT.Domain.Infrastructure;
namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    class PopupVisaAndPermitHelperTest
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
        public void PopupVisaAndPermit_NullEmployee_EmptyString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            
            //Act
            string result = helper.CustomPopupVisaAndPermit(null).ToString();
            
            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeNoVisaNoPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 15).LastOrDefault();

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeVisaAndPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            emp.VisaRegistrationDate = new VisaRegistrationDate { EmployeeID = 15, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" };
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 08, 08), IsKartaPolaka = false, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };
            emp.Visa = new Visa { EmployeeID = 15, VisaType = "D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 20, DaysUsedInPrivateTrips = 21, Entries = 0, EntriesUsedInBT = 5, EntriesUsedInPrivateTrips = 1, PrivateTrips = new List<PrivateTrip>() };


            string visa = String.Format("V: {0} - {1}", emp.Visa.StartDate.ToShortDateString(), emp.Visa.DueDate.ToShortDateString());

            string regDate=""; 
            if(emp.VisaRegistrationDate.RegistrationDate.HasValue)
            {
                regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());
             }
            string permit = String.Format("P: {0} - {1}", emp.Permit.StartDate.Value.ToShortDateString(), emp.Permit.EndDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\n"+visa+"\r\n"+regDate+"\r\n"+permit, result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeKartaPolaka_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 3).LastOrDefault();

            string visa = String.Format("V: {0} - {1}", emp.Visa.StartDate.ToShortDateString(), emp.Visa.DueDate.ToShortDateString());
            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());
            //string permit = String.Format("P: {0} - {1}", emp.Permit.StartDate.Value.ToShortDateString(), emp.Permit.EndDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\n"+visa+"\r\n"+regDate+"\r\nKarta Polaka", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeIsVisaIsRegDateNoPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 13).LastOrDefault();

            string visa = String.Format("V: {0} - {1}", emp.Visa.StartDate.ToShortDateString(), emp.Visa.DueDate.ToShortDateString());
            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());
            
            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\n"+visa + "\r\n" + regDate + "\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeNoVisaNoPermitIsRegDate_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
           // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 6).LastOrDefault();
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            emp.VisaRegistrationDate = new VisaRegistrationDate { EmployeeID = 15, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" };
           // emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", IsKartaPolaka = false, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };
            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n" + regDate + "\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeNoVisaNoRegDateIsPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
           // emp.VisaRegistrationDate = new VisaRegistrationDate { EmployeeID = 15, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" };
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 08, 08), IsKartaPolaka = false, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };


            string permit = String.Format("P: {0} - {1}", emp.Permit.StartDate.Value.ToShortDateString(), emp.Permit.EndDate.Value.ToShortDateString());
                        
            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n" + permit, result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeIsKartaPolakaIsPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 14).LastOrDefault();

            string permit = String.Format("P: {0} - {1}", emp.Permit.StartDate.Value.ToShortDateString(), emp.Permit.EndDate.Value.ToShortDateString());
            
            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n" + permit + "\r\nKarta Polaka", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeOnlyKartaPolaka_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", IsKartaPolaka = true, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\nKarta Polaka", result);
        }


        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeNoVisaIsRegDateIsKarta_FormattedString()
        {
            //Arrange
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            emp.VisaRegistrationDate = new VisaRegistrationDate { EmployeeID = 15, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" };
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013",  IsKartaPolaka = true,  CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
           // var emp = mock.Object.Employees.Where(e => e.EmployeeID == 17).LastOrDefault();

            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n" + regDate + "\r\nKarta Polaka", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeIsVisaIsKarta_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 2).LastOrDefault();

            string visa = String.Format("V: {0} - {1}", emp.Visa.StartDate.ToShortDateString(), emp.Visa.DueDate.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\n"+visa+"\r\nKarta Polaka", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeIsVisaIsRegDateIsKarta_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 3).LastOrDefault();

            string visa = String.Format("V: {0} - {1}", emp.Visa.StartDate.ToShortDateString(), emp.Visa.DueDate.ToShortDateString());
            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\n"+visa +"\r\n"+regDate+"\r\nKarta Polaka", result);
        }

        [Test]
        public void PopupVisaAndPermit_CorrectEmployeeIsPassport_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = mock.Object.Employees.Where(e => e.EmployeeID == 16).LastOrDefault();

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Visa\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_EmptyEmployee_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            Employee emp = new Employee(); 

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_OnlyRegDate_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            emp.VisaRegistrationDate = new VisaRegistrationDate { EmployeeID = 15, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" };

            string regDate = String.Format("Registration Date: {0}", emp.VisaRegistrationDate.RegistrationDate.Value.ToShortDateString());

            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n"+regDate+"\r\nNo Permit", result);
        }

        [Test]
        public void PopupVisaAndPermit_OnlyPermit_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 08, 08), IsKartaPolaka = false, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };

            string permit = String.Format("P: {0} - {1}", emp.Permit.StartDate.Value.ToShortDateString(), emp.Permit.EndDate.Value.ToShortDateString());
            
            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\n"+permit, result);
        }


        [Test]
        public void PopupVisaAndPermit_OnlyKartaPolaka_FormattedString()
        {
            //Arrange
            var vc = new ViewContext();
            vc.HttpContext = new FakeHttpContext();
            HtmlHelper helper = new HtmlHelper(vc, new FakeViewDataContainer());
            var emp = new Employee { EmployeeID = 15, FirstName = "John", LastName = "McLain", DepartmentID = 2, EID = "jmln", DateEmployed = new DateTime(2006, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() };
     
        
            emp.Permit = new Permit { EmployeeID = 15, Number = "01/2013", IsKartaPolaka = true, CancelRequestDate = DateTime.Now.ToLocalTimeAzure() };

            
            //Act
            string result = helper.CustomPopupVisaAndPermit(emp).ToString();

            //Assert
            Assert.AreEqual("No Passport\r\nNo Visa\r\nKarta Polaka", result);
        }
    }
}
