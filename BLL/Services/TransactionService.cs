using AutoMapper;
using BLL.DTOs.Transaction;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Interfaces;
using Common.Models;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository,
            IMapper mapper,
            IPaypalService paypalService,
            ITransactionDetailRepository transactionDetailRepository,
            IRedisCacheService redisCacheService,
            IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }


        public async Task<TransactionListResponse<PagedResult<TransactionViewDto>>> GetListTransaction(QueryParameters queryParameters,int accountId)
        {
            var result = await _transactionRepository.GetAsyncWithConditions<TransactionViewDto>(queryParameters,
                queryConditions: query => query.Where(x => x.AccountId == accountId).OrderByDescending(x => x.Id));
            result.Data = result.Data.Select(x =>
            {
                x.StatusName = ((TransactionStatus)Enum.ToObject(
                    typeof(TransactionStatus),
                    x.Status)).ToString();
                return x;
            }).ToList();
            return new TransactionListResponse<PagedResult<TransactionViewDto>>(result);
        }

        public async Task<TransactionResponse<TransactionDetailViewDto>> GetTransactionDetail(int transactionId)
        {

            var transactionEntity = await _transactionRepository.GetTransactionDetail(transactionId);

            if (transactionEntity == null)
            {
                throw new NotFoundException($"Transaction id {transactionId} not found!");
            }
            var result = _mapper.Map<TransactionDetailViewDto>(transactionEntity);
            return new TransactionResponse<TransactionDetailViewDto>(result);
        }

        public async Task<TransactionListResponse<PagedResult<TransactionListDto>>> GetListAsync(QueryParameters queryParameters)
        {
           // var transactions = await _transactionRepository.GetListAsync<TransactionListDto>(queryParameters);
            var transactions = await _transactionRepository.GetAsyncWithConditions<TransactionListDto>(queryParameters, includeDeleted: true, queryConditions: query =>
            {
                return query
                        .Include(x => x.Account)
                        .Include(x => x.Booking)
                        .OrderByDescending(x => x.CreateTime);
            });
            return new TransactionListResponse<PagedResult<TransactionListDto>>(transactions);
        }

        public async Task<TransactionResponse<TransactionDetailDto>> GetTransactionDetailAsync(int transactionId)
        {
            var result = await _transactionRepository.GetAllTransactionDetailAsync(transactionId);
            return new TransactionResponse<TransactionDetailDto>(_mapper.Map<TransactionDetailDto>(result));
        }

        

    }
}
