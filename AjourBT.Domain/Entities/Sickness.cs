using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Sickness
    {
        [Key]
        public int SickID { get; set; }

        public int EmployeeID { get; set; }
        public virtual Employee SickOf { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string SicknessType { get; set; }
        public int Workdays { get; set; }

    }
}
