using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    public class CalendarItemsCreator
    {
        private IRepository repository;
        private List<CalendarItem> calendarItems;
        private List<Journey> journeysToAdd;
        private List<Journey> journeysToRemove;

        public CalendarItemsCreator(IRepository repository, List<CalendarItem> calendarItems, 
            List<Journey> journeysToAdd, List<Journey> journeysToRemove)
        {
            this.repository = repository;
            this.calendarItems = calendarItems;
            this.journeysToAdd = journeysToAdd;
            this.journeysToRemove = journeysToRemove;
        }

        public void GenerateItemsFromDataBase(BusinessTrip bt)
        {
            BusinessTrip dbEntry = (from b in repository.BusinessTrips where b.BusinessTripID == bt.BusinessTripID select b).FirstOrDefault();


            if (dbEntry != null)
            {
                //if (!dbEntry.RowVersion.SequenceEqual(bt.RowVersion))
                //{
                //    throw new DbUpdateConcurrencyException();
                //}

                //if ((dbEntry.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (dbEntry.Status != BTStatus.Cancelled) && (dbEntry.Status != bt.Status))
                //{
                //    DeleteBusinessTripCalendarItem(bt, dbEntry);

                //}
                //else 
                if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled))
                {

                    CreateBusinessTripCalendarItem(bt, dbEntry);
                }

                dbEntry.StartDate = (bt.StartDate == default(DateTime)) ? dbEntry.StartDate : bt.StartDate;
                dbEntry.EndDate = bt.EndDate == default(DateTime) ? dbEntry.EndDate : bt.EndDate;
                dbEntry.Status = bt.Status;
                dbEntry.LocationID = bt.LocationID == 0 ? dbEntry.LocationID : bt.LocationID;
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

                #region CreateJourney
                if (bt.OrderStartDate != null && bt.OrderEndDate != null)
                {
                    BusinessTrip existingBtsWithSameOrderDates;

                    existingBtsWithSameOrderDates = (from b in repository.BusinessTrips
                                                     where b.EmployeeID == bt.EmployeeID
                                                     && b.BusinessTripID != bt.BusinessTripID
                                                     && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                                     && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                                     select b).FirstOrDefault();

                    if (existingBtsWithSameOrderDates == null)
                    {
                        if (dbEntry.OrderStartDate != null && dbEntry.OrderEndDate != null)
                        {
                            foreach (Journey jour in dbEntry.Journeys)
                            {
                                journeysToRemove.Add(jour);
                            }
                        }
                        CreateJourney(bt);
                    }
                    else
                    {
                        var bts = (from b in repository.BusinessTrips
                                   where b.EmployeeID == bt.EmployeeID
                                         && b.OrderStartDate.Value == bt.OrderStartDate.Value
                                         && b.OrderEndDate.Value == bt.OrderEndDate.Value
                                   select b).ToList();

                        //List<int> journeysID = new List<int>();

                        //BusinessTrip bussinesTrip = (from b in repository.BusinessTrips
                        //                             where b.BusinessTripID == bt.BusinessTripID
                        //                             select b).FirstOrDefault();

                        //bts.Add(bussinesTrip);

                        foreach (BusinessTrip b in bts)
                        {
                            if (b.Journeys != null)
                            {
                                foreach (Journey jour in b.Journeys)
                                {
                                    journeysToRemove.Add(jour);
                                }
                            }
                        }
                        CreateJourneyFromPairedBts(bts);
                    }
                }

                #endregion

                dbEntry.OrderStartDate = (bt.OrderStartDate == null) ? dbEntry.OrderStartDate : bt.OrderStartDate;
                dbEntry.OrderEndDate = (bt.OrderEndDate == null) ? dbEntry.OrderEndDate : bt.OrderEndDate;

                dbEntry.OrderStartDate = bt.OrderStartDate;
                dbEntry.OrderEndDate = bt.OrderEndDate;
                dbEntry.DaysInBtForOrder = bt.DaysInBtForOrder;
                dbEntry.RowVersion = bt.RowVersion;
            }
            //context.SaveChanges();

        }

        private void DeleteBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            //delete CalendarItem
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = GetBTCalendarItem(dbEntry, employee);

                List<Journey> journey = GetJourneysForBusinessTrip(bt);

                List<CalendarItem> items = GetJourneyCalendarItem(employee);

                if (item != null)
                {
                    employee.CalendarItems.Remove(item);
                    repository.DeleteCalendarItem(item.CalendarItemID);
                }

                DeleteJourneyCalendarItems(employee, journey, items);

            }
        }

        private void CreateBusinessTripCalendarItem(BusinessTrip bt, BusinessTrip dbEntry)
        {
            Employee employee = (from emp in repository.Employees where emp.EmployeeID == dbEntry.EmployeeID select emp).FirstOrDefault();
            if (employee != null)
            {
                CalendarItem item = new CalendarItem();
                item.Employee = employee;
                item.EmployeeID = employee.EmployeeID;
                item.From = bt.StartDate;
                item.To = bt.EndDate;
                item.Location = bt.Location.Title;
                item.Type = CalendarItemType.BT;
                calendarItems.Add(item);
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
                        calendarItems.Remove(it);
                    }
                }
            }
        }

        private List<CalendarItem> GetJourneyCalendarItem(Employee employee)
        {
            List<CalendarItem> items = (from it in repository.CalendarItems
                                        where
                                            it.EmployeeID == employee.EmployeeID &&
                                            it.Type == CalendarItemType.Journey
                                        select it).ToList();
            return items;
        }

        private List<Journey> GetJourneysForBusinessTrip(BusinessTrip bt)
        {
            List<Journey> journey = (from journ in repository.Journeys
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
            int DaysBetweenStartDateAndOrderStartDate = (bt.StartDate - bt.OrderStartDate.Value).Days;
            int DaysBetweenOrderEndDateAndEndDate = (bt.OrderEndDate.Value - bt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = bt.OrderStartDate.Value;
            DateTime? StartDateForJourneyForEndDifference = bt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            Employee emp = (from e in repository.Employees where e.EmployeeID == bt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = bt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in repository.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                journeysToAdd.Add(journey);

                if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled))
                {
                    item.EmployeeID = bt.EmployeeID;
                    item.From = journey.Date;
                    item.To = journey.Date;
                    item.Type = CalendarItemType.Journey;

                    calendarItems.Add(item);

                    item = new CalendarItem();
                }

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

                foreach (Holiday hol in repository.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                journeysToAdd.Add(journey);

                if ((bt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (bt.Status != BTStatus.Cancelled))
                {
                    item.EmployeeID = bt.EmployeeID;
                    item.From = journey.Date;
                    item.To = journey.Date;
                    item.Type = CalendarItemType.Journey;

                    calendarItems.Add(item);

                    item = new CalendarItem();
                }
                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.Date.AddDays(1));
            }
        }

        private void CreateJourneyFromPairedBts(List<BusinessTrip> bts)
        {
            BusinessTrip firstBt = bts.OrderBy(bt => bt.StartDate).FirstOrDefault();
            BusinessTrip lastBt = bts.OrderByDescending(bt => bt.EndDate).FirstOrDefault();

            int DaysBetweenStartDateAndOrderStartDate = (firstBt.StartDate - firstBt.OrderStartDate.Value).Days;
            int DaysBetweenOrderEndDateAndEndDate = (lastBt.OrderEndDate.Value - lastBt.EndDate.Date).Days;

            DateTime? StartDateForJourneyForStartDifference = firstBt.OrderStartDate;
            DateTime? StartDateForJourneyForEndDifference = lastBt.EndDate.AddDays(1);

            CalendarItem item = new CalendarItem();
            //Employee empFirstBt = (from e in repository.Employees where e.EmployeeID == firstBt.EmployeeID select e).FirstOrDefault();
            //Employee empLastBt = (from e in repository.Employees where e.EmployeeID == lastBt.EmployeeID select e).FirstOrDefault();

            for (int i = 0; i < DaysBetweenStartDateAndOrderStartDate; i++)
            {
                Journey journey = new Journey();
                journey.BusinessTripID = firstBt.BusinessTripID;
                journey.Date = StartDateForJourneyForStartDifference.Value;
                if (journey.Date.DayOfWeek == DayOfWeek.Saturday || journey.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    journey.DayOff = true;
                }


                foreach (Holiday hol in repository.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                //if(!journeysToAdd.Contains(journey, new JourneyComparer()))
                if (journeysToAdd.Where(j => ((j.BusinessTripID == journey.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    journeysToAdd.Add(journey);

                    if ((firstBt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (firstBt.Status != BTStatus.Cancelled))
                    {
                        item.EmployeeID = firstBt.EmployeeID;
                        item.From = journey.Date;
                        item.To = journey.Date;
                        item.Type = CalendarItemType.Journey;

                        //if(!calendarItems.Contains(item, new CalendarItemComparer()))
                        calendarItems.Add(item);
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

                foreach (Holiday hol in repository.Holidays)
                {
                    if (journey.Date == hol.HolidayDate.Date)
                    {
                        journey.DayOff = true;
                    }
                }

                //if (!journeysToAdd.Contains(journey, new JourneyComparer()))
                if (journeysToAdd.Where(j => ((j.BusinessTripID == journey.BusinessTripID) && (j.Date == journey.Date))).Count() == 0)
                {
                    journeysToAdd.Add(journey);

                    if ((lastBt.Status == (BTStatus.Confirmed | BTStatus.Reported)) && (lastBt.Status != BTStatus.Cancelled))
                    {
                        item.EmployeeID = lastBt.EmployeeID;
                        item.From = journey.Date;
                        item.To = journey.Date;
                        item.Type = CalendarItemType.Journey;

                        //if (!calendarItems.Contains(item, new CalendarItemComparer()))
                        calendarItems.Add(item);
                        item = new CalendarItem();
                    }
                }
                StartDateForJourneyForEndDifference = new DateTime?(StartDateForJourneyForEndDifference.Value.AddDays(1));
            }
        }
    }
}
