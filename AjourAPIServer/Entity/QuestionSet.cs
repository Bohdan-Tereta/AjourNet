using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourAPIServer.Entity
{
    public class QuestionSet
    {
        [Key]
        public int QuestionSetId { get; set; }
        public string Title { get; set; }

        [AllowHtml]
        public string Questions { get; set; }
    }
}