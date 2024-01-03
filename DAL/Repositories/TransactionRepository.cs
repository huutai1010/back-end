using AutoMapper;
using Common.Interfaces;
using Common.Models;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransactionRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
            _redisCacheService= redisCacheService;
        }

        public async Task<Transaction> GetTransactionDetail(int transactionId)
        {
            var result = await _context.Transactions
                .Include(q => q.Booking)
                .ThenInclude(q => q.BookingPlaces)
                .Include(x => x.TransactionDetails.OrderByDescending(o => o.UpdateTime).ThenByDescending(o => o.CreateTime))
                .FirstOrDefaultAsync(x => x.Id == transactionId);
            return result;
        }

        public async Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters)
        {
            List<Transaction>? items = await _context.Transactions
                .Include(x => x.Account)
                .Include(x => x.Booking)
                .IgnoreQueryFilters()
                .OrderByDescending(x => x.CreateTime)
                .ToListAsync();
            
            var result = _mapper.Map<List<T>>(items);
            var totalSize = items.Count;
            var pageCount = (double)totalSize / queryParameters.PageSize;
            return new PagedResult<T>
            {
                Data = result
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }


        public async Task<Transaction> GetAllTransactionDetailAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(x => x.Account)
                .Include(x => x.Booking)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == transactionId);

            if(transaction is null)
            {
                throw new DllNotFoundException("transaction id not found!");
            }
            return transaction;
        }

        public async Task<TransactionDetail> FindByPaymentId(string paymentId)
        {
            var result = await _context.TransactionDetails.Include(q => q.Transaction)
                .ThenInclude(q => q.Booking)
                .ThenInclude(q => q.BookingPlaces)
                .FirstAsync(x => x.PaymentId == paymentId);

            return result;
        }

        public async Task<List<Transaction>> FindByBookingIdAsync(int bookingId)
        {
            return await _context.Transactions
                .Where(x => x.BookingId == bookingId).ToListAsync();
        }
    }
}
