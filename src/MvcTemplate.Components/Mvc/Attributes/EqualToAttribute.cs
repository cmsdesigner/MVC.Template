﻿using MvcTemplate.Resources;
using MvcTemplate.Resources.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MvcTemplate.Components.Mvc
{
    public class EqualToAttribute : ValidationAttribute
    {
        public String OtherPropertyName { get; private set; }
        public String OtherPropertyDisplayName { get; set; }

        public EqualToAttribute(String otherPropertyName)
            : base(() => Validations.FieldMustBeEqualTo)
        {
            OtherPropertyName = otherPropertyName;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, OtherPropertyDisplayName);
        }
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(OtherPropertyName);
            Object otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance);

            if (Object.Equals(value, otherPropertyValue))
                return null;

            OtherPropertyDisplayName = ResourceProvider.GetPropertyTitle(validationContext.ObjectType, OtherPropertyName);

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
