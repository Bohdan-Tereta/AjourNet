using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;


namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class EMPControllerTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
        }

        #region Index
        [Test]
        public void Index_Default_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            string userName = "";
            var view = controller.Index();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(userName, ((ViewResult)view).ViewBag.UserName);
        }


        [Test]
        public void Index_asto_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            string userName = "ascr";
            var view = controller.Index(userName);

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(userName, ((ViewResult)view).ViewBag.UserName);
        }


        #endregion

        #region GetLastBTDataPerEmployee

        [Test]
        public void GetLastBTDataPerEmployee_Default_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            var view = controller.GetLastBTDataPerEmployee();

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
        }



        [Test]
        public void GetLastBTDataPerEmployee_xoko_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "xomi";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 35).FirstOrDefault();

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);
            List<BusinessTrip> bt = (List<BusinessTrip>)((PartialViewResult)view).Model;

            //Assert
            Assert.IsTrue(((PartialViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual(businessTrip, ((PartialViewResult)view).ViewBag.BTsGeneralInformation);

        }

        [Test]
        public void GetLastBTDataPerEmployeeWithoutOrderDates_iwpe_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwpe";

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName) as PartialViewResult;
            var bView = view.Model as List<BusinessTrip>;


            //Assert
            Assert.AreEqual(1, bView.Count);
            Assert.AreEqual(39, bView.ToArray()[0].BusinessTripID);
        }

        [Test]
        public void GetLastBTDataPerEmployee_lala_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "lala";

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);

        }

        [Test]
        public void GetLastBTDataPerEmployee_EmployeeWithoutBTs_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwooo";

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("Empty", ((PartialViewResult)view).ViewName);

        }

        [Test]
        public void GetLastBTDataPerEmployee_EmployeeWithCancelledBT_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tadk";

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("Empty", ((PartialViewResult)view).ViewName);

        }

        [Test]
        public void GetLastBTDataPerEmployee_EmployeeWithPlannedBT_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "ascr";

            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("Empty", ((PartialViewResult)view).ViewName);

        }

        [Test]
        public void GetLastBTDataPerEmployee_NotSameBT_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tedk";

            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 36).FirstOrDefault();
            List<BusinessTrip> bts = mock.Object.BusinessTrips.Where(b => b.OrderStartDate == businessTrip.OrderStartDate && b.OrderEndDate == businessTrip.OrderEndDate).ToList();
            //Act
            var view = controller.GetLastBTDataPerEmployee(userName);
            List<BusinessTrip> bt = (List<BusinessTrip>)((PartialViewResult)view).Model;

            //Assert
            Assert.IsTrue(((PartialViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreNotEqual(bt, bts);
        }

        #endregion

        #region GetVisaDataPerEmployee
        [Test]
        public void GetVisaDataPerEmployee_Default_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            var view = controller.GetVisaDataPerEmployee();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("NoData", ((ViewResult)view).ViewName);
        }

        [Test]
        public void GetVisaDataPerEmployee_Null_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = null;

            //Act
            var view = controller.GetVisaDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("NoData", ((ViewResult)view).ViewName);
        }

        [Test]
        public void GetVisaDataPerEmployee_lala_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "lala";

            //Act
            var view = controller.GetVisaDataPerEmployee(userName);

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("NoData", ((ViewResult)view).ViewName);
        }


        [Test]
        public void GetVisaDataPerEmployee_tepy_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tedk";
            Employee emp = mock.Object.Employees.Where(e => e.EID == userName).FirstOrDefault();

            //Act
            var view = controller.GetVisaDataPerEmployee(userName);
            Employee model = (Employee)view.Model;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(model, ((ViewResult)view).Model);
        }






        #endregion

        #region GetAbsencePerEmp

        [Test]
        public void GetAbsencePerEmp_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            var result = controller.GetAbsencePerEMP() as ViewResult;

            //Assert

            Assert.AreEqual("", result.ViewName);
        }
        #endregion

        #region GetAbsenceDataPerEmp

        [Test]
        public void GetAbsenceDataPerEmp_EmptyDates_EmptyView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "";
            string toDate = "";
            string userName = "";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("~/Views/WTR/GetWTRDataEmpty.cshtml", result.ViewName);
        }

        [Test]
        public void GetAbsenceDataPerEmp_CorrectDatesBadEmpName_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "01.01.2014";
            string toDate = "10.10.2014";
            string userName = "xyz";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetAbsenceDataPerEmp_SicknessNotNull_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "27.02.2014";
            string toDate = "27.03.2014";
            string userName = "siol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(1, SicknessValue.Count);
            Assert.AreEqual(0, PaidVacationValue.Count);
            Assert.AreEqual(0, UnPaidVacationValue.Count);
            Assert.AreEqual(0, OvertimeValue.Count);
            Assert.AreEqual(0, PaidOvertimeValue.Count);
            Assert.AreEqual(0, PrivateOvertimeValue.Count);
            Assert.AreEqual(0, JourneyValue.Count);
        }

        [Test]
        public void GetAbsenceDataPerEmp_VacationNotNullPaidVacation_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "27.03.2014";
            string toDate = "30.03.2014";
            string userName = "pvol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(0, SicknessValue.Count);
            Assert.AreEqual(1, PaidVacationValue.Count);
            Assert.AreEqual(0, UnPaidVacationValue.Count);
            Assert.AreEqual(0, OvertimeValue.Count);
            Assert.AreEqual(0, PaidOvertimeValue.Count);
            Assert.AreEqual(0, PrivateOvertimeValue.Count);
            Assert.AreEqual(0, JourneyValue.Count);
        }

        [Test]
        public void GetAbsenceDataPerEmp_VacationNotNullUnPaidVacation_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "27.04.2014";
            string toDate = "30.04.2014";
            string userName = "uvol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(0, SicknessValue.Count);
            Assert.AreEqual(0, PaidVacationValue.Count);
            Assert.AreEqual(1, UnPaidVacationValue.Count);
            Assert.AreEqual(0, OvertimeValue.Count);
            Assert.AreEqual(0, PaidOvertimeValue.Count);
            Assert.AreEqual(0, PrivateOvertimeValue.Count);
            Assert.AreEqual(0, JourneyValue.Count);
        }

        //[Test]
        //public void GetAbsenceDataPerEmp_OvertimesNotNullOvertime_View()
        //{
        //    //Arrange
        //    EMPController controller = new EMPController(mock.Object);
        //    string fromDate = "27.02.2015";
        //    string toDate = "02.03.2015";
        //    string userName = "ovol";

        //    //Act
        //    var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
        //    var resModel = result.Model as AbsenceViewModelForEMP;

        //    var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
        //    var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
        //    var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
        //    var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
        //    var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
        //    var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
        //    var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

        //    //Assert
        //    Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
        //    Assert.AreEqual(0, SicknessValue.Count);
        //    Assert.AreEqual(0, PaidVacationValue.Count);
        //    Assert.AreEqual(0, UnPaidVacationValue.Count);
        //    Assert.AreEqual(1, OvertimeValue.Count);
        //    Assert.AreEqual(0, PaidOvertimeValue.Count);
        //    Assert.AreEqual(0, PrivateOvertimeValue.Count);
        //    Assert.AreEqual(0, JourneyValue.Count);
        //}

        [Test]
        public void GetAbsenceDataPerEmp_OvertimesNotNullPaidOvertime_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "30.07.2013";
            string toDate = "04.08.2013";
            string userName = "povol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;
            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(0, SicknessValue.Count);
            Assert.AreEqual(0, PaidVacationValue.Count);
            Assert.AreEqual(0, UnPaidVacationValue.Count);
            Assert.AreEqual(1, OvertimeValue.Count);
            Assert.AreEqual(1, PaidOvertimeValue.Count);
            Assert.AreEqual(0, PrivateOvertimeValue.Count);
            Assert.AreEqual(0, JourneyValue.Count);
        }

        [Test]
        public void GetAbsenceDataPerEmp_OvertimesNotNullPrivateOvertime_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "07.06.2013";
            string toDate = "12.06.2013";
            string userName = "prvol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(0, SicknessValue.Count);
            Assert.AreEqual(0, PaidVacationValue.Count);
            Assert.AreEqual(0, UnPaidVacationValue.Count);
            Assert.AreEqual(0, OvertimeValue.Count);
            Assert.AreEqual(0, PaidOvertimeValue.Count);
            Assert.AreEqual(1, PrivateOvertimeValue.Count);
            Assert.AreEqual(0, JourneyValue.Count);
        }

        [Test]
        public void GetAbsenceDataPerEmp_AllValid_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "01.01.2008";
            string toDate = "31.12.2018";
            string userName = "andl";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            var SicknessValue = resModel.FactorDetails[CalendarItemType.SickAbsence];
            var PaidVacationValue = resModel.FactorDetails[CalendarItemType.PaidVacation];
            var UnPaidVacationValue = resModel.FactorDetails[CalendarItemType.UnpaidVacation];
            var OvertimeValue = resModel.FactorDetails[CalendarItemType.ReclaimedOvertime];
            var PaidOvertimeValue = resModel.FactorDetails[CalendarItemType.OvertimeForReclaim];
            var PrivateOvertimeValue = resModel.FactorDetails[CalendarItemType.PrivateMinus];
            var JourneyValue = resModel.FactorDetails[CalendarItemType.Journey];

            //Assert
            Assert.AreEqual("GetAbsenceDataPerEMP", result.ViewName);
            Assert.AreEqual(2, SicknessValue.Count);
            Assert.AreEqual(1, PaidVacationValue.Count);
            Assert.AreEqual(1, UnPaidVacationValue.Count);
            Assert.AreEqual(2, OvertimeValue.Count);
            Assert.AreEqual(2, PaidOvertimeValue.Count);
            Assert.AreEqual(1, PrivateOvertimeValue.Count);
            Assert.AreEqual(1, JourneyValue.Count);
        }

        [Test]
        public void GetAbsenceDataPerEmp_AllInValid_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string fromDate = "01.01.2014";
            string toDate = "31.12.2018";
            string userName = "prvol";

            //Act
            var result = controller.GetAbsenceDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as AbsenceViewModelForEMP;

            //Assert
            Assert.AreEqual("NoAbsenceData", result.ViewName);

        }
        #endregion

        #region GetBirthday


        [Test]
        public void GetBirthdayNoBirthDates_NoData()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);

            //Act
            var result = controller.GetBirthdays() as ViewResult;

            //Assert
            Assert.AreEqual("NoBirthdays", result.ViewName);

        }

        [Test]
        public void GetBirthDay_View()
        {
            //Arrange


            List<Employee> employees = new List<Employee> 
            { 
            new Employee { EmployeeID = 4, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, PositionID = 2, EID = "andl", DateDismissed = new DateTime(2013, 11, 01), DateEmployed = new DateTime(2011, 11, 01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(1) },
            new Employee { EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, PositionID = 2, EID = "ascr", DateEmployed = new DateTime(2013, 04, 11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), BirthDay = DateTime.Now.AddDays(10) },
            new Employee { EmployeeID = 1, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(21) },
            new Employee { EmployeeID = 3, FirstName = "Abc", LastName = "Cde", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddYears(2) }
                
            };

            mock.Setup(m => m.Employees).Returns(employees);

            EMPController controller = new EMPController(mock.Object);
            //Act
            var result = controller.GetBirthdays() as ViewResult;
            var oRes = result.Model as List<Employee>;

            //Assert
            Assert.AreEqual(3, oRes.Count);
            Assert.AreEqual("Abc", oRes[0].FirstName);
            Assert.AreEqual("Anatoliy", oRes[1].FirstName);
            Assert.AreEqual("Tymur", oRes[2].FirstName);
        }

        [Test]
        public void GetBirthDayBirthdayDatesFrom10DaysAgoT_ViewMissingabout10daysBirthdays()
        {
            //Arrange

            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;

            List<Employee> employees = new List<Employee> 
            { 
            new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, PositionID = 2, EID = "andl", DateDismissed = new DateTime(2013, 11, 01), DateEmployed = new DateTime(2011, 11, 01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.ToLocalTimeAzure() },
            
            new Employee { EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, PositionID = 2, EID = "ascr", DateEmployed = new DateTime(2013, 04, 11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), BirthDay = DateTime.Now.ToLocalTimeAzure() },
            new Employee { EmployeeID = 3, FirstName = "Tymur1", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(1).ToLocalTimeAzure() }, 
            new Employee { EmployeeID = 4, FirstName = "Tymur2", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-1).ToLocalTimeAzure() }, 

            new Employee { EmployeeID = 5, FirstName = "Tymur3", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-10).ToLocalTimeAzure() },
            new Employee { EmployeeID = 6, FirstName = "Tymur4", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-11).ToLocalTimeAzure() }, 
            new Employee { EmployeeID = 7, FirstName = "Tymur5", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-9).ToLocalTimeAzure() }, 

            new Employee { EmployeeID = 8, FirstName = "Andriy", LastName = "Cdess", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(30).ToLocalTimeAzure() },
            new Employee { EmployeeID = 9, FirstName = "Oleg", LastName = "Lopp", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(31).ToLocalTimeAzure() }, 
            new Employee { EmployeeID = 10, FirstName = "Igor", LastName = "Kopl", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(29).ToLocalTimeAzure() }, 

            
            new Employee { EmployeeID = 11, FirstName = "Andriy2", LastName = "Cdess", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddMonths(2).ToLocalTimeAzure() },
            new Employee { EmployeeID = 12, FirstName = "Anton", LastName = "Cde", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddYears(2).ToLocalTimeAzure() },
            };

            mock.Setup(m => m.Employees).Returns(employees);

            EMPController controller = new EMPController(mock.Object);
            //Act
            var result = controller.GetBirthdays() as ViewResult;
            var oRes = result.Model as List<Employee>;

            //Assert
            Assert.AreEqual(8, oRes.Count);
            Assert.AreEqual("Tymur3", oRes[0].FirstName);
            Assert.AreEqual("Tymur5", oRes[1].FirstName);
            Assert.AreEqual("Tymur2", oRes[2].FirstName);
            Assert.AreEqual("Anatoliy", oRes[3].FirstName);
            Assert.AreEqual("Anton", oRes[4].FirstName);
            Assert.AreEqual("Tymur1", oRes[5].FirstName);
            Assert.AreEqual("Igor", oRes[6].FirstName);
            Assert.AreEqual("Andriy", oRes[7].FirstName);

        }

        [Test]
        public void GetBirthDayDatesFromNowTo30DaysAhead_ViewFutureBirthdays()
        {
            //Arrange


            List<Employee> employees = new List<Employee> 
            { 
            new Employee { EmployeeID = 4, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, PositionID = 2, EID = "andl", DateDismissed = new DateTime(2013, 11, 01), DateEmployed = new DateTime(2011, 11, 01), IsManager = false, BusinessTrips = new List<BusinessTrip>(),CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-10) },
            new Employee { EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, PositionID = 2, EID = "ascr", DateEmployed = new DateTime(2013, 04, 11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), BirthDay = DateTime.Now.AddDays(-11) },
            new Employee { EmployeeID = 1, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddDays(-2) },
            new Employee { EmployeeID = 3, FirstName = "Anton", LastName = "Cde", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddYears(2) },
            new Employee { EmployeeID = 5, FirstName = "Andriy", LastName = "Cdess", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddMonths(-1) },
            new Employee { EmployeeID = 6, FirstName = "Andriy", LastName = "Cdess", DepartmentID = 1, PositionID = 2, EID = "tepyee", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(),BirthDay = DateTime.Now.AddMonths(-2) },
            };

            mock.Setup(m => m.Employees).Returns(employees);

            EMPController controller = new EMPController(mock.Object);
            //Act
            var result = controller.GetBirthdays() as ViewResult;
            var oRes = result.Model as List<Employee>;

            //Assert
            Assert.AreEqual(2, oRes.Count);
            Assert.AreEqual("Tymur", oRes[0].FirstName);
            Assert.AreEqual("Anton", oRes[1].FirstName);
        }

        [Test]
        public void TransformDateTime_1990_03_21__2014_12_13()
        {
            //Arange
            EMPController controller = new EMPController(mock.Object);

            //Act
            DateTime result = controller.TransformBirthDate(new DateTime(1990, 03, 21), new DateTime(2014, 12, 31));

            //Assert
            Assert.AreEqual(new DateTime(2014, 03, 21), result);

        }

        [Test]
        public void TransformDateTime_1990_01_21__2014_12_02()
        {
            //Arange
            EMPController controller = new EMPController(mock.Object);

            //Act
            DateTime result = controller.TransformBirthDate(new DateTime(1990, 01, 21), new DateTime(2014, 12, 02));

            //Assert
            Assert.AreEqual(new DateTime(2015, 01, 21), result);

        }

        [Test]
        public void TransformDateTime_1990_01_21__2014_11_02()
        {
            //Arange
            EMPController controller = new EMPController(mock.Object);

            //Act
            DateTime result = controller.TransformBirthDate(new DateTime(1990, 01, 21), new DateTime(2014, 11, 02));

            //Assert
            Assert.AreEqual(new DateTime(2014, 01, 21), result);

        }

        #endregion

        #region GetReportedBTs

        [Test]
        public void GetReportedBTsIncorrectUserName_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "abcd";
            int selectedYear = 2012;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetReportedBTsEmptyUserName_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "";
            int selectedYear = 2012;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetReportedBTsNullUserName_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = null;
            int selectedYear = 2012;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetReportedBTsNonExistingYear_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwoo";
            int selectedYear = 2000;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoBtsInThisYear", result.ViewName);
        }

        [Test]
        public void GetReportedBTsZeroYear_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwoo";
            int selectedYear = 0;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoBtsInThisYear", result.ViewName);
        }

        [Test]
        public void GetReportedBTsNegativeYear_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwoo";
            int selectedYear = -1;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoBtsInThisYear", result.ViewName);
        }

        [Test]
        public void GetReportedBTs_View_OnlyRegisteredOrConfirmedButNotCancelledBTs()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "xtwe";
            int selectedYear = 2014;

            //Act
            PartialViewResult result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(2, (result.Model as List<BusinessTrip>).Count);
        }

        [Test]
        public void GetReportedBTsNoBTs_NoData()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tebl";
            int selectedYear = 2014;

            //Act
            var result = controller.GetReportedBTs(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoBtsInThisYear", result.ViewName);
        }

        #endregion

        #region GetBusinessTripByYearsEMP
        [Test]
        public void GetBussinessTripByYearsEMP_IncorrectUserName_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "abcd";
            int selectedYear = 2012;

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBussinessTripByYearsEMP_UserNameNull_NoDataView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "";
            int selectedYear = 0;

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
            Assert.AreEqual(null, ((PartialViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBussinessTripByYearsEMP__EmptyView()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "povol";
            int selectedYear = 0;

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("Empty", result.ViewName);
            Assert.AreEqual(0, ((PartialViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByYearsEMP_SelectedYearInSelectedList_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "iwoo";
            int selectedYear = 2013;

            //selected.Add(2014);

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(userName, ((PartialViewResult)result).ViewBag.UserName);
            Assert.AreEqual(selectedYear, ((PartialViewResult)result).ViewBag.SelectedYear);

        }

        [Test]
        public void GetBusinessTripByYearsEmp_SelectedYearIsntInSelectedList_View()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "xomi";
            int selectedYear = 2014;
            int realSelectedYear = 2012;

            //selected.Add(2014);

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(userName, ((PartialViewResult)result).ViewBag.UserName);
            Assert.AreEqual(realSelectedYear, ((PartialViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByYearsEMPNoBTs_NoData()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tebl";
            int selectedYear = 2010;

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(userName, ((PartialViewResult)result).ViewBag.UserName);
            Assert.AreEqual(selectedYear, ((PartialViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByYearsEMPBTs_BtsData()
        {
            //Arrange
            EMPController controller = new EMPController(mock.Object);
            string userName = "tebl";
            int selectedYear = 2014;

            //Act
            var result = controller.GetBusinessTripByYearsEMP(selectedYear, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(userName, ((PartialViewResult)result).ViewBag.UserName);
            Assert.AreEqual(selectedYear, ((PartialViewResult)result).ViewBag.SelectedYear);
        }

        #endregion

        #region GetVisaData
        [Test]
        public void CalculateVisaDays_EmployeeWithoutVisa_ZeroDays()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            int spentDays = 0;

            //Act        
            var result = controller.CalculateVisaDays(15);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }

        [Test]
        public void CalculateVisaDays_EmployeeWithoutBtsAndPrivatetrips_ZeroDays()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);

            int spentDays = 0;

            //Act        
            var result = controller.CalculateVisaDays(18);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTButNotConfirmedReported_CorrectNumber()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-9).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4).Date,
                OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10).Date,
                OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5).Date,
                Status = BTStatus.Confirmed,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });
            int spentDays = 13;

            //Act        
            var result = controller.CalculateVisaDays(1);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);

        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported_OrderStartDateNull_CorrectNumber()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-9).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-4).Date,
                OrderStartDate = null,
                OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-5).Date,
                Status = BTStatus.Confirmed | BTStatus.Reported,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });


            int spentDays = 13;

            //Act        
            var result = controller.CalculateVisaDays(1);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);

        }

        [Test]
        public void CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported_OrderEndDateNull_CorrectNumber()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();

            BusinessTrip bTrip = new BusinessTrip
            {
                StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(5).Date,
                EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(10).Date,
                Status = BTStatus.Planned,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            int spentDays = 13;

            //Act        
            var result = controller.CalculateVisaDays(1);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);

        }

        [TestCase(-5, -6, Result = 13)]
        [TestCase(1, 2, Result = 13)]
        [TestCase(0, 1, Result = 13)]
        [TestCase(-1, 0, Result = 14)]
        [TestCase(-180, -179, Result = 15)]
        [TestCase(-181, -180, Result = 14)]
        [TestCase(-182, -181, Result = 13)]
        public int CalculateVisaDays_NewPlanedBT_EmployeeHasBTConfirmedReported(int orderStartDaysDelta, int orderEndDaysDelta)
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();


            mock.Object.BusinessTrips.Add(new BusinessTrip
            {
                BTof = employee,
                StartDate = DateTime.Now.AddDays(orderStartDaysDelta),
                EndDate = DateTime.Now.AddDays(orderEndDaysDelta),
                OrderStartDate = DateTime.Now.AddDays(orderStartDaysDelta),
                OrderEndDate = DateTime.Now.AddDays(orderEndDaysDelta),
                Status = BTStatus.Confirmed | BTStatus.Reported,
                EmployeeID = 1,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "test",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            });

            //Act        
            var result = controller.CalculateVisaDays(1);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);


            return result;

        }

        [TestCase(-5, -6, Result = 13)]
        [TestCase(1, 2, Result = 13)]
        [TestCase(0, 1, Result = 14)]
        [TestCase(-1, 0, Result = 15)]
        [TestCase(-180, -179, Result = 15)]
        [TestCase(-181, -180, Result = 14)]
        [TestCase(-182, -181, Result = 13)]
        public int CalculateVisaDays_NewPlanedBT_EmployeeHasPrivateTrip(int StartDateDelta, int EndDateDelta)
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();


            mock.Object.PrivateTrips.Add(new PrivateTrip
            {
                StartDate = DateTime.Now.AddDays(StartDateDelta).Date,
                EndDate = DateTime.Now.AddDays(EndDateDelta).Date,
                EmployeeID = 1
            });

            //Act        
            var result = controller.CalculateVisaDays(1);


            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);

            return result;

        }

        [TestCase(-5, -6, Result = 0)]
        [TestCase(1, 2, Result = 0)]
        [TestCase(0, 1, Result = 1)]
        [TestCase(-1, 0, Result = 2)]
        [TestCase(-180, -179, Result = 2)]
        [TestCase(-181, -180, Result = 1)]
        [TestCase(-182, -181, Result = 0)]
        public int DaysSpentInTrip(int StartDateDelta, int EndDateDelta)
        {

            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            DateTime EndDate = DateTime.Now.ToLocalTimeAzure();
            DateTime StartDate = EndDate.AddDays(-180);

            //Act        
            var result = controller.DaysSpentInTrip(StartDate, EndDate, EndDate.AddDays(StartDateDelta), EndDate.AddDays(EndDateDelta));

            //Assert 
            return result;

        }


        [Test]
        public void DaysSpentInTrip_StartDateNull_0Days()
        {
            //Arrange 

            EMPController controller = new EMPController(mock.Object);
            int spentDays = 0;
            DateTime startPeriod = DateTime.Now.ToLocalTimeAzure().AddDays(-180).Date;
            DateTime endPeriod = DateTime.Now.ToLocalTimeAzure();
            DateTime? startDate = null;
            DateTime? endDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10).Date;

            //Act        
            var result = controller.DaysSpentInTrip(startPeriod, endPeriod, startDate, endDate);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }

        [Test]
        public void DaysSpentInTrip_EndDateNull_0Days()
        {
            //Arrange 
            EMPController controller = new EMPController(mock.Object);
            int spentDays = 0;
            DateTime startPeriod = DateTime.Now.ToLocalTimeAzure().AddDays(-180).Date;
            DateTime endPeriod = DateTime.Now.ToLocalTimeAzure();
            DateTime? startDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10).Date;
            DateTime? endDate = null;

            //Act        
            var result = controller.DaysSpentInTrip(startPeriod, endPeriod, startDate, endDate);

            //Assert 
            Assert.IsInstanceOf(typeof(Int32), result);
            Assert.AreEqual(spentDays, result);
        }
        #endregion
    }
}

