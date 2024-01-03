using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Journey;
using BLL.Responses;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IJourneyService
    {
        Task<JourneyResponse<JourneyViewDto>> GetJourneyDetail(int journeyId, string languageCode);
        Task<JourneyListResponse<List<JourneyListDto>>> GetListJourney(int status, int accountId, string languageCode);
        Task<JourneyResponse<JourneyViewDto>> CreateJourney(PostJourneyDto journeyDto, string languageCode);
        Task<bool> PutJourneyStatus(int journeyId, int status);
    }
}
