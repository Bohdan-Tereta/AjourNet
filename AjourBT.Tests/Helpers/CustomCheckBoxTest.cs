using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomCheckBoxTest
    {
        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } } 
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }

        ViewContext viewContext;

        [SetUp]
        public void SetUpFixture()
        {
            viewContext = new ViewContext();
            viewContext.HttpContext = new FakeHttpContext();
        }

        [Test]
        public void CreateCustomCheckBoxForRoles_Checked_RoleChecked()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());

            //Act
            string result = helper.CreateCustomCheckBox("CheckBox", true, "ADM").ToString();
            string expected = "<input id=\"CheckBox\" type=\"checkbox\" value=\"ADM\" name=\"CheckBox\" checked=\"checked\"></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForRoles_UnChecked_RoleUnChecked()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());

            //Act
            string result = helper.CreateCustomCheckBox("CheckBox", false, "ADM").ToString();
            string expected = "<input id=\"CheckBox\" type=\"checkbox\" value=\"ADM\" name=\"CheckBox\" ></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForRolesEmptyIdEmpty_UnChecked_RolesEmptyIdEmpty()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());

            //Act
            string result = helper.CreateCustomCheckBox("", false, "").ToString();
            string expected = "<input id=\"\" type=\"checkbox\" value=\"\" name=\"\" ></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForRolesNullIdNull_Checked_RolesNullIdNull()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            string role = null;
            //Act
            string result = helper.CreateCustomCheckBox(null, true, role).ToString();
            string expected = "<input id=\"\" type=\"checkbox\" value=\"\" name=\"\" checked=\"checked\"></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_CheckedBusinessTripFieldsNotNull_ProperHTMLString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip {BusinessTripID = 223, Manager = "ncru", Purpose = "Training", Responsible = "ncru" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", true, businessTrip).ToString();
            string expected = "<input id=\"checkBox\" type=\"checkbox\" value=\"223\" name=\"checkBox\" checked=\"checked\"></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripFieldsNotNull_ProperHTMLString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = "Training", Responsible = "ncru" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "<input id=\"checkBox\" type=\"checkbox\" value=\"223\" name=\"checkBox\" ></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripFieldsStringEmpty_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "", Purpose = "", Responsible = "" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_CheckedBusinessTripFieldsNull_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = null, Purpose = null, Responsible = null };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", true, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForForExistingBusinessTrip_Checked_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = "Training", Responsible = "ncru" };
            //Act
            string result = helper.CreateCustomCheckBox(null, true, businessTrip).ToString();
            string expected = "<input id=\"\" type=\"checkbox\" value=\"223\" name=\"\" checked=\"checked\"></input>";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripPurposeNull_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = null, Responsible = "ncru" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripResponsibleNull_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = "meeting", Responsible = null };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripPurposeEmpty_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = "", Responsible = "ncru" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxForExistingBusinessTrip_UnCheckedBusinessTripResponsibleEmpty_EmptyString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 223, Manager = "ncru", Purpose = "meeting", Responsible = "" };
            //Act
            string result = helper.CreateCustomCheckBox("checkBox", false, businessTrip).ToString();
            string expected = "";

            //Assert   
            Assert.AreEqual(result, expected);
        }

        [Test]
        public void CreateCustomCheckBoxIsCheckedFalse_MvcString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            int BusinessTripID = 1;
            bool chk = false;
            string name = "Test";
            string valueChecked = "";


            //Act
            string result = helper.CreateCustomCheckBox(name, chk, BusinessTripID).ToString();
            string expected = String.Format("<input id=\"{0}\" type=\"checkbox\" value=\"{1}\" name=\"{0}\" {2}></input>", name, BusinessTripID.ToString(), valueChecked);

            //Assert   
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CreateCustomCheckBoxIsCheckedTrue_MvcString()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            int BusinessTripID = 12;
            bool chk = true;
            string name = "Test";
            string valueChecked = "checked=\"checked\""; 


            //Act
            string result = helper.CreateCustomCheckBox(name, chk, BusinessTripID).ToString();
            string expected = String.Format("<input id=\"{0}\" type=\"checkbox\" value=\"{1}\" name=\"{0}\" {2}></input>", name, BusinessTripID.ToString(), valueChecked);

            //Assert   
            Assert.AreEqual(expected, result);
        }

     }
}
