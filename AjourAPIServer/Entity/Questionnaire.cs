using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Entity
{
    public class Questionnaire
    {
        [Key]
        public int QuestionnaireId { get; set; }
        public string Title { get; set; }
        public string QuestionSetId { get; set; }
    }
}