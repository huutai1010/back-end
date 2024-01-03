using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class TourResponse<T> : BaseResponse
    {
        public T Tour { get; set; }
        public TourResponse(T tour) : base()
        {
            Tour = tour;
        }
    }

    public class ToursResponse<T> : BaseResponse
    {
        public T Tours { get; set; }

        public ToursResponse(T tours) : base()
        {
            Tours = tours;
        }
    }
}
