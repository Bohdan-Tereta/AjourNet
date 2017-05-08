using AjourAPIServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class BusinessTrip
    {
        [Key]
        public int BusinessTripID { get; set; }

        [Required]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Old From")]
        public DateTime? OldStartDate { get; set; }

        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Old To")]
        public DateTime? OldEndDate { get; set; }

        public DateTime? OrderStartDate { get; set; }
        public DateTime? OrderEndDate { get; set; }

        public int? DaysInBtForOrder { get; set; }

        private BTStatus status;
        public BTStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        public virtual Location Location { get; set; }

        [Display(Name = "Location")]
        public int OldLocationID { get; set; }

        [Display(Name = "Unit")]
        public int UnitID { get; set; }

        public virtual Unit Unit { get; set; }

        [Display(Name = "Old Location")]
        public string OldLocationTitle { get; set; }

        public int EmployeeID { get; set; }
        public virtual Employee BTof { get; set; }

        public virtual List<Journey> Journeys { get; set; }

        public string Purpose { get; set; }
        public string Manager { get; set; }
        public string Responsible { get; set; }
        public string Comment { get; set; }

        [Display(Name = "Reject Comment")]
        public string RejectComment { get; set; }
        [Display(Name = "Cancel Comment")]
        public string CancelComment { get; set; }
        [Display(Name = "ACC's Comment")]
        public string AccComment { get; set; }
        [Display(Name = "BTM's Comment")]
        public string BTMComment { get; set; }
        public string Habitation { get; set; }
        [Display(Name = "Habitation Confirmed")]
        public bool HabitationConfirmed { get; set; }
        public string Flights { get; set; }
        [Display(Name = "Flights Confirmed")]
        public bool FlightsConfirmed { get; set; }
        public bool Invitation { get; set; }
        public string LastCRUDedBy { get; set; }
        public DateTime LastCRUDTimestamp { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}