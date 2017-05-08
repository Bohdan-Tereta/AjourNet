using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class PrivateTrip
    {
        [Key]
        public int PrivateTripID { get; set; }

        [Required]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        public int EmployeeID { get; set; }
        public virtual Visa PrivateTripOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}