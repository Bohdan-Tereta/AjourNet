function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPosition;
var dialogEmpEdit;

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
$(document).on("click", "#CreateEmployee", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Employee";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    $(dialogDiv).load(this.href, function () {
        var dialogEmpCreate = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create Employee",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#createDateEmployed').length > 0) {
                    $('#createDateEmployed').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });

                }
                if ($('#createDateDismissed').length > 0) {
                    $('#createDateDismissed').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        showButtonPanel: true,
                        closeText: 'Clear', // Text to show for "close" button
                        beforeShow: function (input) {
                            setTimeout(function () {
                                var clearButton = $(input)
                                    .datepicker("widget")
                                    .find(".ui-datepicker-close");
                                clearButton.unbind("click").bind("click", function () { $.datepicker._clearDate(input); });
                            }, 1);
                        }

                    });
                }
                if ($('#createDateBirthDay').length > 0) {
                    $('#createDateBirthDay').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }

                if ($('#createEducationAcquired').length > 0) {
                    $('#createEducationAcquired').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        beforeShow: function () {
                            this.value = '';
                        }

                    });
                }

                if ($('#createEducationInProgress').length > 0) {
                    $('#createEducationInProgress').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        beforeShow: function () {
                            this.value = '';
                        }


                    });
                }

                $("#btnSaveOnCreateEmployee").click(function (event) {
                    event.preventDefault();
                    $("#createEmployeeForm").validate();
                    if ($("#createEmployeeForm").valid()) {
                        createEmp = $('#createEmployeeForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/PU/EmployeeCreate",
                            data: createEmp,
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogEmpCreate).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert('Server not responding');
                            }
                        });
                    }
                });
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/PU/GetEmployeeData/",
                    type: "GET",
                    data: { selectedDepartment: $('#depDropList :selected').val(), searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("EmployeeData").getElementsByTagName("th");
                        var scrollPositionObject = $("div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#EmployeeData");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                })
            },

            close: function (event, ui) {
                $('#createDateEmployed').datepicker("destroy");
                $('#createDateDismissed').datepicker("destroy");
                $('#createDateBirthDay').datepicker("destroy");
                $('#createEducationAcquired').datepicker("destroy");
                $('#createEducationInProgress').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            },
        });
        $("#btnSaveOnCreateEmployee").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", ".empEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Edit Employee";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPosition = $("div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(this.href, function () {
        dialogEmpEdit = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Employee",
            position: {
                my: "center center",
                at: "center center"
            },
            open: function (event, ui) {
                if ($('#editDateEmployed').length > 0) {
                    $('#editDateEmployed').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editDateDismissed').length > 0) {
                    $("#editDateDismissed").datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        showButtonPanel: true,
                        closeText: 'Clear', // Text to show for "close" button
                        beforeShow: function (input) {
                            setTimeout(function () {
                                var clearButton = $(input)
                                    .datepicker("widget")
                                    .find(".ui-datepicker-close");
                                clearButton.unbind("click").bind("click", function () { $.datepicker._clearDate(input); });
                            }, 1);
                        }
                    });
                }

                if ($('#editDateBirthDay').length > 0) {
                    $('#editDateBirthDay').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }

                if ($('#editEducationAcquired').length > 0) {
                    $('#editEducationAcquired').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        beforeShow: function () {
                            this.value = '';
                        }

                    });
                }

                if ($('#editEducationInProgress').length > 0) {
                    $('#editEducationInProgress').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true,
                        beforeShow: function () {
                            this.value = '';
                        }
                    });
                }

                $("#btnSaveEmployee").click(function (event) {
                    event.preventDefault();
                    $("#editEmployeeForm").validate();
                    if ($("#editEmployeeForm").valid()) {
                        editEmp = $('#editEmployeeForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/PU/EmployeeEdit",
                            data: editEmp,
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogEmpEdit).dialog("close");
                                }
                            },

                            error: function (data) {
                                alert('Server not responding');
                            }
                        });
                    }
                });

            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/PU/GetEmployeeData/",
                    type: "GET",
                    data: { selectedDepartment: $('#depDropList :selected').val(), searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("EmployeeData").getElementsByTagName("th");
                        var scrollPositionObject = $("div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#EmployeeData");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                })
            },

            close: function (event, ui) {
                $('#editDateEmployed').datepicker("destroy");
                $("#editDateDismissed").datepicker("destroy");
                $('#editDateBirthDay').datepicker("destroy");
                $('#editEducationAcquired').datepicker("destroy");
                $('#editEducationInProgress').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $("#btnSaveEmployee, #btnDeleteEmployee, #btnResetPassword").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnResetPassword", function (event) {
    event.preventDefault();
    editEmployeeForm = $('#editEmployeeForm').serialize();
    $.ajax({
        url: "/PU/ResetPassword/",
        data: editEmployeeForm,
        method: "get",
        success: function (data) {
            var rcdialog = $("#resetPassword-Confirm").html(data).dialog({
                title: "Reset Password",
                buttons: {
                    "Ok": function (event) {
                        $.ajax({
                            url: "/PU/ResetPasswordConfirmed/",
                            type: "post",
                            data: editEmployeeForm,
                            success: function (event) {
                                $(rcdialog).dialog("destroy");
                                var dialog = $("<div />", { text: event }).dialog(
                                {
                                    title: "Reset password",
                                    buttons:
                                    {
                                        "OK": function () {
                                            $(dialog).dialog("destroy");
                                        }
                                    },
                                    close: function () {
                                        $(dialog).dialog("destroy");
                                    }
                                });
                            }
                        })
                    },
                    "Cancel": function () {
                        $(rcdialog).dialog("destroy");
                    }
                },
                close: function () {
                    $(rcdialog).dialog("destroy");
                }
            })

        }
    })
    return false;
});

$(document).on("click", "#btnDeleteEmployee", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteEmployee-Confirm").load(url, function () {
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
        })

        $("#OKDeleteEmployee").button();
        $("#OKDeleteEmployee").click(function (event) {
            scrollPosition = $("div.dataTables_scrollBody").scrollTop();
            event.preventDefault();
            $("#DeleteEmployeeForm").validate();
            if ($("#DeleteEmployeeForm").valid()) {
                visa = $('#DeleteEmployeeForm').serialize();
                $.ajax({
                    type: "POST",
                    url: "/PU/EmployeeDelete",
                    data: visa,

                    success: function (data) {
                        $("#deleteEmployee-Confirm").dialog("close");
                        $(dialogEmpEdit).dialog("close");
                    },

                    error: function (data) {
                        alert('Server not responding');
                    }
                });
            }
        })
    });
    return false;
});

$(document).on("click", "#btnCancelDeleteEmployee", function (event) {
    $("#deleteEmployee-Confirm").dialog("close");
});

$(document).on("change", "#HasPassportEmp", function (event) {
    event.preventDefault();
    var eid = $(this).attr('value');
    var ich = $(this).attr('checked');
    $.ajax({
        url: "/BTM/ModifyPassport/",
        type: "post",
        data: AddAntiForgeryToken({ id: eid, isChecked: ich }),
        success: function (data) {
            $('#response').html(data);
        }
    });
    return false;
});

$(document).on("click", ".empDetailsDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Edit Employee";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPosition = $("div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(this.href, function () {
        dialogEmpEdit = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Employee Details",
            position: {
                my: "center center",
                at: "center center"
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
    });
    return false;
});

$(document).on("click", "#btnGetMailAliasEMailsADM", function (event) {
    event.preventDefault();
    var path = "/ADM/GetMailAliasEMails";
    var sendPath = '/ADM/GetMailToLinkWithBcc';
    MailingList(path, sendPath);
});

$(document).on("click", "#btnGetSecondMailAliasEMailsADM", function (event) {
    event.preventDefault();
    var path = "/ADM/GetSecondMailAliasEMails";
    var sendPath = '/ADM/GetSecondMailToLinkWithBcc';
    MailingList(path, sendPath);
});

$(document).on("click", "#btnGetMailAliasEMailsPU", function (event) {
    event.preventDefault();
    var path = "/PU/GetMailAliasEMails";
    var sendPath = '/PU/GetMailToLinkWithBcc';
    MailingList(path, sendPath);
});

$(document).on("click", "#btnGetSecondMailAliasEMailsPU", function (event) {
    event.preventDefault();
    var path = "/PU/GetSecondMailAliasEMails";
    var sendPath = '/PU/GetSecondMailToLinkWithBcc';
    MailingList(path, sendPath);
});

function MailingList(path, sendPath) {
    createEmp = $('#GetEmployeeDataReadOnlyForm').serialize();
    if (createEmp == "") {
        createEmp = $('#formForPU').serialize();
    }
    var wWidth = $(window).width();
    var maxWidth = wWidth * 0.8;
    var wHeight = $(window).height();
    var maxHeight = wHeight * 0.8;
    var popup = $("#EMailData").load(path, createEmp, function () {
        $(this).dialog({
            autoOpen: true,
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Mailing list",
            position: {
                my: "center",
                at: "center"
            },

            buttons: ({
                "Send": {
                    text: 'Send',
                    id: 'btnSendMailWithBcc',
                    click: function (event) {
                        event.preventDefault();
                        $.ajax({
                            type: 'post',
                            url: sendPath,
                            data: createEmp,

                            success: function (data) {
                                location.href = data;
                            }
                        });
                    }
                }
            })
        });
    });
}