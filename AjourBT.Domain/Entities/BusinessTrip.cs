using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
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

        [GreaterThan("StartDate", ErrorMessage = "From Date should be lesser than To Date")]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Old To")]
        public DateTime? OldEndDate { get; set; }
       
        public DateTime? OrderStartDate { get; set; }
        [GreaterThan("OrderStartDate", ErrorMessage = "Order From Date should be lesser than Order To Date")]
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

        public BusinessTrip()
        {

        }

        public BusinessTrip(BusinessTrip businessTrip)
        {
            BusinessTripID = businessTrip.BusinessTripID;
            BTof = businessTrip.BTof;
            CancelComment = businessTrip.CancelComment;
            Comment = businessTrip.Comment;
            EmployeeID = businessTrip.EmployeeID;
            EndDate = businessTrip.EndDate;
            Flights = businessTrip.Flights;
            FlightsConfirmed = businessTrip.FlightsConfirmed;
            Habitation = businessTrip.Habitation;
            HabitationConfirmed = businessTrip.HabitationConfirmed;
            Invitation = businessTrip.Invitation;
            LocationID = businessTrip.LocationID;
            UnitID = businessTrip.UnitID;
            Unit = businessTrip.Unit;
            Manager = businessTrip.Manager;
            OldEndDate = businessTrip.OldEndDate;
            OldLocationID = businessTrip.OldLocationID;
            OldLocationTitle = businessTrip.OldLocationTitle;
            OldStartDate = businessTrip.OldStartDate;
            Purpose = businessTrip.Purpose;
            RejectComment = businessTrip.RejectComment;
            AccComment = businessTrip.AccComment;
            BTMComment = businessTrip.BTMComment;
            Responsible = businessTrip.Responsible;
            StartDate = businessTrip.StartDate;
            Status = businessTrip.Status;
            LastCRUDedBy = businessTrip.LastCRUDedBy;
            LastCRUDTimestamp = businessTrip.LastCRUDTimestamp;
            OrderStartDate = businessTrip.OrderStartDate;
            OrderEndDate = businessTrip.OrderEndDate;
            RowVersion = businessTrip.RowVersion;
            Location = businessTrip.Location;
            DaysInBtForOrder = businessTrip.DaysInBtForOrder;
        }
    }
}
