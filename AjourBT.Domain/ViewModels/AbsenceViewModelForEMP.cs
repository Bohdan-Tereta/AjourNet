using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourBT.Domain.ViewModels
{
    public class AbsenceViewModelForEMP
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ID { get; set; }
        public Dictionary<CalendarItemType, List<AbsenceFactorData>> FactorDetails { get; set; }
    }

    public class AbsenceFactorData
    {
        public int AbsenceFactorDataID { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime? ReclaimDate { get; set; }
    }
}