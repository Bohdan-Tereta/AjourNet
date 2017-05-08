using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class PrepBusinessTripViewModelForVU
    {
        //[Required]
        public string EID { get; set; }
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }

        public int EmployeeID { get; set; }

        public int BusinessTripID { get; set; }

        public bool IsManager { get; set; }

        public Dictionary<BTStatus, List<BusinessTrip>> BusinessTripsByEmployee { get; set; }
       
        [Display(Name = "Date Dismissed")]
        public string DateDismissed { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}