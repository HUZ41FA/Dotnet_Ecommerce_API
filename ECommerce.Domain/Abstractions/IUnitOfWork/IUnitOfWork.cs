using ECommerce.Domain.Abstractions.IRepository.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Abstractions.IUnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task BeginTransactionAsync();
        Task CommitChangesAsync();
        Task RollbackChangesAsync();
    }
}
