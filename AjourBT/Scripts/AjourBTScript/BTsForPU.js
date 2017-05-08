function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPositionPU;
var dialogPU
$(document).on("click", "#EditFinishedBTPU", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Update BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionPU = $("div#tableViewBTsPU > div#exampleBtsView_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    var dateFormat = element.attr('data-date-format');
    $(dialogDiv).load(this.href, function () {
        dialogPU = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit BT",
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                $("#btnSave").focus();

                if ($('#editStartDateFinishedBTPU').length > 0) {
                    $('#editStartDateFinishedBTPU').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editEndDateFinishedBTPU').length > 0) {
                    $('#editEndDateFinishedBTPU').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderStartDateFinishedBTPU').length > 0) {
                    $('#orderStartDateFinishedBTPU').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#orderEndDateFinishedBTPU').length > 0) {
                    $('#orderEndDateFinishedBTPU').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                

                $("#btnSaveFinished").button();

                $("#btnSaveFinished").click(function (event) {
                    event.preventDefault();
                    $("#editFinishedBTForm").validate();
                    if ($("#editFinishedBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/PU/EditFinishedBT/",
                            data: $("#editFinishedBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else if (data.success) {
                                    $(dialogPU).dialog("close");
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
                    url: "/PU/GetBusinessTripDataByDatesPU/",
                    type: "GET",
                    data: { selectedYear: $('#selectedYear :selected').val()},
                    success: function (data) {

                        var elementId = document.getElementById("puByDatesSecondHeader").getElementsByTagName("th");
                        var scrollPositionObject = $("div#btsByDatesPU > div#exampleBtsViewByDatesPU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#exampleBtsViewByDatesPU_wrapper");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionPU = scrollPositionBTBefore;
                        setTimeout(function () { $("div.dataTables_scrollBody").scrollTop(window.scrollPositionPU) }, 100);
                    }
                });
            },

            close: function (event, ui) {
                $('#editEndDateFinishedBTPU').datepicker("destroy");
                $('#editStartDateFinishedBTPU').datepicker("destroy");
                $('#orderStartDateFinishedBTPU').datepicker("destroy");
                $('#orderEndDateFinishedBTPU').datepicker("destroy");

                $(this).dialog("destroy");
                $(this).remove();
            },
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});
