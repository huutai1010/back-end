using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class AccountResponse<T> : BaseResponse
    {
        public T Account { get; set; }
        public AccountResponse(T data) : base()
        {
            Account = data;
        }
    }

    public class AccountListResponse<T> : BaseResponse
    {
        public T Accounts { get; set; }

        public AccountListResponse(T data) : base()
        {
            Accounts = data;
        }

    }

    public class StatusResposne : BaseResponse
    {
        public string Message { get; set; }

        public StatusResposne(string message) : base()
        {
            Message = message;
        }
    }
}
