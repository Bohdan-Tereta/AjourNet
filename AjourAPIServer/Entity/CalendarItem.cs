﻿using AjourAPIServer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class CalendarItem
    {
        [Key]
        public int CalendarItemID { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        public int EmployeeID { get; set; }
        public string Location { get; set; }
        public virtual Employee Employee { get; set; }
        private CalendarItemType type;
        public CalendarItemType Type
        {
            get { return type; }
            set { type = value; }
        }
    }
}