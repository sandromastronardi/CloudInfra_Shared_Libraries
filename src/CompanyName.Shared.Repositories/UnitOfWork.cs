using Microsoft.EntityFrameworkCore;
using LuxExcel.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuxExcel.Shared.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private TContext context = null;// new DeviceContext();
        private readonly Dictionary<string, IRepository> _repositories;

        public UnitOfWork(TContext context)
        {
            this.context = context;
            _repositories = new Dictionary<string, IRepository>();
        }

        public void Register(IRepository repository)
        {
            _repositories.Add(repository.GetType().FullName, repository);
        }

        public void Commit()
        {
            _repositories.ToList().ForEach(x => x.Value.Submit());
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
