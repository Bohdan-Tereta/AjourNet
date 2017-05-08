using AjourAPIServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Employee
    {
            [Key]
            public int EmployeeID { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string EID { get; set; }

            public Nullable<int> DepartmentID { get; set; }
            public virtual Department Department { get; set; }

            public DateTime? DateEmployed { get; set; }

            public Nullable<int> PositionID { get; set; }
            public virtual Position Position { get; set; }

            public DateTime? BirthDay { get; set; }
            public string Comment { get; set; }
            public string FullNameUk { get; set; }

            public DateTime? DateDismissed { get; set; }
            public bool IsManager { get; set; }
            public virtual List<BusinessTrip> BusinessTrips { get; set; }
            public virtual Visa Visa { get; set; }
            public virtual VisaRegistrationDate VisaRegistrationDate { get; set; }
            public virtual Permit Permit { get; set; }
            public virtual Insurance Insurance { get; set; }
            public virtual Passport Passport { get; set; }
            public virtual List<CalendarItem> CalendarItems { get; set; }
            public virtual List<Overtime> Overtimes { get; set; }
            public virtual List<Vacation> Vacations { get; set; }
            public virtual List<Sickness> Sicknesses { get; set; }
            [ConcurrencyCheck]
            [Timestamp]
            public byte[] RowVersion { get; set; }

            public string BTRestrictions { get; set; }

            public bool IsUserOnly { get; set; }
            public bool IsGreetingMessageAllow { get; set; }
            public string EMail { get; set; }

            //private string StringifiedName 

            [Display(Name = "Education Acquired")]
            public EducationType EducationAcquiredType { get; set; }
            [Display(Name = "Education Acquired Date")]
            public DateTime? EducationAcquiredDate { get; set; }

            [Display(Name = "Education In Progress")]
            public EducationType EducationInProgressType { get; set; }
            [Display(Name = "Education In Progress Date")]
            public DateTime? EducationInProgressDate { get; set; }

            public string EducationComment { get; set; }
        }
    }