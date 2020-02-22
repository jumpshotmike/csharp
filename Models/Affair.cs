using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CSharpBeltExam.Models
{
    public class Affair
    {
        [Key]
        public int AffairId { get; set; }

        
        [Display(Name = "Title")]
        [Required(ErrorMessage="Title field is required.")]
        [MinLength(2)]
        public string Title { get; set; }


        [Display(Name = "Time")]
        [Required(ErrorMessage="Time field is required.")]
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }


        [FutureDate]
        [Display(Name = "Date")]
        [Required(ErrorMessage="Date field is required.")]
        [DataType(DataType.Date)]
        public DateTime AffairDate { get; set; }

        
        [Display(Name = "Duration")]
        [Required(ErrorMessage = "Duration field is required.")]
        [DataType(DataType.Duration)]
        public TimeSpan Duration { get; set; }
        

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description field is required.")]
        public string Description { get; set; }


        // CreatedAt and UpdatedAt
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        // Navigation Properties
        public User Creator { get; set; }
        public List<UserAffair> ReserveSpot { get; set; }    // using the bridge class UserAffair
    }
}