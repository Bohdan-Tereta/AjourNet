using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    class EmployeeForLoader
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EID { get; set; }
        public string FullNameUk { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DateEmployed { get; set; }
        public DateTime? DateDismissed { get; set; }
        public string DepartmentName { get; set; }
        public bool IsManager { get; set; }
        public string roles { get; set; }
    }
}
