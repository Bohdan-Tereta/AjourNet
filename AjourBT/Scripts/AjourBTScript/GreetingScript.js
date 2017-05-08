AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on('click', '#CreateGreeting', function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = 'Create Greeting';
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');

    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            rasizable: false,
            title: 'Create Greeting',
            position: {
                my: 'center',
                at: 'center'
            },
            buttons: {
                "Save": function () {
                    $('#createGreetingForm').submit();
                }
            },
            close: function (event, ui) {
                $(this).dialog('destroy');
                $(this).remove();
            }
        })
        $.validator.unobtrusive.parse(this);
    })
})

$(document).on("click", ".greetingEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Greeting"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    loc = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Greeting",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {
                $("#btnSaveGreeting").click(function (event) {
                    event.preventDefault();
                    $("#editGreetingForm").validate();
                    if ($("#editGreetingForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/Greeting/Edit/",
                            type: "POST",
                            data: AddAntiForgeryToken($("#editGreetingForm").serialize()),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(loc).dialog("close");
                                }
                            }
                        });
                    }
                })
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Greeting/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#GreetingData").replaceWith($(data));
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveGreeting, #btnDeleteGreeting").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteGreeting", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteGreeting-Confirm").load(url, function () {

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

$(document).on("click", "#btnCancelDeleteGreeting", function (event) {
    $("#deleteGreeting-Confirm").dialog("close");
});