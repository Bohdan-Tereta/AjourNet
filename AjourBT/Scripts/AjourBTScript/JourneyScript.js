var scrollPositionABM;

function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

function SetNoErrorStyle() {
    $('#ReclaimDate').css("border", "1px solid #e2e2e2");
    $('#errorMsg').text('');
}

function SetErrorStyle() {
    $('#errorMsg').text('Wrong DateTime value');
    $('#ReclaimDate').css("border", "1px solid #e80c4d");
}


$(document).on("click", "#journeyLink", function (event) {
    event.preventDefault();
    scrollPositionABM = $('div#journeysViewABM_wrapper>div.dataTables_scroll>div.dataTables_scrollBody').scrollTop();
    var element = $(this);
    var dialogId = "Edit Reclaim Date";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        var journeyDialog = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Edit Reclaim Date',
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                $('#btnEditReclaimDate').button();
                $('#ReclaimDate').datepicker({
                    firstDay: 1,
                    dateFormat: "dd.mm.yy",
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true
                });

                $('#btnEditReclaimDate').click(function (event) {
                    event.preventDefault()
                    SetNoErrorStyle();

                    var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                    var regResult = reg.test($('#ReclaimDate').val());
                    if ($('#ReclaimDate').val().length == 0) {
                        regResult = true;
                    }

                    if (regResult == true) {
                        date = $('#editJourneyForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/Journey/EditJourney",
                            data: $('#editJourneyForm').serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#journeysViewABM").html(data.error);
                                }
                                else {
                                    $(journeyDialog).dialog("close");
                                    var elementId = document.getElementById("journeysView").getElementsByTagName("th");
                                    var scrollPositionObject = $("div.dataTables_scrollBody").scrollTop();
                                    var elementToReplace = $("#tbodyJourneys");
                                    var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                                    scrollPositionABM = scrollPositionBTBefore;
                                    setTimeout(function () { $("div.dataTables_scrollBody").scrollTop(scrollPositionABM); }, 100);
                                }
                            },

                            error: function (data) {
                                //alert(data)
                            }
                        })
                    }
                    else {
                        SetErrorStyle();
                    }
                })
            },

                close: function (event, ui) {
                    $(this).dialog("destroy");
                    $(this).remove();
                }
            });
    });
    return false;
});