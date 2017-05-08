using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class OvertimeByEmployeeModel
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EID { get; set; }
        public string Department { get; set; }
        public List<Overtime> Overtimes { get; set; }

    }
}