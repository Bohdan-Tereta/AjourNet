using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Position
    {
        [Key]
        public int PositionID { get; set; }

        [Required(ErrorMessage = "Please enter TitleUk")]
        public string TitleUk { get; set; }

        [Required(ErrorMessage = "Please enter TitleEn")]
        public string TitleEn { get; set; }

        public virtual List<Employee> Employees { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}