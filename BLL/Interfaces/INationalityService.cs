using BLL.DTOs.Nationality;
using BLL.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface INationalityService
    {
        Task<NationalitiesResponse<List<NationalityListDto>>> GetNationalitiesForAdmin();
        Task<NationalitiesResponse<List<NationalityListDto>>> GetNationalities();
    }
}
