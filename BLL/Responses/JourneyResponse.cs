using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class JourneyResponse<T> : BaseResponse
    {
        public T Journeys { get; set; }

        public JourneyResponse(T journey) : base()
        {
            Journeys = journey;
        }
    }

    public class JourneyListResponse<T> : BaseResponse
    {
        public T Journeys { get; set; }

        public JourneyListResponse(T journeys) : base()
        {
            Journeys = journeys;
        }
    }

}
