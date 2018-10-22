using NBAMetrics.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.DataAccess.Repositories
{
    public class RepositoryBase<T> : IRepository<T>, IDisposable where T : class
    {
        /// <summary>
        /// </summary>
        public virtual IDbSet<T> DbSet
        {
            get { return this._context.Set<T>(); }
        }

        /// <summary>
        /// </summary>
        private DbContext _context;

        /// <summary>
        /// </summary>
        private static bool HasDeletedProperty
        {
            get
            {
                return HasPropertyNotMapped("Deleted");
            }
        }

        private static bool HasEnabledProperty
        {
            get
            {
                return HasPropertyNotMapped("Enabled");
            }
        }

        private static bool HasDatesProperties
        {
            get
            {
                var property = typeof(T).GetProperty("DateCreate");
                return property == null;
            }
        }

        public RepositoryBase(DbContext context)
        {
            this._context = context;
        }

        #region IRepository Members

        public virtual DbContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        public virtual bool HasChanges
        {
            get { return this.Context.ChangeTracker.Entries().Any(); }
        }

        public virtual T Add(T entity)
        {
            return this.DbSet.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            this.DbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.DbSet.Remove(entity);
            }
        }

        public virtual IList<T> GetAll()
        {
            return this.AsQueryable().ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="whereCondition">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual IQueryable<T> Where(Expression<Func<T, bool>> whereCondition)
        {
            return this.AsQueryable().Where(whereCondition);
        }

        /// <summary>
        /// </summary>
        /// <param name="whereCondition">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> whereCondition)
        {
            return this.DbSet.Where(whereCondition).FirstOrDefault();
        }

        public virtual T First(Expression<Func<T, bool>> whereCondition)
        {
            return this.DbSet.First(whereCondition);
        }

        /// <summary>
        /// </summary>
        /// <param name="entity">
        /// </param>
        public virtual void Attach(T entity)
        {
            this.DbSet.Attach(entity);
        }

        /// <summary>
        /// </summary>
        /// <param name="entity">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual bool Exists(T entity)
        {
            return this.DbSet.Local.Any(e => e == entity);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual IQueryable<T> AsQueryable()
        {
            return this.DbSet.AsQueryable();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual long Count()
        {
            return this.DbSet.LongCount();
        }

        /// <summary>
        /// </summary>
        /// <param name="whereCondition">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual long Count(Expression<Func<T, bool>> whereCondition)
        {
            return this.DbSet.Where(whereCondition).LongCount();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public virtual bool SaveChanges()
        {
            try
            {
                ((IObjectContextAdapter)this._context).ObjectContext.CommandTimeout = 1800;
                int result = this._context.SaveChanges();
                return 0 < result;
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new ApplicationException(sb.ToString());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="includes">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual IQueryable<T> GetIncluding(string[] includes)
        {
            IQueryable<T> q = this.DbSet;
            includes.ToList().ForEach(x => q = q.Include(x));
            return q;
        }

        /// <summary>
        /// </summary>
        /// <param name="includes">
        /// </param>
        /// <param name="whereCondition">
        /// </param>
        /// <returns>
        /// </returns>
        public virtual IQueryable<T> GetIncluding(string[] includes, Expression<Func<T, bool>> whereCondition)
        {
            var q = this.DbSet.AsQueryable().Where(whereCondition);
            includes.ToList().ForEach(x => q = q.Include(x));
            return q;
        }

        public virtual IQueryable<T> GetIncludingsLamba(params Expression<Func<T, object>>[] includes)
        {
            var query = this.DbSet.AsQueryable();
            if (includes != null)
            {

                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));

            }

            return query;
        }

        public virtual bool Save()
        {
            var result = this.SaveChanges();
            return result;
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        /// <returns>
        /// </returns>
        private static bool HasPropertyNotMapped(string propertyName)
        {
            return true;
        }

        public IQueryable<T> GetIncludingLambda(params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }

}
