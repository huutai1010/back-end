using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class LanguageResponse<T> : BaseResponse
    {
        public T Language { get; set; }

        public LanguageResponse(T language) : base()
        {
            Language = language;
        }
    }

    public class LanguageListResponse<T> : BaseResponse
    {
        public T Languages { get; set; }
        public LanguageListResponse(T data) : base()
        {
            Languages = data;
        }
    }
}
