
$('#buttonSubmitAbsence').button();

$(document).on('keyup','#searchInputABM',(function (event) {
    if (event.keyCode == 13) {
        if ($('#From').val.length > 0 && $('#To').val.length > 0 && $('#To').val >= $('#From').val) {
            $('#formWTR').submit();
        }
    }
})
);

$(document).on('click', '#buttonSubmit', (function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#From').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#To').val());

    if (to >= from) {
        $('#errorFrom').text('');
        $('#From').css('border', '1px solid rgb(226,226,226)');
        $('#errorTo').text('');
        $('#To').css('border', '1px solid rgb(226, 226, 226)');

        $('#formWTR').submit();
    }
    else {
        if (from > to) {
            $('#errorFrom').text('');
            $('#errorFrom').append('The From Date must be less than To date');
        }
    }
})
);

$(document).on('click', '#buttonSubmitAbsence', (function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#FromAbsence').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#ToAbsence').val());
    if ($('#FromAbsence').val().length > 0 && $('#ToAbsence').val().length > 0 && to >= from) {

        $('#errorFromAbsence').text('');
        $('#FromAbsence').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbsence').text('');
        $('#ToAbsence').css('border', '1px solid rgb(226, 226, 226)');

        $('#formAbsence').submit();
    }
    else {
        if ($('#FromAbsence').val().length == 0) {
            $('#errorFromAbsence').text('');
            $('#errorFromAbsence').append('The From field is required');
            $('#FromAbsence').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#ToAbsence').val().length == 0) {
            $('#errorToAbsence').text('');
            $('#errorToAbsence').append('The To field is required');
            $('#ToAbsence').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbsence').text('');
            $('#errorToAbsence').text('');
            $('#errorToAbsence').append('The From Date must be less than To date');
        }
    }
})
);


/*ABM(Absence)*/

$(document).on('keyup', '#absenceSearchInput',(function (event) {
    if (event.keyCode == 13) {
        if ($('#fromDate').val.length > 0 && $('#toDate').val.length > 0 && $('#toDate').val >= $('#fromDate').val) {
            $('#formAbsence').submit();
        }
    }
})
);

$(document).on('click', '#absenceButton',(function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#fromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#toDate').val());

    if (to >= from) {
        $('#errorMessageAbs').text('');
        $('#fromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#toDate').css('border', '1px solid rgb(226, 226, 226)');

        $('#formAbsence').submit();
    }
    else {
        if (from > to) {
            $('#errorMessageAbs').text('');
            $('#errorMessageAbs').append('The From Date must be less than To date');
        }
    }
})
);

$(document).on('click', '#calendarAbsenceSubmitButton', (function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());

    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');
    }

    if (from > to) {
        $('#errorFromAbs').text('');
        $('#errorToAbs').text('');
        $('#errorFromAbs').append('The From Date must be less than To date');
    }
    else {
        $.ajax({
            cache: false,
            url: "/Calendar/GetCalendarData",
            type: "GET",
            data: { calendarFromDate: $('#calendarFromDate').val(), calendarToDate: $("#calendarToDate").val(), selectedDepartment: $('#depDropList :selected').val() },
            success: function (data) {
                $("#CalendarData").html(data);
            }
        })
    }
})
);

$(document).on("change", '#depDropList', (function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());

    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');
    }

    if (from > to) {
        $('#errorFromAbs').text('');
        $('#errorToAbs').text('');
        $('#errorFromAbs').append('The From Date must be less than To date');
    }
    else {
        $.ajax({
            cache: false,
            url: "/Calendar/GetCalendarData",
            type: "GET",
            data: { calendarFromDate: $('#calendarFromDate').val(), calendarToDate: $("#calendarToDate").val(), selectedDepartment: $('#depDropList :selected').val() },
            success: function (data) {
                $("#CalendarData").html(data);
            }
        })
    }
})
);


$(document).on('click', '#pdfPrintBtn', (function (event) {
    event.preventDefault();
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());

    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');
    }

    if (from > to) {
        $('#errorFromAbs').text('');
        $('#errorToAbs').text('');
        $('#errorFromAbs').append('The From Date must be less than To date');
    }
    else {
        $('#printCalendarToPdf').submit();
    }
})
);