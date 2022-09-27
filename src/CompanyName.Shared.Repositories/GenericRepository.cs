using Microsoft.EntityFrameworkCore;
using LuxExcel.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LuxExcel.Shared.Repositories
{
    public class GenericRepository<TContext,TEntity> : IRepository<TEntity>, IDisposable where TEntity : class where TContext : DbContext
    {
        internal TContext context;
        internal DbSet<TEntity> dbSet;
        private bool _disposed = false;

        public GenericRepository(TContext context, IUnitOfWork unitOfWork)
        {
            unitOfWork.Register(this);
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        //public IQueryable<TEntity> Query => dbSet;

        public async virtual Task<IQueryable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await Task.FromResult(orderBy(query));
            }
            else
            {
                return await Task.FromResult(query);
            }
        }

        public async virtual Task<TEntity> GetByID(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public async virtual void Insert(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async virtual void Delete(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async void Submit()
        {
            await context.SaveChangesAsync();
        }
    }
}
