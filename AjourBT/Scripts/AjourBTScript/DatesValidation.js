jQuery.validator.addMethod("checkdates", function (value, element, params) {
    var endDate = parseDate(value);
    var startDate = parseDate(document.getElementsByName(params).item(0).value);
    if (endDate < startDate) {
        return false;
    }
    else
        return true;
}, '');

jQuery.validator.unobtrusive.adapters.add("checkdates", ["startdate"], function (options) {
    options.rules['checkdates'] = options.params.startdate;
    options.messages['checkdates'] = options.message;
});

function parseDate(input) {
    var parts = input.split('.');
    return new Date(parts[2], parts[1] - 1, parts[0]);//months are 0-based
}