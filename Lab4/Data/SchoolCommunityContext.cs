using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Data
{
    public class SchoolCommunityContext : DbContext
    {
        public SchoolCommunityContext(DbContextOptions<SchoolCommunityContext> options) : base(options)
        {
        }

        public DbSet<Community> Communities { 
            get; 
            set; 
        }
        public DbSet<Student> Students { 
            get; 
            set; 
        }
        public DbSet<CommunityMembership> CommunityMemberships
        {
            get;
            set;
        }
        public DbSet<Advertisement> Advertisements
        {
            get;
            set;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advertisement>().ToTable("Advertisement");
            modelBuilder.Entity<Community>().ToTable("Community");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<CommunityMembership>()
                .HasKey(c => new { c.StudentID, c.CommunityID });


        }
    }
}