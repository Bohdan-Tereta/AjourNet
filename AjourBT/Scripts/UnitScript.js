var un;

$(document).on("click", "#CreateUnit", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Unit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
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

$(document).on("click", ".unEditDialog", function (event) {
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

            open: function () {
                $("#btnSaveUnit").click(function (event) {
                    event.preventDefault();
                    $("#editUnitForm").validate();
                    if ($("#editUnitForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/Unit/Edit/",
                            type: "POST",
                            data: AddAntiForgeryToken($("#editUnitForm").serialize()),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(un).dialog("close");
                                }
                            }
                        });
                    }
                })
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Unit/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#UnitData").replaceWith($(data));
                    }
                });
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
            }
        });
    });
    return false;
});

$(document).on("click", "#btnCancelDeleteUnit", function (event) {
    $("#deleteUnit-Confirm").dialog("close");
});
