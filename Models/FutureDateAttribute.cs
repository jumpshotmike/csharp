using System;
using System.ComponentModel.DataAnnotations;


namespace CSharpBeltExam
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // You first may want to unbox "value" here and cast to a DateTime variable!
            DateTime CurrentTime = DateTime.Now;
            if( (DateTime)value < CurrentTime )
            {
                return new ValidationResult( "Date must be in the future." );
            }

            return ValidationResult.Success;
        }
    }
}