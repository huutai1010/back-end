using BLL.Responses;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class CategoryLanguageResponse<T> : BaseResponse
    {
        public T CategoryLanguage { get; set; }
        public CategoryLanguageResponse(T data)
        {
                CategoryLanguage = data;
        }
    }

    public class CategoryLanguageListResponse<T> : BaseResponse
    {
        public T CategoryLanguages { get; set; }
        public CategoryLanguageListResponse(T data)
        {
                CategoryLanguages = data;
        }
    }
}
