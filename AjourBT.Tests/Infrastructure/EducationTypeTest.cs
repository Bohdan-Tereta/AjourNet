using AjourBT.Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    class EducationTypeTest
    {
        [Test]
        [TestCase(EducationType.BasicHigher, Result = "базова вища")]
        [TestCase(EducationType.BasicSecondary, Result = "базова загальна середня")]
        [TestCase(EducationType.CompleteHigher, Result = "повна вища")]
        [TestCase(EducationType.CompleteSecondary, Result = "повна загальна середня")]
        [TestCase(EducationType.None, Result = "")]
        [TestCase(EducationType.Undergraduate, Result = "неповна вища")]
        [TestCase(EducationType.Vocational, Result = "професійно-технічна")]
        public string Description_properDescription(EducationType educationType)
        {
            //Arrange

            //Act 
            return educationType.Description();

            //Assert        
            
        }
    }
}
