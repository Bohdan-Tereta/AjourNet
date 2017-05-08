AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on("click", "#forgotPassword", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "resetPwrd";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Reset password',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Recover": function () {
                    $.ajax({
                        cache: false,
                        url: "/Account/ForgotPassword/",
                        type: "Post",
                        data: AddAntiForgeryToken($("#forgotPasswordForm").serialize()),
                        success: function (data) {
                            $("div #resetPwrd").html(data);
                        }

                    });
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

$(document).on("submit", "#forgotPasswordForm", function (event) {
    event.preventDefault();
    $.ajax({
        cache: false,
        url: "/Account/ForgotPassword/",
        type: "Post",
        data: AddAntiForgeryToken($("#forgotPasswordForm").serialize()),
        success: function (data) {
            $("div #resetPwrd").html(data);
        }

    });
});

