using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class JourneyControllerTest
    {
        Mock<IRepository> mock;
        Mock<IRepository> mock1;
        List<DateTime> dates;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            mock1 = Mock_Repository.CreateMock();


            dates = new List<DateTime>();
        }

        public JourneyController SetUpABM()
        {
            JourneyController controller;
            mock = Mock_Repository.CreateMock();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://localhost/Home/ABMView"));
            context.Setup(c => c.Request).Returns(request.Object);
            controller = new JourneyController(mock.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            return controller;
        }

        public JourneyController SetUpVU()
        {
            JourneyController controller;
            mock = Mock_Repository.CreateMock();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.Setup(r => r.UrlReferrer).Returns(new Uri("http://localhost/Home/VUView"));
            context.Setup(c => c.Request).Returns(request.Object);
            controller = new JourneyController(mock.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            return controller;
        }

        #region SearchJourneyData
        [Test]
        public void SearchJourneyData_EmptyString_ListOfAllEmployeeNoJourneysWithDayOffFalse()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            string searchString = "";
            List<JourneysByEmployeeViewModel> data = controller.SearchJourneyData(searchString);
            //Assert
            Assert.AreEqual(24, data.Count);
            Assert.AreEqual("Kowood", data.First().LastName);
            Assert.AreEqual(4, data.First().Journeys.Count());
            Assert.AreEqual("Zarose", data.Last().LastName);
            Assert.AreEqual(1, data.Last().Journeys.Count());
        }

        [Test]
        public void SearchJourneyData_Pyorge_AllEmployeesPyorgeNoJourneysWithDayOffFalse()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            string searchString = "Pyorge";
            List<JourneysByEmployeeViewModel> data = controller.SearchJourneyData(searchString);
            //Assert
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("Pyorge", data.First().LastName);
            Assert.AreEqual(1, data.First().Journeys.Count());
        }

        [Test]
        public void SearchJourneyData_TEPY_AllEmployeesTEPYNoJourneysWithDayOffFalse()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            string searchString = "tedk";
            List<JourneysByEmployeeViewModel> data = controller.SearchJourneyData(searchString);
            //Assert
            Assert.AreEqual(1, data.Count);
            Assert.AreEqual("Pyorge", data.First().LastName);
            Assert.AreEqual(1, data.First().Journeys.Count());
        }

        [Test]
        public void SearchJourneyData_Tamila_EmptyList()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            string searchString = "Tamila";
            List<JourneysByEmployeeViewModel> data = controller.SearchJourneyData(searchString);
            //Assert
            Assert.AreEqual(0, data.Count);
        }

        #endregion

        #region GetJourneyData

        [Test]
        public void GetJourneyData_Default_View()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "";
            var view = controller.GetJourneyData(searchString);

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }


        [Test]
        public void GetJourneyData_Default_ListOfAllEmployee()
        {
            //Arrange
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "";
            var view = controller.GetJourneyData(searchString);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyData(searchString).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(24, employeeList.Length);
            Assert.AreEqual(4, employeeList[0].EmployeeID);
            Assert.AreEqual(7, employeeList[1].EmployeeID);
            Assert.AreEqual(15, employeeList[15].EmployeeID);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetJourneyData_Pyorge_ListOfAllEmployeePyorge()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "Pyorge";
            var view = controller.GetJourneyData(searchString);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyData(searchString).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(1, employeeList.Length);
            Assert.AreEqual(3, employeeList[0].EmployeeID);
            Assert.AreEqual("Pyorge", ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetJourneyData_Tamila_ListOfAllEmployeePyorge()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "Tamila";
            var view = controller.GetJourneyData(searchString);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyData(searchString).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(0, employeeList.Length);
            Assert.AreEqual("Tamila", ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetJourneyData_Pyorge_View()
        {
            //Arrange
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "Pyorge";
            var view = controller.GetJourneyData(searchString);

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetJourneyData_PyorgeVU_View()
        {
            //Arrange
            JourneyController controller = SetUpVU();

            //Act
            string searchString = "Pyorge";
            var view = controller.GetJourneyData(searchString);

            //Assert
            Assert.IsTrue(view.ViewName == "GetJourneyDataForVU");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion



        #region GetJourneyDataEMP

        [Test]
        public void GetJourneyDataEMP_Default_View()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string userName = "";
            var view = controller.GetJourneyDataEMP(userName);

            //Assert
            Assert.IsTrue(view.ViewName == "NoData");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.UserName);
        }


        [Test]
        public void GetJourneyDataEMP_Default_ListOfAllEmployee()
        {
            //Arrange
            JourneyController controller = SetUpABM();

            //Act
            string userName = "";
            var view = controller.GetJourneyDataEMP(userName);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyData(userName).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "NoData");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(24, employeeList.Length);
            Assert.AreEqual(4, employeeList[0].EmployeeID);
            Assert.AreEqual(7, employeeList[1].EmployeeID);
            Assert.AreEqual(15, employeeList[15].EmployeeID);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.UserName);
        }

        [Test]
        public void GetJourneyDataEMP_Pyorge_ListOfAllEmployeePyorge()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string userName = "tedk";
            var view = controller.GetJourneyData(userName);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyDataEMP(userName).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(1, employeeList.Length);
            Assert.AreEqual(3, employeeList[0].EmployeeID);
            Assert.AreEqual("tedk", ((ViewResult)view).ViewBag.UserName);
        }

        [Test]
        public void GetJourneyDataEMP_Tamila_ListOfAllEmployeePyorge()
        {
            //Arrange
            SetUpABM();
            JourneyController controller = SetUpABM();

            //Act
            string userName = "Tamila";
            var view = controller.GetJourneyData(userName);
            IEnumerable<JourneysAndOvertimesModel> result = (IEnumerable<JourneysAndOvertimesModel>)controller.GetJourneyData(userName).Model;
            JourneysAndOvertimesModel[] employeeList = result.ToArray();

            //Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(0, employeeList.Length);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.UserName);
        }

        [Test]
        public void GetJourneyDataEmp_Pyorge_View()
        {
            //Arrange
            JourneyController controller = SetUpABM();

            //Act
            string searchString = "tedk";
            var view = controller.GetJourneyDataEMP(searchString);

            //Assert
            Assert.IsTrue(view.ViewName == "GetJourneyDataForEmp");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.UserName);
        }

        [Test]
        public void GetJourneyDataEMP_PyorgeEMP_View()
        {
            //Arrange
            JourneyController controller = SetUpVU();

            //Act
            string searchString = "tedk";
            var view = controller.GetJourneyDataEMP(searchString);

            //Assert
            Assert.IsTrue(view.ViewName == "GetJourneyDataForEmp");
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.UserName);
        }

        #endregion

        #region GetJourney

        [Test]
        public void GetJourney_Default_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            string searchString = "";

            //Act
            var result = controller.GetJourney(searchString) as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void GetJourney_searchString_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            string searchString = "Hello";

            //Act
            var result = controller.GetJourney(searchString) as ViewResult;

            //Assert
            Assert.AreEqual("Hello", result.ViewBag.SearchString);
        }

        [Test]
        public void GetEditJourney_correctID_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            //Act
            var result = controller.EditJourney(1, "");
            //Assert

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(1, ((ViewResult)result).ViewBag.ID);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);

        }
        #endregion

        #region GetEditJourney
        
        [Test]
        public void GetEditJourney_nullEmployee_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.EditJourney(0, "") as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
 
        }

        [Test]
        public void GetEditJourney_NotNullEmployee_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.EditJourney(1, "a") as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("a", result.ViewBag.SearchString);

        }
        #endregion

        #region PostEditJourney
        [Test]
        public void PostEditJourney_InvalidJourney_View()
        {
            //Arrange
            Journey journey = new Journey();
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            controller.ModelState.AddModelError("error", "error");
            var result = controller.EditJourney(1500, new DateTime(2014, 02, 27), "");

            //Assert
            mock.Verify(m => m.SaveJourney(journey), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1, "error");
        }

        [Test]
        public void PostEditJourney_ValidJourney_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.EditJourney(1, new DateTime(2014, 02, 27), "") as ViewResult;
            Journey journey = (from journ in mock.Object.Journeys where journ.JourneyID == 1 select journ).FirstOrDefault();

            //Assert
            mock.Verify(j => j.SaveJourney(journey), Times.Once);
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.AreEqual(new DateTime(2014, 02, 27), journey.ReclaimDate);

        }

        [Test]
        public void PostEditJourney_ValidJourneyAndsearchString_View()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.EditJourney(1, new DateTime(2014, 02, 27), "x") as ViewResult;
            Journey journey = (from journ in mock.Object.Journeys where journ.JourneyID == 1 select journ).FirstOrDefault();

            //Assert
            mock.Verify(j => j.SaveJourney(journey), Times.Once);
            Assert.AreEqual("TableViewJourneyAndOvertimeData", result.ViewName);
            Assert.AreEqual("x", result.ViewBag.SearchString);
            Assert.AreEqual(new DateTime(2014, 02, 27), journey.ReclaimDate);

        }

        [Test]
        public void PostEdit_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            mock.Setup(m => m.SaveJourney(It.IsAny<Journey>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.EditJourney(1, new DateTime(2014, 02, 02), "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveJourney(It.IsAny<Journey>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region CreateDatesToReclaimForDropdown
        [Test]
        public void CreateDatesToReclaimForDropdown_EmptyList_EmptyOption()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            //Act
            string result = controller.CreateDatesToReclaimForDropdown(dates).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CreateDatesToReclaimForDropdown_OneDateInList_OneDateInOption()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            dates.Add(new DateTime(2014, 04, 01));

            //Act
            string result = controller.CreateDatesToReclaimForDropdown(dates).ToString();
            string expected = "<option value=\"01.04.2014\">01.04.2014</option>";

            //Assert   
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CreateDatesToReclaimForDropdown_TwoDatesInList_TwoDatesInOption()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);
            dates.Add(new DateTime(2014, 04, 01));
            dates.Add(new DateTime(2013, 10, 21));

            //Act
            string result = controller.CreateDatesToReclaimForDropdown(dates).ToString();
                       
            //Act
            string expected = "<option value=\"01.04.2014\">01.04.2014</option><option value=\"21.10.2013\">21.10.2013</option>";

            //Assert   
            Assert.AreEqual(expected, result);

        }
        #region

        #endregion GetJourneysAndOvertimes
        [Test]
        public void GetJourneysAndOvertimes_SearchStringEmpty_AllEmployees()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimes("");
            List<JourneysAndOvertimesModel> list = new List<JourneysAndOvertimesModel>();
           
            
            //Assert   
            Assert.AreEqual(24, result.ToList().Count());
            Assert.AreEqual("tadk", result[0].EID);
            Assert.AreEqual("andl", result[23].EID);
            Assert.AreEqual(4, result[0].EmployeeID);
            Assert.AreEqual("Tanya", result[0].FirstName);
            Assert.AreEqual("xtwe", result[1].EID);
            Assert.IsInstanceOf(typeof(List<JourneysAndOvertimesModel>), result.ToList());

        }

        [Test]
        public void GetJourneysAndOvertimesFor_NotEmpty_OneEmployeeDataNoJourneysWithDayOffFalse()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimes("tadk");
            List <JourneysAndOvertimesModel> list = new List<JourneysAndOvertimesModel>();
            
            //Assert   
            Assert.AreEqual(1, result.ToList().Count());
            Assert.AreEqual(4, result[0].EmployeeID);
            Assert.AreEqual("Tanya", result[0].FirstName);
            Assert.AreEqual("Kowood", result[0].LastName);
            Assert.AreEqual(4, result[0].Journeys.Count());
            Assert.IsInstanceOf(typeof(List<JourneysAndOvertimesModel>), result.ToList());

        }

        [Test]
        public void GetJourneysAndOvertimes_JourneysAndOvertimesAllEemployees_()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimes(String.Empty);
            List<JourneysAndOvertimesModel> list = new List<JourneysAndOvertimesModel>();

            //Assert   
            Assert.AreEqual(24, result.ToList().Count());
            Assert.IsInstanceOf(typeof(List<JourneysAndOvertimesModel>), result.ToList());


        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmp_NotExistingEmployee_EmptyList()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimes("111111111");
            List<JourneysAndOvertimesModel> list = new List<JourneysAndOvertimesModel>();

            //Assert   
            Assert.AreEqual(0, result.ToList().Count());
            Assert.IsInstanceOf(typeof(List<JourneysAndOvertimesModel>), result.ToList());


        }
        #endregion
        #region GetJourneysAndOvertimesForOneEmp

        [Test]
        public void GetJourneysAndOvertimesForOneEmp_JourneysAndOvertimesEmptyListsExistingEmployee_EmptyString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmp(24);
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmp_JourneysAndOvertimesNotEmptyListsExistingEmployee_FormattedString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmp(4);
            var expected = new MvcHtmlString("<option value=\"31.08.2013\">31.08.2013</option><option value=\"11.10.2014\">11.10.2014</option><option value=\"30.11.2014\">30.11.2014</option>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmp_JourneysAndOvertimesExistingemployee_EmptyString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmp(20);
            var expected = new MvcHtmlString("");

            //Assert   

            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmp_NotExistingEmployee_EmptyMVcString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmp(10000);
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }
        #endregion

        #region GetJourneysAndOvertimesForOneEmpEdit
        
        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_JourneysAndOvertimesEmptyListsExistingEmployee_EmptyString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(24, "");
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_JourneysAndOvertimesExistingemployee_EmptyString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            string date = "30.11.2014";
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(20, date);
          
            var expected = new MvcHtmlString("");

            //Assert   

            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_NotExistingEmployee_EmptyMVcString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(10000, "");
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }


        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_ExistingEmployee_SortedByDates()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(4, "31.08.2013");
            DateTime reclaimed = new DateTime(2014, 10, 11);
          
            var expected = new MvcHtmlString("<option value=\"31.08.2013\">31.08.2013</option><option value=\"11.10.2014\">11.10.2014</option><option value=\"30.11.2014\">30.11.2014</option>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }


        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_ExistingEmployee_FormattedString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            DateTime reclaimed = new DateTime(2014, 10, 11);
            var result =  controller.GetJourneysAndOvertimesForOneEmpEdit(4, "10.10.2014");
            var expected = new MvcHtmlString("<option value=\"11.10.2014\">11.10.2014</option><option value=\"31.08.2013\">31.08.2013</option><option value=\"11.10.2014\">11.10.2014</option><option value=\"30.11.2014\">30.11.2014</option>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_OvertimesOfExistingEmployee_SortedByDates()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(1, "02.02.2013");


            var expected = new MvcHtmlString("<option value=\"01.01.2013\">01.01.2013</option><option value=\"17.06.2013\">17.06.2013</option>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }


        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_OvertimesofExistingEmployee_FormattedString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            DateTime reclaimed = new DateTime(2014, 10, 11);
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(4, "02.04.2013");
            var expected = new MvcHtmlString("<option value=\"04.01.2013\">04.01.2013</option><option value=\"31.08.2013\">31.08.2013</option><option value=\"11.10.2014\">11.10.2014</option><option value=\"30.11.2014\">30.11.2014</option>");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_OvertimesofExistingEmployeeDateNull_FormattedString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            DateTime reclaimed = new DateTime(2014, 10, 11);
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(1, null);
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);
        }

        [Test]
        public void GetJourneysAndOvertimesForOneEmpEdit_JourneysOfExistingEmployeeDateNull_FormattedString()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            DateTime reclaimed = new DateTime(2014, 10, 11);
            var result = controller.GetJourneysAndOvertimesForOneEmpEdit(4, null);
            var expected = new MvcHtmlString("");

            //Assert   
            Assert.AreEqual(expected.ToString(), result.ToString());
            Assert.IsInstanceOf(typeof(MvcHtmlString), result);

        }

        #endregion

        #region SearchJourneyDataForOneEmp

        [Test]
        public void SearchJourneyDataForOneEmp_JourneysExistingEmployee_ListDateTime()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result1 = controller.SearchJourneyDataForOneEmp(4);
            var expected1 = new List<DateTime>();
            expected1.Add(new DateTime(2013, 08, 31));
            expected1.Add(new DateTime(2014, 11, 30));
            expected1.Add(new DateTime(2014, 10, 11));

            //Assert   
            CollectionAssert.AreEqual(expected1, result1.ToList());
            Assert.IsInstanceOf(typeof(List<DateTime>), result1);
        }


        [Test]
        public void SearchJourneyDataForOneEmp_NotExistingEmployee_EmptyList()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result1 = controller.SearchJourneyDataForOneEmp(10000);
            var expected1 = new List<DateTime>();
      

            //Assert   
            CollectionAssert.AreEqual(expected1, result1.ToList());
            Assert.IsInstanceOf(typeof(List<DateTime>), result1);
        }


        [Test]
        public void GetJourneysAndOvertimesForOneEmp_NoJourneysAndOvertimesExistingemployee_()
        {
            //Arrange
            JourneyController controller = new JourneyController(mock.Object);

            //Act
            var result1 = controller.SearchJourneyDataForOneEmp(1);
            var expected1 = new List<DateTime>();

            //Assert   
            Assert.AreEqual(expected1, result1);
            Assert.IsInstanceOf(typeof(List<DateTime>), result1);
        }
        #endregion
    }
}
