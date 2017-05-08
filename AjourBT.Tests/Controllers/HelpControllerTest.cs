using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Tests.Controllers
{
    public class HelpControllerTest
    {

        [Test]
        public void GetIndex_View()
        {
            //Arrange
            HelpController controller = new HelpController();

            //Act
            var result = controller.Index() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void GetMap_View()
        {
            //Arrange
            HelpController controller = new HelpController();

            //Act
            var result = controller.Map() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

    }
}
