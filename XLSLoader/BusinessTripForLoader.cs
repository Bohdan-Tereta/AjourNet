using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjourBT.Domain.Entities;

namespace XLSLoader
{
    class BusinessTripForLoader
    {
        public string EID { get; set; }
        public String LocationTitle { get; set; }
        public string Manager { get; set; }
        public string Responsible { get; set; }
        public string LastCRUDedBy { get; set; }
        public string Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BTStatus status { get; set; }
    }
}
