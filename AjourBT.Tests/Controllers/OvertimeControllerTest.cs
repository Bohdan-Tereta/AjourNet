using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class OvertimeControllerTest
    {
        Mock<IRepository> mock;
        OvertimeController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            mock.Setup(m => m.SaveOvertime(It.IsAny<Overtime>())).Verifiable();
            mock.Setup(m => m.SaveJourney(It.IsAny<Journey>())).Verifiable();
            controller = new OvertimeController(mock.Object);
        }

        [Test]
        public void GetOvertime_view()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            var result = controller.GetOvertime();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }

        [Test]
        public void GetOvertimeDataEmptySearchStrign_view()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            var result = controller.GetOvertimeData();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }

        [Test]
        public void GetOvertimeDataNotEmptyString_view()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            string searchString = "Igor";
            List<Employee> emp = (from e in mock.Object.Employees where e.FirstName == "Igor" select e).ToList();

            //Act
            var result = controller.GetOvertimeData(searchString) as ViewResult;
            var viewRes = result.Model as List<OvertimeByEmployeeModel>;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<OvertimeByEmployeeModel>), result.Model);
            Assert.AreEqual(emp.Count, viewRes.Count);
            Assert.AreEqual(12, viewRes.ToArray()[0].EmployeeID);
            Assert.AreEqual("RAAA3", viewRes.ToArray()[0].Department);
            Assert.AreEqual("Woody", viewRes.ToArray()[0].LastName);
            Assert.AreEqual(15, viewRes.ToArray()[3].EmployeeID);
            Assert.AreEqual("RAAA3", viewRes.ToArray()[3].Department);
            Assert.AreEqual("Woooody", viewRes.ToArray()[3].LastName);
        }

        [Test]
        public void SearchOvertimeData_emptySearchString__ListOfOvertimeModel()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            string searchString = "";
            List<Employee> emp = (from e in mock.Object.Employees where e.EmployeeID > 0 orderby e.EmployeeID select e).ToList();

            //Act
            List<OvertimeByEmployeeModel> result = controller.SearchOvertimeData(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(List<OvertimeByEmployeeModel>), result);
            Assert.AreEqual(emp.Count, result.Count);
            Assert.AreEqual(8, result.ToArray()[0].EmployeeID);
            Assert.AreEqual("Oleksiy", result.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", result.ToArray()[0].LastName);
            Assert.AreEqual(11, result.ToArray()[16].EmployeeID);
            Assert.AreEqual("Oleg", result.ToArray()[16].FirstName);
            Assert.AreEqual("Talee", result.ToArray()[16].LastName);
        }

        [Test]
        public void SearchOvertimeData_NotEmptrySearchString_ListOfOvertimeModel()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            string searchString = "Oleg";
            List<Employee> emp = (from e in mock.Object.Employees where e.FirstName == searchString select e).ToList();

            //Act
            List<OvertimeByEmployeeModel> result = controller.SearchOvertimeData(searchString);

            //Assert
            Assert.IsInstanceOf(typeof(List<OvertimeByEmployeeModel>), result);
            Assert.AreEqual(emp.Count, result.Count);
            Assert.AreEqual(11, result.ToArray()[0].EmployeeID);
            Assert.AreEqual("Oleg", result.ToArray()[0].FirstName);
            Assert.AreEqual("Talee", result.ToArray()[0].LastName);
        }

        [Test]
        public void SearchOvertimeData_BadSearchString_EmptyList()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            string searchString = "BAD SEARCH STRING";
            List<Employee> emp = (from e in mock.Object.Employees
                                  where
                                      (e.FirstName == searchString
                                      || e.LastName == searchString
                                      || e.EID == searchString)
                                  select e).ToList();

            //Act
            List<OvertimeByEmployeeModel> result = controller.SearchOvertimeData(searchString);

            //Assert
            Assert.AreEqual(emp.Count, result.Count);
        }

        [Test]
        public void GetEditOvertime_correctIDNotEmptySearchString_View()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            string searchString = "9";
            var result = controller.EditOvertime(3, searchString) as ViewResult;
            var resModel = result.Model as Overtime;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
            Assert.AreEqual(3, result.ViewBag.ID);
            Assert.AreEqual(3, resModel.EmployeeID);
            Assert.IsTrue(resModel.DayOff);
        }

        [Test]
        public void GetEditOvertime_incorrectIDNotEmptySearchString_View()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            string searchString = "9";
            var result = controller.EditOvertime(3333, searchString) as ViewResult;
            var resModel = result.Model as Overtime;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
            Assert.AreEqual(3333, result.ViewBag.ID);
            Assert.AreEqual(null, resModel);
        }

        [Test]
        public void GetEditOvertime_0IDNotEmptySearchString_View()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            string searchString = "9";
            var result = controller.EditOvertime(0, searchString) as ViewResult;
            var resModel = result.Model as Overtime;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
            Assert.AreEqual(0, result.ViewBag.ID);
            Assert.AreEqual(null, resModel);
        }

        [Test]
        public void PostEditOvertimeConfirmed_InvalidOvertime_error()
        {
            //Arrange
            Overtime overtime = new Overtime { };
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            controller.ModelState.AddModelError("error", "error");
            var result = controller.EditOvertimeConfirmed(100500, null, "") as JsonResult;
            //string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(o => o.SaveOvertime(overtime), Times.Never);
        }

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            mock.Setup(m => m.SaveOvertime(It.IsAny<Overtime>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.EditOvertimeConfirmed(3);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveOvertime(It.IsAny<Overtime>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelReclaimDateNull_reclaimDateChangedView()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == false && over.ReclaimDate == null select over).FirstOrDefault();

            //Act
            var result = controller.EditOvertimeConfirmed(overtime.OvertimeID, new DateTime(2014, 05, 05)) as ViewResult;
            var resModel = result.Model as List<JourneysAndOvertimesModel>;
            JourneysAndOvertimesModel changedEmp = (from emp in resModel where emp.EmployeeID == overtime.EmployeeID select emp).FirstOrDefault();

            //Assert
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual(overtime.EmployeeID, changedEmp.EmployeeID);
            Assert.IsTrue(overtime.DayOff == true);
            Assert.IsTrue(overtime.ReclaimDate == new DateTime(2014, 05, 05));
            mock.Verify(o => o.SaveOvertime(overtime), Times.Once);
            
        }

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelreclaimDateNotNull_reclaimdateChangedView()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();

            //Act
            var result = controller.EditOvertimeConfirmed(3, new DateTime(2014, 09, 09)) as ViewResult;
            var resModel = result.Model as List<JourneysAndOvertimesModel>;
            JourneysAndOvertimesModel changedEmp = (from emp in resModel where emp.EmployeeID == overtime.EmployeeID select emp).FirstOrDefault();

            //Assert
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual(overtime.EmployeeID, changedEmp.EmployeeID);
            Assert.IsTrue(overtime.ReclaimDate == new DateTime(2014, 09, 09));
            Assert.AreEqual("tedk", changedEmp.EID);
            mock.Verify(o => o.SaveOvertime(overtime), Times.Once);
            Assert.AreEqual(new DateTime(2014, 09, 09), overtime.ReclaimDate.Value);
      
        }

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelreclaimDateNotNullPassedNotNull_reclaimdateChangedView_RelatedItemDeleted()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem {CalendarItemID = 9999, EmployeeID = 3, 
                From =new DateTime(2013, 03, 02), To = new DateTime(2013, 03, 02), Type = CalendarItemType.ReclaimedOvertime})
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act

            var result = controller.EditOvertimeConfirmed(3, new DateTime(2014, 09, 09)) as ViewResult;
            var resModel = result.Model as List<JourneysAndOvertimesModel>;
            JourneysAndOvertimesModel changedEmp = (from emp in resModel where emp.EmployeeID == overtime.EmployeeID select emp).FirstOrDefault();

            //Assert
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual(overtime.EmployeeID, changedEmp.EmployeeID);
            Assert.IsTrue(overtime.ReclaimDate == new DateTime(2014, 09, 09));
            Assert.AreEqual("tedk", changedEmp.EID);
            mock.Verify(o => o.SaveOvertime(overtime), Times.Once);
            Assert.AreEqual(new DateTime(2014, 09, 09), overtime.ReclaimDate.Value);
            mock.Verify(o => o.DeleteCalendarItem(It.IsAny<Int32>()), Times.Once);
            mock.Verify(o => o.SaveCalendarItem(It.IsAny<CalendarItem>()) , Times.Once);

        }

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelreclaimDateNotNullPassedNull_reclaimdateChangedView_RelatedItemDeleted()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem {CalendarItemID = 9999, EmployeeID = 3, 
                From =new DateTime(2013, 03, 02), To = new DateTime(2013, 03, 02), Type = CalendarItemType.ReclaimedOvertime})
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act

            var result = controller.EditOvertimeConfirmed(3) as ViewResult;
            var resModel = result.Model as List<JourneysAndOvertimesModel>;
            JourneysAndOvertimesModel changedEmp = (from emp in resModel where emp.EmployeeID == overtime.EmployeeID select emp).FirstOrDefault();

            //Assert
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual(overtime.EmployeeID, changedEmp.EmployeeID);
            Assert.IsTrue(overtime.ReclaimDate == null);
            Assert.AreEqual("tedk", changedEmp.EID);
            mock.Verify(o => o.SaveOvertime(overtime), Times.Once);
            Assert.IsNull(overtime.ReclaimDate);
            mock.Verify(o => o.DeleteCalendarItem(It.IsAny<Int32>()), Times.Once);
            mock.Verify(o => o.SaveCalendarItem(It.IsAny<CalendarItem>()) , Times.Never);

        }

       

        [Test]
        public void PostEditOvertimeConfirmed_ValidModelreclaimDateNullPassedNull_reclaimdateChangedView_RelatedItemDeleted()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.ReclaimDate = null;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act

            var result = controller.EditOvertimeConfirmed(3) as ViewResult;
            var resModel = result.Model as List<JourneysAndOvertimesModel>;
            JourneysAndOvertimesModel changedEmp = (from emp in resModel where emp.EmployeeID == overtime.EmployeeID select emp).FirstOrDefault();

            //Assert
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual(overtime.EmployeeID, changedEmp.EmployeeID);
            Assert.IsTrue(overtime.ReclaimDate == null);
            Assert.AreEqual("tedk", changedEmp.EID);
            mock.Verify(o => o.SaveOvertime(overtime), Times.Once);
            Assert.IsNull(overtime.ReclaimDate);
            mock.Verify(o => o.DeleteCalendarItem(It.IsAny<Int32>()), Times.Never);
            mock.Verify(o => o.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);

        }

        [Test]
        public void PostCreateNullEmployee_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 0;
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateNotNullEmployeeBadDates_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "Bad Type";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        #region Create
        [Test]
        public void PostCreateAllCorrectTypeOvertime_NoReclaimDate_False()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "30.11.2014";
            string to = "30.11.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateAllCorrectTypePaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "02.01.2013";
            string to = "02.01.2013";
            string type = "OvertimeForReclaim";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostCreateAllCorrectTypeUnpaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "PrivateMinus";

            bool result = controller.Create(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostCreateAllCorrectTypeReclaimedOvertime_CorrectReclaimDateForOvertimeUseJourney_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "31.10.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void PostCreateAllCorrectTypeReclaimedOvertime_JourneyIsAlreadyReclaimed_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "30.11.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert

            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);

        }

        [Test]
        public void PostCreateAllCorrectTypeReclaimedOvertime_CorrectReclaimDateForOvertimeUseOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 8;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "28.02.2013";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Once);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Once);

        }

        [Test]
        public void PostCreateTypeReclaimedOvertime_EmployeeHasNoOvertimesAndJourneys_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 22;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.10.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert

            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);

        }

        [Test]
        public void PostCreateTypeReclaimedOvertime_OvertimeIsReclaimed_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.10.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert

            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);
        }

        [Test]
        public void PostCreateTypeReclaimedOvertime_OvertimeIsOfWrongType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 13;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.10.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert

            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);
        }

        [Test]
        public void PostCreateTypeReclaimedOvertime_IncorrectReclaimDateForOvertimeAndJourney_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.10.2014";

            bool result = controller.Create(id, from, to, type, reclaimDate);

            //Assert

            Assert.IsFalse(result); 
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
            mock.Verify(m => m.SaveCalendarItem(It.IsAny<CalendarItem>()), Times.Never);
        }

        #endregion

        #region  PostDelete
        [Test]
        public void PostDeleteNullEmployee_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 0;
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmployeeBadDates_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "Bad Type";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteAllCorrectTypeOvertime_ReclaimDateFalse()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "11.30.2014";
            string to = "11.30.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteAllCorrectTypeOvertimeForReclaim_ReclaimDateTrue_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
                   Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "OvertimeForReclaim";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.DeleteCalendarItem(9999), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Exactly(2));
        }
        

        [Test]
        public void PostDeleteAllCorrectTypePaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 11;
            string from = "03.01.2013";
            string to = "03.01.2013";
            string type = "OvertimeForReclaim";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostDeleteAllCorrectTypeUnpaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 2;
            string from = "02.01.2013";
            string to = "02.01.2013";
            string type = "PrivateMinus";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostDeleteAllCorrectTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "02.02.2013";
            string to = "02.02.2013";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Once);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
        }

        [Test]
        public void PostDeleteIncorrectEmployeeWithSimilarOvertimeTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 8;
            string from = "22.06.2014";
            string to = "22.06.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
        }

        [Test]
        public void PostDeleteIncorrectDateTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "22.06.2000";
            string to = "22.06.2000";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
        }

        [Test]
        public void PostDeleteIncorrectEmployeeIdForJourneyTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "26.02.2014";
            string to = "26.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Never);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyCorrectCalendarItemTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            //mock.Object.CalendarItems.Concat(new[] { new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
            //    From = new DateTime(2014,02,27), 
            //    To = new DateTime(2014, 02, 27), 
            //    Type = CalendarItemType.ReclaimedOvertime}  });
            mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault().CalendarItems.Add(new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
                From = new DateTime(2014,02,27), 
                To = new DateTime(2014, 02, 27), 
                Type = CalendarItemType.ReclaimedOvertime});

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Once);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyIncorrectCalendarItemWrongEmpIDTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            //mock.Object.CalendarItems.Concat(new[] { new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
            //    From = new DateTime(2014,02,27), 
            //    To = new DateTime(2014, 02, 27), 
            //    Type = CalendarItemType.ReclaimedOvertime}  });
            mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault().CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 90,
                EmployeeID = 2,
                From = new DateTime(2014, 02, 27),
                To = new DateTime(2014, 02, 27),
                Type = CalendarItemType.ReclaimedOvertime
            });

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Never);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyIncorrectCalendarItemWrongFromTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            //mock.Object.CalendarItems.Concat(new[] { new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
            //    From = new DateTime(2014,02,27), 
            //    To = new DateTime(2014, 02, 27), 
            //    Type = CalendarItemType.ReclaimedOvertime}  });
            mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault().CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 90,
                EmployeeID = 1,
                From = new DateTime(2015, 02, 27),
                To = new DateTime(2014, 02, 27),
                Type = CalendarItemType.ReclaimedOvertime
            });

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Never);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyIncorrectCalendarItemWrongToTypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            //mock.Object.CalendarItems.Concat(new[] { new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
            //    From = new DateTime(2014,02,27), 
            //    To = new DateTime(2014, 02, 27), 
            //    Type = CalendarItemType.ReclaimedOvertime}  });
            mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault().CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 90,
                EmployeeID = 1,
                From = new DateTime(2014, 02, 27),
                To = new DateTime(2015, 02, 27),
                Type = CalendarItemType.ReclaimedOvertime
            });

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Never);
        }

        [Test]
        public void PostDeleteAllCorrectJourneyIncorrectCalendarItemWrongType__TypeReclaimedOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            //mock.Object.CalendarItems.Concat(new[] { new CalendarItem{CalendarItemID = 90, EmployeeID = 1, 
            //    From = new DateTime(2014,02,27), 
            //    To = new DateTime(2014, 02, 27), 
            //    Type = CalendarItemType.ReclaimedOvertime}  });
            mock.Object.Employees.Where(e => e.EmployeeID == 1).FirstOrDefault().CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 90,
                EmployeeID = 1,
                From = new DateTime(2014, 02, 27),
                To = new DateTime(2014, 02, 27),
                Type = CalendarItemType.OvertimeForReclaim
            });

            //Act
            int id = 1;
            string from = "27.02.2014";
            string to = "27.02.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Delete(id, from, to, type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Never);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
            mock.Verify(m => m.DeleteCalendarItem(It.IsAny<Int32>()), Times.Never);
        }


        #endregion

        #region PostEdit

        [Test]
        public void PostEditNullEmployee_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 0;
            string oldFrom = "";
            string oldTo = "";
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_NullReclaimDate_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "17.06.2013";
            string oldTo = "17.06.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = null;

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_EmptyReclaimDate_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "17.06.2013";
            string oldTo = "17.06.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = null;

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_DefaultReclaimDate_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "17.06.2013";
            string oldTo = "17.06.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = DateTime.MinValue.ToString("dd.MM.yyyy");

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeWrongID_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 9999;
            string oldFrom = "17.06.2013";
            string oldTo = "17.06.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "02.02.2013";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeWrongoType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "17.06.2013";
            string oldTo = "17.06.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "22.06.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundDefaultReclimDatePassed_ReclaimDateOfJourneyChanged_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "27.02.2014";
            string oldTo = "27.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = DateTime.MinValue.ToString("dd.MM.yyyy");

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundReclaimDateNotChanged_ReclaimDateOfJourneyChanged_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "27.02.2014";
            string oldTo = "27.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "30.11.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Once);
        }
        
        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundReclaimDateChangedToExisting_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "27.02.2014";
            string oldTo = "27.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.12.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveJourney(It.IsAny<Journey>()), Times.Exactly(2));
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundReclaimDateChangedToNotExisting_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "27.02.2014";
            string oldTo = "27.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.12.2011";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundDefaultReclimDatePassed_ReclaimDateOfOvertimeChanged_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = DateTime.MinValue.ToString("dd.MM.yyyy");

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Once);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateNotChanged_ReclaimDateOfOvertimeChanged_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "01.01.2013";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Once);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateChangedToExisting_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "17.06.2013";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.SaveOvertime(It.IsAny<Overtime>()), Times.Exactly(2));
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateChangedToNotExisting_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.12.2011";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateChangedToExistingWithWrongType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "28.02.2013";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateChangedToExistingWithNotNullReclaimDate_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "17.06.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeOvertimeFoundReclaimDateChangedToExistingButForWrongEmp_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "02.02.2013";
            string oldTo = "02.02.2013";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "12.02.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundReclaimDateChangedToExistingButIsAlreadyReclaimed_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 4;
            string oldFrom = "28.02.2014";
            string oldTo = "28.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "21.08.2013";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void PostEdit_ReclaimedOvertimeJourneyFoundReclaimDateChangedToExistingButForWrongEmp_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 4;
            string oldFrom = "28.02.2014";
            string oldTo = "28.02.2014";
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "ReclaimedOvertime";
            string reclaimDate = "11.12.2014";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type, reclaimDate);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNotNullEmployeeBadDates_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "";
            string oldTo = "";
            string from = "";
            string to = "";
            string type = "";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 1;
            string oldfrom = "01.01.2013";
            string oldto = "01.01.2013";
            string from = "03.03.2013";
            string to = "04.04.2013";
            string type = "Bad Type";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditAllCorrectTypeOvertime_NoReclaimedDateFalse()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 4;
            string oldfrom = "11.30.2014";
            string oldto = "11.30.2014";
            string from = "10.11.2014";
            string to = "10.11.2014";
            string type = "ReclaimedOvertime";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditAllCorrectTypePaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 8;
            string oldfrom = "28.02.2013";
            string oldto = "28.02.2013";
            string from = "30.05.2013";
            string to = "30.05.2013";
            string type = "OvertimeForReclaim";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostEditAllCorrectTypeUnpaidOvertime_true()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            int id = 13;
            string oldfrom = "22.02.2013";
            string oldto = "22.02.2013";
            string from = "14.07.2013";
            string to = "14.07.2013";
            string type = "PrivateMinus";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type);

            //Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region SearchJourneyDataForOneEmp

        [Test]
        public void SearchOvertimeDataForOneEmp_EmployeeExists_JourneysExistButAlreadyReclaimed_ListOfOvertimes()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            List<DateTime> result = controller.SearchOvertimeDataForOneEmp(1);

            //Assert 
            Assert.AreEqual(result.Count, 1);
            //Assert.AreEqual(result[0], new DateTime(2013, 01, 01));
            //Assert.AreEqual(result[0], new DateTime(2013, 01, 17));

        }

        [Test]
        public void SearchOvertimeDataForOneEmp_EmployeeExists_NoOvertimes_EmptyList()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);

            //Act
            List<DateTime> result = controller.SearchOvertimeDataForOneEmp(5);

            //Assert 
            Assert.AreEqual(result.Count, 0);
            //Assert.AreEqual(result[0], new DateTime(2013, 01, 17));

        }

        #endregion

        #region GetReclaimedOvertime
        [Test]
        public void GetReclaimedOvertime_UnparsableFromDate_HttpNotFound()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.20dddd13";

            ActionResult result = controller.GetReclaimedOvertime(id, from, to);

            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void GetReclaimedOvertime_UnparsableToDate_HttpNotFound()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 3;
            string from = "03.01.sdsdsds2013";
            string to = "03.01.2013";

            ActionResult result = controller.GetReclaimedOvertime(id, from, to);

            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void GetReclaimedOvertime_NotExistingEmployee_HttpNotFound()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 336;
            string from = "03.01.2013";
            string to = "03.01.2013";

            ActionResult result = controller.GetReclaimedOvertime(id, from, to);

            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void GetReclaimedOvertime_NotExistingOvertime_HttpNotFound()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 3;
            string from = "13.01.2013";
            string to = "13.01.2013";

            ActionResult result = controller.GetReclaimedOvertime(id, from, to);

            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void GetReclaimedOvertime_ExistingOvertime_HttpNotFound()
        {
            //Arrange
            OvertimeController controller = new OvertimeController(mock.Object);
            Overtime overtime = (from over in mock.Object.Overtimes where over.DayOff == true && over.ReclaimDate != null select over).FirstOrDefault();
            mock.Object.Overtimes.Where(o => o.OvertimeID == 3).FirstOrDefault().Employee.CalendarItems.Add(new CalendarItem
            {
                CalendarItemID = 9999,
                EmployeeID = 3,
                From = new DateTime(2013, 03, 02),
                To = new DateTime(2013, 03, 02),
                Type = CalendarItemType.ReclaimedOvertime
            })
                ;
            overtime.Employee.EmployeeID = 3;
            mock.Setup(m => m.DeleteCalendarItem(It.IsAny<Int32>())).Verifiable();
            //Act
            int id = 3;
            string from = "03.01.2013";
            string to = "03.01.2013";

            ActionResult result = controller.GetReclaimedOvertime(id, from, to);
            int resultId = GetValueFromJsonResult<int>(result as JsonResult, "id");
            DateTime resultFrom = GetValueFromJsonResult<DateTime>(result as JsonResult, "from");
            DateTime resultTo = GetValueFromJsonResult<DateTime>(result as JsonResult, "to");

            //Assert

            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(3, resultId);
            Assert.AreEqual(new DateTime(2013, 03, 02), resultFrom);
            Assert.AreEqual(new DateTime(2013, 03, 02), resultTo);
        }

        private T GetValueFromJsonResult<T>(JsonResult jsonResult, string propertyName)
        {
            var property =
                jsonResult.Data.GetType().GetProperties()
                .Where(p => string.Compare(p.Name, propertyName) == 0)
                .FirstOrDefault();

            if (null == property)
                throw new ArgumentException("propertyName not found", "propertyName");
            return (T)property.GetValue(jsonResult.Data, null);
        }

        #endregion

    }
}
