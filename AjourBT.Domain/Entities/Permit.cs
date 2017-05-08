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
        [RequiredIf("IsKartaPolaka",false,ErrorMessage="Required")]
        [Display(Name = "Num")]
        public string Number { get; set; }
        [RequiredIf("IsKartaPolaka", false, ErrorMessage = "Required")]
        [Display(Name = "From")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [RequiredIf("IsKartaPolaka", false, ErrorMessage = "Required")]
        [Display(Name = "To")]
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Cancel Request Date")]
        public DateTime? CancelRequestDate { get; set; }

        [Display(Name = "Prolong Request Date")]
        public DateTime? ProlongRequestDate { get; set; }       

        public virtual Employee PermitOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var results = new List<ValidationResult>();

        //    if (!IsKartaPolaka)
        //    {
        //        if (Number == null || StartDate == null || EndDate == null)

        //            results.Add(new ValidationResult("Please, specify values for all fields"));

        //        else
        //        {
        //            if (StartDate > EndDate)
        //                results.Add(new ValidationResult("'From' field must not be greater than 'To' field"));
        //        }
        //    }            
        //    else
        //    {
        //        if (!((Number == null && StartDate == null && EndDate == null) || (Number != null && StartDate != null && EndDate != null)))
        //            results.Add(new ValidationResult("Please, specify values for all fields, or leave them all empty"));

        //        if (StartDate > EndDate)
        //            results.Add(new ValidationResult("'From' field must not be greater than 'To' field"));
        //    }
              

        //    return results;
        //}
    }
}
