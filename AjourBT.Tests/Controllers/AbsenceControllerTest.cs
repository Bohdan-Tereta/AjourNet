using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Tests.Controllers
{
        [TestFixture]
        public class AbsenceControllerTest
        {
            Mock<IRepository> mock;
            Mock<IXLSExporter> xlsExporterMock; 

            AbsenceController controller;

            [SetUp]
            public void SetUp()
            {
                mock = Mock_Repository.CreateMock();
                xlsExporterMock = new Mock<IXLSExporter>();
                controller = new AbsenceController(mock.Object, xlsExporterMock.Object);
            }

            #region GetAbsence

            [Test]
            public void GetAbsence_View()
            {
                //Arrange
                
                //Act
                var result = controller.GetAbsence() as ViewResult;

                //Assert
                Assert.AreEqual("", result.ViewName);
            }

            [Test]
            public void GetAbsenceSearchString_View()
            {
                //Arrange
                string searchString = "ABC";

                //Act
                var result = controller.GetAbsence(searchString) as ViewResult;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual("ABC", result.ViewBag.SearchString);
            }
            #endregion

            #region GetAbsenceData

            [Test]
            public void GetAbsenceDataBadDates_NoDataView()
            {
                //Arrange
                string fromDate = "123.456.789";
                string toDate = "45.67.89";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate) as PartialViewResult;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            [Test]
            public void GetAbsenceDataCalendarItemsNull_NoDataView()
            {
                //Arrange
                string fromDate = "10.11.1000";
                string toDate = "10.11.1000";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate) as PartialViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            [Test]
            public void GetAbsenceDataCalendarItemsNullAndsearchString_NoDataView()
            {
                //Arrange
                string fromDate = "10.11.1000";
                string toDate = "10.11.1000";
                string searchString = "ABC";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as PartialViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }


            [Test]
            public void GetAbsenceDataCorrectDates_View()
            {
                //Arrange 
                string fromDate = "01.01.2000";
                string toDate = "28.07.2017";
                string searchString = "";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewBag.SearchString);
                Assert.AreEqual(fromDate, result.ViewBag.FromDate);
                Assert.AreEqual(toDate, result.ViewBag.ToDate);
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(1, modelResult.Count);

                //Assert.AreEqual(0, modelResult.ToArray()[0].Journeys.Count);
                //Assert.AreEqual(1, modelResult.ToArray()[0].Overtimes.Count);
                //Assert.AreEqual(1, modelResult.ToArray()[0].Sickness.Count);
                //Assert.AreEqual(2, modelResult.ToArray()[0].Vacations.Count);
                //Assert.AreEqual(1, modelResult.ToArray()[0].EmployeeID);
                //Assert.AreEqual(3, modelResult.ToArray()[0].BusinessTrips.Count);

                //Assert.AreEqual(0, modelResult.ToArray()[8].Journeys.Count);
                //Assert.AreEqual(1, modelResult.ToArray()[8].Overtimes.Count);
                //Assert.AreEqual(0, modelResult.ToArray()[8].Sickness.Count);
                //Assert.AreEqual(0, modelResult.ToArray()[8].Vacations.Count);
                //Assert.AreEqual(13, modelResult.ToArray()[8].EmployeeID);
                //Assert.AreEqual(0, modelResult.ToArray()[8].BusinessTrips.Count);

                //Assert.AreEqual(1, modelResult.ToArray()[2].Journeys.Count);
                //Assert.AreEqual(0, modelResult.ToArray()[2].Overtimes.Count);
                //Assert.AreEqual(0, modelResult.ToArray()[2].Sickness.Count);
                //Assert.AreEqual(0, modelResult.ToArray()[2].Vacations.Count);
                //Assert.AreEqual(3, modelResult.ToArray()[2].EmployeeID);
                //Assert.AreEqual(0, modelResult.ToArray()[2].BusinessTrips.Count);

                //Assert.AreEqual(5, modelResult.ToArray()[8].BusinessTrips.ToArray()[0].EmployeeID);
                //Assert.AreEqual(new DateTime(2014, 06, 12), modelResult.ToArray()[8].Vacations.ToArray()[0].From);
                //Assert.AreEqual(new DateTime(2014, 06, 28), modelResult.ToArray()[8].Vacations.ToArray()[0].To);


            }

            [Test]
            public void SearchStringEmpty_View()
            {
                //Arrange
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(1, modelResult.Count);
            }

            [Test]
            public void SearchStringNotEmpty_View()
            {
                //Arrange
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "andl";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(1, modelResult.Count);
            }

            [Test]
            public void SearchStringNotEmptyabcdef_View()
            {
                //Arrange
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "abcdef";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as PartialViewResult;
                var modelResult = result.Model as IList<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            #endregion

            #region ExportAbsenceToExcel

            [Test]
            public void ExportAbsenceToExcel_SearchStringNotNull_XLSFile()
            {
                //Arrange 
                DateTime from = new DateTime(2014,01,01); 
                DateTime to = new DateTime(2014,02,01); 
                string searchString = "abc"; 

                //Act 
                var file = controller.ExportAbsenceToExcel(from.ToString("dd.MM.yyyy"), to.ToString("dd.MM.yyyy"), searchString);

                //Assert 
                mock.Verify(m => m.SearchAbsenceData(from, to, searchString), Times.Once);
                xlsExporterMock.Verify(m => m.ExportAbsenceToExcel(It.IsAny<IList<AbsenceViewModel>>()), Times.Once);
                Assert.IsTrue(file is FileResult);
            }

            [Test]
            public void ExportAbsenceToExcel_NoSearchString_XLSFile()
            {
                //Arrange 
                Worksheet workSheet = new Worksheet("Absence");
                DateTime from = new DateTime(2014, 01, 01);
                DateTime to = new DateTime(2014, 02, 01);

                //Act 
                var file = controller.ExportAbsenceToExcel(from.ToString("dd.MM.yyyy"), to.ToString("dd.MM.yyyy"));

                //Assert 
                mock.Verify(m => m.SearchAbsenceData(from, to, ""), Times.Once);
                xlsExporterMock.Verify(m => m.ExportAbsenceToExcel(It.IsAny<IList<AbsenceViewModel>>()), Times.Once);
                Assert.IsTrue(file is FileResult);
            }

            #endregion

        }
    }