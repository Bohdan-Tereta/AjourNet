using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class PassportViewModel
    {

        public int EmployeeID { get; set; }
       
        [Required]
        [Display(Name = "To")]
        public string EndDate { get; set; }
        public virtual Employee PassportOf { get; set; }

        public PassportViewModel(Passport passport)
        {
            EmployeeID = passport.EmployeeID;
            EndDate = string.Format("{0:d}", passport.EndDate);
            PassportOf = passport.PassportOf;
            RowVersion = passport.RowVersion;
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}