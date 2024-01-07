using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Comment
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Time { get; set; }

		// Foreign key
		public int TicketNumber { get; set; }

		// Navigation property
		public TableColumnsVm TableColumn { get; set; }
	}
}
