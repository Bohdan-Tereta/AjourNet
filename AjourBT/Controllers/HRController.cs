using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AjourBT.Infrastructure;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;

namespace AjourBT.Controllers
{
    public class HRController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;


        public HRController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;
        }

        [Authorize(Roles = ("HR"))]
        public List<KeyValuePair<string, List<SelectListItem>>> SetDropDownList(Questionnaire quest)
        {
            List<KeyValuePair<string, List<SelectListItem>>> localListItems = new List<KeyValuePair<string, List<SelectListItem>>>();

            var setList = from set in repository.QuestionSets
                          orderby set.Title
                          select set;

            if (!String.IsNullOrEmpty(quest.QuestionSetId))
            {

                string[] OrderQusetionSetPair = quest.QuestionSetId.Split(',');
                foreach (string item in OrderQusetionSetPair)
                {
                    string[] dotDelimiter = item.Split(':');

                    List<SelectListItem> tempList = new List<SelectListItem>();

                    foreach (var list in setList)
                    {
                        if (list.QuestionSetId == Convert.ToInt32(dotDelimiter[1]))
                        {
                            tempList.Add(new SelectListItem { Value = list.QuestionSetId.ToString(), Text = list.Title, Selected = true });
                        }
                        else
                        {
                            tempList.Add(new SelectListItem { Value = list.QuestionSetId.ToString(), Text = list.Title });
                        }
                    }
                    localListItems.Add(new KeyValuePair<string, List<SelectListItem>>(dotDelimiter[0], tempList));
                }
            }
            else
            {
                List<SelectListItem> emptyList = new List<SelectListItem>();

                foreach (var list in setList)
                {
                    emptyList.Add(new SelectListItem { Value = list.QuestionSetId.ToString(), Text = list.Title });
                }

                localListItems.Add(new KeyValuePair<string, List<SelectListItem>>("", emptyList));
            }

            return localListItems;
        }

        #region QuestionSets Tab

        [Authorize(Roles = ("HR"))]
        public ViewResult GetQuestionSets()
        {
            return View();
        }

        public ActionResult GetListOfQuestionSets()
        {
            List<QuestionSet> model = repository.QuestionSets.ToList();
            //string json = JsonConvert.SerializeObject(model);
            //return this.Content(json, "application/json"); 
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ViewResult CreateQuestionSet()
        {
            return View();
        }

        [HttpPost]
        public int CreateQuestionSet(string questionSet)
        {
            try
            {
                JObject jObject = JObject.Parse(questionSet);
                QuestionSet deserializedQuestionSet = new QuestionSet();
                deserializedQuestionSet.Title = jObject.Value<string>("Title");
                deserializedQuestionSet.QuestionSetId = jObject.Value<int>("QuestionSetId");
                deserializedQuestionSet.Questions = jObject.Value<JArray>("Questions").ToString(Formatting.None);
                repository.SaveQuestionSet(deserializedQuestionSet);
                return deserializedQuestionSet.QuestionSetId;
            }
            catch (JsonReaderException)
            {
                return 0;
            }
        }

        public bool DeleteQuestionSet(int questionSetId)
        {
            QuestionSet questionSetToDelete = repository.QuestionSets.Where(q => q.QuestionSetId == questionSetId).FirstOrDefault();
            if (questionSetToDelete != null)
            {
                if (repository.DeleteQuestionSet(questionSetId) != null)
                {
                    return true;
                };
            }
            return false;
        }

        #endregion

        #region GetQuestionnaire Tab

        [Authorize(Roles = ("HR"))]
        public ActionResult GetQuestionnaire(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        [Authorize(Roles = ("HR"))]
        public ActionResult GetQuestionnaireList()
        {
            List<Questionnaire> model = repository.Questionnaires.ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = ("HR"))]
        public ActionResult GetQuestWindow(int questionnaireId)
        {
            Questionnaire questionnaire = repository.Questionnaires.Where(id => id.QuestionnaireId == questionnaireId).FirstOrDefault();
            //ViewBag.Id = questTitle;
            ViewBag.questId = questionnaireId;
            ViewBag.QuestSetList = SetDropDownList(questionnaire);
            return View(questionnaire);
        }

        [Authorize(Roles = ("HR"))]
        public bool SaveQuestionnaire(Questionnaire quest)
        {
            try
            {
                repository.SaveQuestionnaire(quest);
                return true;
            } 
            catch(DbUpdateException)
            {
                return false;
            }
        }

        [Authorize(Roles = ("HR"))]
        public bool DeleteQuestionnaire(int questId)
        {
            try
            {
                repository.DeleteQuestionnaire(questId);
                return true;
            }
            catch(DbUpdateException)
            {
                return false;
            }
        }

        #endregion


        #region AddQuestionnaire

        public ActionResult CreateQuestionnaire()
        {
            return View();
        }

        [HttpPost]
        public int CreateQuestionnaire(string questionnaire)
        {
            if (ModelState.IsValid)
            {
                JObject jObject = JObject.Parse(questionnaire);
                Questionnaire deserializedQuestionnaire = new Questionnaire();
                deserializedQuestionnaire.Title = jObject.Value<string>("title");
                //deserializedQuestionnaire.QuestionnaireId = jObject.Value<int>("questionnaireId");
                //deserializedQuestionary.QuestionSetId = questionary.Substring(questionary.LastIndexOf("["), questionary.Length - questionary.LastIndexOf("[") - 1);
                if (!repository.Questionnaires.Contains(deserializedQuestionnaire))
                    repository.SaveQuestionnaire(deserializedQuestionnaire);
                else
                    return 0;

                return deserializedQuestionnaire.QuestionnaireId;

            }

            return 0;

        }

        #endregion

        #region Generate Questionnaire

        public ViewResult GenerateQuestionnaire(int questionaireID)
        {
            Questionnaire questionaire = repository.Questionnaires.Where(q => q.QuestionnaireId == questionaireID).FirstOrDefault();
            if (questionaire == null)
            {
                return View("~/Views/Error/ShowErrorPage.cshtml");
            }
            else
            {
                QuestionnairePreviewModel model = GenerateQuestionnaireModel(questionaire);
                if (model.GeneratedQuestionary.Count == 0)
                    return View(model);
                return View("~/Views/HR/GenerateQuestionnaire.cshtml", model);
            }
        }

        public QuestionnairePreviewModel GenerateQuestionnaireModel(Questionnaire questionaire)
        {
            Random random = new Random();

            QuestionnairePreviewModel model = new QuestionnairePreviewModel();
            model.QuestionnaireId = questionaire.QuestionnaireId;
            model.Title = questionaire.Title;
            model.QuestionSetId = questionaire.QuestionSetId;
            model.Questions = GetListKeysWithQuestionSets(questionaire.QuestionSetId);

            List<KeyValuePair<string, string>> questions = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> questionsForPdf = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> item in model.Questions)
            {
                string[] stringSeparators = new string[] { @"[""", @""",""", @"""]" };

                if (item.Value != null && item.Value != String.Empty)
                {
                    string[] values = item.Value.ToString().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    string value = values[random.Next(0, values.Count())];
                    questionsForPdf.Add(new KeyValuePair<string, string>(item.Key, value.ToString()));
                    value = value.Replace("\\n", "<br/>");
                    questions.Add(new KeyValuePair<string, string>(item.Key, value.ToString()));
                }
            }

            model.GeneratedQuestionary = questions.ToList();
            model.GeneratedQuestionary.Sort(Compare);
            model.GeneratedQuestionaryForPdf = questionsForPdf.OrderBy(q => q.Key).ToList();

            TempData["Questionnaire"] = model;
            return model;

        }


        public static int Compare(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
        {
            decimal aDec;
            decimal bDec;
            if (decimal.TryParse(a.Key, out aDec) && decimal.TryParse(b.Key, out bDec))
                return aDec.CompareTo(bDec);

            return a.Key.CompareTo(b.Key);
        }


        public ActionResult GenerateQuestionnairePDF()
        {
            TempData.Keep("Questionnaire");
            QuestionnairePreviewModel model = TempData["Questionnaire"] as QuestionnairePreviewModel;

            return File(model.GeneratePDF().ToArray(), "application/pdf", "Questionnaire.pdf");
        }


        public List<KeyValuePair<string, string>> GetListKeysWithQuestionSets(string questionSetID)
        {
            List<KeyValuePair<string, string>> questions = new List<KeyValuePair<string, string>>();
            if (questionSetID != null && questionSetID != String.Empty)
            {
                string[] splitted = questionSetID.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splitted.Length; i++)
                {
                    var numbers = splitted[i].Split(':').ToList();
                    questions.Add(new KeyValuePair<string, string>(numbers[0], GetQuestionsByQuestionSetID(numbers[1])));
                }
                questions.Sort((x, y) => y.Key.CompareTo(x.Key));
            }
            return questions;
        }


        public string GetQuestionsByQuestionSetID(string id)
        {
            string questions = "";
            QuestionSet questionSet = repository.QuestionSets.Where(q => q.QuestionSetId == Convert.ToInt32(id)).FirstOrDefault();
           
            if(questionSet!=null)
                questions = questionSet.Questions;
           
            return questions;
        }
        #endregion
    }
}
