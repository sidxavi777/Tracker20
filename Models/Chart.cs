using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Chart
	{
		public int InProgres;
		public int onHold;
		public int createdPastWeek;
		public int createdPastMonth;
		public int closedPastWeek;
		public int closedPastMonth;
		public int overallClosedCount;
		public int overAllNew;

	}
}
