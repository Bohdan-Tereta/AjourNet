using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class PermitViewModel
    {
        //---------For Permits------------

        public int EmployeeID { get; set; }
        [Display(Name = "Karta Polaka")]
        public bool IsKartaPolaka { get; set; }
        [RequiredIf("IsKartaPolaka",false,ErrorMessage="Required")]
        [Display(Name = "Num")]
        public string Number { get; set; }
        [RequiredIf("IsKartaPolaka", false, ErrorMessage = "Required")]
        [Display(Name = "From")]
        public string StartDate { get; set; }
        [RequiredIf("IsKartaPolaka", false, ErrorMessage = "Required")]
        [GreaterThan("StartDate", ErrorMessage = "To Date should be greater than From Date")]
        [Display(Name = "To")]
        public string EndDate { get; set; }

        [Display(Name = "Cancel Request Date")]
        public string CancelRequestDate { get; set; }

        [Display(Name = "Prolong Request Date")]
        public string ProlongRequestDate { get; set; }  

        public PermitViewModel(Permit permit)
        {
            EmployeeID = permit.EmployeeID;
            IsKartaPolaka = permit.IsKartaPolaka;
            Number = permit.Number;
            StartDate = string.Format("{0:d}", permit.StartDate);
            EndDate = string.Format("{0:d}", permit.EndDate);
            CancelRequestDate = string.Format("{0:d}", permit.CancelRequestDate);
            ProlongRequestDate = string.Format("{0:d}", permit.ProlongRequestDate);
            RowVersion = permit.RowVersion;
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}