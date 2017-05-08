var userScrollPosition;

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on("click", "#CreateUser", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create User";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    $(dialogDiv).load(this.href, function () {
        var dialogEmpCreate = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create User",
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
                    url: "/PU/GetUsersOnlyData/",
                    type: "GET",
                    data: { selectedDepartment: $('#userDepDropList  :selected').val(), searchString: $("#seachInput").val() },
                    success: function (data) {

                        var elementId = document.getElementById("UsersData").getElementsByTagName("th");
                        var scrollPositionObject = $("div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#UsersData");
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
                $(this).dialog("destroy");
                $(this).remove();
            },
        });
        $("#btnSaveOnCreateEmployee").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});
$(document).on("click", ".UserEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Edit User";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    userScrollPosition = $("#UsersData div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(this.href, function () {
        dialogEmpEdit = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit User",
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

                $("#btnSaveEmployee").click(function (event) {
                    event.preventDefault();
                    $("#editEmployeeForm").validate();
                    if ($("#editEmployeeForm").valid()) {
                        editUser = $('#editEmployeeForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/PU/EmployeeEdit",
                            data: editUser,
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
                    type: 'GET',
                    url: '/PU/GetUsersOnlyData/',
                    data: { selectedDepartment: $('#userDepDropList :selected').val(), searchString: $('#userSearchInput').val() },
                    success: function (data) {
                        var elementId = document.getElementById("UsersData").getElementsByTagName("th");
                        var scrollPositionObject = $("#UsersData div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#UsersData");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);
                        userScrollPosition = scrollPositionBTBefore;
                        setTimeout(function () { $("#UsersData div.dataTables_scrollBody").scrollTop(userScrollPosition); }, 100);
                    }
                })
            },

            close: function (event, ui) {
                $('#editDateEmployed').datepicker("destroy");
                $("#editDateDismissed").datepicker("destroy");
                $('#editDateBirthDay').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $("#btnSaveEmployee, #btnDeleteEmployee, #btnResetPassword").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});