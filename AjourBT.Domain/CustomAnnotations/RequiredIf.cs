using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.CustomAnnotations
{

    public class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        private RequiredAttribute _innerAttribute = new RequiredAttribute();

        private string _anotherProperty { get; set; }
        public object TargetValue { get; set; }

        public RequiredIfAttribute(string AnotherProperty, object targetValue)
        {
            this._anotherProperty = AnotherProperty;
            this.TargetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get a reference to the property this validation depends upon
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(this._anotherProperty);

            if (field != null)
            {
                // get the value of the dependent property
                var dependentvalue = field.GetValue(validationContext.ObjectInstance, null);

                // compare the value against the target value
                if ((dependentvalue == null && this.TargetValue == null) ||
                    (dependentvalue != null && dependentvalue.Equals(this.TargetValue)))
                {
                    // match => means we should try validating this field
                    if (!_innerAttribute.IsValid(value))
                        //validation failed - return an error
                        return new ValidationResult(this.ErrorMessage, new[] { validationContext.DisplayName });
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };

            //string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            //find the value on the control we depend on;
            //if it's a bool, format it javascript style 
            // (the default is True or False!)
            string targetValue = (this.TargetValue ?? "").ToString();
            if (this.TargetValue != null)
                if (this.TargetValue.GetType() == typeof(bool))
                    targetValue = targetValue.ToLower();

            rule.ValidationParameters.Add("dependentproperty", _anotherProperty);
            rule.ValidationParameters.Add("targetvalue", targetValue);

            yield return rule;
        }
    }
}
