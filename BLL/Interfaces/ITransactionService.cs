using BLL.DTOs.Tour;
using BLL.DTOs.Transaction;
using BLL.Responses;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionListResponse<PagedResult<TransactionViewDto>>> GetListTransaction(QueryParameters queryParameters,int accountId);
        Task<TransactionResponse<TransactionDetailViewDto>> GetTransactionDetail(int transactionId);

        #region operator
        Task<TransactionListResponse<PagedResult<TransactionListDto>>> GetListAsync(QueryParameters queryParameters);
        Task<TransactionResponse<TransactionDetailDto>> GetTransactionDetailAsync(int transactionId);
        #endregion
    }
}
