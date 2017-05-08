using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Abstract;

namespace AjourBT.Domain.Concrete
{
    public static class Scheduler
    {
        private static TimeSpan zeroTimeSpan = new TimeSpan(0, 0, 0);
        private static TimeSpan dayTimeSpan = new TimeSpan(24, 0, 0);
        private static TimeSpan twelveHours = new TimeSpan(12, 0, 0);
        private static TimeSpan oneSecondTimeSpan = new TimeSpan(0, 0, 1);
        private static TimeSpan negativeTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

        private static Timer timer;
        public static TimeSpan eventTime { get; set; }
        private static IMessenger messenger;

        public static TimeSpan getTimespan(TimeSpan eventTime, TimeSpan now)
        {
            TimeSpan result;
            if (eventTime < zeroTimeSpan || eventTime > dayTimeSpan ||
                now < zeroTimeSpan || now > dayTimeSpan)
                return twelveHours;
            if (eventTime > now)
                result = eventTime - now;
            else
                result = dayTimeSpan - (now - eventTime);
            if (result < oneSecondTimeSpan)
                return twelveHours;
            return result;

        }

        public static void Start(TimeSpan timeOfDay, IMessenger _messenger)
        {
            eventTime = timeOfDay;
            messenger = _messenger;
            TimeSpan timespan = getTimespan(timeOfDay, DateTime.Now.ToLocalTimeAzure().TimeOfDay);
            timer = new Timer(OnTimerElapsed, null, timespan, negativeTimeSpan);
        }

        public static void Stop()
        {
            timer.Dispose();
        }

        private static void OnTimerElapsed(Object state)
        {
            messenger.SendGreetingMessages(DateTime.Now.ToLocalTimeAzure());
            messenger.SendVisaWarningMessage(DateTime.Now.ToLocalTimeAzure());

            TimeSpan timespan = getTimespan(eventTime, DateTime.Now.ToLocalTimeAzure().TimeOfDay);
            timer.Change(timespan, negativeTimeSpan);
        }

    }

}
