using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class CategoryResponse<T> : BaseResponse
    {
        public T Category { get; set; }
        public CategoryResponse(T data)
        {
            Category = data;
        }
    }

    public class CategoryeListResponse<T> : BaseResponse
    {
        public T Categories { get; set; }
        public CategoryeListResponse(T data)
        {
            Categories = data;
        }
    }
}
