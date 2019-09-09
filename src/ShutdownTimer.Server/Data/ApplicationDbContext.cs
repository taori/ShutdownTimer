using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Data
{
	public class ApplicationDbContext : IdentityDbContext<ServiceUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<ShutdownHistoryItem> ShutdownExecutionHotlinks { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ShutdownHistoryItem>().HasKey(d => new {d.UserId, d.Hours, Seconds = d.Minutes});
		}

		public override int SaveChanges()
		{
			ModifyChangeTrackerEntries();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			ModifyChangeTrackerEntries();
			return base.SaveChangesAsync(cancellationToken);
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			ModifyChangeTrackerEntries();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
		{
			ModifyChangeTrackerEntries();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void ModifyChangeTrackerEntries()
		{
			UpdateCreateStamps();
		}

		private void UpdateCreateStamps()
		{
			var newEntities = this.ChangeTracker.Entries()
				.Where(
					x => x.State == EntityState.Added &&
					     x.Entity != null &&
					     x.Entity is ICreatedStamp
				)
				.Select(x => x.Entity as ICreatedStamp);

			foreach (var newEntity in newEntities)
			{
				newEntity.Created = DateTime.UtcNow;
			}
		}
	}
}
