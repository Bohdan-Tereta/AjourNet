using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourAPIServer.Entity
{
    public class Permit
    {
        [Display(Name = "Karta Polaka")]
        public bool IsKartaPolaka { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Employee")]
        [Key]
        [ForeignKey("PermitOf")]
        public int EmployeeID { get; set; }

        [Display(Name = "Num")]
        public string Number { get; set; }

        [Display(Name = "From")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Cancel Request Date")]
        public DateTime? CancelRequestDate { get; set; }

        [Display(Name = "Prolong Request Date")]
        public DateTime? ProlongRequestDate { get; set; }

        public virtual Employee PermitOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}