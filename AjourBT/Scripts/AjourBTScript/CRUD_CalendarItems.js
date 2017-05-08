var EditHtmlDialog =
    '<div id="htmlDialog">' +
    '<form id="vacationForm" method="post" action="">' +
    '<table id="vacationTable">' +
    '<tbody>' +
    '<tr id="LastnameEdit" style="display:table-row"><td><label for="Name">Name</td><td><label id="LastNameText"></td></tr>' +
    '<tr><td><label for="From">From:</label></td><td><input id="dateVacationFrom" type="text" name=""></input></td><td><p id="errorMsgFrom"></p></td><tr>' +
    '<tr><td><label for="To">To:</td><td><input id="dateVacationTo" type="text" name=""></input></td><td><p id="errorMsgTo"></p></td></tr>' +
    '<tr id="daysToReclaimRow" style="display:none"><td><label for="From">Days to reclaim</label></td>' +
    '<td><select id="daysToReclaimEvent"></select></td><td><p id="errorMsgReclaim"></p></td></tr>' +
    '</tbody>' +
    '</table>' +
    '<br>' +
     '<div style="float:left">' +
    '<a id="Save" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"' +
    '><span>Save</span></a>' +
    '</div>' +
    '<div style="float:right">' +
    '<a id="Delete" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"' +
    '><span>Delete</span></a>' +
    '</div>' +
    '</form>' +
    '</div>';

var EditHtmlSickDialog =
    '<div id="htmlDialog">' +
    '<form id="vacationForm" method="post" action="">' +
    '<table id="vacationTable">' +
    '<tbody>' +
    '<tr id="LastnameEditSick" style="display:table-row"><td><label for="Name">Name</td><td><label id="LastNameText"></td></tr>' +
    '<tr><td><label for="From">From:</label></td><td><input id="dateVacationFrom" type="text" name=""></input></td><td><p id="errorMsgFrom"></p></td><tr>' +
    '<tr><td><label for="To">To:</td><td><input id="dateVacationTo" type="text" name=""></input></td><td><p id="errorMsgTo"></p></td></tr>' +
    '<tr><td><label for="Sickness">Sickness:</td><td><input id="sicknessText" type="text" name=""></input></td><td><p id="errorMsgSick"></p></td></tr>' +
    '</tbody>' +
    '</table>' +
    '<br>' +
     '<div style="float:left">' +
    '<a id="Save" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"' +
    '><span>Save</span></a>' +
    '</div>' +
    '<div style="float:right">' +
    '<a id="Delete" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"' +
    '><span>Delete</span></a>' +
    '</div>' +
    '</form>' +
    '</div>';

var CreateHtmlDialog =
    '<div id="htmlDialog">' +
    '<form id="vacationFormCreate" method="post" action="">' +
    '<table id="vacationTable">' +
    '<tbody>' +
    '<tr id="hideLastname" style="display:table-row"><td><label for="Name">Name</td><td><label id="LastNameText"></td></tr>' +
    '<tr><td><label for="From">Event Type</label></td>' +
    '<td><select id="event"><option value="Paid Vacation">Paid Vacation</option><option value="Unpaid Vacation">Unpaid Vacation</option>' +
    '<option value="ReclaimedOvertime">ReclaimedOvertime</option><option value="OvertimeForReclaim">OvertimeForReclaim</option><option value="PrivateMinus">PrivateMinus</option><option value="SickAbsence">SickAbsence</option></select></td></tr>' +
    '<tr><td><label for="From">From </label></td><td><input id="dateVacationFrom" type="text" name="" readonly="readonly"></label></td></tr>' +
    '<tr><td><label for="To">To </label></td><td><input id="dateVacationTo" type="text" name=""></input></td><td><p id="errorMsg"></p></td></tr>' +
    '<tr id="hideSick" style="display:none"><td><label for="Sickness">Sickness </td><td><input id="sickText" type="text" name=""></input></td><td><p id="errorMsgSick"></p></td></tr>' +
    '<tr id="hideVacation" style="display:table-row"><td><label for="Workdays">Workdays</td><td><label id="calcWorkdays"></td></tr>' +
    '<tr id="hideOverralDays" style="display:table-row"><td><label for="Overral Days">Calendar Days</td><td><label id="calcOverralDays"></td></tr>' +
    '<tr id="daysToReclaimRow" style="display:none"><td><label for="From">Days to reclaim</label></td>' +
    '<td><select id="daysToReclaimEvent"></select></td><td><p id="errorMsgReclaim"></p></td></tr>' +
    '</tbody>' +
    '</table>' +
    '<br>' +
    '<div style="float:right">' +
    '<a id="CreateEvent" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"' +
    '><span>Create Event</span></a>' +
    '</div>' +
    '</form>' +
    '</div>';

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

function convertDate(dateToFormat) {
    var theDateObj = new Date(parseFloat(dateToFormat));
    var year = theDateObj.getFullYear();
    var month = theDateObj.getMonth() + 1;
    if (month < 10) {
        month = '0' + month;
    }
    var day = theDateObj.getDate();

    if (day < 10) {
        day = '0' + day;
    }

    var result = day + '.' + month + '.' + year;

    return result;
}

function SetErrorStyle() {
    $('#errorMsg').text('Wrong DateTime Value');
    $('#dateVacationTo').css('border', '1px solid rgb(232,12,77)');
}

function SetNoErrorStyle() {
    $('#dateVacationTo').css('border', '1px solid rgb(226,226,226)');
    $('#errorMsg').text('');
}

function SetErrorStyleEdit(resultFrom, resultTo, vacationFrom, vacationTo) {
    SetNoErrorStyleEdit();
    //check emtpy fields
    if (resultFrom == false) {
        $('#errorMsgFrom').text('Wrong DateTime Value');
        $('#dateVacationFrom').css('border', '1px solid rgb(232,12,77)');
    }
    if (resultTo == false) {
        $('#errorMsgTo').text('Wrong DateTime Value');
        $('#dateVacationTo').css('border', '1px solid rgb(232,12,77)');
    }
    //check equality
    if (vacationTo.getTime() < vacationFrom.getTime()) {
        $('#errorMsgTo').text('Wrong DateTime Value');
        $('#dateVacationTo').css('border', '1px solid rgb(232,12,77)');
    }

}

function SetNoErrorStyleEdit() {
    $('#dateVacationFrom').css('border', '1px solid rgb(226,226,226)');
    $('#errorMsgFrom').text('');
    $('#dateVacationTo').css('border', '1px solid rgb(226,226,226)');
    $('#errorMsgTo').text('');
}

function EditEvent(type, from, to, id, sickType, reclaimDate) {

    switch (type) {

        case 'PaidVacation':
        case 'Paid Vacation':
        case 'UnpaidVacation':
        case 'Unpaid Vacation':
            {
                var element = $(this);
                var dialogId = 'Edit ' + type;
                var dialogDiv = "<div id='" + dialogId + "'></div>";
                var url = "";

                $(EditHtmlDialog).dialog({
                    modal: true,
                    height: 'auto',
                    width: 'auto',
                    resizable: false,
                    title: 'Edit ' + type,
                    position: {
                        my: 'center',
                        at: 'center'
                    },

                    open: function (event, ui) {
                        $('#Save').button();
                        $('#Delete').button();
                        $('#dateVacationFrom').val(convertDate(from));
                        $('#dateVacationTo').val(convertDate(to));




                        $('#dateVacationFrom').datepicker({
                            firstDay: 1,
                            dateFormat: 'dd.mm.yy',
                            showWeek: true,
                            calculateWeek: myWeekCalc,
                            showOn: 'button',
                            buttonImage: '/Content/themes/base/images/calendar2.gif',
                            buttonImageOnly: true
                        });

                        $('#dateVacationTo').datepicker({
                            firstDay: 1,
                            dateFormat: 'dd.mm.yy',
                            showWeek: true,
                            calculateWeek: myWeekCalc,
                            showOn: 'button',
                            buttonImage: '/Content/themes/base/images/calendar2.gif',
                            buttonImageOnly: true

                        });


                        var lastNameEdit = $("div[data-id='" + id + "']").find($(".fn-label")).text();
                        $('#LastNameText').text(lastNameEdit);


                        $('#daysToReclaimRow').css("display", "none");

                        //$(document).on('click','#Save',(function () {
                        $("#Save").click(function () {
                            {
                                var vacationFrom = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationFrom').val()));
                                var vacationTo = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationTo').val()));

                                var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                                var regResultFrom = reg.test($('#dateVacationFrom').val());
                                var regResultTo = reg.test($('#dateVacationTo').val());

                                if ((regResultFrom == true && regResultTo == true) && (vacationTo.getTime() >= vacationFrom.getTime())) {

                                    SetNoErrorStyleEdit();

                                    var createEvent = {
                                        id: id,
                                        oldFrom: convertDate(from),
                                        oldTo: convertDate(to),
                                        newFrom: $('#dateVacationFrom').val(),
                                        newTo: $('#dateVacationTo').val(),
                                        type: type,
                                    };

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Vacation/Edit',
                                        data: AddAntiForgeryToken(createEvent),
                                        success: function (data) {
                                            editBarForGantt(createEvent, from, to)
                                        },
                                        error: function (data) { }
                                    })

                                    $('#dateVacationFrom').datepicker('destroy');
                                    $('#dateVacationTo').datepicker('destroy');
                                    $('#htmlDialog').dialog("destroy");
                                    $('#htmlDialog').remove();
                                }

                                else {
                                    SetErrorStyleEdit(regResultFrom, regResultTo, vacationFrom, vacationTo);
                                }
                            }
                        });                        

                        //$(document).on('click','#Delete', function () {
                        $("#Delete").click(function () {
                            {
                                var createEvent = {
                                    id: id,
                                    from: convertDate(from),
                                    to: convertDate(to),
                                    type: type
                                };

                                $.ajax({
                                    type: 'POST',
                                    url: '/Vacation/Delete',
                                    data: AddAntiForgeryToken(createEvent),
                                    success: function (data) {
                                        deleteBarForGantt(from, to, id)
                                    },
                                    error: function (data) { }
                                })
                                $('#daveVacationFrom').datepicker('destroy');
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                    });

                    },
                    close: function (event, ui) {
                        $(this).dialog('destroy');
                        $(this).remove();
                    }
                })
            }
            break;

        case 'ReclaimedOvertime':
        case 'Reclaimed Overtime':
        case 'OvertimeForReclaim':
        case 'PrivateMinus':
        case 'Overtime For Reclaim':
        case 'Private Minus':

            {
                var element = $(this);
                var dialogId = 'Edit ' + type;
                var dialogDiv = "<div id='" + dialogId + "'></div>";
                var url = "";

                $(EditHtmlDialog).dialog({
                    modal: true,
                    height: 'auto',
                    width: 'auto',
                    resizable: false,
                    title: 'Edit ' + type,
                    position: {
                        my: 'center',
                        at: 'center'
                    },

                    open: function (event, ui) {
                        $('#Save').button();
                        $('#Delete').button();
                        $('#dateVacationFrom').val(convertDate(from));
                        $('#dateVacationTo').val(convertDate(to));
                        $('#dateVacationFrom').datepicker({
                            firstDay: 1,
                            dateFormat: 'dd.mm.yy',
                            showWeek: true,
                            calculateWeek: myWeekCalc,
                            showOn: 'button',
                            buttonImage: '/Content/themes/base/images/calendar2.gif',
                            buttonImageOnly: true
                        });

                        $('#dateVacationTo').attr("readonly", "");

                        $('#dateVacationFrom').on('change', function () {

                            $('#dateVacationTo').val($('#dateVacationFrom').val());
                        });

                        var lastNameEditRec = $("div[data-id='" + id + "']").find($(".fn-label")).text();
                        $('#LastNameText').text(lastNameEditRec);
                        if (type == "ReclaimedOvertime") {

                            $('#daysToReclaimRow').css("display", "table-row");
                            $.ajax({
                                type: 'POST',
                                url: '/Journey/GetJourneysAndOvertimesForOneEmpEdit',
                                data: AddAntiForgeryToken( { eID: id, from: convertDate(from) }),
                                success: function (data) {
                                    $('#daysToReclaimEvent').append(data);
                                },
                                error: function (data) {
                                }
                            })
                        }
                        else
                            {
                            $('#daysToReclaimRow').css("display", "none");
                        }

                        //$(document).on('click', '#Save', function () {
                        $("#Save").click(function(){
                            {
                                var vacationFrom = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationFrom').val()));
                                var vacationTo = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationTo').val()));

                                var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                                var regResultFrom = reg.test($('#dateVacationFrom').val());
                                var regResultTo = reg.test($('#dateVacationTo').val());

                                if ((regResultFrom == true && regResultTo == true) && (vacationTo.getTime() >= vacationFrom.getTime())) {

                                    SetNoErrorStyleEdit();

                                    var createEvent = {
                                        id: id,
                                        oldFrom: convertDate(from),
                                        oldTo: convertDate(to),
                                        newFrom: $('#dateVacationFrom').val(),
                                        newTo: $('#dateVacationTo').val(),
                                        from: $('#dateVacationFrom').val(),
                                        to: $('#dateVacationTo').val(),
                                        type: type,
                                        reclaimDate: $("#daysToReclaimEvent :selected").text()
                                    };

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Overtime/Edit',
                                        data: AddAntiForgeryToken(createEvent),
                                        success: function (data) {
                                            editBarForGantt(createEvent, from, to)
                                        },
                                        error: function (data) { }
                                    })
                                    $('#dateVacationFrom').datepicker('destroy');
                                    $('#htmlDialog').dialog("destroy");
                                    $('#htmlDialog').remove();
                                }
                                else {
                                    SetErrorStyleEdit(regResultFrom, regResultTo, vacationFrom, vacationTo);
                                }
                            }
                        });

                        //$(document).on('click','#Delete', function () {
                        $("#Delete").click(function(){
                            {
                                var createEvent = {
                                    id: id,
                                    from: convertDate(from),
                                    to: convertDate(to),
                                    type: type
                                };

                                var fromReclaimed;
                                var toReclaimed;
                                var idReclaimed;
                                $.ajax({
                                    type: 'POST',
                                    url: '/Overtime/GetReclaimedOvertime',
                                    data: AddAntiForgeryToken({ id: id, from: convertDate(from), to: convertDate(to) }),
                                    dataType: 'json', 
                                    success: function (data) {
                                        if (typeof data == 'object') {
                                            deleteBarForGantt(data.from.slice(6, -2), data.to.slice(6, -2), data.id);
                                            $.ajax({
                                                type: 'POST',
                                                url: '/Overtime/Delete',
                                                data: AddAntiForgeryToken(createEvent),
                                                success: function (data) {
                                                    deleteBarForGantt(from, to, id)
                                                },
                                                error: function (data) { }
                                            })
                                        }
                                    },
                                    error: function (data) {
                                        $.ajax({
                                            type: 'POST',
                                            url: '/Overtime/Delete',
                                            data: AddAntiForgeryToken(createEvent),
                                            success: function (data) {
                                                deleteBarForGantt(from, to, id)
                                            },
                                            error: function (data) { }
                                        })
                                    }
                                })

                                $('#daveVacationFrom').datepicker('destroy');
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                        });
                    },
                    close: function (event, ui) {
                        $(this).dialog('destroy');
                        $(this).remove();
                    }
                })
            }
            break;

        case 'SickAbsence':
        case 'Sick Absence':
            {
                var element = $(this);
                var dialogId = 'Edit ' + type;
                var dialogDiv = "<div id='" + dialogId + "'></div>";
                var url = "";

                $(EditHtmlSickDialog).dialog({
                    modal: true,
                    height: 'auto',
                    width: 'auto',
                    resizable: false,
                    title: 'Edit ' + type,
                    position: {
                        my: 'center',
                        at: 'center'
                    },

                    open: function (event, ui) {
                        $('#Save').button();
                        $('#Delete').button();
                        $('#dateVacationFrom').val(convertDate(from));
                        $('#dateVacationTo').val(convertDate(to));
                        $('#sicknessText').val(sickType);
                        $('#dateVacationFrom').datepicker({
                            firstDay: 1,
                            dateFormat: 'dd.mm.yy',
                            showWeek: true,
                            calculateWeek: myWeekCalc,
                            showOn: 'button',
                            buttonImage: '/Content/themes/base/images/calendar2.gif',
                            buttonImageOnly: true
                        });

                        $('#dateVacationTo').datepicker({
                            firstDay: 1,
                            dateFormat: 'dd.mm.yy',
                            showWeek: true,
                            calculateWeek: myWeekCalc,
                            showOn: 'button',
                            buttonImage: '/Content/themes/base/images/calendar2.gif',
                            buttonImageOnly: true

                        });

                        var lastNameEditSick = $("div[data-id='" + id + "']").find($(".fn-label")).text();
                        $('#LastNameText').text(lastNameEditSick);

                        //$(document).on('click','#Save', function () {
                        $("#Save").click(function(){
                            {
                                var vacationFrom = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationFrom').val()));
                                var vacationTo = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationTo').val()));

                                var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                                var regResultFrom = reg.test($('#dateVacationFrom').val());
                                var regResultTo = reg.test($('#dateVacationTo').val());

                                if ((regResultFrom == true && regResultTo == true) && (vacationTo.getTime() >= vacationFrom.getTime())) {

                                    SetNoErrorStyleEdit();

                                    var createEvent = {
                                        id: id,
                                        oldFrom: convertDate(from),
                                        oldTo: convertDate(to),
                                        newFrom: $('#dateVacationFrom').val(),
                                        newTo: $('#dateVacationTo').val(),
                                        from: $('#dateVacationFrom').val(),
                                        to: $('#dateVacationTo').val(),
                                        sickness: $('#sicknessText').val(),
                                        type: type
                                    };

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Sick/Edit',
                                        data: AddAntiForgeryToken(createEvent),
                                        success: function (data) {
                                            editBarForGantt(createEvent, from, to)

                                        },
                                        error: function (data) { }
                                    })
                                    $('#dateVacationFrom').datepicker('destroy');
                                    $('#dateVacationTo').datepicker('destroy');
                                    $('#htmlDialog').dialog("destroy");
                                    $('#htmlDialog').remove();
                                }
                                else {
                                    SetErrorStyleEdit(regResultFrom, regResultTo, vacationFrom, vacationTo);
                                }
                            }
                        });

                        //$(document).on('click', '#Delete', function () {
                        $("#Delete").click(function(){
                            {
                                var createEvent = {
                                    id: id,
                                    from: convertDate(from),
                                    to: convertDate(to),
                                    type: type,
                                    sickness: $('#sicknessText').val(),
                                };

                                $.ajax({
                                    type: 'POST',
                                    url: '/Sick/Delete',
                                    data: AddAntiForgeryToken(createEvent),

                                    success: function (data) {
                                        deleteBarForGantt(from, to, id)
                                    },
                                    error: function (data) { }
                                })
                                $('#daveVacationFrom').datepicker('destroy');
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                    });
                        
                    },
                    close: function (event, ui) {
                        $(this).dialog('destroy');
                        $(this).remove();
                    }
                })
            }
            break;
        default:
            break;
    }
}

function CreateEvent(rowId, startDate) {

    var element = $(this);
    var dialogId = 'Create Event';

    $(CreateHtmlDialog).dialog({
        modal: true,
        height: 'auto',
        width: 'auto',
        resizable: false,
        title: 'Create Event',
        position: {
            my: 'center',
            at: 'center'
        },

        open: function (event, ui) {
            $('#CreateEvent').button();
            $('#dateVacationFrom').val(convertDate(startDate));
            var lastName = $("div[data-id='" + rowId + "']").find($(".fn-label")).text();

            $('#LastNameText').text(lastName);
            $('#dateVacationFrom').datepicker({
                firstDay: 1,
                dateFormat: 'dd.mm.yy',
                showWeek: true,
                calculateWeek: myWeekCalc,
                showOn: 'button',
                buttonImage: '/Content/themes/base/images/calendar2.gif',
                buttonImageOnly: true
            });
            $('#dateVacationTo').datepicker({
                firstDay: 1,
                dateFormat: 'dd.mm.yy',
                showWeek: true,
                calculateWeek: myWeekCalc,
                showOn: 'button',
                buttonImage: '/Content/themes/base/images/calendar2.gif',
                buttonImageOnly: true,
                defaultDate: new Date($.datepicker.parseDate("dd.mm.yy",convertDate(startDate)))
            });

            //$(document).on('change','#event', function () {
            $("#event").on("change",function(){
                var selectedOption = $("#event :selected").text();

                switch (selectedOption.toLowerCase()) {
                    case 'ReclaimedOvertime'.toLowerCase():
                        $('#dateVacationTo').datepicker('destroy');
                        $('#dateVacationTo').val($('#dateVacationFrom').val());
                        $('#daysToReclaimRow').css("display", "table-row");
                        $('#hideSick').css("display", "none");
                        $('#hideVacation').css("display", "none");
                        $('#hideOverralDays').css("display", "none");
                        $('#dateVacationFrom').attr("readonly", "");
                        $('#dateVacationTo').attr("readonly", "");
                        $.ajax({
                            type: 'POST',
                            url: '/Journey/GetJourneysAndOvertimesForOneEmp',
                            data: AddAntiForgeryToken({ eID: rowId }),
                            success: function (data) {
                                $('#daysToReclaimEvent').append(data);
                            },
                            error: function (data) {
                            }
                        })
                        break;
                    case 'OvertimeForReclaim'.toLowerCase():
                    case 'PrivateMinus'.toLowerCase():
                        {
                            $('#dateVacationTo').datepicker('destroy');
                            $('#dateVacationTo').val($('#dateVacationFrom').val());
                            $('#hideSick').css("display", "none");
                            $('#hideVacation').css("display", "none");
                            $('#hideOverralDays').css("display", "none");
                            $('#dateVacationFrom').attr("readonly", "");
                            $('#dateVacationTo').attr("readonly", "");
                            $('#daysToReclaimRow').css("display", "none");


                        }
                        break;
                    case 'SickAbsence'.toLowerCase():
                        {
                            $('#hideVacation').css("display", "none");
                            $('#hideOverralDays').css("display", "none");
                            $('#hideSick').css("display", "table-row");
                            $('#dateVacationTo').val('');

                            var dateTxtFormat = $('#dateVacationFrom').val()
                            var dateVacFromDate = $.datepicker.parseDate("dd.mm.yy", dateTxtFormat);
                            $('#dateVacationTo').datepicker('destroy');

                            $('#dateVacationTo').datepicker({
                                firstDay: 1,
                                dateFormat: 'dd.mm.yy',
                                showWeek: true,
                                calculateWeek: myWeekCalc,
                                showOn: 'button',
                                buttonImage: '/Content/themes/base/images/calendar2.gif',
                                buttonImageOnly: true,
                                defaultDate: new Date(dateVacFromDate)
                            });
                            $('#daysToReclaimRow').css("display", "none");
                        }
                        break;

                    case 'Paid Vacation'.toLowerCase():
                    case 'Unpaid Vacation'.toLowerCase():
                        {
                            $('#hideVacation').css("display", "table-row");
                            $('#hideOverralDays').css("display", "table-row");
                            $('#hideSick').css("display", "none");
                            $('#dateVacationTo').val('');

                            var dateTxtFormat = $('#dateVacationFrom').val()
                            var dateVacFromDate = $.datepicker.parseDate("dd.mm.yy", dateTxtFormat);
                            $('#dateVacationTo').datepicker('destroy');

                            $('#dateVacationTo').datepicker({
                                firstDay: 1,
                                dateFormat: 'dd.mm.yy',
                                showWeek: true,
                                calculateWeek: myWeekCalc,
                                showOn: 'button',
                                buttonImage: '/Content/themes/base/images/calendar2.gif',
                                buttonImageOnly: true,
                                defaultDate: new Date(dateVacFromDate)
                            });                            
                            $('#daysToReclaimRow').css("display", "none");
                        }
                        break;

                    default:
                        {
                            $('#dateVacationTo').datepicker({
                                firstDay: 1,
                                dateFormat: 'dd.mm.yy',
                                showWeek: true,
                                calculateWeek: myWeekCalc,
                                showOn: 'button',
                                buttonImage: '/Content/themes/base/images/calendar2.gif',
                                buttonImageOnly: true
                            });
                            $('#hideSick').css("display", "none");
                            $('#hideVacation').css("display", "none");
                            $('#hideOverralDays').css("display", "none");
                            $('#dateVacationTo').removeAttr("readonly");
                            $('#daysToReclaimRow').css("display", "none");
                        }
                        break;
                }
            });

            //$(document).on('change','#dateVacationFrom', function () {
            $("#dateVacationFrom").on("change",function(){
                var selectedOption = $("#event :selected").text();

                switch (selectedOption.toLowerCase()) {
                    case 'ReclaimedOvertime'.toLowerCase():
                    case 'OvertimeForReclaim'.toLowerCase():
                    case 'PrivateMinus'.toLowerCase():
                        {
                            $('#dateVacationTo').val($('#dateVacationFrom').val());
                        }
                        break;
                }

                var dateTxtFormat = $('#dateVacationFrom').val()
                var dateVacFromDate = $.datepicker.parseDate("dd.mm.yy", dateTxtFormat);

                $('#dateVacationTo').datepicker('destroy');

                $('#dateVacationTo').datepicker({
                    firstDay: 1,
                    dateFormat: 'dd.mm.yy',
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true,
                    defaultDate: new Date(dateVacFromDate)
                });
            });

            //$(document).on('change','#dateVacationTo', function () {
            $("#dateVacationTo").on("change",function(){
                var selectedOption = $("#event :selected").text();

                switch (selectedOption.toLowerCase()) {
                    case 'Paid Vacation'.toLowerCase():
                    case 'Unpaid Vacation'.toLowerCase():
                        {
                            var vacationDates = {
                                fromDate: $('#dateVacationFrom').val(),
                                toDate: $('#dateVacationTo').val()
                            }

                            $.ajax({
                                type: 'POST',
                                url: '/Vacation/CalculateVacation/',
                                data: AddAntiForgeryToken(vacationDates),
                                success: function (data) {
                                    $('#calcWorkdays').text(data);
                                },
                                error: function (data) { }
                            })

                            $.ajax({
                                type: 'POST',
                                url: '/Vacation/CalculateOverralDays/',
                                data: AddAntiForgeryToken(vacationDates),
                                success: function (data) {
                                    $('#calcOverralDays').text(data);
                                },
                                error: function (data) { }
                            })
                        }
                }
            })

            //$(document).on('click', '#CreateEvent', function () {
            $('#CreateEvent').click(function(){

                var vacationFrom = new Date($.datepicker.parseDate("dd.mm.yy", /*convertDate(startDate)*/ $('#dateVacationFrom').val()));
                var vacationTo = new Date($.datepicker.parseDate("dd.mm.yy", $('#dateVacationTo').val()));

                var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                var regResultTo = reg.test($('#dateVacationTo').val());

                if (regResultTo == true && (vacationTo.getTime() >= vacationFrom.getTime()))
                {
                    SetNoErrorStyle();
                   

                    var selectedValue = $("#event :selected").text();
                    var createEvent;

                    switch (selectedValue.toLowerCase()) {
                        case 'ReclaimedOvertime'.toLowerCase():
                            {
                                if ($("#daysToReclaimEvent :selected").text() == "") {
                                    $("#daysToReclaimEvent").css('border', '1px solid rgb(232,12,77)');
                                    $('#errorMsgReclaim').text('Nothing to reclaim!');
                                }
                                else {
                                    $('#daysToReclaimEvent').css('border', '1px solid rgb(226,226,226)');
                                    $('#errorMsgReclaim').text('');
                                    var createEvent = {
                                        id: rowId,
                                        from: $('#dateVacationFrom').val(),
                                        to: $('#dateVacationTo').val(),
                                        type: $("#event :selected").text(),
                                        reclaimDate: $('#daysToReclaimEvent :selected').text()
                                    };
                                    $.ajax({
                                        type: 'POST',
                                        url: '/Overtime/Create',
                                        data: AddAntiForgeryToken(createEvent),
                                        success: function (data) {
                                            createBarForGantt(createEvent)
                                        },
                                        error: function (data) { }
                                    })
                                    $('#dateVacationTo').datepicker('destroy');
                                    $('#htmlDialog').dialog("destroy");
                                    $('#htmlDialog').remove();
                                }
                            }
                            break;
                        case 'OvertimeForReclaim'.toLowerCase():
                        case 'PrivateMinus'.toLowerCase():

                            {
                                var createEvent = {
                                    id: rowId,
                                    from: $('#dateVacationFrom').val(),
                                    to: $('#dateVacationTo').val(),
                                    type: $("#event :selected").text(),
                                    reclaimDate: $('#daysToReclaimEvent :selected').text()
                                };

                                $.ajax({
                                    type: 'POST',
                                    url: '/Overtime/Create',
                                    data: AddAntiForgeryToken(createEvent),
                                    success: function (data) {
                                        createBarForGantt(createEvent)
                                    },
                                    error: function (data) { }
                                })
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                            break;

                        case 'Paid Vacation'.toLowerCase():
                        case 'Unpaid Vacation'.toLowerCase():
                            {
                                $('#dateVacationTo').datepicker({
                                    firstDay: 1,
                                    dateFormat: 'dd.mm.yy',
                                    showWeek: true,
                                    calculateWeek: myWeekCalc,
                                    showOn: 'button',
                                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                                    buttonImageOnly: true
                                });

                                var createEvent = {
                                    id: rowId,
                                    from: $('#dateVacationFrom').val(),
                                    to: $('#dateVacationTo').val(),
                                    type: $("#event :selected").text()
                                };

                                $.ajax({
                                    type: 'POST',
                                    url: '/Vacation/Create',
                                    data: AddAntiForgeryToken(createEvent),
                                    success: function (data) {
                                        createBarForGantt(createEvent)
                                    },
                                    error: function (data) { }
                                })
                                $('#dateVacationFrom').datepicker('destroy');
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                            break;

                        case 'SickAbsence'.toLowerCase():
                            {
                                $('#dateVacationTo').datepicker({
                                    firstDay: 1,
                                    dateFormat: 'dd.mm.yy',
                                    showWeek: true,
                                    calculateWeek: myWeekCalc,
                                    showOn: 'button',
                                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                                    buttonImageOnly: true
                                });

                                var createEvent = {
                                    id: rowId,
                                    from: $('#dateVacationFrom').val(),
                                    to: $('#dateVacationTo').val(),
                                    sickness: $('#sickText').val(),
                                    type: $("#event :selected").text()
                                };

                                $.ajax({
                                    type: 'POST',
                                    url: '/Sick/Create',
                                    data: AddAntiForgeryToken(createEvent),
                                    success: function (data) {
                                        createBarForGantt(createEvent)
                                    },
                                    error: function (data) { }
                                })
                                $('#hideSick').css("display", "none");
                                $('#dateVacationFrom').datepicker('destroy');
                                $('#dateVacationTo').datepicker('destroy');
                                $('#htmlDialog').dialog("destroy");
                                $('#htmlDialog').remove();
                            }
                            break;
                    }
                }
                else
                {
                    SetErrorStyle();
                }
            })
        },

        close: function (event, ui) {
            $('#dateVacationTo').datepicker('destroy');
            $(this).dialog('destroy');
            $(this).remove();
        }
    })
}

function createBarForGantt(calendarItem) {

    $(".dataPanel").append(createElementForGantt(calendarItem));

}

function createElementForGantt(calendarItem) {

    if (calendarItem.from == null)
    {
        var calendarItem = {
            id: calendarItem.id,
            from: calendarItem.newFrom,
            to: calendarItem.newTo,
            type: calendarItem.type,
        };
    }

    var startDate = parseFloat($("div[id^='dw-']").first().attr("repdate"));

    var xTemp = (($.datepicker.parseDate("dd.mm.yy", calendarItem.from)).getTime() - startDate);
    var xOffset = (xTemp / 3600000) + 1; //left attribute
    var yOffset = parseInt($('div[data-id="' + calendarItem.id + '"]').attr("offset")) + 122;//top attribute
    var customClass;
    var desc;
    var sick_desc = "";
    var datesForDescription = " From: " + calendarItem.from + " - To: " + calendarItem.to;
    var dayfrom;
    var dayto;
    var style;
    var width;

    switch (calendarItem.type.toLowerCase().replace(/\s/g, "")) {
        case 'ReclaimedOvertime'.toLowerCase():
            customClass = "bar ganttOrange";
            desc = "ReclaimedOvertime" + datesForDescription;
            break;
        case 'OvertimeForReclaim'.toLowerCase():
            customClass = "bar ganttMagenta";
            desc = "OvertimeForReclaim" + datesForDescription;
            break;
        case 'PrivateMinus'.toLowerCase():
            customClass = "bar ganttYellow";
            desc = "PrivateMinus" + datesForDescription;
            break;
        case 'PaidVacation'.toLowerCase():
            customClass = "bar ganttBlue";
            desc = "PaidVacation" + datesForDescription;
            break;
        case 'UnpaidVacation'.toLowerCase():
            desc = "UnpaidVacation" + datesForDescription;
            customClass = "bar ganttRed";
            break;
        case 'SickAbsence'.toLowerCase():
            customClass = "bar ganttViolet";
            desc = "SickAbsence" + datesForDescription;
            sick_desc = calendarItem.sickness;
            break;
        default:
            customClass = "bar ganttWhite";
            desc = "";
            break;
    }

    dayfrom = $.datepicker.parseDate("dd.mm.yy", calendarItem.from).getTime();
    dayto = $.datepicker.parseDate("dd.mm.yy", calendarItem.to).getTime();
    var temp = ($.datepicker.parseDate("dd.mm.yy", calendarItem.to) - $.datepicker.parseDate("dd.mm.yy", calendarItem.from))
    width = (temp / (86400000) * 24) + 23;
    style = "width: " + width + "px; top: " + yOffset + "px; left: " + xOffset + "px;";

    var result = document.createElement("div");

    result.setAttribute("class", customClass);
    result.setAttribute("style", style);
    result.setAttribute("dayfrom", dayfrom);
    result.setAttribute("dayto", dayto);
    result.setAttribute("data_id", calendarItem.id);
    result.setAttribute("desc", desc);
    result.setAttribute("sick_desc", sick_desc);

    var fnLabel = document.createElement("div");
    fnLabel.setAttribute("class", "fn-label");
    fnLabel.setAttribute("style", "");

    jQuery.data(result, "dataObj", "foo.bar[i]");

    result.appendChild(fnLabel);

    result.onmouseover = function (e) {
        var hint = $('<div class="fn-gantt-hint" />').html(desc);
        $("body").append(hint);
        hint.css("left", e.pageX);
        hint.css("top", e.pageY);
        hint.show();
    }
    result.onmouseout = function () {
        $(".fn-gantt-hint").remove();
    }
    result.onmousemove = function (e) {
        $(".fn-gantt-hint").css("left", e.pageX);
        $(".fn-gantt-hint").css("top", e.pageY + 15);
    };

    result.addEventListener('click', (function (e) {
        e.stopPropagation();
        EditEvent(calendarItem.type, dayfrom, dayto, calendarItem.id, calendarItem.sickness);
    }));
    return result;
}

function deleteBarForGantt(dayfrom, dayto, id) {
    var selectorForRmove = $("div[data_id='" + id + "'][dayfrom='" + dayfrom + "']");
    $("div[data_id='" + id + "'][dayfrom='" + dayfrom + "'][dayto='" + dayto + "']").remove();
}

function editBarForGantt(calendarItem, from, to) {
    var editedElement = $("div[data_id='" + calendarItem.id + "'][dayfrom='" + from + "'][dayto='" + to + "']");
    editedElement.replaceWith(createElementForGantt(calendarItem));
}