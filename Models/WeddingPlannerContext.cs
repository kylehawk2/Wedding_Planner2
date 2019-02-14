using Microsoft.EntityFrameworkCore;

namespace Wedding_Planner2.Models
{
    public class WeddingPlannerContext : DbContext 
    {
        public WeddingPlannerContext(DbContextOptions options) : base(options) {}
        
        public DbSet<User> Users {get;set;}
        public DbSet<Wedding> Weddings {get;set;}
        public DbSet<RSVP> RSVP {get;set;}
    }
}