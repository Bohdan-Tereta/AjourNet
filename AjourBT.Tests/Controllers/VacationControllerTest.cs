using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Tests.MockRepository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class VacationControllerTest
    {
        Mock<IRepository> mock;
        VacationController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            controller = new VacationController(mock.Object);
        }

        #region CreateVacation
        [Test]
        public void PostCreateVacationNullEmployee_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            Vacation vacation = new Vacation { EmployeeID = 0 };

            //Act
            int id = 0;
            string fromDate = "";
            string toDate = "";
            string type = "";

            bool result = controller.Create(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveVacation(vacation), Times.Never);
        }

        [Test]
        public void PostCreateVacationNotNullEmployeeBadTypeBadDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            Vacation vacation = new Vacation { EmployeeID = 0 };

            //Act
            int id = 1;
            string fromDate = "";
            string toDate = "";
            string type = "Bad Type";

            bool result = controller.Create(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveVacation(vacation), Times.Never);
        }

        [Test]
        public void PostCreateVacationNotNullEmployeeBadTypeCorrectDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 1;
            string fromDate = "01.01.2014";
            string toDate = "15.01.2014";
            string type = "Bad Type";

            bool result = controller.Create(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateVacationNotNullEmployeePaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 1;
            string fromDate = "01.01.2014";
            string toDate = "15.01.2014";
            string type = "Paid Vacation";

            bool result = controller.Create(id, fromDate, toDate, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostCreateVacationNotNullEmployeeUnpaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            Vacation vacation = new Vacation { EmployeeID = 1, From = new DateTime(2014, 02, 10), To = new DateTime(2014, 02, 15), Type = VacationType.UnpaidVacation };

            //Act
            int id = 1;
            string fromDate = "10.02.2014";
            string toDate = "15.02.2014";
            string type = "Unpaid Vacation";

            bool result = controller.Create(id, fromDate, toDate, type);

            //Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteVacation

        [Test]
        public void PostDeleteVacationNullEmployee_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 0;
            string fromDate = "";
            string toDate = "";
            string type = "";

            bool result = controller.Delete(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.DeleteVacation(0), Times.Never);
        }

        [Test]
        public void PostDeleteVacationNotNullEmployeeBadTypeBadDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 1;
            string fromDate = "";
            string toDate = "";
            string type = "Bad Type";

            bool result = controller.Delete(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.DeleteVacation(0), Times.Never);
        }

        [Test]
        public void PostDeleteVacationNotNullEmployeeBadTypeCorrectDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 1;
            string fromDate = "01.01.2014";
            string toDate = "15.01.2014";
            string type = "Bad Type";

            bool result = controller.Delete(id, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteVacationNotNullEmployeePaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            DateTime fromDate = DateTime.ParseExact("12.02.2014", "dd.MM.yyyy", null);
            DateTime toDate = DateTime.ParseExact("28.02.2014", "dd.MM.yyyy", null);

            Vacation vacation = (from vac in mock.Object.Vacations
                                 where
                                     vac.EmployeeID == 1 &&
                                     vac.Type == VacationType.PaidVacation &&
                                     vac.From == fromDate &&
                                     vac.To == toDate
                                 select vac).FirstOrDefault();

            //Act
            int id = 1;
            string type = "PaidVacation";

            bool result = controller.Delete(id, "12.02.2014", "28.02.2014", type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.DeleteVacation(vacation.VacationID), Times.Once);
        }

        [Test]
        public void PostDeleteVacationNotNullEmployeeUnpaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            DateTime fromDate = DateTime.ParseExact("12.03.2014", "dd.MM.yyyy", null);
            DateTime toDate = DateTime.ParseExact("28.03.2014", "dd.MM.yyyy", null);

            Vacation vacation = (from vac in mock.Object.Vacations
                                 where
                                     vac.EmployeeID == 2 &&
                                     vac.Type == VacationType.UnpaidVacation &&
                                     vac.From == fromDate &&
                                     vac.To == toDate
                                 select vac).FirstOrDefault();

            //Act
            int id = 2;
            string type = "UnpaidVacation";

            bool result = controller.Delete(id, "12.03.2014", "28.03.2014", type);

            //Assert
            Assert.IsTrue(result);
            mock.Verify(m => m.DeleteVacation(vacation.VacationID), Times.Once);
        }

        #endregion

        #region EditVacation

        [Test]
        public void PostEditVacationNullEmployee_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            Vacation vacation = new Vacation { EmployeeID = 0 };

            //Act
            int id = 0;
            string fromDate = "";
            string toDate = "";
            string oldfrom = "";
            string oldTo = "";
            string type = "";

            bool result = controller.Edit(id, oldfrom, oldTo, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveVacation(vacation), Times.Never);
        }

        [Test]
        public void PostEditVacationNotNullEmployeeBadTypeBadDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            Vacation vacation = new Vacation { EmployeeID = 0 };

            //Act
            int id = 1;
            string fromDate = "";
            string toDate = "";
            string oldfrom = "";
            string oldTo = "";
            string type = "Bad Type";

            bool result = controller.Edit(id, oldfrom, oldTo, fromDate, toDate, type);

            //Assert
            Assert.IsFalse(result);
            mock.Verify(m => m.SaveVacation(vacation), Times.Never);
        }

        [Test]
        public void PostEditVacationNotNullEmployeeBadTypeCorrectDate_false()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 1;
            string oldfrom = "01.01.2014";
            string oldTo = "15.01.2014";
            string newFrom = "01.02.2014";
            string newTo = "15.02.2014";
            string type = "Bad Type";

            bool result = controller.Edit(id, oldfrom, oldTo, newFrom, newTo, type);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditVacationNotNullEmployeePaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 5;
            string oldFrom = "12.06.2014";
            string oldTo = "28.06.2014";
            string newFrom = "15.07.2014";
            string newTo = "01.08.2014";
            string type = "PaidVacation";

            bool result = controller.Edit(id, oldFrom, oldTo, newFrom, newTo, type);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostEditVacationNotNullEmployeeUnpaidVacation_true()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);

            //Act
            int id = 6;
            string oldFrom = "12.07.2014";
            string oldTo = "28.07.2014";
            string newFrom = "15.09.2014";
            string newTo = "30.09.2014";
            string type = "UnpaidVacation";

            bool result = controller.Edit(id, oldFrom, oldTo, newFrom, newTo, type);

            //Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region CalculateVacation

        [Test]
        public void PostCalculateVacation_BadDates_0()
        { 
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            string fromDate = "123.456.444";
            string toDate = "12.34.111";

            //Act
            var result = controller.CalculateVacation(fromDate, toDate);

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PostCalculateVacation_CorrectDatesWithHolidays_2()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            string fromDate = "30.04.2014";
            string toDate = "05.05.2014";

            //Act
            var result = controller.CalculateVacation(fromDate, toDate);

            //Arrange
            Assert.AreEqual(2, result);
        }


        #endregion

        #region CalculateOverralDays

        [Test]
        public void PostCalculateOverralDays_BadDatesFormat_0()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            string fromDate = "123.456.444";
            string toDate = "12.34.111";

            //Act
            var result = controller.CalculateOverralDays(fromDate, toDate);

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PostCalculateOverralDays_EndDateLessThanStartDate_0()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            string fromDate = "15.03.2014";
            string toDate = "01.03.2014";

            //Act
            var result = controller.CalculateOverralDays(fromDate, toDate);

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PostCalculateOverralDays_CorrectDates_DifferenceBetweenEndDateAndStartDate()
        {
            //Arrange
            VacationController controller = new VacationController(mock.Object);
            string fromDate = "01.03.2014";
            string toDate = "15.03.2014";

            //Act
            var result = controller.CalculateOverralDays(fromDate, toDate);

            //Assert
            Assert.AreEqual(15, result);
        }
        #endregion
    }
}
