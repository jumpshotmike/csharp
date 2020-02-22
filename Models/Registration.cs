using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CSharpBeltExam.Models
{   
    [NotMapped]
    public class Registration
    {
        [Display(Name="First Name")]
        [Required]
        [MinLength(2, ErrorMessage="First Name must be at least 2 characters long")]
        public string FirstName { get; set; }

        [Display(Name="Last Name")]
        [Required]
        [MinLength(2, ErrorMessage="Last Name must be at least 2 characters long")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name="Password")]
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*\d).{4,8}$", ErrorMessage="Password must include one Upper, one lower, and a special char")]
        // [MinLength(8, ErrorMessage="Password must be at least 8 characters long")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [Display(Name="Confirm Password")]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}