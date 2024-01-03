using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class BookingPlaceResponse<T> : BaseResponse
    {
        public T BookingPlace { get; set; }
        public BookingPlaceResponse(T data) : base()
        {
            BookingPlace = data;
        }
    }

    public class BookingPlaceListResponse<T> : BaseResponse
    {
        public T BookingPlaces { get; set; }

        public BookingPlaceListResponse(T data) : base()
        {
            BookingPlaces = data;
        }

    }
}
