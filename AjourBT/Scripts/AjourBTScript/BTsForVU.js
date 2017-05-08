var scrollPositionVUBTsDataInQuarters;
var scrollPositionVUPrepBTsData;

$(document).on("click", "a#ShowBTInformation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Show BT Data";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = $(this).attr('href');

    scrollPositionVUBTsDataInQuarters = $('div#tableBodyVUForQuarter > div#VUBusinessTripDataInQuarter > div#BTsInQuarterForViewerexample_wrapper >  div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $(this).load(url);
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/VU/GetBusinessTripDataInQuarterVU/",
                    type: "Get",
                    data: $('#formBTsDataInQuarterVU').serialize(),
                    success: function (data) {

                        var elementId = document.getElementById("VUBusinessTripDataInQuarter").getElementsByTagName("th");
                        var scrollPositionObject = $('div#tableBodyVUForQuarter > div#VUBusinessTripDataInQuarter > div#BTsInQuarterForViewerexample_wrapper >  div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
                        var elementToReplace = $("#VUBusinessTripDataInQuarter");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);
                        
                        scrollPositionVUBTsDataInQuarters = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionVUBTsDataInQuarters);
                    }
                })
            }
        });
        $("#ShowBTInformation-VU").dialog('open');
        return false;
    });
});

$(document).on("click", "a#PrepBTsDataShowBTInformation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Show BT Data";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPositionVUPrepBTsData = $('div#PrepBTsDataVU > div#prepBTDataVU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $(this).load(url);
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/VU/GetPrepBusinessTripDataVU/",
                    type: "Get",
                    data: $('#formPrepBTsDataVU').serialize(),
                    success: function (data) {

                        var elementId = document.getElementById("PrepBTsDataVU").getElementsByTagName("th");
                        var scrollPositionObject = $('div#PrepBTsDataVU > div#prepBTDataVU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
                        var elementToReplace = $("#PrepBTsDataVU");
                        var scrollPositionBTBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionVUPrepBTsData = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionVUPrepBTsData);
                    }
                })
            }
        });
        $("#ShowBTInformation-VU").dialog('open');
        return false;
    });
});

$(document).on("click", "#ShowBTDataVU", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "ShowBTData-VU";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionVU = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
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
                    url: "/VU/GetVisaDataVU/",
                    type: "GET",
                    data: { searchString: $('#visaVUSearch #seachInput').val() },
                    success: function (data) {
                        var elementId = document.getElementById("visaTableForVU").getElementsByTagName("th");
                        var scrollPositionObject = $("div#visasViewVU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        var elementToReplace = $("#visaTableForVU");
                        var scrollPositionVUBefore = SaveSortAfterTableRefresh(elementId, elementToReplace, scrollPositionObject, data);

                        scrollPositionVU = scrollPositionVUBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionVU);
                    }
                });
            },
        });
        return false;
    });
});

$(document).on("click", "#ExportVisasAndPermitsToXlsVU", function () {
    var formVisasAndPermitsVU = $('#formExportToXlsVU');
    var searchString = $('#searchStringForVisasAndPermitsVU');
    searchString.attr("value", $('#ui-tabs-6 #seachInput').val());
    formVisasAndPermitsVU.submit();

})