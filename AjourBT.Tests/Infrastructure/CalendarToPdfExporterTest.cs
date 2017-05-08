using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using AjourBT.Domain.Infrastructure;
using PDFjet.NET;
using AjourBT.Domain.ViewModels;
using System.IO;

namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    public class CalendarToPdfExporterTest
    {
        List<CalendarRowViewModel> calendarModel;
        List<List<Cell>> calendarTable;

        [SetUp]
        public void SetUpFixture()
        {
            calendarModel = new List<CalendarRowViewModel>();
            for (int i = 0; i < 6; i++)
            {
                calendarModel.Add(new CalendarRowViewModel());
            }
            calendarModel[0].id = "1";
            calendarModel[0].name = "Rose Johnny";
            calendarModel[1].id = "2";
            calendarModel[1].name = "Cruz Norma";
            calendarModel[2].id = "3";
            calendarModel[2].name = "George Anatoliy";
            calendarModel[3].id = "4";
            calendarModel[3].name = "Wood Harold";
            calendarModel[4].id = "5";
            calendarModel[4].name = "Olson Wayne";
            calendarModel[5].id = "6";
            calendarModel[5].name = "";

            calendarTable = new List<List<Cell>>();

        }

        #region GetLesserOfTimeSpans

        [Test]
        public void GetlesserOfTimeSpans_ToDateIsGreaterThanLastDate_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 09);
            DateTime lastDate = new DateTime(2013, 01, 08);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(9, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_LastDateIsGreaterThanToDate_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 08);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(6, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_DatesAreEqual_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 05);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_FromDateIsGreaterThanStartDate_LastDateMinusFromDateNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 07);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(-1, result);
        }

        #endregion

        #region GetColSpanForYear

        [Test]
        public void GetColSpanForYear_ToIsGreaterThanEndOfYear_TimespanUpToEndOfYear()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsEqualToTheEndOfYear_TimespanUpToEndOfYear()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 31);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsLesserThanEndOfYear_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 01);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(355, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region GetColSpanForMonth

        [Test]
        public void GetColSpanForMonth_ToIsGreaterThanEndOfMonth_TimespanUpToEndOfMonth()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(11, result);

        }

        [Test]
        public void GetColSpanForMonth_ToIsEqualToTheEndOfMonth_TimespanUpToEndOfMonth()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 31);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsLesserThanEndOfMonth_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 01);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(31, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 11, 21);
            DateTime to = new DateTime(2013, 11, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region GetColSpanForWeek

        [Test]
        public void GetColSpanForWeek_ToIsGreaterThanEndOfWeek_TimespanUpToEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsEqualToTheEndOfWeek_TimespanUpToEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 22);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsLesserThanEndOfWeek_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 02);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetColSpanForWeek_FromIsMonday_ToIsGreaterThanEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 05, 05);
            DateTime to = new DateTime(2014, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(7, result);
        }

        [Test]
        public void GetColSpanForWeek_FromIsSunday_ToIsGreaterThanEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 05, 04);
            DateTime to = new DateTime(2014, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        #endregion

        #region CreateCalendarHeader

        [Test]
        public void CreateCalendarheader_ProperDates_ProperHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2014, 1, 1);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(734, result[0].Count());
            Assert.AreEqual("2011", result[0][0].GetText());
            Assert.AreEqual("2012", result[0][2].GetText());
            Assert.AreEqual("2013", result[0][368].GetText());
            Assert.AreEqual("2014", result[0][733].GetText());
            Assert.AreEqual(2, result[0][0].GetColSpan());
            Assert.AreEqual(366, result[0][2].GetColSpan());
            Assert.AreEqual(365, result[0][368].GetColSpan());
            Assert.AreEqual(1, result[0][733].GetColSpan());
            Assert.AreEqual(734, result[1].Count);
            Assert.AreEqual("Dec", result[1][0].GetText());
            Assert.AreEqual("Jan", result[1][733].GetText());
            Assert.AreEqual(734, result[2].Count);
            Assert.AreEqual("W1", result[2][0].GetText());
            Assert.AreEqual("W1", result[2][731].GetText());
            Assert.AreEqual(734, result[3].Count);
            Assert.AreEqual("30", result[3][0].GetText());
            Assert.AreEqual("1", result[3][733].GetText());
            Assert.AreEqual(734, result[4].Count);
            Assert.AreEqual("Fr", result[4][0].GetText());
            Assert.AreEqual("We", result[4][733].GetText());
            Assert.AreEqual(result[0][0].GetTextAlignment(), Align.CENTER);
            Assert.AreEqual(result[4][733].GetTextAlignment(), Align.CENTER);

        }

        [Test]
        public void CreateCalendarheader_EqualDates_ProperHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual("2011", result[0][0].GetText());
            Assert.AreEqual(1, result[0][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[0][0].GetTextAlignment());
            Assert.AreEqual(1, result[1].Count);
            Assert.AreEqual("Dec", result[1][0].GetText());
            Assert.AreEqual(1, result[1][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[1][0].GetTextAlignment());
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual("W53", result[2][0].GetText());
            Assert.AreEqual(1, result[2][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[2][0].GetTextAlignment());
            Assert.AreEqual(1, result[3].Count);
            Assert.AreEqual("30", result[3][0].GetText());
            Assert.AreEqual(1, result[3][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[3][0].GetTextAlignment());
            Assert.AreEqual(1, result[4].Count);
            Assert.AreEqual("Fr", result[4][0].GetText());
            Assert.AreEqual(1, result[3][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[4][0].GetTextAlignment());

        }

        [Test]
        public void CreateCalendarheader_FromGreaterThanTo_EmptyHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2011, 12, 30);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getYearRow

        [Test]
        public void getYearRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2014, 1, 1);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearsRow(from, to);

            //Assert
            Assert.AreEqual(734, result.Count);
            Assert.AreEqual("2011", result[0].GetText());
            Assert.AreEqual("2012", result[2].GetText());
            Assert.AreEqual("2013", result[368].GetText());
            Assert.AreEqual("2014", result[733].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(366, result[2].GetColSpan());
            Assert.AreEqual(365, result[368].GetColSpan());
            Assert.AreEqual(1, result[733].GetColSpan());

        }

        [Test]
        public void getYearRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearsRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("2011", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getYearRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearsRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getMonthRow

        [Test]
        public void getMonthRow_ProperDatesMoreThanThreeDaysInMonthsOnTable_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2013, 1, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthsRow(from, to);

            //Assert
            Assert.AreEqual(399, result.Count);
            Assert.AreEqual("Dec", result[0].GetText());
            Assert.AreEqual("February", result[33].GetText());
            Assert.AreEqual("December", result[337].GetText());
            Assert.AreEqual("January", result[368].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(29, result[33].GetColSpan());
            Assert.AreEqual(31, result[337].GetColSpan());
            Assert.AreEqual(31, result[368].GetColSpan());

        }

        [Test]
        public void getMonthRow_EqualDatesLessThanThreeDaysInMonthOnTable_ProperRowShortedMonthName()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthsRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Dec", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getMonthRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthsRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }


        #endregion

        #region getWeekRow

        [Test]
        public void getWeekRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeeksRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("W1", result[0].GetText());
            Assert.AreEqual("W2", result[2].GetText());
            Assert.AreEqual("W3", result[380].GetText());
            Assert.AreEqual("W4", result[387].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(7, result[2].GetColSpan());
            Assert.AreEqual(7, result[380].GetColSpan());
            Assert.AreEqual(3, result[387].GetColSpan());

        }

        [Test]
        public void getWeekRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeeksRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("W54", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getWeekRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeeksRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }


        #endregion

        #region getDaysRow

        [Test]
        public void getDaysRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("31", result[0].GetText());
            Assert.AreEqual("2", result[2].GetText());
            Assert.AreEqual("14", result[380].GetText());
            Assert.AreEqual("21", result[387].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());
            Assert.AreEqual(1, result[2].GetColSpan());
            Assert.AreEqual(1, result[380].GetColSpan());
            Assert.AreEqual(1, result[387].GetColSpan());

        }

        [Test]
        public void getDaysRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("31", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getDaysRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getDaysOfWeekRow

        [Test]
        public void getDaysOfWeekRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("Sa", result[0].GetText());
            Assert.AreEqual("Su", result[1].GetText());
            Assert.AreEqual("Mo", result[2].GetText());
            Assert.AreEqual("Tu", result[3].GetText());
            Assert.AreEqual("We", result[4].GetText());
            Assert.AreEqual("Th", result[5].GetText());
            Assert.AreEqual("Fr", result[6].GetText());
            Assert.AreEqual("Sa", result[7].GetText());
            Assert.AreEqual("Mo", result[380].GetText());
            Assert.AreEqual("Mo", result[387].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());
            Assert.AreEqual(1, result[1].GetColSpan());
            Assert.AreEqual(1, result[2].GetColSpan());
            Assert.AreEqual(1, result[3].GetColSpan());
            Assert.AreEqual(1, result[4].GetColSpan());
            Assert.AreEqual(1, result[5].GetColSpan());
            Assert.AreEqual(1, result[6].GetColSpan());
            Assert.AreEqual(1, result[7].GetColSpan());
            Assert.AreEqual(1, result[380].GetColSpan());
            Assert.AreEqual(1, result[387].GetColSpan());

        }

        [Test]
        public void getDaysOfWeekRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mo", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getDaysOfWeekRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region CreateCalendarLeftPanel

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsNotEmpty_ProperLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(calendarModel);

            //Assert        

            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(true, result[0][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, result[0][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, result[2][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, result[2][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, result[4][0].GetBorder(Border.TOP));
            Assert.AreEqual(true, result[4][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual("Rose Johnny", result[5][0].GetText());
            Assert.AreEqual("Olson Wayne", result[9][0].GetText());

        }

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsEmpty_EmptyLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(new List<CalendarRowViewModel>());

            //Assert        

            Assert.AreEqual(0, result.Count);

        }

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsNull_EmptyLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(null);

            //Assert        

            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region                     CreateEmployeesList

        [Test]
        public void CreateEmployeesList_CalendarModelIsNotEmpty_ProperEmployeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(calendarModel, calendarTable);

            //Assert        

            Assert.AreEqual(5, calendarTable.Count);

            Assert.AreEqual("Rose Johnny", calendarTable[0][0].GetText());
            Assert.AreEqual("Olson Wayne", calendarTable[4][0].GetText());

        }

        [Test]
        public void CreateEmployeesList_CalendarModelIsEmpty_EmptyEmplyeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(new List<CalendarRowViewModel>(), calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }

        [Test]
        public void CreateEmployeesList_CalendarModelIsNull_EmptyEmployeeList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(null, calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }
        [Test]
        public void CreateEmployeesList_CalendaTableIsNull_NoException()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(calendarModel, null);

            //Assert        

        }

        #endregion

        #region CreateHeaderSpacer

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsNotEmpty_ProperEmployeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(calendarModel, calendarTable);

            //Assert        

            Assert.AreEqual(5, calendarTable.Count);

            Assert.AreEqual(true, calendarTable[0][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, calendarTable[0][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, calendarTable[2][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, calendarTable[2][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, calendarTable[4][0].GetBorder(Border.TOP));
            Assert.AreEqual(true, calendarTable[4][0].GetBorder(Border.BOTTOM));

        }

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsEmpty_EmptyEmplyeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(new List<CalendarRowViewModel>(), calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsNull_EmptyEmployeeList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(null, calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }
        [Test]
        public void CreateHeaderSpacer_CalendarTableIsNull_NoException()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(calendarModel, null);

            //Assert        

        }

        #endregion

        #region CreateEmptyCalendarBody

        [Test]
        public void CreateEmptyCalendarBody_FromLesserThanToProperCalendarData_TableWithEmptyCells()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 31);
            DateTime to = new DateTime(2011, 01, 11);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(12, result[0].Count);
            Assert.AreEqual(12, result[4].Count);
            Assert.IsNull(result[0][0].GetText());
            Assert.IsNull(result[4][11].GetText());
        }

        [Test]
        public void CreateEmptyCalendarBody_ToEqualsToFromProperCalendarData_TableWithEmptyCells()
        {
            //Arrange
            DateTime from = new DateTime(2009, 01, 10);
            DateTime to = new DateTime(2009, 01, 10);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual(1, result[4].Count);
            Assert.IsNull(result[0][0].GetText());
            Assert.IsNull(result[4][0].GetText());
        }

        [Test]
        public void CreateEmptyCalendarBody_FromGreaterThanToProperCalendarData_TableWithEmptyRows()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(0, result[0].Count);
            Assert.AreEqual(0, result[4].Count);
        }

        [Test]
        public void CreateEmptyCalendarBody_EmptyCalendarData_EmptyTable()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(new List<CalendarRowViewModel>(), from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        [Test]
        public void CreateEmptyCalendarBody_NullCalendarData_EmptyTable()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(null, from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region GetColumnIndexForCalendarItem

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAndEndsBetweenDates_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 31),
                To = new DateTime(2011, 12, 31),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAtStartAndEndsAtEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 30),
                To = new DateTime(2012, 1, 1),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsBeforeStartAndEndsAtStartDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 30),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAtEndAndEndsAfterEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 1),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(2, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAfterEndDate_NegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 2),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btEndsBeforeEndDate_NegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 29),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsBeforeStartAndEndsAfterEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(0, result);

        }

        #endregion

        #region GetColumnSpanForCalendarItem

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAndEndsBetweenDates_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 31),
                To = new DateTime(2011, 12, 31),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAtStartAndEndsAtEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 30),
                To = new DateTime(2012, 1, 1),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(3, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsBeforeStartAndEndsAtStartDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 30),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAtEndAndEndsAfterEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 1),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAfterEndDate_NegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 2),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btEndsBeforeEndDate_NegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 29),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsBeforeStartAndEndsAfterEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(3, result);

        }

        #endregion

        #region GetColorForCalendarItem

        [Test]
        public void GetColorForCalendarItem_UnknownClass_GanttWhite()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttWhite, result);
        }


        [Test]
        public void GetColorForCalendarItem_GanttGreenClass_GanttGreen()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttGreen" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result);
        }

        [Test]
        public void GetColorForCalendarItem_DarkGreenClass_GanttDarkGreen()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttDarkGreen" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttDarkGreen, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttOrangeClass_GanttOrange()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttOrange" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttOrange, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttBlueClass_GanttBlue()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttBlue" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttBlue, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttVioletClass_GanttViolet()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttViolet" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttViolet, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttRedClass_GanttRed()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttRed" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttRed, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttMagentaClass_GanttMagenta()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttMagenta" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttMagenta, result);
        }

        [Test]
        public void GetColorForCalendarItem_GanttYellowClass_GanttYellow()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttYellow" };

            //Act
            int result = CalendarToPdfExporter.GetColorForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttYellow, result);
        }
        #endregion

        #region ApplyHolidays

        [Test]
        public void ApplyHolidays_AllParametersAreProper_ProperColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, from, to);

            //Assert      
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][1].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][5].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][11].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][11].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayYellow, calendar[0][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayYellow, calendar[4][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayYellow, calendar[0][4].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayYellow, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyHolidays_CalendarIsNull_NoException()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            CalendarToPdfExporter.ApplyHolidays(null, holidays, from, to);

            //Assert      
        }

        [Test]
        public void ApplyHolidays_CalendarIsEmpty_NoColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = new List<List<Cell>>();

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, from, to);

            //Assert      
            Assert.IsEmpty(calendar);
        }

        [Test]
        public void ApplyHolidays_HolidaysAreNull_NoException()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, null, from, to);

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyHolidays_HolidaysAreEmpty_NoColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, from, to);

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyHolidays_FromIsgreaterThanTo_NoColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, to, from);

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyHolidays_FromMinusToIsGreaterThanCellsInRow_NoColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, from, to.AddDays(-1));

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyHolidays_FromMinusToIsLesserThanCellsInRow_NoColorsApplied()
        {
            //Arrange
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidays(calendar, holidays, from, to.AddDays(1));

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        #endregion

        #region ApplyWeekends

        [Test]
        public void ApplyWeekends_AllParametersAreProper_ColumnsOfSundaysAndSaturdaysGetBgColor()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyWeekends(calendar, from, to);

            //Assert      
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][3].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][3].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][4].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][4].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][10].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][10].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[0][11].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayPink, calendar[4][11].GetBgColor());
        }
        [Test]
        public void ApplyWeekends_CalendarIsNull_NoException()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            //Act
            CalendarToPdfExporter.ApplyWeekends(null, from, to);

            //Assert      
        }

        [Test]
        public void ApplyWeekends_CalendarIsEmpty_NoColorsApplied()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = new List<List<Cell>>();

            //Act
            CalendarToPdfExporter.ApplyWeekends(calendar, from, to);

            //Assert      
            Assert.IsEmpty(calendar);
        }

        [Test]
        public void ApplyWeekends_FromMinusToIsGreaterThanCellsInRow_NoColorsApplied()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyWeekends(calendar, from, to.AddDays(-1));

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        [Test]
        public void ApplyWeekends_FromMinusToIsLesserThanCellsInRow_NoColorsApplied()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyWeekends(calendar, from, to.AddDays(1));

            //Assert      
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][11].GetBgColor());
            Assert.AreEqual(-1, calendar[4][11].GetBgColor());
            Assert.AreEqual(-1, calendar[0][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[0][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][4].GetBgColor());
        }

        #endregion

        #region ColorRow

        [Test]
        public void ColorRow_AllParametersOK_RowIsColoredExceptForTheFirstCell()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ColorRow(calendar, 2, 0xff00ff);

            //Assert
            Assert.AreEqual(-1, calendar[0][13].GetBgColor());
            Assert.AreEqual(-1, calendar[2][0].GetBgColor());
            Assert.AreEqual(0xff00ff, calendar[2][1].GetBgColor());
            Assert.AreEqual(0xff00ff, calendar[2][13].GetBgColor());
            Assert.AreEqual(-1, calendar[3][0].GetBgColor());

        }

        [Test]
        public void ColorRow_CalendarIsNull_NoException()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            //Act
            CalendarToPdfExporter.ColorRow(null, 2, 0xff00ff);

            //Assert

        }

        [Test]
        public void ColorRow_CalendarIsEmpty_NoException()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = new List<List<Cell>>();

            //Act
            CalendarToPdfExporter.ColorRow(calendar, 2, 0xff00ff);

            //Assert


        }

        [Test]
        public void ColorRow_RowNumberIsGreaterThanLastRowNumber_NothingIsColored()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ColorRow(calendar, 5, 0xff00ff);

            //Assert
            Assert.AreEqual(-1, calendar[0][13].GetBgColor());
            Assert.AreEqual(-1, calendar[2][0].GetBgColor());
            Assert.AreEqual(-1, calendar[2][1].GetBgColor());
            Assert.AreEqual(-1, calendar[2][13].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());

        }

        [Test]
        public void ColorRow_RowNumberIsNegative_NothingIsColored()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ColorRow(calendar, -1, 0xff00ff);

            //Assert
            Assert.AreEqual(-1, calendar[0][13].GetBgColor());
            Assert.AreEqual(-1, calendar[2][0].GetBgColor());
            Assert.AreEqual(-1, calendar[2][1].GetBgColor());
            Assert.AreEqual(-1, calendar[2][13].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());

        }

        [Test]
        public void ColorRow_RowNumberIsZero_ZeroRowIsColored()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ColorRow(calendar, -1, 0xff00ff);

            //Assert
            Assert.AreEqual(-1, calendar[1][13].GetBgColor());
            Assert.AreEqual(-1, calendar[0][0].GetBgColor());
            Assert.AreEqual(-1, calendar[0][1].GetBgColor());
            Assert.AreEqual(-1, calendar[0][13].GetBgColor());

        }

        #endregion

        #region ApplyHolidaysToHeader

        [Test]
        public void ApplyHolidaysToHeader_AllParametersAreOK_HolidaysMarkedWithBgColor()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to.AddDays(1));

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, holidays, from, to);

            //Assert       
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[3][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[3][3].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[3][5].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[3][12].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[4][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[4][3].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[4][5].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.holidayOrange, calendar[4][12].GetBgColor());
            Assert.AreEqual(-1, calendar[3][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][8].GetBgColor());
        }

        [Test]
        public void ApplyHolidaysToHeader_CalendarNull_NoException()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(null, holidays, from, to);

            //Assert       

        }

        [Test]
        public void ApplyHolidaysToHeader_CalendarEmpty_NoException()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = new List<List<Cell>>();

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, holidays, from, to);

            //Assert       

        }

        [Test]
        public void ApplyHolidaysToHeader_HolidaysAreNull_NothingIsApplied()
        {
            //Arrange

            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to.AddDays(1));

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, null, from, to);

            //Assert       
            Assert.AreEqual(-1, calendar[3][2].GetBgColor());
            Assert.AreEqual(-1, calendar[3][3].GetBgColor());
            Assert.AreEqual(-1, calendar[3][5].GetBgColor());
            Assert.AreEqual(-1, calendar[3][12].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][3].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());
            Assert.AreEqual(-1, calendar[4][12].GetBgColor());
            Assert.AreEqual(-1, calendar[3][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][8].GetBgColor());
        }

        [Test]
        public void ApplyHolidaysToHeader_HolidaysAreEmpty_NothingIsApplied()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to.AddDays(1));

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, holidays, from, to);

            //Assert       
            Assert.AreEqual(-1, calendar[3][2].GetBgColor());
            Assert.AreEqual(-1, calendar[3][3].GetBgColor());
            Assert.AreEqual(-1, calendar[3][5].GetBgColor());
            Assert.AreEqual(-1, calendar[3][12].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][3].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());
            Assert.AreEqual(-1, calendar[4][12].GetBgColor());
            Assert.AreEqual(-1, calendar[3][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][8].GetBgColor());
        }

        [Test]
        public void ApplyHolidaysToHeader_CalendarIsTooBig_NothingIsApplied()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to.AddDays(2));

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, holidays, from, to);

            //Assert       
            Assert.AreEqual(-1, calendar[3][2].GetBgColor());
            Assert.AreEqual(-1, calendar[3][3].GetBgColor());
            Assert.AreEqual(-1, calendar[3][5].GetBgColor());
            Assert.AreEqual(-1, calendar[3][12].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][3].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());
            Assert.AreEqual(-1, calendar[4][12].GetBgColor());
            Assert.AreEqual(-1, calendar[3][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][8].GetBgColor());
        }

        [Test]
        public void ApplyHolidaysToHeader_CalendarIsTooSmall_NothingIsApplied()
        {
            //Arrange

            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });
            List<List<Cell>> calendar = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Act
            CalendarToPdfExporter.ApplyHolidaysToHeader(calendar, holidays, from, to);

            //Assert       
            Assert.AreEqual(-1, calendar[3][2].GetBgColor());
            Assert.AreEqual(-1, calendar[3][3].GetBgColor());
            Assert.AreEqual(-1, calendar[3][5].GetBgColor());
            Assert.AreEqual(-1, calendar[3][12].GetBgColor());
            Assert.AreEqual(-1, calendar[4][2].GetBgColor());
            Assert.AreEqual(-1, calendar[4][3].GetBgColor());
            Assert.AreEqual(-1, calendar[4][5].GetBgColor());
            Assert.AreEqual(-1, calendar[4][12].GetBgColor());
            Assert.AreEqual(-1, calendar[3][4].GetBgColor());
            Assert.AreEqual(-1, calendar[4][8].GetBgColor());
        }

        #endregion

        #region GetAbbreviationForCalendarItem

        [Test]
        public void GetAbbreviationForCalendarItem_UnknownClass_EmptyString()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttGreenClass_GanttGreen()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttGreen" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.BT, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_DarkGreenClass_GanttDarkGreen()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttDarkGreen" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.Journey, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttOrangeClass_GanttOrange()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttOrange" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.ReclaimedOvertime, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttBlueClass_GanttBlue()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttBlue" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.PaidVacation, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttVioletClass_GanttViolet()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttViolet" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.SickAbsence, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttRedClass_GanttRed()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttRed" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.UnpaidVacation, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttMagentaClass_GanttMagenta()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttMagenta" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.OvertimeForReclaim, result);
        }

        [Test]
        public void GetAbbreviationForCalendarItem_GanttYellowClass_GanttYellow()
        {
            //Arrange 
            CalendarItemViewModel calendarItem = new CalendarItemViewModel() { customClass = "ganttYellow" };

            //Act
            string result = CalendarToPdfExporter.GetAbbreviationForCalendarItem(calendarItem);

            //Assert
            Assert.AreEqual(CalendarToPdfExporter.Abbreviations.PrivateMinus, result);
        }

        #endregion

        #region CreateLegend

        [Test]
        public void CreateLegend_NoParameters_ProperLegendCreated()
        {
            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateLegend();

            //Assert        
            Assert.AreEqual(9, result.Count);

            Assert.AreEqual("Legend: ", result[0][0].GetText());
            Assert.AreEqual(2, result[0][0].GetColSpan());

            Assert.AreEqual("Business Trip", result[1][1].GetText());
            Assert.AreEqual("Journey", result[2][1].GetText());
            Assert.AreEqual("Overtime for Reclaim", result[3][1].GetText());
            Assert.AreEqual("Paid Vacation", result[4][1].GetText());
            Assert.AreEqual("Private Minus", result[5][1].GetText());
            Assert.AreEqual("Reclaimed Overtime", result[6][1].GetText());
            Assert.AreEqual("Sick Absence", result[7][1].GetText());
            Assert.AreEqual("Unpaid Vacation", result[8][1].GetText());

            Assert.AreEqual("BT", result[1][0].GetText());
            Assert.AreEqual("J", result[2][0].GetText());
            Assert.AreEqual("OR", result[3][0].GetText());
            Assert.AreEqual("PV", result[4][0].GetText());
            Assert.AreEqual("P-", result[5][0].GetText());
            Assert.AreEqual("RO", result[6][0].GetText());
            Assert.AreEqual("SA", result[7][0].GetText());
            Assert.AreEqual("UV", result[8][0].GetText());

            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[1][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttDarkGreen, result[2][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttMagenta, result[3][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttBlue, result[4][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttYellow, result[5][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttOrange, result[6][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttViolet, result[7][0].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttRed, result[8][0].GetBgColor());
        }

        #endregion

        #region GeneratePdf

        [Test]
        public void GeneratePdf_ProperParameters_ProperMemoryStream()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            var result = CalendarToPdfExporter.GeneratePDF(calendarModel, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(MemoryStream), result.GetType());
            Assert.AreNotEqual(0, result.GetBuffer().Length);
        }

        [Test]
        public void GeneratePdf_FromGreaterthanTo_EmptyMemoryStream()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2012, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            var result = CalendarToPdfExporter.GeneratePDF(calendarModel, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(MemoryStream), result.GetType());
            Assert.AreEqual(0, result.GetBuffer().Length);
        }

        [Test]
        public void GeneratePdf_CalendarModelIsNull_EmptyMemoryStream()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            var result = CalendarToPdfExporter.GeneratePDF(null, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(MemoryStream), result.GetType());
            Assert.AreEqual(0, result.GetBuffer().Length);
        }

        [Test]
        public void GeneratePdf_CalendarmodelIsEmpty_EmptyMemoryStream()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            var result = CalendarToPdfExporter.GeneratePDF(new List<CalendarRowViewModel>(), holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(MemoryStream), result.GetType());
            Assert.AreEqual(0, result.GetBuffer().Length);
        }


        #endregion

        #region getCalendarBody

        [Test]
        public void getCalendarBody_ProperCalendarItemNotBT_ProperBodyMyTestMethod()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttOrange",
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(2)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(2, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttOrange, result[0][1].GetBgColor());
        }

        [Test]
        public void getCalendarBody_OneDayBT_ProperBody ()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(1)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(1, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][1].GetBgColor());
        }

        [Test]
        public void getCalendarBody_TwoDaysBT_ProperBody()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(2)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(2, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][1].GetBgColor());
        }

        [Test]
        public void getCalendarBody_PairedFirstItemButNotBT_ProperBody()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttOrange",   
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(2)
            });

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(2),
                to = dateFrom.AddDays(4)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(2, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttOrange, result[0][1].GetBgColor());
            Assert.AreNotEqual(false, result[0][1].GetBorder(Border.RIGHT));
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.LEFT));
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.RIGHT));
            Assert.AreEqual(3, result[0][2].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][2].GetBgColor());
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.LEFT));

        } 


        [Test]
        public void getCalendarBody_PairedSecondItemButNotBT_ProperBody()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(2)
            });

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttOrange",
                from = dateFrom.AddDays(2),
                to = dateFrom.AddDays(4)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(2, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][1].GetBgColor());
            Assert.AreNotEqual(false, result[0][1].GetBorder(Border.RIGHT));
            Assert.AreNotEqual(CalendarToPdfExporter.PdfColors.pairedBTGreen, result[0][2].GetBgColor());
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.LEFT));
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.RIGHT));
            Assert.AreEqual(3, result[0][2].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttOrange, result[0][2].GetBgColor());
            Assert.AreNotEqual(false, result[0][2].GetBorder(Border.LEFT));

        } 

        [Test]
        public void getCalendarBody_PairedBTs_ProperBody()
        {
            //Arrange
            DateTime dateFrom = new DateTime(2012, 01, 01);
            DateTime dateTo = new DateTime(2012, 01, 04);

            calendarModel[0].values = new List<CalendarItemViewModel>();

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(1),
                to = dateFrom.AddDays(2)
            });

            calendarModel[0].values.Add(new CalendarItemViewModel()
            {
                customClass = "ganttGreen",
                from = dateFrom.AddDays(2),
                to = dateFrom.AddDays(4)
            });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarBody(calendarModel, new List<Holiday>(), dateFrom, dateTo.AddDays(2));

            //Assert  
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(6, result[0].Count);
            Assert.AreEqual(1, result[0][1].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][1].GetBgColor());
            Assert.AreEqual(false, result[0][1].GetBorder(Border.RIGHT));
            Assert.AreEqual(1, result[0][2].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.pairedBTGreen, result[0][2].GetBgColor());
            Assert.AreEqual(false, result[0][2].GetBorder(Border.LEFT));
            Assert.AreEqual(false, result[0][2].GetBorder(Border.RIGHT));
            Assert.AreEqual(2, result[0][3].GetColSpan());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.ganttGreen, result[0][3].GetBgColor());
            Assert.AreEqual(false, result[0][3].GetBorder(Border.LEFT));

        } 

        #endregion

        #region CreateCalendar

        [Test]
        public void CreateCalendar_ProperParameters_ProperCalendar()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendar(calendarModel, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(List<List<Cell>>), result.GetType());
            Assert.AreNotEqual(0, result.Count);
        }

        [Test]
        public void CreateCalendar_FromGreaterthanTo_EmptyCalendar()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2012, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendar(calendarModel, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(List<List<Cell>>), result.GetType());
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void CreateCalendar_CalendarModelIsNull_EmptyCalendar()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendar(null, holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(List<List<Cell>>), result.GetType());
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void CreateCalendar_CalendarmodelIsEmpty_EmptyCalendar()
        {
            //Arrange 
            List<Holiday> holidays = new List<Holiday>();
            DateTime from = new DateTime(2012, 12, 26);
            DateTime to = new DateTime(2013, 1, 8);

            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 30), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 28), IsPostponed = false });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2013, 1, 6), IsPostponed = true });
            holidays.Add(new Holiday() { HolidayDate = new DateTime(2012, 12, 27), IsPostponed = true });

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendar(new List<CalendarRowViewModel>(), holidays, from, to);

            //Assert
            Assert.AreEqual(typeof(List<List<Cell>>), result.GetType());
            Assert.AreEqual(0, result.Count);
        }




        #endregion

        #region MarkToday

        [Test]
        public void MarkToday_TodayIsEqualToFrom_TodayMarked()
        {
            //Arrange
            DateTime from  = DateTime.Now.ToLocalTimeAzure();
            DateTime to = DateTime.Now.ToLocalTimeAzure().AddDays(1); 

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to); 

            //Assert        
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[3][1].GetBgColor( )); 
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[4][1].GetBgColor( )); 
        }

        [Test]
        public void MarkToday_TodayIsEqualToTo_TodayMarked()
        {
            //Arrange
            DateTime from = DateTime.Now.ToLocalTimeAzure().AddDays(-1);
            DateTime to = DateTime.Now.ToLocalTimeAzure();

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to);

            //Assert        
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[3][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[4][2].GetBgColor()); 

        }

        [Test]
        public void MarkToday_TodayIsBetweenFromAndTo_TodayMarked()
        {
            //Arrange
            DateTime from = DateTime.Now.ToLocalTimeAzure().AddDays(-1);
            DateTime to = DateTime.Now.ToLocalTimeAzure().AddDays(1);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to);

            //Assert        
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[3][2].GetBgColor());
            Assert.AreEqual(CalendarToPdfExporter.PdfColors.todayGreen, calendar[4][2].GetBgColor()); 
        }

        [Test]
        public void MarkToday_TodayIsLesserThanFrom_TodayNotMarked()
        {
            //Arrange
            DateTime from = DateTime.Now.ToLocalTimeAzure().AddDays(1);
            DateTime to = DateTime.Now.ToLocalTimeAzure().AddDays(2);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to);

            //Assert        
            Assert.AreEqual(-1, calendar[3][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor()); 
        }

        [Test]
        public void MarkToday_TodayIsGreaterThanTo_TodayNotMarked()
        {
            //Arrange
            DateTime from = DateTime.Now.ToLocalTimeAzure().AddDays(-2);
            DateTime to = DateTime.Now.ToLocalTimeAzure().AddDays(-1);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to);

            //Assert        
            Assert.AreEqual(-1, calendar[3][1].GetBgColor());
            Assert.AreEqual(-1, calendar[4][1].GetBgColor()); 
        }

        [Test]
        public void MarkToday_ToIsLesserThanFrom_NoException()
        {
            //Arrange
            DateTime from = DateTime.Now.ToLocalTimeAzure().AddDays(1);
            DateTime to = DateTime.Now.ToLocalTimeAzure().AddDays(-1);

            List<List<Cell>> calendar = CalendarToPdfExporter.CreateCalendar(calendarModel, null, from, to);

            //Act 
            CalendarToPdfExporter.MarkToday(calendar, from, to);

            //Assert        

        }

        #endregion 




    }
}
