$(document).ready(function () {
    $("[href^=#ui-tabs-1]").click(function () {
        $.datepicker.regional[""].dateFormat = 'dd.mm.yy';
        $.datepicker.setDefaults($.datepicker.regional[""]);
    })
})

$(document).ready(function () {
    $("[href=#ui-tabs-5]").click(function () {
        $.datepicker.regional[""].dateFormat = 'yy-mm-dd';
        $.datepicker.setDefaults($.datepicker.regional[""]);
    })
})