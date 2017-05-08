var loc;
var locEdit;

$(document).on("click", "#CreateLocationButton", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "CreateLocation";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    loc =  $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Location',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $('#createLocationForm').submit();
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

$(document).on("click", ".locEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Location"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    locEdit = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Location",
            position: {
                my: "center",
                at: "center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveLocation, #btnDeleteLocation").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteLocation", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteLocation-Confirm").load(url, function () {

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
                    $('#deleteLocationConfirmedForm').submit();

                    $(this).dialog("destroy");
                    $(this).remove();

                    $(locEdit).dialog("close");
                    $(locEdit).remove();
                },
                "Cancel": function () {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(locEdit).dialog("close");
                    $(locEdit).remove();
                }
            }
        });
    });
    return false;
});

$(document).on("click", "#btnCancelDeleteLocation", function (event) {
    $("#deleteLocation-Confirm").dialog("close");
});

function LocationCreateOnSuccess(data)
{
    if (data.search("LocationData") != -1) {
        $('#LocationData').replaceWith($(data))

        $(loc).dialog("close");
        $(loc).remove();
    }
    else {
        $(loc).html(data);
    }
}

function LocationEditOnSuccess(data)
{
    if (data.error) {
        $("#LocationEditModelError").html(data.error);
    }
    else if (data.search("LocationData") != -1) {
        $('#LocationData').replaceWith($(data))

        $(locEdit).dialog("close");
        $(locEdit).remove();
    }
    else {
        $(locEdit).find("table").html($(data).find("table"));
    }
}
