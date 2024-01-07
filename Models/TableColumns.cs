using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities;

namespace Models
{
	public class TableColumnsVm
	{
		public TableColumnsVm()
		{
			CommentOfaTicket = new List<Comment>();
		}

		[Key]
		public int TableColumnsId { get; set; }

		[Required]
		public string TicketNumber { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public DateOnly CreatedOn { get; set; }

		[Required]
		[ValidateNever]
		public string CreatedBy { get; set; }

		[Required]
		[ValidateNever]
		[BindProperty]
		public string AssignedTo { get; set; }
		[NotMapped]
		public IEnumerable<SelectListItem>? AssignedToDropdown { get; set; }

		[Required]
		[BindProperty]
		public Status Status { get; set; }

		public string? Type { get; set; }

		public string? Application { get; set; }

		[ValidateNever]
		public List<DateOnly?>? Eta { get; set; }

		[NotMapped]
		[ValidateNever]
		public DateOnly? NewEta { get; set; }

		public DateOnly? CompletedDate { get; set; }

		[NotMapped]
		public int RowCount { get; set; }

		public List<Comment?>? CommentOfaTicket { get; set; }
	}
}