using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CompanyName.Shared.Contracts
{
    public interface IRepository<TEntity> : IRepository where TEntity : class
    {
        Task<TEntity> GetByID(object id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
        void Delete(TEntity entityToDelete);
        Task<IQueryable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
    }

    public interface IRepository
    {
        void Submit();
    }
}