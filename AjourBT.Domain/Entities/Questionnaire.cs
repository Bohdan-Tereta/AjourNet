using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Questionnaire
    {
        [Key]
        public int QuestionnaireId { get; set; }
        public string Title { get; set; }
        public string QuestionSetId { get; set; }
    }
}
