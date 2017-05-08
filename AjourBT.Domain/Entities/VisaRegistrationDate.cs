using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class VisaRegistrationDate
    {
        
        [Required]
        [Display(Name = "Employee")]
        [Key]
        [ForeignKey("VisaRegistrationDateOf")]
        public int EmployeeID { get; set; }

        [Required]
        [Display(Name = "Visa Type")]
        public string VisaType { get; set; }

        [RequiredIf("PaymentDate", null, ErrorMessage = "Either Registration Date or Payment Date must be filled in")]
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Registration Time")]
        public string RegistrationTime { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }
     
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [RequiredIf("RegistrationDate", null, ErrorMessage = "Either Registration Date or Payment Date must be filled in")]
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Payment Time")]
        public string PaymentTime { get; set; }

        [Display(Name = "Payment PIN")]
        public string PaymentPIN { get; set; }

        public virtual Employee VisaRegistrationDateOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
