﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Passport
    {
        [Key]
        [ForeignKey("PassportOf")]
        public int EmployeeID { get; set; }

        [Display(Name = "To")]
        public DateTime? EndDate { get; set; }
        public virtual Employee PassportOf { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}