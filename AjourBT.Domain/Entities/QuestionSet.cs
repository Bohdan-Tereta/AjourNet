using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Domain.Entities
{
    public class QuestionSet
    {
        public QuestionSet()
        {
            Questions = "[]";
        }

        [Key]
        public int QuestionSetId { get; set; }
        public string Title { get; set; }
      
        [AllowHtml]
        public string Questions { get; set; }
    }
}
