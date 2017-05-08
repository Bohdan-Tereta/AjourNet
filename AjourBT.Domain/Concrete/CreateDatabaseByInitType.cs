using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Abstract;
using WebMatrix.WebData;

namespace AjourBT.Domain.Concrete
{
    public class CreateDatabaseByInitType : IDatabaseInitializer<AjourDbContext>
    {
        public void InitializeDatabase(AjourDbContext context)
        {
            string DBInitType = System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"].ToString();

            switch (DBInitType)
            {
                case ("InitForTest"):
                    InitForTest(context);
                    break;
                case ("InitDbClear"):
                    InitDBClear(context);
                    break;
                default:
                    InitDbNotChanged(context);
                    break;
            }
        }

        public void InitForTest(AjourDbContext context)
        {
            if (context.Database.Exists()) 
                context.Database.Delete();
            context.Database.Create();
            
            #region initDB

            #region List<Employee>
            List<Employee> employees = new List<Employee>
       
             {
                new Employee {EmployeeID = 1,FirstName = "Johnny", LastName = "Rose", DepartmentID = 8, EID = "mlan",  DateEmployed = new DateTime(2011,11,01), IsManager = false, BirthDay = new DateTime(1987,04,17), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), EMail = "JohnnyRoseMail", IsGreetingMessageAllow = true, EducationAcquiredType = EducationType.BasicHigher },
                new Employee {EmployeeID = 2,FirstName = "Norma",LastName = "Cruz",DepartmentID = 1, EID = "ncru", DateEmployed = new DateTime(2003,04,11), IsManager = true, BirthDay = new DateTime(1987,04,16), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true, EducationAcquiredType = EducationType.BasicHigher, EducationAcquiredDate = new DateTime(2012,01,03) },          
                new Employee {EmployeeID = 3,FirstName = "Anatoliy",LastName = "George",DepartmentID = 8, EID = "sfis", DateEmployed = new DateTime(2003,04,11), IsManager = false, BirthDay = new DateTime(1987,04,18), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true, EducationInProgressType = EducationType.BasicHigher },
                new Employee {EmployeeID = 4,FirstName = "Harold",LastName = "Wood",DepartmentID = 8 , EID = "dkim", DateEmployed = new DateTime(2002,04,11), IsManager = false, BirthDay = new DateTime(1987,05,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true, EducationInProgressType = EducationType.BasicHigher, EducationInProgressDate = new DateTime(DateTime.Now.AddYears(2).Year,01,05) },
                new Employee {EmployeeID = 5,FirstName = "Wayne",LastName = "Olson",DepartmentID = 1, EID = "olsa", DateEmployed = new DateTime(2003,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true, EducationAcquiredType = EducationType.CompleteSecondary, EducationAcquiredDate = new DateTime(2001,05,01), EducationInProgressType = EducationType.BasicHigher, EducationInProgressDate = new DateTime(DateTime.Now.AddYears(2).Year,03,03) },
                new Employee {EmployeeID = 6,FirstName = "Russell",LastName = "Shaw",DepartmentID = 1, EID = "blon", DateEmployed = new DateTime(2003,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 7,FirstName = "Aaron",LastName = "Owens",DepartmentID = 1, EID = "wens", DateEmployed = new DateTime(2002,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 8,FirstName = "Hilary",LastName = "Hamilton",DepartmentID = 1, EID = "milt", DateEmployed = new DateTime(2003,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 9,FirstName = "Linda",LastName = "Morrison",DepartmentID = 1, EID = "acox",DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2001,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 10,FirstName = "Raymond",LastName = "Lee",DepartmentID = 1, EID = "aada", DateEmployed = new DateTime(2002,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 11,FirstName = "Karen", LastName = "Harris", DepartmentID = 1, EID = "warn", DateDismissed = new DateTime(2012,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 12,FirstName = "Antonio",LastName = "Perez",DepartmentID = 1, EID = "gwoo", DateEmployed = new DateTime(2003,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 13,FirstName = "Kenneth",LastName = "Cox",DepartmentID = 1, EID = "jriv", DateEmployed = new DateTime(2003,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 14,FirstName = "Tammy",LastName = "Williams",DepartmentID = 1, EID = "show", DateEmployed = new DateTime(2002,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 15,FirstName = "John",LastName = "Coleman",DepartmentID = 8, EID = "jton", DateEmployed = new DateTime(2003,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
               
                new Employee {EmployeeID = 16,FirstName = "Linda",LastName = "Reid",DepartmentID = 8, EID = "mhan", DateEmployed = new DateTime(2001,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 17,FirstName = "Aaron",LastName = "Knight",DepartmentID = 2, EID = "murp", DateEmployed = new DateTime(2002,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 18,FirstName = "Katherine",LastName = "Owens",DepartmentID = 2, EID = "owek", DateEmployed = new DateTime(2005,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 19,FirstName = "Brandon",LastName = "Reyes",DepartmentID = 2, EID = "ayou", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 20,FirstName = "Elizabeth",LastName = "Carpenter",DepartmentID = 2, EID = "jben", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 21,FirstName = "Catherine", LastName = "Mitchell", DepartmentID = 2, EID = "dcao",  DateEmployed = new DateTime(2001,11,01), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 22,FirstName = "Robert",LastName = "Knight",DepartmentID = 4, EID = "rkni", DateEmployed = new DateTime(2006,04,11), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 23,FirstName = "Laura",LastName = "Smith",DepartmentID = 2, EID = "cbur", DateEmployed = new DateTime(2004,04,11), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 24,FirstName = "Alice",LastName = "Elliott",DepartmentID = 2 , EID = "ldia", DateEmployed = new DateTime(2006,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 25,FirstName = "Melissa",LastName = "Calak",DepartmentID = 2, EID = "carn", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 26,FirstName = "Daniel",LastName = "Cunningham",DepartmentID = 2, EID = "phar", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 27,FirstName = "Kathryn",LastName = "Baker",DepartmentID = 2, EID = "salz", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 28,FirstName = "Earl",LastName = "Harper",DepartmentID = 2, EID = "swhe", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 29,FirstName = "Betty",LastName = "Moore",DepartmentID = 2, EID = "fbur", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 30,FirstName = "Teresa",LastName = "Simmons",DepartmentID = 2, EID = "wsim", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},

                new Employee {EmployeeID = 31,FirstName = "Gregory", LastName = "Jackson", DepartmentID = 3, EID = "sban", DateEmployed = new DateTime(2005,11,01), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 32,FirstName = "Lois",LastName = "Moreno",DepartmentID = 3, EID = "lmor", DateEmployed = new DateTime(2004,07,11), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 33,FirstName = "Alice",LastName = "Berry",DepartmentID = 3, EID = "rkey", DateEmployed = new DateTime(2005,08,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 34,FirstName = "Diana",LastName = "Dixon",DepartmentID = 3 , EID = "sros", DateEmployed = new DateTime(2007,04,18), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 35,FirstName = "Kathleen",LastName = "Hunt",DepartmentID = 3, EID = "bher", DateEmployed = new DateTime(2008,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 36,FirstName = "Rebecca",LastName = "Williamson",DepartmentID = 3, EID = "pros", DateEmployed = new DateTime(2005,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 37,FirstName = "Roger",LastName = "Marshall",DepartmentID = 3, EID = "jcru", DateEmployed = new DateTime(2005,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 38,FirstName = "Lisa",LastName = "Peterson",DepartmentID = 3, EID = "jbia", DateEmployed = new DateTime(2008,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 39,FirstName = "Jonathan",LastName = "Day",DepartmentID = 3, EID = "ealv",  DateDismissed = new DateTime(2012,11,01), DateEmployed = new DateTime(2008,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
               
                new Employee {EmployeeID = 40,FirstName = "Rachel",LastName = "Stanley",DepartmentID = 9, EID = "dsto", DateEmployed = new DateTime(2008,09,04), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 41,FirstName = "Theresa", LastName = "Mills", DepartmentID = 9, EID = "kdan", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2010,11,01), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 42,FirstName = "Virginia",LastName = "Owens",DepartmentID = 9, EID = "ewel", DateEmployed = new DateTime(2012,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 43,FirstName = "Beverly",LastName = "Black",DepartmentID = 9, EID = "arey", DateEmployed = new DateTime(2012,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 44,FirstName = "Paul",LastName = "Bishop",DepartmentID = 9, EID = "ndoz", DateEmployed = new DateTime(2010,09,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 45,FirstName = "Billy",LastName = "Bishop",DepartmentID = 9, EID = "smey", DateEmployed = new DateTime(2010,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
               
                new Employee {EmployeeID = 46,FirstName = "Jason",LastName = "Fuller",DepartmentID = 4, EID = "apat", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 47,FirstName = "Maria",LastName = "Johnston",DepartmentID = 4, EID = "lark", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 48,FirstName = "Lillian",LastName = "Ray",DepartmentID = 4, EID = "ayes", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 49,FirstName = "Pamela",LastName = "Long",DepartmentID = 4, EID = "lgon", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 50,FirstName = "Doris",LastName = "Carr",DepartmentID = 4, EID = "vper", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 51,FirstName = "Helen", LastName = "Owens", DepartmentID = 8, EID = "oweh", DateEmployed = new DateTime(2011,11,01), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 52,FirstName = "Katherine",LastName = "Hall",DepartmentID = 8, EID = "khal", DateEmployed = new DateTime(2013,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 53,FirstName = "Maria",LastName = "Lawrence",DepartmentID = 4, EID = "bden", DateEmployed = new DateTime(2013,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 54,FirstName = "Louis",LastName = "Crawford",DepartmentID = 4 , EID = "pkey", DateEmployed = new DateTime(2012,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 55,FirstName = "Jonathan",LastName = "Austin",DepartmentID = 4, EID = "ppez", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 56,FirstName = "Sharon",LastName = "Nelson",DepartmentID = 4, EID = "mter", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 57,FirstName = "Steve",LastName = "Scott",DepartmentID = 4, EID = "foll", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 58,FirstName = "Christopher",LastName = "Peters",DepartmentID = 4, EID = "kins",  DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2009,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 59,FirstName = "Laura",LastName = "Butler",DepartmentID = 4, EID = "jclk", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 60,FirstName = "Robert",LastName = "Reed",DepartmentID = 4, EID = "ender", DateDismissed = new DateTime(2013,11,01),DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},

                new Employee {EmployeeID = 61,FirstName = "Frank", LastName = "Hunter", DepartmentID = 8, EID = "briv",  DateEmployed = new DateTime(2010,11,01), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 62,FirstName = "Joshua",LastName = "Hanson",DepartmentID = 5, EID = "jhan", DateEmployed = new DateTime(2009,04,11), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 63,FirstName = "Mary",LastName = "Hughes",DepartmentID = 5, EID = "sdea", DateEmployed = new DateTime(2009,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 64,FirstName = "Judy",LastName = "Perez",DepartmentID = 5 , EID = "stho", DateEmployed = new DateTime(2008,08,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 65,FirstName = "Douglas",LastName = "Chapman",DepartmentID = 5, EID = "rper", DateEmployed = new DateTime(2009,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 66,FirstName = "Kathy",LastName = "Foster",DepartmentID = 5, EID = "lpar", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2009,04,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 67,FirstName = "Judy",LastName = "Morris",DepartmentID = 5, EID = "fcar", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 68,FirstName = "Beverly",LastName = "Alvarez",DepartmentID = 5, EID = "tmas", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 69,FirstName = "Samuel",LastName = "Payne",DepartmentID = 5, EID = "scha", DateEmployed = new DateTime(2011,02,11), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 70,FirstName = "Lois",LastName = "Walker",DepartmentID = 5, EID = "mful", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          

                new Employee {EmployeeID = 71,FirstName = "Phillip", LastName = "Morris", DepartmentID = 6, EID = "moph", DateEmployed = new DateTime(2011,11,01), IsManager = true, BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(4), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 72,FirstName = "Brandon",LastName = "Ferguson",DepartmentID = 6, EID = "rlan", DateEmployed = new DateTime(2013,04,11), IsManager = false, BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(3), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 73,FirstName = "Stephanie",LastName = "Armstrong",DepartmentID = 6, EID = "jann", DateEmployed = new DateTime(2013,09,11), IsManager = false,BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(2), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 74,FirstName = "Jerry",LastName = "Garza",DepartmentID = 6, EID = "jasn", DateDismissed = new DateTime(2013,01,01), DateEmployed = new DateTime(2009,04,17), IsManager = false, BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(1), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                
                new Employee {EmployeeID = 75,FirstName = "Marie",LastName = "Andrews",DepartmentID = 7, EID = "maad", DateEmployed = new DateTime(2013,07,21), IsManager = true, BirthDay = new DateTime(1987,11,10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 76,FirstName = "John",LastName = "Reyes",DepartmentID = 7, EID = "ljac", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay =  DateTime.Now.ToLocalTimeAzure().AddDays(-11), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 77,FirstName = "Edward",LastName = "Little",DepartmentID = 8, EID = "mtuc", DateEmployed = new DateTime(2012,09,04), IsManager = true, BirthDay =  DateTime.Now.ToLocalTimeAzure().AddDays(30), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 78,FirstName = "Andrea",LastName = "Lewis",DepartmentID = 7, EID = "kwal", DateEmployed = new DateTime(2013,07,21), IsManager = false, BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(31), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 79,FirstName = "Donald",LastName = "Hawkins",DepartmentID = 7, EID = "tebo", DateEmployed = new DateTime(2011,04,11), IsManager = false, BirthDay = DateTime.Now.ToLocalTimeAzure(), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 80,FirstName = "Richard",LastName = "Shaw",DepartmentID = 8, EID = "jmil", DateEmployed = new DateTime(2012,09,04), IsManager = false, BirthDay = DateTime.Now.ToLocalTimeAzure().AddDays(-10), FullNameUk = "Джоні Роус", Comment = "Happy Birthday!!!", PositionID = 2,CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(),Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true}, 
                new Employee {EmployeeID = 81, FirstName = "d", LastName = "c", DepartmentID = 1, EID = "User", IsUserOnly = true , IsGreetingMessageAllow = true}, 
                new Employee {EmployeeID = 82, FirstName = "a", LastName = "b", DepartmentID = 1, EID = "User4", IsUserOnly = true , IsGreetingMessageAllow = true }, 
                new Employee {EmployeeID = 83, FirstName = "User1", LastName = "User3", DepartmentID = 1, EID = "User2", IsUserOnly = true , IsGreetingMessageAllow = true}, 
                new Employee {EmployeeID = 84, FirstName = "User2", LastName = "User2", DepartmentID = 1, EID = "User1", IsUserOnly = true , IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 85, FirstName = "User3", LastName = "User1", DepartmentID = 1, EID = "User3", IsUserOnly = true , IsGreetingMessageAllow = true}, 

             };
            #endregion

            #region List<Location>
            List<Location> locations = new List<Location>
             { 
                new Location {LocationID = 1, Title = "AT/BW", Address = "Atherton, 512 Burrows Way St.", CountryID = 2}, 
                new Location {LocationID = 2, Title = "EC/BV", Address = "El Centro, 51560 Buhler Way", CountryID = 3},
                new Location {LocationID = 3, Title = "RB/MA", Address = "Redondo Beach, 820 Mockingbird Alley  St.", CountryID = 4}, 
                new Location {LocationID = 4, Title = "HO/LR", Address = "Hollister, 31 Lien Road", CountryID = 2},
                new Location {LocationID = 5, Title = "LO/JA", Address = "Loomis, 884 Jenna Avenue", CountryID = 2}, 
                new Location {LocationID = 6, Title = "CO/RP", Address = "Colma, 45 Reindahl Place", CountryID = 2},
                new Location {LocationID = 7, Title = "PA/MC", Address = "Paradise, 17 Michigan Circle St.", CountryID = 3}, 
                new Location {LocationID = 8, Title = "MP/NL", Address = "Menlo Park, 5 North Lane", CountryID = 3}           
             };
            #endregion

            #region List<Unit>
            List<Unit> units = new List<Unit>
                { 
                new Unit { UnitID = 1, Title = "Unknown", ShortTitle = "-" },
                new Unit { UnitID = 2, Title = "Business Development Unit", ShortTitle = "BD" },
                new Unit { UnitID = 3, Title = "EPUA Board", ShortTitle = "EPUA_B"}, 
                new Unit { UnitID = 4, Title = "EPOL Board", ShortTitle = "B" }, 
                new Unit { UnitID = 5, Title = "Finance Unit", ShortTitle = "F" }
             };
            #endregion

            #region List<BusinessTrip>
            List<BusinessTrip> businessTrips = new List<BusinessTrip> 
            {          
                new BusinessTrip { BusinessTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(18),  OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(32), UnitID = 1, DaysInBtForOrder = 15, Responsible= "mter",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(15), Status= BTStatus.Planned, EmployeeID = 1, LocationID = 1 , Manager="ncru", Purpose = "Meeting"},
                new BusinessTrip { BusinessTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(50), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(27), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(52), UnitID = 1,  DaysInBtForOrder = 26, Responsible= "mter", LastCRUDedBy = "ncru", LastCRUDTimestamp =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(15), Status= BTStatus.Planned| BTStatus.Cancelled, EmployeeID = 2, LocationID = 1, Manager="ncru", Purpose = "Meeting" },
                new BusinessTrip { BusinessTripID = 3, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(39), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(18), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(41), UnitID = 1,DaysInBtForOrder = 24, Responsible= "mter",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Planned | BTStatus.Modified, OldStartDate=new DateTime (2013,08,02), OldEndDate= new DateTime (2014,10,02),OldLocationID=1, OldLocationTitle="AT/BW", Manager="ncru", Purpose = "Meeting", EmployeeID = 3, LocationID = 5 },
                new BusinessTrip { BusinessTripID = 4, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(365), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(366), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(364), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(367), UnitID = 1,DaysInBtForOrder = 4, Responsible= "mter",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Registered, Comment = " employee registered",  EmployeeID = 4, LocationID = 1 , Manager="ncru", Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(8), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(9), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(7), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(10), UnitID = 1, DaysInBtForOrder = 4, Responsible= "mter", LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,01,02), Status= BTStatus.Registered | BTStatus.Cancelled,Comment = " employee registered and cancelled", EmployeeID = 5, LocationID = 1, Manager="ncru", Purpose = "Meeting" }, 
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,12,02), OrderStartDate = new DateTime(2013,08,31), OrderEndDate = new DateTime (2013,12,03), DaysInBtForOrder = 95, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,12,02), OldStartDate=new DateTime (2013,08,02), OldEndDate= new DateTime (2014,01,02),OldLocationID=1, UnitID = 1,Responsible= "mter",OldLocationTitle="AT/BW", Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 6, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 7, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(19), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(31), DaysInBtForOrder = 22, UnitID = 1, Responsible= "mter",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), Status= BTStatus.Confirmed, Comment = " employee confirmed and cancelled", EmployeeID = 7, Manager="ncru", Purpose = "Meeting", LocationID = 2, Flights = "Lviv - Linkopin(3400 UAH), Linkopin - Lviv(3500 UAH)", Habitation = "Linkopin. Hotel(ElegantSolutions apartments, ul.Kapelanka 6a), price = 15000 PLN"}, 
                new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,10,02),OrderStartDate = new DateTime(2013,08,31), OrderEndDate = new DateTime (2013,10,03), DaysInBtForOrder = 34, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,01,02), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 8, LocationID = 2, Manager="ncru", UnitID = 1,Responsible= "mter",Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,11,12), OrderStartDate = new DateTime(2013,09,30), OrderEndDate = new DateTime (2013,11,13), DaysInBtForOrder = 45, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,05,12), OldStartDate=new DateTime (2013,08,01), OldEndDate= new DateTime (2014,01,15),OldLocationID=1, UnitID = 1,OldLocationTitle="AT/BW", Responsible= "mter",Manager="ncru", Purpose = "Meeting",Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 10, LocationID = 3 }, 
                new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2013,08,21), EndDate = new DateTime (2013,11,12), OrderStartDate = new DateTime(2013,08,20), OrderEndDate = new DateTime (2013,11,13), DaysInBtForOrder = 85, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,05,12), OldStartDate=new DateTime (2013,08,01), OldEndDate= new DateTime (2014,02,15),OldLocationID=2, UnitID = 1,OldLocationTitle="CC/BV",Responsible= "mter", Manager="ncru", Purpose = "Meeting", Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 12, LocationID = 3 },
                new BusinessTrip { BusinessTripID = 11, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,12,02), OrderStartDate = new DateTime(2013,08,31), OrderEndDate = new DateTime (2013,12,03), DaysInBtForOrder = 95 , LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,10,02), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 13, LocationID = 4, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter", Flights = "Lviv - Linkopin(3400 UAH), Linkopin - Lviv(3500 UAH)", Habitation = "Linkopin. Hotel(ElegantSolutions apartments, ul.Kapelanka 6a), price = 15000 PLN"},              
                new BusinessTrip { BusinessTripID = 12, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(49), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(19), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(50), DaysInBtForOrder = 32,UnitID = 1, Manager="ncru",Responsible= "mter", Purpose = "Meeting", LastCRUDedBy = "ncru", LastCRUDTimestamp =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), Status= BTStatus.Planned, EmployeeID = 21, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 13, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,12,30), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,12,31), DaysInBtForOrder = 62, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2015,12,30), Status= BTStatus.Planned| BTStatus.Cancelled, EmployeeID = 22, LocationID = 1, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter" },
                new BusinessTrip { BusinessTripID = 14, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(1), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(5), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure(), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(6), DaysInBtForOrder = 7, LastCRUDedBy = "ncru", UnitID = 1,Manager="ncru", Purpose = "Meeting",Responsible= "mter", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-3), Status= BTStatus.Planned | BTStatus.Modified, OldStartDate=new DateTime (2013,11,02), OldEndDate= new DateTime (2014,01,02),OldLocationID=1, OldLocationTitle="AT/BW", EmployeeID = 23, LocationID = 7 },
                new BusinessTrip { BusinessTripID = 15, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(13), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(14), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(12), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(15), DaysInBtForOrder = 4,UnitID = 1, LastCRUDedBy = "ncru", Manager="ncru", Purpose = "Meeting", Responsible= "mter",LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-5), Status= BTStatus.Registered, EmployeeID = 23, LocationID = 1 }, 
                new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,11,02), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,11,03), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,01,02), Status= BTStatus.Registered | BTStatus.Cancelled, Comment =  " employee registered and cancelled" ,UnitID = 1, EmployeeID = 24, Manager="ncru", Purpose = "Meeting", Responsible= "mter",LocationID = 1 }, 
                new BusinessTrip { BusinessTripID = 17, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(1), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(32), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure(), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(35), DaysInBtForOrder = 36, LastCRUDedBy = "ncru", UnitID = 1,Manager="ncru", Responsible= "mter",Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-20), OldStartDate=new DateTime (2013,11,01), OldEndDate= new DateTime (2014,01,02),OldLocationID=1, OldLocationTitle="AT/BW", Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 25, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 18, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(3), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(4), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(2), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(5), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", UnitID = 1,Responsible= "mter",Manager="ncru", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-7), Status= BTStatus.Confirmed, EmployeeID = 26, LocationID = 2}, 
                new BusinessTrip { BusinessTripID = 19, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,11,02), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,11,01), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,01,02), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 27, LocationID = 2, Manager="ncru", Responsible= "mter",UnitID = 1,Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 20, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-9), EndDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), OrderStartDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-10), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure(), DaysInBtForOrder = 11, Responsible= "mter",LastCRUDedBy = "ncru", UnitID = 1,Manager="ncru", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-30), OldStartDate=new DateTime (2013,11,01), OldEndDate= new DateTime (2014,02,15),OldLocationID=2, OldLocationTitle="CC/BV", Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 28, LocationID = 3, Flights = "Lviv - Lodz(700 UAH), Lodz - Lviv(750 UAH)", Habitation = "Lodz. Hotel(Mega apartments, ul.Kapelanka 6a), price = 4500 PLN" }, 
                new BusinessTrip { BusinessTripID = 21, StartDate = new DateTime(2013,12,01), EndDate = new DateTime (2013,12,12), OrderStartDate = new DateTime(2013,11,30), OrderEndDate = new DateTime (2013,12,13), DaysInBtForOrder = 14, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,05,12),  OldStartDate=new DateTime (2013,11,01), OldEndDate= new DateTime (2014,02,15),OldLocationID=2,Responsible= "mter", UnitID = 1,OldLocationTitle="CC/BV", Manager="ncru", Purpose = "Meeting", Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 30, LocationID = 3 },
                new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,11,11), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,11,12), DaysInBtForOrder = 13, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,01,11), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 26, LocationID = 4, Manager="ncru", Responsible= "mter",UnitID = 1,Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 23, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(60), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(65), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(52), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(66), DaysInBtForOrder = 15, LastCRUDedBy = "ncru", UnitID = 1, Responsible= "mter", Manager="ncru", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-9), Status= BTStatus.Planned, EmployeeID = 31, LocationID = 1 },
                //new BusinessTrip { BusinessTripID = 24, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-295), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-200), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-285), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-200), DaysInBtForOrder = 96, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-300),  Status= BTStatus.Planned| BTStatus.Cancelled, EmployeeID = 32, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 25, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(8), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(17), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(6), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), DaysInBtForOrder = 15, UnitID = 1,Responsible = "mter",LastCRUDedBy = "ncru", Manager="ncru", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Planned | BTStatus.Modified, OldStartDate=new DateTime (2012,08,02), OldEndDate= new DateTime (2013,10,02),OldLocationID=1, OldLocationTitle="AT/BW", EmployeeID = 33, LocationID = 6 },
                new BusinessTrip { BusinessTripID = 26, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(3), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(2), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(29), DaysInBtForOrder = 30, UnitID = 1,Responsible = "mter",LastCRUDedBy = "ncru", Manager="ncru", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Registered, Comment= " employee registered", EmployeeID = 34, LocationID = 1 }, 
                new BusinessTrip { BusinessTripID = 27, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,11,02), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,11,03), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,01,02), Status= BTStatus.Registered | BTStatus.Cancelled, Comment=" employee registered", EmployeeID = 35, UnitID = 1,Responsible = "mter",LocationID = 1 , Manager="ncru", Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 28, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,12,02), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,12,03), DaysInBtForOrder = 34, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2012,12,02), OldStartDate=new DateTime (2012,11,01), OldEndDate= new DateTime (2013,01,02),OldLocationID=1,Responsible= "mter",UnitID = 1, Manager="ncru", Purpose = "Meeting", OldLocationTitle="AT/BW", Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 36, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 29, StartDate = new DateTime(2013,11,01), EndDate = new DateTime (2013,12,01), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,12,02), DaysInBtForOrder = 33, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,01,02), Status= BTStatus.Confirmed, EmployeeID = 37, Manager="ncru", Purpose = "Meeting", Responsible= "mter",UnitID = 1, LocationID = 2}, 
                new BusinessTrip { BusinessTripID = 30, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-325), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-300), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-326), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-299), Responsible= "mter", UnitID = 1,Manager="ncru", Purpose = "Meeting", DaysInBtForOrder = 28, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-335), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 38, LocationID = 2}, 
                new BusinessTrip { BusinessTripID = 31, StartDate = new DateTime(2012,12,01), EndDate = new DateTime (2012,12,12), OrderStartDate = new DateTime(2012,11,30), OrderEndDate = new DateTime (2012,12,13), DaysInBtForOrder = 14, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,05,12), OldStartDate = new DateTime (2012,11,01), OldEndDate= new DateTime (2013,02,15),Responsible= "mter",UnitID = 1, Manager="ncru", Purpose = "Meeting",OldLocationID=2, OldLocationTitle="CC/BV", Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 40, LocationID = 3 }, 
                new BusinessTrip { BusinessTripID = 32, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,10), OrderStartDate = new DateTime(2014,11,30), OrderEndDate = new DateTime (2014,12,11), DaysInBtForOrder = 12, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,05,12),  OldStartDate=new DateTime (2012,11,01), OldEndDate= new DateTime (2013,02,15), Responsible= "mter",UnitID = 1,Manager="ncru", Purpose = "Meeting",OldLocationID=2, OldLocationTitle="CC/BV", Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 42, LocationID = 3 },
                //new BusinessTrip { BusinessTripID = 33, StartDate = new DateTime(2014,11,01), EndDate = new DateTime (2014,12,02), OrderStartDate = new DateTime(2014,10,31), OrderEndDate = new DateTime (2014,12,03), DaysInBtForOrder = 34, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2012,01,02), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 44, LocationID = 4, Responsible= "mter",UnitID = 1,Manager="ncru", Purpose = "Meeting"}, 
                new BusinessTrip { BusinessTripID = 34, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(365), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(395), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(364), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(396),Responsible= "mter", UnitID = 1,Manager="ncru", Purpose = "Meeting",DaysInBtForOrder = 33, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Planned | BTStatus.Modified, EmployeeID = 54, LocationID = 6, Comment = " employee plan + modif", OldStartDate = new DateTime(2013,11,10), OldEndDate = new DateTime(2013,11,10), OldLocationID = 1,  OldLocationTitle = "AT/BW", RejectComment = "Visa expired"  },
                new BusinessTrip { BusinessTripID = 35, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(365), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(395), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(364), OrderEndDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(396),Responsible= "mter",UnitID = 1, DaysInBtForOrder = 33,LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-15), Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 62, OldStartDate=new DateTime (2013,08,02), OldEndDate= new DateTime (2014,10,02),OldLocationID=1, OldLocationTitle="AT/BW", LocationID = 4, Comment = "employee reg + modif", Manager = "rkni", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 37, StartDate = new DateTime(2013,12,01), EndDate = new DateTime (2013,12,25), OrderStartDate = new DateTime(2013,11,30), OrderEndDate = new DateTime (2013,12,26), DaysInBtForOrder = 27, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,12,25), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 72, Responsible= "mter",LocationID = 4, UnitID = 1,Comment = " employee conf and reported", Manager = "rkni", Purpose = "meeting",  Flights = "Lviv - Linkopin(3400 UAH), Linkopin - Lviv(3500 UAH)", Habitation = "Linkopin. Hotel(ElegantSolutions apartments, ul.Kapelanka 6a), price = 15000 PLN" },
                new BusinessTrip { BusinessTripID = 38, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), OrderStartDate = new DateTime(2014,11,30), OrderEndDate = new DateTime (2014,12,26), DaysInBtForOrder = 27, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID =72, Responsible= "mter",OldStartDate=new DateTime (2013,08,02), Manager="lmor",UnitID = 1, Purpose = "Meeting", OldEndDate= new DateTime (2014,10,02),OldLocationID=1, OldLocationTitle="AT/BW", LocationID = 4, Comment = "employee conf and reported and modif", Flights = "Lviv - Krakow(500 UAH), Krakow - Lviv(500 UAH)", Habitation = "Krakow. Hotel(Stay apartments, ul.Kapelanka 6a), price = 6000 PLN" },
                new BusinessTrip { BusinessTripID = 39, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(300), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(360), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(299), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(362), Responsible= "mter",DaysInBtForOrder = 63,LastCRUDedBy = "ncru", UnitID = 1, Manager="lmor", Purpose = "Meeting",LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(5), Status= BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 67, LocationID = 2, Comment = " employee reg and cancelled", CancelComment = "visa expired"},
                new BusinessTrip { BusinessTripID = 40, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,25), OrderStartDate = new DateTime(2013,08,31), OrderEndDate = new DateTime (2013,09,26), DaysInBtForOrder = 27, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,09,25), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 71, LocationID = 3, Responsible= "mter",Comment = " employee confirmed and cancelled", UnitID = 1,Manager = "lmor", Purpose = "meeting", CancelComment = "visa expired", Invitation = true },
                new BusinessTrip { BusinessTripID = 41, StartDate = new DateTime(2014,09,01), EndDate = new DateTime (2014,09,27), OrderStartDate = new DateTime(2014,08,31), OrderEndDate = new DateTime (2014,09,28), DaysInBtForOrder = 29, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2014,09,27), Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 71,Responsible= "mter", LocationID = 3, Manager="lmor", Purpose = "Meeting", UnitID = 1,Comment = "conf,mod,cancelled", CancelComment = "visa expired", OldLocationID = 1, OldLocationTitle = "AT/BW", OldEndDate = new DateTime(2012,01,02), OldStartDate = new DateTime(2012,04,05) },
                new BusinessTrip { BusinessTripID = 42, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(65), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(90), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(64), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(92), Responsible= "mter",DaysInBtForOrder = 38, Manager="lmor", Purpose = "Meeting",UnitID = 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(60), Status= BTStatus.Planned, EmployeeID = 61, LocationID = 5, Comment = "2 employee planned and rejected(with comment)"},
                new BusinessTrip { BusinessTripID = 43, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), EndDate= DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(40), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(29), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(42), Responsible= "mter",DaysInBtForOrder = 13, LastCRUDedBy = "ncru",  Manager="lmor",UnitID = 1, Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(29), Status= BTStatus.Planned | BTStatus.Modified, EmployeeID = 61, LocationID = 5, Comment = "2 employee planned and rejected(with comment)",  RejectComment = "visa expired", OldLocationID = 1, OldLocationTitle = "AT/BW", OldStartDate = new DateTime(2012,02,01), OldEndDate = new DateTime(2013,02,01) },
                new BusinessTrip { BusinessTripID = 44, StartDate = new DateTime(2013,09,09), EndDate = new DateTime (2013,09,10), OrderStartDate = new DateTime(2013,09,09), OrderEndDate = new DateTime (2013,09,11), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2013,09,09), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 61, LocationID = 1, Responsible= "mter",Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting", Flights = "Lviv - Krakow(500 UAH), Krakow - Lviv(500 UAH)", Habitation = "Krakow. Hotel(Stay apartments, ul.Kapelanka 6a), price = 6000 PLN" },
                new BusinessTrip { BusinessTripID = 45, StartDate = new DateTime(2014,01,09), EndDate = new DateTime (2014,04,08), OrderStartDate = new DateTime(2014,01,08), OrderEndDate = new DateTime (2014,04,09),  DaysInBtForOrder = 93, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2012,08,08), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 61, LocationID = 1, Responsible= "mter", Comment = "conf,cancelled", Manager = "khal",UnitID = 1, Purpose = "meeting" },              
                new BusinessTrip { BusinessTripID = 47, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), DaysInBtForOrder = 11, Responsible= "mter",LastCRUDedBy = "ncru", UnitID = 1, Manager="lmor", Purpose = "Meeting", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-14), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 62, LocationID = 1, Comment = "confirmed and reported",   Flights = "Lviv - Linkopin(3400 UAH), Linkopin - Lviv(3500 UAH)", Habitation = "Linkopin. Hotel(ElegantSolutions apartments, ul.Kapelanka 6a), price = 15000 PLN" },
                new BusinessTrip { BusinessTripID = 48, StartDate = new DateTime(2013,01,01), EndDate = new DateTime (2013,03,10), OrderStartDate = new DateTime(2013,01,01), OrderEndDate = new DateTime (2013,03,12), DaysInBtForOrder = 73, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-15), Status= BTStatus.Confirmed | BTStatus.Reported, Responsible= "mter",EmployeeID = 62, LocationID = 1,UnitID = 1, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting", Flights = "Lviv - Krakow(500 UAH), Krakow - Lviv(500 UAH)", Habitation = "Krakow. Hotel(Stay apartments, ul.Kapelanka 6a), price = 6000 PLN" },
                new BusinessTrip { BusinessTripID = 49, StartDate = new DateTime(2013,04,25), EndDate = new DateTime (2013,05,11), OrderStartDate = new DateTime(2013,04,25), OrderEndDate = new DateTime (2013,05,12), DaysInBtForOrder = 18, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-10), Status= BTStatus.Confirmed | BTStatus.Reported,Responsible= "mter", EmployeeID = 62, LocationID = 1,UnitID = 1, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting", BTMComment="First BT" },
                new BusinessTrip { BusinessTripID = 50, StartDate = new DateTime(2014,01,01), EndDate = new DateTime (2014,01,03), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,04), DaysInBtForOrder = 5, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012,10,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1, Responsible= "mter",Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 51, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-40), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-51), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-11), Responsible= "mter", DaysInBtForOrder = 40, Manager="lmor", Purpose = "Meeting", UnitID = 1,LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-60), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 52, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,04), OrderStartDate = new DateTime(2013,09,01), OrderEndDate = new DateTime (2013,09,05), DaysInBtForOrder = 6, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed |  BTStatus.Cancelled, EmployeeID = 62, LocationID = 1,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal",UnitID = 1, Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 53, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,10,02), OrderStartDate = new DateTime(2013,09,30), OrderEndDate = new DateTime (2013,10,04), DaysInBtForOrder = 6, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 62, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal",UnitID = 1, Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 54, StartDate = new DateTime(2014,05,01), EndDate = new DateTime (2014,05,10), OrderStartDate = new DateTime(2014,05,01), OrderEndDate = new DateTime (2014,05,12), DaysInBtForOrder = 13, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 62, LocationID = 3,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 55, StartDate = new DateTime(2014,11,11), EndDate = new DateTime (2014,12,12), OrderStartDate = new DateTime(2014,11,10), OrderEndDate = new DateTime (2014,12,14), DaysInBtForOrder = 34, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 62, LocationID = 2, Responsible= "mter",Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                //new BusinessTrip { BusinessTripID = 56, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-40), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-38), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-40), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-37), DaysInBtForOrder = 4, Responsible= "mter", Manager="lmor", Purpose = "Meeting", UnitID = 1,LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-50), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 2, Comment = "confirmed and reported" },
                //new BusinessTrip { BusinessTripID = 57, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-41), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-51), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-41), DaysInBtForOrder = 11, Responsible= "mter",   Manager="lmor", Purpose = "Meeting",UnitID = 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-55), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 3, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 58, StartDate = new DateTime(2013,02,02), EndDate = new DateTime (2013,02,05), OrderStartDate = new DateTime(2013,02,01), OrderEndDate = new DateTime (2013,02,06), DaysInBtForOrder = 6, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 4, Responsible= "mter",Comment = "confirmed and reported", Manager = "khal",UnitID = 1, Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 59, StartDate = new DateTime(2013,02,13), EndDate = new DateTime (2013,03,08), OrderStartDate = new DateTime(2013,02,09), OrderEndDate = new DateTime (2013,03,09), DaysInBtForOrder = 29, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 5,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 60, StartDate = new DateTime(2013,02,02), EndDate = new DateTime (2013,02,05), OrderStartDate = new DateTime(2013,02,01), OrderEndDate = new DateTime (2013,03,06), DaysInBtForOrder = 34, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 3, LocationID = 4,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 61, StartDate = new DateTime(2013,03,15), EndDate = new DateTime (2013,04,20), OrderStartDate = new DateTime(2013,03,14), OrderEndDate = new DateTime (2013,04,21), DaysInBtForOrder = 38, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 4, LocationID = 3,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal",UnitID = 1, Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 62, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-9), Responsible= "mter",DaysInBtForOrder = 12, Manager="dsto", Purpose = "Meeting", UnitID = 1,LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-20), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 4, Comment = "confirmed and reported "},
                new BusinessTrip { BusinessTripID = 63, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-30), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-31), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), DaysInBtForOrder = 30, Manager="dsto", Purpose = "Meeting",LastCRUDedBy = "ncru", UnitID = 1,LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-40), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 6, LocationID = 5, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 64, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(50), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(59), OrderStartDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(49), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(60),Responsible= "mter", DaysInBtForOrder = 12, Manager="dsto", UnitID = 1,Purpose = "Meeting",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 3, LocationID = 2, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 65, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(70), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(89), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(69), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(90),Responsible= "mter", DaysInBtForOrder = 22,Manager="dsto", UnitID = 1,Purpose = "Meeting", LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-6), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 14, LocationID = 3, Comment = "confirmed and reported"},
                new BusinessTrip { BusinessTripID = 66, StartDate = new DateTime(2013,03,04), EndDate = new DateTime(2013,03,07), OrderStartDate = new DateTime(2013,03,03), OrderEndDate = new DateTime(2013,03,08),Responsible= "mter", DaysInBtForOrder = 6, Manager="dsto", Purpose = "Meeting",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-65), Status= BTStatus.Confirmed | BTStatus.Reported, UnitID = 1,EmployeeID = 15, LocationID = 4, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 67, StartDate = new DateTime(2013,04,10), EndDate = new DateTime (2013,04,25), OrderStartDate = new DateTime(2013,04,09), OrderEndDate = new DateTime (2013,04,26), DaysInBtForOrder = 18, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 16, LocationID = 5,Responsible= "mter", Comment = "confirmed and reported", UnitID = 1,Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 68, StartDate = new DateTime(2013,05,02), EndDate = new DateTime (2013,06,05), OrderStartDate = new DateTime(2013,05,01), OrderEndDate = new DateTime (2013,06,06), DaysInBtForOrder = 37, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 2, Responsible= "mter",Comment = "confirmed and reported",UnitID = 1, Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 69, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-21), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), DaysInBtForOrder = 21,Responsible= "mter", Manager="dsto", UnitID = 1,Purpose = "Meeting",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-25), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 9, LocationID = 3, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 70, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-35), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-101), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-34), DaysInBtForOrder = 68, Manager="dsto", Responsible= "mter",UnitID = 1,Purpose = "Meeting",LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-120), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 9, LocationID = 4, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 71, StartDate = new DateTime(2013,07,10), EndDate = new DateTime (2013,07,26), OrderStartDate = new DateTime(2013,07,09), OrderEndDate = new DateTime (2013,07,27), DaysInBtForOrder = 19, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 9, LocationID = 5,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 72, StartDate = new DateTime(2013,08,02), EndDate = new DateTime (2013,09,05), OrderStartDate = new DateTime(2013,08,01), OrderEndDate = new DateTime (2013,09,06), DaysInBtForOrder = 37, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 23, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", UnitID = 1,Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 73, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(205), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(215), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(204), OrderEndDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(216), Responsible= "mter",Manager="dsto", Purpose = "Meeting",DaysInBtForOrder = 18,UnitID = 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-3), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 34, LocationID = 3, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 74, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(2), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(5), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(1), OrderEndDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(6), DaysInBtForOrder = 6,Responsible= "mter", LastCRUDedBy = "ncru",UnitID = 1, LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 35, LocationID = 4, Comment = "confirmed and reported" },
                new BusinessTrip { BusinessTripID = 75, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(5), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(35), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(4), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(36), DaysInBtForOrder = 33, Responsible= "mter",LastCRUDedBy = "ncru", UnitID = 1,LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-15), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 36, LocationID = 5, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                //new BusinessTrip { BusinessTripID = 76, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), EndDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(39), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(39), DaysInBtForOrder = 10, LastCRUDedBy = "ncru", LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-9), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 33, LocationID = 2, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 77, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(39), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(29), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(40), DaysInBtForOrder = 12, Responsible= "mter", LastCRUDedBy = "ncru",UnitID = 1, LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-9), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 43, LocationID = 3, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 78, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(10), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(20), OrderStartDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(09), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(21), DaysInBtForOrder = 12,LastCRUDedBy = "ncru", Responsible= "mter",UnitID = 1,LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 45, LocationID = 4, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 79, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(250), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(199), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(251), DaysInBtForOrder = 53, Responsible= "mter",LastCRUDedBy = "ncru", UnitID = 1,LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(39), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 66, LocationID = 5, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 80, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(35), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(45), OrderStartDate =DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(34), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(46), DaysInBtForOrder = 13, LastCRUDedBy = "ncru",Responsible= "mter",UnitID = 1, LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-10), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 25, LocationID = 5, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 81, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-275), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-265), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-276), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-264), DaysInBtForOrder = 13, LastCRUDedBy = "ncru",Responsible= "mter",UnitID = 1, LastCRUDTimestamp = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-385), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 26, LocationID = 5, Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 82, StartDate = new DateTime(2013,02,25), EndDate = new DateTime (2013,03,01), OrderStartDate = new DateTime(2013,02,24), OrderEndDate = new DateTime (2013,03,02), DaysInBtForOrder = 6, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 26, LocationID = 5, UnitID = 1, Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 83, StartDate = new DateTime(2013,12,29), EndDate = new DateTime (2013,12,30), OrderStartDate = new DateTime(2013,12,28), OrderEndDate = new DateTime (2013,12,31), DaysInBtForOrder = 3, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 26, LocationID = 5, UnitID = 1, Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 84, StartDate = new DateTime(2013,06,02), EndDate = new DateTime (2013,06,07), OrderStartDate = new DateTime(2013,06,01), OrderEndDate = new DateTime (2013,06,08), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 25, LocationID = 5, UnitID = 1, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 85, StartDate = new DateTime(2014,02,27), EndDate = new DateTime (2014,03,01), OrderStartDate = new DateTime(2014,02,26), OrderEndDate = new DateTime (2014,03,02), DaysInBtForOrder = 5, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, UnitID = 2, Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 86, StartDate = new DateTime(2014,01,15), EndDate = new DateTime (2014,01,20), OrderStartDate = new DateTime(2014,01,14), OrderEndDate = new DateTime (2014,01,31), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, UnitID = 3, LocationID = 1,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 87, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,04), OrderStartDate = new DateTime(2014,11,29), OrderEndDate = new DateTime (2014,12,06), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed, EmployeeID = 22, LocationID = 5,  UnitID = 3, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
            
                new BusinessTrip { BusinessTripID = 89, StartDate = new DateTime(2015,01,02), EndDate = new DateTime (2015,01,06), OrderStartDate = new DateTime(2015,01,01), OrderEndDate = new DateTime (2015,01,18), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, UnitID = 1, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 90, StartDate = new DateTime(2015,01,06), EndDate = new DateTime (2015,01,12), OrderStartDate = new DateTime(2015,01,01), OrderEndDate = new DateTime (2015,01,18), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 2, UnitID = 2, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 91, StartDate = new DateTime(2015,01,12), EndDate = new DateTime (2015,01,17), OrderStartDate = new DateTime(2015,01,01), OrderEndDate = new DateTime (2015,01,18), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 3, UnitID = 3, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 92, StartDate = new DateTime(2014,01,20), EndDate = new DateTime (2014,01,30), OrderStartDate = new DateTime(2014,01,14), OrderEndDate = new DateTime (2014,01,31), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, UnitID = 3, LocationID = 1,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },                 

                new BusinessTrip { BusinessTripID = 93, StartDate = new DateTime(2014,03,02), EndDate = new DateTime (2014,03,10), OrderStartDate = new DateTime(2014,03,01), OrderEndDate = new DateTime (2014,03,20), DaysInBtForOrder = 20, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed, EmployeeID = 57, LocationID = 3, UnitID = 3, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 94, StartDate = new DateTime(2014,03,10), EndDate = new DateTime (2014,03,19), OrderStartDate = new DateTime(2014,03,01), OrderEndDate = new DateTime (2014,03,20), DaysInBtForOrder = 20, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed, EmployeeID = 57, UnitID = 3, LocationID = 1,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },                 
               // new BusinessTrip { BusinessTripID = 194, StartDate = new DateTime(2014,02,10), EndDate = new DateTime (2014,02,23), DaysInBtForOrder = 20, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, UnitID = 3, LocationID = 1,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },                 
                new BusinessTrip { BusinessTripID = 96, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,04), OrderStartDate = new DateTime(2014,11,29), OrderEndDate = new DateTime (2014,12,06), DaysInBtForOrder = 20, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 57, LocationID = 3, UnitID = 3, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 95, StartDate = new DateTime(2014,12,04), EndDate = new DateTime (2014,12,05), OrderStartDate = new DateTime(2014,11,29), OrderEndDate = new DateTime (2014,12,06), DaysInBtForOrder = 20, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 57, LocationID = 3, UnitID = 3, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 

                new BusinessTrip { BusinessTripID = 130, StartDate = new DateTime(2012,11,01), EndDate = new DateTime (2012,12,30), OrderStartDate = new DateTime(2013,10,31), OrderEndDate = new DateTime (2013,12,31), DaysInBtForOrder = 62, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2015,12,30), Status= BTStatus.Reported, EmployeeID = 22, LocationID = 1, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter" },

                new BusinessTrip { BusinessTripID = 131, StartDate = new DateTime(2012,11,01), EndDate = new DateTime (2012,12,30), OrderStartDate = new DateTime(2012,10,31), OrderEndDate = new DateTime (2012,12,31), DaysInBtForOrder = 62, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2015,12,30), Status=BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 39, LocationID = 1, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter" },
                new BusinessTrip { BusinessTripID = 132, StartDate = new DateTime(2014,11,01), EndDate = new DateTime (2014,12,30), OrderStartDate = new DateTime(2014,10,31), OrderEndDate = new DateTime (2014,12,31), DaysInBtForOrder = 62, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2015,12,30), Status=BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 39, LocationID = 1, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter" },
                new BusinessTrip { BusinessTripID = 133, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-1), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(10), OrderStartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(-2), OrderEndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().Date.AddDays(11), DaysInBtForOrder = 62, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime (2015,12,30), Status=BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 19, LocationID = 1, Manager="ncru", UnitID = 1,Purpose = "Meeting",Responsible= "mter" },
                new BusinessTrip { BusinessTripID = 134, StartDate = new DateTime(2014,02,18), EndDate = new DateTime (2014,02,20), OrderStartDate = new DateTime(2014,01,14), OrderEndDate = new DateTime (2014,01,31), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, UnitID = 3, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 135, StartDate = new DateTime(2014,06,18), EndDate = new DateTime (2014,06,20), OrderStartDate = new DateTime(2014,06,14), OrderEndDate = new DateTime (2014,06,30), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 55, UnitID = 3, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 136, StartDate = new DateTime(2014,09,18), EndDate = new DateTime (2014,09,20), OrderStartDate = new DateTime(2014,06,14), OrderEndDate = new DateTime (2014,06,30), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 55, UnitID = 3, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 137, StartDate = new DateTime(2014,09,18), EndDate = new DateTime (2014,09,20), OrderStartDate = new DateTime(2014,09,17), OrderEndDate = new DateTime (2014,09,21), DaysInBtForOrder = 4, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2014,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 55, UnitID = 3, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" }, 
                new BusinessTrip { BusinessTripID = 138, StartDate = new DateTime(2013,01,20).ToLocalTimeAzure(), EndDate = new DateTime (2013,01,26).ToLocalTimeAzure(), OrderStartDate = new DateTime(2013,01,19).ToLocalTimeAzure(), OrderEndDate = new DateTime (2013,01,27).ToLocalTimeAzure(), DaysInBtForOrder = 8, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2015,01,31), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 68, UnitID = 3, LocationID = 2,Responsible= "mter", Comment = "confirmed and reported", Manager = "khal", Purpose = "meeting" }, 
                
//new BusinessTrip { BusinessTripID = 88, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 89, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 90, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 91, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 92, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 93, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 94, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 95, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 96, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 97, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 99, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 101, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 102, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 103, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 104, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 105, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 106, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 107, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 108, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 109, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 110, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 111, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
                //new BusinessTrip { BusinessTripID = 112, StartDate = new DateTime(2014,01,02), EndDate = new DateTime (2014,01,08), OrderStartDate = new DateTime(2014,01,01), OrderEndDate = new DateTime (2014,01,09), DaysInBtForOrder = 9, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2013,09,21), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 22, LocationID = 5, Responsible= "mter", Comment = "Bt for employee", Manager = "khal", Purpose = "meeting" }, 
            
            
            };
            #endregion

            #region List<Journey>
            List<Journey> journeys = new List<Journey>
                {
                    new Journey { JourneyID = 1, BusinessTripID = 6, Date = new DateTime (2013, 08, 31), DayOff = true, ReclaimDate = new DateTime(2013, 10, 31)},
                    new Journey { JourneyID = 2, BusinessTripID = 6, Date = new DateTime (2013, 12, 03), DayOff = false},
                    new Journey { JourneyID = 3, BusinessTripID = 8, Date = new DateTime (2013, 08, 31), DayOff = true},
                    new Journey { JourneyID = 4, BusinessTripID = 8, Date = new DateTime (2013, 10, 03), DayOff = false},
                    new Journey { JourneyID = 5, BusinessTripID = 9, Date = new DateTime (2013, 09, 30), DayOff = false},
                    new Journey { JourneyID = 6, BusinessTripID = 9, Date = new DateTime (2013, 11, 13), DayOff = false},
                    new Journey { JourneyID = 7, BusinessTripID = 10, Date = new DateTime (2013, 08, 20), DayOff = false},
                    new Journey { JourneyID = 8, BusinessTripID = 10, Date = new DateTime (2013, 11, 13), DayOff = false},
                    new Journey { JourneyID = 9, BusinessTripID = 11, Date = new DateTime (2013, 08, 31), DayOff = true},
                    new Journey { JourneyID = 10, BusinessTripID = 11, Date = new DateTime (2013, 12, 03), DayOff = false},
                    new Journey { JourneyID = 11, BusinessTripID = 13, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 12, BusinessTripID = 13, Date = new DateTime (2013, 12, 31), DayOff = false},
                    new Journey { JourneyID = 13, BusinessTripID = 16, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 14, BusinessTripID = 16, Date = new DateTime (2013, 11, 03), DayOff = true},
                    new Journey { JourneyID = 15, BusinessTripID = 19, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 16, BusinessTripID = 19, Date = new DateTime (2013, 11, 01), DayOff = false},
                    new Journey { JourneyID = 17, BusinessTripID = 21, Date = new DateTime (2013, 11, 30), DayOff = true},
                    new Journey { JourneyID = 18, BusinessTripID = 21, Date = new DateTime (2013, 12, 13), DayOff = false},
                    new Journey { JourneyID = 19, BusinessTripID = 22, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 20, BusinessTripID = 22, Date = new DateTime (2013, 11, 12), DayOff = false},
                    new Journey { JourneyID = 21, BusinessTripID = 27, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 22, BusinessTripID = 27, Date = new DateTime (2013, 11, 03), DayOff = false},
                    new Journey { JourneyID = 23, BusinessTripID = 28, Date = new DateTime (2013, 12, 03), DayOff = false},
                    new Journey { JourneyID = 24, BusinessTripID = 28, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 25, BusinessTripID = 29, Date = new DateTime (2013, 12, 02), DayOff = false},
                    new Journey { JourneyID = 26, BusinessTripID = 29, Date = new DateTime (2013, 10, 31), DayOff = false},
                    new Journey { JourneyID = 27, BusinessTripID = 31, Date = new DateTime (2012, 11, 30), DayOff = false},
                    new Journey { JourneyID = 28, BusinessTripID = 31, Date = new DateTime (2012, 12, 13), DayOff = false},
                    new Journey { JourneyID = 29, BusinessTripID = 32, Date = new DateTime (2014, 11, 30), DayOff = true},
                    new Journey { JourneyID = 30, BusinessTripID = 32, Date = new DateTime (2014, 12, 11), DayOff = false},
                    //new Journey { JourneyID = 31, BusinessTripID = 33, Date = new DateTime (2014, 11, 01), DayOff = true},
                    ////new Journey { JourneyID = 32, BusinessTripID = 33, Date = new DateTime (2014, 12, 11), DayOff = false},
                    new Journey { JourneyID = 33, BusinessTripID = 37, Date = new DateTime (2013, 11, 30), DayOff = true},
                    new Journey { JourneyID = 34, BusinessTripID = 37, Date = new DateTime (2013, 12, 26), DayOff = false},
                    new Journey { JourneyID = 35, BusinessTripID = 38, Date = new DateTime (2014, 12, 26), DayOff = false},
                    new Journey { JourneyID = 36, BusinessTripID = 38, Date = new DateTime (2014, 11, 30), DayOff = true},
                    new Journey { JourneyID = 37, BusinessTripID = 40, Date = new DateTime (2013, 08, 31), DayOff = true},
                    new Journey { JourneyID = 38, BusinessTripID = 40, Date = new DateTime (2013, 09, 26), DayOff = false},
                    new Journey { JourneyID = 39, BusinessTripID = 41, Date = new DateTime (2014, 08, 31), DayOff = true,  ReclaimDate = new DateTime(2013, 10, 31)},
                    new Journey { JourneyID = 40, BusinessTripID = 41, Date = new DateTime (2014, 09, 28), DayOff = true},
                    new Journey { JourneyID = 41, BusinessTripID = 44, Date = new DateTime (2013, 09, 09), DayOff = false},
                    new Journey { JourneyID = 42, BusinessTripID = 44, Date = new DateTime (2013, 09, 11), DayOff = false},
                    new Journey { JourneyID = 43, BusinessTripID = 45, Date = new DateTime (2014, 01, 08), DayOff = false},
                    new Journey { JourneyID = 44, BusinessTripID = 45, Date = new DateTime (2014, 04, 09), DayOff = false},
                    new Journey { JourneyID = 45, BusinessTripID = 48, Date = new DateTime (2013, 01, 01), DayOff = true,  ReclaimDate = new DateTime(2013, 10, 31)},
                    new Journey { JourneyID = 46, BusinessTripID = 48, Date = new DateTime (2013, 03, 12), DayOff = false},
                    new Journey { JourneyID = 47, BusinessTripID = 48, Date = new DateTime (2013, 03, 11), DayOff = false},
                    new Journey { JourneyID = 48, BusinessTripID = 49, Date = new DateTime (2013, 04, 25), DayOff = false},
                    new Journey { JourneyID = 49, BusinessTripID = 49, Date = new DateTime (2013, 05, 12), DayOff = true,  ReclaimDate = new DateTime(2013, 10, 31)},
                    new Journey { JourneyID = 50, BusinessTripID = 50, Date = new DateTime (2014, 01, 01), DayOff = true},
                    new Journey { JourneyID = 51, BusinessTripID = 50, Date = new DateTime (2014, 01, 04), DayOff = true},
                    new Journey { JourneyID = 52, BusinessTripID = 52, Date = new DateTime (2013, 09, 01), DayOff = true},
                    new Journey { JourneyID = 53, BusinessTripID = 52, Date = new DateTime (2013, 09, 04), DayOff = false},
                    new Journey { JourneyID = 54, BusinessTripID = 53, Date = new DateTime (2013, 09, 30), DayOff = false},
                    new Journey { JourneyID = 55, BusinessTripID = 53, Date = new DateTime (2013, 10, 04), DayOff = false},
                    new Journey { JourneyID = 56, BusinessTripID = 53, Date = new DateTime (2013, 10, 03), DayOff = false},
                    new Journey { JourneyID = 57, BusinessTripID = 54, Date = new DateTime (2014, 11, 10), DayOff = false},
                    new Journey { JourneyID = 58, BusinessTripID = 54, Date = new DateTime (2014, 12, 14), DayOff = true},
                    new Journey { JourneyID = 59, BusinessTripID = 54, Date = new DateTime (2014, 12, 13), DayOff = true},   
                    new Journey { JourneyID = 60, BusinessTripID = 58, Date = new DateTime (2013, 02, 01), DayOff = false},
                    new Journey { JourneyID = 61, BusinessTripID = 58, Date = new DateTime (2013, 02, 06), DayOff = false},
                    new Journey { JourneyID = 62, BusinessTripID = 59, Date = new DateTime (2013, 02, 09), DayOff = true},
                    new Journey { JourneyID = 63, BusinessTripID = 59, Date = new DateTime (2013, 02, 10), DayOff = true},
                    new Journey { JourneyID = 64, BusinessTripID = 59, Date = new DateTime (2013, 02, 11), DayOff = false},
                    new Journey { JourneyID = 65, BusinessTripID = 59, Date = new DateTime (2013, 02, 12), DayOff = false},
                    new Journey { JourneyID = 66, BusinessTripID = 59, Date = new DateTime (2013, 03, 09), DayOff = false},
                    new Journey { JourneyID = 67, BusinessTripID = 60, Date = new DateTime (2013, 02, 01), DayOff = false},
                    new Journey { JourneyID = 68, BusinessTripID = 60, Date = new DateTime (2013, 03, 06), DayOff = false},
                    new Journey { JourneyID = 69, BusinessTripID = 61, Date = new DateTime (2013, 03, 14), DayOff = false},
                    new Journey { JourneyID = 70, BusinessTripID = 61, Date = new DateTime (2013, 04, 21), DayOff = true},
                    new Journey { JourneyID = 71, BusinessTripID = 66, Date = new DateTime (2013, 03, 03), DayOff = true,  ReclaimDate = new DateTime(2013, 10, 31)},
                    new Journey { JourneyID = 72, BusinessTripID = 66, Date = new DateTime (2013, 03, 08), DayOff = true,  ReclaimDate = new DateTime(2013, 07, 31)},
                    new Journey { JourneyID = 73, BusinessTripID = 67, Date = new DateTime (2013, 04, 09), DayOff = false},
                    new Journey { JourneyID = 74, BusinessTripID = 67, Date = new DateTime (2013, 04, 26), DayOff = false},
                    new Journey { JourneyID = 75, BusinessTripID = 68, Date = new DateTime (2013, 05, 01), DayOff = true,  ReclaimDate = new DateTime(2013, 07, 31)},
                    new Journey { JourneyID = 76, BusinessTripID = 68, Date = new DateTime (2013, 06, 06), DayOff = false},
                    new Journey { JourneyID = 77, BusinessTripID = 71, Date = new DateTime (2013, 07, 09), DayOff = true},
                    new Journey { JourneyID = 78, BusinessTripID = 71, Date = new DateTime (2013, 07, 27), DayOff = true},
                    new Journey { JourneyID = 79, BusinessTripID = 72, Date = new DateTime (2013, 08, 01), DayOff = false},
                    new Journey { JourneyID = 80, BusinessTripID = 72, Date = new DateTime (2013, 09, 06), DayOff = false}, 
                    new Journey { JourneyID = 81, BusinessTripID = 138, Date = new DateTime (2013, 01, 19).ToLocalTimeAzure(), DayOff = true, ReclaimDate = new DateTime(2013, 02, 02).ToLocalTimeAzure()},
                    new Journey { JourneyID = 82, BusinessTripID = 138, Date = new DateTime (2013, 01, 27).ToLocalTimeAzure(), DayOff = true}, 

                };
            #endregion

            #region List<Country>
            List<Country> countries = new List<Country>
                {
                    new Country { CountryID = 1, CountryName = "Ukraine", Holidays = new List<Holiday>(), Locations = new List<Location>(), Comment = "UTC + 2"},
                    new Country { CountryID = 2, CountryName = "Poland", Holidays = new List<Holiday>(), Locations = new List<Location>(), Comment = "UTC + 1"},
                    new Country { CountryID = 3, CountryName = "Sweden", Holidays = new List<Holiday>(), Locations = new List<Location>(), Comment = "UTC + 1"},
                    new Country { CountryID = 4, CountryName = "Belarus", Holidays = new List<Holiday>(), Locations = new List<Location>(), Comment = "UTC + 3"}
                };
            #endregion

            #region List<Holiday>
            List<Holiday> holidays = new List<Holiday>
                {
                    //Ukraine 2013
                    new Holiday { HolidayID = 1, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 2, Title = "Christmas", HolidayDate = new DateTime(2013, 01, 07), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 3, Title = "Woman's day", HolidayDate = new DateTime(2013, 03, 08), CountryID = 1, IsPostponed = true},
                    new Holiday { HolidayID = 4, Title = "Easter day", HolidayDate = new DateTime(2013, 05, 05), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 5, Title = "May 1", HolidayDate = new DateTime(2013, 05, 01), CountryID = 1, IsPostponed = true},
                    new Holiday { HolidayID = 6, Title = "May 2", HolidayDate = new DateTime(2013, 05, 02), CountryID = 1, IsPostponed = true},
                    new Holiday { HolidayID = 7, Title = "Victory Day", HolidayDate = new DateTime(2013, 05, 09), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 8, Title = "Green Holiday", HolidayDate = new DateTime(2013, 06, 23), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 9, Title = "Constitution Day", HolidayDate = new DateTime(2013, 06, 28), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 10, Title = "Independence Day", HolidayDate = new DateTime(2013, 08, 24), CountryID = 1, IsPostponed = true},
                    //Poland 2013
                    new Holiday { HolidayID = 11, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 2, IsPostponed = true},
                    new Holiday { HolidayID = 12, Title = "Epiphany", HolidayDate = new DateTime(2013, 01, 06), CountryID = 2, IsPostponed = true},
                    new Holiday { HolidayID = 13, Title = "Easter Monday", HolidayDate = new DateTime(2013, 04, 21), CountryID = 2, IsPostponed = true},
                    //Sweden 2013
                    new Holiday { HolidayID = 14, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 3, IsPostponed = true},
                    new Holiday { HolidayID = 15, Title = "Epiphany", HolidayDate = new DateTime(2013, 01, 06), CountryID = 3, IsPostponed = true},
                    new Holiday { HolidayID = 16, Title = "Easter Monday", HolidayDate = new DateTime(2013, 04, 21), CountryID = 3, IsPostponed = true},
                    //Belarus 2013
                    new Holiday { HolidayID = 17, Title = "NewYear", HolidayDate = new DateTime(2013, 01, 01), CountryID = 4, IsPostponed = true},
                    new Holiday { HolidayID = 18, Title = "Christmas", HolidayDate = new DateTime(2013, 01, 07), CountryID = 4, IsPostponed = true},
                    new Holiday { HolidayID = 19, Title = "Woman's day", HolidayDate = new DateTime(2013, 03, 08), CountryID = 4, IsPostponed = true},

                    //Ukraine 2014
                    new Holiday { HolidayID = 20, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 21, Title = "Christmas", HolidayDate = new DateTime(2014, 01, 07), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 22, Title = "Woman's day", HolidayDate = new DateTime(2014, 03, 08), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 23, Title = "Easter day", HolidayDate = new DateTime(2014, 04, 20), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 24, Title = "May 1", HolidayDate = new DateTime(2014, 05, 01), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 25, Title = "May 2", HolidayDate = new DateTime(2014, 05, 02), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 26, Title = "Victory Day", HolidayDate = new DateTime(2014, 05, 09), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 27, Title = "Green Holiday", HolidayDate = new DateTime(2014, 06, 08), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 28, Title = "Constitution Day", HolidayDate = new DateTime(2014, 06, 28), CountryID = 1, IsPostponed = false},
                    new Holiday { HolidayID = 29, Title = "Independence Day", HolidayDate = new DateTime(2014, 08, 24), CountryID = 1, IsPostponed = false},
                    //Poland 2014
                    new Holiday { HolidayID = 31, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 2, IsPostponed = false},
                    new Holiday { HolidayID = 32, Title = "Epiphany", HolidayDate = new DateTime(2014, 01, 06), CountryID = 2, IsPostponed = false},
                    new Holiday { HolidayID = 33, Title = "Easter Monday", HolidayDate = new DateTime(2014, 04, 21), CountryID = 2, IsPostponed = false},
                    //Sweden 201
                    new Holiday { HolidayID = 34, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 3, IsPostponed = false},
                    new Holiday { HolidayID = 35, Title = "Epiphany", HolidayDate = new DateTime(2014, 01, 06), CountryID = 3, IsPostponed = false},
                    new Holiday { HolidayID = 36, Title = "Easter Monday", HolidayDate = new DateTime(2014, 04, 21), CountryID = 3, IsPostponed = true},
                    //Belarus 2014
                    new Holiday { HolidayID = 37, Title = "NewYear", HolidayDate = new DateTime(2014, 01, 01), CountryID = 4, IsPostponed = false},
                    new Holiday { HolidayID = 38, Title = "Christmas", HolidayDate = new DateTime(2014, 01, 07), CountryID = 4, IsPostponed = false},
                    new Holiday { HolidayID = 39, Title = "Woman's day", HolidayDate = new DateTime(2014, 03, 08), CountryID = 4, IsPostponed = true},
                };
            #endregion

            #region List<CalendarItem>
            List<CalendarItem> calendarItems = new List<CalendarItem>
                {
                    new CalendarItem{CalendarItemID = 2,From = new DateTime(2014,01,25),To=new DateTime(2014,02,05),Type=CalendarItemType.PaidVacation,EmployeeID = 1},
                  //new CalendarItem{CalendarItemID = 3,From = new DateTime(2014,01,01),To=new DateTime(2014,01,01),Type=CalendarItemType.ReclaimedOvertime,EmployeeID = 1},
                    new CalendarItem{CalendarItemID = 4,From = new DateTime(2014,02,01),To=new DateTime(2014,02,14),Type=CalendarItemType.BT,EmployeeID = 25, Location="KR/B"},
                    new CalendarItem{CalendarItemID = 5,From = new DateTime(2014,03,01),To=new DateTime(2014,03,14),Type=CalendarItemType.UnpaidVacation,EmployeeID = 25},
                  //new CalendarItem{CalendarItemID = 7,From = new DateTime(2014,05,09),To=new DateTime(2014,05,09),Type=CalendarItemType.ReclaimedOvertime,EmployeeID = 25},
                    new CalendarItem{CalendarItemID = 8,From = new DateTime(2014,05,09),To=new DateTime(2014,06,09),Type=CalendarItemType.BT,EmployeeID = 2, Location="LD/D"},
                    new CalendarItem{CalendarItemID = 9 ,From = new DateTime(2014,02,24),To = new DateTime(2014,03,01),Type = CalendarItemType.SickAbsence, EmployeeID = 1},
                    new CalendarItem{CalendarItemID = 10 ,From = new DateTime(2014,02,24),To = new DateTime(2014,02,26),Type = CalendarItemType.SickAbsence, EmployeeID = 2},
                    new CalendarItem{CalendarItemID = 11 ,From = new DateTime(2014,03,3),To = new DateTime(2014,03,7),Type = CalendarItemType.SickAbsence, EmployeeID = 2},
                    new CalendarItem{CalendarItemID = 12 ,From = new DateTime(2014,03,10),To = new DateTime(2014,03,14),Type = CalendarItemType.SickAbsence, EmployeeID = 1}, 
                    new CalendarItem{CalendarItemID = 13 ,From = new DateTime(2013,02,02).ToLocalTimeAzure(),To = new DateTime(2013,02,02).ToLocalTimeAzure(),Type = CalendarItemType.ReclaimedOvertime, EmployeeID = 68},
                };
            #endregion

            #region List<Department>
            List<Department> departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "DEPT1",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "DEPT2",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "DEPT3",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "DEPT4",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "DEPT5",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "DEPT6",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "DEPT7",Employees = new List<Employee>()},
                     new Department{DepartmentID = 8, DepartmentName = "BOARD",Employees = new List<Employee>()},
                     new Department{DepartmentID = 9, DepartmentName = "DEPT8",Employees = new List<Employee>()}
            };
            #endregion

            #region List<VisaRegistrationDate>
            List<VisaRegistrationDate> visaRegistrationDates = new List<VisaRegistrationDate>
            {
                new VisaRegistrationDate {EmployeeID=5, RegistrationDate=new DateTime(2012,01,01), City = "Lviv", RegistrationNumber = "11486", RegistrationTime = "08:00",  VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=6, RegistrationDate=new DateTime(2013,10,02), City = "Lviv", VisaType="V_C07"},   
                new VisaRegistrationDate {EmployeeID=7, RegistrationDate=new DateTime(2013,01,01), City = "Lviv", VisaType="V_C07"},
                new VisaRegistrationDate {EmployeeID=15, RegistrationDate=new DateTime(2013,01,04), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=16, RegistrationDate=new DateTime(2016,01,04), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=17, RegistrationDate=new DateTime(2013,01,04), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=24, RegistrationDate=new DateTime(2013,01,04), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=25, RegistrationDate=new DateTime(2013,01,04), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=35, RegistrationDate=new DateTime(2013,01,01), City = "Lviv", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=36, RegistrationDate=new DateTime(2013,10,02), City = "Lviv", VisaType="V_C07"},   
                new VisaRegistrationDate {EmployeeID=48, RegistrationDate=new DateTime(2013,01,01), RegistrationNumber = "114861", RegistrationTime = "09:00", City = "Ivano-Frankivs'k", VisaType="V_C07"},
                new VisaRegistrationDate {EmployeeID=49, RegistrationDate=new DateTime(2013,01,04),  RegistrationNumber = "114862", RegistrationTime = "10:00", City = "Ivano-Frankivs'k", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=55, RegistrationDate=new DateTime(2013,01,01),  RegistrationNumber = "114863", RegistrationTime = "07:00", City = "Ivano-Frankivs'k", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=56, RegistrationDate=new DateTime(2013,10,02),  RegistrationNumber = "114864", RegistrationTime = "11:00", City = "Ivano-Frankivs'k", VisaType="V_C07"},   
                new VisaRegistrationDate {EmployeeID=68, RegistrationDate=new DateTime(2013,01,01),  RegistrationNumber = "1148645", RegistrationTime = "12:00", City = "Ivano-Frankivs'k", VisaType="V_C07"},
                new VisaRegistrationDate {EmployeeID=69, RegistrationDate=new DateTime(2013,01,04), RegistrationNumber = "114865", RegistrationTime = "13:00", City = "Ivano-Frankivs'k", VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=78, RegistrationDate=new DateTime(2013,01,01),VisaType="V_C07"},
                new VisaRegistrationDate {EmployeeID=71, RegistrationDate=new DateTime(2013,01,04),VisaType="V_D08"},
                new VisaRegistrationDate {EmployeeID=2, RegistrationDate=DateTime.Now.ToLocalTimeAzure().Date.AddDays(1),VisaType="V_D08"},
            };
            #endregion

            #region List<Visa>
            List<Visa> visas = new List<Visa>
            {
            	new Visa { EmployeeID = 1, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0,  VisaOf = employees.Find(e => e.EmployeeID == 1) },
            	new Visa { EmployeeID = 2, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(89), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 2) },
            	new Visa { EmployeeID = 3, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(-1), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 3) },
            	new Visa { EmployeeID = 4, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure().Date, Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 4) },
            	new Visa { EmployeeID = 10, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(1), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 10) },
            	new Visa { EmployeeID = 11, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 11) },
            	new Visa { EmployeeID = 12, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(91), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 12) },
            	new Visa { EmployeeID = 13, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 13) },
            	new Visa { EmployeeID = 14, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 14) },
            	new Visa { EmployeeID = 16, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, CorrectionForVisaDays = 0, VisaOf = employees.Find(e => e.EmployeeID == 16) },
            	new Visa { EmployeeID = 18, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 18) },
            	new Visa { EmployeeID = 21, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 21) },
            	new Visa { EmployeeID = 22, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 22) },
            	new Visa { EmployeeID = 23, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 23) },
            	new Visa { EmployeeID = 27, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 33, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 3, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 27) },
                new Visa { EmployeeID = 31, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 31) },
            	new Visa { EmployeeID = 32, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 32) },
            	new Visa { EmployeeID = 33, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 33) },
            	new Visa { EmployeeID = 34, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 34) },
                new Visa { EmployeeID = 35, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 35) },
            	new Visa { EmployeeID = 40, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,01,02), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 40) },
            	new Visa { EmployeeID = 42, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 42) },
            	new Visa { EmployeeID = 43, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,01,02), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 43) },
                new Visa { EmployeeID = 44, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2012,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 10, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 44) },
            	new Visa { EmployeeID = 44, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 19, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 2, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 44) },
            	new Visa { EmployeeID = 51, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 51) },
            	new Visa { EmployeeID = 52, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 11, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 52) },
            	new Visa { EmployeeID = 53, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,01,02), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 53) },
            	new Visa { EmployeeID = 54, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5,  DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 54) },
            	new Visa { EmployeeID = 55, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 8, EntriesUsedInBT = 1, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 55) },
            	new Visa { EmployeeID = 61, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 61) },
            	new Visa { EmployeeID = 62, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 62) },
            	new Visa { EmployeeID = 63, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 63) },
            	new Visa { EmployeeID = 64, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-95), DueDate =DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 64) },
            	new Visa { EmployeeID = 65, VisaType = "V_D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,01,02), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 65) },
                new Visa { EmployeeID = 74, VisaType = "V_C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2012,05,13), Days = 20, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 4, EntriesUsedInBT = 2, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 75) },
            	new Visa { EmployeeID = 75, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 5, DaysUsedInPrivateTrips = 32, CorrectionForVisaDays = 0, Entries = 8, EntriesUsedInBT = 1, EntriesUsedInPrivateTrips = 2, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 75) },
            	new Visa { EmployeeID = 78, VisaType = "V_C07", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-366), DueDate = DateTime.Now.ToLocalTimeAzure(), Days = 180, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0, VisaOf = employees.Find(e => e.EmployeeID == 78) },
                new Visa { EmployeeID = 76, VisaType = "V_D08", StartDate = new DateTime(2010,08,01), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(90), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0,  VisaOf = employees.Find(e => e.EmployeeID == 1) },
                new Visa { EmployeeID = 77, VisaType = "V_D08", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(300), DueDate = DateTime.Now.ToLocalTimeAzure().AddDays(330), Days = 90, DaysUsedInBT = 0, DaysUsedInPrivateTrips = 0, CorrectionForVisaDays = 0, Entries = 0, EntriesUsedInBT = 0, EntriesUsedInPrivateTrips = 0, CorrectionForVisaEntries = 0,  VisaOf = employees.Find(e => e.EmployeeID == 1) },

            };
            #endregion

            #region List<Permit>
            List<Permit> permits = new List<Permit>
            {
                new Permit { EmployeeID = 1, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},
                new Permit { EmployeeID = 2, Number = "01/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 2)},
                new Permit { EmployeeID = 3, Number = "01/2013", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 3)},
                new Permit { EmployeeID = 5, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 5)},
                new Permit { EmployeeID = 12, Number = "01/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 12)},
                new Permit { EmployeeID = 13, Number = "01/2013", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 13)},
                new Permit { EmployeeID = 14, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 14)},
                new Permit { EmployeeID = 15, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 15)},
                new Permit { EmployeeID = 16, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 16)},
                new Permit { EmployeeID = 18, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 18)},
                new Permit { EmployeeID = 21, Number = "01/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 21)},
                new Permit { EmployeeID = 23, Number = "01/2013", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 23)},
                new Permit { EmployeeID = 24, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 24)},
                new Permit { EmployeeID = 32, Number = "01/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 32), ProlongRequestDate = DateTime.Now.ToLocalTimeAzure().AddDays(-15)},
                new Permit { EmployeeID = 33, Number = "01/2013", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 33)},
                new Permit { EmployeeID = 34, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 34)},
                new Permit { EmployeeID = 40, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 40), CancelRequestDate = DateTime.Now.ToLocalTimeAzure().AddDays(-15)},
                new Permit { EmployeeID = 42, Number = "01/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 42)},
                new Permit { EmployeeID = 44, Number = "01/2013", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 44)},
                new Permit { EmployeeID = 45, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 45)},
                new Permit { EmployeeID = 46, Number = "01/2012", StartDate = new DateTime(2011, 01,01), EndDate = new DateTime (2011, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 46)},
                new Permit { EmployeeID = 47, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 47)},
                new Permit { EmployeeID = 48, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 48)},
                new Permit { EmployeeID = 52, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 52)},
                new Permit { EmployeeID = 53, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 53)},
                new Permit { EmployeeID = 54, Number = "04/2012", StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 54)},
                new Permit { EmployeeID = 61, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 61)},
                new Permit { EmployeeID = 62, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 62)},
                new Permit { EmployeeID = 63, Number = "04/2012", StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 63)},
                new Permit { EmployeeID = 64, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 64)},
                new Permit { EmployeeID = 68, Number = "04/2012", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-65), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 68)},
                new Permit { EmployeeID = 22, IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 22)},
                new Permit { EmployeeID = 78, Number = "04/2010", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},
                new Permit { EmployeeID = 80, Number = "04/2015", StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(100), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(200), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},


                };
            #endregion

            #region List<Passport>
            List<Passport> passports = new List<Passport>
                {
                    new Passport{EmployeeID=1},
                    new Passport{EmployeeID=2},
                    new Passport{EmployeeID=3},
                    new Passport{EmployeeID=4},
                    new Passport{EmployeeID=5},
                    new Passport{EmployeeID=6},
                    new Passport{EmployeeID=7},
                    new Passport{EmployeeID=8},
                    new Passport{EmployeeID=9},
                    new Passport{EmployeeID=10},
                    new Passport{EmployeeID=11},
                    new Passport{EmployeeID=12},
                    new Passport{EmployeeID=13},
                    new Passport{EmployeeID=14},
                    new Passport{EmployeeID=15},
                    new Passport{EmployeeID=16},
                    new Passport{EmployeeID=17},
                    new Passport{EmployeeID=18},
                    new Passport{EmployeeID=19},
                    new Passport{EmployeeID=20},
                    new Passport{EmployeeID=21},
                    new Passport{EmployeeID=22},
                    new Passport{EmployeeID=23},
                    new Passport{EmployeeID=31},
                    new Passport{EmployeeID=32},
                    new Passport{EmployeeID=33},
                    new Passport{EmployeeID=34},
                    new Passport{EmployeeID=35},
                    new Passport{EmployeeID=36},
                    new Passport{EmployeeID=37},
                    new Passport{EmployeeID=38},
                    new Passport{EmployeeID=39, EndDate = DateTime.Now.AddDays(5).ToLocalTimeAzure()},
                    new Passport{EmployeeID=40},
                    new Passport{EmployeeID=41},
                    new Passport{EmployeeID=42},
                    new Passport{EmployeeID=43},
                    new Passport{EmployeeID=44},
                    new Passport{EmployeeID=45},
                    new Passport{EmployeeID=46},
                    new Passport{EmployeeID=47},
                    new Passport{EmployeeID=48},
                    new Passport{EmployeeID=49},
                    new Passport{EmployeeID=50},
                    new Passport{EmployeeID=51},
                    new Passport{EmployeeID=52},
                    new Passport{EmployeeID=53},
                    new Passport{EmployeeID=54},
                    new Passport{EmployeeID=55},
                    new Passport{EmployeeID=56},
                    new Passport{EmployeeID=57},
                    new Passport{EmployeeID=58},
                    new Passport{EmployeeID=59},
                    new Passport{EmployeeID=60},
                    new Passport{EmployeeID=61},
                    new Passport{EmployeeID=62},
                    new Passport{EmployeeID=63},
                    new Passport{EmployeeID=64},
                    new Passport{EmployeeID=65},
                    new Passport{EmployeeID=66},
                    new Passport{EmployeeID=67},
                    new Passport{EmployeeID=68},
                    new Passport{EmployeeID=69},
                    new Passport{EmployeeID=70},
                    new Passport{EmployeeID=71},
                    new Passport{EmployeeID=72},
                    new Passport{EmployeeID=73},
                    new Passport{EmployeeID=74},
                    new Passport{EmployeeID=75, EndDate = DateTime.Now.AddDays(5).ToLocalTimeAzure()},
                    new Passport{EmployeeID=76, EndDate = DateTime.Now.AddDays(5).ToLocalTimeAzure()},
                    new Passport{EmployeeID=77},
                    new Passport{EmployeeID=78},
                    new Passport{EmployeeID=79},
                    new Passport{EmployeeID=80}

                };
            #endregion

            #region List<PrivateTrip>
            List<PrivateTrip> privateTrips = new List<PrivateTrip>
                {
                new PrivateTrip{ PrivateTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-190), EmployeeID = 52 },
                new PrivateTrip{ PrivateTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-15), EmployeeID = 75 },
                new PrivateTrip{ PrivateTripID = 3, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-75), EmployeeID = 75 },
                new PrivateTrip{ PrivateTripID = 4, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-90), EmployeeID = 27 },
                new PrivateTrip{ PrivateTripID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-70), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-60), EmployeeID = 27 },
                new PrivateTrip{ PrivateTripID = 6, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-30), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EmployeeID = 27 },
                new PrivateTrip{ PrivateTripID = 7, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-100), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-90), EmployeeID = 44 },
                new PrivateTrip{ PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-20), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-13), EmployeeID = 44 },
                };
            #endregion

            #region List<Position>
            List<Position> positions = new List<Position>
                {
                new Position {PositionID = 1, TitleEn = "Employee", TitleUk = "Працівник" },
                new Position {PositionID = 2, TitleEn = "Software developer", TitleUk = "Розробник програмного забезпечення"}
                };
            #endregion

            #region List<Overtime>
            List<Overtime> overtimes = new List<Overtime>
                {
                    new Overtime{OvertimeID = 1, EmployeeID = 1,Date = new DateTime(2013,01,01), ReclaimDate = new DateTime(2013,02,02), DayOff = true, Type = OvertimeType.Overtime},
                    new Overtime{OvertimeID = 2, EmployeeID = 2,Date = new DateTime(2013,01,02), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 3, EmployeeID = 3,Date = new DateTime(2013,01,03), ReclaimDate = new DateTime(2013,03,10), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 4, EmployeeID = 4,Date = new DateTime(2013,01,04), ReclaimDate = new DateTime(2013,01,02), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 5, EmployeeID = 5,Date = new DateTime(2013,01,05), ReclaimDate = new DateTime(2013,05,02), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 6, EmployeeID = 6,Date = new DateTime(2013,01,14), ReclaimDate = new DateTime(2013,06,02), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 7, EmployeeID = 7,Date = new DateTime(2013,01,13), ReclaimDate = new DateTime(2013,07,02), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 8, EmployeeID = 8,Date = new DateTime(2013,01,12), ReclaimDate = new DateTime(2013,08,02), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 9, EmployeeID = 9,Date = new DateTime(2013,01,11), ReclaimDate = new DateTime(2013,09,02), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 10, EmployeeID = 10,Date = new DateTime(2013,01,10),DayOff = false, Type = OvertimeType.Overtime },

                    new Overtime{OvertimeID = 11, EmployeeID = 11,Date = new DateTime(2013,02,11), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 12, EmployeeID = 12,Date = new DateTime(2013,02,21), ReclaimDate = new DateTime(2013,02,02), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 13, EmployeeID = 13,Date = new DateTime(2013,02,22), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 14, EmployeeID = 14,Date = new DateTime(2013,02,14), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 15, EmployeeID = 15,Date = new DateTime(2013,02,15), DayOff = false, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 16, EmployeeID = 16,Date = new DateTime(2013,02,16), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 17, EmployeeID = 17,Date = new DateTime(2013,02,17), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 18, EmployeeID = 18,Date = new DateTime(2013,02,28), DayOff = false, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 19, EmployeeID = 19,Date = new DateTime(2013,02,28), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 20, EmployeeID = 20,Date = new DateTime(2013,02,27), ReclaimDate = new DateTime(2013,02,02), DayOff = true, Type = OvertimeType.Overtime },

                    new Overtime{OvertimeID = 21, EmployeeID = 21,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 22, EmployeeID = 32,Date = new DateTime(2014,03,02), DayOff = true, Type = OvertimeType.Overtime, ReclaimDate = new DateTime(2013,04,02) },
                    new Overtime{OvertimeID = 23, EmployeeID = 23,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 24, EmployeeID = 24,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 25, EmployeeID = 25,Date = new DateTime(2014,02,23), DayOff = false, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 26, EmployeeID = 26,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 27, EmployeeID = 27,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 28, EmployeeID = 28,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 29, EmployeeID = 29,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 30, EmployeeID = 30,Date = new DateTime(2014,03,02), DayOff = false, Type = OvertimeType.Paid },

                    new Overtime{OvertimeID = 31, EmployeeID = 31,Date = new DateTime(2014,11,10), ReclaimDate = new DateTime(2014,12,02), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 32, EmployeeID = 32,Date = new DateTime(2014,11,09), ReclaimDate = new DateTime(2014,12,12), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 33, EmployeeID = 33,Date = new DateTime(2014,11,08), ReclaimDate = new DateTime(2014,12,03), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 34, EmployeeID = 34,Date = new DateTime(2014,11,07), ReclaimDate = new DateTime(2014,12,13), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 35, EmployeeID = 35,Date = new DateTime(2014,11,06), ReclaimDate = new DateTime(2014,12,04), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 36, EmployeeID = 36,Date = DateTime.Now.ToLocalTimeAzure().AddDays(50), ReclaimDate = new DateTime(2014,12,14), DayOff = true, Type = OvertimeType.Overtime },
                    //new Overtime{OvertimeID = 37, EmployeeID = 37,Date = new DateTime(2014,11,04), ReclaimDate = new DateTime(2014,12,05), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 38, EmployeeID = 38,Date = new DateTime(2014,11,03), ReclaimDate = new DateTime(2014,12,15), DayOff = true, Type = OvertimeType.Overtime },
                  //  new Overtime{OvertimeID = 39, EmployeeID = 39,Date = new DateTime(2014,11,02), ReclaimDate = new DateTime(2014,12,06), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 40, EmployeeID = 40,Date = new DateTime(2014,11,01), ReclaimDate = new DateTime(2014,12,16), DayOff = true, Type = OvertimeType.Overtime },

                    new Overtime{OvertimeID = 41, EmployeeID = 41,Date = new DateTime(2013,01,01), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 42, EmployeeID = 42,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 43, EmployeeID = 43,Date = new DateTime(2013,01,01), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 44, EmployeeID = 44,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 45, EmployeeID = 45,Date = new DateTime(2013,01,01), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 46, EmployeeID = 46,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 47, EmployeeID = 47,Date = new DateTime(2013,01,01), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 48, EmployeeID = 48,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 49, EmployeeID = 49,Date = new DateTime(2013,01,01), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 50, EmployeeID = 50,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Overtime },

                    new Overtime{OvertimeID = 51, EmployeeID = 51,Date = new DateTime(2013,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 52, EmployeeID = 52,Date = new DateTime(2013,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 53, EmployeeID = 53,Date = new DateTime(2013,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 54, EmployeeID = 54,Date = new DateTime(2013,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 55, EmployeeID = 55,Date = new DateTime(2013,03,22), ReclaimDate = new DateTime(2014,03,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 56, EmployeeID = 56,Date = new DateTime(2014,04,03), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 57, EmployeeID = 57,Date = new DateTime(2014,04,04), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 58, EmployeeID = 58,Date = new DateTime(2014,04,05), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 59, EmployeeID = 59,Date = new DateTime(2014,04,06), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 60, EmployeeID = 60,Date = new DateTime(2014,04,07), DayOff = false, Type = OvertimeType.Overtime },

                    new Overtime{OvertimeID = 61, EmployeeID = 61,Date = new DateTime(2013,04,01), ReclaimDate = new DateTime(2013,05,02), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 62, EmployeeID = 62,Date = new DateTime(2014,03,20), ReclaimDate = new DateTime(2014,04,03), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 63, EmployeeID = 63,Date = new DateTime(2013,04,02), ReclaimDate = new DateTime(2013,05,04), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 64, EmployeeID = 64,Date = new DateTime(2014,03,21), ReclaimDate = new DateTime(2014,04,05), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 65, EmployeeID = 65,Date = new DateTime(2013,04,03), ReclaimDate = new DateTime(2013,05,06), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 66, EmployeeID = 66,Date = new DateTime(2014,03,22), ReclaimDate = new DateTime(2014,04,07), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 67, EmployeeID = 67,Date = new DateTime(2013,04,04), ReclaimDate = new DateTime(2013,05,08), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 68, EmployeeID = 68,Date = new DateTime(2014,03,23), ReclaimDate = new DateTime(2014,04,08), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 69, EmployeeID = 69,Date = new DateTime(2013,04,05), ReclaimDate = new DateTime(2013,05,09), DayOff = true, Type = OvertimeType.Paid },
                    new Overtime{OvertimeID = 70, EmployeeID = 70,Date = new DateTime(2014,03,24), ReclaimDate = new DateTime(2014,04,10), DayOff = true, Type = OvertimeType.Paid },

                    new Overtime{OvertimeID = 71, EmployeeID = 11,Date = new DateTime(2013,01,01),  DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 72, EmployeeID = 12,Date = new DateTime(2013,06,07), ReclaimDate = new DateTime(2013,07,30), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 73, EmployeeID = 13,Date = new DateTime(2014,06,07), ReclaimDate = new DateTime(2014,07,30), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 74, EmployeeID = 14,Date = new DateTime(2014,07,01), ReclaimDate = new DateTime(2014,12,21), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 75, EmployeeID = 15,Date = new DateTime(2014,06,02), ReclaimDate = new DateTime(2014,11,22), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 76, EmployeeID = 16,Date = new DateTime(2014,05,03), DayOff = false, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 77, EmployeeID = 17,Date = new DateTime(2014,04,04), ReclaimDate = new DateTime(2014,10,23), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 78, EmployeeID = 18,Date = new DateTime(2014,03,05), ReclaimDate = new DateTime(2014,09,24), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 79, EmployeeID = 19,Date = new DateTime(2014,02,06), ReclaimDate = new DateTime(2014,08,25), DayOff = true, Type = OvertimeType.Private },
                    new Overtime{OvertimeID = 80, EmployeeID = 20,Date = new DateTime(2014,01,07), DayOff = false, Type = OvertimeType.Overtime },
                    
                    new Overtime{OvertimeID = 81, EmployeeID = 22,Date = new DateTime(2014,03,05), DayOff = true, Type = OvertimeType.Overtime },
                    new Overtime{OvertimeID = 82, EmployeeID = 22,Date = new DateTime(2014,04,05), DayOff = true, Type = OvertimeType.Overtime, ReclaimDate = new DateTime(2014,05,05)},
                    new Overtime{OvertimeID = 83, EmployeeID = 22,Date = new DateTime(2014,07,05), DayOff = true, Type = OvertimeType.Paid, ReclaimDate = new DateTime(2014,08,05)},
                    new Overtime{OvertimeID = 84, EmployeeID = 22,Date = new DateTime(2014,06,05), DayOff = true, Type = OvertimeType.Private, ReclaimDate = new DateTime(2014,09,05)}, 

                };
            #endregion

            #region List<Vacation>
            List<Vacation> vacations = new List<Vacation>
                {
                    new Vacation{VacationID = 1, EmployeeID = 1, From = new DateTime(2014,02,12), To = new DateTime(2014,02,20), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 2, EmployeeID = 2, From = new DateTime(2014,03,12), To = new DateTime(2014,03,28), Type = VacationType.UnpaidVacation},
                    new Vacation{VacationID = 3, EmployeeID = 3, From = new DateTime(2014,04,12), To = new DateTime(2014,04,28), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 4, EmployeeID = 4, From = new DateTime(2014,05,12), To = new DateTime(2014,05,28), Type = VacationType.UnpaidVacation},
                    new Vacation{VacationID = 5, EmployeeID = 5, From = new DateTime(2014,06,12), To = new DateTime(2014,06,28), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 6, EmployeeID = 6, From = new DateTime(2014,07,12), To = new DateTime(2014,07,28), Type = VacationType.UnpaidVacation},
                    new Vacation{VacationID = 7, EmployeeID = 7, From = new DateTime(2014,08,12), To = new DateTime(2014,08,28), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 8, EmployeeID = 8, From = new DateTime(2014,09,12), To = new DateTime(2014,09,28), Type = VacationType.UnpaidVacation},
                    new Vacation{VacationID = 9, EmployeeID = 9, From = DateTime.Now.ToLocalTimeAzure().AddDays(10), To = DateTime.Now.ToLocalTimeAzure().AddDays(26), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 10, EmployeeID = 10, From = new DateTime(2014,11,12), To = new DateTime(2014,11,28), Type = VacationType.UnpaidVacation},
                    new Vacation{VacationID = 11, EmployeeID = 22, From = new DateTime(2014,02,01), To = new DateTime(2014,02,10), Type = VacationType.PaidVacation},
                    new Vacation{VacationID = 12, EmployeeID = 22, From = new DateTime(2014,02,13), To = new DateTime(2014,02,20), Type = VacationType.UnpaidVacation},


                };
            #endregion

            #region List<Sickness>
            List<Sickness> sicks = new List<Sickness>
                {
                    new Sickness{EmployeeID = 1,SickID = 1, From = new DateTime(2014,02,24),To = new DateTime(2014,03,01),SicknessType = "ГРЗ"},
                    new Sickness{EmployeeID = 2,SickID = 2, From = new DateTime(2014,02,24),To = new DateTime(2014,02,26),SicknessType = "ГРЗ"},
                    new Sickness{EmployeeID = 2,SickID = 3, From = new DateTime(2014,03,3),To = new DateTime(2014,03,7),SicknessType = "ГРЗ"},
                    new Sickness{EmployeeID = 1,SickID = 4, From = new DateTime(2014,03,10),To = new DateTime(2014,03,14),SicknessType = "ГРЗ"},
                    new Sickness{EmployeeID = 22, SickID = 5, From = new DateTime(2014,09,09), To = new DateTime(2014,09,20), SicknessType = "ABC"}
                };
            #endregion

            #region List<Greeting>
            List<Greeting> greetings = new List<Greeting>
                {
                    new Greeting{GreetingId = 1,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be\n filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 2,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and\n that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 3,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all\n the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 4,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 5,GreetingHeader = "Greeting 5", GreetingBody = "Celebrate your birthday today.\n Celebrate being Happy every day"},
                    new Greeting{GreetingId = 6,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be\n filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 7,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and\n that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 8,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all\n the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 9,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 10,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 11,GreetingHeader = "Greeting 5", GreetingBody = "Celebrate your birthday today.\n Celebrate being Happy every day"},
                    new Greeting{GreetingId = 12,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be\n filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 13,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and\n that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 14,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all\n the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 15,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 16,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be\n filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 17,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and\n that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 18,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all\n the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 19,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 20,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}, 
                    new Greeting{GreetingId = 21,GreetingHeader = "Greeting 1", GreetingBody = "May your birthday and every day be\n filled with the warmth of sunshine, the happiness of smiles, the sounds of laughter, the feeling of love and the sharing of good cheer."},
                    new Greeting{GreetingId = 22,GreetingHeader = "Greeting 2", GreetingBody = "I hope you have a wonderful day and\n that the year ahead is filled with much love, many wonderful surprises and gives you lasting memories that you will cherish in all the days ahead. Happy Birthday."}, 
                    new Greeting{GreetingId = 23,GreetingHeader = "Greeting 3", GreetingBody = "On this special day, i wish you all\n the very best, all the joy you can ever have and may you be blessed abundantly today, tomorrow and the days to come! May you have a fantastic birthday and many more to come... HAPPY BIRTHDAY!!!!"},               
                    new Greeting{GreetingId = 24,GreetingHeader = "Greeting 4", GreetingBody = "They say that you can count your true\n friends on 1 hand - but not the candles on your birthday cake! #1Happybirthday"}
                    
                };
            #endregion

            #region List<Message>
            List<Message> messages = new List<Message>
                {
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0]),
                    new Message(MessageType.ADMConfirmsPlannedOrRegisteredToBTM, businessTrips.Where(bt => bt.BusinessTripID >=1 && bt.BusinessTripID <=2).ToList(), employees[0])
                };
            #endregion

            #region List<Insurance>
            List<Insurance> insurances = new List<Insurance>
            {
                new Insurance { EmployeeID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 1)},
                new Insurance { EmployeeID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 2)},
                new Insurance { EmployeeID = 3, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 3)},
                new Insurance { EmployeeID = 5, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 5)},
                new Insurance { EmployeeID = 12, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 12)},
                new Insurance { EmployeeID = 13, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 13)},
                new Insurance { EmployeeID = 14, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 14)},
                new Insurance { EmployeeID = 15, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 15)},
                new Insurance { EmployeeID = 16, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 16)},
                new Insurance { EmployeeID = 18, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 18)},
                new Insurance { EmployeeID = 21, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 21)},
                new Insurance { EmployeeID = 23, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 23)},
                new Insurance { EmployeeID = 24, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 24)},
                new Insurance { EmployeeID = 32, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 32)},
                new Insurance { EmployeeID = 33, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 180,InsuranceOf = employees.Find(e => e.EmployeeID == 33)},
                new Insurance { EmployeeID = 34, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 180, InsuranceOf = employees.Find(e => e.EmployeeID == 34)},
                new Insurance { EmployeeID = 40, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 360, InsuranceOf = employees.Find(e => e.EmployeeID == 40)},
                new Insurance { EmployeeID = 42, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 360,InsuranceOf = employees.Find(e => e.EmployeeID == 42)},
                new Insurance { EmployeeID = 44, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 360,InsuranceOf = employees.Find(e => e.EmployeeID == 44)},
                new Insurance { EmployeeID = 45, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-50), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95), Days = 360,InsuranceOf = employees.Find(e => e.EmployeeID == 45)},
                new Insurance { EmployeeID = 46, StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), Days = 90, InsuranceOf = employees.Find(e => e.EmployeeID == 46)},
                new Insurance { EmployeeID = 47, StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 47)},
                new Insurance { EmployeeID = 48, StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 48)},
                new Insurance { EmployeeID = 52, StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), Days = 90, InsuranceOf = employees.Find(e => e.EmployeeID == 52)},
                new Insurance { EmployeeID = 53, StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 53)},
                new Insurance { EmployeeID = 54, StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), Days = 90, InsuranceOf = employees.Find(e => e.EmployeeID == 54)},
                new Insurance { EmployeeID = 61, StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), Days = 90, InsuranceOf = employees.Find(e => e.EmployeeID == 61)},
                new Insurance { EmployeeID = 62, StartDate = new DateTime(2014, 01,01), EndDate = new DateTime (2014, 08, 08),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 62)},
                new Insurance { EmployeeID = 63, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(99), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(135), Days = 90, InsuranceOf = employees.Find(e => e.EmployeeID == 63)},
                new Insurance { EmployeeID = 64, StartDate = new DateTime(2010, 01,01), EndDate = new DateTime (2011, 08, 08),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 64)},
                new Insurance { EmployeeID = 68, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-65), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(95),  Days = 90,InsuranceOf = employees.Find(e => e.EmployeeID == 68)},

              


                };
            #endregion 

            #region QuestionSets 
            List<QuestionSet> questionSets = new List<QuestionSet> {
                new QuestionSet{QuestionSetId = 1, Title = "FirstQuestionSet",  Questions = "[\"FirstQuestion\",\"SecondQuestion\",\"ThirdQuestion\"]"}, 
                new QuestionSet{QuestionSetId = 2, Title = "SecondQuestionSet", Questions = "[\"FoutrthQuestion\",\"FifthQuestion\",\"SixthQuestion\"]"}, 
                new QuestionSet{QuestionSetId = 3, Title = "ThirdQuestionSet", Questions = "[\"SeventhQuestion\",\"EighthQuestion\",\"NinthQuestion\"]"}, 
               new QuestionSet{QuestionSetId = 4,  Title = "FourthQuestionSet", Questions = "[\"SeventhQuestion\",\"EighthQuestion\",\"NinthQuestion\"]"}, 


            };
            #endregion

            #region Questionnaires

            List<Questionnaire> questionnaires = new List<Questionnaire>{
                new Questionnaire{QuestionnaireId = 1, Title = "Questionnaire 1", QuestionSetId = "5:1"},
                new Questionnaire{QuestionnaireId = 2, Title = "Questionnaire 2", QuestionSetId = "1:2"},
                new Questionnaire{QuestionnaireId = 3, Title = "Questionnaire 3", QuestionSetId = "3:2,11:2,10:1,4:1,1:2,1.1:2,6:1,1a:3,1b:2"},
                new Questionnaire{QuestionnaireId = 4, Title = "Questionnaire 4", QuestionSetId = "1a:2,1b:2"},

            };

            #endregion

            #region Fill context with data from lists

            foreach (Country con in countries)
            {
                context.Countries.Add(con);
            }

            foreach (Holiday hol in holidays)
            {
                context.Holidays.Add(hol);
            }

            foreach (Location loc in locations)
            {
                context.Locations.Add(loc);
            }

            foreach (Unit un in units)
            {
                context.Units.Add(un);
            }

            foreach (Department dep in departments)
            {
                context.Departments.Add(dep);
            }

            foreach (Position position in positions)
            {
                context.Positions.Add(position);
            }

            foreach (Employee emp in employees)
            {
                context.Employees.Add(emp);
            }

            foreach (VisaRegistrationDate visareg in visaRegistrationDates)
            {
                context.VisaRegistrationDates.Add(visareg);
            }

            foreach (Visa visa in visas)
            {
                context.Visas.Add(visa);
            }

            foreach (Permit permit in permits)
            {
                context.Permits.Add(permit);
            }

            foreach (BusinessTrip bTrip in businessTrips)
            {
                context.BusinessTrips.Add(bTrip);
            }

            foreach (Journey jou in journeys)
            {
                context.Journeys.Add(jou);
            }

            foreach (Sickness sick in sicks)
            {
                context.Sicknesses.Add(sick);
            }

            foreach (Message message in messages)
            {
                context.Messages.Add(message);
            }

            foreach (Passport passport in passports)
            {
                context.Passports.Add(passport);
            }

            foreach (PrivateTrip privateTrip in privateTrips)
            {
                context.PrivateTrips.Add(privateTrip);
            }

            foreach (CalendarItem calendarItem in calendarItems)
            {
                context.CalendarItems.Add(calendarItem);
            }

            foreach (Overtime overtime in overtimes)
            {
                context.Overtimes.Add(overtime);
            }

            foreach (Vacation vacation in vacations)
            {
                context.Vacations.Add(vacation);
            }

            foreach (Greeting greeting in greetings)
            {
                context.Greetings.Add(greeting);
            }

            foreach (Insurance insurance in insurances)
            {
                context.Insurances.Add(insurance);
            }

            foreach (QuestionSet questionSet in questionSets)
            {
                context.QuestionSets.Add(questionSet);
            }

            foreach (Questionnaire questionnaire in questionnaires)
            {
                context.Questionnaires.Add(questionnaire);
            }

            context.SaveChanges();
            #endregion

            #region Binding

            #region Journeys to CalendarItem
            //Add Journeys to CalendarItem for Employee

            var empWithJourneys = from emp in employees
                                  where emp.BusinessTrips != null
                                  from journ in emp.BusinessTrips
                                  where journ.Journeys != null
                                  from journExact in journ.Journeys
                                  where journExact.JourneyOf != null
                                  select new
                                  {
                                      EmpID = emp.EmployeeID,
                                      Emps = emp,
                                      Loc = journ.Location.Title.ToString(),
                                      fromDate = journExact.Date,
                                      toDate = journExact.Date,
                                      journType = CalendarItemType.Journey
                                  };
            foreach (var emp in empWithJourneys)
            {
                Employee correctEmp = (from e in employees where e.EmployeeID == emp.EmpID select e).FirstOrDefault();
                CalendarItem newItem = new CalendarItem();
                newItem.Employee = emp.Emps;
                newItem.EmployeeID = emp.EmpID;
                newItem.From = emp.fromDate;
                newItem.To = emp.toDate;
                newItem.Type = emp.journType;
                //newItem.Location = emp.Loc;

                correctEmp.CalendarItems.Add(newItem);
                context.SaveChanges();
            }
            #endregion

            #region BusinessTrips to CalendarItem
            //Add BusinessTrips to CalendarItem for Employee


            var empWithBTs = from empBt in employees
                             where empBt.BusinessTrips != null
                             from exactBt in empBt.BusinessTrips
                             where exactBt.BTof != null && (exactBt.Status == (BTStatus.Reported | BTStatus.Confirmed) && (exactBt.Status != BTStatus.Cancelled))
                             select new
                             {
                                 EmpID = empBt.EmployeeID,
                                 Emps = empBt,
                                 Loc = exactBt.Location.Title.ToString(),
                                 fromDate = exactBt.StartDate,
                                 toDate = exactBt.EndDate,
                                 btType = CalendarItemType.BT

                             };

            foreach (var emp in empWithBTs)
            {
                Employee correctEmp = (from e in employees where e.EmployeeID == emp.EmpID select e).FirstOrDefault();
                CalendarItem newItem = new CalendarItem();
                newItem.Employee = emp.Emps;
                newItem.EmployeeID = emp.EmpID;
                newItem.From = emp.fromDate;
                newItem.To = emp.toDate;
                newItem.Type = emp.btType;
                newItem.Location = emp.Loc;

                correctEmp.CalendarItems.Add(newItem);
                context.SaveChanges();
            }

            #endregion

            #region Overtimes to CalendarItem
            //Add Overtime to CalendarItem

            var empWithOvertimes = from emp in employees
                                   where emp.Overtimes != null
                                   from overTimes in emp.Overtimes
                                   where overTimes.Employee != null
                                   select new
                                   {
                                       overtimeID = overTimes.OvertimeID,
                                       employeeID = emp.EmployeeID,
                                       empl = emp,
                                       date = overTimes.Date,
                                       reclaimDate = overTimes.ReclaimDate,
                                       dayOff = overTimes.DayOff,
                                       type = overTimes.Type //Change diff type of Overtime
                                   };

            foreach (var overtime in empWithOvertimes)
            {
                Employee correctEmp = (from empl in employees where empl.EmployeeID == overtime.employeeID select empl).FirstOrDefault();
                CalendarItem item = new CalendarItem();

                item.Employee = overtime.empl;
                item.EmployeeID = overtime.employeeID;
                item.From = overtime.date;
                item.To = overtime.date;
                //item.Type = emp.type;
                switch (overtime.type)
                {
                    //case OvertimeType.Overtime:
                    //    {
                    //        item.Type = CalendarItemType.ReclaimedOvertime;
                    //    }
                    //    break;
                    case OvertimeType.Paid:
                        {
                            item.Type = CalendarItemType.OvertimeForReclaim;
                            if (overtime.reclaimDate != null)
                            {
                                CalendarItem reclaimed = new CalendarItem();
                                reclaimed.Employee = overtime.empl;
                                reclaimed.EmployeeID = overtime.employeeID;
                                reclaimed.From = overtime.reclaimDate.Value;
                                reclaimed.To = overtime.reclaimDate.Value;
                                reclaimed.Type = CalendarItemType.ReclaimedOvertime;
                                correctEmp.CalendarItems.Add(reclaimed);
                                context.SaveChanges();
                            }

                        }
                        break;
                    case OvertimeType.Private:
                        {
                            item.Type = CalendarItemType.PrivateMinus;
                        }
                        break;
                    default:
                        break;
                }
                item.Location = "";

                correctEmp.CalendarItems.Add(item);
                context.SaveChanges();
            }

            #endregion

            #region Vacation to CalendarItem
            //Add Vacations to CalendarItem for Employee

            var empWithVacations = from emp in employees
                                   where emp.Vacations != null
                                   from vac in emp.Vacations
                                   where vac.Employee != null
                                   select new
                                   {
                                       vacationID = vac.VacationID,
                                       employeeID = emp.EmployeeID,
                                       empl = emp,
                                       fromDate = vac.From,
                                       toDate = vac.To,
                                       type = vac.Type //Change diff type of Overtime
                                   };

            foreach (var emp in empWithVacations)
            {
                Employee correctEmp = (from empl in employees where empl.EmployeeID == emp.employeeID select empl).FirstOrDefault();
                CalendarItem item = new CalendarItem();

                item.Employee = emp.empl;
                item.EmployeeID = emp.employeeID;
                item.From = emp.fromDate;
                item.To = emp.toDate;
                //item.Type = emp.type;
                switch (emp.type)
                {
                    case VacationType.PaidVacation:
                        {
                            item.Type = CalendarItemType.PaidVacation;
                        }
                        break;
                    case VacationType.UnpaidVacation:
                        {
                            item.Type = CalendarItemType.UnpaidVacation;
                        }
                        break;
                    default:
                        break;
                }
                item.Location = "";

                correctEmp.CalendarItems.Add(item);
                context.SaveChanges();
            }

            #endregion

            context.SaveChanges();

            #endregion

           
                        
#endregion

        }

        public void InitDBClear(AjourDbContext context)
        {
            if (context.Database.Exists()) context.Database.Delete();
            context.Database.Create();

            Department department = new Department { DepartmentID = 1, DepartmentName = "Unreal" };
            context.Departments.Add(department);

            Position position = new Position { PositionID = 1, TitleEn = "Unreal", TitleUk = "Нереальна" };
            context.Positions.Add(position);

            Employee powerUser = new Employee { EmployeeID = 1, FirstName = "Robert", LastName = "Knight", DepartmentID = 1, EID = "admin", DateEmployed = new DateTime(2006, 04, 11), IsManager = true, PositionID = 1, CalendarItems = new List<CalendarItem>(), Overtimes = new List<Overtime>(), Vacations = new List<Vacation>(), Sicknesses = new List<Sickness>(), IsGreetingMessageAllow = true };
            context.Employees.Add(powerUser);

            Country country = new Country { CountryID = 1, CountryName = "Ukraine", Holidays = new List<Holiday>(), Locations = new List<Location>(), Comment = "UTC + 2" };
            context.Countries.Add(country);

            Holiday holiday = new Holiday { HolidayID = 1, Title = "NewYear", HolidayDate = new DateTime(2013, 04, 10), CountryID = 1, IsPostponed = false };
            context.Holidays.Add(holiday);

            context.SaveChanges();
        }

        public void InitDbNotChanged(AjourDbContext context)
        {
        }
    }
    
}