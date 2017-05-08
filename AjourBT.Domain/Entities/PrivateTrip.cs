using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace AjourBT.Domain.Entities
{
    public class PrivateTrip
    {
        [Key]
        public int PrivateTripID { get; set; }

        [Required]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Required]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        [ForeignKey("PrivateTripOf")]
        public int EmployeeID { get; set; }
        public virtual Visa PrivateTripOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
