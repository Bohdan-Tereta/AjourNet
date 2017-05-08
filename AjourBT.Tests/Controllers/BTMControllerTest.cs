using System;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Domain.ViewModels;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using System.Web;
using System.Web.Routing;
using System.Security.Principal;
using System.IO;
using Newtonsoft.Json;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class BTMControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        BTMController controller;
        Mock<ControllerContext> controllerContext;

        string modelError = "The record you attempted to edit "
                              + "was modified by another user after you got the original value. The "
                              + "edit operation was canceled.";

        byte[] rowVersion = { 0, 0, 0, 0, 0, 0, 8, 40 };

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();

            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdateVisaRegistrationDateToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMCreateVisaRegistrationDateToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMCancelsPermitToADM))).Verifiable();

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
            }

            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);
            controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);

            controller = new BTMController(mock.Object, messengerMock.Object);
            controller.ControllerContext = controllerContext.Object;
        }

        #region "Visas and permits tab"
        
        #region GetVisaBTM

        [Test]
        public void GetVisaBTM_AllEmployees()
        {
            //Arrange
            
            //Act
            var resultView = controller.GetVisaBTM("");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaBTM_EmployeesContain_Te()
        {
            //Arrange
           
            //Act
            var resultView = controller.GetVisaBTM("Te");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("Te", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaBTM_EmployeesContain_qq()
        {
            //Arrange
            

            //Act
            var resultView = controller.GetVisaBTM("qq");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("qq", ((ViewResult)resultView).ViewBag.SearchString);
        }

        #endregion

        #region GetVisaDataBTM
        [Test]
        public void GetVisaDataBTM_Default_ProperView()
        {
            // Arrange - create the controller 
            string searchString = String.Empty; 
            
            // Act - call the action method 
            var view = controller.GetVisaDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model; 

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            mock.Verify(m => m.SearchVisaDataExcludingDismissed(searchString), Times.Once());
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result);
            Assert.AreEqual(MvcApplication.JSDatePattern, view.ViewBag.JsDatePattern);
            Assert.AreEqual(searchString, view.ViewBag.SearchString); 
        }

        [Test]
        public void GetVisaDataBTM_NotEmptySearchString_ProperViewTrimmedSearchString()
        {
            // Arrange - create the controller 
            string searchString = " D07 ";

            // Act - call the action method 
            var view = controller.GetVisaDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            mock.Verify(m => m.SearchVisaDataExcludingDismissed(searchString.Trim()), Times.Once());
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result);
            Assert.AreEqual(MvcApplication.JSDatePattern, view.ViewBag.JSDatePattern);
            Assert.AreEqual(searchString.Trim(), view.ViewBag.SearchString);
        }



        //[Test]
        //public void GetVisaDataBTM_FilterTe_EmployeesContain_Te()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    var view = controller.GetVisaDataBTM("Te");
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed("Te"), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}
        //[Test]
        //public void GetVisaDataBTM_FilterTep_EmployeesContain_Tep()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    var view = controller.GetVisaDataBTM("Tep");
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed("Tep"), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}

        //[Test]
        //public void GetVisaDataBTM_Filteraa_EmployeesContain_aa()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    var view = controller.GetVisaDataBTM("aa");
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed("aa"), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}

        //[Test]
        //public void GetVisaDataBTM_FilterD07_EmployeesWithVisaTypeContainD07()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    var view = controller.GetVisaDataBTM(" D07  ");
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed("Tep"), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter1082012_EmployeesWithVisaStartDateContain01082012()
        //{
        //    // Arrange - create the controller     
        //    mock.Object.Employees.FirstOrDefault().DateDismissed = null;
            
        //    // Act - call the action method
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("  " + new DateTime(2012, 08, 01).ToString()).Model;
        //    var view = controller.GetVisaDataBTM(" " + new DateTime(2012, 08, 01).ToString());
        //    Employee[] employeeVisaView = result.ToArray();
        //    mock.Object.Employees.FirstOrDefault().DateDismissed = new DateTime(2013, 11, 01);

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.IsTrue(employeeVisaView.Length == 1);
        //    Assert.AreEqual(employeeVisaView[0].LastName, "Zarose");
        //    Assert.AreEqual(employeeVisaView[0].FirstName, "Anastasia");
        //    Assert.AreEqual(employeeVisaView[0].EID, "andl");

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter1082012EmployeeDismissed_NoEmployees()
        //{
        //    // Arrange - create the controller    
            
        //    // Act - call the action method
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("  " + new DateTime(2012, 08, 01).ToString()).Model;
        //    var view = controller.GetVisaDataBTM(" " + new DateTime(2012, 08, 01).ToString());
        //    Employee[] employeeVisaView = result.ToArray();

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.IsTrue(employeeVisaView.Length == 0);

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter13505_EmployeesWithVisaDueDateContain13505()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    string dateWithoutYear = GetDateWithoutYear(05, 13);

        //    IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("  " + dateWithoutYear + " ").Model;
        //    var view = controller.GetVisaDataBTM("  " + dateWithoutYear + "  ");
        //    Employee[] employeeVisaView = result.ToArray();

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);

        //    Assert.AreEqual(employeeVisaView.Length, 1);
        //    Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
        //    Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
        //    Assert.AreEqual(employeeVisaView[0].EID, "xomi");


        //}

        //[Test]
        //public void GetVisaDataBTM_FilterMULT_NotDismissed_EmployeesWithVisaMULT()
        //{
        //    // Arrange - create the controller     
            
        //    // Act - call the action method
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM(" MULT").Model;
        //    var view = controller.GetVisaDataBTM(" MULT");
        //    Employee[] employeeVisaView = result.ToArray();

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(employeeVisaView.Length, 8);
        //    Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
        //    Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
        //    Assert.AreEqual(employeeVisaView[0].EID, "xomi");

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter0401_EmployeesWithVisaRegDate0401()
        //{
        //    // Arrange - create the controller     
        //    string searchString = " " + new DateTime(2013, 01, 04).ToString(); 
            
        //    // Act - call the action method
        //    var view = controller.GetVisaDataBTM(searchString);
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed(searchString.Trim()), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter01082012_EmployeesWithPermitStartDate01082012()
        //{
        //    // Arrange - create the controller     
        //    mock.Object.Employees.FirstOrDefault().DateDismissed = null;

        //    // Act - call the action method
        //    string dateWithoutYear = GetDateWithoutYear(08, 01);
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM(new DateTime(2012, 08, 01).ToString() + " ").Model;
        //    var view = controller.GetVisaDataBTM(dateWithoutYear + " ");
        //    Employee[] employeeVisaView = result.ToArray();

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(1, employeeVisaView.Length);
        //    Assert.AreEqual("Zarose", employeeVisaView[0].LastName);
        //    Assert.AreEqual("Anastasia", employeeVisaView[0].FirstName);
        //    Assert.AreEqual("andl", employeeVisaView[0].EID);

        //}

        //[Test]
        //public void GetVisaDataBTM_Filter08082014_EmployeesWithPermitEndDate08082014()
        //{
        //    // Arrange - create the controller   
        //    string searchString = new DateTime(2014, 08, 08).ToString() + " ";
            
        //    // Act - call the action method            
        //    var view = controller.GetVisaDataBTM(searchString);
        //    IEnumerable<Employee> result = (IEnumerable<Employee>)view.Model;

        //    // Assert - check the result
        //    Assert.IsInstanceOf(typeof(PartialViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    mock.Verify(m => m.SearchVisaDataExcludingDismissed(searchString.Trim()), Times.Once());
        //    Assert.IsInstanceOf(typeof(IEnumerable<Employee>), result); 

        //}

        #endregion
        
        //TODO: duplicated in BTM and Visa controller tests
        //#region SearchVisaData

        private string GetDateWithoutYear(int month, int day)
        {
            int DateWithSeparatorLength = 5;
            string tempDate = new DateTime(2014, month, day).ToShortDateString();
            string dateWithoutYear = "";
            bool endWith = tempDate.EndsWith("2014", StringComparison.CurrentCulture);
            bool startWith = tempDate.StartsWith("2014", StringComparison.CurrentCulture);

            if (endWith)
            {
                dateWithoutYear = tempDate.Substring(0, tempDate.Length - DateWithSeparatorLength);
            }
            if (startWith)
            {
                dateWithoutYear = tempDate.Substring(DateWithSeparatorLength, tempDate.Length);
            }

            return dateWithoutYear;
        }

        //[Test]
        //public void SearchVisaData_ListNotEmptySearchStringEmpty_AllEmployees()
        //{
        //    //Arrange
        //    List<Employee> empList = mock.Object.Employees.ToList();
        //    List<Employee> resultList = new List<Employee>();
        //    string searchString = "";
        //    //Act
        //    resultList = controller.SearchVisaData(empList, searchString);
        //    //Assert
        //    Assert.AreEqual(empList.Count, resultList.Count);
        //    Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
        //    Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
        //    Assert.AreEqual("Tanya", resultList.ToArray()[3].FirstName);
        //    Assert.AreEqual("Kowood", resultList.ToArray()[3].LastName);
        //    Assert.AreEqual("UPaidVac", resultList.ToArray()[7].FirstName);
        //    Assert.AreEqual("Only", resultList.ToArray()[7].LastName);
        //}

        //[Test]
        //public void SearchVisaData_ListEmptySearchStringEmpty_AllEmployees()
        //{
        //    //Arrange
        //    List<Employee> empList = new List<Employee>();
        //    List<Employee> resultList = new List<Employee>();
        //    string searchString = "";
        //    //Act
        //    resultList = controller.SearchVisaData(empList, searchString);
        //    //Assert
        //    Assert.AreEqual(0, resultList.Count);

        //}
        //[Test]
        //public void SearchVisaData_ListNotEmptySearchStringAnd_AllEmployees()
        //{
        //    //Arrange
        //    List<Employee> empList = mock.Object.Employees.ToList();
        //    List<Employee> resultList = new List<Employee>();
        //    string searchString = "And";
        //    //Act
        //    resultList = controller.SearchVisaData(empList, searchString);
        //    //Assert
        //    Assert.AreEqual(1, resultList.Count);
        //    Assert.AreEqual("Anastasia", resultList.ToArray()[0].FirstName);
        //    Assert.AreEqual("Zarose", resultList.ToArray()[0].LastName);

        //}

        //#endregion


        #region ModifyPassport
        [Test]
        public void ModifyPassport_ValidIDisCheckedNullEmployeeHasPassport_TrueDeletePassport()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            ActionResult result = controller.ModifyPassport("1", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_ValidIDisCheckedNullEmployeeHasNoPassport_True()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            ActionResult result = controller.ModifyPassport("2", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_SearckStringAValidIDisCheckedNotNullEmployeeHasPassport_TrueSavePassport()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            ActionResult result = controller.ModifyPassport("2", "checked", "A");

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Once);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_SearchStringEmptyValidIDisCheckedNotNullEmployeeHasNoPassport_True()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            //Act
            var result = controller.ModifyPassport("1", "checked");

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Once);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

        [Test]
        public void ModifyPassport_NotValidIDNotParsable_False()
        {
            //Arrange


            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("abc", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDZero_False()
        {
            //Arrange
           
            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("0", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDNull_False()
        {
            //Arrange


            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport(null, null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDNotExistingEmployee_False()
        {
            //Arrange

            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("10000", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new InvalidOperationException());

            //Act
            var result = controller.ModifyPassport("2", "checked");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region PassportAddDateGet

        [Test]
        public void PassportAddDateGet_DaSearchString_ExistingEmployee()
        {
            // Arrange                          
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PassportAddDate(5, "da") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("da", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(PassportViewModel), passport);
        }

        [Test]
        public void PassportAddDateGet_ABBBSSSearchString_ExistingEmployee()
        {
            // Arrange                          

            // Act - call the action method 
            var result = controller.PassportAddDate(5, "ABBBSS") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("ABBBSS", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), passport);
        }

        [Test]
        public void PassportAddDateGet_EmptySearchString_ExistingEmployee()
        {
            // Arrange                          

            // Act - call the action method 
            var result = controller.PassportAddDate(5) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), passport);
        }

        [Test]
        public void PassportAddDateGet_PassportOf_NotExistingEmployee()
        {
            // Arrange                

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PassportAddDate(1500);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(passport);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);

        }

        #endregion

        #region PassportAddDatePost

        [Test]
        public void PassportAddDatePost_CanCreateAndEmptySearchString_ValidPassport()
        {
            // Arrange                
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PassportAddDate(passport);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);

        }

        [Test]
        public void PassportAddDatePost_CanCreateAndADDDASearchString_ValidPassport()
        {
            // Arrange                
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };

            // Act - call the action method 
            var result = controller.PassportAddDate(passport, "ADDDA");

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Once);
            Assert.AreEqual("ADDDA", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void PassportAddDatePost_CannotCreateAndEmptySearchString_InvalidPassport()
        {
            // Arrange
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.PassportAddDate(passport, "");
            // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void PassportAddDatePost_CannotCreateAndDaSearchString_InvalidPassport()
        {
            // Arrange
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.PassportAddDate(passport, "Da");

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Da", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void PassportAddDatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };

            //Act
            JsonResult result = (JsonResult)controller.PassportAddDate(passport);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region PassportEditDateGET

        [Test]
        public void PassportEditDateGet_CannotEdit_InvalidEmployeeIDEmptySearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PassportEditDate(15, "");
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void PassportEditDateGet_CannotEdit_InvalidEmployeeIDValidSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PassportEditDate(1500, "a");
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void PassportEditDateGet_CannotEdit_InvalidEmployeeIDNullSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PassportEditDate(1500, null);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void PassportEditDateGet_CanEdit_ValidEmployeeID()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.PassportEditDate(1) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PassportEditDateGet_CanEdit_ValidEmployeeIDAndEmptySearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.PassportEditDate(1, "") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PassportEditDateGet_CanEdit_ValidEmployeeIDAndSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.PassportEditDate(1, "A") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PassportEditDateGet_CanEdit_ValidEmployeeIDNullSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.PassportEditDate(1, null) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PassportEditDateGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PassportEditDate(1500);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);
        }
        #endregion

        #region PassportEditDatePost

        [Test]
        public void PassportEditDatePost_CanEdit_ValidPassport()
        {
            // Arrange - create the controller 
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PassportEditDate(passport);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePassport(passport), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }


        [Test]
        public void PassportEditDatePost_CannotEdit_InvalidPassport()
        {
            // Arrange - create the controller 
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.PassportEditDate(passport);

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void PassportEditDatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.PassportEditDate(mock.Object.Passports.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }


        #endregion
        
        #region GetEmployeeData
        [Test]
        public void GetEmployeeData_ListNotEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            List<Employee> empList = mock.Object.Employees.ToList();
            List<Employee> resultList = new List<Employee>();
            string searchString = "";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(empList.Count, resultList.Count);
            Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
            Assert.AreEqual("PaidVac", resultList.ToArray()[6].FirstName);
            Assert.AreEqual("Only", resultList.ToArray()[6].LastName);
        }
        [Test]
        public void GetEmployeeData_ListEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            List<Employee> empList = new List<Employee>();
            List<Employee> resultList = new List<Employee>();
            string searchString = "";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(0, resultList.Count);

        }
        [Test]
        public void GetEmployeeData_ListNotEmptySearchStringAnd_AllEmployees()
        {
            //Arrange
            List<Employee> empList = mock.Object.Employees.ToList();
            List<Employee> resultList = new List<Employee>();
            string searchString = "And";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Anastasia", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", resultList.ToArray()[0].LastName);

        }


        #endregion

        #region VisaCreateGet
        [Test]
        public void VisaCreateGet_VisaOf_ExistingEmployeeAndDefault()
        {
            // Arrange - create the controller                 
            
            // Act - call the action method 
            var result = controller.VisaCreate(3) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void VisaCreateGet_VisaOf_ExistingEmployeeAndNull()
        {
            // Arrange - create the controller                 
            
            string searchString = null;

            // Act - call the action method 
            var result = controller.VisaCreate(3, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaCreateGet_VisaOf_ExistingEmployeeAnd_dan_()
        {
            // Arrange - create the controller                 
            
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.VisaCreate(3, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaCreateGet_VisaOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 
            
            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaCreate(1500);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(visa);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }
        #endregion

        #region VisaCreatePost
        [Test]
        public void VisaCreatePost_CanCreate_ValidVisaSearchStringEmpty()
        {
            // Arrange - create the controller                 
            
            Visa visa = new Visa { EmployeeID = 3, VisaType = "D08", StartDate = new DateTime(2012, 01, 08), DueDate = new DateTime(2013, 01, 02), Days = 90, Entries = 0, CorrectionForVisaDays = null, CorrectionForVisaEntries = null };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaCreate(visa);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 3), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void VisaCreatePost_CanCreate_ValidVisaSearchStringNotEmpty()
        {
            // Arrange - create the controller                 
            
            Visa visa = new Visa { EmployeeID = 4, VisaType = "C08", StartDate = new DateTime(2012, 01, 08), DueDate = new DateTime(2013, 01, 02), Days = 90, Entries = 20, CorrectionForVisaDays = null, CorrectionForVisaEntries = null };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaCreate(visa, "b");
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 4), Times.Once);
            Assert.AreEqual("b", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void VisaCreatePost_CannotCreate_InvalidVisa()
        {
            // Arrange - create the controller
            
            Visa visa = new Visa { EmployeeID = 2, VisaOf = mock.Object.Employees.Where(e => e.EmployeeID == 2).FirstOrDefault() };

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            ViewResult result = controller.VisaCreate(visa) as ViewResult;


            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(VisaViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void VisaCreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Visa visa = mock.Object.Visas.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisa(visa, visa.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.VisaCreate(visa);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisa(visa, visa.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }


        #endregion

        #region VisaEditGet

        [Test]
        public void VisaEditGet_ValidEmployeeIDAndDefault_CanEdit()
        {
            // Arrange - create the controller 
            
            // Act - call the action method 
            var result = controller.VisaEdit(2) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void VisaEditGet_ValidEmployeeIDAndNull_CanEdit()
        {
            // Arrange - create the controller 
            string searchString = null;

            // Act - call the action method 
            var result = controller.VisaEdit(2, searchString) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaEditGet_ValidEmployeeIDAnd_dan__CanEdit()
        {
            // Arrange - create the controller 
            
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.VisaEdit(2, searchString) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaEditGet_InvalidEmployeeIDAndDefault_CannotEdit()
        {
            // Arrange - create the controller 
            
            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaEdit(15);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }

        [Test]
        public void VisaEditGet_InvalidEmployeeIDAndNull_CannotEdit()
        {
            // Arrange - create the controller 
            string searchString = null;

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaEdit(15, searchString);
            var viewResult = controller.VisaEdit(15, searchString) as ViewResult;
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }


        #endregion

        #region VisaEditPost

        [Test]
        public void VisaEditPost_ValidVisaAndDefault_CanEdit()
        {
            // Arrange - create the controller 
            
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.CorrectionForVisaDays = 10;
            visa.CorrectionForVisaEntries = 1;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaEdit(visa);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Once);
            Assert.AreEqual(visaAfter.CorrectionForVisaEntries, 1);
            Assert.AreEqual(visaAfter.CorrectionForVisaDays, 10);
        }

        [Test]
        public void VisaEditPost_ValidVisaAnd_dan_CanEdit()
        {
            // Arrange - create the controller 
            
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.CorrectionForVisaDays = 10;
            visa.CorrectionForVisaEntries = 1;
            string searchString = "dan";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act - call the action method 
            var result = controller.VisaEdit(visa, searchString);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Once);
            Assert.AreEqual(visaAfter.CorrectionForVisaEntries, 1);
            Assert.AreEqual(visaAfter.CorrectionForVisaDays, 10);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void VisaEditPost_CannotEdit_InvalidVisa()
        {
            // Arrange - create the controller 
            
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.Days = 0;
            visa.Entries = 0;

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.VisaEdit(visa);


            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(VisaViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void VisaEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Visa visa = mock.Object.Visas.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisa(visa, visa.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.VisaEdit(visa);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisa(visa, visa.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region VisaDeleteGet
        [Test]
        public void VisaDeleteGet_ValidEmployeeIDAndDefault()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = controller.VisaDelete(2) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void VisaDeleteGet_ValidEmployeeIDAndEmptyString()
        {
            // Arrange - create the controller 
            string searchString = "";

            // Act - call the action method 
            var result = controller.VisaDelete(2, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaDeleteGet_ValidEmployeeIDAnd_dan_()
        {
            // Arrange - create the controller 
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.VisaDelete(2, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void VisaDeleteGet_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaDelete(15);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }

        #endregion

        #region VisaDeleteConfirmed
        [Test]
        public void VisaDeleteConfirmed_ValidVisa()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaDeleteConfirmed(1, "as");

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(1), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }
        [Test]
        public void VisaDeleteConfirmed_ValidVisaSearchStringEmpty()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaDeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(2), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }
        #endregion

        #region VisaRegistrationDate
        [Test]
        public void CreateGet_VisaRegistrationDateOf_ExistingEmployee()
        {
            // Arrange - create the controller                 
            
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            var result = controller.VisaRegCreate(5, "as") as ViewResult;
            VisaRegistrationDate visaRegDate = (VisaRegistrationDate)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("as", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void CreateGet_VisaRegistrationDateOf_ExistingEmployeeSearchStringEmpty()
        {
            // Arrange - create the controller                 

            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            var result = controller.VisaRegCreate(5) as ViewResult;
            VisaRegistrationDate visaRegDate = (VisaRegistrationDate)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void CreateGet_VisaRegistrationDateOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaRegCreate(1500);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(visaRegDate);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void CreatePost_CanCreate_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller                 

            VisaRegistrationDate visaRegDate = new VisaRegistrationDate { EmployeeID = 5, RegistrationDate = new DateTime(2013, 04, 01), VisaType = "D10", City = "Kyiv", RegistrationTime = "09:00", RegistrationNumber = "0001" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            controller.ControllerContext = controllerContext.Object;

            // Act - call the action method 
            var result = controller.VisaRegCreate(visaRegDate, "as");

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

        [Test]
        public void CreatePost_CannotCreate_InvalidVisaRegistrationDate()
        {
            // Arrange - create the controller
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            ViewResult result = controller.VisaRegCreate(visaRegDate) as ViewResult;


            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(It.IsAny<VisaRegistrationDate>(), It.IsAny<int>()), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            Assert.IsInstanceOf(typeof(RegistrationDateViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.VisaRegCreate(visaRegDate);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void EditGet_CanEdit_ValidEmployeeID()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaRegEdit(5, "st") as ViewResult;
            RegistrationDateViewModel visaRegDate = (RegistrationDateViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(5, visaRegDate.EmployeeID);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("st", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);

        }

        [Test]
        public void EditGet_CanEdit_ValidEmployeeIDSearchStruingEmpty()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaRegEdit(5) as ViewResult;
            RegistrationDateViewModel visaRegDate = (RegistrationDateViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(5, visaRegDate.EmployeeID);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);

        }

        [Test]
        public void EditGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaRegEdit(15);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void EditPost_CanEdit_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate { EmployeeID = 5, RegistrationDate = new DateTime(2013, 04, 01), VisaType = "D10" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            controller.ControllerContext = controllerContext.Object;

            // Act - call the action method 
            var result = controller.VisaRegEdit(visaRegDate);

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);

        }

        [Test]
        public void EditPost_CannotEdit_InvalidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.VisaRegEdit(visaRegDate);

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(RegistrationDateViewModel), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.VisaRegEdit(visaRegDate);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCreateVisaRegistrationDateToBTM) && msg.ReplyTo == "User@elegant.com")), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeletePost_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.VisaRegDeleteConfirmed(1, "as");

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisaRegistrationDate(1), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);

        }

        [Test]
        public void DeletePost_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.VisaRegDeleteConfirmed(15);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisaRegistrationDate(15), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visaRegDate);
        }
        [Test]
        public void GetEmployeeData_StringEmpty_AllEmployees()
        {
            //Arrange

            // Act - call the action method 
            var result = controller.GetEmployeeData(mock.Object.Employees.ToList(), "");
            // Assert - check the result 
            Assert.AreEqual(24, result.ToArray().Length);
            Assert.AreEqual("Kowwood", result.ToArray()[0].LastName);
            Assert.AreEqual("Only", result.ToArray()[6].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(result.ToList(), typeof(Employee));

        }

        [Test]
        public void GetEmployeeData_String0801_SelectedEmployees()
        {
            //Arrange

            // Act - call the action method 
            var result = controller.GetEmployeeData(mock.Object.Employees.ToList(), new DateTime(2012, 08, 01).ToString());
            // Assert - check the result 
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Zarose", result.ToArray()[0].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(result.ToList(), typeof(Employee));

        }
        [Test]
        public void GetEmployeeData_StringABRAKADABRA_NothingFound()
        {
            //Arrange

            // Act - call the action method 
            var result = controller.GetEmployeeData(mock.Object.Employees.ToList(), "ABRAKADABRA");
            // Assert - check the result 
            Assert.AreEqual(0, result.ToArray().Length);

        }

        #endregion

        #region Permit
        [Test]
        public void CreateGet_PermitOf_SearchStringEmpty_ExistingEmployee()
        {
            // Arrange - create the controller                 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitCreate(5) as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(permit);
        }

        [Test]
        public void CreateGet_PermitOf_SearchStringA_ExistingEmployee()
        {
            // Arrange - create the controller                 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitCreate(5, "A") as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(permit);
        }
        [Test]
        public void CreateGet_PermitOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PermitCreate(1500);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(permit);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringEmpty_ValidPermit()
        {
            // Arrange - create the controller                 
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitCreate(permit);

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringA_ValidPermit()
        {
            // Arrange - create the controller                 
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitCreate(permit, "A");

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidPermit()
        {
            // Arrange - create the controller
            Permit permit = new Permit();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            ViewResult result = controller.PermitCreate(permit) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(It.IsAny<Permit>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(PermitViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void PermitCreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SavePermit(permit, permit.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.PermitCreate(permit);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePermit(permit, permit.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void PermitEditGet_CanEditSearchStringEmpty_ValidEmployeeID()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitEdit(2) as ViewResult;
            PermitViewModel permit = (PermitViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PermitEditGet_CanEditSearchStringA_ValidEmployeeID()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitEdit(2, "A") as ViewResult;
            PermitViewModel permit = (PermitViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);

            Assert.AreEqual(2, permit.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void PermitEditGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PermitEdit(15);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(permit);
        }

        [Test]
        public void PermitEditPost_CanEditSearchStringEmpty_ValidPermit()
        {
            // Arrange - create the controller 
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitEdit(permit);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
        }
        [Test]
        public void PermitEditPost_CanEditSearchStringA_ValidPermit()
        {
            // Arrange - create the controller 
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitEdit(permit, "A");

            // Assert - check the result 
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
        }

        [Test]
        public void PermitEditPost_CancellationRequestCanEdit_ValidPermit()
        {
            // Arrange - create the controller 
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 5).FirstOrDefault();
            controller.ControllerContext = controllerContext.Object;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitEdit(permit);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => (msg.messageType.Equals(MessageType.BTMCancelsPermitToADM)) && msg.ReplyTo == "User@elegant.com")), Times.Once);
        }

        [Test]
        public void PermitEditPost_CannotEdit_InvalidPermit()
        {
            // Arrange - create the controller 
            Permit permit = new Permit();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.PermitEdit(permit);

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PermitViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void PermitEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SavePermit(permit, permit.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.PermitEdit(permit);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePermit(permit, permit.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void PermitDeleteGet_SearchStringEmptyValidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = controller.PermitDelete(2) as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Permit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
        }

        [Test]
        public void PermitDeleteGet_SearchStringDValidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = controller.PermitDelete(2, "D") as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("D", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Permit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
        }

        [Test]
        public void PermitDeleteGet_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PermitDelete(15);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(permit);
        }

        [Test]
        public void PermitDeleteConfirmedPost_SearchStringEmptyValidPermit()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(1), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void PermitDeleteConfirmedPost_SearchStringDValidPermit()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PermitDeleteConfirmed(1, "D");

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(1), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }
        #endregion


        #region Insurance
        [Test]
        public void CreateGet_InsuranceOf_SearchStringEmpty_ExistingEmployee()
        {
            // Arrange - create the controller                 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceCreate(5) as ViewResult;
            Insurance insurance = (Insurance)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(insurance);
        }

        [Test]
        public void CreateGet_InsuranceOf_SearchStringA_ExistingEmployee()
        {
            // Arrange - create the controller                 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceCreate(5, "A") as ViewResult;
            Insurance insurance = (Insurance)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(insurance);
        }
        [Test]
        public void CreateGet_InsuranceOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.PermitCreate(1500);
            Insurance insurance = mock.Object.Insurances.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(insurance);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringEmpty_ValidInsurance()
        {
            // Arrange - create the controller                 
            Insurance insurance = new Insurance { EmployeeID = 5, StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), Days = 180 };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceCreate(insurance);

            // Assert - check the result 
            mock.Verify(m => m.SaveInsurance(insurance, 5), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringA_ValidInsurance()
        {
            // Arrange - create the controller                 
            Insurance insurance = new Insurance { EmployeeID = 5, StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), Days = 180};
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceCreate(insurance, "A");

            // Assert - check the result 
            mock.Verify(m => m.SaveInsurance(insurance, 5), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidInsurance()
        {
            // Arrange - create the controller
            Insurance insurance = new Insurance();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            ViewResult result = controller.InsuranceCreate(insurance) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveInsurance(It.IsAny<Insurance>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(InsuranceViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void InsuranceCreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Insurance insurance = mock.Object.Insurances.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveInsurance(insurance, insurance.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.InsuranceCreate(insurance);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveInsurance(insurance, insurance.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void InsuranceEditGet_CanEditSearchStringEmpty_ValidEmployeeID()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceEdit(2) as ViewResult;
            InsuranceViewModel insurance = (InsuranceViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, insurance.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void InsuranceEditGet_CanEditSearchStringA_ValidEmployeeID()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceEdit(2, "A") as ViewResult;
            InsuranceViewModel insurance = (InsuranceViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);

            Assert.AreEqual(2, insurance.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void InsuranceEditGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.InsuranceEdit(15);
            Insurance insurance = mock.Object.Insurances.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(insurance);
        }

        [Test]
        public void InsuranceEditPost_CanEditSearchStringEmpty_ValidInsurance()
        {
            // Arrange - create the controller 
            Insurance insurance = new Insurance { EmployeeID = 5, StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), Days = 180 };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceEdit(insurance);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveInsurance(insurance, 5), Times.Once);

        }


        [Test]
        public void SaveInsuranceEditPost_CanEditSearchStringA_ValidInsurance()
        {
            // Arrange - create the controller 
            Insurance insurance = new Insurance { EmployeeID = 5, StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), Days = 180 };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceEdit(insurance, "A");

            // Assert - check the result 
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveInsurance(insurance, 5), Times.Once);
        }

     
        [Test]
        public void InsuranceEditPost_CannotEdit_InvalidInsurance()
        {
            // Arrange - create the controller 
            Insurance insurance = new Insurance();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.InsuranceEdit(insurance);

            // Assert - check the result 
            mock.Verify(m => m.SaveInsurance(insurance, 1), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(InsuranceViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void InsuranceEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Insurance insurance = mock.Object.Insurances.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveInsurance(insurance, insurance.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.InsuranceEdit(insurance);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveInsurance(insurance, insurance.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void InsuranceDeleteGet_SearchStringEmptyValidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = controller.InsuranceDelete(2) as ViewResult;
            Insurance insurance = (Insurance)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Insurance), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, insurance.EmployeeID);
        }

        [Test]
        public void InsuranceDeleteGet_SearchStringDValidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = controller.InsuranceDelete(2, "D") as ViewResult;
            Insurance insurance = (Insurance)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("D", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Insurance), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, insurance.EmployeeID);
        }

        [Test]
        public void InsuranceDeleteGet_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.InsuranceDelete(15);
            Insurance Insurance = mock.Object.Insurances.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeleteInsurance(It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(Insurance);
        }

        [Test]
        public void InsuranceDeleteConfirmedPost_SearchStringEmptyValidInsurance()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteInsurance(1), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void InsuranceDeleteConfirmedPost_SearchStringDValidInsurance()
        {
            // Arrange - create the controller 
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.InsuranceDeleteConfirmed(1, "D");

            // Assert - check the result 
            mock.Verify(m => m.DeleteInsurance(1), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        #endregion
        #endregion

        #region "BTs in process" tab

        #region GetBusinessTripBTM

        [Test]
        public void GetBusinessTripBTM_SearchStringNull_SearchStringNull()
        {
            // Arrange

            // Act
            string searchString = null;
            var view = controller.GetBusinessTripBTM(searchString);
            SelectList SelectedList = ((ViewResult)view).ViewBag.DepartmentsList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SearchString);

        }
        
        [Test]
        public void GetBusinessTripBTM_SearchStringEmpty_SearchStringEmpty()
        {
            // Arrange

            // Act
            string searchString = "";
            var view = controller.GetBusinessTripBTM(searchString);
            SelectList SelectedList = ((ViewResult)view).ViewBag.DepartmentsList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SearchString);

        }
        
        [Test]
        public void GetBusinessTripBTM_SearchStingAA_SearchStingAA()
        {
            // Arrange

            // Act
            string selectedDepartment = "AA";
            var view = controller.GetBusinessTripBTM(selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("AA", ((ViewResult)view).ViewBag.SearchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("AA", ((ViewResult)view).ViewBag.SearchString);

        }

        #endregion

        #region GEtBusinessTripDataBTM

        [Test]
        public void GEtBusinessTripDataBTM_SearchStringEmpty_ViewAllEmployees()
        {
            // Arrange
            mock = new Mock<IRepository>();
            // Act
            string searchString = "";
            var view = controller.GetBusinessTripDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataBTM(searchString).Model;
            Employee[] employeeList = result.ToArray();

            // Assert

            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(10, employeeList.Length);
            Assert.AreEqual(8, employeeList[0].EmployeeID);
            Assert.AreEqual(17, employeeList[5].EmployeeID);

        }

        [Test]
        public void GEtBusinessTripdataBTM_SearchStringAND_ViewSelectedEmployees()
        {
            // Arrange

            // Act
            string searchString = "ANO";
            var view = controller.GetBusinessTripDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataBTM(searchString).Model;
            Employee[] employeeList = result.ToArray();

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(employeeList.Length, 1);
            Assert.AreEqual(employeeList[0].EmployeeID, 7);
        }

        #endregion

        
        //TODO: duplicated in ACC, BTM
        [Test]
        public void CountingDaysUsedInBT_ReportedBT_IncreasedDaysUsedInBT()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            int result1 = controller.CountingDaysUsedInBT(bTrip);

            //Assert
            Assert.AreEqual(20, result1);
        }
                
        #region  ReportConfirmedBTs

        [Test]
        public void ReportConfirmedBTs_SelectedTwoConfirmedBTs_ChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs, "AS");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 5
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("AS", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_Null_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = null;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_BadArrayOfString_CannotParseIDs()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4gt" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_BadArrayOfString_ParsedOnlyOneID()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "4gr" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_NotExistingBT_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "100" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            Assert.IsNull(bTrip1);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_RegisteredBTsInputs_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        }

        [Test]
        public void ReportConfirmedBTs_VisaIsNull_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsNull(bTrip1.BTof.Visa);
        }

        [Test]
        public void ReportConfirmedBTs_VisaIsNotNull_DaysAndEntriesUsedInBTAdded()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip1.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 0, EmployeeID = bTrip1.EmployeeID, Entries = 2, EntriesUsedInBT = 0, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip1.BTof.Visa = visa;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs, "A");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(20, bTrip1.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(1, bTrip1.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void ReportConfirmedBTs_TwoBtsAreAdded_DaysAndEntriesUsedInBTAddedTwice()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "3", "4" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == bTrip1.EmployeeID).FirstOrDefault();
            Visa visa = new Visa { VisaOf = employee, VisaType = "C07", Days = 180, DaysUsedInBT = 0, EmployeeID = employee.EmployeeID, Entries = 2, EntriesUsedInBT = 0, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip1.BTof.Visa = visa;
            bTrip2.BTof.Visa = visa;
            employee.Visa = visa;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs, "D");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 3
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(23, employee.Visa.DaysUsedInBT);
            Assert.AreEqual(2, employee.Visa.EntriesUsedInBT);
        }

        [Test]
        public void ReportConfirmedBTs_SelectedTwoConfirmedBTsConcurrency_JsonErrorResult()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "5" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4)), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 5)), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion
        
        #region BTMArrangeBT

        [Test]
        public void BTMArrangeBT_NotExistingBT_HttpNotFound()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.BTMArrangeBT(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void BTMArrangeBT_ExistingPlannedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.BTMArrangeBT(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
        }

        [Test]
        public void BTMArrangeBT_ExistingRegisteredBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            var view = controller.BTMArrangeBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(2, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNull(businessTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBTOrderDatesNull_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.OrderStartDate = null;
            businessTrip.OrderEndDate = null;
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.StartDate.AddDays(-1), businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBTOrderStartDateNull_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.OrderStartDate = null;
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedModifiedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            var view = controller.BTMArrangeBT(4);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(4, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        }

        #endregion
        
        #region SaveArrangeBT

        [Test]
        public void SaveArrangedBT_ValidRegisteredBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 2 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidRegisteredModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 13 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt, "");

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void SaveArrangedBT_ValidConfirmedModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 14 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidConfirmedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 3 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Confirmed, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Once); 
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);
        }

        [Test]
        public void SaveArrangedBT_PlannedModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 12 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never); 
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);
            Assert.IsTrue(bt.HabitationConfirmed);
        }

        [Test]
        public void SaveArrangedBT_NotValidBT_BTSaved()
        {
            //Arrange
            controller.ModelState.AddModelError("error", "error");
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 12 select b).FirstOrDefault();

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never); 
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("BTMArrangeBT", ((ViewResult)result).ViewName);
            Assert.AreEqual("krakow", bt.Habitation);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidRegisteredBTConcurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 2 select b).FirstOrDefault();

            //Act
            JsonResult result = (JsonResult)controller.SaveArrangedBT(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never); 
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);

        }

        //TODO: testcase refactoring is needed: controller.Plan(bTrip) from ADMController is used when testing SaveArrangedBT
        //[Test]
        //public void SaveArrangedBT_OrderDatesInVacationPeriod_JsonErrorResult()
        //{
        //    //Arrange
        //    mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new VacationAlreadyExistException());
        //    BusinessTrip bTrip = new BusinessTrip
        //    {
        //        OrderStartDate = new DateTime(2014,02,12),
        //        OrderEndDate = new DateTime(2014,02,28),
        //        Status = BTStatus.Planned,
        //        EmployeeID = 1,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "test",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bTrip);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert
        //    Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("Absence already planned on this period for this user. "
        //                              + "Please change OrderDates or if BT haven\'t OrderDates "
        //                              + "change \'From\' or \'To\'", data);
        //}
        
        #endregion

        #region DeleteBTBTM

        [Test]
        public void DeleteBTBTM_NotExistingBT_HttpNotFound()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);

        }
        [Test]
        public void DeleteBTBTM_BTNotCancelled_HttpNotFound()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(1);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);

        }

        [Test]
        public void DeleteBTBTM_ExistingBT_ViewDeleteBTConfirmedBTM()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(19);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 19).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(19, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);

        }
        #endregion
        
        #region DeleteBTBTMConfirmed()

        [Test]
        public void DeleteBTBTMConfirmed_BTisNotCancelled_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(2);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(2), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }

        [Test]
        public void DeleteBTBTMConfirmed_ExistingCancelledBT_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 19).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(19);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(19), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }

        [Test]
        public void DeleteBTBTMConfirmed_NotExistingBusinessTripID_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(1000);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(1000), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }
        #endregion
        
        #region  Reject_BT_BTM

        [Test]
        public void Reject_BT_BTM_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_BTM(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_BTM_EmptyString_ExistingBT()
        {
            //Arrange
            string searchString = "";

            // Act
            var view = controller.Reject_BT_BTM(3, searchString);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void Reject_BT_BTM_aaAndDefaultJsondata_ExistingBT()
        {
            //Arrange
            string searchString = "aa";
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            // Act
            var view = controller.Reject_BT_BTM(id: 3, searchString: searchString);
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
            Assert.AreEqual(rowVersion, businessTripAfterCall.RowVersion);
        }

        [Test]
        public void Reject_BT_BTM_aaAndEmptyJsondata_ExistingBT()
        {
            //Arrange
            string searchString = "aa";
            string json = "";
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            // Act
            var view = controller.Reject_BT_BTM(id: 3, jsonRowVersionData: json, searchString: searchString);
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
        }

        [Test]
        public void Reject_BT_BTM_JsonDataNotEmpty_ExistingBT()
        {
            //Arrange
            string searchString = "AA";
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_BTM(3, jsonData, searchString);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        [Test]
        public void Reject_BT_BTM_JsonDataContainsWhiteSpace_ExistingBT()
        {
            //Arrange
            string searchString = "AA";
            byte[] rowVersion = { 0, 0, 0, 0, 0, 2, 159, 230 };
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_BTM(3, jsonData.Replace("+", " "), searchString);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        #endregion
        
        #region  Reject_BT_BTM_Confirm
        [Test]
        public void Reject_BT_BTM_Confirm_Default_NotExistingBT()
        {
            //Arrange
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            // Act
            var view = controller.Reject_BT_BTM_Confirm(bt);

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(bt);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_EmptyStringComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.RejectComment = "";

            // Act
            var view = controller.Reject_BT_BTM_Confirm(businessTrip, null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("Reject_BT_BTM", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual("", businessTrip.RejectComment);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_aSearchAndDefaultComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            // Act
            var view = controller.Reject_BT_BTM_Confirm(businessTrip, "a");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("Reject_BT_BTM", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
            Assert.AreEqual("a", ((ViewResult)view).ViewBag.SearchString);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingRegisteredBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Once);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingRegisteredModifiedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip, searchString: "D");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Once);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingConfirmedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip, "d");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("d", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingConfirmedModifiedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsible) && msg.ReplyTo == "User@elegant.com")), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_ConfirmConcurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            businessTrip.RejectComment = "reject";

            // Act
            JsonResult result = (JsonResult)controller.Reject_BT_BTM_Confirm(businessTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion
        
        #region RewriteBTsPropsFromRepositoryBTM

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_RegisteredBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderEndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), EmployeeID = 2, Habitation = "Linkopin", HabitationConfirmed = false, Status = BTStatus.Registered };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("Linkopin", result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_PlannedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, LocationID = 1, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 27), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 2, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting", Habitation = "Kyiv", HabitationConfirmed = true };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.BusinessTripID);
            Assert.AreEqual(2, result.OldLocationID);
            Assert.AreEqual("LDL", result.OldLocationTitle);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual("Kyiv", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(BTStatus.Modified | BTStatus.Planned, result.Status);
            Assert.AreEqual(new DateTime(2013, 08, 31), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2013, 09, 28), result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
            Assert.AreEqual("visa expired", result.RejectComment);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_ConfirmedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), OrderStartDate = new DateTime(2013, 02, 01), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("LDL", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("lodz", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderStartDate);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_ConfirmedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014, 10, 01), EndDate = new DateTime(2014, 10, 12), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2, OrderStartDate = new DateTime(2013, 02, 01), OrderEndDate = new DateTime(2013, 02, 01), DaysInBtForOrder = 1, RejectComment = "reject" };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("LDL", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual(null, result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderEndDate);
            Assert.AreEqual(1, result.DaysInBtForOrder);
            Assert.AreEqual("reject", result.RejectComment);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_CancelledBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 18).FirstOrDefault();
            businessTripFromRepository.RejectComment = "reject";
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 18, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 25), OrderStartDate = new DateTime(2014, 01, 10), OrderEndDate = new DateTime(2014, 02, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and cancelled", Manager = "xtwe", Purpose = "meeting" };
            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(18, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual(null, result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2014, 01, 10), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
            Assert.AreEqual("reject", result.RejectComment);
            Assert.AreEqual("visa expired", result.CancelComment);

        }

        #endregion

        #region RewriteBTsPropsFromRepositoryWhenReject

        [Test]
        public void RewriteBTsPropsFromRepositoryWhenReject_RegisteredBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderEndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), EmployeeID = 2, Habitation = "Linkopin", HabitationConfirmed = false, Status = BTStatus.Registered };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryWhenReject(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("krakow", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.OrderStartDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryWhenReject_NullPropsInBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderStartDate = new DateTime(2012, 10, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), EmployeeID = 2, Status = BTStatus.Registered, FlightsConfirmed = true };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryWhenReject(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("krakow", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.IsFalse(result.FlightsConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.OrderStartDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);

        }
        #endregion

        #endregion

        #region PrivateTrips tab

        #region GetPrivateTripBTM

        [Test]
        public void GetPrivateTripBTM_EmptyString_searchString()
        {
            // Arrange
            string searchString = "";

            // Act

            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripBTM_NullString_searchString()
        {
            // Arrange
            string searchString = null;

            // Act
            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripBTM_dan_searchString()
        {
            // Arrange
            string searchString = "dan";

            // Act

            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("dan", ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion


        #region GetPrivateTripDataBTM

        [Test]
        public void GetPrivateTripData_Default_AllEmployees()
        {
            // Arrange
            string searchString = "";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Kowwood");
            Assert.AreEqual(result.ToArray()[1].LastName, "Struz");
            Assert.AreEqual(23, result.ToArray().Length);
            Assert.AreEqual(result.ToArray()[2].LastName, "Daolson");
            Assert.AreEqual(result.ToArray()[3].LastName, "Kowood");
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataBTM_FilterTep_EmployeesContain_Tep()
        {
            // Arrange - create the controller     
            string searchString = "ted";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[0].FirstName, "Tymur");
            Assert.AreEqual(result.ToArray()[0].EID, "tedk");
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataBTM_Filteraa_EmployeesContain_aa()
        {
            // Arrange
            string searchString = "aa";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual(0, result.ToArray().Length);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        #endregion


        #region PrivateTripCreateGet
        [Test]
        public void PrivateTripCreate_ExistingVisaDefault_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            var result = controller.PrivateTripCreate(1);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void PrivateTripCreate_ExistingVisaStringEmpty_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            string searchString = "";
            var result = controller.PrivateTripCreate(1, searchString);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void PrivateTripCreate_ExistingVisa_dan_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripCreate(1, searchString);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void PrivateTripCreate_NonExistingVisa_CreateBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1000 select e).FirstOrDefault();

            //Act
            var result = controller.PrivateTripCreate(1000);
            //Assert       
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region PrivateTripCreatePost

        [Test]
        public void PrivateTripCreatePost_CreatedPT_CreatePTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.PrivateTripCreate(privateTrip, "sd");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("sd", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripCreatePost_PTSerchStringEmpty_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.PrivateTripCreate(privateTrip, "");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripCreatePost_PTSerchString_dan_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripCreate(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripCreatePost_PTSerchString_danVisaNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 10 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripCreate(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripCreatePost_PTSerchString_danVisaNotNull_AndDaysEntriesAreNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 11 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripCreate(privateTrip, searchString);

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 11), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripCreatePost_NotValidBT_dan_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 0, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-180), EmployeeID = 1 };
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.PrivateTripCreate(privateTrip, "dan");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            mock.Verify(m => m.SaveVisa(It.IsAny<Visa>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void PrivateTripCreatePost_NotValidBTNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 0, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-180), EmployeeID = 1 };

            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.PrivateTripCreate(privateTrip);

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            mock.Verify(m => m.SaveVisa(It.IsAny<Visa>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
        }


        #endregion

        #region PrivateTripEditGet

        [Test]
        public void PrivateTripEditGet_CannotEdit_InvalidPrivateTripID()
        {
            // Arrange 
            string searchString = "t";

            // Act - call the action method
            var result = (HttpNotFoundResult)controller.PrivateTripEdit(0, searchString);
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.PrivateTripID == 0).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void PrivateTripEditGet_CanEdit_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = "";

            // Act - call the action method
            var result = controller.PrivateTripEdit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void PrivateTripEditGet_CanEditNullSearchString_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = null;

            // Act - call the action method
            var result = controller.PrivateTripEdit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void PrivateTripEditGet_CanEdit_TeSearchString_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = "Te";

            // Act - call the action method
            var result = controller.PrivateTripEdit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        #endregion

        #region PrivateTripEditPost

        [Test]
        public void PrivateTripEditPost_CanEdit_ValidPrivateTrip_OldPrivateTripNotNull_StartDateModified()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 2, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), EmployeeID = 3 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault();
            visa.DaysUsedInPrivateTrips = 1;
            mock.Object.PrivateTrips.Add(privateTrip); 
            privateTrip.StartDate = privateTrip.StartDate.AddDays(1);
            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(9, visa.DaysUsedInPrivateTrips);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_ValidPrivateTrip_OldPrivateTripNotNull_EndDateModified()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 2, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014,10,30), EmployeeID = 3 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault();
            visa.DaysUsedInPrivateTrips = 1;
            mock.Object.PrivateTrips.Add(privateTrip);
            privateTrip.EndDate = privateTrip.EndDate.AddDays(1);
            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(11, visa.DaysUsedInPrivateTrips);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_ValidPrivateTrip_NoOldPrivateTrip()
        {
            //Artificial case, that shouldn't happen under normal circumstances

            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-25), EmployeeID = 3 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault();
            visa.DaysUsedInPrivateTrips = 0; 
            mock.Object.PrivateTrips.Clear();

            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(16, visa.DaysUsedInPrivateTrips);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_VisaNull_ValidPrivateTrip()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-25), EmployeeID = 10 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_VisaNotNull_DaysEntriesAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 9, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 11 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 11), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_VisaNotNull_EntriesAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 10, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-9), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 12 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 12), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(3, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripEditPost_CanEdit_VisaNotNull_DaysAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 11, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-9), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 13 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.PrivateTripEdit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 13), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripEditPost_CannotEdit_InvalidPrivateTrip()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip();
            string searchString = "";

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            JsonResult result = (JsonResult)controller.PrivateTripEdit(privateTrip, searchString);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("", data);
            //Assert.AreEqual("", result.ViewName);
            //Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.ViewData.Model);
        }

        [Test]
        public void PrivateTripEditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePrivateTrip(It.IsAny<PrivateTrip>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.PrivateTripEdit(mock.Object.PrivateTrips.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePrivateTrip(It.IsAny<PrivateTrip>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region PrivateTripDeleteGet
        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripIDAndStringEmpty()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDelete(1);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripIDAndEmptyString()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDelete(1, searchString);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripIDAnd_tep()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.PrivateTripDelete(1, searchString);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripID_AndNull_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            var result = controller.PrivateTripDelete(1);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripID_AndEmptyString_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDelete(1, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void PrivateTripDeleteGet_ValidPrivateTripID_And_tep_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.PrivateTripDelete(1, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void PrivateTripDeleteGet_InValidPrivateTripID_AndNull()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            var result = controller.PrivateTripDelete(100);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void PrivateTripDeleteGet_InValidPrivateTripID_AndEmptyString()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDelete(100, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void PrivateTripDeleteGet_InValidPrivateTripID_And_tep()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.PrivateTripDelete(100, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        #endregion

        #region PrivateTripDeleteConfirmed

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_AndNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            var result = controller.PrivateTripDeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_AndStringNotEmpty__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            string searchString = "a";
            var result = controller.PrivateTripDeleteConfirmed(1, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("a", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_And_tep__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            string searchString = "tep";
            var result = controller.PrivateTripDeleteConfirmed(1, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_And_tep_VisaNotNullAndDaysEntriesNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method
            string searchString = "tep";
            var result = controller.PrivateTripDeleteConfirmed(9, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(9), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_tep_VisaNull__PTIsDeleted()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.EmployeeID == 10).FirstOrDefault();
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.PrivateTripDeleteConfirmed(privateTrip.PrivateTripID, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(8), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_ValidPrivateTripID_EmptySearchString_VisaNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDeleteConfirmed(8, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(8), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void PrivateTripDeleteConfirmed_InValidPrivateTripID_EmptySearchString_VisaNull__PTIsNotDeleted()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.EmployeeID == 100).FirstOrDefault();
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 100).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            string searchString = "";
            var result = controller.PrivateTripDeleteConfirmed(100, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(100), Times.Never);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, privateTrip);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }


        #endregion

        #region CountingDaysUsedInPT

        [Test]
        public void CountingDaysUsedInPT_PT_DaysCount()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(b => b.PrivateTripID == 1).FirstOrDefault();

            //Act
            int result = controller.CountingDaysUsedInPT(privateTrip);

            //Assert
            Assert.AreEqual(21, result);
        }

        #endregion

        #region SearchPrivateTripData

        [Test]
        public void SearchPrivateTripData_StringEmpty_AllEmpployees()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "");

            //Assert
            Assert.AreEqual(23, resultList.Count);
            Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
            Assert.AreEqual("PaidVac", resultList.ToArray()[6].FirstName);
            Assert.AreEqual("Only", resultList.ToArray()[6].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(resultList, typeof(Employee));

        }
        [Test]
        public void SearchPrivateTripData_StringABRAKADARA_NothingFound()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "ABRAKADARA");

            //Assert
            Assert.AreEqual(0, resultList.Count);


        }

        [Test]
        public void SearchPrivateTripData_StringAN_SelectedEmployees()
        {
            //Arrange

            //Act
            var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "AN");

            //Assert
            Assert.AreEqual(6, resultList.Count);
            Assert.AreEqual("Anatoliy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Struz", resultList.ToArray()[0].LastName);
            Assert.AreEqual("Ivan", resultList.ToArray()[1].FirstName);
            Assert.AreEqual("Daolson", resultList.ToArray()[1].LastName);


        }

        #endregion



        #endregion

    }
}
