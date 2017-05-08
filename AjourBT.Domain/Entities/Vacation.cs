using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Vacation
    {
        [Key]
        public int VacationID { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        private VacationType type;
        public VacationType Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}
