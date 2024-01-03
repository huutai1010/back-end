using AutoMapper;
using BLL.DTOs.Journey;
using BLL.DTOs.Tour;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly IBookingPlaceRepository _bookingDetailRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JourneyService(IJourneyRepository journeyRepository, IMapper mapper, IBookingPlaceRepository bookingPlaceRepository ,IUnitOfWork unitOfWork,IBookingRepository bookingRepository, IPlaceRepository placeRepository)
        {
            _journeyRepository = journeyRepository;
            _mapper = mapper;
            _bookingDetailRepository = bookingPlaceRepository;
            _placeRepository = placeRepository;
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<JourneyListResponse<List<JourneyListDto>>> GetListJourney(int status, int accountId,string languageCode)
        {

            var result = await _bookingRepository.GetHistoryJourney(accountId);
            var listJourneys = new List<int>();
            foreach (var entity in result)
            {
                var listJourneyIds = entity.BookingPlaces
                    .Where(x => x.JourneyId != null)
                    .Select(x => (int)x.JourneyId!)
                .ToList();
                listJourneys.AddRange(listJourneyIds.ToList());

            }
            listJourneys = listJourneys.Distinct().ToList(); ;
            
            var journeys = await _journeyRepository.GetJourneyByBookingId(status, listJourneys, languageCode);
            var journeyListDto = new List<JourneyListDto>();
            foreach (var journey in journeys)
            {
                var dto = _mapper.Map<JourneyListDto>(journey);
                var firstPlace = journey.BookingPlaces.OrderBy(x => x.Ordinal).First();
                var lastPlace = journey.BookingPlaces.OrderBy(x => x.Ordinal).Last();
                if (firstPlace.Place.PlaceDescriptions.FirstOrDefault(x=>x.LanguageCode.Trim() == languageCode) == null)
                {
                   dto.FirstPlaceName = firstPlace.Place.Name;
                }
                if (lastPlace.Place.PlaceDescriptions.FirstOrDefault(x => x.LanguageCode.Trim() == languageCode) == null)
                {
                    dto.LastPlaceName = lastPlace.Place.Name;
                }

                journeyListDto.Add(dto);
            }
            return new JourneyListResponse<List<JourneyListDto>>(journeyListDto);
        }


        public async Task<JourneyResponse<JourneyViewDto>> CreateJourney(PostJourneyDto journeyDto, string languageCode)
        {
            var validator = new PostJourneyDtoValidator();
            var validationResult = await validator.ValidateAsync(journeyDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
            var journeyEntity = _mapper.Map<Journey>(journeyDto);
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var check = await _journeyRepository.PostJourney(journeyEntity);
                    if (check != null)
                    {
                        foreach (var entity in journeyDto.JourneyDetails)
                        {
                            var bookingDetailEntity = await _bookingDetailRepository.FindByIdAsync(entity.BookingDetailId);
                            bookingDetailEntity!.JourneyId = check.Id;
                            bookingDetailEntity.IsJourney = true;
                            bookingDetailEntity.Ordinal = entity.Ordinal;
                            bookingDetailEntity.Status = (int)BookingPlaceStatus.Future;
                            await _bookingDetailRepository.UpdateAsync(bookingDetailEntity);
                        }

                    }
                    await transaction.CommitAsync();
                    return new JourneyResponse<JourneyViewDto>(_mapper.Map<JourneyViewDto>(check));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<JourneyResponse<JourneyViewDto>> GetJourneyDetail(int journeyId, string languageCode)
        {
            var check = await _journeyRepository.Exist(journeyId);
            if (check == false)
            {
                throw new NotFoundException("Journey Not Found!");
            }
            var journeyEntity = await _journeyRepository.FindJourneyById(journeyId);
           
            int bookingId = journeyEntity.BookingPlaces.Select(x => x.BookingId).FirstOrDefault();
            var bookingEntities = await _bookingRepository.FindByIdAsync(bookingId);
           var journeyViewDto = _mapper.Map<JourneyViewDto>(journeyEntity);

            foreach (var entity in journeyViewDto.BookingPlaces)
            {
                var checkName = await _placeRepository.GetPlaceNameByLanguageCode(entity.PlaceId, languageCode);
                if (checkName != null)
                {
                    if (checkName.Status == (int)PlaceDescStatus.active)
                    {
                        entity.PlaceName = checkName.Name;
                        entity.IsSupport = true;
                    }
                    else
                    {
                        entity.PlaceName = checkName.Name;
                        entity.IsSupport = false;
                    }
                }
                else
                {
                    entity.PlaceName = entity.DefaultName;
                    entity.IsSupport = false;
                }
            }
            int? tourId = bookingEntities?.ItineraryId;
                journeyViewDto.TourId = tourId;
            return new JourneyResponse<JourneyViewDto>(journeyViewDto);
        }

        public async Task<bool> PutJourneyStatus(int journeyId, int status)
        {
            var task = await _journeyRepository.PutJourneyStatus(journeyId, status);
            if (!task)
            {
                throw new NotFoundException("Journey Id Not Found!");
            }
            return true;
        }
    }
}
