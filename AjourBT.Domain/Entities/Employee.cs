using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Domain.Entities
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

        [RequiredIf("IsUserOnly", false, ErrorMessage = "Required")]
        public Nullable<int> DepartmentID { get; set; }
        public virtual Department Department { get; set; }

        [RequiredIf("IsUserOnly", false, ErrorMessage = "Required")]
        public DateTime? DateEmployed { get; set; }

        [RequiredIf("IsUserOnly", false, ErrorMessage = "Required")]
        public Nullable<int> PositionID { get; set; }
        public virtual Position Position { get; set; }

        public DateTime? BirthDay { get; set; }
        public string Comment { get; set; }
        public string FullNameUk { get; set; }

        [GreaterThan("DateEmployed", ErrorMessage = "Date Dismissed should be greater than Date Employed")]
        public DateTime? DateDismissed { get; set; }
        public bool IsManager { get; set; }
        public virtual List<BusinessTrip> BusinessTrips { get; set; }
        public virtual Visa Visa { get; set; }
        public virtual VisaRegistrationDate VisaRegistrationDate { get; set; }
        public virtual Permit Permit { get; set; }
        public virtual Insurance Insurance { get; set; }
        public virtual Passport Passport { get; set; }
        public virtual List<CalendarItem> CalendarItems { get; set;}
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

        public List<string> PrepareToXLSExportVisasVU()
        {
            List<string> stringifiedProperties = new List<string>();

            stringifiedProperties.Add(this.EID);
            if (this.DateDismissed != null)
            {
                stringifiedProperties.Add(this.LastName + " " + this.FirstName + "\r\n" + this.DateDismissed.Value.ToShortDateString());
            }
            else
            {
                stringifiedProperties.Add(this.LastName + " " + this.FirstName);
            }

            if (this.Passport != null)
            {
                if (this.Passport.EndDate.HasValue)
                {
                    stringifiedProperties.Add("till\r\n" + this.Passport.EndDate.Value.ToString(String.Format("dd.MM.yyyy")));
                }
                else
                {
                    stringifiedProperties.Add("yes");
                }
            }
            else
            {
                stringifiedProperties.Add("no");
            }

            if (this.Visa != null)
            {
                stringifiedProperties.Add(this.Visa.VisaType);
                stringifiedProperties.Add(this.Visa.StartDate.ToString(String.Format("yyyy-MM-dd")));
                stringifiedProperties.Add(this.Visa.DueDate.ToString(String.Format("yyyy-MM-dd")));

                if (this.Visa.Entries == 0)
                {
                    stringifiedProperties.Add("MULT");
                }
                else
                {
                    stringifiedProperties.Add(this.Visa.Entries.ToString() + "(" + (this.Visa.EntriesUsedInPrivateTrips.GetValueOrDefault() + this.Visa.EntriesUsedInBT.GetValueOrDefault() + this.Visa.CorrectionForVisaEntries.GetValueOrDefault()).ToString() + ")");
                }

                stringifiedProperties.Add(this.Visa.Days.ToString() + "(" + (this.Visa.DaysUsedInPrivateTrips.GetValueOrDefault() + this.Visa.DaysUsedInBT.GetValueOrDefault() + this.Visa.CorrectionForVisaDays.GetValueOrDefault()).ToString() + ")");
            }
            else
            {
                stringifiedProperties.Add("");
                stringifiedProperties.Add("No Visa");
                stringifiedProperties.Add("No Visa");
                stringifiedProperties.Add("");
                stringifiedProperties.Add("");
            }

            if (this.VisaRegistrationDate != null && this.VisaRegistrationDate.RegistrationDate != null && this.VisaRegistrationDate.RegistrationDate.HasValue)
            {
                stringifiedProperties.Add(this.VisaRegistrationDate.RegistrationDate.Value.ToString(String.Format("dd.MM.yyyy")));

            }
            else
            {
                stringifiedProperties.Add("");
            }

            if (this.Permit != null)
            {
                if (this.Permit.Number != null || this.Permit.StartDate != null || this.Permit.EndDate != null)
                {
                    stringifiedProperties.Add(this.Permit.Number??"");

                    if (this.Permit.IsKartaPolaka)
                    {
                        if (this.Permit.StartDate != null && this.Permit.EndDate != null)
                        {
                            stringifiedProperties.Add("Karta Polaka" + "\r\n" + this.Permit.StartDate.Value.ToString(String.Format("dd.MM.yyyy")) + " - " + this.Permit.EndDate.Value.ToString(String.Format("dd.MM.yyyy")));
                        } 
                        else
                        {
                            stringifiedProperties.Add("Karta Polaka");
                        }
                    }
                    else
                    {
                        stringifiedProperties.Add(this.Permit.StartDate.Value.ToShortDateString() + " - " + this.Permit.EndDate.Value.ToShortDateString());
                    }
                }
                else
                {
                    stringifiedProperties.Add("");
                    if (this.Permit.IsKartaPolaka == true && this.Permit.Number == null && this.Permit.StartDate == null && this.Permit.EndDate == null)
                    {
                        stringifiedProperties.Add("Karta Polaka");
                    }
                    else
                    {
                        stringifiedProperties.Add("");
                    }
                }
            }
            else
            {
                stringifiedProperties.Add("");
                stringifiedProperties.Add("No Permit");
            }

            if (this.BusinessTrips != null)
            {
                BusinessTrip lastBT = this.BusinessTrips
                                              .Where(b => (b.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                                                           && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                                                           .OrderBy(b => b.EndDate)
                                              .LastOrDefault();
                if (lastBT != null)
                {
                    stringifiedProperties.Add(lastBT.Location.Title + ":" + lastBT.StartDate.ToString("dd.MM.yy") + " - " + lastBT.EndDate.ToString("dd.MM.yy"));
                }
                else
                {
                    stringifiedProperties.Add("");
                }
            }
            else
            {
                stringifiedProperties.Add("");
            }

            if (this.Permit != null && this.Permit.EndDate != null)
            {

                DateTime StartingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(this);
                if (DisplayPermitStatusHelper.Is90DaysAndAbove(StartingPoint))
                {
                    stringifiedProperties.Add("Contact Gov");
                } 
                
                else if (DisplayPermitStatusHelper.Is60ToLessThan90Days(StartingPoint))
                {
                    stringifiedProperties.Add("Contact Gov");
                }

                else if (this.Permit.ProlongRequestDate != null)
                {
                    stringifiedProperties.Add(this.Permit.ProlongRequestDate.Value.ToString(String.Format("dd.MM.yyyy")));
                }

                else if (this.Permit.CancelRequestDate != null)
                {
                    stringifiedProperties.Add(this.Permit.CancelRequestDate.Value.ToString(String.Format("dd.MM.yyyy")));
                } 
            }
            else
            {
                stringifiedProperties.Add("");
            }
            return stringifiedProperties; 
        }
    }
}

