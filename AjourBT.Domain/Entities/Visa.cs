using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Visa
    {
        [Required]
        [Display(Name = "Employee")]
        [Key]
        [ForeignKey("VisaOf")]
        public int EmployeeID { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string VisaType { get; set; }
        [Required]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Required]
        [Display(Name = "To")]
        public DateTime DueDate { get; set; }
        public int Days { get; set; }
        [Display(Name = "Days Used in BT")]
        public int? DaysUsedInBT { get; set; }
        [Display(Name = "Days Used in Private Trips")]
        public int? DaysUsedInPrivateTrips { get; set; }
        public int Entries { get; set; }
        [Display(Name = "Entries Used in BT")]
        public int? EntriesUsedInBT { get; set; }
        [Display(Name = "Entries Used in Private Trips")]
        public int? EntriesUsedInPrivateTrips { get; set; }
        [Display(Name = "Correction for VisaDays")]
        public int? CorrectionForVisaDays { get; set; }
        [Display(Name = "Correction for VisaEntries")]
        public int? CorrectionForVisaEntries { get; set; }


        public virtual Employee VisaOf { get; set; }

        public virtual List<PrivateTrip> PrivateTrips { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

}