var pos;

$(document).on("click", "#CreatePosition", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Position";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    pos = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Position',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $('#createPositionForm').submit();
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

$(document).on("click", ".posEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Position"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    pos = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Position",
            position: {
                my: "center",
                at: "center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $("#btnSavePosition, #btnDeletePosition").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeletePosition", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deletePosition-Confirm").load(url, function () {

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

            buttons: {
                "OK": function () {
                    $('#positionDeleteForm').submit();

                    $(this).dialog("destroy");
                    $(this).remove();

                    $(pos).dialog("close");
                    $(pos).remove();
                },
                "Cancel": function () {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(pos).dialog("close");
                    $(pos).remove();
                }
            }
        });
    });
    return false;
});

$(document).on("click", "#btnCancelDeletePosition", function (event) {
    $("#deletePosition-Confirm").dialog("close");
});

function PositionCreateOnSuccess(data)
{
    if (data.search("PositionData") != -1) { 
        $('#PositionData').replaceWith($(data))

        $(pos).dialog("close");
        $(pos).remove();
    }
    else {
        $(pos).html(data);
    }
}

function PositionEditOnSuccess(data)
{
    if (data.error) {
        $("#PositionModelError").html(data.error);
    }
    else if (data.search("PositionData") != -1)
    {
        $('#PositionData').replaceWith($(data))

        $(pos).dialog("close");
        $(pos).remove();
    }
    else {
        $(pos).find("table").html($(data).find("table"));
    }


}
