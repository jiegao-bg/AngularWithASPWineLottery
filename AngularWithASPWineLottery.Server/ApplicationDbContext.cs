using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AngularWithASPWineLottery.Server
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// IMPORTANT NOTE: the following method override is redundant 
        /// (since we've already configured our entities using Data Annotations)
        /// and has been left there for demonstration purposes only.
        /// See "Entity Types configuration methods" in Chapter 4 for details.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LotteryUser>().ToTable("LotteryUser");
            modelBuilder.Entity<LotteryUser>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<LotteryUser>()
                .Property(x => x.Id).IsRequired();
            modelBuilder.Entity<LotteryUser>()
                .Property(x => x.Name);
            modelBuilder.Entity<LotteryUser>()
                .Property(x => x.Date).HasColumnType("datetime");
            modelBuilder.Entity<LotteryUser>()
                .Property(x => x.Ticket).HasColumnType("int");

           
            // add the EntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly
            );
        }

        public DbSet<LotteryUser> LotteryPod => Set<LotteryUser>();
    }
}
