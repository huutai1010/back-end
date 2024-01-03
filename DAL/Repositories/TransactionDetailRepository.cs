using AutoMapper;

using Common.Interfaces;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TransactionDetailRepository : BaseRepository<TransactionDetail>, ITransactionDetailRepository
    {
        public TransactionDetailRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService cacheService) : base(context, mapper, unitOfWork, cacheService)
        {
        }
    }
}
