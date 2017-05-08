using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.ViewModels;
using AjourBT.Tests.MockRepository;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AjourBT.Tests.Controllers
{
    class HRControllerTest
    {

        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        HRController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
                        mock.Setup(m => m.DeleteQuestionSet(0)).Returns<int>(null);
                        mock.Setup(m => m.DeleteQuestionSet(1)).Returns<int>((int questionSetId) => { return mock.Object.QuestionSets.FirstOrDefault(); });
                        mock.Setup(m => m.DeleteQuestionSet(2)).Returns<int>(null);
            messengerMock = new Mock<IMessenger>();
            controller = new HRController(mock.Object, messengerMock.Object);
        }

        #region GetQuestionSets

        [Test]
        public void GetQuestionSets_View()
        {
            //Arrange

            //Act
            ViewResult result = controller.GetQuestionSets();

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreateQuestionSet_View()
        {
            //Arrange

            //Act
            ViewResult result = controller.CreateQuestionSet(); 

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreateQuestionSet_JsonReaderExceptionIllFormedJson_Id0()
        {
            //Arrange 
            string questionSet = "";

            //Act 
            var result = controller.CreateQuestionSet(questionSet);

            //Assert 
            Assert.AreEqual(0, result);
        } 

        [Test]
        public void CreateQuestionSet_QuestionSet_QuestionSetId()
        {
            //Arrange 
            string serializedQuestionSet = "{\"QuestionSetId\":1,\"Title\":\"FirstQuestionSet\",\"Questions\":[\"FirstQuestion\",\"SecondQuestion\",\"ThirdQuestion\"]}";

            //Act 
            var result = controller.CreateQuestionSet(serializedQuestionSet);

            //Assert 
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CreateQuestionSet_NonExistingQuestionSet_Id0()
        {
            //Arrange 
            string questionSet = "{\"QuestionSetId\":0,\"Title\":null,\"Questions\":[]}";

            //Act 
            var result = controller.CreateQuestionSet( questionSet);

            //Assert 
            Assert.AreEqual( 0, result);
        }

        [Test]
        public void GetListOfQuestionSets_ListOfQuestionSets()
        {
            //Arrange

            //Act 
            JsonResult result = controller.GetListOfQuestionSets() as JsonResult;
            string json = new JavaScriptSerializer().Serialize(result.Data);
            string expected = "[{\"QuestionSetId\":1,\"Title\":\"FirstQuestionSet\",\"Questions\":\"[\\\"FirstQuestion\\\",\\\"SecondQuestion\\\",\\\"ThirdQuestion\\\"]\"},{\"QuestionSetId\":2,\"Title\":\"SecondQuestionSet\",\"Questions\":\"[\\\"FoutrthQuestion\\\",\\\"FifthQuestion\\\",\\\"SixthQuestion\\\"]\"}]";

            //Assert  
            Assert.AreEqual(expected, json);             
        } 

        [Test]
        public void DeleteQuestionSet_ProperId_True()
        {
            //Arrange

            //Act 
            var result = controller.DeleteQuestionSet(1);

            //Assert  
            Assert.AreEqual(true, result); 
            
        }

        [Test]
        public void DeleteQuestionSet_WrongId_False()
        {
            //Arrange

            //Act
            var result = controller.DeleteQuestionSet(0);

            //Assert    
            Assert.AreEqual(false, result); 

        }

        [Test]
        public void DeleteQuestionSet_ProperIdSaveError_False()
        {
            //Arrange

            //Act
            var result = controller.DeleteQuestionSet(2);

            //Assert    
            Assert.AreEqual(false, result);

        }

        #endregion

        #region GetQuestionnaire

        [Test]
        public void GetGetQuestionnaire_searchStringNotDefined_View()
        {
            //Arrange
            //Act
            var result = controller.GetQuestionnaire() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(String.Empty, result.ViewBag.SearchString);
        }

        [Test]
        public void GetQuestionnaire_searchstringNotEmpty_View()
        {
            //Arrange
            string searchString = "Test string";

            //Act
            var result = controller.GetQuestionnaire(searchString) as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        #endregion

        #region GetQuestionnaireList

        [Test]
        public void GetQuestionnaireList_returnJsonResult()
        {
            //Arrange
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string output = serializer.Serialize(mock.Object.Questionnaires.ToList());

            //Act
            var result = controller.GetQuestionnaireList() as JsonResult;
            var resultToJson = serializer.Serialize(result.Data);

            //Assert
            Assert.AreEqual(output, resultToJson);
        }

        #endregion

        #region Create Questionnaire

        [Test]
        public void GetCreateQuestionnaire_View()
        {
            //Arrange

            //Act
            var result = controller.CreateQuestionnaire();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
        }

        [Test]
        public void PostCreateQuestionnaire_aValidModel_ID0()
        {
            //Arrange

            //Act
            var result = controller.CreateQuestionnaire("{'QuestionnaireId':'0','Title':'Questionn111','QuestionSetId':['']}" );
            //Assert
            Assert.IsInstanceOf(typeof(int), result);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void PostCreateQuestionnaire_aValidModel_ID1()
        {
            //Arrange

            //Act
            var result = controller.CreateQuestionnaire("{'QuestionnaireId':'1','Title':'Questionnaire 1','QuestionSetId':['1']}");

            //Assert
            Assert.IsInstanceOf(typeof(int), result);
            Assert.AreEqual(0, result);

        }

          [Test]
        public void PostCreateQuestionnaire_InvalidValidModel_ID10()
        {
            //Arrange

            //Act
       

            controller.ModelState.AddModelError("error", "error");

            var result = controller.CreateQuestionnaire("");

            //Assert
            Assert.IsInstanceOf(typeof(int), result);
            Assert.AreEqual(0, result);
        }
        #endregion

        #region GenerateQuestionnaire
        
        [Test]
          public void GenerateQuestionnaire_aValidModel_ID1()
          {
              //Arrange

              //Act
              var result = controller.GenerateQuestionnaire(1);
              var modelResult = ((ViewResult)result).Model as QuestionnairePreviewModel;           
              
              //Assert
              Assert.IsInstanceOf(typeof(ViewResult), result);
              Assert.AreEqual("~/Views/HR/GenerateQuestionnaire.cshtml", ((ViewResult)result).ViewName);
              Assert.AreEqual(1, modelResult.QuestionnaireId);
              Assert.AreEqual("5:1", modelResult.QuestionSetId);
              Assert.AreEqual("Questionnaire 1", modelResult.Title);
              Assert.IsTrue(modelResult.GeneratedQuestionary.Count()==1);
              Assert.IsTrue(modelResult.GeneratedQuestionary[0].Key == "5");
          }

        [Test]
        public void GenerateQuestionnaire_NotValidModel_EmptyListOfQuestions()
        {
            //Arrange

            //Act
            var result = controller.GenerateQuestionnaire(4);
            var modelResult = ((ViewResult)result).Model as QuestionnairePreviewModel;

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(4, modelResult.QuestionnaireId);
            Assert.AreEqual("Questionnaire 4", modelResult.Title);
            Assert.IsTrue(modelResult.GeneratedQuestionary.Count() == 0);
        }

         
        [Test]
          public void GenerateQuestionnaire_InvalidID_ID100()
          {
              //Arrange

              //Act
              var result = controller.GenerateQuestionnaire(100);
              var modelResult = ((ViewResult)result).Model as QuestionnairePreviewModel;

              //Assert
              Assert.IsNull(modelResult);
              Assert.IsInstanceOf(typeof(ViewResult), result);
              Assert.AreEqual("~/Views/Error/ShowErrorPage.cshtml", ((ViewResult)result).ViewName);
              
          }

        [Test]
        public void GenerateQuestionnaireModel_aValidModel_ID1()
        {
            //Arrange
            Questionnaire quest = new Questionnaire { QuestionnaireId = 1, Title = "Questionnaire 1", QuestionSetId = "5:1" };
            //Act
            var result = controller.GenerateQuestionnaireModel(quest);
        

            //Assert
            Assert.IsInstanceOf(typeof(QuestionnairePreviewModel), result);
            Assert.AreEqual(1, result.QuestionnaireId);
            Assert.AreEqual("5:1", result.QuestionSetId);
            Assert.AreEqual("Questionnaire 1", result.Title);
            Assert.IsTrue(result.GeneratedQuestionary.Count() == 1);
            Assert.IsTrue(result.GeneratedQuestionary[0].Key == "5");
        }

       
        [Test]
        public void GenerateQuestionnaireModel_EmptyListOfQuestions()
        {
            //Arrange
           Questionnaire quest = new Questionnaire { QuestionnaireId = 4, Title = "Questionnaire 4", QuestionSetId = "0:0" };

            //Act
           var result = controller.GenerateQuestionnaireModel(quest);

            //Assert
           Assert.IsInstanceOf(typeof(QuestionnairePreviewModel), result);
            Assert.AreEqual(4, result.QuestionnaireId);
            Assert.AreEqual("Questionnaire 4", result.Title);
            Assert.IsTrue(result.GeneratedQuestionary.Count() == 0);
        }


        [Test]
        public void GenerateQuestionnaireModel_InvalidID_ID100()
        {
            //Arrange
            Questionnaire quest = new Questionnaire { QuestionnaireId = 100, Title = "Questionnaire 4", QuestionSetId = "0:0" };

            //Act
            var result = controller.GenerateQuestionnaireModel(quest);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(QuestionnairePreviewModel), result);
            Assert.AreEqual("Questionnaire 4", result.Title);

        }

           [Test]
          public void GetListKeysWithQuestionSets_ValidString()
          {
              //Arrange
              string questSetID = "5:1";
              //Act
              var result = controller.GetListKeysWithQuestionSets(questSetID);

              //Assert
              Assert.IsNotNull(result);
              Assert.IsInstanceOf(typeof(List<KeyValuePair<string, string>>), result);
              Assert.AreEqual("5", result[0].Key);
              Assert.AreEqual("[\"FirstQuestion\",\"SecondQuestion\",\"ThirdQuestion\"]", result[0].Value);              
          }
         
        
        [Test]
        public void GetListKeysWithQuestionSets_Null()
          {
              //Arrange
              string questSetID = null;
              //Act
              var result = controller.GetListKeysWithQuestionSets(questSetID);

              //Assert
              Assert.IsNotNull(result);
              Assert.IsInstanceOf(typeof(List<KeyValuePair<string, string>>), result);
              Assert.AreEqual(result.Count(), 0);

          }
       
        
        [Test]
        public void GetQuestionsByQuestionSetID_Null()
        {
            //Arrange
            string questSetID = null;
            //Act
            var result = controller.GetQuestionsByQuestionSetID(questSetID);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(result, String.Empty);
        }


        [Test]
        public void GetQuestionsByQuestionSetID_ValidID()
        {
            //Arrange
            string questSetID = "1";
            //Act
            var result = controller.GetQuestionsByQuestionSetID(questSetID);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(result, "[\"FirstQuestion\",\"SecondQuestion\",\"ThirdQuestion\"]");
        }

        [Test]
        public void GetQuestionsByQuestionSetID_InValidID()
        {
            //Arrange
            string questSetID = "100";
            //Act
            var result = controller.GetQuestionsByQuestionSetID(questSetID);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.IsTrue(result=="");
        }
        
        [Test]
        public void Compare_OrderedList()
        {
            //Arrange
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string,string>("1","value"));
            list.Add(new KeyValuePair<string, string>("10", "value"));
            list.Add(new KeyValuePair<string, string>("1a", "value"));
            list.Add(new KeyValuePair<string, string>("2", "value"));
            list.Add(new KeyValuePair<string, string>("3", "value"));
            list.Add(new KeyValuePair<string, string>("1.1", "value"));

            //Act
            list.Sort(HRController.Compare);

           //Assert
           Assert.AreEqual(list[0].Key, "1");
           Assert.AreEqual(list[1].Key, "1.1");
           Assert.AreEqual(list[2].Key, "1a");
           Assert.AreEqual(list[3].Key, "2");
           Assert.AreEqual(list[4].Key, "3");
           Assert.AreEqual(list[5].Key, "10");

        }



        #endregion

        #region SetDropDownList()

        [Test]
        public void SetDropDownList_QuestionnaireIsNull_DictionarywithDefaultValues()
        {
            //Arrange
            Questionnaire quest = new Questionnaire();

            //Act
            List<KeyValuePair<string, List<SelectListItem>>> result = controller.SetDropDownList(quest);
            KeyValuePair<string, List<SelectListItem>> pair = result.FirstOrDefault();

            //Assert
            //Assert.AreEqual(1, result.Keys.Count);
            //Assert.AreEqual(1, result.Values.Count);

            Assert.AreEqual(String.Empty, pair.Key);
            Assert.AreEqual(2, pair.Value.Count);

            Assert.AreEqual("FirstQuestionSet", pair.Value.FirstOrDefault().Text);
            Assert.AreEqual("SecondQuestionSet", pair.Value.LastOrDefault().Text);
        }

        [Test]
        public void SetDropDownList_QuestionnaireIsNotNull_DictionarywithCorrectValues()
        {
            //Arrange
            Questionnaire quest = mock.Object.Questionnaires.Where(q => q.QuestionnaireId == 1).FirstOrDefault();

            //Act
            List<KeyValuePair<string, List<SelectListItem>>> result = controller.SetDropDownList(quest);
            KeyValuePair<string, List<SelectListItem>> pair = result.FirstOrDefault();

            //Assert
            //Assert.AreEqual(1, result.Keys.Count);
            //Assert.AreEqual(1, result.Values.Count);

            Assert.AreEqual("5", pair.Key);
            Assert.AreEqual(2, pair.Value.Count);

            Assert.AreEqual("FirstQuestionSet", pair.Value.FirstOrDefault().Text);
            Assert.AreEqual("SecondQuestionSet", pair.Value.LastOrDefault().Text);
            Assert.AreEqual(true, pair.Value.FirstOrDefault().Selected);
        }

        #endregion
    }
}
