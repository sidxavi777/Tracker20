using Database.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Data.Repository
{
	public class Tracker : Repo<TableColumnsVm>, ITracker
	{
		private readonly ApplicationDbContext _db;

		public Tracker(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public async Task UpdateAsync(TableColumnsVm obj)
		{
			_db.Tracker.Update(obj);
			await _db.SaveChangesAsync();
		}
	}
}
