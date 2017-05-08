function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPositionPT;
$(document).on("click", "#AddPrivateTrip", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "AddPrivateTrip-dialog";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    scrollPositionPT = $("div#privateTripsBTMexample_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create Private Trip",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#createStartDatePTBTM').length > 0) {
                    $('#createStartDatePTBTM').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#createEndDatePTBTM').length > 0) {
                    $('#createEndDatePTBTM').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                $("#btnSavePrivateTrip").button();
                $("#btnSavePrivateTrip").click(function (event) {
                    event.preventDefault();
                    $("#CreatePrivateTripForm").validate();
                    if ($("#CreatePrivateTripForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/BTM/PrivateTripCreate",
                            data: $("#CreatePrivateTripForm").serialize(),
                            success: function (data) {
                                $("#" + dialogId).dialog("close");
                            },
                            error: function () {
                                alert("Server is not responding");
                            }

                        })
                    }
                })

            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BTM/GetPrivateTripDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInputVU").val() },
                    success: function (data) {

                        var elementId = document.getElementById("privateTripsTableForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("div#privateTripsBTMexample_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#privateTripsTableForBTM");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionPT = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionPT);
                    }
                });
            },

            close: function (event, ui) {
                $('#createStartDatePTBTM').datepicker("destroy");
                $('#createEndDatePTBTM').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#editPrivateTrip", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "EditPrivateTrip-dialog";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = $(element).attr('href');
    scrollPositionPT = $("div#privateTripsBTMexample_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Private Trip",
            position: {
                my: "center",
                at: "center"

            },
            open: function (event, ui) {
                if ($('#editStartDatePT').length > 0) {
                    $('#editStartDatePT').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editEndDatePT').length > 0) {
                    $('#editEndDatePT').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                $("#btnSavePrivateTrip, #btnDeletePrivateTrip").button();
                $("#btnSavePrivateTrip").click(function (event) {
                    event.preventDefault();
                    $("#EditPrivateTripForm").validate();
                    if ($("#EditPrivateTripForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/BTM/PrivateTripEdit",
                            data: $("#EditPrivateTripForm").serialize(),
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
                    url: "/BTM/GetPrivateTripDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInputVU").val() },
                    success: function (data) {

                        var elementId = document.getElementById("privateTripsTableForBTM").getElementsByTagName("th");
                        var scrollPositionObject = $("div#privateTripsBTMexample_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#privateTripsTableForBTM");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionPT = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionPT);
                    }
                });
            },
            close: function (event, ui) {
                $('#editStartDatePT').datepicker("destroy");
                $('#editEndDatePT').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeletePrivateTrip", function (event) {
    event.preventDefault();
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteEmployee-Confirm").load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete Private Trip",
            position: {
                my: "center",
                at: "center"
            }
        });
        $("#btnCancelDeletePrivateTrip, #btnOKDeletePrivateTrip").button();
        $("#btnOKDeletePrivateTrip").click(function (event) {
            event.preventDefault();
                 $.ajax({
                    type: "POST",
                    url: "/BTM/PrivateTripDelete",
                    data: $("#DeletePrivateTripForm").serialize(),
                    success: function (data) {
                        $("#deleteEmployee-Confirm").dialog("close");
                        $("#EditPrivateTrip-dialog").dialog("close");
                    },
                    error: function (data) {
                        alert("Server is not responding");
                    }
                })

            

        })
    });

    return false;
});

$(document).on("click", "#btnCancelDeletePrivateTrip", function (event) {
    $("#deleteEmployee-Confirm").dialog("close");
});