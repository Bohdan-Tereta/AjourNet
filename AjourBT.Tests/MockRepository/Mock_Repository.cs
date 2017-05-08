using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels; 


namespace AjourBT.Tests.MockRepository
{
    public static class Mock_Repository
    {

        public static Mock<IRepository> mock;

        static List<Department> departments;
        static List<Employee> employees;
        static List<BusinessTrip> businessTrips;
        static List<Location> locations;
        static List<VisaRegistrationDate> visaRegistrationDates;
        static List<Visa> visas;
        static List<Permit> permits;
        static List<Passport> passports;
        static List<Position> positions;
        static List<PrivateTrip> privateTrips;
        static List<Country> countries;
        static List<Journey> journeys;
        static List<Holiday> holidays;
        static List<CalendarItem> calendarItems;
        static List<Unit> units;
        static List<Overtime> overtimes;
        static List<Vacation> vacations;
        static List<Sickness> sicks;
        static List<Insurance> insurances;
        static List<QuestionSet> questionSets;
        static List<Questionnaire> questionnaires;
        public static Mock<IRepository> CreateMock()
        {
            //if (mock == null)
            //    mock = new Mock<IRepository>();
            //else
            //No need for singleton behavior because for mock.Verify to work correctly you need to recreate mock
            Setup();
            return mock;
        }

        public static void Setup()
        {
            mock = new Mock<IRepository>();
            departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            positions = new List<Position>();
            AddPosition(new Position { PositionID = 1, TitleEn = "Employee", TitleUk = "Працівник", Employees = new List<Employee>() });
            AddPosition(new Position { PositionID = 2, TitleEn = "Software developer", TitleUk = "Розробник програмного забезпечення", Employees = new List<Employee>() });
            AddPosition(new Position { PositionID = 3, TitleEn = "Director", TitleUk = "Директор", Employees = new List<Employee>() });
            AddPosition(new Position { PositionID = 4, TitleEn = "Manager", TitleUk = "Лайн-менеджер", Employees = new List<Employee>() });

            countries = new List<Country>
            {
                 new Country { CountryID = 1, CountryName = "Ukraine", Holidays = new List<Holiday>(), Locations = new List<Location>()},
                 new Country { CountryID = 2, CountryName = "Poland", Holidays = new List<Holiday>(), Locations = new List<Location>()},
                 new Country { CountryID = 3, CountryName = "Sweden", Holidays = new List<Holiday>(), Locations = new List<Location>()},
                 new Country { CountryID = 4, CountryName = "Belarus", Holidays = new List<Holiday>(), Locations = new List<Location>()},
                 new Country { CountryID = 5, CountryName = "Zimbabve", Holidays = new List<Holiday>(), Locations = new List<Location>()}
            };

            holidays = new List<Holiday>();
            {
                //Ukraine 2013
                AddHoliday(new Holiday { HolidayID = 1, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 2, Title = "Christmas", HolidayDate = new DateTime(2013, 01, 07), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 3, Title = "Woman's day", HolidayDate = new DateTime(2013, 03, 08), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 4, Title = "Easter day", HolidayDate = new DateTime(2013, 05, 05), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 5, Title = "May 1", HolidayDate = new DateTime(2013, 05, 01), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 6, Title = "May 2", HolidayDate = new DateTime(2013, 05, 02), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 7, Title = "Victory Day", HolidayDate = new DateTime(2013, 05, 09), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 8, Title = "Green Holiday", HolidayDate = new DateTime(2013, 06, 23), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 9, Title = "Constitution Day", HolidayDate = new DateTime(2013, 06, 28), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 10, Title = "Independence Day", HolidayDate = new DateTime(2013, 08, 24), CountryID = 1 });
                //Poland 2013
                AddHoliday(new Holiday { HolidayID = 11, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 2 });
                AddHoliday(new Holiday { HolidayID = 12, Title = "Epiphany", HolidayDate = new DateTime(2013, 01, 06), CountryID = 2 });
                AddHoliday(new Holiday { HolidayID = 13, Title = "Easter Monday", HolidayDate = new DateTime(2013, 04, 21), CountryID = 2 });
                //Sweden 201
                AddHoliday(new Holiday { HolidayID = 14, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 3 });
                AddHoliday(new Holiday { HolidayID = 15, Title = "Epiphany", HolidayDate = new DateTime(2013, 01, 06), CountryID = 3 });
                AddHoliday(new Holiday { HolidayID = 16, Title = "Easter Monday", HolidayDate = new DateTime(2013, 04, 21), CountryID = 3 });
                //Belarus 2013
                AddHoliday(new Holiday { HolidayID = 17, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 4 });
                AddHoliday(new Holiday { HolidayID = 18, Title = "Christmas", HolidayDate = new DateTime(2013, 01, 07), CountryID = 4 });
                AddHoliday(new Holiday { HolidayID = 19, Title = "Woman's day", HolidayDate = new DateTime(2013, 03, 08), CountryID = 4 });

                //Ukraine 2014
                AddHoliday(new Holiday { HolidayID = 20, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 21, Title = "Christmas", HolidayDate = new DateTime(2014, 01, 07), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 22, Title = "Woman's day", HolidayDate = new DateTime(2014, 03, 08), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 23, Title = "Easter day", HolidayDate = new DateTime(2014, 04, 20), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 24, Title = "May 1", HolidayDate = new DateTime(2014, 05, 01), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 25, Title = "May 2", HolidayDate = new DateTime(2014, 05, 02), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 26, Title = "Victory Day", HolidayDate = new DateTime(2014, 05, 09), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 27, Title = "Green Holiday", HolidayDate = new DateTime(2014, 06, 08), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 28, Title = "Constitution Day", HolidayDate = new DateTime(2014, 06, 28), CountryID = 1 });
                AddHoliday(new Holiday { HolidayID = 29, Title = "Independence Day", HolidayDate = new DateTime(2014, 08, 24), CountryID = 1 });
                //Poland 2014
                AddHoliday(new Holiday { HolidayID = 31, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 2 });
                AddHoliday(new Holiday { HolidayID = 32, Title = "Epiphany", HolidayDate = new DateTime(2014, 01, 06), CountryID = 2 });
                AddHoliday(new Holiday { HolidayID = 33, Title = "Easter Monday", HolidayDate = new DateTime(2014, 04, 21), CountryID = 2 });
                //Sweden 201
                AddHoliday(new Holiday { HolidayID = 34, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 3 });
                AddHoliday(new Holiday { HolidayID = 35, Title = "Epiphany", HolidayDate = new DateTime(2014, 01, 06), CountryID = 3 });
                AddHoliday(new Holiday { HolidayID = 36, Title = "Easter Monday", HolidayDate = new DateTime(2014, 04, 21), CountryID = 3 });
                //Belarus 2014
                AddHoliday(new Holiday { HolidayID = 37, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 4 });
                AddHoliday(new Holiday { HolidayID = 38, Title = "Christmas", HolidayDate = new DateTime(2014, 01, 07), CountryID = 4 });
                AddHoliday(new Holiday { HolidayID = 39, Title = "Woman's day", HolidayDate = new DateTime(2014, 03, 08), CountryID = 4 });

            }



            employees = new List<Employee>();

            AddEmployee(new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, PositionID = 2, EID = "andl", DateDismissed = new DateTime(2013, 11, 01), DateEmployed = new DateTime(2011, 11, 01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), FullNameUk = "Джонні Роус Олександрович", IsGreetingMessageAllow = true, EMail = "Anl@E-mail.ua", EducationAcquiredDate = DateTime.Now, EducationAcquiredType = EducationType.BasicSecondary, EducationInProgressDate = DateTime.Now.AddYears(5), EducationInProgressType = EducationType.CompleteHigher });
            AddEmployee(new Employee { EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, PositionID = 2, EID = "ascr", DateEmployed = new DateTime(2013, 04, 11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, PositionID = 2, EID = "tedk", DateEmployed = new DateTime(2013, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true, EMail = "abc" });
            AddEmployee(new Employee { EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4, PositionID = 2, EID = "tadk", DateEmployed = new DateTime(2012, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, PositionID = 2, EID = "daol", DateEmployed = new DateTime(2013, 07, 21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, PositionID = 2, EID = "tebl", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, PositionID = 4, EID = "xtwe", DateEmployed = new DateTime(2012, 09, 04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, PositionID = 3, EID = "xomi", DateEmployed = new DateTime(11 / 02 / 2011), IsManager = true, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 9, FirstName = "Natalia", LastName = "Zamorrison", DepartmentID = 5, PositionID = 3, EID = "namo", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 10, FirstName = "Natalia", LastName = "Talee", DepartmentID = 5, PositionID = 3, EID = "tale", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 11, FirstName = "Oleg", LastName = "Talee", DepartmentID = 5, PositionID = 3, EID = "taee", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 12, FirstName = "Igor", LastName = "Woody", DepartmentID = 5, PositionID = 3, EID = "igwo", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 13, FirstName = "Igor", LastName = "Wooody", DepartmentID = 5, PositionID = 3, EID = "iwou", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 14, FirstName = "Igor", LastName = "Wooody", DepartmentID = 5, PositionID = 3, EID = "iwoo", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 15, FirstName = "Igor", LastName = "Woooody", DepartmentID = 5, PositionID = 3, EID = "iwooo", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 16, FirstName = "Ivan", LastName = "Petriv", DepartmentID = 5, PositionID = 3, EID = "iwpe", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 17, FirstName = "Chuck", LastName = "Petrenko", DepartmentID = 1, PositionID = 3, EID = "chap", DateEmployed = new DateTime(2010, 04, 10), IsManager = false, BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 18, FirstName = "Ivan", LastName = "Test", DepartmentID = 1, PositionID = 3, EID = "ivte", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 19, FirstName = "Sick", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "siol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 20, FirstName = "PaidVac", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "pvol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 21, FirstName = "UPaidVac", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "uvol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 22, FirstName = "Overtime", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "ovol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 23, FirstName = "PaidOvertime", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "povol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 24, FirstName = "PrvOvertime", LastName = "Only", DepartmentID = 5, PositionID = 3, EID = "prvol", DateEmployed = new DateTime(), BusinessTrips = new List<BusinessTrip>(), CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true });
            AddEmployee(new Employee { EmployeeID = 25, FirstName = "User", LastName = "User", EID = "User", IsUserOnly = true, IsGreetingMessageAllow = true, DepartmentID = 5 });

            //AddEmployee(new Employee { EmployeeID = 17, FirstName = "Ivan", LastName = "Petriv", DepartmentID = 5, EID = "iwpe", DateEmployed = new DateTime(2011, 04, 11), IsManager = false, BusinessTrips = new List<BusinessTrip>() });

            employees.Find(b => b.EmployeeID == 1).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 2).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 3).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 4).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 5).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 6).Position = (positions.Find(l => l.PositionID == 2));
            employees.Find(b => b.EmployeeID == 7).Position = (positions.Find(l => l.PositionID == 4));
            employees.Find(b => b.EmployeeID == 8).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 9).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 10).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 11).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 12).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 13).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 14).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 15).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 16).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 17).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 18).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 19).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 20).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 21).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 22).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 23).Position = (positions.Find(l => l.PositionID == 3));
            employees.Find(b => b.EmployeeID == 24).Position = (positions.Find(l => l.PositionID == 3));

            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 1));
            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 2));
            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 3));
            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 4));
            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 5));
            positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 6));
            positions.Find(l => l.PositionID == 4).Employees.Add(employees.Find(e => e.EmployeeID == 7));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 8));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 9));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 10));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 11));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 12));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 13));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 14));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 15));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 16));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 17));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 18));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 19));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 20));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 21));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 22));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 24));
            positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 23));

             locations = new List<Location>();
             { 
                AddLocation(new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 1 });
                AddLocation(new Location { LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 1 });
                AddLocation(new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 });
             };

             units = new List<Unit>();
             {
                 AddUnit(new Unit { UnitID = 1, Title = "Unknown", ShortTitle = "Unknown", BusinessTrips = new List<BusinessTrip>() });
                 AddUnit(new Unit { UnitID = 2, Title = "Business Development Unit", ShortTitle = "BD", BusinessTrips = new List<BusinessTrip>() });
                 AddUnit(new Unit { UnitID = 3, Title = "EPUA Board", ShortTitle = "EPUA_B", BusinessTrips = new List<BusinessTrip>() });
                 AddUnit(new Unit { UnitID = 4, Title = "EPOL Board", ShortTitle = "B", BusinessTrips = new List<BusinessTrip>() });
                 AddUnit(new Unit { UnitID = 5, Title = "Finance Unit", ShortTitle = "F", BusinessTrips = new List<BusinessTrip>() });
             }
             businessTrips = new List<BusinessTrip>();

             AddBusinessTrip(new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(1), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true, UnitID = 1, Journeys = new List<Journey>(), });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), OrderStartDate = new DateTime(2014, 10, 19), OrderEndDate = new DateTime(2014, 10, 31), DaysInBtForOrder = 13, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, UnitID = 1, Habitation = "lodz", HabitationConfirmed = true, RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 8, 40 }, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014, 10, 01), EndDate = new DateTime(2014, 10, 12), OrderStartDate = new DateTime(2014, 09, 30), OrderEndDate = new DateTime(2014, 10, 13), DaysInBtForOrder = 14, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, UnitID = 1, LocationID = 2, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013, 10, 01), EndDate = new DateTime(2013, 10, 20), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = new DateTime(2013, 10, 21), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 14, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013, 08, 01), EndDate = new DateTime(2013, 08, 20), OrderStartDate = new DateTime(2013, 07, 31), OrderEndDate = new DateTime(2013, 08, 21), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 4, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 20), OrderStartDate = new DateTime(2013, 08, 31), OrderEndDate = new DateTime(2014, 10, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 4, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 20), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 10, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 4, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 25), OrderStartDate = new DateTime(2014, 08, 31), OrderEndDate = new DateTime(2013, 09, 26), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 5, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 02), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 03), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned, EmployeeID = 6, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 11, StartDate = new DateTime(2014, 12, 03), EndDate = new DateTime(2014, 12, 13), OrderStartDate = new DateTime(2014, 12, 02), OrderEndDate = new DateTime(2014, 12, 05), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned, EmployeeID = 7, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 12, StartDate = new DateTime(2014, 11, 06), EndDate = new DateTime(2014, 11, 06), OrderStartDate = new DateTime(2014, 12, 05), OrderEndDate = new DateTime(2014, 12, 08), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 13, StartDate = new DateTime(2014, 12, 09), EndDate = new DateTime(2014, 12, 10), OrderStartDate = new DateTime(2014, 12, 08), OrderEndDate = new DateTime(2014, 12, 11), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee reg + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 14, StartDate = DateTime.Now.AddDays(2), EndDate = DateTime.Now.AddDays(3), OrderStartDate = DateTime.Now.AddDays(1), OrderEndDate = DateTime.Now.AddDays(4), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee conf + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 15, StartDate = new DateTime(2014, 12, 15), EndDate = new DateTime(2014, 12, 16), OrderStartDate = new DateTime(2014, 12, 14), OrderEndDate = new DateTime(2014, 12, 17), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", AccComment = "Test Comment", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 16, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(5), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), DaysInBtForOrder = ((new DateTime(2014, 12, 27)).Date - (new DateTime(2014, 11, 30)).Date).Days + 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), UnitID = 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 14, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 17, StartDate = new DateTime(2014, 12, 18), EndDate = new DateTime(2014, 12, 19), OrderStartDate = new DateTime(2014, 12, 19), OrderEndDate = new DateTime(2014, 12, 20), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 7, LocationID = 1, Comment = "7 employee reg and cancelled", UnitID = 1, Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 18, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 25), OrderStartDate = new DateTime(2013, 08, 31), OrderEndDate = new DateTime(2013, 09, 26), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and cancelled", UnitID = 1, Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 19, StartDate = new DateTime(2014, 09, 01), EndDate = new DateTime(2014, 09, 27), OrderStartDate = new DateTime(2014, 08, 31), OrderEndDate = new DateTime(2014, 09, 28), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, UnitID = 1, Comment = "4 employee confirmed and modified and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 20, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 27), OrderStartDate = new DateTime(2013, 08, 31), OrderEndDate = new DateTime(2013, 09, 28), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 2, LocationID = 1, UnitID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting", RejectComment = "visa expired", OldLocationID = 2, OldLocationTitle = "LDL", Flights = "Kyiv - Krakow", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 21, StartDate = new DateTime(2013, 12, 25), EndDate = new DateTime(2014, 01, 25), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = new DateTime(2014, 01, 26), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), DaysInBtForOrder = ((DateTime.Now.ToLocalTimeAzure().Date.AddDays(2)).Date - (new DateTime(2013, 09, 30)).Date).Days + 1, UnitID = 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 14, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 23, StartDate = new DateTime(2013, 10, 01), EndDate = new DateTime(2013, 10, 03), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = new DateTime(2013, 10, 04), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>(), AccComment = "Test Comment" });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 24, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", UnitID = 1, CancelComment = "visa expired", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2013, 10, 01), EndDate = new DateTime(2013, 10, 03), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = new DateTime(2013, 10, 04), DaysInBtForOrder = ((new DateTime(2013, 10, 04)).Date - (new DateTime(2013, 09, 30)).Date).Days + 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), UnitID = 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 26, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2013, 10, 13), OrderStartDate = new DateTime(2013, 10, 09), OrderEndDate = new DateTime(2013, 10, 14), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 27, StartDate = new DateTime(2013, 10, 10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", UnitID = 1, Journeys = new List<Journey>(), AccComment = "Test Comment" });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 28, StartDate = new DateTime(2014, 12, 21), EndDate = new DateTime(2014, 12, 22), OrderStartDate = new DateTime(2014, 12, 20), OrderEndDate = new DateTime(2014, 12, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true, RejectComment = "visa expired", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 29, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), UnitID = 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 30, StartDate = new DateTime(2014, 12, 23), EndDate = new DateTime(2014, 12, 25), OrderStartDate = new DateTime(2014, 12, 23), OrderEndDate = new DateTime(2014, 12, 26), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Cancelled, EmployeeID = 7, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 31, StartDate = new DateTime(2012, 04, 22), EndDate = new DateTime(2012, 07, 22), OrderStartDate = new DateTime(2012, 04, 21), OrderEndDate = new DateTime(2012, 07, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 3, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>(), AccComment = "" });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 32, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 10, 25), OrderStartDate = new DateTime(2013, 08, 31), OrderEndDate = new DateTime(2013, 10, 26), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>(), AccComment = "ВКО №  , від   , Сума:  , UAH.\nВКО №  , від   , Сума:  , USD." });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 33, StartDate = new DateTime(2013, 10, 01), EndDate = new DateTime(2013, 10, 05), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 34, StartDate = new DateTime(2012, 12, 01), EndDate = new DateTime(2013, 03, 01), OrderStartDate = new DateTime(2013, 02, 28), OrderEndDate = new DateTime(2014, 03, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 8, LocationID = 2, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 35, StartDate = new DateTime(2013, 03, 01), EndDate = new DateTime(2013, 03, 22), OrderStartDate = new DateTime(2013, 02, 28), OrderEndDate = new DateTime(2014, 03, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 8, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 36, StartDate = new DateTime(2013, 12, 01), EndDate = new DateTime(2013, 12, 20), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2014, 12, 22), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 3, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 37, StartDate = new DateTime(2014, 03, 01), EndDate = new DateTime(2014, 03, 22), OrderStartDate = new DateTime(2013, 11, 30), OrderEndDate = new DateTime(2014, 03, 23), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 3, LocationID = 1, UnitID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting", Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 38, StartDate = new DateTime(2014, 01, 22), EndDate = new DateTime(2014, 02, 22), OrderStartDate = DateTime.Now, OrderEndDate = DateTime.Now.Date.AddDays(-3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014, 01, 22), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 17, LocationID = 1, UnitID = 1, Journeys = new List<Journey>() });
             AddBusinessTrip(new BusinessTrip { BusinessTripID = 39, StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2013, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 16, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true, UnitID = 1, Journeys = new List<Journey>(), });


            journeys = new List<Journey>();

            AddJourney(new Journey { JourneyID = 1, BusinessTripID = 1, Date = new DateTime(2014, 11, 30), DayOff = true, ReclaimDate = new DateTime(2014, 02, 27) });
            AddJourney(new Journey { JourneyID = 2, BusinessTripID = 1, Date = new DateTime(2014, 12, 11), DayOff = false });
            //AddJourney(new Journey { JourneyID = 3, BusinessTripID = 2, Date = new DateTime(2014, 09, 30), DayOff = false });
            //AddJourney(new Journey { JourneyID = 4, BusinessTripID = 2, Date = new DateTime(2014, 11, 11), DayOff = false});
            AddJourney(new Journey { JourneyID = 5, BusinessTripID = 3, Date = new DateTime(2014, 10, 19), DayOff = true });
            AddJourney(new Journey { JourneyID = 6, BusinessTripID = 3, Date = new DateTime(2014, 10, 31), DayOff = false });

            AddJourney(new Journey { JourneyID = 7, BusinessTripID = 4, Date = new DateTime(2014, 09, 30), DayOff = false });
            AddJourney(new Journey { JourneyID = 8, BusinessTripID = 4, Date = new DateTime(2014, 10, 13), DayOff = false });
            AddJourney(new Journey { JourneyID = 9, BusinessTripID = 5, Date = new DateTime(2013, 09, 30), DayOff = false });
            AddJourney(new Journey { JourneyID = 10, BusinessTripID = 5, Date = new DateTime(2013, 10, 21), DayOff = false });
            AddJourney(new Journey { JourneyID = 11, BusinessTripID = 6, Date = new DateTime(2013, 07, 31), DayOff = false, ReclaimDate = new DateTime(2014, 02, 28) });
            AddJourney(new Journey { JourneyID = 12, BusinessTripID = 6, Date = new DateTime(2013, 08, 21), DayOff = false, ReclaimDate = new DateTime(2014, 02, 26) });
            AddJourney(new Journey { JourneyID = 13, BusinessTripID = 7, Date = new DateTime(2013, 08, 31), DayOff = true });
            AddJourney(new Journey { JourneyID = 14, BusinessTripID = 7, Date = new DateTime(2013, 10, 11), DayOff = false });
            AddJourney(new Journey { JourneyID = 15, BusinessTripID = 8, Date = new DateTime(2014, 11, 30), DayOff = true });
            AddJourney(new Journey { JourneyID = 16, BusinessTripID = 8, Date = new DateTime(2014, 10, 11), DayOff = true });
            AddJourney(new Journey { JourneyID = 17, BusinessTripID = 8, Date = new DateTime(2014, 10, 11), DayOff = true, ReclaimDate = new DateTime(2014, 10, 10) });

            visaRegistrationDates = new List<VisaRegistrationDate>();

            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 1, RegistrationDate = new DateTime(2013, 01, 01), VisaType = "D08", City = "Kyiv", RegistrationNumber = "1111", RegistrationTime = "09:00" });
            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 5, RegistrationDate = new DateTime(2013, 10, 02), VisaType = "C07", City = "Kyiv", RegistrationNumber = "5555", RegistrationTime = "15:00" });
            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 3, RegistrationDate = new DateTime(2013, 01, 01), VisaType = "C07", City = "Kyiv", RegistrationNumber = "9999", RegistrationTime = "00:00" });
            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 4, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" });
            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 6, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" });
            AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 13, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" });
           //AddVisaRegistrationDate(new VisaRegistrationDate { EmployeeID = 17, RegistrationDate = new DateTime(2013, 01, 04), VisaType = "D08" });


            calendarItems = new List<CalendarItem>();

            AddCalendarItem(new CalendarItem { CalendarItemID = 2, From = new DateTime(2014, 01, 25), To = new DateTime(2014, 02, 05), Type = CalendarItemType.PaidVacation, EmployeeID = 1 });
            AddCalendarItem(new CalendarItem { CalendarItemID = 3, From = new DateTime(2014, 01, 01), To = new DateTime(2014, 01, 01), Type = CalendarItemType.OvertimeForReclaim, EmployeeID = 1 });
            AddCalendarItem(new CalendarItem { CalendarItemID = 4, From = new DateTime(2014, 02, 01), To = new DateTime(2014, 02, 14), Type = CalendarItemType.BT, EmployeeID = 2, Location = "KR/B" });
            AddCalendarItem(new CalendarItem { CalendarItemID = 5, From = new DateTime(2014, 03, 01), To = new DateTime(2014, 03, 14), Type = CalendarItemType.UnpaidVacation, EmployeeID = 2 });
            AddCalendarItem(new CalendarItem { CalendarItemID = 7, From = new DateTime(2014, 05, 09), To = new DateTime(2014, 05, 09), Type = CalendarItemType.ReclaimedOvertime, EmployeeID = 5 });
            AddCalendarItem(new CalendarItem { CalendarItemID = 8, From = new DateTime(2014, 05, 09), To = new DateTime(2014, 06, 09), Type = CalendarItemType.BT, EmployeeID = 2, Location = "LD/D" });

            AddCalendarItem(new CalendarItem { CalendarItemID = 9, EmployeeID = 1, From = new DateTime(2013, 01, 01), To = new DateTime(2013, 01, 01), Type = CalendarItemType.ReclaimedOvertime });
            AddCalendarItem(new CalendarItem { CalendarItemID = 10, EmployeeID = 3, From = new DateTime(2013, 01, 03), To = new DateTime(2013, 01, 03), Type = CalendarItemType.OvertimeForReclaim });
            AddCalendarItem(new CalendarItem { CalendarItemID = 11, EmployeeID = 2, From = new DateTime(2013, 01, 02), To = new DateTime(2013, 01, 02), Type = CalendarItemType.PrivateMinus });
            AddCalendarItem(new CalendarItem { CalendarItemID = 12, EmployeeID = 11, From = new DateTime(2013, 02, 27), To = new DateTime(2013, 02, 27), Type = CalendarItemType.ReclaimedOvertime });
            AddCalendarItem(new CalendarItem { CalendarItemID = 13, EmployeeID = 8, From = new DateTime(2013, 02, 28), To = new DateTime(2013, 02, 28), Type = CalendarItemType.OvertimeForReclaim });
            AddCalendarItem(new CalendarItem { CalendarItemID = 14, EmployeeID = 13, From = new DateTime(2013, 02, 22), To = new DateTime(2013, 02, 22), Type = CalendarItemType.PrivateMinus });
            AddCalendarItem(new CalendarItem { CalendarItemID = 15, EmployeeID = 1, From = new DateTime(2014, 02, 12), To = new DateTime(2014, 02, 28), Type = CalendarItemType.PaidVacation });
            AddCalendarItem(new CalendarItem { CalendarItemID = 16, EmployeeID = 2, From = new DateTime(2014, 03, 12), To = new DateTime(2014, 03, 28), Type = CalendarItemType.UnpaidVacation });
            AddCalendarItem(new CalendarItem { CalendarItemID = 17, EmployeeID = 5, From = new DateTime(2014, 06, 12), To = new DateTime(2014, 06, 28), Type = CalendarItemType.PaidVacation });
            AddCalendarItem(new CalendarItem { CalendarItemID = 18, EmployeeID = 6, From = new DateTime(2014, 07, 12), To = new DateTime(2014, 07, 28), Type = CalendarItemType.UnpaidVacation });
            AddCalendarItem(new CalendarItem { CalendarItemID = 19, EmployeeID = 4, From = new DateTime(2014, 04, 24), To = new DateTime(2014, 04, 28), Type = CalendarItemType.SickAbsence });
            AddCalendarItem(new CalendarItem { CalendarItemID = 1, EmployeeID = 1, From = new DateTime(2014, 02, 21), To = new DateTime(2014, 02, 27), Type = CalendarItemType.SickAbsence });
            AddCalendarItem(new CalendarItem { CalendarItemID = 6, EmployeeID = 2, From = new DateTime(2014, 02, 21), To = new DateTime(2014, 03, 27), Type = CalendarItemType.SickAbsence });
            AddCalendarItem(new CalendarItem { CalendarItemID = 20, EmployeeID = 3, From = new DateTime(2014, 02, 21), To = new DateTime(2014, 02, 21), Type = CalendarItemType.Journey });
            AddCalendarItem(new CalendarItem { CalendarItemID = 21, EmployeeID = 1, Location = "KR/B", From = new DateTime(2013, 01, 01), To = new DateTime(2013, 01, 01), Type = CalendarItemType.BT });
            AddCalendarItem(new CalendarItem { CalendarItemID = 22, EmployeeID = 1, Location = "KR/B", From = new DateTime(2014, 01, 01), To = new DateTime(2014, 01, 01), Type = CalendarItemType.BT });
            AddCalendarItem(new CalendarItem { CalendarItemID = 23, EmployeeID = 1, Location = "KR/B", From = new DateTime(2012, 01, 01), To = new DateTime(2012, 01, 01), Type = CalendarItemType.BT });
            AddCalendarItem(new CalendarItem { CalendarItemID = 60, EmployeeID = 2, From = new DateTime(2014, 03, 11), To = new DateTime(2014, 03, 27), Type = CalendarItemType.SickAbsence });
            visas = new List<Visa>();
            //AddVisa(new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2012, 08, 01), DueDate = new DateTime(2013, 11, 02), Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 });
            //AddVisa(new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012, 02, 13), DueDate = new DateTime(2013, 05, 13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 });
            AddVisa(new Visa { EmployeeID = 3, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 20, DaysUsedInBT = 19, DaysUsedInPrivateTrips = 2, Entries = 5, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 3), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 4, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-180), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(186), Days = 20, DaysUsedInBT = 10, DaysUsedInPrivateTrips = 11, Entries = 2, EntriesUsedInBT = 1, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 4), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 5, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 20, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 12, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 2, VisaOf = employees.Find(e => e.EmployeeID == 5), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 6, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 40, DaysUsedInBT = 2, DaysUsedInPrivateTrips = 11, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 6), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 7, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-10), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(100), Days = 20, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 2, Entries = 2, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 7), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 8, VisaType = "C07", StartDate = new DateTime(2012, 02, 13), DueDate = new DateTime(2013, 05, 13), Days = 20, DaysUsedInBT = 5, Entries = 0, EntriesUsedInBT = 4, VisaOf = employees.Find(e => e.EmployeeID == 8), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 9, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 0, VisaOf = employees.Find(e => e.EmployeeID == 9), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 11, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 20, DaysUsedInBT = 5, Entries = 0, EntriesUsedInBT = 4, VisaOf = employees.Find(e => e.EmployeeID == 11), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 12, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 2, Entries = 0, EntriesUsedInBT = 4, VisaOf = employees.Find(e => e.EmployeeID == 12), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 13, VisaType = "C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-90), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 20, DaysUsedInBT = 5, Entries = 0, EntriesUsedInBT = 4, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 13), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 1, VisaType = "D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 20, DaysUsedInPrivateTrips = 21, Entries = 0, EntriesUsedInBT = 5, EntriesUsedInPrivateTrips = 1, VisaOf = employees.Find(e => e.EmployeeID == 1), PrivateTrips = new List<PrivateTrip>() });
            AddVisa(new Visa { EmployeeID = 2, VisaType = "D07", StartDate = DateTime.Now.ToLocalTimeAzure(), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(365), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, VisaOf = employees.Find(e => e.EmployeeID == 2), PrivateTrips = new List<PrivateTrip>() });

            privateTrips = new List<PrivateTrip>();

            AddPrivateTrip(new PrivateTrip { PrivateTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-180), EmployeeID = 1, PrivateTripOf = visas.Find(e => e.EmployeeID == 1) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-16), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-15), EmployeeID = 3, PrivateTripOf = visas.Find(e => e.EmployeeID == 3) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 3, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EmployeeID = 4, PrivateTripOf = visas.Find(e => e.EmployeeID == 4) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 4, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-45), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EmployeeID = 5, PrivateTripOf = visas.Find(e => e.EmployeeID == 5) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-15), EmployeeID = 5, PrivateTripOf = visas.Find(e => e.EmployeeID == 5) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 6, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-30), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-20), EmployeeID = 6, PrivateTripOf = visas.Find(e => e.EmployeeID == 6) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 7, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 7, PrivateTripOf = visas.Find(e => e.EmployeeID == 7) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 10, PrivateTripOf = visas.Find(e => e.EmployeeID == 10) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 9, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 11, PrivateTripOf = visas.Find(e => e.EmployeeID == 11) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 10, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 12, PrivateTripOf = visas.Find(e => e.EmployeeID == 12) });
            AddPrivateTrip(new PrivateTrip { PrivateTripID = 11, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 13, PrivateTripOf = visas.Find(e => e.EmployeeID == 13) });


            permits = new List<Permit>();
            AddPermit(new Permit { EmployeeID = 1, Number = "04/2012", StartDate = new DateTime(2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1) });
            AddPermit(new Permit { EmployeeID = 2, Number = "01/2012", IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 2) });
            AddPermit(new Permit { EmployeeID = 3, Number = "01/2013", IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 3) });
            AddPermit(new Permit { EmployeeID = 5, Number = "01/2013", StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 5), CancelRequestDate = DateTime.Now.ToLocalTimeAzure() });
            AddPermit(new Permit { EmployeeID = 14, Number = "01/2013", StartDate = new DateTime(2013, 01, 01), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(101), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 14), CancelRequestDate = DateTime.Now.ToLocalTimeAzure() });
         //   AddPermit(new Permit { EmployeeID = 17, Number = "01/2013",  IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 17), CancelRequestDate = DateTime.Now.ToLocalTimeAzure() });

            passports = new List<Passport>();

            AddPassport(new Passport { EmployeeID = 1 });
         //   AddPassport(new Passport { EmployeeID = 3 });
            AddPassport(new Passport { EmployeeID = 4 });
            AddPassport(new Passport { EmployeeID = 5 });
            AddPassport(new Passport { EmployeeID = 6 });
            AddPassport(new Passport { EmployeeID = 7 });
            AddPassport(new Passport { EmployeeID = 16 });

            overtimes = new List<Overtime>();
            //AddOvertime(new Overtime { OvertimeID = 1, EmployeeID = 1, Date = new DateTime(2013, 01, 01), ReclaimDate = new DateTime(2013, 02, 02), DayOff = true, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 2, EmployeeID = 2, Date = new DateTime(2013, 01, 02), DayOff = false, Type = OvertimeType.Private });
            AddOvertime(new Overtime { OvertimeID = 3, EmployeeID = 3, Date = new DateTime(2013, 01, 03), ReclaimDate = new DateTime(2013, 03, 02), DayOff = true, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 4, EmployeeID = 4, Date = new DateTime(2013, 01, 04), ReclaimDate = new DateTime(2013, 04, 02), DayOff = true, Type = OvertimeType.Paid });
            //AddOvertime(new Overtime { OvertimeID = 5, EmployeeID = 5, Date = new DateTime(2013, 01, 05), ReclaimDate = new DateTime(2013, 05, 02), DayOff = true, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 6, EmployeeID = 6, Date = new DateTime(2013, 01, 14), ReclaimDate = new DateTime(2013, 06, 02), DayOff = true, Type = OvertimeType.Private });
            AddOvertime(new Overtime { OvertimeID = 7, EmployeeID = 7, Date = new DateTime(2013, 01, 13), ReclaimDate = new DateTime(2013, 07, 02), DayOff = true, Type = OvertimeType.Private });
            //AddOvertime(new Overtime { OvertimeID = 8, EmployeeID = 8, Date = new DateTime(2013, 01, 12), ReclaimDate = new DateTime(2013, 08, 02), DayOff = true, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 9, EmployeeID = 9, Date = new DateTime(2013, 01, 11), ReclaimDate = new DateTime(2013, 09, 02), DayOff = true, Type = OvertimeType.Paid });
            //AddOvertime(new Overtime { OvertimeID = 10, EmployeeID = 10, Date = new DateTime(2013, 01, 10), DayOff = false, Type = OvertimeType.Overtime });

            //AddOvertime(new Overtime { OvertimeID = 11, EmployeeID = 11, Date = new DateTime(2013, 02, 11), DayOff = false, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 12, EmployeeID = 12, Date = new DateTime(2013, 02, 21), ReclaimDate = new DateTime(2013, 02, 02), DayOff = true, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 13, EmployeeID = 13, Date = new DateTime(2013, 02, 22), DayOff = false, Type = OvertimeType.Private });
            //AddOvertime(new Overtime { OvertimeID = 14, EmployeeID = 14, Date = new DateTime(2013, 02, 14), DayOff = false, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 15, EmployeeID = 15, Date = new DateTime(2013, 02, 15), DayOff = false, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 16, EmployeeID = 16, Date = new DateTime(2013, 02, 16), DayOff = false, Type = OvertimeType.Private });
            //AddOvertime(new Overtime { OvertimeID = 17, EmployeeID = 17, Date = new DateTime(2013, 02, 17), DayOff = false, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 18, EmployeeID = 8, Date = new DateTime(2013, 02, 28), DayOff = false, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 19, EmployeeID = 1, Date = new DateTime(2013, 02, 28), DayOff = false, Type = OvertimeType.Private });
            //AddOvertime(new Overtime { OvertimeID = 20, EmployeeID = 11, Date = new DateTime(2013, 02, 27), ReclaimDate = new DateTime(2013, 02, 02), DayOff = true, Type = OvertimeType.Overtime });
            //AddOvertime(new Overtime { OvertimeID = 21, EmployeeID = 22, Date = new DateTime(2015, 02, 27), ReclaimDate = new DateTime(2015, 03, 02), DayOff = true, Type = OvertimeType.Overtime });
            AddOvertime(new Overtime { OvertimeID = 22, EmployeeID = 23, Date = new DateTime(2013, 07, 30), ReclaimDate = new DateTime(2013, 08, 04), DayOff = true, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 23, EmployeeID = 24, Date = new DateTime(2013, 06, 07), ReclaimDate = new DateTime(2013, 06, 12), DayOff = true, Type = OvertimeType.Private });
            AddOvertime(new Overtime { OvertimeID = 24, EmployeeID = 1, Date = new DateTime(2013, 06, 17), DayOff = true, Type = OvertimeType.Paid });
            AddOvertime(new Overtime { OvertimeID = 25, EmployeeID = 1, Date = new DateTime(2014, 06, 17), ReclaimDate = new DateTime(2014, 06, 22), DayOff = true, Type = OvertimeType.Private });
            AddOvertime(new Overtime { OvertimeID = 26, EmployeeID = 1, Date = new DateTime(2013, 01, 01), ReclaimDate = new DateTime(2013, 02, 02), DayOff = true, Type = OvertimeType.Paid });


            vacations = new List<Vacation>();
            AddVacation(new Vacation { VacationID = 1, EmployeeID = 1, From = new DateTime(2014, 02, 12), To = new DateTime(2014, 02, 28), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 2, EmployeeID = 2, From = new DateTime(2014, 03, 12), To = new DateTime(2014, 03, 28), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 3, EmployeeID = 3, From = new DateTime(2014, 04, 12), To = new DateTime(2014, 04, 28), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 4, EmployeeID = 4, From = new DateTime(2014, 05, 12), To = new DateTime(2014, 05, 28), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 5, EmployeeID = 5, From = new DateTime(2014, 06, 12), To = new DateTime(2014, 06, 28), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 6, EmployeeID = 6, From = new DateTime(2014, 07, 12), To = new DateTime(2014, 07, 28), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 7, EmployeeID = 7, From = new DateTime(2014, 08, 12), To = new DateTime(2014, 08, 28), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 8, EmployeeID = 8, From = new DateTime(2014, 09, 12), To = new DateTime(2014, 09, 28), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 9, EmployeeID = 9, From = new DateTime(2014, 10, 12), To = new DateTime(2014, 10, 28), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 10, EmployeeID = 10, From = new DateTime(2014, 11, 12), To = new DateTime(2014, 11, 28), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 11, EmployeeID = 20, From = new DateTime(2014, 03, 27), To = new DateTime(2014, 03, 30), Type = VacationType.PaidVacation });
            AddVacation(new Vacation { VacationID = 12, EmployeeID = 21, From = new DateTime(2014, 04, 27), To = new DateTime(2014, 04, 30), Type = VacationType.UnpaidVacation });
            AddVacation(new Vacation { VacationID = 13, EmployeeID = 1, From = new DateTime(2014, 04, 28), To = new DateTime(2014, 04, 30), Type = VacationType.UnpaidVacation });



            sicks = new List<Sickness>();
            AddSickItem(new Sickness { SickID = 1, EmployeeID = 1, From = new DateTime(2014, 02, 21), To = new DateTime(2014, 02, 27), SicknessType = "GRZ" });
            AddSickItem(new Sickness { SickID = 2, EmployeeID = 1, From = new DateTime(2014, 03, 21), To = new DateTime(2014, 03, 27), SicknessType = "GRZ" });
            AddSickItem(new Sickness { SickID = 3, EmployeeID = 2, From = new DateTime(2014, 02, 21), To = new DateTime(2014, 02, 27), SicknessType = "" });
            AddSickItem(new Sickness { SickID = 4, EmployeeID = 2, From = new DateTime(2014, 03, 11), To = new DateTime(2014, 03, 27), SicknessType = "GRZ" });
            AddSickItem(new Sickness { SickID = 5, EmployeeID = 3, From = new DateTime(2014, 03, 24), To = new DateTime(2014, 03, 28), SicknessType = "GRZ" });
            AddSickItem(new Sickness { SickID = 6, EmployeeID = 4, From = new DateTime(2014, 04, 24), To = new DateTime(2014, 04, 28), SicknessType = "GRZ" });
            AddSickItem(new Sickness { SickID = 7, EmployeeID = 19, From = new DateTime(2014, 02, 27), To = new DateTime(2014, 03, 27), SicknessType = "Test" });

            List<Greeting> greetings = new List<Greeting>
                {
                    new Greeting{GreetingId = 1,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 2,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 3,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 4,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 5,GreetingHeader = "Greeting 5", GreetingBody = "Celebrate your birthday today. Celebrate being Happy every day"}
                };


            insurances = new List<Insurance>();
            AddInsurance(new Insurance { EmployeeID = 1, Days = 180, StartDate = new DateTime(2012, 08, 01), EndDate = new DateTime(2013, 12, 30), InsuranceOf = employees.Find(e => e.EmployeeID == 1) });
            AddInsurance(new Insurance { EmployeeID = 2, Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 2) });
            AddInsurance(new Insurance { EmployeeID = 3, Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 3) });
            AddInsurance(new Insurance { EmployeeID = 5, Days = 180, StartDate = new DateTime(2013, 01, 01), EndDate = new DateTime(2014, 08, 08), InsuranceOf = employees.Find(e => e.EmployeeID == 5) });
            AddInsurance(new Insurance { EmployeeID = 14, Days = 90, StartDate = new DateTime(2013, 01, 01), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(101), InsuranceOf = employees.Find(e => e.EmployeeID == 14) });

            questionSets = new List<QuestionSet>();
            AddQuestionSet(new QuestionSet { QuestionSetId = 1, Title = "FirstQuestionSet", Questions = "[\"FirstQuestion\",\"SecondQuestion\",\"ThirdQuestion\"]" });
            AddQuestionSet(new QuestionSet { QuestionSetId = 2, Title = "SecondQuestionSet", Questions = "[\"FoutrthQuestion\",\"FifthQuestion\",\"SixthQuestion\"]" });


            questionnaires = new List<Questionnaire>();
            AddQuestionnaires(new Questionnaire { QuestionnaireId = 1, Title = "Questionnaire 1", QuestionSetId = "5:1" });
            AddQuestionnaires(new Questionnaire { QuestionnaireId = 2, Title = "Questionnaire 2" , QuestionSetId = "1:2"});
            AddQuestionnaires(new Questionnaire { QuestionnaireId = 3, Title = "Questionnaire 3", QuestionSetId = "3:2,4:1,1:2,15:2,6:1" });
            AddQuestionnaires(new Questionnaire { QuestionnaireId = 4, Title = "Questionnaire 4", QuestionSetId = "0:0" });



            mock.Setup(m => m.Departments).Returns(departments);
            mock.Setup(m => m.Users).Returns(employees);
            mock.Setup(m => m.Locations).Returns(locations);
            mock.Setup(m => m.Visas).Returns(visas);
            mock.Setup(m => m.VisaRegistrationDates).Returns(visaRegistrationDates);
            mock.Setup(m => m.Permits).Returns(permits);
            mock.Setup(m => m.BusinessTrips).Returns(businessTrips);
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Verifiable();
            mock.Setup(m => m.Passports).Returns(passports);
            mock.Setup(m => m.Positions).Returns(positions);
            mock.Setup(m => m.PrivateTrips).Returns(privateTrips);
            mock.Setup(m => m.Countries).Returns(countries);
            mock.Setup(m => m.Journeys).Returns(journeys);
            mock.Setup(m => m.Holidays).Returns(holidays);
            mock.Setup(m => m.Units).Returns(units);
            mock.Setup(m => m.Overtimes).Returns(overtimes);
            mock.Setup(m => m.Vacations).Returns(vacations);
            mock.Setup(m => m.Sicknesses).Returns(sicks);
            mock.Setup(m => m.CalendarItems).Returns(calendarItems);
            mock.Setup(m => m.Employees).Returns(employees.Where(e => e.IsUserOnly == false).ToList<Employee>());
            mock.Setup(m => m.Greetings).Returns(greetings);
            mock.Setup(m => m.Insurances).Returns(insurances);
            mock.Setup(m => m.QuestionSets).Returns(questionSets); 
            mock.Setup(m => m.Questionnaires).Returns(questionnaires);
            mock.Setup(m => m.SaveQuestionnaire(It.IsAny<Questionnaire>())).Verifiable();


            mock.Setup(m => m.SearchAbsenceData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<String>())).Returns(new List<AbsenceViewModel>());
            mock.Setup(m => m.SearchAbsenceData(new DateTime(2000, 01, 01), new DateTime(2017, 07, 28), "")).Returns(new List<AbsenceViewModel> { new AbsenceViewModel() });
            mock.Setup(m => m.SearchAbsenceData(new DateTime(2012, 01, 01), new DateTime(2015, 01, 01), "")).Returns(new List<AbsenceViewModel> { new AbsenceViewModel() });
            mock.Setup(m => m.SearchAbsenceData(new DateTime(2012, 01, 01), new DateTime(2015, 01, 01), "andl")).Returns(new List<AbsenceViewModel> { new AbsenceViewModel() });
            mock.Setup(m => m.SearchAbsenceData(new DateTime(2012, 01, 01), new DateTime(2015, 01, 01), "andl")).Returns(new List<AbsenceViewModel> { new AbsenceViewModel() });
            mock.Setup(m => m.SearchUsersData("", "")).Returns(new List<EmployeeViewModel> { new EmployeeViewModel(employees[0]) });
            mock.Setup(m => m.SearchUsersData("SDDDA", "")).Returns(new List<EmployeeViewModel> { new EmployeeViewModel(employees[0]) });
            mock.Setup(m => m.SearchUsersData("SDDDA", "")).Returns(new List<EmployeeViewModel> { new EmployeeViewModel(employees[0]) });
            mock.Setup(m => m.GetBusinessTripDataByUnits(It.IsAny<int>())).Returns(new List<BusinessTripViewModel> { new BusinessTripViewModel(businessTrips[0]) });
            mock.Setup(m => m.GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(It.IsAny<int>())).Returns(new List<BusinessTripViewModel> { new BusinessTripViewModel(businessTrips[0]) });
            mock.Setup(m => m.SearchVisaData(It.IsAny<string>())).Returns(new List<Employee> { employees[0] });
            mock.Setup(m => m.SearchVisaDataExcludingDismissed(It.IsAny<string>())).Returns(new List<Employee> { employees[0] }); 
            mock.Setup(m => m.SearchWTRData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(new List<WTRViewModel>());
            mock.Setup(m => m.SearchWTRDataPerEMP(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Employee>())).Returns(new List<WTRViewModel>()); 
        }

        public static void AddEmployee(Employee emp)
        {
            employees.Add(emp);
            SetEmployeeRelations(emp);
        }

        private static void SetEmployeeRelations(Employee emp)
        {
            if (!emp.IsUserOnly)
            {
            departments.Find(d => d.DepartmentID == emp.DepartmentID).Employees.Add(employees.Find(e => e.EmployeeID == emp.EmployeeID));
            employees.Find(e => e.EmployeeID == emp.EmployeeID).Department = departments.Find(v => v.DepartmentID == emp.DepartmentID);
            }
        }

        public static void AddLocation(Location location)
        {
            locations.Add(location);
            SetLocationRelations(location);
        }

        private static void SetLocationRelations(Location location)
        {
            countries.Find(c => c.CountryID == location.CountryID).Locations.Add(locations.Find(l => l.LocationID == location.LocationID));
            locations.Find(l => l.LocationID == location.LocationID).Country = countries.Find(c => c.CountryID == location.CountryID);
        }

        public static void AddUnit(Unit unit)
        {
            units.Add(unit);
        }

        public static void AddOvertime(Overtime overtime)
        {
            overtimes.Add(overtime);
            SetOvertimeRelations(overtime);
        }

        private static void SetOvertimeRelations(Overtime overtime)
        {
            employees.Find(e => e.EmployeeID == overtime.EmployeeID).Overtimes.Add(overtimes.Find(o => o.OvertimeID == overtime.OvertimeID));
            overtimes.Find(o => o.OvertimeID == overtime.OvertimeID).Employee = employees.Find(e => e.EmployeeID == overtime.EmployeeID);
        }

        public static void AddVacation(Vacation vacation)
        {
            vacations.Add(vacation);
            SetVacationRelations(vacation);
        }

        public static void SetVacationRelations(Vacation vacation)
        {
            employees.Find(e => e.EmployeeID == vacation.EmployeeID).Vacations.Add(vacations.Find(v => v.VacationID == vacation.VacationID));
            vacations.Find(v => v.VacationID == vacation.VacationID).Employee = employees.Find(e => e.EmployeeID == vacation.EmployeeID);
        }
        
        public static void AddBusinessTrip(BusinessTrip bt)
        {
            businessTrips.Add(bt);
            SetBusinessTripRelations(bt);
        }

        private static void SetBusinessTripRelations(BusinessTrip bt)
        {
            employees.Find(e => e.EmployeeID == bt.EmployeeID).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == bt.BusinessTripID));
            businessTrips.Find(b => b.BusinessTripID == bt.BusinessTripID).BTof = (employees.Find(l => l.EmployeeID == bt.EmployeeID));
            businessTrips.Find(b => b.BusinessTripID == bt.BusinessTripID).Location = (locations.Find(l => l.LocationID == bt.LocationID));
            locations.Find(l => l.LocationID == bt.LocationID).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == bt.BusinessTripID));
            units.Find(l => l.UnitID == bt.UnitID).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == bt.BusinessTripID));
            businessTrips.Find(l => l.BusinessTripID == bt.BusinessTripID).Unit = units.Find(c => c.UnitID == bt.UnitID);
        }

        public static void AddVisa(Visa visa)
        {
            visas.Add(visa);
            SetVisaRelations(visa);
        }

        private static void SetVisaRelations(Visa visa)
        {
            employees.Find(e => e.EmployeeID == visa.EmployeeID).Visa = visas.Find(v => v.EmployeeID == visa.EmployeeID);
            visas.Find(v => v.EmployeeID == visa.EmployeeID).VisaOf = employees.Find(e => e.EmployeeID == visa.EmployeeID);
        }

        public static void AddVisaRegistrationDate(VisaRegistrationDate visaRegDate)
        {
            visaRegistrationDates.Add(visaRegDate);
            SetVisaRegistrationDate(visaRegDate);
        }

        private static void SetVisaRegistrationDate(VisaRegistrationDate visaRegDate)
        {
            employees.Find(e => e.EmployeeID == visaRegDate.EmployeeID).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == visaRegDate.EmployeeID);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == visaRegDate.EmployeeID).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == visaRegDate.EmployeeID);
        }



        public static void AddPermit(Permit permit)
        {
            permits.Add(permit);
            SetPermitRelations(permit);
        }

        private static void SetPermitRelations(Permit permit)
        {
            employees.Find(e => e.EmployeeID == permit.EmployeeID).Permit = permits.Find(v => v.EmployeeID == permit.EmployeeID);
            permits.Find(v => v.EmployeeID == permit.EmployeeID).PermitOf = employees.Find(e => e.EmployeeID == permit.EmployeeID);
        }

        public static void AddPassport(Passport passport)
        {
            passports.Add(passport);
            SetPassportRelations(passport);
        }

        private static void SetPassportRelations(Passport passport)
        {
            employees.Find(e => e.EmployeeID == passport.EmployeeID).Passport = passports.Find(p => p.EmployeeID == passport.EmployeeID);
            passports.Find(e => e.EmployeeID == passport.EmployeeID).PassportOf = employees.Find(e => e.EmployeeID == passport.EmployeeID);
        }

        public static void AddPosition(Position position)
        {
            positions.Add(position);
        }

        public static void AddPrivateTrip(PrivateTrip privateTrip)
        {
            privateTrips.Add(privateTrip);
            SetPrivateTrip(privateTrip);
        }

        private static void SetPrivateTrip(PrivateTrip privateTrip)
        {
            visas.Find(e => e.EmployeeID == 1).PrivateTrips.Add(privateTrips.Find(e => e.EmployeeID == privateTrip.EmployeeID));
            privateTrip.PrivateTripOf = visas.Find(c => c.EmployeeID == privateTrip.EmployeeID);
          // employees.Find(h => h.EmployeeID == visaOf.EmployeeID).Visa = visas.Find(c => c.EmployeeID == privateTrip.EmployeeID);
           //journey.JourneyOf = businessTrips.Find(b => b.BusinessTripID == journey.BusinessTripID);
        }

        public static void AddHoliday(Holiday holiday)
        {
            holidays.Add(holiday);
            SetHolidayRelations(holiday);
        }

        private static void SetHolidayRelations(Holiday holiday)
        {
            countries.Find(c => c.CountryID == holiday.CountryID).Holidays.Add(holidays.Find(h => h.HolidayID == holiday.HolidayID));
            holidays.Find(h => h.HolidayID == holiday.HolidayID).Country = countries.Find(c => c.CountryID == holiday.CountryID);

        }

        public static void AddJourney(Journey journey)
        {
            journeys.Add(journey);
            SetJourneyRelations(journey);
        }

        private static void SetJourneyRelations(Journey journey)
        {
            businessTrips.Find(b => b.BusinessTripID == journey.BusinessTripID).Journeys.Add(journey);
            journey.JourneyOf = businessTrips.Find(b => b.BusinessTripID == journey.BusinessTripID);
            //journeys.Find(v => v.BusinessTripID == journey.BusinessTripID).JourneyOf.EmployeeID = businessTrips.Find(b => b.BusinessTripID == journey.BusinessTripID).EmployeeID;
        }

        public static void AddCalendarItem(CalendarItem calendarItem)
        {
            calendarItems.Add(calendarItem);
            SetCalendarRelations(calendarItem);
        }

        private static void SetCalendarRelations(CalendarItem calendarItem)
        {
            employees.Find(e => e.EmployeeID == calendarItem.EmployeeID).CalendarItems.Add(calendarItem);
            calendarItem.Employee = employees.Find(c => c.EmployeeID == calendarItem.EmployeeID);

        }
        public static void AddSickItem(Sickness sick)
        {
            sicks.Add(sick);
            SetSickRelations(sick);

        }

        private static void SetSickRelations(Sickness sick)
        {
            employees.Find(e => e.EmployeeID == sick.EmployeeID).Sicknesses.Add(sicks.Find(s => s.SickID == sick.SickID));
            sicks.Find(s => s.SickID == sick.SickID).SickOf = employees.Find(s => s.EmployeeID == sick.EmployeeID);
        }

        public static void AddInsurance(Insurance insurance)
        {
            insurances.Add(insurance);
            SetInsuranceRelations(insurance);
        }

        private static void SetInsuranceRelations(Insurance insurance)
        {
            employees.Find(e => e.EmployeeID == insurance.EmployeeID).Insurance = insurances.Find(v => v.EmployeeID == insurance.EmployeeID);
            insurances.Find(v => v.EmployeeID == insurance.EmployeeID).InsuranceOf = employees.Find(e => e.EmployeeID == insurance.EmployeeID);
        }

        public static void AddQuestionSet(QuestionSet questionSet)
        {
            questionSets.Add(questionSet);
        }

        private static void SetQuestionSetRelations()
        {
            //No relations yet; 
        }

        public static void AddQuestionnaires(Questionnaire quest)
        {
            questionnaires.Add(quest);
        }

    }
}
