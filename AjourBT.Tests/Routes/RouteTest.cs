using System;
using System.Web;
using System.Web.Routing;
using Moq;
using System.Reflection;
using NUnit.Framework;
using System.Web.Mvc;

namespace AjourBT.Tests.Routes
{
    [TestFixture]
    public class RouteTest
    {
        #region Incoming Routes

        private HttpContextBase CreateHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            // create the mock request 
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            // create the mock response 
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            // create the mock context, using the request and response 
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);

            // return the mocked context 
            return mockContext.Object;
        }

        public void TestRouteMatch(string url, string controller, string action, object routeProperties = null, string httpMethod = "GET")
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            // Act - process the route 
            RouteData result = routes.GetRouteData(CreateHttpContext(url, httpMethod));
            // Assert 
            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeProperties));
        }

        private bool TestIncomingRouteResult(RouteData routeResult, string controller, string action, object propertySet = null)
        {
            Func<object, object, bool> valCompare = (v1, v2) =>
            {
                return StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;
            };
            bool result = valCompare(routeResult.Values["controller"], controller)
            && valCompare(routeResult.Values["action"], action);
            if (propertySet != null)
            {
                PropertyInfo[] propInfo = propertySet.GetType().GetProperties();
                foreach (PropertyInfo pi in propInfo)
                {
                    if (!(routeResult.Values.ContainsKey(pi.Name) && valCompare(routeResult.Values[pi.Name], pi.GetValue(propertySet, null))))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void TestRouteFail(string url)
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            // Act - process the route 
            RouteData result = routes.GetRouteData(CreateHttpContext(url));
            // Assert 
            Assert.IsTrue(result == null || result.Route == null);
        }

        [Test]
        public void TestIncomingRoutes()
        {
            TestRouteMatch("~/", "Home", "Index");
            TestRouteMatch("~/Account/Login", "Account", "Login");
            TestRouteMatch("~/Account/Manage", "Account", "Manage");
            TestRouteMatch("~/Account/Login/UnknownSegment", "Account", "Login");
            TestRouteMatch("~/Account/Login/UnknownSegment", "Account", "Login", new { id = "UnknownSegment" });
            TestRouteMatch("~/Home/PUView", "Home", "PUView");

            TestRouteFail("~/Account/Login/UnknownSegment1/UnknownSegment2");
            TestRouteFail("~/UnknownSegment1/Account/Login/UnknownSegment2");
            TestRouteFail("~/UnknownSegment1/Account/Login/UnknownSegment2/UnknownSegment3");
        }

        [Test]
        public void RouteHasDefaultActionWhenUrlWithoutAction()
        {
            // Arrange
            var context = CreateHttpContext("~/Home");
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Act
            RouteData routeData = routes.GetRouteData(context);

            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Home", routeData.Values["controller"]);
            Assert.AreEqual("Index", routeData.Values["action"]);
            Assert.AreEqual(UrlParameter.Optional, routeData.Values["id"]);
        }

        #endregion

        #region RouteForEmbeddedResources

        [Test]
        public void RouteForFinanceReportExamplePdf()
        {
            // Arrange
            var context = CreateHttpContext("~/Images/FinReport.pdf");
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Act
            RouteData routeData = routes.GetRouteData(context);

            // Assert
            Assert.IsNotNull(routeData);
            Assert.IsInstanceOf(typeof(MvcRouteHandler), routeData.RouteHandler);
        }


        [Test]
        public void RouteForBTReportTemplateDoc()
        {
            // Arrange
            var context = CreateHttpContext("~/Images/BTreport.doc");
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Act
            RouteData routeData = routes.GetRouteData(context);

            // Assert
            Assert.IsNotNull(routeData);
            Assert.IsInstanceOf(typeof(MvcRouteHandler), routeData.RouteHandler);
        }

        #endregion


        #region Outgoing Routes

        UrlHelper GetUrlHelper(string appPath = "/", RouteCollection routes = null)
        {
            if (routes == null)
            {
                routes = new RouteCollection();
                RouteConfig.RegisterRoutes(routes);
            }

            HttpContextBase httpContext = CreateHttpContext(appPath);
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Home");
            routeData.Values.Add("action", "Index");
            RequestContext requestContext = new RequestContext(httpContext, routeData);
            UrlHelper helper = new UrlHelper(requestContext, routes);
            return helper;
        }


         [Test]
         public void TestWithDefaultControllerSpecificAction()
         {
             UrlHelper helper = GetUrlHelper();

             string url = helper.Action("action");

             Assert.AreEqual("/Home/action", url);
         }


         [Test]
         public void TestWithSpecificControllerAndAction()
         {
             UrlHelper helper = GetUrlHelper();

             string url = helper.Action("action", "controller");

             Assert.AreEqual("/controller/action", url);
         }

         [Test]
         public void TestWithSpecificControllerActionAndId()
         {
             UrlHelper helper = GetUrlHelper();

             string url = helper.Action("action", "controller", new { id = 42 });

             Assert.AreEqual("/controller/action/42", url);
         }

         [Test]
         public void RouteUrlWithDefaultValues()
         {
             UrlHelper helper = GetUrlHelper();

             string url = helper.RouteUrl(new { });
             
             Assert.AreEqual("/", url);
         }

         [Test]
         public void RouteUrlWithNewValuesOverridesDefaultValues()
         {
             UrlHelper helper = GetUrlHelper();

             string url = helper.RouteUrl(new
             {
                 controller = "controller",
                 action = "action"
             });

             Assert.AreEqual("/controller/action", url);
         }

        #endregion
    }
}
