using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


// Joining Table or Bridge Class
namespace CSharpBeltExam.Models
{
    public class UserAffair
    {
      [Key]
      public int UserAffairId { get; set; }
      public int UserId { get; set; }
      public int AffairId { get; set; }

      
      // Navigation properties
      public User User { get; set; }
      public Affair Affair { get; set; }
    }
}