using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<ClientLocation> ClientLocations { get; set; } = null!;
        public DbSet<ClientUnit> ClientUnits { get; set; } = null!;
        public DbSet<AppModule> AppModules { get; set; }
        public DbSet<ProjectMaster> ProjectMasters { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<MilestoneMaster> MilestoneMasters { get; set; } = null!;
        public DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<VendorWork> VendorWorks { get; set; } = null!;
        public DbSet<ApprovalDesk> ApprovalDesks { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            // Ticket CreatedBy / AssignedTo
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1:1 UserSettings
            modelBuilder.Entity<UserSettings>()
                .HasIndex(s => s.UserId)
                .IsUnique();

            // Vendor -> User (login identity)
            modelBuilder.Entity<Vendor>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --------- VENDOR WORK (ONLY 1 FK LEFT: VendorId) ---------

            modelBuilder.Entity<VendorWork>()
                .HasOne(vw => vw.Vendor)
                .WithMany(v => v.VendorWorks)
                .HasForeignKey(vw => vw.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

        
        }
    }
}
