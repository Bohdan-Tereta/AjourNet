using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Journey
    {
        [Key]
        public int JourneyID { get; set; }

        public int BusinessTripID { get; set; }
        public virtual BusinessTrip JourneyOf { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Reclaim Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ReclaimDate { get; set; }
        public bool DayOff { get; set; }


    }
}
