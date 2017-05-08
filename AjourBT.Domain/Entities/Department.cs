using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        [Required(ErrorMessage = "Please enter department name ")]
        public string DepartmentName { get; set; }
        public virtual List<Employee> Employees { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
