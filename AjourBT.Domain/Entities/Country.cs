using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Country
    {
        [Key]
        public int CountryID { get; set; }
        [Display(Name = "Name")]
        [Required]
        public string CountryName { get; set; }
        public virtual List<Holiday> Holidays { get; set; }
        public virtual List<Location> Locations { get; set; }
        public string Comment { get; set; }
    
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
