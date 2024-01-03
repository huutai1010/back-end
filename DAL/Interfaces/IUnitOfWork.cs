using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
