using AjourBT.Domain.CustomAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Education
    {
        [Key]
        public int EducationId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public EducationType EducationType { get; set; }
        public DateTime? DateReceived { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
