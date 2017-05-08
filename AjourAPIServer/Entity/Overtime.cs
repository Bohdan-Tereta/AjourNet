using AjourAPIServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Overtime
    {
        [Key]
        public int OvertimeID { get; set; }
        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime Date { get; set; }

        public DateTime? ReclaimDate { get; set; }
        public bool DayOff { get; set; }
        private OvertimeType type;
        public OvertimeType Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}