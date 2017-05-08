using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Infrastructure; 
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


namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    class XLSExporterTest
    {
        XlsExporter xlsExporter; 
        Mock<IRepository> mock;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            xlsExporter = new XlsExporter();
            xlsExporter.workSheet = new Worksheet("test"); 
        }

        #region CustomWeekInterval 

        [Test]
        public void CustomWeekInterval_from2000_to2013()
        {
            //Arrange

            //Act
            List<XlsExporter.Interval> result = xlsExporter.CreateWeekInterval(2000, 2013, 12, 46);

            //Assert
            Assert.AreEqual(14, result.Count());
            Assert.AreEqual(12, result.ToArray()[0].weekFrom);
            Assert.AreEqual(52, result.ToArray()[0].weekTo);
            Assert.AreEqual(2000, result.ToArray()[0].year);
            Assert.AreEqual(1, result.ToArray()[6].weekFrom);
            Assert.AreEqual(52, result.ToArray()[6].weekTo);
            Assert.AreEqual(2006, result.ToArray()[6].year);
            Assert.AreEqual(1, result.ToArray()[13].weekFrom);
            Assert.AreEqual(46, result.ToArray()[13].weekTo);
            Assert.AreEqual(2013, result.ToArray()[13].year);
        }

        [Test]
        public void CustomInterval_from2013_to2013()
        {
            //Arrange

            //Act
            List<XlsExporter.Interval> result = xlsExporter.CreateWeekInterval(2013, 2013, 12, 46);

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(12, result.ToArray()[0].weekFrom);
            Assert.AreEqual(46, result.ToArray()[0].weekTo);
            Assert.AreEqual(2013, result.ToArray()[0].year);

        }
        #endregion

        #region WriteRow
        [Test]
        public void WriteRow_EmptyList_EmptyRow()
        {
            //Arrange
            List<string> properties = new List<string>(); 

            //Act
            xlsExporter.WriteRow(properties, 0);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[0,0].Value);
        }

        [Test]
        public void WriteRow_List_FilledRow()
        {
            //Arrange
            List<string> properties = new List<string>{"a", "b"};

            //Act
            xlsExporter.WriteRow(properties, 0);

            //Assert
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[0, 0].Value);
            Assert.AreEqual("b", xlsExporter.workSheet.Cells[0, 1].Value);
        }

        [Test]
        public void WriteRow_ListAndRowNumber_FilledRow()
        {
            //Arrange
            List<string> properties = new List<string> { "a", "b" };

            //Act
            xlsExporter.WriteRow(properties, 1);

            //Assert
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("b", xlsExporter.workSheet.Cells[1, 1].Value);
        }
        #endregion

        #region WriteAbsenceData
        [Test]
        public void WriteAbsenceData_NoAbsences_EmptyWorkSheet()
        {
            //Arrange
            List<AbsenceViewModel> absences = new List<AbsenceViewModel> ();

            //Act
            xlsExporter.WriteAbsenceData(absences);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 1].Value);
        }

        [Test]
        public void WriteAbsenceData_Absences_FilledWorkSheet()
        {
            //Arrange
            List<AbsenceViewModel> absences = new List<AbsenceViewModel>
            {
                new AbsenceViewModel{EID = "a"}, 
                new AbsenceViewModel{EID = "b"}
            };

            absences[0].BusinessTrips.Add(new CalendarItem());
            absences[1].BusinessTrips.Add(new CalendarItem()); 

            //Act
            xlsExporter.WriteAbsenceData(absences);

            //Assert
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[1, 2].Value);
            Assert.AreEqual("b", xlsExporter.workSheet.Cells[2, 2].Value);
        }
        #endregion 

        #region SetColumnWidths

        [Test]
        public void SetColumnWidths_EmptyArray_NoException()
        {
            //Arrange
            ushort[] columnWidths = new ushort[0]; 

            //Act
            xlsExporter.SetColumnWidths(columnWidths);

            //Assert
        }

        [Test]
        public void SetColumnWidths_NotEmptyArray_WidthsSet()
        {
            //Arrange
            ushort[] columnWidths = new ushort[2]{10,40};

            //Act
            xlsExporter.SetColumnWidths(columnWidths);

            //Assert 
            Assert.AreEqual(10, xlsExporter.workSheet.Cells.ColumnWidth[0]);
            Assert.AreEqual(40, xlsExporter.workSheet.Cells.ColumnWidth[1]); 
        }

        #endregion

        #region CreateCaption

        [Test]
        public void CreateCaption_EmptyArray_NoException()
        {
            //Arrange
            string[] captions = new string[0];

            //Act
            xlsExporter.CreateCaption(captions);

            //Assert
        }

        [Test]
        public void CreateCaption_NotEmptyArray_CaptionSet()
        {
            //Arrange
            string[] captions = new string[2] { "a", "b" };

            //Act
            xlsExporter.CreateCaption(captions);

            //Assert 
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[0,0].Value);
            Assert.AreEqual("b", xlsExporter.workSheet.Cells[0, 1].Value);
        }

        #endregion

        #region WriteBusinessTripsDataVU
        [Test]
        public void WriteBusinessTripsDataVU_NoBTs_EmptyWorkSheet()
        {
            //Arrange
            List<BusinessTripViewModel> businessTrips = new List<BusinessTripViewModel>();

            //Act
            xlsExporter.WriteBusinessTripDataVU(businessTrips);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 1].Value);
        }

        [Test]
        public void WriteBusinessTripsDataVU_BTs_FilledWorkSheet()
        {
            //Arrange
            List<BusinessTripViewModel> businessTrips = new List<BusinessTripViewModel>
                {
                    new BusinessTripViewModel(new BusinessTrip(mock.Object.BusinessTrips[0])), 
                    new BusinessTripViewModel(new BusinessTrip(mock.Object.BusinessTrips[0])), 
                    new BusinessTripViewModel(new BusinessTrip(mock.Object.BusinessTrips[0])), 
                };
            businessTrips[0].Status = BTStatus.Confirmed | BTStatus.Reported;
            businessTrips[0].Status = BTStatus.Confirmed;
            businessTrips[0].Status = BTStatus.Confirmed;

            xlsExporter.columnWidths = (new ushort[] { 10, 10, 10, 10, 10 });

            //Act
            xlsExporter.WriteBusinessTripDataVU(businessTrips);

            //Assert
            Assert.IsNotNull(xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.IsNotNull(xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.IsNotNull(xlsExporter.workSheet.Cells[3, 0].Value);
            Assert.IsNull(xlsExporter.workSheet.Cells[4, 0].Value);
            Assert.AreEqual(7166, xlsExporter.columnWidths[4]);
        }
        #endregion 

        #region WriteEmployeeDataVU
        [Test]
        public void WriteEmployeeDataVU_NoEmployees_EmptyWorkSheet()
        {
            //Arrange
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            //Act
            xlsExporter.WriteEmployeeDataVU(employees);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 1].Value);
        }

        [Test]
        public void WriteEmployeeDataVU_Employees_FilledWorkSheet()
        {
            //Arrange
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel(mock.Object.Employees[0]), 
                new EmployeeViewModel(mock.Object.Employees[0])
            };

            employees[0].EID = "abc"; 

            //Act
            xlsExporter.WriteEmployeeDataVU(employees);

            //Assert
            Assert.AreEqual("abc", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("andl", xlsExporter.workSheet.Cells[2, 0].Value);
        }
        #endregion 

        #region WriteEmployeeDataADM
        [Test]
        public void WriteEmployeeDataADM_NoEmployees_EmptyWorkSheet()
        {
            //Arrange
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            //Act
            xlsExporter.WriteEmployeeDataADM(employees);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 1].Value);
        }

        [Test]
        public void WriteEmployeeDataADM_Employees_FilledWorkSheet()
        {
            //Arrange
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel(mock.Object.Employees[0]), 
                new EmployeeViewModel(mock.Object.Employees[0])
            };

            employees[0].EID = "abc";

            //Act
            xlsExporter.WriteEmployeeDataADM(employees);

            //Assert
            Assert.AreEqual("abc", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("andl", xlsExporter.workSheet.Cells[2, 0].Value);
        }
        #endregion 

        #region WriteVisasAndPermitsDataVU
        [Test]
        public void WriteVisasAndPermitsDataVU_NoVisasAndPermits_EmptyWorkSheet()
        {
            //Arrange
            IList<Employee> employees = new List<Employee>();

            //Act
            xlsExporter.WriteVisasAndPermitsDataVU(employees);

            //Assert
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[1, 1].Value);
        }

        [Test]
        public void WriteVisasAndPermitsDataVU_VisasAndPermits_FilledWorkSheet()
        {
            //Arrange
            IList<Employee> employees = new List<Employee>();
            employees.Add(mock.Object.Employees[0]);
            employees.Add(mock.Object.Employees[1]);

            //Act
            xlsExporter.WriteVisasAndPermitsDataVU(employees);

            //Assert
            Assert.IsNotNull(xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.IsNotNull(xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.IsNull(xlsExporter.workSheet.Cells[3, 0].Value);
        }
        #endregion 

        #region SelectEmployeeByWeek

        [Test]
        public void SelectEmployeeByWeek_NullList_EmptyList()
        {
            //Arrange
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel>();

            //Act
            var result = xlsExporter.SelectEmployeeByWeek(emp, 1, 2014);
            //Assert
            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void SelectEmployeeByWeek_NotEmptyListAndWrongWeek_EmptyList()
        {
            //Arrange
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>()},

            };
            //Act
            var result = xlsExporter.SelectEmployeeByWeek(emp, 5, 2014);
            //Assert

            Assert.AreEqual(0, result.ToList().Count);
        }

        [Test]
        public void SelectEmployeeByWeek_NotEmptyListAndWeek_List()
        {
            //Arrange
            IEnumerable<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,01), To = new DateTime(2014,01,05), Hours = 0, WeekNumber = 1},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,08), To = new DateTime(2014,01,15), Hours = 0, WeekNumber = 2},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,07), Hours = 0, WeekNumber = 1}

                }
                },
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 2 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 3},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 4 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 5},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }}

            };

            //Act
            var result = xlsExporter.SelectEmployeeByWeek(emp, 1, 2014);
            //Assert
            Assert.AreEqual(3, result.ToList().Count);
        }

        #endregion 

        #region WritingOfWTRData

        [Test]
        public void WritingOfWTRData_NullList_EmptyList()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel>();

            //Act
            xlsExporter.WritingOfWTRData(new DateTime(2013, 12, 30), new DateTime(2014, 01, 05), emp); 

            //Assert
            Assert.AreEqual("", xlsExporter.workSheet.Cells[1,0].Value); 
            Assert.AreEqual("2014- W 1", xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.AreEqual("No absence data", xlsExporter.workSheet.Cells[3, 0].Value);
            Assert.AreEqual("", xlsExporter.workSheet.Cells[4, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 0].Value);

        }

        [Test]
        public void WritingOfWTRData_NotEmptyListAndWrongWeek_EmptyList()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>()},

            };
            //Act
            xlsExporter.WritingOfWTRData(new DateTime(2014, 01, 27), new DateTime(2014, 02, 01), emp); 

            //Assert

            Assert.AreEqual("", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("2014- W 5", xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.AreEqual("No absence data", xlsExporter.workSheet.Cells[3, 0].Value);
            Assert.AreEqual("", xlsExporter.workSheet.Cells[4, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 0].Value);
        }

        [Test]
        public void WritingOfWTRData_NotEmptyListAndWeek_List()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,01), To = new DateTime(2014,01,05), Hours = 0, WeekNumber = 1, Location = "a"},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,08), To = new DateTime(2014,01,15), Hours = 0, WeekNumber = 2},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,07), Hours = 0, WeekNumber = 1}

                }
                },
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 2 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 3},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 4 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 5},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }}

            };

            //Act
            xlsExporter.WritingOfWTRData(new DateTime(2014, 01, 01), new DateTime(2014, 01, 05), emp); 

            //Assert
            Assert.AreEqual("", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("2014- W 1", xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.AreEqual("", xlsExporter.workSheet.Cells[3, 0].Value);
            Assert.AreEqual("Crudk Nazar(1)", xlsExporter.workSheet.Cells[4, 0].Value);
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[4, 1].Value);
            Assert.AreEqual("BT", xlsExporter.workSheet.Cells[4, 2].Value);
            Assert.AreEqual("01.01.2014 - 05.01.2014", xlsExporter.workSheet.Cells[4, 3].Value);
            Assert.AreEqual(0, xlsExporter.workSheet.Cells[4, 4].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 1].Value);
            Assert.AreEqual("ReclaimedOvertime", xlsExporter.workSheet.Cells[5, 2].Value);
            Assert.AreEqual("01.01.2014 - 07.01.2014", xlsExporter.workSheet.Cells[5, 3].Value);
            Assert.AreEqual(0, xlsExporter.workSheet.Cells[5, 4].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 0].Value);
            Assert.AreEqual("def abc(2)", xlsExporter.workSheet.Cells[6, 0].Value);
            Assert.AreEqual("ReclaimedOvertime", xlsExporter.workSheet.Cells[7, 2].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[8, 0].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[8, 2].Value);
        }

        [Test]
        public void WritingOfWTRData_NotEmptyListAndMoreThanOneWeek_List()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,01), To = new DateTime(2014,01,05), Hours = 0, WeekNumber = 1, Location = "a"},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,08), To = new DateTime(2014,01,15), Hours = 0, WeekNumber = 2},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,07), Hours = 0, WeekNumber = 1}

                }
                },
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 2 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 3},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>
                {
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.BT, From = new DateTime(2014,01,11), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 4 },
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.Journey, From = new DateTime(2014,01,18), To = new DateTime(2014,01,25), Hours = 0, WeekNumber = 5},
                    new FactorData{ Factor= Domain.Entities.CalendarItemType.ReclaimedOvertime, From = new DateTime(2014,01,01), To = new DateTime(2014,01,17), Hours = 0, WeekNumber = 1}

                }}

            };

            //Act
            xlsExporter.WritingOfWTRData(new DateTime(2014, 01, 01), new DateTime(2014, 01, 06), emp);

            //Assert
            Assert.AreEqual("", xlsExporter.workSheet.Cells[1, 0].Value);
            Assert.AreEqual("2014- W 1", xlsExporter.workSheet.Cells[2, 0].Value);
            Assert.AreEqual("", xlsExporter.workSheet.Cells[3, 0].Value);
            Assert.AreEqual("Crudk Nazar(1)", xlsExporter.workSheet.Cells[4, 0].Value);
            Assert.AreEqual("a", xlsExporter.workSheet.Cells[4, 1].Value);
            Assert.AreEqual("BT", xlsExporter.workSheet.Cells[4, 2].Value);
            Assert.AreEqual("01.01.2014 - 05.01.2014", xlsExporter.workSheet.Cells[4, 3].Value);
            Assert.AreEqual(0, xlsExporter.workSheet.Cells[4, 4].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 1].Value);
            Assert.AreEqual("ReclaimedOvertime", xlsExporter.workSheet.Cells[5, 2].Value);
            Assert.AreEqual("01.01.2014 - 07.01.2014", xlsExporter.workSheet.Cells[5, 3].Value);
            Assert.AreEqual(0, xlsExporter.workSheet.Cells[5, 4].Value);
            Assert.AreEqual(null, xlsExporter.workSheet.Cells[5, 0].Value);
            Assert.AreEqual("def abc(2)", xlsExporter.workSheet.Cells[6, 0].Value);
            Assert.AreEqual("ReclaimedOvertime", xlsExporter.workSheet.Cells[7, 2].Value);
            Assert.AreEqual("", xlsExporter.workSheet.Cells[8, 0].Value);
            Assert.AreEqual("2014- W 2", xlsExporter.workSheet.Cells[9, 0].Value);
        }

        #endregion

        #region ExportWTR

        [Test]
        public void ExportWTR_EmptyList_ByteArray()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel> 
            {

            };
            //Act
            byte[] result = xlsExporter.ExportWTR(new DateTime(2014, 01, 27), new DateTime(2014, 02, 01), emp);

            //Assert
            Assert.IsNotNull(result); 
        }

        [Test]
        public void ExportWTR_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<WTRViewModel> emp = new List<WTRViewModel> 
            {
                new WTRViewModel{ FirstName = "Nazar", LastName = "Crudk", ID = "1", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "abc", LastName = "def", ID = "2", FactorDetails = new List<FactorData>()},
                new WTRViewModel{ FirstName = "xyz", LastName = "nmb", ID = "3", FactorDetails = new List<FactorData>()},

            };
            //Act
            byte[] result = xlsExporter.ExportWTR(new DateTime(2014, 01, 27), new DateTime(2014, 02, 01), emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        #endregion 

        #region ExportVisasAndPermitsVU

        [Test]
        public void ExportVisasAndPermitsVU_EmptyList_ByteArray()
        {
            //Arrange
            IList<Employee> emp = new List<Employee>
            {

            };
            //Act
            byte[] result = xlsExporter.ExportVisasAndPermitsVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        [Test]
        public void ExportVisasAndPermitsVU_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<Employee> emp = new List<Employee> 
            {
                new Employee(),
            };
            //Act
            byte[] result = xlsExporter.ExportVisasAndPermitsVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        #endregion 

        #region ExportBusinessTripsToExcelVU

        [Test]
        public void ExportBusinessTripsToExcelVU_EmptyList_ByteArray()
        {
            //Arrange
            IList<BusinessTripViewModel> emp = new List<BusinessTripViewModel>
            {

            };
            //Act
            byte[] result = xlsExporter.ExportBusinessTripsToExcelVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        [Test]
        public void ExportBusinessTripsToExcelVU_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<BusinessTripViewModel> emp = new List<BusinessTripViewModel> 
            {
                new BusinessTripViewModel(mock.Object.BusinessTrips[0]),
            };
            //Act
            byte[] result = xlsExporter.ExportBusinessTripsToExcelVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        #endregion 

        #region ExportEmployeesToExcelVU

        [Test]
        public void ExportEmployeesToExcelVU_EmptyList_ByteArray()
        {
            //Arrange
            IList<EmployeeViewModel> emp = new List<EmployeeViewModel>
            {

            };
            //Act
            byte[] result = xlsExporter.ExportEmployeesToExcelVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        [Test]
        public void ExportEmployeesToExcelVU_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<EmployeeViewModel> emp = new List<EmployeeViewModel> 
            {
                new EmployeeViewModel(mock.Object.Employees[1]),
            };
            //Act
            byte[] result = xlsExporter.ExportEmployeesToExcelVU(emp);

            //Assert 
            Assert.IsNotNull(result); 
        }

        #endregion 

        #region ExportEmployeesToExcelADM

        [Test]
        public void ExportEmployeesToExcelADM_EmptyList_ByteArray()
        {
            //Arrange
            IList<EmployeeViewModel> emp = new List<EmployeeViewModel>
            {

            };
            //Act
            byte[] result = xlsExporter.ExportEmployeesToExcelADM(emp);

            //Assert 
            Assert.IsNotNull(result);
        }

        [Test]
        public void ExportEmployeesToExcelADM_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<EmployeeViewModel> emp = new List<EmployeeViewModel> 
            {
                new EmployeeViewModel(mock.Object.Employees[1]),
            };
            //Act
            byte[] result = xlsExporter.ExportEmployeesToExcelADM(emp);

            //Assert 
            Assert.IsNotNull(result);
        }

        #endregion 

        #region ExportAbsenceToExcel

        [Test]
        public void ExportAbsenceToExcel_EmptyList_ByteArray()
        {
            //Arrange
            IList<AbsenceViewModel> emp = new List<AbsenceViewModel>
            {

            };
            //Act
            byte[] result = xlsExporter.ExportAbsenceToExcel(emp);

            //Assert 
            Assert.IsNotNull(result);
        }

        [Test]
        public void ExportAbsenceToExcel_NotEmptyList_ByteArray()
        {
            //Arrange
            IList<AbsenceViewModel> emp = new List<AbsenceViewModel> 
            {
                new AbsenceViewModel(),
            };
            //Act
            byte[] result = xlsExporter.ExportAbsenceToExcel(emp);

            //Assert 
            Assert.IsNotNull(result);
        }

        #endregion

    }
}
