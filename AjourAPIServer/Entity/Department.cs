using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
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