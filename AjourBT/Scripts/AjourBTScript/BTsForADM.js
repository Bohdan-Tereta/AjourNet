function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

function RefreshTableAfterSubmit(data) {

    var elementId = document.getElementById("ADMtableBTs").getElementsByTagName("th");
    var scrollPositionObject = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    var elementToReplace = $("#tbodyBTADM");
    var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

    scrollPositionADM = scrollPositionBTBefore;
    $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
}

var scrollPositionADM;

$(document).on("click", "#PlanForAdm", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Plan-BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();

    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Plan",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $.ajax({
                    type: "POST",
                    url: "/ADM/CalculateVisaDays",
                    data: $("#planBTForm").serialize(),
                    success: function (data) {
                        if (data.error) {
                            $("#ModelError").html(data.error);
                        }
                        {
                            $("#numberOfDays").text(data);
                        }
                    }, error: function (data) {

                        alert("Server is not responding");
                    }
                })

                if ($('#planStartDateBTs').length > 0) {
                    $('#planStartDateBTs').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#planEndDateBTs').length > 0) {
                    $('#planEndDateBTs').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
              
                $("#btnPlanBT").button();
                $("#btnPlanBT").click(function (event) {
                    event.preventDefault();
                    $("#planBTForm").validate();
                    if ($("#planBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ADM/Plan",
                            data: $("#planBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $("#Plan-BT").dialog("close");
                                }
                            },

                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                })

                $("#btnCalculateDays").button();
                $("#btnCalculateDays").click(function (event) {
                    event.preventDefault();
                    $("#planBTForm").validate();
                    if ($("#planBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ADM/CalculateVisaDays",
                            data: $("#planBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                {
                                    $("#numberOfDays").change($("#numberOfDays").text(data));
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
                    url: "/ADM/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {

                        var elementId = document.getElementById("ADMtableBTs").getElementsByTagName("th");
                        var scrollPositionObject = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#ADMtableBTs");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
            close: function (event, ui) {
                $('#planStartDateBTs').datepicker("destroy");
                $('#planEndDateBTs').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#EditPlannedBT", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "EditPlanedBT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Planned BT",
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                $.ajax({
                    type: "POST",
                    url: "/ADM/CalculateVisaDays",
                    data: $("#editPlanedBTForm").serialize(),
                    success: function (data) {
                        if (data.error) {
                            $("#ModelError").html(data.error);
                        }
                        {
                            $("#numberOfDays").text(data);
                        }
                    }, error: function (data) {
                        alert("Server is not responding");
                    }
                })

                if ($('#editPlannedBTADMStartDate').length > 0) {
                    $('#editPlannedBTADMStartDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editPlannedBTADMEndDate').length > 0) {
                    $('#editPlannedBTADMEndDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                $("#planEditedBT, #btnDeletePlannedBT, #btnCalculateDaysEdit").button();
                $("#btnCalculateDaysEdit").button();
                $("#btnCalculateDaysEdit").click(function (event) {
                    event.preventDefault();
                    $("#editPlanedBTForm").validate();
                    if ($("#editPlanedBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ADM/CalculateVisaDays",
                            data: $("#editPlanedBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                {
                                    $("#numberOfDays").change($("#numberOfDays").text(data));
                                }
                            },

                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                })
            
                $("#planEditedBT").click(function (event) {
                    event.preventDefault();
                    $("#editPlanedBTForm").validate();
                    if ($("#editPlanedBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/ADM/Plan",
                            data: $("#editPlanedBTForm").serialize(),
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
                    url: "/ADM/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {

                        var elementId = document.getElementById("ADMtableBTs").getElementsByTagName("th");
                        var scrollPositionObject = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#ADMtableBTs");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
            close: function (event, ui) {
                $('#editPlannedBTADMEndDate').datepicker("destroy");
                $('#editPlannedBTADMStartDate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#ShowBTDataADM", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "ShowBTData-ADM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
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
                    url: "/ADM/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {

                        var elementId = document.getElementById("ADMtableBTs").getElementsByTagName("th");
                        var scrollPositionObject = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#ADMtableBTs");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
        });
        return false;
    });
});

$(document).on("click", "#btnDeletePlannedBT", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    $("#DeletePlannedBT-ADM").load(url, function () {
        var deleteDialog = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete Planned BT",
            position: {
                my: "center",
                at: "center"
            }
        });
        $(this).dialog('open');
        $("#deleteBTADMConfirmation").button();
        $("#deleteBTADMConfirmation").click(function (event) {
            event.preventDefault();
            $.ajax({
                type: "POST",
                url: "/ADM/DeletePlannedBT",
                data: $("#DeletePlannedBTForm").serialize(),
                success: function (data) {
                    $(deleteDialog).dialog("close");
                    $("#EditPlanedBT").dialog("close");
                },
                error: function (data) {
                    alert("Server is not responding");
                }
            })
        })
    });

    return false;
});

$(document).on("click", "#registerPlanedBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedPlannedBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/ADM/RegisterPlannedBTs",
        traditional: true,
        data: AddAntiForgeryToken({ selectedPlannedBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() }),

        success: function (data) {

            RefreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })
});

$(document).on("click", "#confirmPlanedBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedPlannedBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/ADM/ConfirmPlannedBTs",
        traditional: true,
        data: AddAntiForgeryToken({ selectedPlannedBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() }),
        success: function (data) {

            RefreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })
});

$(document).on("click", "#confirmRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })

    $.ajax({
        type: "POST",
        url: "/ADM/ConfirmRegisteredBTs",
        traditional: true,
        data: AddAntiForgeryToken({ selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() }),
        success: function (data) {

            RefreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })
});

$(document).on("click", "#replanRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/ADM/ReplanRegisteredBTs",
        traditional: true,
        data: AddAntiForgeryToken({ selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() }),
        success: function (data) {

            RefreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })
});

$(document).on("click", "#cancelRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/ADM/CancelRegisteredBTs",
        traditional: true,
        data: AddAntiForgeryToken({ selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() }),
        success: function (data) {

            RefreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })
});

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};