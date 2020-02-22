using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CSharpBeltExam.Models
{
    [NotMapped]
    public class Login
    {
        [Display(Name="Email")]
        [Required]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Display(Name="Password")]
        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}