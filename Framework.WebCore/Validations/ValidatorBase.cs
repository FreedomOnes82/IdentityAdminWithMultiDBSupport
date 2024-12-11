﻿using System.ComponentModel.DataAnnotations;

namespace Framework.WebCore.Validations
{
    public class ValidatorBase
    {
        public virtual ValidationResult Validate(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }

}
