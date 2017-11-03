using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DotnetSpider.Enterprise.Domain;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;

namespace DotnetSpider.Enterprise.EntityFrameworkCore
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
	{
		public DbSet<Domain.Entities.Logs.Exception> Exceptions { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Task> Tasks { get; set; }
		public DbSet<AgentHeartBeat> Agent_HeartBeats { get; set; }

		private readonly IHttpContextAccessor _accessor;


		//public ApplicationDbContext(DbContextOptions options) : base(options)
		//{
		//}

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			_accessor = DI.IocManager?.GetRequiredService<IHttpContextAccessor>();
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

			//builder.Entity<ApplicationUser>()
			//	.HasMany(e => e.Claims)
			//	.WithOne()
			//	.HasForeignKey(e => e.UserId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			//builder.Entity<ApplicationUser>()
			//	.HasMany(e => e.Logins)
			//	.WithOne()
			//	.HasForeignKey(e => e.UserId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			//builder.Entity<ApplicationUser>()
			//	.HasMany(e => e.Roles)
			//	.WithOne()
			//	.HasForeignKey(e => e.UserId)
			//	.IsRequired()
			//	.OnDelete(DeleteBehavior.Cascade);

			//builder.Entity<TaskBatch>()
			//.HasIndex(b => new { b.TaskId, b.Batch })
			//.IsUnique(true);
		}

		public void DoWithTransaction(Action action)
		{
			if (Database.CurrentTransaction == null)
			{
				IDbContextTransaction transaction = null;
				try
				{
					transaction = Database.BeginTransaction();
					action();
					SaveChanges();
					transaction.Commit();
				}
				catch (System.Exception e)
				{
					if (transaction != null)
					{
						transaction.Rollback();
					}
					throw;
				}
			}
			else
			{
				action();
			}
		}

		public override int SaveChanges()
		{
			ApplyConcepts();
			return base.SaveChanges();
		}

		private void ApplyConcepts()
		{
			var value = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

	public class ToDoContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
			builder.UseSqlServer("Server=.\\SQLEXPRESS;DataBase=DotnetSpiderEnterprise_Dev;Integrated Security = SSPI;");
			return new ApplicationDbContext(builder.Options);
		}
	}
}
