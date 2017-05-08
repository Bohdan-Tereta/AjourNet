$.validator.addMethod('requiredif', function (value, element, parameters) {
    var id = '#' + parameters['dependentproperty'];
    var checkboxValue;
    if (document.getElementById(parameters['dependentproperty']).getAttribute("type") == "checkbox") {
        checkboxValue = (document.getElementById(parameters['dependentproperty']).checked).toString();
    }
    else {
        checkboxValue = document.getElementById(parameters['dependentproperty']).value.toString();
    }
    var targetvalue = parameters['targetvalue'];
    targetvalue =  (targetvalue == null ? '' : targetvalue).toString();
    if (targetvalue == checkboxValue && (value == null || value == ""))
        return false;
    else
      return true;
}
 );

$.validator.unobtrusive.adapters.add(
'requiredif',
['dependentproperty', 'targetvalue'],
function (options) {
    options.rules['requiredif'] = {
        dependentproperty: options.params['dependentproperty'],
        targetvalue: options.params['targetvalue']
    };
    options.messages['requiredif'] = options.message;
});