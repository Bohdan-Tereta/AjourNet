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

namespace AjourBT.Tests.ViewModels
{
    [TestFixture]
    public class WTRViewModelTest
    {
        Mock<IRepository> mock;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
        }

        #region Constructors 

        [Test]
        public void EmptyConstructor_NoParameters_EmptyViewModel()
        {
            //Arrange
            
            //Act
            WTRViewModel wtrViewModel = new WTRViewModel();
            //Assert 
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek); 
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.FirstDayOfWeek); 

        }

        [Test]
        public void Constructor_EmployeeDateTimeDateTime_ViewModel()
        {
            //Arrange

            //Act
            WTRViewModel wtrViewModel = new WTRViewModel(mock.Object.Employees[0], new DateTime(2014,02,21), new DateTime(2014,02,22));

            //Assert 
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.FirstDayOfWeek); 
            Assert.AreEqual("andl", wtrViewModel.ID);
            Assert.AreEqual("Anastasia", wtrViewModel.FirstName);
            Assert.AreEqual("Zarose", wtrViewModel.LastName);
            Assert.AreEqual(2, wtrViewModel.FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, wtrViewModel.FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 21), wtrViewModel.FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 02, 22), wtrViewModel.FactorDetails[0].To);
            Assert.AreEqual(0, wtrViewModel.FactorDetails[0].Hours);
            Assert.AreEqual(null, wtrViewModel.FactorDetails[0].Location);
            Assert.AreEqual(8, wtrViewModel.FactorDetails[0].WeekNumber);
            Assert.AreEqual(CalendarItemType.SickAbsence, wtrViewModel.FactorDetails[1].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 21), wtrViewModel.FactorDetails[1].From);
            Assert.AreEqual(new DateTime(2014, 02, 22), wtrViewModel.FactorDetails[1].To);
            Assert.AreEqual(0, wtrViewModel.FactorDetails[1].Hours);
            Assert.AreEqual(null, wtrViewModel.FactorDetails[1].Location);
            Assert.AreEqual(8, wtrViewModel.FactorDetails[1].WeekNumber); 

        }

        [Test]
        public void Constructor_WTRViewModelINtInt_ViewModel()
        {
            //Arrange

            //Act 
            WTRViewModel parentWtrViewModel = new WTRViewModel(mock.Object.Employees[0], new DateTime(2014, 02, 01), new DateTime(2014, 02, 28));
            WTRViewModel wtrViewModel = new WTRViewModel(parentWtrViewModel, 8, 2014);

            //Assert 
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            Assert.AreEqual(DayOfWeek.Monday, System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.FirstDayOfWeek); 
            Assert.AreEqual("andl", wtrViewModel.ID);
            Assert.AreEqual("Anastasia", wtrViewModel.FirstName);
            Assert.AreEqual("Zarose", wtrViewModel.LastName);
            Assert.AreEqual(2, wtrViewModel.FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, wtrViewModel.FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 17), wtrViewModel.FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 02, 23), wtrViewModel.FactorDetails[0].To);
            Assert.AreEqual(0, wtrViewModel.FactorDetails[0].Hours);
            Assert.AreEqual(null, wtrViewModel.FactorDetails[0].Location);
            Assert.AreEqual(8, wtrViewModel.FactorDetails[0].WeekNumber);
            Assert.AreEqual(CalendarItemType.SickAbsence, wtrViewModel.FactorDetails[1].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 21), wtrViewModel.FactorDetails[1].From);
            Assert.AreEqual(new DateTime(2014, 02, 23), wtrViewModel.FactorDetails[1].To);
            Assert.AreEqual(0, wtrViewModel.FactorDetails[1].Hours);
            Assert.AreEqual(null, wtrViewModel.FactorDetails[1].Location);
            Assert.AreEqual(8, wtrViewModel.FactorDetails[1].WeekNumber);

        }

        #endregion

        #region GetWeeksInTimeSpan
        [Test]
        public void GetWeeksInTimeSpan_SingleDay_SingleWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 04, 23);
            DateTime to = new DateTime(2014, 04, 23);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(17, result.Keys.FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 04, 23), result[17].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 23), result[17].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_TwoDaysAtBeginningOfWeek_SingleWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 04, 21);
            DateTime to = new DateTime(2014, 04, 22);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(17, result.Keys.FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 04, 21), result[17].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 22), result[17].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_TwoDaysAtEndOfWeek_SingleWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 04, 26);
            DateTime to = new DateTime(2014, 04, 27);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(17, result.Keys.FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 04, 26), result[17].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_TwoDaysAtEdgeOfTwoWeeks_TwoWeeks()
        {
            //Arrange
            DateTime from = new DateTime(2014, 04, 27);
            DateTime to = new DateTime(2014, 04, 28);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(17, result.Keys.FirstOrDefault());
            Assert.AreEqual(18, result.Keys.Skip(1).FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 04, 27), result[17].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
            Assert.AreEqual(new DateTime(2014, 04, 28), result[18].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 28), result[18].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_ManyDaysAcrossThreeWeeks_ThreeWeeks()
        {
            //Arrange
            DateTime from = new DateTime(2014, 04, 26);
            DateTime to = new DateTime(2014, 05, 07);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(17, result.Keys.FirstOrDefault());
            Assert.AreEqual(18, result.Keys.Skip(1).FirstOrDefault());
            Assert.AreEqual(19, result.Keys.Skip(2).FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 04, 26), result[17].startDate);
            Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
            Assert.AreEqual(new DateTime(2014, 04, 28), result[18].startDate);
            Assert.AreEqual(new DateTime(2014, 05, 04), result[18].endDate);
            Assert.AreEqual(new DateTime(2014, 05, 05), result[19].startDate);
            Assert.AreEqual(new DateTime(2014, 05, 07), result[19].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_DaysAcrossTwoYears_TwoWeeksSecondOneHasNumber1()
        {
            //Arrange
            DateTime from = new DateTime(2014, 12, 29);
            DateTime to = new DateTime(2015, 01, 02);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(53, result.Keys.FirstOrDefault());
            Assert.AreEqual(1, result.Keys.Skip(1).FirstOrDefault());
            Assert.AreEqual(new DateTime(2014, 12, 29), result[53].startDate);
            Assert.AreEqual(new DateTime(2014, 12, 31), result[53].endDate);
            Assert.AreEqual(new DateTime(2015, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2015, 01, 02), result[1].endDate);
        }

        [Test]
        public void GetWeeksInTimeSpan_IllegalDatesToIsLesserThanFrom_0Weeks()
        {
            //Arrange
            DateTime from = new DateTime(2014, 01, 02);
            DateTime to = new DateTime(2014, 01, 01);
            WTRViewModel model = new WTRViewModel();

            //Act
            Dictionary<int, WTRViewModel.StartEndDatePair> result = model.GetWeeksInTimeSpan(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region IntersectDatePairDicts

        [Test]
        public void IntersectDatePairDicts_CalendarItemStartedBeforeAndEndedAfterDates_TwoKeysDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 02, 03);
            DateTime to = new DateTime(2014, 02, 12);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 02, 01);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(6, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);

        }

        public void IntersectDatePairDicts_CalendarItemStartINAndEndedAfterDates_TwoKeysDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 02, 03);
            DateTime to = new DateTime(2014, 02, 12);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 02, 03);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(6, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);

        }
        public void IntersectDatePairDicts_CalendarItemStartIAndEndIn_TwoKeysDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 02, 03);
            DateTime to = new DateTime(2014, 02, 12);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 02, 03);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(6, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);
        }

        public void IntersectDatePairDicts_CalendarItemStartBeforeEndIn_TwoKeysDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 02, 03);
            DateTime to = new DateTime(2014, 02, 12);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 02, 03);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(6, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);

        }
        [Test]
        public void IntersectDatePairDicts_WTRFromInLastYearToCurrentYear_TwoKeysDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 02, 03);
            DateTime to = new DateTime(2014, 02, 12);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2013, 12, 12);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(6, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);
        }

        [Test]
        public void IntersectDatePairDicts_CalendarItemDuesForOneDay_OneKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 01, 01);
            DateTime to = new DateTime(2014, 01, 01);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2013, 12, 12);
            DateTime wtrTo = new DateTime(2014, 01, 10);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result.Keys.FirstOrDefault());
            Assert.AreEqual(1, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].endDate);
        }

        [Test]
        public void IntersectDatePairDicts_WTRFromToNotContainCalendarItem_OneKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2014, 01, 01);
            DateTime to = new DateTime(2014, 01, 01);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2015, 12, 12);
            DateTime wtrTo = new DateTime(2015, 01, 10);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void IntersectDatePairDicts_CalendarItemContainWTRFromTo_OneKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 01);
            DateTime to = new DateTime(2014, 02, 25);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 01, 01);
            DateTime wtrTo = new DateTime(2014, 2, 10);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual(1, result.Keys.FirstOrDefault());
            Assert.AreEqual(7, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result[7].endDate);
        }

        [Test]
        public void IntersectDatePairDicts_WTRFromToContainsToDateOfCalendarItem_FifthKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 01);
            DateTime to = new DateTime(2014, 02, 01);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 01, 01);
            DateTime wtrTo = new DateTime(2014, 2, 10);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(1, result.Keys.FirstOrDefault());
            Assert.AreEqual(5, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 01), result[5].endDate);
            Assert.AreEqual(new DateTime(2014, 01, 27), result[5].startDate);
        }

        [Test]
        public void IntersectDatePairDicts_WTRFromToContainsFromDateOfCalendarItem_SixKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 01);
            DateTime to = new DateTime(2014, 02, 01);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2013, 11, 01);
            DateTime wtrTo = new DateTime(2014, 01, 01);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual(48, result.Keys.FirstOrDefault());
            Assert.AreEqual(1, result.Keys.Last());
            Assert.AreEqual(new DateTime(2013, 12, 01), result[48].startDate);
            Assert.AreEqual(new DateTime(2013, 12, 01), result[48].endDate);
            Assert.AreEqual(new DateTime(2013, 12, 29), result[52].endDate);
            Assert.AreEqual(new DateTime(2013, 12, 30), result[53].startDate);
            Assert.AreEqual(new DateTime(2013, 12, 31), result[53].endDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].endDate);
        }

        [Test]
        public void IntersectDatePairDicts_CalendarItemContainsFromDateOfWTRFromTo_SixKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 01);
            DateTime to = new DateTime(2014, 02, 25);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2014, 01, 01);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(9, result.Count);
            Assert.AreEqual(1, result.Keys.FirstOrDefault());
            Assert.AreEqual(9, result.Keys.Last());
            Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
            Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 24), result[9].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 25), result[9].endDate);
        }
        [Test]
        public void IntersectDatePairDicts_CalendarItemContainsFromAndToDatesOfWTRFromTo_SixKeyDictionary()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 01);
            DateTime to = new DateTime(2014, 02, 25);
            WTRViewModel model = new WTRViewModel();

            DateTime wtrFrom = new DateTime(2013, 11, 30);
            DateTime wtrTo = new DateTime(2014, 02, 28);
            Dictionary<int, WTRViewModel.StartEndDatePair> dict = model.GetWeeksInTimeSpan(from, to);
            Dictionary<int, WTRViewModel.StartEndDatePair> wtrDict = model.GetWeeksInTimeSpan(wtrFrom, wtrTo);

            //Act
            var result = model.IntersectDatePairDicts(dict, wtrDict);
            //Assert
            Assert.AreEqual(15, result.Count);
            Assert.AreEqual(48, result.Keys.FirstOrDefault());
            Assert.AreEqual(9, result.Keys.Last());
            Assert.AreEqual(new DateTime(2013, 12, 01), result[48].startDate);
            Assert.AreEqual(new DateTime(2013, 12, 01), result[48].endDate);
            Assert.AreEqual(new DateTime(2014, 02, 24), result[9].startDate);
            Assert.AreEqual(new DateTime(2014, 02, 25), result[9].endDate);
        }
        #endregion 

        #region SortFactorDataByStartDate

        [Test]
        public void SortFactorDataByStartDate_FactorDetailsNull_NoException()
        {      
            //Arrange
            WTRViewModel model = new WTRViewModel(); 

            //Act 
            model.SortFactorDataByStartDate(); 
        
            //Assert        
            Assert.IsNull(model.FactorDetails); 
        }

        [Test]
        public void SortFactorDataByStartDate_FactorDetailsEmpty_NoException()
        {
            //Arrange
            WTRViewModel model = new WTRViewModel(mock.Object.Employees[0], new DateTime(1990, 02, 21), new DateTime(1990, 02, 22));

            //Act 
            model.SortFactorDataByStartDate();

            //Assert        
            CollectionAssert.IsEmpty(model.FactorDetails);
        }

        [Test]
        public void SortFactorDataByStartDate_FactorDetailsNotEmpty_SortedFactorDetails()
        {
            //Arrange
            WTRViewModel model = new WTRViewModel(mock.Object.Employees[0], new DateTime(2014, 02, 20), new DateTime(2014, 02, 22));
            model.FactorDetails.Reverse();
            List<FactorData> factorDetails = new List<FactorData>();
            foreach (FactorData factorData in model.FactorDetails)
	        {
		        factorDetails.Add(factorData);
	        }
            //Act 
            model.SortFactorDataByStartDate();

            //Assert        
            CollectionAssert.AreEquivalent(model.FactorDetails, factorDetails); 
            CollectionAssert.AreNotEqual(model.FactorDetails, factorDetails);
        } 

        #endregion
    }
}
