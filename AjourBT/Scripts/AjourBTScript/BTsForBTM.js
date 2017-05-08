/// <reference path="../../Views/BusinessTrip/GetBusinessTripDataDIR.cshtml" />
function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPositionBTM;
var win;
var arrangeBTdialog;

$(document).on("click", "#ReportBT", function (event) {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: "/BTM/ReportConfirmedBTs",
            data: $("#ReportConfirmedBTsForm").serialize(),
            success: function (data) {

                var elementId = document.getElementById("tableBTsForBTM").getElementsByTagName("th");
                var scrollPositionObject = $("#ReportConfirmedBTsForm > div#viewBTsBTMexample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                var elementToReplace = $("#tbodyBts");
                var scrollPositionBTMBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                scrollPositionBTM = scrollPositionBTMBefore;
                $("div.dataTables_scrollBody").scrollTop(window.scrollPositionBTM);
            },
            error: function (data) {
                alert("Server is not responding");
            }
        })
    })


$(document).on("click", "#Habitation, #AddHabitation, #AddFlights, #ConfirmHabitation, #ConfirmFlights, #AddInvitation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Habitation-create";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    var updateID = element.attr('data-updateid');
    scrollPositionBTM = $("#ReportConfirmedBTsForm > div#viewBTsBTMexample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        arrangeBTdialog = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Arrange BT",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#OrderStartDate').length > 0) {
                    $('#OrderStartDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#OrderEndDate').length > 0) {
                    $('#OrderEndDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                $("#btnSave,#DeleteBTBTM,#RejectBTBTM").button();
                $("#btnSave").click(function (event) {
                    event.preventDefault();
                    $("#ArrangeBTForm").validate();
                    if ($("#ArrangeBTForm").valid()) {

                        $.ajax({
                            type: "POST",
                            url: "/BTM/SaveArrangedBT",
                            traditional: true,
                            data: $("#ArrangeBTForm").serialize(),
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
                    url: "/BTM/GetBusinessTripDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInputBT").val() },
                    success: function (data) {

                        var elementId = document.getElementById("tableBTsForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("#ReportConfirmedBTsForm > div#viewBTsBTMexample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#tableBTsForBTM");
                        var scrollPositionBTMBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionBTM = scrollPositionBTMBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionBTM);
                    }
                });
            },
            close: function (event, ui) {
                $('#OrderStartDate').datepicker("destroy");
                $('#OrderEndDate').datepicker("destroy");
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

$(document).on("click", "#ShowBTData", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "ShowBTData-BTM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionBTM = $("#ReportConfirmedBTsForm > div#viewBTsBTMexample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
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
                    url: "/BTM/GetBusinessTripDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInputBT").val() },
                    success: function (data) {

                        var elementId = document.getElementById("tableBTsForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("#ReportConfirmedBTsForm > div#viewBTsBTMexample_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#tableBTsForBTM");
                        var scrollPositionBTMBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionBTM = scrollPositionBTMBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionBTM);
                    }
                });
            },
        });
        return false;
    });
});

$(document).on("click", "#DeleteBTBTM", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var updateID = $(this).attr('data-updateID');
    var dialogId = "DeleteBTBTM-BTM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";

    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {

        var dialogDel = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete  BT",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $("#DeleteBTButton").button();
                $("#DeleteBTButton").click(function (event) {
                    event.preventDefault();
                    $.ajax({
                        type: "POST",
                        data: $("#DeleteBTBTMForm").serialize(),
                        url: "/BTM/DeleteBTBTM",
                        success: function (data) {
                            $(dialogDel).dialog("close");
                            $("#Habitation-create").dialog("close");
                        },
                        error: function (data) {
                            alert("Server is not responding");
                        }
                    })
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

$(document).on("click", "#RejectBTBTM", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "RejectBT-BTM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var updateID = $(this).attr('data-updateID')
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Reject BT",
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                $("#RejectBTButton").button();
                $("#RejectBTButton").click(function (event) {
                    event.preventDefault();
                    $("#RejectFormForBTM").validate();
                    if ($("#RejectFormForBTM").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/BTM/Reject_BT_BTM",
                            data: $("#RejectFormForBTM").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#RejectByBTMModelError").html(data.error);
                                }
                                else {
                                    $("#" + dialogId).dialog("close");
                                    $("#Habitation-create").dialog("close");
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
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("change", "#OrderStartDate, #OrderEndDate", function (event) {
    Calculate();
});

function Calculate() {

    var StartDate = document.getElementById('OrderStartDate').value;
    var EndDate = document.getElementById('OrderEndDate').value;

    var parts1 = StartDate.split('.');
    var parts2 = EndDate.split('.');
    // this will split your string into date parts, eg. 11/30/2012 would result as an array ['11','30','2012'];

    var date1 = new Date(parts1[2], parts1[1] - 1, parts1[0]);
    var date2 = new Date(parts2[2], parts2[1] - 1, parts2[0]);


    var msecPerMinute = 1000 * 60;
    var msecPerHour = msecPerMinute * 60;
    var msecPerDay = msecPerHour * 24;

    var StartDateMsec = date1.getTime();
    var EndDateMsec = date2.getTime();

    var interval = EndDateMsec - StartDateMsec;

    var days = Math.round(interval / msecPerDay) + 1;
    document.getElementById('DaysInBTForOrder').value = days;
}

$(document).on("click", "#ShowBTDataBTM", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "ShowBTData-BTM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionBTM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
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
                    url: "/BTM/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $('#seachInput').val() },
                    success: function (data) {

                        var elementId = document.getElementById("visaTableForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("div#visasView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#visaTableForBTM");
                        var scrollPositionBTMBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionBTM = scrollPositionBTMBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionBTM);
                    }
                });
            },
        });
        return false;
    });
});