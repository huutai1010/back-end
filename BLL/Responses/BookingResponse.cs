using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class BookingResponse<T> : BaseResponse
    {
        public T Booking { get; set; }
        public BookingResponse(T data) : base()
        {
            Booking = data;
        }
    }

    public class BookingListResponse<T> : BaseResponse
    {
        public T Bookings { get; set; }

        public BookingListResponse(T data) : base()
        {
            Bookings = data;
        }

    }
}
