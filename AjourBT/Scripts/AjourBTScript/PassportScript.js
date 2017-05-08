var scrollPosition;

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

$(document).on("click", "#AddDatePassport", function (event) {
    event.preventDefault();
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    var element = $(this);
    var dialogId = "passportDialog-create";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    $(dialogDiv).load(url, function () {
        var dialogPassportAddDate = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Add Passport's End Date",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#CreatePassportDate').length > 0) {
                    $('#CreatePassportDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                $("#btnSavePassport").button();
                $("#btnSavePassport").click(function (event) {
                        event.preventDefault();
                        $("#AddDatePassportForm").validate();
                        if ($("#AddDatePassportForm").valid()) {
                            $.ajax({
                                type: "POST",
                                url: "/BTM/PassportAddDate",
                                data: AddAntiForgeryToken($('#AddDatePassportForm').serialize()),
                                success: function (data) {
                                    if (data.error) {
                                        $("#PassportModellError").html(data.error);
                                    }
                                    else {
                                        $(dialogPassportAddDate).dialog("close");
                                    }
                                },

                                error: function (data) {
                                    alert("Server is not responding");
                                }
                            });
                        }
                })
    },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BTM/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("visaTableForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#visaTableForBTM");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                }
                    });
            },

            close: function (event, ui) {
                $('#CreatePassportDate').datepicker("destroy");
                $(dialogPassportAddDate).dialog("destroy");
                $(dialogPassportAddDate).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});


$(document).on("click", "#passportEdit", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "passportDialog-edit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    var parentID = element.parent().parent().attr("id");
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Update Passport',
            position: {
                my: "center",
                at: "center"
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BTM/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("visaTableForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#visaTableForBTM");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                });
            },


            open: function (event, ui) {
                if ($('#editPassport').length > 0) {
                    $('#editPassport').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                $("#btnSaveEditedPassport").button();
                $("#btnSaveEditedPassport").click(function (event) {
                        event.preventDefault();
                        $('#EditPassportForm').validate();
                        if ($("#EditPassportForm").valid()) {
                            $.ajax({
                                type: 'POST',
                                url: '/BTM/PassportEditDate',
                                data: AddAntiForgeryToken($('#EditPassportForm').serialize()),

                                success: function (data) {
                                    if (data.error) {
                                        $("#PassportModelError").html(data.error);
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
            close: function (event, ui) {
                $('#editPassport').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);

    });
    return false;
});