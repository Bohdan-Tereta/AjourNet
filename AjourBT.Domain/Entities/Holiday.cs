using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
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
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HolidayDate { get; set; }
     
        public bool IsPostponed { get; set; }
        
        [Display(Name = "Comment")]
        public string HolidayComment { get; set; } 
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
