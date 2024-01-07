using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Models;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Database
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<TableColumnsVm> Tracker { get; set; }
		public DbSet<Comment> comment { get; set; }
		public DbSet<ApplicationUser> applicationUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			//identity
			base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Comment>()
			.HasOne(c => c.TableColumn)
			.WithMany(tc => tc.CommentOfaTicket)
			.HasForeignKey(c => c.TicketNumber);

			modelBuilder.Entity<TableColumnsVm>().HasData(new TableColumnsVm
			{
				TableColumnsId = 1,
				TicketNumber = "ABC123",
				Description = "Sample Description 1",
				CreatedOn = DateOnly.FromDateTime(DateTime.Now),
				CreatedBy = "John Doe",
				AssignedTo = "fake",
				Status = Utilities.Status.InProgress,
				Type = "Bug",
				Application = "SampleApp",
				Eta = new List<DateOnly?> { DateOnly.FromDateTime(DateTime.Now.AddDays(3)), DateOnly.FromDateTime(DateTime.Now.AddDays(6)) },
				CompletedDate = null
			});

			modelBuilder.Entity<Comment>().HasData(new Comment
			{
				Id = 1,
				Name = "Commenter",
				Description = "Sample Comment",
				Time = DateTime.Now,
				TicketNumber = 1 // Assuming this matches the TableColumnsId
			});



		}


	}
}
