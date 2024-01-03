using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class TransactionResponse<T> : BaseResponse
    {
        public T Transaction { get; set; }
        public TransactionResponse(T data)
        {
            Transaction = data;
        }
    }

    public class TransactionListResponse<T> : BaseResponse
    {
        public T Transactions { get; set; }
        public TransactionListResponse(T transactions)
        {
            Transactions = transactions;
        }
    }
}
