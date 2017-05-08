using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Journey
    {
        [Key]
        public int JourneyID { get; set; }
        public int BusinessTripID { get; set; }
        public virtual BusinessTrip JourneyOf { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ReclaimDate { get; set; }
        public bool DayOff { get; set; }
    }
}