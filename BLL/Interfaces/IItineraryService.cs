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
    public interface IItineraryService
    {
        Task<ToursResponse<PagedResult<ItineraryListOperation>>> GetListAsync(QueryParameters queryParameters);
        Task<TourResponse<ItineraryPlaceOpDto>> GetDetailAsync(int  itineraryId);
        Task<TourResponse<ItineraryPlaceOpDto>> CreateAsync(CreateItineraryDto createTourDto);
        Task<TourResponse<ItineraryPlaceOpDto>> UpdateAsync(UpdateItineraryDto updateTourDto, int  itineraryId);
        Task<bool> ChangeStatusAsync(int  itineraryId);
        Task<ToursResponse<List<TopTourDto>>> GetTopItinerariesAsync(string languageCode, int topCount);

        Task<TourResponse<ItineraryViewDto>> GetItineraryPlaceByLanguageId(int  itineraryId, string languageCode);
        Task<ToursResponse<PagedResult<TopTourDto>>> GetItinerariesAsync(string languageCode,QueryParameters queryParameters);
        Task<TourResponse<TopTourDto>> GetDetailByIdAsync(int  itineraryId, string languageCode);
    }
}
