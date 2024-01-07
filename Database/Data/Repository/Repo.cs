using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Database.Data.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Repository
{
	public class Repo<T> : IRepo<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		protected DbSet<T> Set { get; private set; }

		public Repo(ApplicationDbContext db)
		{
			this._db = db;
			this.Set = _db.Set<T>();
		}

		public async Task AddAsync(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			await Set.AddAsync(entity);
		}

		public void Delete(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			Set.Remove(entity);
		}

		public void DeleteRange(IEnumerable<T> entities)
		{
			if (entities == null)
			{
				throw new ArgumentNullException(nameof(entities));
			}

			Set.RemoveRange(entities);
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
		{
			try
			{
				if (filter == null)
				{
					throw new ArgumentNullException(nameof(filter));
				}

				return await Set.FirstOrDefaultAsync(filter);
			}
			catch (Exception ex)
			{
				// Handle or log the exception as needed
				throw new Exception("An error occurred while executing GetAsync.", ex);
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
		{
			try
			{
				return filter != null ? await Set.Where(filter).ToListAsync() : await Set.ToListAsync();
			}
			catch (Exception ex)
			{
				// Handle or log the exception as needed
				throw new Exception("An error occurred while executing GetAllAsync.", ex);
			}
		}

		public async Task SaveAsync()
		{
			try
			{
				await _db.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Handle or log the exception as needed
				throw new Exception("An error occurred while executing SaveAsync.", ex);
			}
		}
	}
}
