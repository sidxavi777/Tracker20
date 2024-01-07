using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Database.Data.IRepository
{
	public interface IRepo<T> where T : class
	{
		Task AddAsync(T entity);
		void Delete(T entity);
		void DeleteRange(IEnumerable<T> entities);
		Task<T> GetAsync(Expression<Func<T, bool>> filter);
		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
		Task SaveAsync();
	}
}
