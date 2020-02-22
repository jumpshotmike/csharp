using Microsoft.EntityFrameworkCore;


namespace CSharpBeltExam.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Affair> Affairs { get; set; }
        public DbSet<UserAffair> UsersAffairs { get; set; }
    }
}