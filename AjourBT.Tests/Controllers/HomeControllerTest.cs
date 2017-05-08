using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AjourBT;
using AjourBT.Controllers;
using NUnit.Framework;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        #region DIRVIew

        [Test]
        public void DIRView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.DIRView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
        }

        [Test]
        public void DIRView_2_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            // Act
            var result = controller.DIRView(tab);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
        }

        #endregion

        #region PUView
        [Test]
        public void PUView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.PUView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, null);
        }

        [Test]
        public void PUView_DefaultTabAndEmptyString_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string selectedDep = String.Empty;

            // Act
            var result = controller.PUView(0, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        [Test]
        public void PUView_2AndEmptyString_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            string selectedDep = String.Empty;

            // Act
            var result = controller.PUView(tab, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        [Test]
        public void PUView_2AndSDDIU_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            string selectedDep = "SDDDA";

            // Act
            var result = controller.PUView(tab, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        #endregion

        #region BTMView
        [Test]
        public void BTMView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.BTMView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, "");
        }

        [Test]
        public void BTMView_DefaultTab_Default_Null_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string searchString = null;

            // Act
            var result = controller.BTMView(0, searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, searchString);
        }

        [Test]
        public void BTMView_DefaultTab_Default_dan_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string searchString = "dan";

          //   Act
            var result = controller.BTMView(0,searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, searchString);
        }

        [Test]
        public void BTMView_DefaultTab_StringEmpty_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string searchString = "";

            // Act
            var result = controller.BTMView(0, searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, searchString);
        }

        [Test]
        public void BTMView_DefaultTab_A_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string searchString = "A";

            // Act
            var result = controller.BTMView(0,searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, searchString);
        }

        [Test]
        public void BTMView_Tab1_SDDDA_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 1;
            string searchString = "A";

            // Act
            var result = controller.BTMView(tab, searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual(((ViewResult)result).ViewBag.SearchString, searchString);
        }

        #endregion

        #region ADMView
        [Test]
        public void ADMView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.ADMView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, null);
        }

        [Test]
        public void ADMView_DefaultTabAndEmptyString_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            string selectedDep = String.Empty;

            // Act
            var result = controller.ADMView(0, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        [Test]
        public void ADMView_2AndEmptyString_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            string selectedDep = String.Empty;

            // Act
            var result = controller.ADMView(tab, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        [Test]
        public void ADMView_2AndSDDIU_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            string selectedDep = "SDDDA";

            // Act
            var result = controller.ADMView(tab, selectedDep);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual(((ViewResult)result).ViewBag.SelectedDepartment, selectedDep);
        }

        #endregion

        #region ACCView

        [Test]
        public void ACCView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.ACCView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
        }

        [Test]
        public void ACCView_2_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            // Act
            var result = controller.ACCView(tab);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
        }

        #endregion

        #region VUView

        [Test]
        public void VUView_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.VUView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, 0);
            Assert.AreEqual("", ((ViewResult)result).ViewName);

        }

        [Test]
        public void VUView_2_model()
        {
            // Arrange
            HomeController controller = new HomeController();
            int tab = 2;
            // Act
            var result = controller.VUView(tab);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(((ViewResult)result).Model);
            Assert.AreEqual(((ViewResult)result).Model, tab);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }

        #endregion

        #region DataBaseDeleteError
        [Test]
        public void DataBaseDeleteError_Default_model()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.DataBaseDeleteError();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }
        #endregion

        #region ABMView


    [Test]
	public void ABMView_Default_view()
	{
		        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            var result = controller.ABMView();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }
	}
	
        #endregion

        #region EMPView

    [Test]
    public void EMPView_Default_model()
    {
        // Arrange
        HomeController controller = new HomeController();

        // Act
        var result = controller.EMPView();

        // Assert
        Assert.IsInstanceOf(typeof(ViewResult), result);
        Assert.IsNotNull(((ViewResult)result).Model);
        Assert.AreEqual(((ViewResult)result).Model, 0);
    }

    [Test]
    public void EMPView_2_model()
    {
        // Arrange
        HomeController controller = new HomeController();
        int tab = 2;
        // Act
        var result = controller.EMPView(tab);

        // Assert
        Assert.IsInstanceOf(typeof(ViewResult), result);
        Assert.IsNotNull(((ViewResult)result).Model);
        Assert.AreEqual(((ViewResult)result).Model, tab);
    }

        #endregion


    #region HelpView

    [Test]
    public void HelpView_Default_model()
    {
        // Arrange
        HomeController controller = new HomeController();

        // Act
        var result = controller.HelpView();

        // Assert
        Assert.IsInstanceOf(typeof(ViewResult), result);
        Assert.IsNotNull(((ViewResult)result).Model);
        Assert.AreEqual(((ViewResult)result).Model, 0);
    }

    [Test]
    public void HelpView_2_model()
    {
        // Arrange
        HomeController controller = new HomeController();
        int tab = 2;
        // Act
        var result = controller.HelpView(tab);

        // Assert
        Assert.IsInstanceOf(typeof(ViewResult), result);
        Assert.IsNotNull(((ViewResult)result).Model);
        Assert.AreEqual(((ViewResult)result).Model, tab);
    }

    #endregion
    }
}
