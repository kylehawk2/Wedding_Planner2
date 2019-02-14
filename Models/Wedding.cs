using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Wedding_Planner2.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        public string Address {get;set;}
        public DateTime Date {get;set;}
        public string WedderOne {get;set;}
        public string WedderTwo {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
        public int UserId {get;set;}
        public List<RSVP> RSVPs {get;set;}
    }
}