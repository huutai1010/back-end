using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class PlaceResponse<T> : BaseResponse
    {
        public T Place { get; set; }
        public PlaceResponse(T data)
        {
            Place = data;
        }
    }

    public class PlaceListResponse<T> : BaseResponse
    {
        public T Places { get; set; }
        public PlaceListResponse(T data)
        {
            Places = data;
        }
    }
}
