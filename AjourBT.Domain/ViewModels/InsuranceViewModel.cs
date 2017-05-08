using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class InsuranceViewModel
    {
        public int EmployeeID { get; set; }
        
        [Required]
        [Display(Name = "From")]
        public string StartDate { get; set; }
        
        [Required]
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Display(Name = "To")]
        public string EndDate { get; set; }
        public int Days { get; set; }

        public InsuranceViewModel(Insurance insurance)
        {
            EmployeeID = insurance.EmployeeID;
            StartDate = string.Format("{0:d}", insurance.StartDate);
            EndDate = string.Format("{0:d}", insurance.EndDate);
            Days = insurance.Days;
            RowVersion = insurance.RowVersion;
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}