var holCreate;
var holEdit;
var dataFormat
function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

$(document).on('click', '#CreateHoliday', function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = 'Create Holiday';
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
        dataFormat = $(this).attr('data-date-format');

    holCreate = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            rasizable: false,
            title: 'Create Holiday',
            position: {
                my: 'center',
                at: 'center'
            },

            open: function (event, ui) {
                $('#HolidayDate').datepicker({
                    firstDay: 1,
                    dateFormat: dataFormat,
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true,
                })

                var DropDownValueOnMainView = $('#selectedCountryID').val();
                $('#dropDownCountry').val(DropDownValueOnMainView);
            },

            buttons: {//buttonSaveHoliday id
                'Save': function () {
                    $('#createHolidayForm').submit();
                }
            },
            close: function (event, ui) {
                $('#HolidayDate').datepicker('destroy');
                $(this).dialog('destroy');
                $(this).remove();
            }
        })
    });
})

$(document).on("click", ".holidayEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Holiday"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
        dataFormat = $(this).attr('data-date-format');

    holEdit = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Holiday",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {

                $('#HolidayDate').datepicker({
                    firstDay: 1,
                    dateFormat: dataFormat,
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true,
                })
            },

            close: function (event, ui) {
                $('#HolidayDate').datepicker('destroy');
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveHoliday, #btnDeleteHoliday").button();
    });
    return false;
});



$(document).on("click", "#btnDeleteHoliday", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');

    $("#deleteHoliday-Confirm").load(url, function () {

        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: title,
            position: {
                my: "center",
                at: "center"
            },

            buttons: {
                "OK": function () {
                    $('#deleteConfirmedForm').submit();

                    $(this).dialog("destroy");
                    $(this).remove();

                    $(holEdit).dialog("close");
                    $(holEdit).remove();
                },
                "Cancel": function () {
                    $(this).dialog("destroy");
                    $(this).remove();

                    $(holEdit).dialog("close");
                    $(holEdit).remove();
                }
            }
        });
    });
    return false;
});


$(document).on("click", "#btnCancelDeleteHoliday", function (event) {
    $("#deleteHoliday-Confirm").dialog("close");
});

function HolidayCreateOnSuccess(data) {
    if (data.search("HolidayData") != -1) {
        $('#HolidayData').replaceWith($(data))

        $(holCreate).dialog("close");
        $(holCreate).remove();
    }
    else {
        $('#HolidayDate').datepicker('destroy');
        $(holCreate).html(data);
        $('#HolidayDate').datepicker({
            firstDay: 1,
            dateFormat: dataFormat,
            showWeek: true,
            calculateWeek: myWeekCalc,
            showOn: 'button',
            buttonImage: '/Content/themes/base/images/calendar2.gif',
            buttonImageOnly: true,
        })
    }
}

function HolidayEditOnSuccess(data) {
    if (data.error) {
        $("#ModelError").html(data.error);
    }
    else if (data.search("HolidayData") != -1) {
        $('#HolidayData').replaceWith($(data))

        $(holEdit).dialog("close");
        $(holEdit).remove();
    }
    else {
        $('#HolidayDate').datepicker('destroy');
        $(holEdit).find("table").html($(data).find("table"));
        $('#HolidayDate').datepicker({
            firstDay: 1,
            dateFormat: dataFormat,
            showWeek: true,
            calculateWeek: myWeekCalc,
            showOn: 'button',
            buttonImage: '/Content/themes/base/images/calendar2.gif',
            buttonImageOnly: true,
        })
    }
}