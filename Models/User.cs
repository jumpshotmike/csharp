using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    
namespace CSharpBeltExam.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Display(Name="First Name")]
        [Required]
        [MinLength(2, ErrorMessage="First name must have at least 2 characters")]
        public string FirstName { get; set; }
        
        [Display(Name="Last Name")]
        [Required]
        [MinLength(2, ErrorMessage="Last name must have at least 2 characters")]
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
        
        [NotMapped]                      // Will not be mapped to your users table!
        [Compare("Password")]
        [Display(Name="Confirm Password")]
        [Required]                
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        // CreatedAt and UpdatedAt
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Navigation Properties
        
        public List<Affair> CreatedAffairs { get; set; }
        public List<UserAffair> Affairs { get; set; }       // using the UserAffair bridge table 
    }   
}