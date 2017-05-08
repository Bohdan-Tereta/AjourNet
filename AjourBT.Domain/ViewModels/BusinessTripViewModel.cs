using AjourBT.Domain.CustomAnnotations;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class BusinessTripViewModel
    {
        public BusinessTripViewModel(BusinessTrip businessTrip)
        {
            BusinessTripID = businessTrip.BusinessTripID;
            BTof = businessTrip.BTof;
            CancelComment = businessTrip.CancelComment;
            Comment = businessTrip.Comment;
            EmployeeID = businessTrip.EmployeeID;
            EndDate = string.Format("{0:d}", businessTrip.EndDate);

            EndDateFormated = string.Format("{0:yyyy-MM-dd}", businessTrip.EndDate);
            Flights = businessTrip.Flights;
            FlightsConfirmed = businessTrip.FlightsConfirmed;
            Habitation = businessTrip.Habitation;
            HabitationConfirmed = businessTrip.HabitationConfirmed;
            Invitation = businessTrip.Invitation;
            LocationID = businessTrip.LocationID;
            UnitID = businessTrip.UnitID;
            Manager = businessTrip.Manager;
            OldEndDate = string.Format("{0:d}", businessTrip.OldEndDate);
            OldLocationID = businessTrip.OldLocationID;
            OldLocationTitle = businessTrip.OldLocationTitle;
            OldStartDate = string.Format("{0:d}", businessTrip.OldStartDate);
            Purpose = businessTrip.Purpose;
            RejectComment = businessTrip.RejectComment;
            AccComment = businessTrip.AccComment;
            BTMComment = businessTrip.BTMComment;
            Responsible = businessTrip.Responsible;
            StartDate = string.Format("{0:d}", businessTrip.StartDate);
            StartDateFormated = string.Format("{0:yyyy-MM-dd}", businessTrip.StartDate);
            Status = businessTrip.Status;
            LastCRUDedBy = businessTrip.LastCRUDedBy;
            LastCRUDTimestamp = businessTrip.LastCRUDTimestamp;
            OrderStartDate = (businessTrip.OrderStartDate == null) ? null : String.Format("{0:d}", businessTrip.OrderStartDate);
            OrderEndDate = (businessTrip.OrderEndDate == null) ? null : String.Format("{0:d}", businessTrip.OrderEndDate);
            DaysInBTForOrder = (businessTrip.DaysInBtForOrder == null) ? null : businessTrip.DaysInBtForOrder;
            Title = businessTrip.Location!=null?businessTrip.Location.Title:null;
            if (businessTrip.Location != null)
            {
                Title = businessTrip.Location.Title;
            }
            if (businessTrip.Unit != null)
            {
                Unit = businessTrip.Unit.ShortTitle;
            }
            RowVersion = businessTrip.RowVersion;
           // Unit = businessTrip.Unit.ShortTitle;
            
        }

        public BusinessTripViewModel(BusinessTrip businessTrip, int BusinessTripID)
            :this(businessTrip)
        {
            this.BusinessTripID = BusinessTripID;
        }

        public int BusinessTripID { get; set; }

        [Required]
        [Display(Name = "From")]
        public string StartDate { get; set; }

        [GreaterThan("StartDate", ErrorMessage = "From Date is greater than To Date")]
        [Required]
        [Display(Name = "To")]
        public string EndDateFormated { get; set; }


        [Display(Name = "From")]
        public string StartDateFormated { get; set; }

        [GreaterThan("StartDate", ErrorMessage = "From Date is greater than To Date")]
        [Required]
        [Display(Name = "To")]
        public string EndDate { get; set; }


        [Display(Name = "Old From")]
        public string OldStartDate { get; set; }

        [Display(Name = "Old To")]
        public string OldEndDate { get; set; }

        private BTStatus status;
        public BTStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        //public virtual Location Location { get; set; }

        [Display(Name = "Old Location")]
        public int OldLocationID { get; set; }

        [Display(Name = "Old Location")]
        public string OldLocationTitle { get; set; }


        public int EmployeeID { get; set; }

        public virtual Employee BTof { get; set; }

        public string Purpose { get; set; }
        public string Manager { get; set; }
        public string Responsible { get; set; }
        public string Comment { get; set; }
        [Required]
        [Display(Name = "Reject Comment")]
        public string RejectComment { get; set; }
        [Required]
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
        public string Title { get; set; }
        public string LastCRUDedBy { get; set; }
        public DateTime LastCRUDTimestamp { get; set; }

        [Display(Name = "Order From")]
        public string OrderStartDate { get; set; }
        [GreaterThan("OrderStartDate", ErrorMessage = "Order From is greater than Order To")]
        [Display(Name = "Order To")]
        public string OrderEndDate { get; set; }
        //[Display(Name = "Entries Used in BT")] 
        public int? DaysInBTForOrder { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name = "Unit")]
        public int UnitID { get; set; }

        public string Unit { get; set; }

        public string Name { get { return this.BTof.LastName + " " + this.BTof.FirstName; } }

            public List<String> PrepareToXLSExportVU()
        {
            List<string> stringifiedProperties = new List<string>();
            stringifiedProperties.Add(this.BusinessTripID.ToString());
            if (this.BTof == null)
            {
                stringifiedProperties.Add(String.Empty);
                stringifiedProperties.Add(String.Empty); 
            }
            else
            {
                stringifiedProperties.Add(this.BTof.EID ?? String.Empty);
                stringifiedProperties.Add(this.Name ?? String.Empty);
            }
            stringifiedProperties.Add(this.Title ?? String.Empty);
            if (this.Status == (BTStatus.Confirmed | BTStatus.Reported))
            {
                stringifiedProperties.Add(this.StartDateFormated ?? String.Empty);
            }
            else
            {
                stringifiedProperties.Add(this.StartDateFormated + " To be updated soon");
            }
            stringifiedProperties.Add(this.EndDateFormated ?? String.Empty);
            stringifiedProperties.Add(this.Unit ?? String.Empty);
            stringifiedProperties.Add(this.Purpose ?? String.Empty);
            stringifiedProperties.Add(this.Manager ?? String.Empty);
            stringifiedProperties.Add(this.Responsible ?? String.Empty); 

            return stringifiedProperties;
        }

    }
}