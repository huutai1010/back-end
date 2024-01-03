using Common.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<Transaction> GetTransactionDetail(int transactionId);
        Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters);
        Task<Transaction> GetAllTransactionDetailAsync(int transactionId);
        Task<TransactionDetail> FindByPaymentId(string paymentId);
        Task<List<Transaction>> FindByBookingIdAsync(int bookingId);
    }
}
