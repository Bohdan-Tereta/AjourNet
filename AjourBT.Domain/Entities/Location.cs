using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Address { get; set; }
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }
        public virtual List<BusinessTrip> BusinessTrips { get; set; }
        public string ResponsibleForLoc { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
