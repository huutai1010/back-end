using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class PlaceItemResponse<T> : BaseResponse
    {
        public T PlaceItem { get; set; }
        public PlaceItemResponse(T data)
        {
            PlaceItem = data;
        }
    }

    public class PlaceItemsResponse<T> : BaseResponse
    {
        public T PlaceItems { get; set; }
        public PlaceItemsResponse(T data)
        {
            PlaceItems = data;
        }
    }
}
