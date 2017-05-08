using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    public class JourneyForLoader
    {

        public JourneyForLoader(Journey journey)
        {
            this.JourneyID = journey.JourneyID;
            this.BusinessTripID = journey.BusinessTripID;
            this.Date = journey.Date;
            this.ReclaimDate = journey.ReclaimDate;
            this.DayOff = journey.DayOff;
        }

        public int JourneyID { get; set; }
        public int BusinessTripID { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ReclaimDate { get; set; }
        public bool DayOff { get; set; }
    
    }
}
