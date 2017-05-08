using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class PrivateTripViewModel
    {
        public PrivateTripViewModel(PrivateTrip privateTrip)
        {
            PrivateTripID = privateTrip.PrivateTripID;
            StartDate = String.Format("{0:d}", privateTrip.StartDate);
            EndDate = String.Format("{0:d}", privateTrip.EndDate);
            EmployeeID = privateTrip.EmployeeID;
            VisaOf = privateTrip.PrivateTripOf;
            RowVersion = privateTrip.RowVersion;
        }

        public int PrivateTripID { get; set; }

        [Required]
        [Display(Name = "From")]
        public string StartDate { get; set; }
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Required]
        [Display(Name = "To")]
        public string EndDate { get; set; }

        public int EmployeeID { get; set; }
        public Visa VisaOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}