using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.Infrastructure
{
    public struct Tabs
    {
        public struct ABM
        {
            public const int Countries = 0;
            public const int Holidays = 1;
            public const int Calendar = 2;
            public const int DaysFromBT = 3;
            public const int WTR = 4;
            public const int Absence = 5;
        }

        public struct ACC
        {
            public const int CurrentAndFutureBTs = 0; 
            public const int AccountableBTs = 1; 
            public const int Messages = 2;
        }

        public struct ADM
        {
            public const int VisasAndPermits = 0;
            public const int BTs = 1;
            public const int Employees = 2;
            public const int Messages = 3;
        }

        public struct BDM
        {
            public const int Greetings = 0;
        }

        public struct BTM
        {
            public const int VisasAndPermits = 0;
            public const int BTsInProcess = 1;
            public const int PrivateTrips = 2;
            public const int Messages = 3;
        }

        public struct DIR
        {
            public const int BusinessTrips = 0;
            public const int Messages = 1;
        }

        public struct EMP
        {
            public const int YourBTs = 0;
            public const int Visa = 1;
            public const int Absence = 2; 
            public const int DaysFromBts = 3; 
            public const int Birthdays = 4;
        }

        public struct PU
        {
            public const int Departments = 0; 
            public const int Employees = 1; 
            public const int Locations = 2; 
            public const int Positions = 3;
            public const int Users = 4; 
            public const int Units = 5; 
            public const int FinishedBTs = 6;
            public const int Log = 7;
        }

        public struct VU
        { 
            public const int BTsByDatesAndLocation = 0; 
            public const int BTsByQuarters = 1; 
            public const int BTsInPreparationProcess = 2; 
            public const int PrivateTrips = 3; 
            public const int Calendar = 4; 
            public const int BTsByUnits = 5; 
            public const int VisasAndPermits = 6; 
            public const int DaysFromBT = 7; 
        }

        public struct Help
        {
            public const int Map = 0;
            public const int Description = 1;
          
        }

        public struct HR
        {
            public const int Foo = 0;
            public const int Foo1 = 1;
        }
    }
}
