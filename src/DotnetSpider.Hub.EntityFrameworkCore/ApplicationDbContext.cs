using System;
using System.Security.Claims;
using DotnetSpider.Hub.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DotnetSpider.Hub.EntityFrameworkCore
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
	{
		public DbSet<Message> Message { get; set; }
		public DbSet<MessageHistory> MessageHistory { get; set; }
		public DbSet<Node> Node { get; set; }
		public DbSet<NodeHeartbeat> NodeHeartbeat { get; set; }
		public DbSet<Task> Task { get; set; }
		public DbSet<TaskHistory> TaskHistory { get; set; }
		public DbSet<TaskStatus> TaskStatus { get; set; }
		public DbSet<TaskLog> TaskLog { get; set; }
		public DbSet<Config> Config { get; set; }

		private readonly IHttpContextAccessor _accessor;


		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor accessor)
			: base(options)
		{
			_accessor = accessor;
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            //如果需要,可以自定义 标识符模型和重写默认值
			// Customize the ASP.NET Identity model and override the defaults if needed.
            //这个示例,重命名了 标识符表字段的名称
			// For example, you can rename the ASP.NET Identity table names and more.
            //需要在base.OnModelCreating(builder)之后再定制它.
			// Add your customizations after calling base.OnModelCreating(builder);

			builder.Entity<Node>().HasAlternateKey(c => c.NodeId).HasName("AlternateKey_NodeId");
			builder.Entity<Config>().HasAlternateKey(c => c.Name).HasName("AlternateKey_Name");
			builder.Entity<TaskLog>().HasIndex(c => c.Identity).HasName("Index_Identity");
		}

		public override int SaveChanges()
		{
			ApplyAudited();
			return base.SaveChanges();
		}

        /// <summary>
        /// 自动审计
        /// </summary>
		private void ApplyAudited()
		{
			var value = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			long userId = 0;
			long.TryParse(value, out userId);
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
			builder.UseSqlServer("Server=.\\SQLEXPRESS;Database=DotnetSpiderHub_Dev;Integrated Security = SSPI;");
			return new ApplicationDbContext(builder.Options, null);
		}
	}
}
