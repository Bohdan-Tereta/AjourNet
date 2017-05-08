using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class EmployeeViewModelForVU
    {
        //[Required]
        public string EID { get; set; }
        //[Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }

        public bool IsManager { get; set; }
        public string DateDismissed { get; set; }

        public int DaysUsedInBt { get; set; }

        public Dictionary<int, List<BusinessTrip>> BusinessTripsByMonth { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}