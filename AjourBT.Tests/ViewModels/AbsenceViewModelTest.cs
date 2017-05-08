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
    public class AbsenceViewModelTest
    {
        Mock<IRepository> mock;
        AbsenceViewModel absenceViewModel;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock(); 
            absenceViewModel = new AbsenceViewModel(); 
        }

        #region Constructor

        [Test]
        public void Constructor_NoParameters_AllListsAreInitializedAsEmpty()
        {
            //Arrange 

            //Act

            //Assert 
            Assert.IsNotNull(absenceViewModel.BusinessTrips);
            Assert.IsNotNull(absenceViewModel.Journeys);
            Assert.IsNotNull(absenceViewModel.Overtimes);
            Assert.IsNotNull(absenceViewModel.Sickness);
            Assert.IsNotNull(absenceViewModel.Vacations);
            Assert.IsEmpty(absenceViewModel.BusinessTrips);
            Assert.IsEmpty(absenceViewModel.Journeys);
            Assert.IsEmpty(absenceViewModel.Overtimes);
            Assert.IsEmpty(absenceViewModel.Sickness);
            Assert.IsEmpty(absenceViewModel.Vacations);
            Assert.AreEqual("", absenceViewModel.FirstName);
            Assert.AreEqual("", absenceViewModel.LastName);
            Assert.AreEqual("", absenceViewModel.EID);
            Assert.AreEqual("", absenceViewModel.Department); 

        }

        #endregion

        #region AppendLocationData

        [Test]
        public void AppendLocationData_DataAppendedCorrectly()
        {
            //Arrange 
            StringBuilder stringBuilder = new StringBuilder(); 
            CalendarItem calendarItem = new CalendarItem{Location = "abc"}; 
            
            //Act
            AbsenceViewModel.AppendLocationData(stringBuilder, calendarItem); 

            //Assert 
            Assert.AreEqual(stringBuilder.ToString(), " (abc)"); 

        }

        #endregion 

        #region AppendStringifiedCalendarItem

        [Test]
        public void AppendStringifiedCalendarItem_Not_BT_ItemAppendedCorrectly()
        {
            //Arrange 
            DateTime from =  new DateTime(2014,01,01); 
            DateTime to = new DateTime(2014,02,02); 
            StringBuilder stringBuilder = new StringBuilder();
            CalendarItem calendarItem = new CalendarItem { Location = "abc" , From = from, To = to}; 

            //Act
            AbsenceViewModel.AppendStringifiedCalendarItem(stringBuilder, calendarItem);

            //Assert 
            Assert.AreEqual(stringBuilder.ToString(), from.ToString(String.Format("dd.MM.yyyy")+" - "+to.ToString(String.Format("dd.MM.yyyy"))));

        }

        [Test]
        public void AppendStringifiedCalendarItem_BT_ItemAppendedCorrectly()
        {
            //Arrange 
            DateTime from = new DateTime(2014, 01, 01);
            DateTime to = new DateTime(2014, 02, 02);
            StringBuilder stringBuilder = new StringBuilder();
            CalendarItem calendarItem = new CalendarItem { Location = "abc", From = from, To = to, Type = CalendarItemType.BT };

            //Act
            AbsenceViewModel.AppendStringifiedCalendarItem(stringBuilder, calendarItem);

            //Assert 
            Assert.AreEqual(stringBuilder.ToString(), from.ToString(String.Format("dd.MM.yyyy") + " - " + to.ToString(String.Format("dd.MM.yyyy")))+" (abc)");

        }

        #endregion

        #region StringifyCalendarItems

        [Test]
        public void StringifyCalendarItems_Items_StringifiedItems()
        {
            //Arrange 
            DateTime from = new DateTime(2014, 01, 01);
            DateTime to = new DateTime(2014, 02, 02);

            List<CalendarItem> calendarItems = new List<CalendarItem>{ 

                new CalendarItem { Location = "abc", From = from, To = to, Type = CalendarItemType.BT }, 
                new CalendarItem { Location = "abc", From = from, To = to, Type = CalendarItemType.BT }
            };


            //Act
            string result = AbsenceViewModel.StringifyCalendarItems(calendarItems);

            //Assert 
            Assert.AreEqual(result, 
                from.ToString(String.Format("dd.MM.yyyy") + " - " + to.ToString(String.Format("dd.MM.yyyy"))) + " (abc)" + "\r\n" +
                from.ToString(String.Format("dd.MM.yyyy") + " - " + to.ToString(String.Format("dd.MM.yyyy"))) + " (abc)" );

        }

        [Test]
        public void StringifyCalendarItems_NoItems_EmptyString()
        {
            //Arrange 

            List<CalendarItem> calendarItems = new List<CalendarItem>();


            //Act
            string result = AbsenceViewModel.StringifyCalendarItems(calendarItems);

            //Assert 
            Assert.AreEqual(result, "");
        }

        #endregion

        #region ContainsCalendarItems

        [Test]
        public void ContainsCalendarItems_NoItems_False()
        {
            //Arrange 

            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, false);
        }

        [Test]
        public void ContainsCalendarItems_Journeys_Truee()
        {
            //Arrange 
            absenceViewModel.Journeys.Add(new CalendarItem()); 

            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ContainsCalendarItems_Overtimes_True()
        {
            //Arrange 
            absenceViewModel.Overtimes.Add(new CalendarItem()); 

            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ContainsCalendarItems_BusinessTrips_True()
        {
            //Arrange 
            absenceViewModel.BusinessTrips.Add(new CalendarItem()); 

            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ContainsCalendarItems_Sickness_True()
        {
            //Arrange 
            absenceViewModel.Sickness.Add(new CalendarItem()); 
            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, true);
        }

        [Test]
        public void ContainsCalendarItems_Vacations_True()
        {
            //Arrange 
            absenceViewModel.Vacations.Add(new CalendarItem()); 

            //Act
            bool result = absenceViewModel.ContainsCalendarItems;

            //Assert 
            Assert.AreEqual(result, true);
        }

        #endregion

        #region prepareToXLSExportABM

        [Test]
        public void prepareToXLSExportABM_AllPropertiesAreNullOrEmpty_EmptyString()
        {
            //Arrange 

            //Act
            List<string> result = absenceViewModel.PrepareToXLSExportABM();

            //Assert 
            Assert.AreEqual(0, result.Count);            
        }

        [Test]
        public void prepareToXLSExportABM_CalendarItemsNullableAreNull_StringifiedNullableAreEmptyStrings()
        {
            //Arrange 
            absenceViewModel.BusinessTrips.Add(new CalendarItem { From = new DateTime(2012, 01, 01) });
            absenceViewModel.Journeys.Add(new CalendarItem { From = new DateTime(2012, 01, 02) });
            absenceViewModel.Overtimes.Add(new CalendarItem { From = new DateTime(2012, 01, 03) });
            absenceViewModel.Sickness.Add(new CalendarItem { From = new DateTime(2012, 01, 04) });
            absenceViewModel.Vacations.Add(new CalendarItem { From = new DateTime(2012, 01, 05) });

            //Act
            List<string> result = absenceViewModel.PrepareToXLSExportABM();

            //Assert 
            Assert.AreEqual(8, result.Count);
            Assert.AreEqual("", result[0]);
            Assert.AreEqual("", result[1]);
            Assert.AreEqual("", result[2]);
            Assert.AreEqual(new DateTime(2012, 01, 02).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[3]);
            Assert.AreEqual(new DateTime(2012, 01, 01).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[4]);
            Assert.AreEqual(new DateTime(2012, 01, 03).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[5]);
            Assert.AreEqual(new DateTime(2012, 01, 04).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[6]);
            Assert.AreEqual(new DateTime(2012, 01, 05).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[7]);
        }

        [ Test]
        public void prepareToXLSExportABM_CalendarItemsNullableAreNotNull_Stringified()
        {
            //Arrange 
            absenceViewModel.Department = "abc"; 
            absenceViewModel.FirstName = "de"; 
            absenceViewModel.LastName = "f"; 
            absenceViewModel.EID = "ghi"; 

            absenceViewModel.BusinessTrips.Add(new CalendarItem { From = new DateTime(2012, 01, 01) });
            absenceViewModel.Journeys.Add(new CalendarItem { From = new DateTime(2012, 01, 02) });
            absenceViewModel.Overtimes.Add(new CalendarItem { From = new DateTime(2012, 01, 03) });
            absenceViewModel.Sickness.Add(new CalendarItem { From = new DateTime(2012, 01, 04) });
            absenceViewModel.Vacations.Add(new CalendarItem { From = new DateTime(2012, 01, 05) }); 

            //Act
            List<string> result = absenceViewModel.PrepareToXLSExportABM();

            //Assert 
            Assert.AreEqual(8, result.Count);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("f de", result[1]);
            Assert.AreEqual("ghi", result[2]);
            Assert.AreEqual(new DateTime(2012, 01, 02).ToString("dd.MM.yyyy")+" - "+new DateTime().ToString("dd.MM.yyyy"), result[3]);
            Assert.AreEqual(new DateTime(2012, 01, 01).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[4]);
            Assert.AreEqual(new DateTime(2012, 01, 03).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[5]);
            Assert.AreEqual(new DateTime(2012, 01, 04).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[6]);
            Assert.AreEqual(new DateTime(2012, 01, 05).ToString("dd.MM.yyyy") + " - " + new DateTime().ToString("dd.MM.yyyy"), result[7]);
        }

        #endregion

        #region FullName

        [Test]
        public void FullName_NoNames_EmptyString()
        {
            //Arrange 

            //Act

            //Assert 
            Assert.AreEqual("", absenceViewModel.FullName); 
        }

        [Test]
        public void FullName_Names_TrimmedCombinationOfNames()
        {
            //Arrange 
            absenceViewModel.FirstName = " abc ";
            absenceViewModel.LastName = " def "; 

            //Act

            //Assert 
            Assert.AreEqual("def abc", absenceViewModel.FullName);
        }

        #endregion

    }
}
