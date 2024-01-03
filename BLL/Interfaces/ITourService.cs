using BLL.DTOs.Tour;
using BLL.Responses;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITourService
    {
        Task<ToursResponse<PagedResult<TourListOperation>>> GetListAsync(QueryParameters queryParameters);
        Task<TourResponse<TourDetailOpDto>> GetDetailAsync(int tourId);
        Task<TourResponse<TourDetailOpDto>> CreateAsync(CreateTourDto createTourDto);
        Task<TourResponse<TourDetailOpDto>> UpdateAsync(UpdateTourDto updateTourDto, int tourId);
        Task<bool> ChangeStatusAsync(int tourId);
        Task<ToursResponse<List<TopTourDto>>> GetTopToursAsync(string languageCode, int topCount);

        Task<TourResponse<TourViewDto>> GetTourDetailByLanguageId(int tourid, string languageCode);
        Task<ToursResponse<PagedResult<TopTourDto>>> GetToursAsync(string languageCode,QueryParameters queryParameters);
        Task<TourResponse<TopTourDto>> GetDetailByIdAsync(int tourId, string languageCode);
    }
}
