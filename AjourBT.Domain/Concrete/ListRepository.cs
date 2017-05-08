using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels; 

namespace AjourBT.Domain.Concrete
{
    public class ListRepository : IRepository
    {
        static ListRepository()
        {
            departments = new List<Department>{
                     new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
                     new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
                     new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
                     new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
                     new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
                     new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            employees = new List<Employee>
             {
                new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},          
                new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>(), IsGreetingMessageAllow = true},
                new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi", DateEmployed = new DateTime(2011,11,02), IsManager = true , IsGreetingMessageAllow = true}
             };

            businessTrips = new List<BusinessTrip> 
            { 
                new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,10), Status= BTStatus.Planned, EmployeeID = 1, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2014,11,01), EndDate = new DateTime (2014,11,10), Status= BTStatus.Registered, EmployeeID = 2, LocationID = 1 }, 
                new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014,10,20), EndDate = new DateTime (2014,10,30), Status= BTStatus.Confirmed, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014,10,01), EndDate = new DateTime (2014,10,12), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2 }, 
                new BusinessTrip { BusinessTripID = 5, StartDate = new DateTime(2013,10,01), EndDate = new DateTime (2013,10,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 6, StartDate = new DateTime(2013,08,01), EndDate = new DateTime (2013,08,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 7, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 8, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,20), Status= BTStatus.Confirmed, EmployeeID = 4, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 9, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,25), Status= BTStatus.Confirmed, EmployeeID = 5, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 10, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned, EmployeeID = 6, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 11, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned, EmployeeID = 7, LocationID = 1 },
                new BusinessTrip { BusinessTripID = 12, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 13, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Registered | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee reg + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 14, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf + modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 15, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported and modif", Manager = "xtwe", Purpose = "meeting" },
                new BusinessTrip { BusinessTripID = 17, StartDate = new DateTime(2014,12,01), EndDate = new DateTime (2014,12,25), Status= BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 7, LocationID = 1, Comment = "7 employee reg and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired"},
                new BusinessTrip { BusinessTripID = 18, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,25), Status= BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired" },
                new BusinessTrip { BusinessTripID = 19, StartDate = new DateTime(2014,09,01), EndDate = new DateTime (2014,09,27), Status= BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and modified and cancelled", Manager = "xtwe", Purpose = "meeting", CancelComment = "visa expired" },
                new BusinessTrip { BusinessTripID = 20, StartDate = new DateTime(2013,09,01), EndDate = new DateTime (2013,09,27), Status= BTStatus.Planned, EmployeeID = 2, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting", RejectComment = "visa expired" }
            };

            locations = new List<Location>
             { 
                new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>()}, 
                new Location {LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>()}
                             
             };

            visaRegistrationDates = new List<VisaRegistrationDate>
            {
                new VisaRegistrationDate {EmployeeID=1, RegistrationDate=new DateTime(2013,01,01),VisaType="D08"},
                new VisaRegistrationDate {EmployeeID=2, RegistrationDate=new DateTime(2013,10,02),VisaType="C07"},   
                new VisaRegistrationDate {EmployeeID=3, RegistrationDate=new DateTime(2013,01,01),VisaType="C07"},
                new VisaRegistrationDate {EmployeeID=4, RegistrationDate=new DateTime(2013,01,04),VisaType="D08"}

            };

            visas = new List<Visa>
            {
                new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2012,08,01), DueDate = new DateTime (2013,11,02), Days = 90, DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0 },
                new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012,02,13), DueDate = new DateTime (2013,05,13), Days = 20, DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4 }
            };

            permits = new List<Permit>
            {
                new Permit { EmployeeID = 1, Number = "04/2012", StartDate = new DateTime (2012, 08, 01), EndDate = new DateTime(2013, 12, 30), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 1)},
                new Permit { EmployeeID = 2, Number = "01/2012", StartDate = new DateTime(2012, 01,01), EndDate = new DateTime (2013, 08, 08), IsKartaPolaka = true, PermitOf = employees.Find(e => e.EmployeeID == 2)},
                new Permit { EmployeeID = 3, Number = "01/2013", StartDate = new DateTime(2013, 01,01), EndDate = new DateTime (2014, 08, 08), IsKartaPolaka = false, PermitOf = employees.Find(e => e.EmployeeID == 3)}
            };
            
            departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 1));
            departments.Find(d => d.DepartmentID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 2));
            departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 3));
            departments.Find(d => d.DepartmentID == 4).Employees.Add(employees.Find(e => e.EmployeeID == 4));
            departments.Find(d => d.DepartmentID == 6).Employees.Add(employees.Find(e => e.EmployeeID == 5));
            departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 6));
            departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 7));
            departments.Find(d => d.DepartmentID == 5).Employees.Add(employees.Find(e => e.EmployeeID == 8));

            employees.Find(e => e.EmployeeID == 1).Department = departments.Find(v => v.DepartmentID == 1);
            employees.Find(e => e.EmployeeID == 2).Department = departments.Find(v => v.DepartmentID == 2);
            employees.Find(e => e.EmployeeID == 3).Department = departments.Find(v => v.DepartmentID == 1);
            employees.Find(e => e.EmployeeID == 4).Department = departments.Find(v => v.DepartmentID == 4);
            employees.Find(e => e.EmployeeID == 5).Department = departments.Find(v => v.DepartmentID == 6);
            employees.Find(e => e.EmployeeID == 6).Department = departments.Find(v => v.DepartmentID == 5);
            employees.Find(e => e.EmployeeID == 7).Department = departments.Find(v => v.DepartmentID == 5);
            employees.Find(e => e.EmployeeID == 8).Department = departments.Find(v => v.DepartmentID == 1);

            employees.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 1));
            employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 2));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 3));
            employees.Find(e => e.EmployeeID == 3).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 4));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 5));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 6));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 7));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 8));
            employees.Find(e => e.EmployeeID == 5).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 9));
            employees.Find(e => e.EmployeeID == 6).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 10));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 11));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 12));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 13));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 14));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 15));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 16));
            employees.Find(e => e.EmployeeID == 7).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 17));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 18));
            employees.Find(e => e.EmployeeID == 4).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 19));
            employees.Find(e => e.EmployeeID == 2).BusinessTrips.Add(businessTrips.Find(v => v.BusinessTripID == 20));

            businessTrips.Find(b => b.BusinessTripID == 1).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 2).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 3).Location = (locations.Find(l => l.LocationID == 2));
            businessTrips.Find(b => b.BusinessTripID == 4).Location = (locations.Find(l => l.LocationID == 2));
            businessTrips.Find(b => b.BusinessTripID == 5).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 6).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 7).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 8).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 9).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 10).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 11).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 12).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 13).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 14).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 15).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 16).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 17).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 18).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 19).Location = (locations.Find(l => l.LocationID == 1));
            businessTrips.Find(b => b.BusinessTripID == 20).Location = (locations.Find(l => l.LocationID == 1));

            businessTrips.Find(b => b.BusinessTripID == 1).BTof = (employees.Find(l => l.EmployeeID == 1));
            businessTrips.Find(b => b.BusinessTripID == 2).BTof = (employees.Find(l => l.EmployeeID == 2));
            businessTrips.Find(b => b.BusinessTripID == 3).BTof = (employees.Find(l => l.EmployeeID == 3));
            businessTrips.Find(b => b.BusinessTripID == 4).BTof = (employees.Find(l => l.EmployeeID == 3));
            businessTrips.Find(b => b.BusinessTripID == 5).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 6).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 7).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 8).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 9).BTof = (employees.Find(l => l.EmployeeID == 5));
            businessTrips.Find(b => b.BusinessTripID == 10).BTof = (employees.Find(l => l.EmployeeID == 6));
            businessTrips.Find(b => b.BusinessTripID == 11).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 12).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 13).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 14).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 15).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 16).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 17).BTof = (employees.Find(l => l.EmployeeID == 7));
            businessTrips.Find(b => b.BusinessTripID == 18).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 19).BTof = (employees.Find(l => l.EmployeeID == 4));
            businessTrips.Find(b => b.BusinessTripID == 20).BTof = (employees.Find(l => l.EmployeeID == 2));

            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 1));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 2));
            locations.Find(l => l.LocationID == 2).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 3));
            locations.Find(l => l.LocationID == 2).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 4));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 5));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 6));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 7));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 8));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 9));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 10));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 11));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 12));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 13));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 14));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 15));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 16));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 17));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 18));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 19));
            locations.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips.Find(b => b.BusinessTripID == 20));

            employees.Find(e => e.EmployeeID == 1).Visa = visas.Find(v => v.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).Visa = visas.Find(v => v.EmployeeID == 2);

            visas.Find(v => v.EmployeeID == 1).VisaOf = employees.Find(e => e.EmployeeID == 1);
            visas.Find(v => v.EmployeeID == 2).VisaOf = employees.Find(e => e.EmployeeID == 2);

            employees.Find(e => e.EmployeeID == 1).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 2);
            employees.Find(e => e.EmployeeID == 3).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 3);
            employees.Find(e => e.EmployeeID == 4).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 4);

            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 1).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 1);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 2).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 2);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 3).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 3);
            visaRegistrationDates.Find(vrd => vrd.EmployeeID == 4).VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 4);

            employees.Find(e => e.EmployeeID == 1).Permit = permits.Find(p => p.EmployeeID == 1);
            employees.Find(e => e.EmployeeID == 2).Permit = permits.Find(p => p.EmployeeID == 2);
            employees.Find(e => e.EmployeeID == 3).Permit = permits.Find(p => p.EmployeeID == 3);
        }

        #region Employeess

        private static List<Employee> employees;
        public IList<Employee> Employees
        {
            get { return employees; }
            set { employees = value as List<Employee>; }
        }

        public void SaveEmployee(Employee employee)
        {
            if (employee.EmployeeID.Equals(default(int)))
            {
                Employee lastEmp = employees.Last();
                employee.EmployeeID = lastEmp.EmployeeID + 1;
                employees.Add(employee);
            }
            else
            {
                Employee emp = employees.Find(e => e.EmployeeID == employee.EmployeeID);
                if (emp != null)
                {
                    emp.FirstName = employee.FirstName;
                    emp.LastName = employee.LastName;
                    emp.DepartmentID = employee.DepartmentID;
                    emp.EID = employee.EID;
                    emp.Department = employee.Department;
                    emp.DateEmployed = employee.DateEmployed;
                    emp.DateDismissed = employee.DateDismissed;
                    emp.IsManager = employee.IsManager;
                }
            }
        }

        public Employee DeleteEmployee(int employeeID)
        {
            Employee emp = employees.Find(e => e.EmployeeID == employeeID);
            if (emp != null)
            {
                if (emp.BusinessTrips == null || emp.BusinessTrips.Count == 0)
                {
                    employees.Remove(emp);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }

            }
            return emp;
        }


        #endregion

        #region Departments
        private static List<Department> departments;
        public IList<Department> Departments
        {
            get { return departments; }
            set { departments = value as List<Department>; }
        }

        public void SaveDepartment(Department department)
        {
            if (department.DepartmentID.Equals(default(int)))
            {
                Department lastDep = departments.Last();
                department.DepartmentID = lastDep.DepartmentID + 1;
                departments.Add(department);
            }
            else
            {
                Department depInList = departments.Where(d => d.DepartmentID == department.DepartmentID).FirstOrDefault();
                if (depInList != null)
                {
                    depInList.DepartmentName = department.DepartmentName;

                }
            }
        }

        public Department DeleteDepartment(int departmentID)
        {
            Department depInList = departments.Where(e => e.DepartmentID == departmentID).FirstOrDefault();

            if (depInList != null)
            {
                if (depInList.Employees == null || depInList.Employees.Count == 0)
                {
                    departments.Remove(depInList);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }

            return depInList;
        }


        #endregion

        #region Visas
        private static List<Visa> visas;
        public IList<Visa> Visas
        {
            get { return visas; }
            set { visas = value as List<Visa>; }
        }

        public void SaveVisa(Visa visa, int id)
        {
            Visa dbEntry = (from v in Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (dbEntry != null)
            {
                dbEntry.EmployeeID = visa.EmployeeID;
                dbEntry.VisaType = visa.VisaType;
                dbEntry.StartDate = visa.StartDate;
                dbEntry.DueDate = visa.DueDate;
                dbEntry.Days = visa.Days;
                if (visa.DaysUsedInBT == null)
                    dbEntry.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                {
                    dbEntry.DaysUsedInPrivateTrips = 0;
                }
                else
                    dbEntry.DaysUsedInPrivateTrips = visa.DaysUsedInPrivateTrips;

                dbEntry.Entries = visa.Entries;
                if (visa.EntriesUsedInBT == null)
                    dbEntry.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                {
                    dbEntry.EntriesUsedInPrivateTrips = 0;
                }
                else
                    dbEntry.EntriesUsedInPrivateTrips = visa.EntriesUsedInPrivateTrips;

            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                visa.VisaOf = employee;
                visa.EmployeeID = employee.EmployeeID;
                visa.VisaOf.Visa = visa;
                if (visa.DaysUsedInBT == null)
                    visa.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                    visa.DaysUsedInPrivateTrips = 0;
                if (visa.EntriesUsedInBT == null)
                    visa.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                    visa.EntriesUsedInPrivateTrips = 0;

                visas.Add(visa);
            }
        }

        public Visa DeleteVisa(int employeeID)
        {
            Visa dbEntry = Visas.Where(v => v.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                dbEntry.VisaOf.Visa = null;
                visas.Remove(dbEntry);

            }

            return dbEntry;
        }
        #endregion

        #region VisaRegistrationDates

        private static List<VisaRegistrationDate> visaRegistrationDates;
        public IList<VisaRegistrationDate> VisaRegistrationDates
        {
            get { return visaRegistrationDates; }
            set { visaRegistrationDates = value as List<VisaRegistrationDate>; }
        }

        public void SaveVisaRegistrationDate(VisaRegistrationDate visaRegistrationDate, int id)
        {
            VisaRegistrationDate dbEntry = (from vrd in VisaRegistrationDates where vrd.EmployeeID == visaRegistrationDate.EmployeeID select vrd).FirstOrDefault();

            if (dbEntry != null)
            {
                dbEntry.EmployeeID = visaRegistrationDate.EmployeeID;
                dbEntry.VisaType = visaRegistrationDate.VisaType;
                dbEntry.RegistrationDate = visaRegistrationDate.RegistrationDate;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                visaRegistrationDate.VisaRegistrationDateOf = employee;
                visaRegistrationDate.EmployeeID = employee.EmployeeID;
                visaRegistrationDate.VisaRegistrationDateOf.VisaRegistrationDate = visaRegistrationDate;
                visaRegistrationDates.Add(visaRegistrationDate);
            }
        }

        public VisaRegistrationDate DeleteVisaRegistrationDate(int employeeID)
        {
            VisaRegistrationDate dbEntry = visaRegistrationDates.Where(vrd => vrd.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                dbEntry.VisaRegistrationDateOf.VisaRegistrationDate = null;
                visaRegistrationDates.Remove(dbEntry);
            }

            return dbEntry;
        }
        #endregion

        #region Permits
        private static List<Permit> permits;
        public IList<Permit> Permits
        {
            get { return permits; }
            set { permits = value as List<Permit>; }
        }

        public void SavePermit(Permit permit, int id)
        {
            Permit dbEntry = (from p in Permits where p.EmployeeID == permit.EmployeeID select p).FirstOrDefault();

            if (dbEntry != null)
            {
                dbEntry.EmployeeID = permit.EmployeeID;
                dbEntry.Number = permit.Number;
                dbEntry.StartDate = permit.StartDate;
                dbEntry.EndDate = permit.EndDate;
                dbEntry.IsKartaPolaka = permit.IsKartaPolaka;
                dbEntry.CancelRequestDate = permit.CancelRequestDate;
                dbEntry.ProlongRequestDate = permit.ProlongRequestDate;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                permit.PermitOf = employee;
                permit.EmployeeID = employee.EmployeeID;
                permit.PermitOf.Permit = permit;
                permits.Add(permit);
            }
        }

        public Permit DeletePermit(int employeeID)
        {
            Permit dbEntry = Permits.Where(p => p.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                dbEntry.PermitOf.Permit = null;
                permits.Remove(dbEntry);
            }

            return dbEntry;
        }
        #endregion


        #region Insurances
        private static List<Insurance> insurances;
        public IList<Insurance> Insurances
        {
            get { return insurances; }
            set { insurances = value as List<Insurance>; }
        }

        public void SaveInsurance(Insurance insurance, int id)
        {
            Insurance dbEntry = (from p in Insurances where p.EmployeeID == insurance.EmployeeID select p).FirstOrDefault();

            if (dbEntry != null)
            {
                dbEntry.EmployeeID = insurance.EmployeeID;
                dbEntry.StartDate = insurance.StartDate;
                dbEntry.EndDate = insurance.EndDate;
                dbEntry.Days = insurance.Days;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                insurance.InsuranceOf = employee;
                insurance.EmployeeID = employee.EmployeeID;
                insurance.InsuranceOf.Insurance = insurance;
                insurances.Add(insurance);
            }
        }

        public Insurance DeleteInsurance(int employeeID)
        {
            Insurance dbEntry = Insurances.Where(p => p.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                dbEntry.InsuranceOf.Insurance = null;
                insurances.Remove(dbEntry);
            }

            return dbEntry;
        }
        #endregion
        #region BusinessTrips
        public static List<BusinessTrip> businessTrips;
        public IList<BusinessTrip> BusinessTrips
        {
            get { return businessTrips; }
            set { businessTrips = value as List<BusinessTrip>; }
        }
        public void SaveBusinessTrip(BusinessTrip bt)
        {
            SaveBusinessTrip(bt, false);
        }
        public void SaveBusinessTrip(BusinessTrip bt, bool IsInitialSave = false)
        {
            BusinessTrip dbEntry = (from b in BusinessTrips where b.BusinessTripID == bt.BusinessTripID select b).FirstOrDefault();

            if (dbEntry != null)
            {
                dbEntry.StartDate = bt.StartDate;
                dbEntry.EndDate = bt.EndDate;
                dbEntry.Status = bt.Status;
                dbEntry.LocationID = bt.LocationID;
               

            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == bt.EmployeeID).SingleOrDefault();
                BusinessTrip btLast = BusinessTrips.Last();
                bt.BusinessTripID = btLast.BusinessTripID + 1;
                bt.BTof = employee;
                bt.EmployeeID = employee.EmployeeID;
                bt.Location = locations.Where(loc => loc.LocationID == bt.LocationID).FirstOrDefault();
               
                employees.Find(e => e.EmployeeID == employee.EmployeeID).BusinessTrips.Add(bt);
                locations.Find(loc => loc.LocationID == bt.LocationID).BusinessTrips.Add(bt);
                businessTrips.Add(bt);
            }
        }

        public BusinessTrip DeleteBusinessTrip(int btID)
        {
            BusinessTrip dbEntry = BusinessTrips.Where(b => b.BusinessTripID == btID).SingleOrDefault();

            if (dbEntry != null)
            {
                Employee employee = Employees.Where(e => e.EmployeeID == dbEntry.EmployeeID).SingleOrDefault();
                employee.BusinessTrips.Remove(dbEntry);
                Location location = Locations.Where(l => l.LocationID == dbEntry.LocationID).SingleOrDefault();
                location.BusinessTrips.Remove(dbEntry);
                businessTrips.Remove(dbEntry);
            }

            return dbEntry;
        }

       

        #endregion

        #region Locations
        private static List<Location> locations;
        public IList<Location> Locations
        {
            get { return locations; }
            set { locations = value as List<Location>; }
        }

        public void SaveLocation(Location location)
        {
            if (location.LocationID == 0)
            {
                location.LocationID = locations.Last().LocationID + 1;
                locations.Add(location);
            }
            else
            {
                Location dbEntry = (from loc in Locations where loc.LocationID == location.LocationID select loc).FirstOrDefault();
                if (dbEntry != null)
                {
                    dbEntry.Title = location.Title;
                    dbEntry.Address = location.Address;
                    dbEntry.ResponsibleForLoc = location.ResponsibleForLoc;
                }
            }
        }

        public Location DeleteLocation(int locationID)
        {
            Location dbEntry = (from loc in Locations where loc.LocationID == locationID select loc).FirstOrDefault();

            if (dbEntry != null)
            {
                if (dbEntry.BusinessTrips == null || dbEntry.BusinessTrips.Count == 0)
                {
                    locations.Remove(dbEntry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }

            return dbEntry;
        }
        #endregion

        #region Units

        private static List<Unit> units;
        public IList<Unit> Units
        {
            get { return units; }
            set { units = value as List<Unit>; }
        }

        public void SaveUnit(Unit unit)
        {
            if (unit.UnitID == 0)
            {
                unit.UnitID = units.Last().UnitID + 1;
                units.Add(unit);
            }
            else
            {
                Unit dbEntry = (from un in Units where un.UnitID == unit.UnitID select un).FirstOrDefault();
                if (dbEntry != null)
                {
                    dbEntry.Title = unit.Title;
                    dbEntry.ShortTitle = unit.ShortTitle;

                }
            }
        }

        public Unit DeleteUnit(int unitID)
        {
            Unit dbEntry = (from un in Units where un.UnitID == unitID select un).FirstOrDefault();

            if (dbEntry != null)
            {
                if (dbEntry.BusinessTrips == null || dbEntry.BusinessTrips.Count == 0)
                {
                    units.Remove(dbEntry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }

            return dbEntry;
        }
        #endregion

        #region Messages
        private static List<IMessage> messages;
        public IList<IMessage> Messages
        {
            get { return messages; }
            set { messages = value as List<IMessage>; }
        }

        public void SaveMessage(IMessage message)
        {
            if (message.MessageID == 0)
            {
                message.MessageID = messages.Last().MessageID + 1;
                messages.Add(message);
            }
            else
            {
                IMessage dbEntry = (from m in Messages where m.MessageID == message.MessageID select m).FirstOrDefault();
                if (dbEntry != null)
                {
                    dbEntry.MessageID = message.MessageID;
                    dbEntry.Role = message.Role;
                    dbEntry.Subject = message.Subject;
                    dbEntry.Body = message.Body;
                    dbEntry.Link = message.Link;
                    dbEntry.TimeStamp = message.TimeStamp;
                    dbEntry.ReplyTo = message.ReplyTo;
                }
            }
        }

        public IMessage DeleteMessage(int messageID)
        {
            IMessage dbEntry = (from loc in Messages where loc.MessageID == messageID select loc).FirstOrDefault();

            if (dbEntry != null)
            {
                    messages.Remove(dbEntry);
            }

            return dbEntry;
        }
        #endregion

        public void SaveRolesForEmployee(string username, string[] roles)
        {
            //throw new NotImplementedException();
        }

        public void DeleteUser(string username)
        {
            //throw new NotImplementedException();
        }

        #region Passports
        private static List<Passport> passports;
        public IList<Passport> Passports
        {
            get { return passports; }
            set { passports = value as List<Passport>; }
        }

        public void SavePassport(Passport passport)
        {
            Passport dbEntry = (from p in Passports where p.EmployeeID == passport.EmployeeID select p).FirstOrDefault();
            if (dbEntry != null)
            {
                dbEntry.EmployeeID = passport.EmployeeID;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == passport.EmployeeID).SingleOrDefault();
                passport.PassportOf = employee;
                passports.Add(passport);
            }
        }

        public Passport DeletePassport(int employeeID)
        {
            Passport dbEntry = (from p in Passports where p.EmployeeID == employeeID select p).FirstOrDefault();
            if (dbEntry != null)
            {
                passports.Remove(dbEntry as Passport);
            };
            return dbEntry;
        }

        #endregion

        #region PrivateTrips
        public IList<PrivateTrip> PrivateTrips
        {
            get { throw new NotImplementedException(); }
        }

        public void SavePrivateTrip(PrivateTrip pt)
        {
            throw new NotImplementedException();
        }

        public PrivateTrip DeletePrivateTrip(int employeeID)
        {
            throw new NotImplementedException();
        }

        

        #endregion
        
        #region Position

        public IList<Position> Positions
        {
            get { throw new NotImplementedException(); }
        }

        public void SavePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public Position DeletePosition(int positionID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Countries
        private static IList<Country> countries;
        public IList<Country> Countries
        {
            get { return countries; }
            set { countries = value; }
        }

        public void SaveCountry( Country country)
        {
            if (country.CountryID == 0)
            {
                country.CountryID = countries.Last().CountryID + 1;
                countries.Add(country);
            }
            else
            {
                Country dbEntry = (from con in Countries where con.CountryID == country.CountryID select con).FirstOrDefault();
                if (dbEntry != null)
                {
                    dbEntry.Comment = country.Comment;
                    dbEntry.CountryName = country.CountryName;
                    dbEntry.Holidays = country.Holidays;
                    dbEntry.Locations = country.Locations;
                }
            }
        }

        public Country DeleteCountry(int countryID)
        {
            Country dbEntry = (from con in Countries where con.CountryID == countryID select con).FirstOrDefault();

            if (dbEntry != null)
            {
                if (dbEntry.Locations == null && dbEntry.Holidays == null)
                {
                    countries.Remove(dbEntry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }

            return dbEntry;
        }

        #endregion

        #region Holidays
        private static IList<Holiday> holidays;

        public IList<Holiday> Holidays
        {
            get { return holidays; }
            set { holidays = value; }
        }

        public void SaveHoliday(Holiday holiday)
        {
            if (holiday.HolidayID == 0)
            {
                holiday.HolidayID = holidays.Last().HolidayID + 1;
                holidays.Add(holiday);
            }
            else
            {
                Holiday dbentry = (from hol in Holidays where hol.HolidayID == holiday.HolidayID select hol).FirstOrDefault();
                if (dbentry != null)
                {
                    dbentry.HolidayDate = holiday.HolidayDate;
                    dbentry.CountryID = holiday.CountryID;
                    dbentry.IsPostponed = holiday.IsPostponed;
                    dbentry.HolidayComment = holiday.HolidayComment;
                }
            }
        }

        public Holiday DeleteHoliday(int holidayID)
        {
            Holiday dbentry = (from hol in Holidays where hol.HolidayID == holidayID select hol).FirstOrDefault();

            if (dbentry != null)
            {
                if (dbentry.Country == null)
                {
                    holidays.Remove(dbentry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }
            return dbentry;
        }

        #endregion

        #region Journeys

        public IList<Journey> Journeys
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveJourney(Journey journey)
        {
            throw new NotImplementedException();
        }

        public Journey DeleteJourney(int journeyID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CalendarItem
        private static List<CalendarItem> calendarItems;
        public IList<CalendarItem> CalendarItems
        {
            get { return calendarItems; }
            set { calendarItems = value as List<CalendarItem>; }
        }

        public void SaveCalendarItem(CalendarItem CalendarItem)
        {
            throw new NotImplementedException();
        }

        public CalendarItem DeleteCalendarItem(int calendarID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Overtime
        private static List<Overtime> overtimes;
        public IList<Overtime> Overtimes
        {
            get { return overtimes; }
            set { overtimes = value as List<Overtime>; }
        }

        public void SaveOvertime(Overtime overtime)
        {
            if (overtime.OvertimeID == 0)
            {
                overtime.OvertimeID = overtimes.Last().OvertimeID + 1;
                overtimes.Add(overtime);
            }

            else
            {
                Overtime dbentry = (from over in Overtimes where over.OvertimeID == overtime.OvertimeID select over).FirstOrDefault();
                if (dbentry != null)
                {
                    dbentry = overtime;
                }
            }
        }

        public Overtime DeleteOvertime(int overtimeID)
        {
            Overtime dbentry = (from over in Overtimes where over.OvertimeID == overtimeID select over).FirstOrDefault();

            if (dbentry != null)
            {
                if (dbentry.Employee == null)
                {
                    overtimes.Remove(dbentry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }
            return dbentry;
        }

        #endregion

        #region Vacation
        private static List<Vacation> vacations;

        public IList<Vacation> Vacations
        {
            get { return vacations; }
            set { vacations = value as List<Vacation>; }
        }

        public void SaveVacation(Vacation vacation)
        {
            if (vacation.VacationID == 0)
            {
                vacation.VacationID = vacations.Last().VacationID + 1;
                vacations.Add(vacation);
            }
            else
            {
                Vacation dbentry = (from vac in Vacations where vac.VacationID == vacation.VacationID select vac).FirstOrDefault();
                if (dbentry != null)
                {
                    dbentry = vacation;
                }
            }
        }

        public Vacation DeleteVacation(int vacationID)
        {
            Vacation dbentry = (from vac in Vacations where vac.VacationID == vacationID select vac).FirstOrDefault();

            if (dbentry != null)
            {
                if (dbentry.Employee == null)
                {
                    vacations.Remove(dbentry);
                }
                else
                {
                    throw new System.Data.Entity.Infrastructure.DbUpdateException();
                }
            }

            return dbentry;
        }

        #endregion

        #region Sick
        public IList<Sickness> Sicknesses
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveSick(Sickness sick)
        {
            throw new NotImplementedException();
        }

        public Sickness DeleteSick(int SickID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region User

        public IList<Employee> Users
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Greeting 

        public IList<Greeting> Greetings
        {
            get { throw new NotImplementedException(); }
        }

        public void  SaveGreeting(Greeting greeting)
        {
            throw new NotImplementedException();
        }

        public Greeting DeleteGreeting(int Greeting)
        {
            throw new NotImplementedException();
        } 

        #endregion 

        #region AbsenceData 
        public IList<AbsenceViewModel> SearchAbsenceData(DateTime fromDate, DateTime toDate, string searchString = "")
        {
            throw new NotImplementedException();
        }
        #endregion 

    

        public IList<EmployeeViewModel> SearchUsersData(string selectedDepartment, string searchString = "")
        {
            throw new NotImplementedException();
        }


        public IList<BusinessTripViewModel> GetBusinessTripDataByUnits(int selectedYear)
        {
            throw new NotImplementedException();
        }

        public IList<BusinessTripViewModel> GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(int selectedYear)
        {
            throw new NotImplementedException();
        }


        public IList<Employee> SearchVisaData(string searchString)
        {
            throw new NotImplementedException();
        }


        public IList<Employee> SearchVisaDataExcludingDismissed(string searchString)
        {
            throw new NotImplementedException();
        }



        public IList<WTRViewModel> SearchWTRData(DateTime fromDate, DateTime toDate, string searchString = "")
        {
            throw new NotImplementedException();
        }


        public IList<WTRViewModel> SearchWTRDataPerEMP(DateTime fromDate, DateTime toDate, Employee employee)
        {
            throw new NotImplementedException();
        }

        public IList<Employee> GetCurrentlyEmployedEmployees(string department)
        {
            throw new NotImplementedException();

        }


        public IList<Education> Educations
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveEducation(Education education)
        {
            throw new NotImplementedException();
        }

        public Education DeleteEducation(int EducationID)
        {
            throw new NotImplementedException();
        }


        public IList<QuestionSet> QuestionSets
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveQuestionSet(QuestionSet questionSet)
        {
            throw new NotImplementedException();
        }

        public QuestionSet DeleteQuestionSet(int questionSet)
        {
            throw new NotImplementedException();
        }


        public IList<Questionnaire> Questionnaires
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveQuestionnaire(Questionnaire questionnaire)
        {
            throw new NotImplementedException();
        }

        public Questionnaire DeleteQuestionnaire(int questionnaire)
        {
            throw new NotImplementedException();
        }
    }

}