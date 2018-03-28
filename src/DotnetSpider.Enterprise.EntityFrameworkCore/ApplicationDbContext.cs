using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DotnetSpider.Enterprise.Domain;
using Microsoft.EntityFrameworkCore.Design;

namespace DotnetSpider.Enterprise.EntityFrameworkCore
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
	{
		public DbSet<Message> Message { get; set; }
		public DbSet<MessageHistory> MessageHistory { get; set; }
		public DbSet<Node> Node { get; set; }
		public DbSet<NodeHeartbeat> NodeHeartbeat { get; set; }
		public DbSet<Task> Task { get; set; }
		public DbSet<TaskHistory> TaskHistory { get; set; }
		public DbSet<TaskStatus> TaskStatus { get; set; }
		public DbSet<TaskLog> TaskLog { get; set; }

		private readonly IHttpContextAccessor _accessor;


		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor accessor)
			: base(options)
		{
			_accessor = accessor;
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);

			builder.Entity<ApplicationUserClaim>().HasOne(pt => pt.ApplicationUser).WithMany(t => t.Claims).HasForeignKey(pt => pt.UserId);
			builder.Entity<ApplicationUserRole>().HasOne(pt => pt.ApplicationUser).WithMany(t => t.Roles).HasForeignKey(pt => pt.UserId);
			builder.Entity<ApplicationUserLogin>().HasOne(pt => pt.ApplicationUser).WithMany(t => t.Logins).HasForeignKey(pt => pt.UserId);
			builder.Entity<Node>().HasAlternateKey(c => c.NodeId).HasName("AlternateKey_NodeId");
			builder.Entity<TaskLog>().HasAlternateKey(c => c.Identity).HasName("AlternateKey_Identity");
		}

		public override int SaveChanges()
		{
			ApplyConcepts();
			return base.SaveChanges();
		}

		private void ApplyConcepts()
		{
			var value = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			long? userId = null;
			long id;
			if (long.TryParse(value, out id))
			{
				userId = id;
			}
			var entries = ChangeTracker.Entries();
			foreach (var entry in entries)
			{
				if (entry.Entity is IAuditedEntity)
				{
					var e = entry.Entity as IAuditedEntity;
					switch (entry.State)
					{
						case EntityState.Added:
							e.CreatorUserId = userId;
							e.CreationTime = DateTime.Now;
							break;
						case EntityState.Modified:
							e.LastModifierUserId = userId;
							e.LastModificationTime = DateTime.Now;
							break;
						case EntityState.Deleted:
							break;
					}
				}
			}
		}
	}

	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
			//builder.UseSqlServer("Server=192.168.90.109,29999;User ID = sa; Password='tZ&$V.ziComjA64S*CZu%;t9Zuh1@iE2';database=DotnetSpider;");
			builder.UseSqlServer("Server=.\\SQLEXPRESS;Database=DotnetSpiderEnterprise_Dev;Integrated Security = SSPI;");
			return new ApplicationDbContext(builder.Options, null);
		}
	}
}
