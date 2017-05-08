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
    public class SickControllerTest
    {
        Mock<IRepository> mock;
        SickController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            controller = new SickController(mock.Object);
        }

        [Test]
        public void PostEditSick_ValidSick_true()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            int id = 4;
            string oldFrom = "24.04.2014";
            string oldTo = "28.04.2014";

            string from = "24.06.2014";
            string to = "28.06.2014";

            string type = "SickAbsence";
            string sickness = "GRZ";

            //Act
            var result = controller.Edit(id,oldFrom,oldTo,from,to,type,sickness);

            //Assert
            mock.Verify(o => o.SaveSick(It.IsAny<Sickness>()), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void PostEditNotNullEmployeeBadDates_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 1;
            string oldFrom = "";
            string oldTo = "";
            string from = "";
            string to = "";
            string type = "SickAbsence";
            string sickness = "";

            bool result = controller.Edit(id, oldFrom, oldTo, from, to, type,sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 1;
            string oldfrom = "01.01.2013";
            string oldto = "01.01.2013";
            string from = "03.03.2013";
            string to = "04.04.2013";
            string type = "Bad Type";
            string sickness = "";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type,sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNotNullEmployee_WithoutSicknessesAndCalendarItems_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 17;
            string oldfrom = "01.01.2013";
            string oldto = "01.01.2013";
            string from = "03.03.2013";
            string to = "04.04.2013";
            string type = "SickAbsence";
            string sickness = "";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }
     
        [Test]
        public void PostEditNotNullEmployeeWithSicknessWithoutCalendarItem_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 17;
            string oldfrom = "01.04.2014";
            string oldto = "07.04.2014";
            string from = "01.04.2014";
            string to = "10.04.2014";
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNotNullEmployeeWithoutSicknessWithCalendarItem_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 8;
            string oldfrom = "28.02.2013";
            string oldto = "28.02.2013";
            string from = "01.04.2014";
            string to = "10.04.2014";
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostEditNullEmployee_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 0;
            string oldfrom = "28.02.2013";
            string oldto = "28.02.2013";
            string from = "01.04.2014";
            string to = "10.04.2014";
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Edit(id, oldfrom, oldto, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }


        [Test]
        public void PostCreateNullEmployee_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 0;
            string from = "";
            string to = "";
            string type = "";
            string sickness = "";

            bool result = controller.Create(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateNotNullEmployeeBadDates_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 1;
            string from = "hhyhyhyh";
            string to = "20854.254.2255";
            string type = "SickAbsence";
            string sickness = "";

            bool result = controller.Create(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 1;
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "Bad Type";
            string sickness = "";

            bool result = controller.Create(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostCreateAllCorrectTypeOvertime_true()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 11;
            string from = "01.01.2013";
            string to = "10.01.2013";
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Create(id, from, to, type, sickness);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void PostDeleteNullEmployee_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 0;
            string from = "";
            string to = "";
            string type = "";
            string sickness = "";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmployeeBadDates_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 1;
            string from = "444448.415444.51474";
            string to = "54848.5154.54554";
            string type = "SickAbsence";
            string sickness = "";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmpCorrectDatesBadType_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 11;
            string from = "01.01.2013";
            string to = "10.01.2013";
            string type = "Bad Type";
            string sickness = "";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmployee_WithoutSicknessesAndCalendarItems_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 17;
            string from = "01.01.2013";
            string to = "01.01.2013";
            string type = "SickAbsence";
            string sickness = "";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmployeeWithSicknessWithoutCalendarItem_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 17;
            string from = "01.04.2014";
            string to = "07.04.2014";
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void PostDeleteNotNullEmployeeWithoutSicknessWithCalendarItem_false()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            //Act
            int id = 8;
            string from = "28.02.2013";
            string to = "28.02.2013";
           
            string type = "SickAbsence";
            string sickness = "GRZ";

            bool result = controller.Delete(id, from, to, type, sickness);

            //Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void PostDeleteSick_ValidSick_true()
        {
            //Arrange
            SickController controller = new SickController(mock.Object);

            int id = 4;
            string from = "24.04.2014";
            string to = "28.04.2014";



            string type = "SickAbsence";
            string sickness = "GRZ";

            //Act
            var result = controller.Delete(id, from, to, type, sickness);

            //Assert
            mock.Verify(o => o.DeleteSick(It.IsAny<int>()), Times.Once);
            Assert.IsTrue(result);
        }


    }
}