using NUnit.Framework;
using AjourBT.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Tests.Helpers
{
    [TestFixture]
    public class CustomDropdownListTest
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
        public void CustomDropdownList_WithoutDefaultOption()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Value = "1", Text = "Test1"},
                new SelectListItem{Value = "2", Text = "Test2"}
            };

            //Act
            string result = helper.CreateCustomDropdownList(items, "testDropdown").ToString();
            string expected = "<select name=testDropdown>" +
                                "<option value=0></option>" +
                                "<option value=1>Test1</option>" +
                                "<option value=2>Test2</option>" +
                              "</select>";

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CustomDropdownList_WithDefaultOption()
        {
            //Arrange
            HtmlHelper helper = new HtmlHelper(viewContext, new FakeViewDataContainer());
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem{Value = "1", Text = "Test1"},
                new SelectListItem{Value = "2", Text = "Test2"}
            };

            //Act
            string result = helper.CreateCustomDropdownList(items, "testDropdown","Default Param").ToString();
            string expected = "<select name=testDropdown>" +
                                "<option value=0>Default Param</option>" +
                                "<option value=1>Test1</option>" +
                                "<option value=2>Test2</option>" +
                              "</select>";

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
