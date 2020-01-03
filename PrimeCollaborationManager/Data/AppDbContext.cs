using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamPermission> TeamPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey(m => m.UserId);
            builder.Entity<User>().HasAlternateKey(m => m.BBUserId);
            builder.Entity<User>().HasIndex(m => m.BBUserId);

            builder.Entity<Team>().HasKey(m => m.TeamId);
            builder.Entity<Team>().HasIndex(m => m.UserId);

            builder.Entity<TeamMember>().HasKey(m => m.TeamMemberId);
            builder.Entity<TeamMember>().HasOne(m => m.Team).WithMany(m => m.TeamMembers).HasForeignKey(m => m.TeamId);
            builder.Entity<TeamMember>().HasIndex(m => m.TeamId);

            builder.Entity<TeamPermission>().HasKey(m => new { m.TeamId, m.PermissionType });
            builder.Entity<TeamPermission>().HasOne(m => m.Team).WithMany(m => m.TeamPermissions).HasForeignKey(m => m.TeamId);
            builder.Entity<TeamPermission>().HasIndex(m => m.TeamId);

            base.OnModelCreating(builder);
        }
    }
}
