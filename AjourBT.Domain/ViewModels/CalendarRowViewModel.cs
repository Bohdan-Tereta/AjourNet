using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class CalendarRowViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public List<CalendarItemViewModel> values { get; set; }
    }
}