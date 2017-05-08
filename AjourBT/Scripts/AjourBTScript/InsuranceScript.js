function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var dialog;
var scrollPosition;
var InsuranceEditDialog;
var InsuranceDelDialog
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on("click", "#CreateInsurance", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "insuranceDialog-create";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Insurance',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": {
                    text: 'Save',
                    id: "btnSave",
                    click: function (event) {
                        event.preventDefault();
                        $("#CreateInsuranceForm").validate();
                        if ($("#CreateInsuranceForm").valid()) {
                            visa = $('#CreateInsuranceForm').serialize();
                            $.ajax({
                                type: "POST",
                                url: "/BTM/InsuranceCreate",
                                data: visa,

                                success: function (data) {
                                    if (data.error) {
                                        $("#InsuranceModelError").html(data.error);
                                    }
                                    else {
                                        $("#" + dialogId).dialog("close");
                                    }
                                },

                                error: function (data) {
                                    alert('Server not responding');
                                }
                            });
                        }
                    }
                }
            },
            open: function (event, ui) {
                if ($('#StartDateInsuranceCreate').length > 0) {
                    $('#StartDateInsuranceCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#EndDateInsuranceCreate').length > 0) {
                    $('#EndDateInsuranceCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BTM/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("visaTableForBTM").getElementsByTagName("th");;
                        var scrollPositionObject = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#visaTableForBTM");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                });
            },

            close: function (event, ui) {
                $('#StartDateInsuranceCreate').datepicker("destroy");
                $('#EndDateInsuranceCreate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});


$(document).on("click", ".insuranceEditDialog", function (event) {

    event.preventDefault();
    var element = $(this);
    var dialogId = "insuranceDialogEdit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        InsuranceEditDialog = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Edit Insurance',
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                if ($('#editStartDateInsurance').length > 0) {
                    $('#editStartDateInsurance').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editEndDateInsurance').length > 0) {
                    $('#editEndDateInsurance').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }                

                $("#btnSaveInsurance,#btnDeleteInsurance").button();
                $("#btnSaveInsurance").click(function (event) {
                    event.preventDefault();
                    $("#InsuranceEditForm").validate();
                    if ($("#InsuranceEditForm").valid()) {
                        insurance = $('#InsuranceEditForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/BTM/InsuranceEdit",
                            data: AddAntiForgeryToken(insurance),
                            success: function (data) {
                                if (data.error) {
                                    $("#InsuranceModelError").html(data.error);
                                }
                                else {
                                    $(InsuranceEditDialog).dialog("close");
                                }
                            },

                            error: function (data) {
                                alert('Server not responding');
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
                $('#editStartDateInsurance').datepicker("destroy");
                $('#editEndDateInsurance').datepicker("destroy");
                $(InsuranceEditDialog).dialog("destroy");
                $(InsuranceEditDialog).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteInsurance", function (event) {
    event.preventDefault();
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#DeleteInsurance-confirm").load(url, function () {
        InsuranceDelDialog = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete insurance",
            position: {
                my: "center",
                at: "center"
            },
        })
        $("#OKDeleteInsurance,#btnCancelDeleteInsurance").button();
        $("#OKDeleteInsurance").click(function (event) {
            event.preventDefault();
            visa = $('#DeleteInsuranceForm').serialize();
            $.ajax({
                type: "POST",
                url: "/BTM/InsuranceDelete",
                data: visa,

                success: function (data) {
                    $("#insuranceDialogEdit").dialog("close");
                    $(InsuranceDelDialog).dialog("close");
                },

                error: function (data) {
                    alert('Server not responding');
                }
            });
        })

    });
    return false;
});

$(document).on("click", "#btnCancelDeleteInsurance", function (event) {
    $(InsuranceDelDialog).dialog("close");
});