using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.Shared.Contracts
{
    public interface IUnitOfWork
    {
        void Register(IRepository repository);
        void Commit();
    }
}
