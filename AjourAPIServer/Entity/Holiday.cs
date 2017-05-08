using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Holiday
    {
        public int HolidayID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }

        [Required]
        public DateTime HolidayDate { get; set; }

        public bool IsPostponed { get; set; }

        public string HolidayComment { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}