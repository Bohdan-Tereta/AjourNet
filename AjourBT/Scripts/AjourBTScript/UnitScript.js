var un;

$(document).on("click", "#CreateUnit", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Unit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
   un = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Unit',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $("#createUnitForm").submit();
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

$(document).on("click", ".unitEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Unit"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    un = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Unit",
            position: {
                my: "center",
                at: "center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveUnit, #btnDeleteUnit").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteUnit", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteUnit-Confirm").load(url, function () {

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
                    $('#deleteUnitForm').submit()

                    $(this).dialog("destroy");
                    $(this).remove();

                    $(un).dialog("close");
                    $(un).remove();
                },

                "Cancel": function () {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(un).dialog("close");
                    $(un).remove();
                }
            }
        });
    });
    return false;
});

$(document).on("click", "#btnCancelDeleteUnit", function (event) {
    $("#deleteUnit-Confirm").dialog("close");
});

function UnitCreateOnSuccess(data)
{
    if (data.search("UnitData") != -1) {
        $('#UnitData').replaceWith($(data))

        $(un).dialog("close");
        $(un).remove();
    }
    else {
        $(un).html(data);
    }
}

function UnitEditOnSuccess(data)
{
    if (data.error) {
        $("#UnitModelError").html(data.error);
    }
    else if (data.search("UnitData") != -1) {
        $('#UnitData').replaceWith($(data))

        $(un).dialog("close");
        $(un).remove();
    }
    else {
        $(un).find("table").html($(data).find("table"));
    }
}
