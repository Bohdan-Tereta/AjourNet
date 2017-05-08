using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WebMatrix.WebData;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels;
using System.Globalization; 

namespace AjourBT.Domain.Concrete
{
    public class AjourDbRepository : IRepository, IDisposable
    {
        private AjourDbContext context;
        private static bool firstTime = true;

        public AjourDbRepository(string connectionString = "AjourBTConnection")
        {
            context = new AjourDbContext(connectionString);

            if (firstTime)
            {
                try
                {
                    context.Database.Initialize(force: false);
                    firstTime = false;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Initialization Failed... " + ex.Message, ex);
                }
            }
        }

        public void Dispose()
        {
            if (context != null)
                ((IDisposable)context).Dispose();
        }


        #region Employees
        public IList<Employee> Employees
        {
            get { return context.Employees.Where(e => e.IsUserOnly == false).ToList<Employee>(); }
        }

        public void SaveEmployee(Employee employee)
        {
            if (employee.EmployeeID.Equals(default(int)))
            {
                context.Employees.Add(employee); 

            }
            else
            {
                Employee emp = context.Employees.Find(employee.EmployeeID);

                if (emp != null)
                {
                    if (!emp.RowVersion.SequenceEqual(employee.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }
                    emp.FirstName = employee.FirstName;
                    emp.LastName = employee.LastName;
                    emp.DepartmentID = employee.DepartmentID;
                    emp.EID = employee.EID;
                    emp.DateEmployed = employee.DateEmployed;
                    emp.DateDismissed = employee.DateDismissed;
                    emp.IsManager = employee.IsManager;
                    emp.BirthDay = employee.BirthDay;
                    emp.Comment = employee.Comment;
                    emp.FullNameUk = employee.FullNameUk;
                    emp.PositionID = employee.PositionID;
                    emp.IsUserOnly = employee.IsUserOnly;
                    emp.IsGreetingMessageAllow = employee.IsGreetingMessageAllow;
                    emp.EMail = employee.EMail;  
                    emp.EducationAcquiredType = employee.EducationAcquiredType; 
                    emp.EducationAcquiredDate = employee.EducationAcquiredDate; 
                    emp.EducationInProgressType = employee.EducationInProgressType; 
                    emp.EducationInProgressDate = employee.EducationInProgressDate;
                    emp.EducationComment = employee.EducationComment; 
                }
            }
            context.SaveChanges();
        }

        public Employee DeleteEmployee(int employeeID)
        {
            Employee emp = context.Employees.Find(employeeID);

            if (emp != null)
            {
                context.Employees.Remove(emp);
            }

            context.SaveChanges();

            return emp;
        }

        #endregion

        #region Departments

        public IList<Department> Departments
        {
            get { return context.Departments.ToList<Department>(); }
        }

        public void SaveDepartment(Department department)
        {
            if (department.DepartmentID == 0)
            {
                context.Departments.Add(department);
                context.SaveChanges();
            }
            else
            {
                Department dbEntry = context.Departments.Find(department.DepartmentID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(department.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.DepartmentName = department.DepartmentName;
                    dbEntry.RowVersion = department.RowVersion;
                    context.SaveChanges();
                }
            }

        }

        public Department DeleteDepartment(int departmentID)
        {
            Department dbEntry = context.Departments.Find(departmentID);

            if (dbEntry != null)
            {
                //if (!dbEntry.RowVersion.SequenceEqual(department.RowVersion))
                //{
                //    throw new DbUpdateConcurrencyException();
                //}
                context.Departments.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Visas
        public IList<Visa> Visas
        {
            get { return context.Visas.ToList<Visa>(); }
        }

        public void SaveVisa(Visa visa, int id)
        {
            Visa dbEntry = (from v in context.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (dbEntry != null && visa.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(visa.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = visa.EmployeeID;
                dbEntry.VisaType = visa.VisaType;
                dbEntry.StartDate = visa.StartDate;
                dbEntry.DueDate = visa.DueDate;
                dbEntry.Days = visa.Days;

                if (visa.DaysUsedInBT == null)
                    dbEntry.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                    dbEntry.DaysUsedInPrivateTrips = 0;
                else
                    dbEntry.DaysUsedInPrivateTrips = visa.DaysUsedInPrivateTrips;

                if (visa.CorrectionForVisaDays == null)
                    dbEntry.CorrectionForVisaDays = 0;
                else
                    dbEntry.CorrectionForVisaDays = visa.CorrectionForVisaDays;

                dbEntry.Entries = visa.Entries;

                if (visa.EntriesUsedInBT == null)
                    dbEntry.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                    dbEntry.EntriesUsedInPrivateTrips = 0;
                else
                    dbEntry.EntriesUsedInPrivateTrips = visa.EntriesUsedInPrivateTrips;

                if (visa.CorrectionForVisaEntries == null)
                    dbEntry.CorrectionForVisaEntries = 0;
                else
                    dbEntry.CorrectionForVisaEntries = visa.CorrectionForVisaEntries;
            }
            else
            {
                //Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();

                if (visa.DaysUsedInBT == null)
                    visa.DaysUsedInBT = 0;
                if (visa.DaysUsedInPrivateTrips == null)
                    visa.DaysUsedInPrivateTrips = 0;
                if (visa.CorrectionForVisaDays == null)
                    visa.CorrectionForVisaDays = 0;

                if (visa.EntriesUsedInBT == null)
                    visa.EntriesUsedInBT = 0;
                if (visa.EntriesUsedInPrivateTrips == null)
                    visa.EntriesUsedInPrivateTrips = 0;
                if (visa.CorrectionForVisaEntries == null)
                    visa.CorrectionForVisaEntries = 0;

                context.Visas.Add(visa);
            }
            context.SaveChanges();
        }

        public Visa DeleteVisa(int employeeID)
        {
            Visa dbEntry = Visas.Where(v => v.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.Visas.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region VisaRegistrationDates
        public IList<VisaRegistrationDate> VisaRegistrationDates
        {
            get { return context.VisaRegistrationDates.ToList<VisaRegistrationDate>(); }
        }

        public void SaveVisaRegistrationDate(VisaRegistrationDate visaRegistrationDate, int id)
        {
            VisaRegistrationDate dbEntry = (from vrd in context.VisaRegistrationDates where vrd.EmployeeID == visaRegistrationDate.EmployeeID select vrd).FirstOrDefault();

            if (dbEntry != null && visaRegistrationDate.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(visaRegistrationDate.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = visaRegistrationDate.EmployeeID;
                dbEntry.VisaType = visaRegistrationDate.VisaType;
                dbEntry.RegistrationDate = visaRegistrationDate.RegistrationDate;
                dbEntry.City = visaRegistrationDate.City;
                dbEntry.RegistrationNumber = visaRegistrationDate.RegistrationNumber;
                dbEntry.RegistrationTime = visaRegistrationDate.RegistrationTime;
                dbEntry.PaymentDate = visaRegistrationDate.PaymentDate;
                dbEntry.PaymentTime = visaRegistrationDate.PaymentTime;
                dbEntry.PaymentPIN = visaRegistrationDate.PaymentPIN;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();

                context.VisaRegistrationDates.Add(visaRegistrationDate);
            }

            context.SaveChanges();
        }

        public VisaRegistrationDate DeleteVisaRegistrationDate(int employeeID)
        {
            VisaRegistrationDate dbEntry = context.VisaRegistrationDates.Where(vrd => vrd.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.VisaRegistrationDates.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Permits
        public IList<Permit> Permits
        {
            get { return context.Permits.ToList<Permit>(); }
        }

        public void SavePermit(Permit permit, int id)
        {
            Permit dbEntry = (from p in context.Permits where p.EmployeeID == permit.EmployeeID select p).FirstOrDefault();

            if (dbEntry != null && permit.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(permit.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

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
                //permit.PermitOf = employee;

                context.Permits.Add(permit);
            }
            context.SaveChanges();
        }

        public Permit DeletePermit(int employeeID)
        {
            Permit dbEntry = context.Permits.Where(p => p.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.Permits.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region BusinessTrips
        public IList<BusinessTrip> BusinessTrips
        {
            get { return context.BusinessTrips.ToList<BusinessTrip>(); }
        }


        int DaysBetweenStartDateAndOrderStartDate = new int();
        int DaysBetweenOrderEndDateAndEndDate = new int();


        public void SaveBusinessTrip(BusinessTrip bt)
        {
            if (!CheckBTCreationPossibility(bt))
            {
                throw new VacationAlreadyExistException();
            }

            BusinessTrip dbEntry = (from b in BusinessTrips where b.BusinessTripID == bt.BusinessTripID select b).FirstOrDefault();

            if (dbEntry != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(bt.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                if ((dbEntry.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (dbEntry.Status != BTStatus.Cancelled) && (dbEntry.Status != bt.Status))
                {
                    DeleteBusinessTripCalendarItem(bt, dbEntry);
                }

                else if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled) && (dbEntry.Status != bt.Status))
                {
                    CreateBusinessTripCalendarItem(bt, dbEntry);
                }

                else if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled) && (dbEntry.Status == bt.Status))
                {
                    DeleteBusinessTripCalendarItem(bt, dbEntry);
                    CreateBusinessTripCalendarItem(bt, dbEntry);
                }

                CalendarItem item = (from cItem in CalendarItems
                                     where
                                         cItem.From == bt.OldStartDate
                                         && cItem.To == bt.OldEndDate
                                         && cItem.Type == CalendarItemType.BT
                                         && cItem.Location == bt.OldLocationTitle
                                         && cItem.EmployeeID == bt.BTof.EmployeeID
                                     select cItem).FirstOrDefault();


                dbEntry.StartDate = (bt.StartDate == default(DateTime)) ? dbEntry.StartDate : bt.StartDate;
                dbEntry.EndDate = bt.EndDate == default(DateTime) ? dbEntry.EndDate : bt.EndDate;
                dbEntry.Status = bt.Status;
                dbEntry.LocationID = bt.LocationID == 0 ? dbEntry.LocationID : bt.LocationID;
                dbEntry.Location = (from loc in context.Locations where loc.LocationID == bt.LocationID select loc).FirstOrDefault();
                dbEntry.Comment = bt.Comment;
                dbEntry.CancelComment = bt.CancelComment;
                dbEntry.RejectComment = bt.RejectComment;
                dbEntry.Flights = bt.Flights;
                dbEntry.FlightsConfirmed = bt.FlightsConfirmed;
                dbEntry.Habitation = bt.Habitation;
                dbEntry.HabitationConfirmed = bt.HabitationConfirmed;
                dbEntry.Invitation = bt.Invitation;
                dbEntry.Manager = (bt.Manager == null) ? "" : bt.Manager;
                dbEntry.OldStartDate = (bt.OldStartDate == default(DateTime)) ? dbEntry.OldStartDate : bt.OldStartDate;
                dbEntry.OldEndDate = (bt.OldEndDate == default(DateTime)) ? dbEntry.OldEndDate : bt.OldEndDate;
                dbEntry.OldLocationID = bt.OldLocationID == 0 ? dbEntry.OldLocationID : bt.OldLocationID;
                dbEntry.OldLocationTitle = (bt.OldLocationTitle == null) ? dbEntry.OldLocationTitle : bt.OldLocationTitle;
                dbEntry.Purpose = (bt.Purpose == null) ? "" : bt.Purpose;
                dbEntry.Responsible = (bt.Responsible == null) ? "" : bt.Responsible;
                dbEntry.LastCRUDedBy = (bt.LastCRUDedBy == null) ? dbEntry.LastCRUDedBy : bt.LastCRUDedBy;
                dbEntry.LastCRUDTimestamp = (bt.LastCRUDTimestamp == default(DateTime)) ? dbEntry.LastCRUDTimestamp : bt.LastCRUDTimestamp;
                dbEntry.BTMComment = bt.BTMComment;
                dbEntry.AccComment = bt.AccComment;
                dbEntry.UnitID = bt.UnitID == 0 ? dbEntry.UnitID : bt.UnitID;
                //dbEntry.Unit = (from unitItem in context.Units where unitItem.UnitID == bt.UnitID select unitItem).FirstOrDefault();

                #region CreateJourney
                if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled))
                {
                    if (bt.OrderStartDate != null && bt.OrderEndDate != null)
                    {
                        BusinessTrip existingBtsWithSameOrderDates;

                        existingBtsWithSameOrderDates = (from b in context.BusinessTrips
                                                         where b.EmployeeID == bt.EmployeeID
                                                         && b.BusinessTripID != bt.BusinessTripID
                                                         && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                                         && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                                         select b).FirstOrDefault();

                        if (existingBtsWithSameOrderDates == null)
                        {
                            if (dbEntry.OrderStartDate != null && dbEntry.OrderEndDate != null)
                            {
                                List<int> journeysID = new List<int>();
                                foreach (Journey jour in dbEntry.Journeys)
                                {
                                    journeysID.Add(jour.JourneyID);
                                }

                                foreach (int id in journeysID)
                                {
                                    DeleteJourney(id);
                                }
                            }
                            CreateJourney(bt);
                        }
                        else
                        {
                            var bts = (from b in context.BusinessTrips
                                       where b.EmployeeID == bt.EmployeeID
                                             && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                             && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                       select b).ToList();

                            List<int> journeysID = new List<int>();

                            foreach (BusinessTrip b in bts)
                            {
                                if (b.Journeys != null)
                                {
                                    foreach (Journey jour in b.Journeys)
                                    {
                                        journeysID.Add(jour.JourneyID);
                                    }
                                }
                            }

                            foreach (BusinessTrip bTrip in bts)
                            {
                                Employee employee = (from emp in Employees where emp.EmployeeID == bTrip.EmployeeID select emp).FirstOrDefault();
                                if (GetBTCalendarItem(bTrip, employee) != null)
                                {
                                    DeleteBusinessTripCalendarItem(bTrip, bTrip);
                                    CreateBusinessTripCalendarItem(bTrip, bTrip);
                                }

                            }

                            foreach (int id in journeysID)
                            {
                                DeleteJourney(id);

                            }

                            context.SaveChanges();
                            CreateJourneyFromPairedBts(bts);
                        }
                    }
                }

                #endregion

                dbEntry.OrderStartDate = (bt.OrderStartDate == null) ? dbEntry.OrderStartDate : bt.OrderStartDate;
                dbEntry.OrderEndDate = (bt.OrderEndDate == null) ? dbEntry.OrderEndDate : bt.OrderEndDate;

                dbEntry.OrderStartDate = bt.OrderStartDate;
                dbEntry.OrderEndDate = bt.OrderEndDate;
                dbEntry.DaysInBtForOrder = bt.DaysInBtForOrder;
                dbEntry.RowVersion = bt.RowVersion;

                if (item != null)
                {
                    item.From = dbEntry.StartDate;
                    item.To = dbEntry.EndDate;
                    item.Location = dbEntry.Location.Title;

                    SaveCalendarItem(item);
                }


            }
            else
            {
                context.BusinessTrips.Add(bt);
            }
            context.SaveChanges();
            RecreateOvrertimeForReclaim(bt); 
        }

        private void CreateBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            Employee employee = (from emp in Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = new CalendarItem();
                item.Employee = employee;
                item.EmployeeID = employee.EmployeeID;
                item.From = bt.StartDate;
                item.To = bt.EndDate;
                item.Location = Locations.Where(l => l.LocationID == bt.LocationID).Select(l => l.Title).FirstOrDefault(); 
                item.Type = CalendarItemType.BT;

                employee.CalendarItems.Add(item);
                SaveCalendarItem(item);
            }
        }

        private void DeleteBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            //delete CalendarItem
            Employee employee = (from emp in Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = GetBTCalendarItem(dbEntry, employee);

                List<Journey> journey = GetJourneysForBusinessTrip(bt);

                List<CalendarItem> items = GetJourneyCalendarItem(employee);

                if (item != null)
                {
                    employee.CalendarItems.Remove(item);
                    DeleteCalendarItem(item.CalendarItemID);
                }

                DeleteJourneyCalendarItems(employee, journey, items);

            }
        }

        private void DeleteJourneyCalendarItems(Employee employee, List<Journey> journey, List<CalendarItem> items)
        {
            foreach (CalendarItem it in items)
            {
                foreach (Journey j in journey)
                {
                    if (it.From == j.Date && it.To == j.Date)
                    {
                        CalendarItem linkedReclaimedOvertimeCalendarItem = employee.CalendarItems.Where(c => (c.From == j.ReclaimDate && c.To == j.ReclaimDate && c.Type == CalendarItemType.ReclaimedOvertime)).FirstOrDefault();
                        if (linkedReclaimedOvertimeCalendarItem != null)
                        {
                            employee.CalendarItems.Remove(linkedReclaimedOvertimeCalendarItem);
                            DeleteCalendarItem(linkedReclaimedOvertimeCalendarItem.CalendarItemID);
                        }                        
                        employee.CalendarItems.Remove(it);
                        DeleteCalendarItem(it.CalendarItemID);
                    }
                }
            }
        }

        private List<CalendarItem> GetJourneyCalendarItem(Employee employee)
        {
            List<CalendarItem> items = (from it in CalendarItems
                                        where
                                            it.EmployeeID == employee.EmployeeID &&
                                            it.Type == CalendarItemType.Journey
                                        select it).ToList();
            return items;
        }

        private List<Journey> GetJourneysForBusinessTrip(BusinessTrip bt)
        {
            List<Journey> journey = (from journ in Journeys
                                     where
                                         journ.BusinessTripID == bt.BusinessTripID
                                     select journ).ToList();
            return journey;
        }

        private static CalendarItem GetBTCalendarItem(BusinessTrip dbEntry, Employee employee)
        {
            CalendarItem item = (from i in employee.CalendarItems
                                 where i.Type == CalendarItemType.BT &&
                                     i.Location == dbEntry.Location.Title &&
                                     i.From == dbEntry.StartDate &&
                                     i.To == dbEntry.EndDate
                                 select i).FirstOrDefault();
            return item;
        }

        private void CreateJourney(BusinessTrip bt)
        {
            DaysBetweenStartDateAndOrderStartDate = (bt.StartDate - bt.OrderStartDate.Value).Days;
            DaysBetweenOrderEndDateAndEndDate = (bt.OrderEndDate.Value - bt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = bt.OrderStartDate.Value;
            DateTime? StartDateForJourneyForEndDifference = bt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            Employee emp = (from e in context.Employees where e.EmployeeID == bt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = bt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                SaveJourney(journey);

                item.EmployeeID = bt.EmployeeID;
                item.From = journey.Date;
                item.To = journey.Date;
                item.Type = CalendarItemType.Journey;

                emp.CalendarItems.Add(item);
                SaveCalendarItem(item);

                item = new CalendarItem();

                StartDateForJourneyForStartDifference = new DateTime?(StartDateForJourneyForStartDifference.Value.Date.AddDays(1));
            }

            for (int i = 0; i < DaysBetweenOrderEndDateAndEndDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = bt.BusinessTripID;
                journey.Date = StartDateForJourneyForEndDifference.Value;

                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }

                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                SaveJourney(journey);

                item.EmployeeID = bt.EmployeeID;
                item.From = journey.Date;
                item.To = journey.Date;
                item.Type = CalendarItemType.Journey;

                emp.CalendarItems.Add(item);
                SaveCalendarItem(item);
                item = new CalendarItem();

                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.Date.AddDays(1));
            }
        }

        private void CreateJourneyFromPairedBts(List<BusinessTrip> bts)
        {
            BusinessTrip firstBt = bts.OrderBy(bt => bt.StartDate).FirstOrDefault();
            BusinessTrip lastBt = bts.OrderByDescending(bt => bt.EndDate).FirstOrDefault();

            DaysBetweenStartDateAndOrderStartDate = (firstBt.StartDate - firstBt.OrderStartDate.Value).Days;
            DaysBetweenOrderEndDateAndEndDate = (lastBt.OrderEndDate.Value - lastBt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = firstBt.OrderStartDate;
            DateTime? StartDateForJourneyForEndDifference = lastBt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            Employee empFirstBt = (from e in context.Employees where e.EmployeeID == firstBt.EmployeeID select e).FirstOrDefault();
            Employee empLastBt = (from e in context.Employees where e.EmployeeID == lastBt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = firstBt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                if (Journeys.Where(j => ((j.BusinessTripID == firstBt.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    SaveJourney(journey);

                    if ((firstBt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (firstBt.Status != BTStatus.Cancelled))
                    {
                        item.EmployeeID = firstBt.EmployeeID;
                        item.From = journey.Date;
                        item.To = journey.Date;
                        item.Type = CalendarItemType.Journey;

                        empFirstBt.CalendarItems.Add(item);
                        SaveCalendarItem(item);
                        item = new CalendarItem();
                    }
                }
                StartDateForJourneyForStartDifference = new DateTime?(StartDateForJourneyForStartDifference.Value.AddDays(1));
            }

            for (int i = 0; i < DaysBetweenOrderEndDateAndEndDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = lastBt.BusinessTripID;
                journey.Date = StartDateForJourneyForEndDifference.Value;

                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }

                foreach (Holiday hol in context.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                if (Journeys.Where(j => ((j.BusinessTripID == lastBt.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    SaveJourney(journey);



                    item.EmployeeID = lastBt.EmployeeID;
                    item.From = journey.Date;
                    item.To = journey.Date;
                    item.Type = CalendarItemType.Journey;

                    empLastBt.CalendarItems.Add(item);
                    SaveCalendarItem(item);
                    item = new CalendarItem();

                }
                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.AddDays(1));
            }
        }

        private bool CheckBTCreationPossibility(BusinessTrip bTrip)
        {
            //Select all CalendarItems for current User
            List<CalendarItem> vacationsList = (from item in CalendarItems
                                                where item.EmployeeID == bTrip.EmployeeID &&
                                                (item.Type != CalendarItemType.BT && item.Type != CalendarItemType.Journey && item.Type != CalendarItemType.OvertimeForReclaim)
                                                select item).ToList();
            if (vacationsList.Count > 0)
            {
                //Check time periods by OrderDates
                if (bTrip.OrderStartDate.HasValue && bTrip.OrderEndDate.HasValue)
                {
                    foreach (CalendarItem vacations in vacationsList)
                    {
                        if (bTrip.OrderStartDate.Value <= vacations.From && bTrip.OrderEndDate.Value >= vacations.From ||
                            bTrip.OrderStartDate.Value >= vacations.From && bTrip.OrderStartDate.Value <= vacations.To)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                //Check time periods by Start and EndDates
                else
                {
                    foreach (CalendarItem vacations in vacationsList)
                    {
                        if (bTrip.StartDate <= vacations.From && bTrip.EndDate >= vacations.From ||
                            bTrip.StartDate >= vacations.From && bTrip.StartDate <= vacations.To)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return true;
        }

        private bool RecreateOvrertimeForReclaim(BusinessTrip bTrip)
        {
            //Select all CalendarItems for current User
            List<CalendarItem> vacationsList = (from item in CalendarItems
                                                where item.EmployeeID == bTrip.EmployeeID &&
                                                (item.Type == CalendarItemType.OvertimeForReclaim)
                                                select item).ToList();
            if (vacationsList.Count > 0)
            {
                //Check time periods by OrderDates
                if (bTrip.OrderStartDate.HasValue && bTrip.OrderEndDate.HasValue)
                {
                    foreach (CalendarItem vacations in vacationsList)
                    {
                        if (bTrip.OrderStartDate.Value <= vacations.From && bTrip.OrderEndDate.Value >= vacations.From ||
                            bTrip.OrderStartDate.Value >= vacations.From && bTrip.OrderStartDate.Value <= vacations.To)
                        {
                            context.CalendarItems.Remove(vacations);
                            context.SaveChanges();
                            context.CalendarItems.Add(vacations);
                            context.SaveChanges();
                            return true;
                        }
                    }
                    return false;
                }
                //Check time periods by Start and EndDates
                else
                {
                    foreach (CalendarItem vacations in vacationsList)
                    {
                        if (bTrip.StartDate <= vacations.From && bTrip.EndDate >= vacations.From ||
                            bTrip.StartDate >= vacations.From && bTrip.StartDate <= vacations.To)
                        {
                            context.CalendarItems.Remove(vacations);
                            context.SaveChanges();
                            context.CalendarItems.Add(vacations);
                            context.SaveChanges(); 
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        public BusinessTrip DeleteBusinessTrip(int btID)
        {
            BusinessTrip dbEntry = BusinessTrips.Where(b => b.BusinessTripID == btID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.BusinessTrips.Remove(dbEntry);
            }

            context.SaveChanges();

            return dbEntry;
        }

        #endregion

        #region Locations
        public IList<Location> Locations
        {
            get { return context.Locations.ToList<Location>(); }
        }

        public void SaveLocation(Location location)
        {
            if (location.LocationID == 0)
                context.Locations.Add(location);
            else
            {
                Location dbEntry = context.Locations.Find(location.LocationID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(location.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.Title = location.Title;
                    dbEntry.Address = location.Address;
                    dbEntry.CountryID = location.CountryID;
                    dbEntry.ResponsibleForLoc = location.ResponsibleForLoc;
                }
            }
            context.SaveChanges();
        }

        public Location DeleteLocation(int locationID)
        {
            Location dbEntry = context.Locations.Find(locationID);

            if (dbEntry != null)
            {
                context.Locations.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion


        #region Units

        public IList<Unit> Units
        {
            get { return context.Units.ToList<Unit>(); }
        }

        public void SaveUnit(Unit unit)
        {
            if (unit.UnitID == 0)
                context.Units.Add(unit);
            else
            {
                Unit dbEntry = context.Units.Find(unit.UnitID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(unit.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.Title = unit.Title;
                    dbEntry.ShortTitle = unit.ShortTitle;
                }
            }
            context.SaveChanges();
        }

        public Unit DeleteUnit(int unitID)
        {
            Unit dbEntry = context.Units.Find(unitID);

            if (dbEntry != null)
            {
                context.Units.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion

        #region Sicknesses
        public IList<Sickness> Sicknesses
        {
            get { return context.Sicknesses.ToList<Sickness>(); }
        }
        public void SaveSick(Sickness sick)
        {
            if (sick.SickID == 0)
            {
                sick.Workdays = CalculateWorkDays(sick);
                context.Sicknesses.Add(sick);
            }
            else
            {
                Sickness dbEntry = context.Sicknesses.Find(sick.SickID);
                if (dbEntry != null)
                {
                    dbEntry.From = sick.From;
                    dbEntry.EmployeeID = sick.EmployeeID;
                    dbEntry.To = sick.To;
                    dbEntry.SicknessType = sick.SicknessType;
                    dbEntry.Workdays = CalculateWorkDays(sick);
                }

            }
            context.SaveChanges();
        }

        private int CalculateWorkDays(Sickness sick)
        {

            int numberOfWorkingDays = 0;
            DateTime loopFrom = sick.From;
            while (loopFrom <= sick.To)
            {
                if (ISWorkDay(sick.From) && IsNotNationalHolidayExcludeWeekends(sick.From))
                {
                    numberOfWorkingDays++;
                }
                loopFrom = new DateTime(loopFrom.Year, loopFrom.Month, loopFrom.Day).Date.AddDays(1);
            }

            return numberOfWorkingDays;

        }
        private static bool ISWorkDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private bool IsNotNationalHolidayExcludeWeekends(DateTime date)
        {
            List<DateTime> holidays = (from h in Holidays where (h.CountryID == 1) && (h.HolidayDate.DayOfWeek != DayOfWeek.Saturday && h.HolidayDate.DayOfWeek != DayOfWeek.Sunday) select h.HolidayDate).ToList();
            return !holidays.Contains(date);
        }
        public Sickness DeleteSick(int SickID)
        {
            Sickness dbEntry = context.Sicknesses.Find(SickID);
            if (dbEntry != null)
            {
                context.Sicknesses.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }


        #endregion

        #region Roles

        public void SaveRolesForEmployee(string username, string[] roles)
        {
            if (Membership.GetUser(username) != null)
            {
                UpdateUserAndRoles(username, roles);
            }
            else
            {
                CreateUserWithRoles(username, roles);
            }
        }

        private void UpdateUserAndRoles(string username, string[] roles)
        {
            if (roles != null)
            {
                RemoveRolesFromUser(username);
                AddRolesForUser(username, roles);
            }
            else
            {
                DeleteUser(username);
            }
        }

        private static void CreateUserWithRoles(string username, string[] roles)
        {
            if (roles != null)
            {
                WebSecurity.CreateUserAndAccount(username, ConfigurationManager.AppSettings["DefaultPassword"]);
                AddRolesForUser(username, roles);
            }
        }

        private static void RemoveRolesFromUser(string username)
        {
            foreach (string role in Roles.GetRolesForUser(username))
            {
                Roles.RemoveUserFromRole(username, role);
            }
        }

        private static void AddRolesForUser(string username, string[] roles)
        {
            foreach (string role in roles)
            {
                if (!Roles.IsUserInRole(username, role))
                {
                    Roles.AddUserToRole(username, role);
                }
            }
        }

        public void DeleteUser(string username)
        {
            if (Membership.GetUser(username) != null)
            {
                RemoveRolesFromUser(username);
                Membership.DeleteUser(username);
            }
        }

        #endregion

        #region Messages

        public IList<IMessage> Messages
        {
            get { return context.Messages.ToList<IMessage>(); }
        }

        public void SaveMessage(IMessage message)
        {
            if (message.MessageID == 0)
                context.Messages.Add(message as Message);
            else
            {
                IMessage dbEntry = context.Messages.Find(message.MessageID);
                if (dbEntry != null)
                {
                    dbEntry.MessageID = message.MessageID;
                    dbEntry.Role = message.Role;
                    dbEntry.Subject = message.Subject;
                    dbEntry.Body = message.Body;
                    dbEntry.Link = message.Link;
                    dbEntry.TimeStamp = message.TimeStamp;
                    dbEntry.ReplyTo = message.ReplyTo;
                    dbEntry.FullName = message.FullName;
                }
            }
            context.SaveChanges();
        }

        public IMessage DeleteMessage(int MessageID)
        {
            IMessage dbEntry = context.Messages.Find(MessageID);

            if (dbEntry != null)
            {
                context.Messages.Remove(dbEntry as Message);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Passports

        public IList<Passport> Passports
        {
            get { return context.Passports.ToList<Passport>(); }
        }

        public void SavePassport(Passport passport)
        {
            Passport dbEntry = context.Passports.Find(passport.EmployeeID);

            if (dbEntry != null && passport.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(passport.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = passport.EmployeeID;
                dbEntry.EndDate = (passport.EndDate == null) ? dbEntry.EndDate : passport.EndDate;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == passport.EmployeeID).SingleOrDefault();
                //passport.PassportOf = employee;

                context.Passports.Add(passport);
            }
            context.SaveChanges();
        }

        public Passport DeletePassport(int employeeID)
        {
            Passport dbEntry = context.Passports.Find(employeeID);
            if (dbEntry != null)
            {
                context.Passports.Remove(dbEntry as Passport);
                context.SaveChanges();
            };
            return dbEntry;
        }

        #endregion

        #region PrivateTrip
        public IList<PrivateTrip> PrivateTrips
        {
            get { return context.PrivateTrips.ToList<PrivateTrip>(); }
        }

        public void SavePrivateTrip(PrivateTrip pt)
        {
            PrivateTrip dbEntry = (from p in PrivateTrips where p.PrivateTripID == pt.PrivateTripID select p).FirstOrDefault();

            if (dbEntry != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(pt.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }
                dbEntry.StartDate = (pt.StartDate == default(DateTime)) ? dbEntry.StartDate : pt.StartDate;
                dbEntry.EndDate = (pt.EndDate == default(DateTime)) ? dbEntry.EndDate : pt.EndDate;
            }
            else
            {
                context.PrivateTrips.Add(pt);
            }

            context.SaveChanges();
        }

        public PrivateTrip DeletePrivateTrip(int ptID)
        {
            PrivateTrip dbEntry = PrivateTrips.Where(p => p.PrivateTripID == ptID).SingleOrDefault();

            if (dbEntry != null)
            {

                context.PrivateTrips.Remove(dbEntry);
            }

            context.SaveChanges();

            return dbEntry;
        }

        #endregion

        #region Position

        public IList<Position> Positions
        {
            get { return context.Positions.ToList<Position>(); }
        }

        public void SavePosition(Position position)
        {
            if (position.PositionID == 0)
            {
                context.Positions.Add(position);
            }
            else
            {
                Position dbEntry = context.Positions.Find(position.PositionID);
                if (dbEntry != null)
                {
                    if (!dbEntry.RowVersion.SequenceEqual(position.RowVersion))
                    {
                        throw new DbUpdateConcurrencyException();
                    }

                    dbEntry.TitleEn = position.TitleEn;
                    dbEntry.TitleUk = position.TitleUk;
                    dbEntry.Employees = position.Employees;
                }
            }

            context.SaveChanges();
        }

        public Position DeletePosition(int positionID)
        {
            Position dbEntry = context.Positions.Find(positionID);

            if (dbEntry != null)
            {
                context.Positions.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Countries

        public IList<Country> Countries
        {
            get { return context.Countries.ToList<Country>(); }
        }

        public void SaveCountry(Country country)
        {
            if (country.CountryID == 0)
            {
                context.Countries.Add(country);
            }
            else
            {
                Country dbentry = context.Countries.Find(country.CountryID);
                if (dbentry != null)
                {
                    dbentry.Comment = country.Comment;
                    dbentry.CountryName = country.CountryName;
                    dbentry.Holidays = country.Holidays;
                    dbentry.Locations = country.Locations;
                }
            }
            context.SaveChanges();
        }

        public Country DeleteCountry(int countryID)
        {
            Country dbentry = context.Countries.Find(countryID);

            if (dbentry != null)
            {
                context.Countries.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Holidays

        public IList<Holiday> Holidays
        {
            get { return context.Holidays.ToList<Holiday>(); }
        }

        public void SaveHoliday(Holiday holiday)
        {
            if (holiday.HolidayID == 0)
            {
                Country con = context.Countries.Where(c => c.CountryID == holiday.CountryID).FirstOrDefault();

                holiday.Country = con;

                context.Holidays.Add(holiday);
            }
            else
            {
                Holiday dbentry = context.Holidays.Find(holiday.HolidayID);

                if (dbentry != null)
                {
                    dbentry.Title = holiday.Title;
                    dbentry.HolidayComment = holiday.HolidayComment;
                    dbentry.HolidayDate = holiday.HolidayDate;
                    dbentry.IsPostponed = holiday.IsPostponed;
                }
            }

            context.SaveChanges();
        }

        public Holiday DeleteHoliday(int holidayID)
        {
            Holiday dbentry = context.Holidays.Find(holidayID);

            if (dbentry != null)
            {
                context.Holidays.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Journeys

        public IList<Journey> Journeys
        {
            get { return context.Journeys.ToList<Journey>(); }
        }

        public void SaveJourney(Journey journey)
        {
            if (journey.JourneyID == 0)
            {
                context.Journeys.Add(journey);
            }
            else
            {
                Journey dbEntry = context.Journeys.Find(journey.JourneyID);
                if (dbEntry != null)
                {
                    dbEntry = journey;
                    //dbEntry.BusinessTripID = journey.BusinessTripID;
                    //dbEntry.Date = journey.Date;
                    //dbEntry.DayOff = journey.DayOff;
                    //dbEntry.JourneyID = journey.JourneyID;
                    //dbEntry.ReclaimDate = journey.ReclaimDate;
                    //dbEntry.Date = journey.Date;
                    //dbEntry.DayOff = journey.DayOff;
                    //dbEntry.ReclaimDate = journey.ReclaimDate;
                }

            }
            context.SaveChanges();
        }

        public Journey DeleteJourney(int journeyID)
        {
            Journey dbEntry = context.Journeys.Find(journeyID);

            if (dbEntry != null)
            {
                context.Journeys.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region CalendarItem

        public IList<CalendarItem> CalendarItems
        {
            get { return context.CalendarItems.ToList<CalendarItem>(); }
        }

        public void SaveCalendarItem(CalendarItem calendarItem)
        {
            if (calendarItem.CalendarItemID == 0)
            {
                context.CalendarItems.Add(calendarItem);
            }
            else
            {
                CalendarItem dbentry = context.CalendarItems.Find(calendarItem.CalendarItemID);
                if (dbentry != null)
                {
                    dbentry.CalendarItemID = calendarItem.CalendarItemID;
                    dbentry.Employee = calendarItem.Employee;
                    dbentry.EmployeeID = calendarItem.EmployeeID;
                    dbentry.From = calendarItem.From;
                    dbentry.Location = calendarItem.Location;
                    dbentry.To = calendarItem.To;
                    dbentry.Type = calendarItem.Type;
                }
            }
            context.SaveChanges();
        }

        public CalendarItem DeleteCalendarItem(int calendarID)
        {
            CalendarItem dbEntry = context.CalendarItems.Find(calendarID);

            if (dbEntry != null)
            {
                context.CalendarItems.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion

        #region Overtime

        public IList<Overtime> Overtimes
        {
            get { return context.Overtimes.ToList<Overtime>(); }
        }

        public void SaveOvertime(Overtime overtime)
        {
            if (overtime.OvertimeID == 0)
            {
                context.Overtimes.Add(overtime);
            }
            else
            {
                Overtime dbentry = context.Overtimes.Find(overtime.OvertimeID);

                if (dbentry != null)
                {
                    dbentry.Date = overtime.Date;
                    dbentry.Employee = overtime.Employee;
                    dbentry.EmployeeID = overtime.EmployeeID;
                    dbentry.OvertimeID = overtime.OvertimeID;
                    dbentry.ReclaimDate = overtime.ReclaimDate;
                    dbentry.Type = overtime.Type;
                    dbentry.DayOff = overtime.DayOff;
                }
            }

            context.SaveChanges();
        }

        public Overtime DeleteOvertime(int overtimeID)
        {
            Overtime dbEntry = context.Overtimes.Find(overtimeID);

            if (dbEntry != null)
            {
                context.Overtimes.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
        #endregion

        #region Vacation

        public IList<Vacation> Vacations
        {
            get { return context.Vacations.ToList<Vacation>(); }
        }

        public void SaveVacation(Vacation vacation)
        {
            if (vacation.VacationID == 0)
            {
                context.Vacations.Add(vacation);
            }
            else
            {
                Vacation dbentry = context.Vacations.Find(vacation.VacationID);

                if (dbentry != null)
                {
                    dbentry = vacation;
                }
            }
            context.SaveChanges();
        }

        public Vacation DeleteVacation(int vacationID)
        {
            Vacation dbentry = context.Vacations.Find(vacationID);

            if (dbentry != null)
            {
                context.Vacations.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

        #region Users
        public IList<Employee> Users
        {
            get { return context.Employees.ToList<Employee>(); }
            //get { return context.Users; }
        }

        #endregion

        #region Greeting

        public IList<Greeting> Greetings
        {
            get { return context.Greetings.ToList<Greeting>(); }
        }

        public void SaveGreeting(Greeting greeting)
        {
            if (greeting.GreetingId == 0)
            {
                context.Greetings.Add(greeting);
            }
            else
            {
                Greeting dbentry = context.Greetings.Find(greeting.GreetingId);

                if (dbentry != null)
                {
                    dbentry.GreetingHeader = greeting.GreetingHeader;
                    dbentry.GreetingBody = greeting.GreetingBody;
                }
            }
            context.SaveChanges();
        }

        public Greeting DeleteGreeting(int GreetingId)
        {

            Greeting dbentry = context.Greetings.Find(GreetingId);

            if (dbentry != null)
            {
                context.Greetings.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;

        }

        #endregion

        #region Insurances
        public IList<Insurance> Insurances
        {
            get { return context.Insurances.ToList<Insurance>(); }
        }

        public void SaveInsurance(Insurance insurance, int id)
        {
            Insurance dbEntry = (from i in context.Insurances where i.EmployeeID == insurance.EmployeeID select i).FirstOrDefault();

            if (dbEntry != null && insurance.RowVersion != null)
            {
                if (!dbEntry.RowVersion.SequenceEqual(insurance.RowVersion))
                {
                    throw new DbUpdateConcurrencyException();
                }

                dbEntry.EmployeeID = insurance.EmployeeID;
                dbEntry.StartDate = insurance.StartDate;
                dbEntry.EndDate = insurance.EndDate;
                dbEntry.Days = insurance.Days;
            }
            else
            {
                Employee employee = Employees.Where(e => e.EmployeeID == id).SingleOrDefault();
                //permit.PermitOf = employee;

                context.Insurances.Add(insurance);
            }
            context.SaveChanges();
        }

        public Insurance DeleteInsurance(int employeeID)
        {
            Insurance dbEntry = context.Insurances.Where(p => p.EmployeeID == employeeID).SingleOrDefault();

            if (dbEntry != null)
            {
                context.Insurances.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        #endregion
        
        #region AbsenceData

        public IList<AbsenceViewModel> SearchAbsenceData(DateTime fromDate, DateTime toDate, string searchString = "")
        {
            List<CalendarItem> calendarItemsList = SearchCalendarItemsByEmployeeAndDepartment(fromDate, toDate, searchString);

            List<int> empID = calendarItemsList.Select(id => id.EmployeeID).Distinct().ToList();
            empID.Sort();
            List<AbsenceViewModel> absenceData = new List<AbsenceViewModel>();

            foreach (int id in empID)
            {
                AbsenceViewModel model = new AbsenceViewModel();

                Employee emp = (from e in context.Employees where e.EmployeeID == id select e).FirstOrDefault();

                model.Department = emp.Department.DepartmentName;
                model.EID = emp.EID;
                model.EmployeeID = emp.EmployeeID;
                model.FirstName = emp.FirstName;
                model.LastName = emp.LastName;
                if (emp.DateDismissed != null)
                {
                    model.DateDismissed = emp.DateDismissed.Value.ToShortDateString();
                }
                model.Journeys = new List<CalendarItem>();
                model.Overtimes = new List<CalendarItem>();
                model.Sickness = new List<CalendarItem>();
                model.Vacations = new List<CalendarItem>();
                model.BusinessTrips = new List<CalendarItem>();

                absenceData.Add(model);
            }

            absenceData.Sort((x, y) => x.Department.CompareTo(y.Department)); 

            foreach (CalendarItem item in calendarItemsList)
            {

                AbsenceViewModel temp = absenceData.Where(id => id.EmployeeID == item.EmployeeID).FirstOrDefault();
                if (temp != null)
                {
                    switch (item.Type)
                    {
                        case CalendarItemType.Journey:

                            temp.Journeys.Add(item);
                            break;

                        case CalendarItemType.ReclaimedOvertime:
                        case CalendarItemType.PrivateMinus:

                            temp.Overtimes.Add(item);
                            break;

                        case CalendarItemType.PaidVacation:
                        case CalendarItemType.UnpaidVacation:

                            temp.Vacations.Add(item);
                            break;

                        case CalendarItemType.SickAbsence:

                            temp.Sickness.Add(item);
                            break;

                        case CalendarItemType.BT:
                            temp.BusinessTrips.Add(item);
                            break;
                    }
                }
            }
            foreach (AbsenceViewModel model in absenceData)
            {
                model.Journeys.Sort((x, y) => x.From.CompareTo(y.From));
                model.Overtimes.Sort((x, y) => x.From.CompareTo(y.From));
                model.Vacations.Sort((x, y) => x.From.CompareTo(y.From));
                model.Sickness.Sort((x, y) => x.From.CompareTo(y.From));
                model.BusinessTrips.Sort((x, y) => x.From.CompareTo(y.From));
            }

            return absenceData;
        }

        private List<CalendarItem> SearchCalendarItemsByEmployeeAndDepartment(DateTime fromDate, DateTime toDate, string searchString = "")
        {
            List<Employee> empList = context.Employees.ToList();

            List<CalendarItem> query = (from emp in empList
                                        where ((emp.CalendarItems.Count != 0) && ((emp.Department.DepartmentName.ToLower().Contains(searchString.ToLower())) || (emp.FirstName.ToLower().Contains(searchString.ToLower())) ||
                                            emp.LastName.ToLower().Contains(searchString.ToLower()) ||
                                            emp.EID.ToLower().Contains(searchString.ToLower())))
                                        from f in emp.CalendarItems
                                        where ((f.From <= fromDate && f.To >= fromDate) || (f.From >= fromDate && f.From <= toDate))
                                        orderby emp.Department.DepartmentID, emp.LastName
                                        select f).Distinct().ToList();
            return query;
        }

        #endregion 

        #region UsersData 
            
        public IList<EmployeeViewModel> SearchUsersData(string selectedDepartment, string searchString)
        {
            List<EmployeeViewModel> data = new List<EmployeeViewModel>();
            data = (from emp in Users
                    where ((selectedDepartment == null || selectedDepartment == String.Empty || (emp.Department != null && emp.Department.DepartmentName == selectedDepartment))
                            && (emp.EID.ToLower().Contains(searchString.ToLower())
                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                            || ((emp.DateEmployed != null) && emp.DateEmployed.Value != null && emp.DateEmployed.Value.ToShortDateString().Contains(searchString))
                            || ((emp.DateDismissed != null) && emp.DateDismissed.Value != null && emp.DateDismissed.Value.ToString().Contains(searchString))
                            || ((emp.BirthDay != null) && emp.BirthDay.Value != null && emp.BirthDay.Value.ToString().Contains(searchString))
                            || ((emp.FullNameUk != null) && emp.FullNameUk.ToLower().Contains(searchString.ToLower()))
                            || ((emp.Position != null) && emp.Position.TitleEn.ToLower().Contains(searchString.ToLower()))
                            ||
                                  ((System.Web.Security.Membership.GetUser(emp.EID) != null)
                                  && System.Web.Security.Roles.GetRolesForUser(emp.EID) != null && String.Join(", ", System.Web.Security.Roles.GetRolesForUser(emp.EID)).ToLower().Contains(searchString.ToLower()))))
                    orderby emp.IsManager descending, emp.LastName
                    select new EmployeeViewModel(emp)).ToList();

            return data;
        }

        #endregion 

        #region BusinessTripsData

        public IList<BusinessTripViewModel> GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(int selectedYear)
        {
            var query = GetBusinessTripDataByUnits(selectedYear).Where(bt => bt.BTof.DateDismissed == null &&
                bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                             || bt.Status == (BTStatus.Confirmed | BTStatus.Modified));
            return query.ToList();
        }

        public IList<BusinessTripViewModel> GetBusinessTripDataByUnits(int selectedYear)
        {
            int FirstBusinessTripIdInYear = GetFirstBusinessTripIdInYear(selectedYear);
            var query = from bt in context.BusinessTrips.AsEnumerable()
                        join emp in context.Employees on bt.EmployeeID equals emp.EmployeeID
                        join loc in context.Locations on bt.LocationID equals loc.LocationID
                        where (bt.StartDate.Year == selectedYear
                              && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                              || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                              || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                        orderby bt.BusinessTripID
                        select new BusinessTripViewModel(bt, CalculateId(bt.BusinessTripID, FirstBusinessTripIdInYear));
            return query.ToList();
        }

        public int GetFirstBusinessTripIdInYear(int selectedYear)
        {
            var query = (from bt in context.BusinessTrips.AsEnumerable()
                         join emp in context.Employees on bt.EmployeeID equals emp.EmployeeID
                         join loc in context.Locations on bt.LocationID equals loc.LocationID
                         where (bt.StartDate.Year == selectedYear
                               && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                               || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                               || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                         orderby bt.BusinessTripID
                         select bt.BusinessTripID).FirstOrDefault();
            return query;
        }

        public int CalculateId(int BusinessTripID, int FirstBusinessTripID)
        {
            return BusinessTripID - FirstBusinessTripID + 1;
        }

        #endregion

        #region SearchVisaData

          public IList<Employee> SearchVisaData(string searchString)  
        {
            var employees = Employees.ToList();
            return SelectVisaData(searchString, employees);
        }

          private static IList<Employee> SelectVisaData(string searchString, List<Employee> employees)
          {
              List<Employee> selected = (from emp in employees
                                         where emp.EID.ToLower().Contains(searchString.ToLower())
                                              || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                              || emp.LastName.ToLower().Contains(searchString.ToLower())
                                   || (emp.Visa != null)
                                                   && (emp.Visa.VisaType.ToLower().Contains(searchString.ToLower())
                                        || emp.Visa.StartDate.ToString().Contains(searchString)
                                        || emp.Visa.DueDate.ToString().Contains(searchString)
                                                   || emp.Visa.Entries == 0 && searchString.ToLower().Contains("mult"))
                                   || (emp.VisaRegistrationDate != null
                                        && emp.VisaRegistrationDate.RegistrationDate.ToString().Contains(searchString))
                                   || (emp.Permit != null)
                                        && (emp.Permit.StartDate.ToString().Contains(searchString)
                                        || emp.Permit.EndDate.ToString().Contains(searchString))
                                        || (emp.Insurance != null)
                                        && (emp.Insurance.ToString().Contains(searchString)
                                        || emp.Insurance.EndDate.ToString().Contains(searchString)
                                        || emp.Insurance.Days == 0)

                                         orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                                         select emp).ToList();
              return selected;
          }

          public IList<Employee> SearchVisaDataExcludingDismissed(string searchString)
          {
              var employeesWithoutDismiseed = Employees.Where(e => e.DateDismissed == null).ToList();
              return SelectVisaData(searchString, employeesWithoutDismiseed);
          }

        #endregion

        #region SearchEmployeeData 

          public IList<WTRViewModel> SearchWTRData(DateTime fromDate, DateTime toDate, string searchString = "")
          {
              IList<Employee> selectedData = SearchEmployeeData(fromDate, toDate, searchString);
              List<WTRViewModel> wtrDataList = new List<WTRViewModel>();



              foreach (var emp in selectedData)
              {
                  WTRViewModel onePerson = new WTRViewModel(emp, fromDate, toDate);
                  wtrDataList.Add(onePerson); 
              }
              return wtrDataList; 
          }

          public IList<WTRViewModel> SearchWTRDataPerEMP(DateTime fromDate, DateTime toDate, Employee employee)
          {
              List<WTRViewModel> wtrDataList = new List<WTRViewModel>();
             
              WTRViewModel onePerson = new WTRViewModel(employee, fromDate, toDate);
              wtrDataList.Add(onePerson);
              
              return wtrDataList; 
          }

          private IList<Employee> SearchEmployeeData(DateTime FromDate, DateTime ToDate, string searchString = "")
          {
              var employees = context.Employees.ToList();
              List<Employee> query = (from emp in employees
                                      where ((emp.CalendarItems.Count != 0))
                                      from f in emp.CalendarItems
                                      where (((f.From <= FromDate && f.To >= FromDate) || (f.From >= FromDate && f.From <= ToDate)) &&
                                          ((emp.FirstName.ToLower().Contains(searchString.ToLower())) ||
                                          emp.LastName.ToLower().Contains(searchString.ToLower()) ||
                                          emp.EID.ToLower().Contains(searchString.ToLower())))
                                      orderby emp.DateDismissed, emp.DateEmployed, emp.LastName
                                      select emp).Distinct().ToList();
              return query;
          }

          

        #endregion

          #region GetMails

          public IList<Employee> GetCurrentlyEmployedEmployees(string selectedDepartment)
          {
              List<Employee> eIds = Employees.Where(emp => selectedDepartment == "" || emp.Department.DepartmentName == selectedDepartment).Where(emp => emp.DateDismissed == null).ToList();
              return eIds;
          }

        #endregion

        #region QuestionSets
        public IList<QuestionSet> QuestionSets
        {
            get { return context.QuestionSets.ToList<QuestionSet>(); }
        }

        public void SaveQuestionSet(QuestionSet questionSet)
        {
            if (questionSet.QuestionSetId == 0)
            {
                context.QuestionSets.Add(questionSet);
            }
            else
            {
                QuestionSet dbentry = context.QuestionSets.Find(questionSet.QuestionSetId);
                if (dbentry != null)
                { 
                    dbentry.Questions = questionSet.Questions;
                    dbentry.Title = questionSet.Title; 
                }
            }
            context.SaveChanges();
        }

        public QuestionSet DeleteQuestionSet(int questionSetId)
        {
            QuestionSet dbentry = context.QuestionSets.Find(questionSetId);

            if (dbentry != null)
            {
                context.QuestionSets.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        } 
          #endregion 

        #region Questionnaire

        public IList<Questionnaire> Questionnaires
        {
            get { return context.Questionnaires.ToList<Questionnaire>(); }
        }

        public void SaveQuestionnaire(Questionnaire questionnaire)
        {
            if (questionnaire.QuestionnaireId == 0)
            {
                context.Questionnaires.Add(questionnaire);
            }
            else
            {
                Questionnaire dbentry = context.Questionnaires.Find(questionnaire.QuestionnaireId);
                if (dbentry != null)
                {
                    dbentry.QuestionSetId = questionnaire.QuestionSetId;
                    dbentry.Title = questionnaire.Title;
                }
            }

            context.SaveChanges();
        }

        public Questionnaire DeleteQuestionnaire(int questionnaireId)
        {
            Questionnaire dbentry = context.Questionnaires.Find(questionnaireId);

            if (dbentry != null)
            {
                context.Questionnaires.Remove(dbentry);
                context.SaveChanges();
            }

            return dbentry;
        }

        #endregion

    }
}