
$(document).on("click", "#RejectBTbyDIR", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Reject-BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Reject BT",
            position: {
                my: "center",
                at: "center",
            },
            open: function () {
                $("#RejectBtnDIR").button();
                $("#RejectBtnDIR").click(function (event) {
                    event.preventDefault();
                    $("#RejectBTDIRForm").validate;
                    if ($("#RejectBTDIRForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/DIR/Reject_BT_DIR/",
                            type: "POST",
                            data: $("#RejectBTDIRForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $("#" + dialogId).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                })
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/DIR/GetBusinessTripDataDIR/",
                    type: "Get",
                    data: $('#formDIR').serialize(),
                    success: function (data) {
                        var elementId = document.getElementById("BTsDataForDirector").getElementsByTagName("th");
                        var scrollPositionObject = $("div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#BTsDataForDirector");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        setTimeout(function () { $("div.dataTables_scrollBody").scrollTop(scrollPositionBTBefore); }, 100);

                    }

                })
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


AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};




