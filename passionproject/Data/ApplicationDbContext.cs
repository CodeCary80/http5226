using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using passionproject.Itinerary;

namespace passionproject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSet properties for your models
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<ActivityMember> ActivityMembers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseSplit> ExpenseSplits { get; set; }
        public DbSet<ActivityRating> ActivityRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the many-to-many relationship through ActivityMember
            builder.Entity<ActivityMember>()
                .HasKey(am => new { am.ActivityId, am.MemberId }); // Set composite key

            builder.Entity<ActivityMember>()
                .HasOne(am => am.Activity)
                .WithMany(a => a.ActivityMembers)
                .HasForeignKey(am => am.ActivityId);

            builder.Entity<ActivityMember>()
                .HasOne(am => am.Member)
                .WithMany(m => m.ActivityMembers)
                .HasForeignKey(am => am.MemberId);

            builder.Entity<Expense>()
                .HasOne(e => e.Activity)
                .WithMany()
                .HasForeignKey(e => e.ActivityId);

            builder.Entity<ExpenseSplit>()
                .HasOne(es => es.Expense)
                .WithMany(e => e.ExpenseSplits)
                .HasForeignKey(es => es.ExpenseId);

            builder.Entity<ExpenseSplit>()
                .HasOne(es => es.Member)
                .WithMany()
                .HasForeignKey(es => es.MemberId);

            builder.Entity<ActivityRating>()
                .HasOne(ar => ar.Activity)
                .WithMany()
                .HasForeignKey(ar => ar.ActivityId);

            builder.Entity<ActivityRating>()
                .HasOne(ar => ar.Member)
                .WithMany()
                .HasForeignKey(ar => ar.MemberId);

        }
    }
}