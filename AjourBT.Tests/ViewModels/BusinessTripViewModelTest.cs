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
    class BusinessTripViewModelTest
    {
        Mock<IRepository> mock;
        BusinessTripViewModel businessTripViewModel;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            businessTripViewModel = new BusinessTripViewModel(new BusinessTrip());
        }

        [Test]
        public void prepareToXLSExportADM_NullableAreNull_Stringified()
        {
            //Arrange 

            //Act
            List<string> result = businessTripViewModel.PrepareToXLSExportVU();

            //Assert 
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("0", result[0]);
            Assert.AreEqual("", result[1]);
            Assert.AreEqual("", result[2]);
            Assert.AreEqual("", result[3]);
            Assert.AreEqual("0001-01-01 To be updated soon", result[4]);
            Assert.AreEqual("0001-01-01", result[5]);
            Assert.AreEqual("", result[6]);
            Assert.AreEqual("", result[7]);
            Assert.AreEqual("", result[8]);
            Assert.AreEqual("", result[9]);
        }

        [Test]
        public void prepareToXLSExportADM_NullableAreNotNull_Stringified()
        {
            //Arrange 
            businessTripViewModel = new BusinessTripViewModel(mock.Object.BusinessTrips[0]);

            businessTripViewModel.Status = BTStatus.Confirmed | BTStatus.Reported; 

            //Act
            List<string> result = businessTripViewModel.PrepareToXLSExportVU();

            //Assert 
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("andl", result[1]);
            Assert.AreEqual("Zarose Anastasia", result[2]);
            Assert.AreEqual("LDF", result[3]);
            Assert.AreEqual(string.Format("{0:yyyy-MM-dd}", new DateTime(2014, 12, 01)), result[4]);
            Assert.AreEqual(string.Format("{0:yyyy-MM-dd}", new DateTime(2014, 12, 10)), result[5]);
            Assert.AreEqual("Unknown", result[6]);
            Assert.AreEqual("", result[7]);
            Assert.AreEqual("", result[8]);
            Assert.AreEqual("", result[9]);
        }
    }
}
