using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wedding_Planner2.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [RegularExpression("^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$")]
        public string First_Name {get;set;}
        [Required]
        [RegularExpression("^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$")]
        public string Last_Name {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password nust be at least 8 characters or longer")]
        public string Password {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        [MinLength(8)]
        public string Cpassword {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<RSVP> Attending {get;set;}

    }
}