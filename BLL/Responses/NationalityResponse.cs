using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class NationalityResponse<T> : BaseResponse
    {
        public T Nationality { get; set; }
        public NationalityResponse(T nationality) : base()
        {
            Nationality = nationality;
        }
    }

    public class NationalitiesResponse<T> : BaseResponse
    {
        public T Nationalities { get; set; }

        public NationalitiesResponse(T nationalities) : base()
        {
            Nationalities = nationalities;
        }
    }
}
