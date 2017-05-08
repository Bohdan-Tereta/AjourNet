var element;
var dialogQuestionnaireDelete;

//GetQuestionSets view model
function QuestionSetsModel() {
    this.questionSets = ko.observableArray();
    this.deleteQuestionSet = function (questionSetId) {
        var self = this;
        $.ajax({
            url: "/HR/DeleteQuestionSet",
            type: "POST",
            data: "questionSetId=" + questionSetId,
            success: function (data) {
                if (data == "True") {
                    getQuestionSetsModel.questionSets.remove(function (item) {
                        if (item.QuestionSetId() == questionSetId) {
                            return true;
                        }
                        return false;
                    });
                    dialogCreateQuestionSetExternalRef.dialog("close");
                }
            }
        });
    }
};

var getQuestionSetsModel = new QuestionSetsModel();

function getAllItems() {
    $.getJSON(
        "/HR/GetListOfQuestionSets", function (data) {
            $.each(data, function (index, obj) {
                var newQSet = new QuestionSet(obj);
                getQuestionSetsModel.questionSets.push(newQSet)
            })

        });
}

function QuestionSet(obj) {
    this.QuestionSetId = ko.observable(obj.QuestionSetId),
    this.Title = ko.observable(obj.Title),
    this.Questions = fillQuestions(obj)
}

function fillQuestions(obj) {
    var Questions = ko.observableArray()

    $.each($.parseJSON(obj.Questions), function (ind, obj) {
        Questions.push(ko.observable(obj))
            })
    //Empty question for Add
    Questions.push(ko.observable(""))
    return Questions
} 



//CreateQuestionSet view model
function CreateQuestionSet() {
    this.QuestionSetId = ko.observable("0"),
    this.Title = ko.observable(""),
    this.Questions = ko.observableArray([ko.observable("")]),

    this.SaveData = function () {
        //this.Questions.pop();
        if (this.Title() != null && this.Title().trim() != "") {
            //Remove empty question for Add before serialization
            var isValid = true;
            for (var i = 0; i < this.Questions().length - 1; i++) {
                if (this.Questions()[i]().trim() == "") {
                    isValid = false;
                }
            }
            if (isValid == true) {
                this.Questions.pop();
                var data_to_send = encodeURIComponent(ko.toJSON(this));
                //Return empty question for Add after serialization
                this.Questions.push(ko.observable(""));
                var self = this;
                $.ajax({
                    url: "/HR/CreateQuestionSet",
                    type: "POST",
                    data: "questionSet=" + data_to_send,
                    success: function (data) {
                        if (data > 0) {
                            self.QuestionSetId = ko.observable(data);
                            var questionSetFromQuestionnaryModel = ko.utils.arrayFirst(getQuestionSetsModel.questionSets(), function (item) {
                                return data == item.QuestionSetId();
                            });
                            if (questionSetFromQuestionnaryModel == null) {
                                getQuestionSetsModel.questionSets.push(self);
                            }
                            else {
                                getQuestionSetsModel.questionSets.replace(questionSetFromQuestionnaryModel, self);
                                questionSetFromQuestionnaryModel = ko.observable(self);
                            }
                            dialogCreateQuestionSetExternalRef.dialog("close");
                        }
                    }
                });
            }
            else {
                alert("Please enter the Question. It can not be empty! ");
            }
        }
        else {
            alert("Please enter the QuestionSet Title. It can not be empty! ");
        }
    }
}

var questionSetModelCreate;
var dialogCreateQuestionSetExternalRef;
$(document).on("click", "#createQuestionSetButton, .questionSetTitle", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "create-QuestionSet";
    var dialogDiv = "<div QuestionSetId='" + dialogId + "'></div>";
    var title = ""; 
    //Create questionSetModelCreate model
    if ($(this).attr("class") != "questionSetTitle") {
        questionSetModelCreate = new CreateQuestionSet();
        title = "Create QuestionSet"; 
    }
    else
    {
        //edit
        var questionSetFromQuestionnaryModel = ko.utils.arrayFirst(getQuestionSetsModel.questionSets(), function (item) {
            return element.attr("data-questionset-id") == item.QuestionSetId();
        });
        questionSetModelCreate = new CreateQuestionSet();
        questionSetModelCreate.QuestionSetId = ko.observable(questionSetFromQuestionnaryModel.QuestionSetId());
        questionSetModelCreate.Questions.pop(); 
        $.each(questionSetFromQuestionnaryModel.Questions(), function (ind, obj) {
            questionSetModelCreate.Questions.push(ko.observable(obj()))
        }); 
        questionSetModelCreate.Title = ko.observable(questionSetFromQuestionnaryModel.Title()); 
        title = "Edit QuestionSet";
    }
    $(dialogDiv).load(this.href, function () {
        var dialogCreateQuestionSet = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: title,
            position: {
                my: "center center",
                at: "center center"
            },
            open: function (event, ui) {
                //Apply questionSetModelCreate model bindings
                dialogCreateQuestionSetExternalRef = $(this);
                ko.applyBindings(questionSetModelCreate, document.getElementById('createQuestionSetTable'));
            },
            close: function (event, ui) {                
                ko.cleanNode(document.getElementById("createQuestionSet"));
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $(".questionToAdd").button();
        $(".questionDelete").button();
        $("#saveQuestionSet").button();
        $("#deleteQuestionSet").button();
    });
    return false;
});


$(document).on("click", ".questionTitle", function (event) {
    event.preventDefault();
    element = $(this);
    var dialogId = "create-Questionnaire";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var questionSetModelCreate = new CreateQuestionSet();
    $(dialogDiv).load(this.href, function () {
        dialogEmpEdit = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Questionnaire",
            position: {
                my: "center center",
                at: "center center"
            },
            open: function (event, ui) {
                var myElements = document.querySelectorAll("#questionSetDelete");

                for (var i = 0; i < myElements.length; i++) {
                    $(myElements[i]).button();
                }
                //$("#questionSetDelete").each().button();
                $("#questionSetToAdd").button();
                $("#questionnaireSaveBtn").button();
                $("#questionnaireDeleteBtn").button();
                $("#generateQuestButton").button();
            },

            beforeClose: function () {
                $.ajax({
                    type: 'GET',
                    url: '/HR/GetQuestionnaire',
                    success: function (data) {
                        $("#ui-tabs-2").html(data); 
                    }
                })
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
    });
    return false;
});

$(document).on("click", "#questionSetDelete", function (event) {
    event.preventDefault();
    var count = $("[id=questionDataRow]").length;
    if (count > 1) {
        //check num of tr node if == 1 then cant remove
        $(this).closest('tr').remove();
    }
})

$(document).on("click", "#questionSetToAdd", function (event) {
    var node = document.getElementById("questWindowTable").lastElementChild;
    if ($(node).find("#orderVal").val().trim() != "" && $(node).find("select[name=testDropDown]").val() != "0") {
        if (!($(node).find("#orderVal").val().indexOf(',') > -1)) {
            var clone = node.cloneNode(true, true);
            $(clone).find("#orderVal").val("");
            $(clone).find("select[name=testDropDown]").val("0");
            var result = document.getElementById("questWindowTable");
            var deleteButtonString = '<a id="questionSetDelete" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" role="button" aria-disabled="false"><span class="ui-button-text">Delete</span></a>';
            $(node).find("#questionSetToAdd").replaceWith(deleteButtonString);
            $(node).find("#questionSetDelete").text("Delete");
            $(node).find("#questionSetDelete").button();
            result.appendChild(clone);
            var button = $(clone).find("#questionSetToAdd");
            $(clone).find("#questionSetToAdd").text("Add");
            $(button).button();
        }
        else {
            alert("Order number cannot contain comma");
        }
    }
    else {
        alert(" № and QuestionSet can not be empty! ");
    }
})

$(document).on("click", ".questionDelete", function (event) {
    var index = $(event.target).attr("data-questionsetid");
    questionSetModelCreate.Questions.splice(index, 1);
})

$(document).on("click", ".questionToAdd", function (event) {
    //selector may be shortened // or databind used
    var question = $(event.target).parent().parent().parent().find(".questionInput").val();
    if (question != null && question.trim() != "") {
        questionSetModelCreate.Questions()[questionSetModelCreate.Questions().length - 1] = ko.observable( question );
        questionSetModelCreate.Questions.push(ko.observable(""));
    }
    else 
    {
        alert("Please enter the Question. It can not be empty! ");
    }
})

//$(document).on("click", "#deleteQuestionSet", function (event) {
//    getQuestionSetsModel.deleteQuestionSet(questionSetModelCreate.QuestionSetId());
//})

var createQuestionSetModel;

$(document).on("click", "#questionnaireSaveBtn", function () {

    var fd = new FormData()
    var questId = $("#QuestionnaryID").val();
    var QuestionSetId = [];
    var orderVal = [];
    var title = $("#Title").val();
    if (title != null && title.trim() != "") {

        QuestionSetId = $("select[name='testDropDown'] option:selected").map(function () {
            return this.value
        }).get()

        orderVal = $("input[id='orderVal']").map(function () {
            return this.value
        }).get()

        var isValid = true;
        var posh = QuestionSetId.length;
        QuestionSetId.splice(posh - 1, 1);
        orderVal.splice(posh - 1, 1);
        posh--;
        var c = 0;
        for (var i = 0; i < posh; i++) {
            if (orderVal[i - c] != "" && (QuestionSetId[i - c] != "" && QuestionSetId[i - c] != "0")) {
                orderVal[i - c] += (":" + QuestionSetId[i - c]);
            }
            else {
                QuestionSetId.splice(i - c, 1);
                orderVal.splice(i - c, 1);
                c++;
                isValid = false;
            }
        }
        if (isValid != false) {
            fd.append("QuestionnaireId", questId)
            fd.append("QuestionSetId", orderVal)
            fd.append("Title", title)

            $.ajax({
                url: "/HR/SaveQuestionnaire",
                data: fd,
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (data) { }
            })
            $("#create-Questionnaire").dialog("close");
        }
        else {
            alert(" № and QuestionSet can not be empty! ");

        }
    }
    else {
        alert("Please enter the Questionnaire Title. It can not be empty! ");
    }

})

$(document).on("click", "#generateQuestButton", function () {
    var fd = new FormData()
    var questId = $("#QuestionnaryID").val();
    var QuestionSetId = [];
    var orderVal = [];
    var title = $("#Title").val();
    if (title != null && title.trim() != "") {

        QuestionSetId = $("select[name='testDropDown'] option:selected").map(function () {
            return this.value
        }).get()

        orderVal = $("input[id='orderVal']").map(function () {
            return this.value
        }).get()

        var isValid = true;
        var posh = QuestionSetId.length;
        QuestionSetId.splice(posh - 1, 1);
        orderVal.splice(posh - 1, 1);
        posh--;
        var c = 0;
        for (var i = 0; i < posh; i++) {
            if (orderVal[i - c] != "" && (QuestionSetId[i - c] != "" && QuestionSetId[i - c] != "0")) {
                orderVal[i - c] += (":" + QuestionSetId[i - c]);
            }
            else {
                QuestionSetId.splice(i - c, 1);
                orderVal.splice(i - c, 1);
                c++;
                isValid = false;
            }
        }
        if (isValid != false) {
            fd.append("QuestionnaireId", questId)
            fd.append("QuestionSetId", orderVal)
            fd.append("Title", title)
            $.ajax({
                url: "/HR/SaveQuestionnaire",
                data: fd,
                type: 'POST',
                processData: false,
                contentType: false,
                success: function (data) {


                }
            });

            $.ajax({
                url: "/HR/GenerateQuestionnaire",
                data: { questionaireID: questId },
                type: 'GET',
                success: function (data) {
                    var generateQ = $("#generateQ"); 
                    generateQ.html(data.trim());
                    var dialogGenerateQuestionnaire = generateQ.dialog({
                        modal: true,
                        height: 'auto',
                        width: 'auto',
                        maxWidth: '100%',
                        resizable: false,
                        title: "Generate Questionnaire",
                        position: {
                            my: "center center",
                            at: "center center",
                            of: window
                        },
                        open: function (event, ui) {
                            $(this).html(data);
                            $(this).dialog('option', 'position', 'center');
                        },
                        close: function (event, ui) {
                            dialogGenerateQuestionnaire.dialog("destroy");
                            generateQ.empty();
                        }
                    });
                }
            });

        }
        else {
            alert(" № and QuestionSet can not be empty! ");

        }
    }
    else {
            alert("Please enter the Questionnaire Title. It can not be empty! "); 
        }
})

$(document).on("click", "#questionnaireDeleteBtn", function (event) {
    event.preventDefault();
    var questId = $("#QuestionnaryID").val();
    var element = $(this);

    $("#ConfirmQuestionnaireDelete").html("<p>Please Confirm</p>")
    $("#ConfirmQuestionnaireDelete").dialog({
        modal: true,
        height: 'auto',
        width: '20%',
        resizable: false,
        title: "Confirm Delete Questionnaire",
        position: {
            my: "center",
            at: "center"
        },
        buttons: {
            "OK": {
                text: "OK",
                id: "okDeleteConfirm",
                click: function () {

                var formD = new FormData()
    formD.append("questId", questId);

    $.ajax({
        url: "/HR/DeleteQuestionnaire",
        data: formD,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (data) { }

                    })
                $(this).dialog("destroy");
    $("#create-Questionnaire").dialog("close");
                }
            },

            "Cancel": {
                text: "Cancel",
                id: "cancelDeleteConfirm",
                click: function () {
                $(this).dialog("destroy");
            }
        }
        }
    })
    //$("#create-Questionnaire").dialog("close");
});

$(document).on("click", "#addQuestionnaireButton", function (event) {
    //event.preventDefault();
    var element = $(this);
    // alert(this.href)
    var dialogId = "create-Questionnaire";
    var dialogDiv = "<div QuestionnaireId='" + dialogId + "'></div>";
    //Create questionSetModelCreate model
    var num = 0;
    var questModelCreate = new CreateQuestionnaire(num);
    $(dialogDiv).load(this.href, function () {
        dialogCreateQuestionnaire = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create Questionnaire",
            position: {
                my: "center center",
                at: "center center"
            },
            open: function (event, ui) {
                //Apply questionSetModelCreate model bindings
                ko.applyBindings(questModelCreate, document.getElementById('createQuestionnaireTable'));
                $("#addQuestionnaire").button();
            },
            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
    });
    return false;
});

$(document).on("click", "#deleteQuestionSet", function (event) {
    event.preventDefault();
    var element = $(this);
    $("#ConfirmQuestionSetDelete").html("<p>Please Confirm</p>")
    $("#ConfirmQuestionSetDelete").dialog({
            modal: true,
            height: 'auto',
            width: '20%',
            resizable: false,
            title: "Confirm Delete QuestionSet",
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "OK":{
                    text: "OK",
                    id: "okDeleteConfirm",
                    click: function () {
                        getQuestionSetsModel.deleteQuestionSet(questionSetModelCreate.QuestionSetId());
                        $(this).dialog("destroy");

                        //$(locEdit).dialog("close");
                        //$(locEdit).remove();
                    }
                },
                "Cancel":{
                    text: "Cancel",
                    id: "cancelDeleteConfirm",
                    click: function () {
                        $(this).dialog("destroy");

                    //$(locEdit).dialog("close");
                    //$(locEdit).remove();
                }
            }
            }
        });
    return false;
});