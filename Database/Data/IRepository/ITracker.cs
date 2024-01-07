using System;
using System.Threading.Tasks;
using Models;

namespace Database.Data.IRepository
{
	public interface ITracker : IRepo<TableColumnsVm>
	{
		Task UpdateAsync(TableColumnsVm tracker);
	}
}
