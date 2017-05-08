using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Domain.Entities
{
    public class Insurance
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Employee")]
        [Key]
        [ForeignKey("InsuranceOf")]
        public int EmployeeID { get; set; }

        [Required]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Required]
        [Display(Name = "Ins")]
        public DateTime EndDate { get; set; }

        public int Days { get; set; }
        public virtual Employee InsuranceOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
