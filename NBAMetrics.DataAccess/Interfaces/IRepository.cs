using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.DataAccess.Interfaces
{
    public interface IRepository<T> where T : class
    {
        DbContext Context { get; set; }

        bool HasChanges { get; }

        bool Save();

        T GetFirstOrDefault(Expression<Func<T, bool>> whereCondition);

        T Add(T entity);

        void Delete(T Entity);

        void DeleteRange(IEnumerable<T> entities);

        void Attach(T entity);

        bool Exists(T entity);

        IQueryable<T> Where(Expression<Func<T, bool>> whereCondition);

        IQueryable<T> AsQueryable();

        long Count();

        bool SaveChanges();

        IQueryable<T> GetIncluding(string[] paths, Expression<Func<T, bool>> whereCondition);

        IQueryable<T> GetIncludingLambda(params Expression<Func<T, object>>[] includes);

        T First(Expression<Func<T, bool>> whereCondition);

        IList<T> GetAll();
    }
}
