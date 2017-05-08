using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Infrastructure;
using System.Threading;

namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    public class SchedulerTest
    {

    #region getTimeSpan
        [Test]
        public void getTimespan_eventTimeLesserThanNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20,10,12);
            TimeSpan now = new TimeSpan(21,8,10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(23, 02, 02), result);
        }

        [Test]
        public void getTimespan_eventTimeGreaterThanNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan (21, 8, 10);
            TimeSpan now = new TimeSpan(20, 10, 12);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(00, 57, 58), result);
        }

        [Test]
        public void getTimespan_eventTimeEqualToNow_properTimespan()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(20, 10, 12);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(1,0, 0, 0), result);
        }

        [Test]
        public void getTimespan_eventTimeGreaterThan24Hours_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(25, 10, 12);
            TimeSpan now = new TimeSpan(21, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12,00,00), result);
        }

        [Test]
        public void getTimespan_eventTime0_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(0, 0, -1);
            TimeSpan now = new TimeSpan(21, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }

        [Test]
        public void getTimespan_nowIsGreaterThan24Hours_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(28, 8, 10);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }

        [Test]
        public void getTimespan_nowIsLesserThan0_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(20, 10, 12);
            TimeSpan now = new TimeSpan(0, 0, -1);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12,00,00), result);
        }

        [Test]
        public void getTimespan_resultIsLesserThan1second_12Hours()
        {
            //Arrange
            TimeSpan eventTime = new TimeSpan(0, 0, 0, 0, 1);
            TimeSpan now = new TimeSpan(0, 0, 0);

            //Act
            TimeSpan result = Scheduler.getTimespan(eventTime, now);

            //Assert        
            Assert.AreEqual(new TimeSpan(12, 00, 00), result);
        }
    #endregion 

        #region Start
        [Test]
        //[Explicit]
        public void Start_TimerActivates()
        {
            //Arrange
            Mock<IMessenger> _messenger = new Mock<IMessenger>();
            _messenger.Setup(m => m.SendGreetingMessages(It.IsAny<DateTime>())).Verifiable();
            TimeSpan eventTime = DateTime.Now.ToLocalTimeAzure().AddMilliseconds(1300).TimeOfDay;

            //Act
            Scheduler.Start(eventTime, _messenger.Object);
            Thread.Sleep(1350);
            //Assert        
            Assert.AreEqual(eventTime, Scheduler.eventTime);
            _messenger.Verify(m => m.SendGreetingMessages(It.IsAny<DateTime>()), Times.Once); 

        }
        #endregion 

        #region Stop
        [Test]
        //[Explicit]
        public void Stop_BeforeTimerActivates_TimerNotActivated()
        {
            //Arrange
            Mock<IMessenger> _messenger = new Mock<IMessenger>();
            _messenger.Setup(m => m.SendGreetingMessages(It.IsAny<DateTime>())).Verifiable();
            TimeSpan eventTime = DateTime.Now.ToLocalTimeAzure().AddMilliseconds(1300).TimeOfDay;

            //Act
            Scheduler.Start(eventTime, _messenger.Object);
            Scheduler.Stop();
            Thread.Sleep(1600);  
            //Assert        
            Assert.AreEqual(eventTime, Scheduler.eventTime);
            _messenger.Verify(m => m.SendGreetingMessages(It.IsAny<DateTime>()), Times.Never);

        }
        #endregion 

        #region OnTimerElapsed
        [Test]
        //  [Explicit]
        public void OnTimerElapsed_Notify()
        {
            //Arrange
            Mock<IMessenger> _messenger = new Mock<IMessenger>();
            _messenger.Setup(m => m.SendGreetingMessages(It.IsAny<DateTime>())).Verifiable();
            TimeSpan eventTime = DateTime.Now.ToLocalTimeAzure().AddMilliseconds(1300).TimeOfDay;

            //Act
            Scheduler.Start(eventTime, _messenger.Object);
            Scheduler.Stop();
            Thread.Sleep(1600);
            //Assert        
            Assert.AreEqual(eventTime, Scheduler.eventTime);
            _messenger.Verify(m => m.SendGreetingMessages(It.IsAny<DateTime>()), Times.Never);

        }

        #endregion 
    }
}
