using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLSLoader
{
    class VisaForLoader
    {
        public string VisaType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public int Days { get; set; }
        public int Entries { get; set; }
        public string EID { get; set; }
    }
}
