using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities;

namespace Models.ViewModel
{
	public class TableColumnsVM
	{
		public TableColumnsVM()
		{
			Tracker = new List<TableColumnsVm?>();
		}
		public List<TableColumnsVm>? Tracker { get; set; }

		public TableColumnsVm? TrackerIndividual { get; set; }
		[NotMapped]
		public string? filterByName { get; set; }
		public bool IsChecked { get; set; }
		[NotMapped]
		public List<SelectListItem>? userNames { get; set; }
		[NotMapped]
		public bool HasPreviousPage { get; set; }
		[NotMapped]
		public bool HasNextPage { get; set; }
		[NotMapped]
		public int PageIndex { get; set; }
		[NotMapped]
		public int TotalPages { get; set; }
	}
}