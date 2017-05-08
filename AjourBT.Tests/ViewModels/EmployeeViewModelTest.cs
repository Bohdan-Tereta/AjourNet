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
    public class EmployeeViewModelTest
    {
        Mock<IRepository> mock;
        EmployeeViewModel employeeViewModel;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            employeeViewModel = new EmployeeViewModel(new Employee { EID = "andl" });
        }

        [Test]
        public void prepareToXLSExportADM_NullableAreNull_Stringified()
        {
            //Arrange 

            //Act
            List<string> result = employeeViewModel.PrepareToXLSExportADM();

            //Assert 
            Assert.AreEqual(11, result.Count);
            for (int i = 1; i < 11; i++)
            {
                Assert.AreEqual("", result[i].Trim());
            }
        }

        [Test]
        public void prepareToXLSExportADM_NullableAreNotNull_Stringified()
        {
            //Arrange 
            employeeViewModel = new EmployeeViewModel(mock.Object.Employees[0]);

            //Act
            List<string> result = employeeViewModel.PrepareToXLSExportADM();

            //Assert 
            Assert.IsNotNull(employeeViewModel.EducationAcquiredDate);
            Assert.IsNotNull(employeeViewModel.EducationInProgressDate);
            Assert.AreEqual(EducationType.BasicSecondary, employeeViewModel.EducationAcquiredType);
            Assert.AreEqual(EducationType.CompleteHigher, employeeViewModel.EducationInProgressType);
            Assert.AreEqual(11, result.Count);
            Assert.AreEqual("andl", result[0]);
            Assert.AreEqual("Zarose Anastasia", result[1]);
            Assert.AreEqual("", result[2]);
            Assert.AreEqual("SDDDA", result[3]);
            Assert.AreEqual("Software developer", result[4]);
            Assert.AreEqual(string.Format("{0:d}", new DateTime(2011, 11, 01)), result[5]);
            Assert.AreEqual("Джонні Роус Олександрович", result[6]);
            Assert.AreEqual("", result[7]);
            Assert.AreEqual("", result[8]);
            Assert.AreEqual(string.Format("{0:d}", new DateTime(2013, 11, 01)), result[9]);
        }

        [Test]
        public void prepareToXLSExportVU_NullableAreNull_Stringified()
        {
            //Arrange 

            //Act
            List<string> result = employeeViewModel.PrepareToXLSExportVU();

            //Assert 
            Assert.AreEqual(8, result.Count);
            for (int i = 1; i < 8; i++)
            {
                Assert.AreEqual("", result[i].Trim());
            }
        }

        [Test]
        public void prepareToXLSExportVU_NullableAreNotNull_Stringified()
        {
            //Arrange 
            employeeViewModel = new EmployeeViewModel(mock.Object.Employees[0]);

            //Act
            List<string> result = employeeViewModel.PrepareToXLSExportVU();

            //Assert 
            Assert.AreEqual(8, result.Count);
            Assert.AreEqual("andl", result[0]);
            Assert.AreEqual("Zarose Anastasia", result[1]);
            Assert.AreEqual("Джонні Роус Олександрович", result[2]);
            Assert.AreEqual("SDDDA", result[3]);
            Assert.AreEqual("Software developer", result[4]);
            Assert.AreEqual("Розробник програмного забезпечення", result[5]);
            Assert.AreEqual(string.Format("{0:d}", new DateTime(2011, 11, 01)), result[6]);
            Assert.AreEqual(string.Format("{0:d}", new DateTime(2013, 11, 01)), result[7]); ;
        }
        #region Education
        [Test]
        public void EducationSummary_AllValuesDefault_EmptyString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
                {
                    EID = "andl",
                    EducationAcquiredDate = null,
                    EducationAcquiredType = 0,
                    EducationInProgressDate = null,
                    EducationInProgressType = 0
                }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("", result);

        }

        [Test]
        public void EducationSummary_EducationAcquiredType_ProperString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
            {
                EID = "andl",
                EducationAcquiredDate = null,
                EducationAcquiredType = EducationType.BasicSecondary,
                EducationInProgressDate = null,
                EducationInProgressType = 0
            }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("базова загальна середня", result);

        }

        [Test]
        public void EducationSummary_EducationAcquiredTypeAndDate_ProperString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
            {
                EID = "andl",
                EducationAcquiredDate = DateTime.Now,
                EducationAcquiredType = EducationType.BasicSecondary,
                EducationInProgressDate = null,
                EducationInProgressType = 0
            }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("базова загальна середня, " + string.Format("{0:d}", DateTime.Now), result);

        }

        [Test]
        public void EducationSummary_EducationInProgressType_ProperString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
            {
                EID = "andl",
                EducationAcquiredDate = null,
                EducationAcquiredType = 0,
                EducationInProgressDate = null,
                EducationInProgressType = EducationType.BasicSecondary
            }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("базова загальна середня", result);

        }

        [Test]
        public void EducationSummary_EducationInProgressTypeAndDate_ProperString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
            {
                EID = "andl",
                EducationAcquiredDate = null,
                EducationAcquiredType = 0,
                EducationInProgressDate = DateTime.Now.AddDays(1),
                EducationInProgressType = EducationType.BasicSecondary
            }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("базова загальна середня, " + string.Format("{0:d}", DateTime.Now.AddDays(1)), result);

        }

        [Test]
        public void EducationSummary_AllParametersAreSet_ProperString()
        {
            //Arrange
            EmployeeViewModel empViewModel = new EmployeeViewModel
            (
            new Employee
            {
                EID = "andl",
                EducationAcquiredDate = DateTime.Now.Date,
                EducationAcquiredType = EducationType.BasicHigher,
                EducationInProgressDate = DateTime.Now.Date.AddDays(1),
                EducationInProgressType = EducationType.BasicSecondary,  
                EducationComment = "A comment"
            }
            );
            //Act
            String result = empViewModel.EducationSummary;

            //Assert      
            Assert.AreEqual("базова вища, " + string.Format("{0:d}", DateTime.Now.Date) + "; "

                + "базова загальна середня, " + string.Format("{0:d}", DateTime.Now.Date.AddDays(1)) + " ", result);

        }
        #endregion
    }
}
