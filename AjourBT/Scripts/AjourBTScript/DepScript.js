var dialogId;
var dialogDiv;
var dep;
var dept;

$(document).on("click", "#CreateDepartment", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Department";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    dept = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create department',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $('#createDepartmentForm').submit();
                }
            },
          
            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });

    return false;
});

$(document).on("click", ".depEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    dialogId = "Edit department"
    dialogDiv = "<div id='" + dialogId + "'></div>";
    dep = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Department",
            position: {
                my: "center",
                at: "center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveDepartment, #btnDeleteDepartment").button();
        $.validator.unobtrusive.parse(this);
    });

    return false;
});


$(document).on("click", "#btnDeleteDepartment", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteDepartment-Confirm").load(url, function () {

        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: title,
            position: {
                my: "center",
                at: "center"
            },
            buttons:{
                "OK": function () {
                    $('#deleteConfirmedForm').submit();

                    $(this).dialog("destroy");
                    $(this).remove();

                    $(dep).dialog("close");
                    $(dep).remove();
                },
                "Cancel": function ()
                {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(dep).dialog("close");
                    $(dep).remove();
                }
            }
        });
    });

    return false;
});

$(document).on("click", "#btnCancelDeleteDepartment", function (event) {
    $("#deleteDepartment-Confirm").dialog("close");
});


    AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
        return data;
    };

    function DepartmentCreateOnSuccess(data)
    {
        if (data.search("DepartmentData") != -1) {
            $('#DepartmentData').replaceWith($(data))

            $(dept).dialog("close");
            $(dept).remove();
        }
        else {
            $(dept).html(data);
        }
    }

    function DepartmentEditOnSuccess(data)
    {
        if (data.error) {
            $("#DepartmentModelError").html(data.error);
        }
        else if (data.search("DepartmentData") != -1) {
            $('#DepartmentData').replaceWith($(data))

            $(dep).dialog("close");
            $(dep).remove();
        }
        else {
            $(dep).find("table").html($(data).find("table"));
        }
    }