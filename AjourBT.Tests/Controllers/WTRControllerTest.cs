using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AjourBT.Infrastructure; 

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class WTRControllerTest
    {
        Mock<IRepository> mock;
        Mock<IXLSExporter> xlsExporterMock; 
        WTRController controller;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock(); 
            xlsExporterMock = new Mock<IXLSExporter>();
            controller = new WTRController(mock.Object, xlsExporterMock.Object);
        }

        #region GetWTR

        [Test]
        public void GetWTR_returnView()
        {
            //Arrange
            //Act
            var result = controller.GetWTR();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        #endregion

        #region GetWTRData

        [Test]
        public void GetWTRData_EmptyDates_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("", "", "");
            //Assert
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
            Assert.IsNull(result.ViewData.Model);
        }

        [Test]
        public void GetWTRData_CorrectDates_List()
        {
            //Arrange
            DateTime from = new DateTime(2014, 01, 01); 
            DateTime to = new DateTime(2014, 12, 12); 
            string searchString = String.Empty; 

            //Act
            PartialViewResult result = controller.GetWTRData(from.ToString("dd.MM.yyyy"), to.ToString("dd.MM.yyyy"), "");
            //Assert
            mock.Verify(m => m.SearchWTRData(from, to, searchString.Trim()), Times.Once);
            Assert.AreEqual(result.ViewName, ""); 
            Assert.AreEqual(1, result.ViewBag.FromWeek);
            Assert.AreEqual(50, result.ViewBag.ToWeek);
            Assert.AreEqual(2014, result.ViewBag.FromYear);
            Assert.AreEqual(2014, result.ViewBag.ToYear);
            Assert.AreEqual(from.ToString("dd.MM.yyyy"), result.ViewBag.fromDate);
            Assert.AreEqual(to.ToString("dd.MM.yyyy"), result.ViewBag.toDate);
            Assert.IsInstanceOf<IList<WTRViewModel>>(result.Model); 
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringToIsGreaterThanFrom_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);    
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringFromEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.201456", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringToEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "24.01.201456", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromGreaterThanFromGreaterThanCalendarItemTo_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("06.02.2014", "06.02.2014", "andl");
            //Assert
            Assert.AreEqual(0, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("", result.ViewName);
           
        }

        [Test]
        public void GetWTRData_returnPartialView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData();
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        #endregion

        #region GetAbsencePerEMP

        [Test]
        public void GetAbsensePerEmp_View()
        {
            //Arrange
            //Act
            var result = controller.GetAbsencePerEMP() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        #endregion

        #region GetWTRPerEMP

        [Test]
        public void GetWTRPerEMP_View()
        {
            //Arrange
            //Act
            var result = controller.GetWTRPerEMP() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

#endregion

        #region GetWTRDataPerEMP

        [Test]
        public void GetWTRDataPerEMP_BadDates_EmptyView()
        {
            //Arrange
            string fromDate = "";
            string toDate = "";
            string userName = "";

            //Act
            var result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDatesBadUserName_NoDataView()
        {
            //Arrange
            string fromDate = "21.01.2014";
            string toDate = "30.03.2014";
            string userName = "gggrr";

            //Act
            var result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDatesCorrectUserName_View()
        {
            //Arrange
            DateTime from = new DateTime(2010, 01, 01);
            DateTime to = new DateTime(2017, 12, 31); 
            string fromDate = "01.01.2010";
            string toDate = "31.12.2017";
            string userName = "andl";

            //Act
            PartialViewResult result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            mock.Verify(m => m.SearchWTRDataPerEMP(from, to, mock.Object.Employees[0]), Times.Once);
            Assert.AreEqual("GetWTRDataPerEMP", result.ViewName);
            Assert.AreEqual(1, result.ViewBag.FromWeek);
            Assert.AreEqual(53, result.ViewBag.ToWeek);
            Assert.AreEqual(2010, result.ViewBag.FromYear);
            Assert.AreEqual(2017, result.ViewBag.ToYear);
            Assert.AreEqual(userName, result.ViewBag.UserName);
            Assert.AreEqual(from.ToString("dd.MM.yyyy"), result.ViewBag.fromDate);
            Assert.AreEqual(to.ToString("dd.MM.yyyy"), result.ViewBag.toDate);
            Assert.IsInstanceOf<IList<WTRViewModel>>(result.Model); 
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringToIsGreaterThanFrom_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringFromEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.201456", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringToEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "24.01.201456", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromGreaterThanFromGreaterThanCalendarItemTo_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("07.02.2014", "07.02.2014", "andl");
            //Assert
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).Count);
            Assert.AreEqual("GetWTRDataPerEMP", result.ViewName);

        }

        #endregion

        #region WTRExportToExcel

        [Test]
        public void ExportWTR_FileResult()
        {
            //Arrange
            string searchString = "";
            DateTime from = new DateTime(2010, 01, 01);
            DateTime to = new DateTime(2017, 12, 31); 

            //Act
            FileResult file = controller.ExportWTR(searchString, from, to) as FileResult;

            //Assert
            mock.Verify(m => m.SearchWTRData(from, to, searchString),  Times.Once );
            xlsExporterMock.Verify(m => m.ExportWTR(from, to, It.IsAny<IList<WTRViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        #endregion 


        #region WTRExportToExcelForEmp

        [Test]
        public void ExportWTRForEMP_FileResult()
        {
            //Arrange
            string userName = "andl";
            DateTime from = new DateTime(2010, 01, 01);
            DateTime to = new DateTime(2017, 12, 31);  


            //Act
            FileResult file = controller.ExportWTRForEMP(userName, from, to) as FileResult;

            //Assert
            mock.Verify(m => m.SearchWTRDataPerEMP(from, to, mock.Object.Employees[0]), Times.Once);
            xlsExporterMock.Verify(m => m.ExportWTR(from, to, It.IsAny<IList<WTRViewModel>>()), Times.Once);
            Assert.IsInstanceOf(typeof(FileResult), file);
             }

        #endregion
    }
}
