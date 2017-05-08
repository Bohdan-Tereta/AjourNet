using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Abstract
{
    public interface IRepository
    {
        IList<Employee> Employees { get; }
        IList<Department> Departments { get; }
        IList<Visa> Visas { get; }
        IList<VisaRegistrationDate> VisaRegistrationDates { get; }
        IList<Permit> Permits { get; }
        IList<BusinessTrip> BusinessTrips { get; }
        IList<Location> Locations { get; }
        IList<IMessage> Messages { get; }
        IList<Passport> Passports { get; }
        IList<PrivateTrip> PrivateTrips { get; }
        IList<Position> Positions { get; }
        IList<Country> Countries { get; }
        IList<Holiday> Holidays { get; }
        IList<Journey> Journeys { get; }
        IList<CalendarItem> CalendarItems { get; }
        IList<Unit> Units { get; }
        IList<Overtime> Overtimes { get; }
        IList<Vacation> Vacations { get; }
        IList<Sickness> Sicknesses { get; }
        IList<Employee> Users { get; }
        IList<Greeting> Greetings { get; }
        IList<Insurance> Insurances { get; }
        IList<QuestionSet> QuestionSets { get; }
        IList<Questionnaire> Questionnaires { get; }

        void SaveEmployee(Employee employee);
        void SaveDepartment(Department department);
        void SaveVisa(Visa visa, int id);
        void SaveVisaRegistrationDate(VisaRegistrationDate visaRegistDate, int id);
        void SavePermit(Permit permit, int id);
        void SaveBusinessTrip(BusinessTrip bt);
        void SaveLocation(Location location);
        void SaveRolesForEmployee(string username, string[] roles);
        void SaveMessage(IMessage message);
        void SavePassport(Passport passport);
        void SavePrivateTrip(PrivateTrip pt);
        void SavePosition(Position position);
        void SaveCountry(Country Country);
        void SaveHoliday(Holiday Holiday);
        void SaveJourney(Journey Journey);
        void SaveCalendarItem(CalendarItem CalendarItem);
        void SaveUnit(Unit unit);
        void SaveOvertime(Overtime overtime);
        void SaveVacation(Vacation vacation);
        void SaveSick(Sickness sick);
        void SaveGreeting(Greeting greeting); 
        void SaveInsurance(Insurance insurance, int id); 
        void SaveQuestionSet(QuestionSet questionSet);
        void SaveQuestionnaire(Questionnaire questionnaire);

        Employee DeleteEmployee(int employeeID);
        Department DeleteDepartment(int departmentID);
        Visa DeleteVisa(int employeeID);
        VisaRegistrationDate DeleteVisaRegistrationDate(int employeeID);
        Permit DeletePermit(int employeeID);
        BusinessTrip DeleteBusinessTrip(int employeeID);
        Location DeleteLocation(int employeeID);
        void DeleteUser(string username);
        IMessage DeleteMessage(int messageID);
        Passport DeletePassport(int employeeID);
        PrivateTrip DeletePrivateTrip(int employeeID);
        Position DeletePosition(int positionID);
        Country DeleteCountry(int countryID);
        Holiday DeleteHoliday(int holidayID);
     //  Journey DeleteJourney(int journeyID);
        CalendarItem DeleteCalendarItem(int calendarID);
        Unit DeleteUnit(int unitID);
        Overtime DeleteOvertime(int OvertimeID);
        Vacation DeleteVacation(int VacationID);
        Sickness DeleteSick(int SickID);
        Greeting DeleteGreeting(int Greeting);
        Insurance DeleteInsurance(int InsuranceID);
        QuestionSet DeleteQuestionSet(int questionSet);
        Questionnaire DeleteQuestionnaire(int questionnaire);

        IList<AbsenceViewModel> SearchAbsenceData(DateTime fromDate, DateTime toDate, string searchString = "");
        IList<EmployeeViewModel> SearchUsersData(string selectedDepartment, string searchString = ""); 
        IList<BusinessTripViewModel> GetBusinessTripDataByUnits(int selectedYear);
        IList<BusinessTripViewModel> GetBusinessTripDataByUnitsWithoutCancelledAndDismissed(int selectedYear);
        IList<Employee> SearchVisaData(string searchString);
        IList<Employee> SearchVisaDataExcludingDismissed(string searchString);
        IList<WTRViewModel> SearchWTRData(DateTime fromDate, DateTime toDate, string searchString = "");
        IList<WTRViewModel> SearchWTRDataPerEMP(DateTime fromDate, DateTime toDate, Employee employee);
        IList<Employee> GetCurrentlyEmployedEmployees(string selectedDepartment);
    }
}