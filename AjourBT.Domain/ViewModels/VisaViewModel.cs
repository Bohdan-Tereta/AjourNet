using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.ViewModels
{
    public class VisaViewModel
    {

        [HiddenInput(DisplayValue = false)]
        public int EmployeeID { get; set; }
        [Required]
        [Display(Name = "Visa Type")]
        public string VisaType { get; set; }
        [Required]
        [Display(Name = "From")]
        public string StartDate { get; set; }
        [GreaterThan("StartDate", ErrorMessage = " To Date should be greater than From Date")]
        [Required]
        [Display(Name = "To")]
        public string DueDate { get; set; }
        [Required]
        public int Entries { get; set; }
        [Display(Name = "Entries Used in BT")]
        public int? EntriesUsedInBT { get; set; }
        [Display(Name = "Private Used Entries")]
        public int? EntriesUsedInPrivateTrips { get; set; }
        [Required]
        public int Days { get; set; }
        [Display(Name = "Days Used in BT")]
        public int? DaysUsedInBT { get; set; }
        [Display(Name = "Private Used Days")]
        public int? DaysUsedInPrivateTrips { get; set; }
        public Employee VisaOf { get; set; }
        [Display(Name = "Correction for Visa's Days")]
        public int? CorrectionForVisaDays { get; set; }
        [Display(Name = "Correction for Visa's Entries")]
        public int? CorrectionForVisaEntries { get; set; }
        [Required]
        [Display(Name = "Registration")]
        public string VisaRegistrationDate { get; set; }

        public VisaViewModel(Visa visa)
        {
            EmployeeID = visa.EmployeeID;
            VisaType = visa.VisaType;
            StartDate = string.Format("{0:d}", visa.StartDate);
            DueDate = string.Format("{0:d}", visa.DueDate);
            Entries = visa.Entries;
            EntriesUsedInBT = visa.EntriesUsedInBT;
            EntriesUsedInPrivateTrips = visa.EntriesUsedInPrivateTrips;
            CorrectionForVisaEntries = visa.CorrectionForVisaEntries;
            Days = visa.Days;
            DaysUsedInBT = visa.DaysUsedInBT;
            DaysUsedInPrivateTrips = visa.DaysUsedInPrivateTrips;
            CorrectionForVisaDays = visa.CorrectionForVisaDays;
            VisaOf = visa.VisaOf;
            RowVersion = visa.RowVersion;
            //if (visa.VisaOf.VisaRegistrationDate != null)
            //    VisaRegistrationDate = string.Format("{0:d}", visa.VisaOf.VisaRegistrationDate.RegistrationDate);
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}