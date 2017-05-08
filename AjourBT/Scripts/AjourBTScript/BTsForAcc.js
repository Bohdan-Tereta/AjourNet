function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPositionACC;
var dialogACC
$(document).on("click", "#EditReportedBTACC", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Update BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionACC = $("div#tableViewBTsACC > div#exampleBtsView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    var dateFormat = element.attr('data-date-format');
    $(dialogDiv).load(this.href, function () {
        dialogACC = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit BT",
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                $("#btnSave").focus();
                if ($('#editEndDateACC').length > 0) {
                    $('#editEndDateACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#editEndDateACCFinished').length > 0) {
                    $('#editEndDateACCFinished').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#editStartDateFutureBTACC').length > 0) {
                    $('#editStartDateFutureBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editEndDateFutureBTACC').length > 0) {
                    $('#editEndDateFutureBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderStartDateFutureBTACC').length > 0) {
                    $('#orderStartDateFutureBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderEndDateFutureBTACC').length > 0) {
                    $('#orderEndDateFutureBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderEndDateCurrentBTACC').length > 0) {
                    $('#orderEndDateCurrentBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderEndDateFinishedBTACC').length > 0) {
                    $('#orderEndDateFinishedBTACC').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                $("#btnChangeFinished, #btnSaveCurrent, #btnSaveFuture, #CancelBTbyACC, #btnSaveAccComment").button();

                $("#btnChangeFinished").click(function (event) {
                    event.preventDefault();
                    $("#editFinishedBTForm").validate();
                    if ($("#editFinishedBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ACC/EditReportedBT/",
                            data: $("#editFinishedBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogACC).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                }),
                $("#btnSaveCurrent").click(function (event) {
                    event.preventDefault();
                    $("#editCurrentBTForm").validate();
                    if ($("#editCurrentBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ACC/EditReportedBT/",
                            data: $("#editCurrentBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogACC).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                }),

                $("#btnSaveFuture").click(function (event) {
                    event.preventDefault();
                    $("#editFutureBTForm").validate();
                    if ($("#editFutureBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ACC/EditReportedBT/",
                            data: $("#editFutureBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else if (data.success) {
                                    $(dialogACC).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                }),

                $("#btnSaveAccComment").click(function (event) {
                    event.preventDefault();
                    $("#saveAccCommentForm").validate();
                    if ($("#saveAccCommentForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ACC/SaveAccComment/",
                            data: $("#saveAccCommentForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogACC).dialog("close");
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
                    url: "/ACC/GetBusinessTripDataACC/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val(), searchString: $("#searchInputACC").val() },
                    success: function (data) {

                        var elementId = document.getElementById("tableViewBTsACC").getElementsByTagName("th");
                        var scrollPositionObject = $("div#tableViewBTsACC > div#exampleBtsView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#tableViewBTsACC");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionACC = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionACC);
                    }
                });
            },

            close: function (event, ui) {
                $('#editEndDateFutureBTACC').datepicker("destroy");
                $('#editStartDateFutureBTACC').datepicker("destroy");
                $('#editEndDateACC').datepicker("destroy");
                $('#editEndDateACCFinished').datepicker("destroy");
                $('#orderStartDateFutureBTACC').datepicker("destroy");
                $('#orderEndDateFutureBTACC').datepicker("destroy");
                $('#orderEndDateCurrentBTACC').datepicker("destroy");
                $('#orderEndDateFinishedBTACC').datepicker("destroy")

                $(this).dialog("destroy");
                $(this).remove();
            },
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#ShowBTDataACC", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "ShowBTDataACC-ACC";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    scrollPositionACC = $("div#btsForAccAccountableBTs > div#btsViewExample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/ACC/IndexForAccountableBTs/",
                    type: "GET",
                    data: $('#formForAcountableACC').serialize(),
                    success: function (data) {

                        var elementId = document.getElementById("btsForAccAccountableBTs").getElementsByTagName("th");
                        var scrollPositionObject = $("div#btsForAccAccountableBTs > div#btsViewExample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#btsForAccAccountableBTs");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionACC = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionACC);
                    }
                });
            },
                close: function (event, ui) {
                    $(this).dialog("destroy");
                    $(this).remove();
            }
        });
        return false;
    });
});

$(document).on("click", "#CancelBTbyACC", function (event) {
    event.preventDefault();
    scrollPositionACC = $("div#tableViewBTsACC > div#exampleBtsView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    var element = $(this);
    var dialogId = "CancelReportedBT-dialog";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        var dialogCancelACC = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Cancel BT',
            position: {
                my: "center",
                at: "center",
            },
            open: function () {
                $("[id^=cancelReportedBTAcc]").button();
                $("[id^=cancelReportedBTAcc]").click(function (event) {
                    event.preventDefault();
                    $("#cancelReportedBTACCForm").validate();
                    if ($("#cancelReportedBTACCForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ACC/CancelReportedBT/",
                            data: $("#cancelReportedBTACCForm").serialize(),
                            success: function (data) {
                                if (data.success) {
                                    $(dialogCancelACC).dialog("close");
                                    $(dialogACC).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            },
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;

});

$(document).on("click", "#ShowClosedBTACC", function (event) {
    event.preventDefault();
    //scrollPositionACC = $("div#tableViewBTsACC > div#exampleBtsView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    var element = $(this);
    var dialogId = "ShowClosedBT-dialog";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        var dialogCancelACC = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Cancel BT',
            position: {
                my: "center",
                at: "center",
            },
            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            },
        });
    });
    return false;

})