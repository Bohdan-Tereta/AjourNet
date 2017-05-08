using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Unit
    {
        [Key]
        public int UnitID { get; set; }

        [Required(ErrorMessage = "Please enter Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter ShortTitle")]
        public string ShortTitle { get; set; }

        public virtual List<BusinessTrip> BusinessTrips { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}