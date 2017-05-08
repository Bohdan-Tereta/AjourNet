using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.ViewModels
{
    public class QuestionnairePreviewModel
    {
        public int QuestionnaireId { get; set; }
        public string Title { get; set; }
        public string QuestionSetId { get; set; }
        public List<KeyValuePair<string,string>> GeneratedQuestionary { get; set; }
        public List<KeyValuePair<string, string>> GeneratedQuestionaryForPdf { get; set; }
        public List<KeyValuePair<string, string>> Questions { get; set; }

 
         
    }

}
