using AutoMapper;
using BLL.DTOs.Currency;
using BLL.DTOs.Place;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Tour;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Interfaces;
using Common.Models;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using static BLL.DTOs.Tour.CreateItineraryDto;

namespace BLL.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly IItineraryRepository _tourRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;

        public ItineraryService(IItineraryRepository tourRepository, IMapper mapper, IRedisCacheService redisCacheService, ILanguageRepository languageRepository)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
            _languageRepository = languageRepository;
        }

        public async Task<ToursResponse<PagedResult<ItineraryListOperation>>> GetListAsync(QueryParameters queryParameters)
        {
            var tourList = await _tourRepository.GetAsync<ItineraryListOperation>(queryParameters,includeDeleted: true, caching: false);
            foreach (var tour in tourList.Data)
            {
                tour.TotalPlace = await _tourRepository.GetTotalPlaceInTour(tour.Id);
            }

            return new ToursResponse<PagedResult<ItineraryListOperation>>(tourList);
        }

        public async Task<TourResponse<ItineraryPlaceOpDto>> GetDetailAsync(int itineraryId)
        {
            var tour = await _tourRepository.GetDetailAsync(itineraryId);

            if (tour == null)
            {
                throw new NotFoundException($"Itinerary id {itineraryId} not found!");
            }

            return new TourResponse<ItineraryPlaceOpDto>(_mapper.Map<ItineraryPlaceOpDto>(tour));
        }

        public async Task<TourResponse<ItineraryPlaceOpDto>> CreateAsync(CreateItineraryDto createTourDto)
        {
            // model validation
            var validator = new CreateTourDtoValidator();
            var validationResult = await validator.ValidateAsync(createTourDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            // map tour dto to entity
            var tourCreate = _mapper.Map<Itinerary>(createTourDto);

            // check create account id
            if (!await _tourRepository.IsValidCreateTourId(tourCreate.CreateById))
            {
                throw new NotFoundException("Account Create id not found!");
            }

            // Check for duplicate PlaceId
            bool hasDuplicatePlaceId = tourCreate.TourDetails
                .GroupBy(pd => pd.PlaceId)
                .Any(g => g.Count() > 1);

            if (hasDuplicatePlaceId)
            {
                throw new BadRequestException("Duplicate PlaceId found in the list");
            }

            // check place id is exist
            foreach (var item in tourCreate.TourDetails)
            {
                if (await _tourRepository.IsPlaceIDExist(item.PlaceId))
                {
                    throw new BadRequestException($"Place id ({item.PlaceId}) is not exist!");
                }
            };

            // check language code of tour description is exist
            foreach (var item in tourCreate.TourDescriptions)
            {
                var check = await _languageRepository.LanguageCodeIsExist(item.LanguageCode);
                if (!check)
                {
                    throw new BadRequestException($"Language code {item.LanguageCode} is not support in system!");
                }
            }

            // check language code is duplicate
            bool hasDuplicateLanguageCode = tourCreate.TourDescriptions
                .GroupBy(pd => pd.LanguageCode)
                .Any(g => g.Count() > 1);

            if (hasDuplicateLanguageCode)
            {
                throw new BadRequestException("Duplicate Language code in the Itinerary description!");
            }

            // create new tour
            var tour = await _tourRepository.CreateTourAsync(tourCreate);

            // remove cache
            if(tour != null)
            {
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPTOURS + language.LanguageCode);
                }
            }
            return new TourResponse<ItineraryPlaceOpDto>(_mapper.Map<ItineraryPlaceOpDto>(tour));
        }

        public async Task<TourResponse<ItineraryPlaceOpDto>> UpdateAsync(UpdateItineraryDto updateTourDto, int itineraryId)
        {
            // model validation
            var validator = new UpdateTourDtoValidator();
            var validationResult = await validator.ValidateAsync(updateTourDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            // mapper tour update
            var tourUpdate = _mapper.Map<Itinerary>(updateTourDto);

            // check tour id
            var tourCheck = await _tourRepository.GetDetailAsync(itineraryId);
            if (tourCheck == null)
            {
                throw new NotFoundException(nameof(Itinerary), itineraryId);
            }
            else
            {
                tourUpdate.Id = itineraryId;
                tourUpdate.CreateTime = tourCheck.CreateTime;
                tourUpdate.CreateById = tourCheck.CreateById;
                tourUpdate.Rate = tourCheck.Rate;
                _tourRepository.DetachedTourInstance(tourCheck);
            }

            // Check for duplicate PlaceId
            bool hasDuplicatePlaceId = tourUpdate.TourDetails
                .GroupBy(pd => pd.PlaceId)
                .Any(g => g.Count() > 1);

            if (hasDuplicatePlaceId)
            {
                throw new BadRequestException("Duplicate PlaceId found in the list");
            }

            // check place id is exist
            foreach (var item in tourUpdate.TourDetails)
            {
                if (await _tourRepository.IsPlaceIDExist(item.PlaceId))
                {
                    throw new BadRequestException($"Place id ({item.PlaceId}) is not exist!");
                }
            };

            foreach (var item in tourUpdate.TourDetails)
            {
                item.ItineraryId = itineraryId;
            }

            foreach (var item in tourUpdate.TourDescriptions)
            {
                item.ItineraryId = itineraryId;
            }

            // check language code of tour description is exist
            foreach (var item in tourUpdate.TourDescriptions)
            {
                var check = await _languageRepository.LanguageCodeIsExist(item.LanguageCode);
                if (!check)
                {
                    throw new BadRequestException($"Language code {item.LanguageCode} is not support in system!");
                }
            }

            // check language code is duplicate
            bool hasDuplicateLanguageCode = tourUpdate.TourDescriptions
                .GroupBy(pd => pd.LanguageCode)
                .Any(g => g.Count() > 1);

            if (hasDuplicateLanguageCode)
            {
                throw new BadRequestException("Duplicate Language code in the Itinerary description!");
            }

            // update tour 
            var tourResponse = await _tourRepository.UpdateTourAsync(tourUpdate);

            // remove cache
            if (tourResponse != null)
            {
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPTOURS + language.LanguageCode);
                }
            }
            return new TourResponse<ItineraryPlaceOpDto>(_mapper.Map<ItineraryPlaceOpDto>(tourResponse));
        }

        public async Task<bool> ChangeStatusAsync(int itineraryId)
        {
            var tour = await _tourRepository.GetDetailAsync(itineraryId);

            if (tour == null)
            {
                throw new NotFoundException();
            }

            var result = await _tourRepository.ChangeStatusAsync(itineraryId);
            if (result)
            {
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPTOURS + language.LanguageCode);
                }
            }
            return result;
        }

        public async Task<ToursResponse<List<TopTourDto>>> GetTopItinerariesAsync(string languageCode, int topCount = 10)
        {
            List<TopTourDto>? result;
            result = await _redisCacheService.Get<List<TopTourDto>>(RedisCacheKeys.TOPTOURS + languageCode);

            if (result == null || !result.Any())
            {
                var entities = await _tourRepository.GetToursAsync(languageCode, topCount);
                result = _mapper.Map<List<TopTourDto>>(entities);
                await _redisCacheService.SaveCacheAsync(RedisCacheKeys.TOPTOURS + languageCode, result);
            }

            return new ToursResponse<List<TopTourDto>>(result);
        }

        public async Task<TourResponse<ItineraryViewDto>> GetItineraryPlaceByLanguageId(int itineraryId, string languageCode)
        {
            var check = await _tourRepository.FindByIdAsync(itineraryId);

            if (check == null)
            {
                throw new NotFoundException();
            }
            var tour = await _tourRepository.GetTourDetailByLanguageId(itineraryId, languageCode);
            
            if (tour == null)
            {
                throw new NotFoundException();
            }

            var result = _mapper.Map<ItineraryViewDto>(tour);
            result.PlaceImages = _mapper.Map<List<PlaceImageDto>>(tour.TourDetails.Select(x => x.Place.PlaceImages.First()));

            List<CurrencyDto> exchangesData;

#if DEBUG
            var now = DateTime.Now.Millisecond;
            exchangesData = new List<CurrencyDto>{
                new CurrencyDto
                {
Timestamp = now,
Currency = "CNH", Value = 7.3101,
                },
                new CurrencyDto
                {
Timestamp = now,
Currency = "USD", Value = 1,
                },new CurrencyDto
                {
Timestamp = now,
Currency = "JPY", Value = 148.836175,
                },
                new CurrencyDto
                {
Timestamp = now,
Currency = "VND", Value = 24375,
                },
            };
#else
            exchangesData = await _redisCacheService.Get<List<CurrencyDto>>("currency");
#endif

            result.Exchanges = new Dictionary<string, double>();
            exchangesData!.ForEach(item =>
            {
                var exchangePrice = Math.Round(item.Value * decimal.ToDouble(result.Price), 3);
                result.Exchanges.Add(item.Currency, exchangePrice);
            });

            return new TourResponse<ItineraryViewDto>(result);
        }

        public async Task<ToursResponse<PagedResult<TopTourDto>>> GetItinerariesAsync(string languageCode, QueryParameters queryParameters)
        {
            var entities = await _tourRepository.GetAsyncWithConditions<TopTourDto>(queryParameters, includeDeleted: false, queryConditions: query =>
            {
                return query
                .Include(q => q.TourDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(q =>q.FeedBacks)
                .Where(x => x.TourDescriptions.Any(x => x.LanguageCode == languageCode));
            });
            var result = _mapper.Map<PagedResult<TopTourDto>>(entities);
            return new ToursResponse<PagedResult<TopTourDto>>(result);
        }
        public async Task<TourResponse<TopTourDto>> GetDetailByIdAsync(int itineraryId, string languageCode)
        {
            var tour = await _tourRepository.GetDetailByIdAsync(itineraryId, languageCode);

            if (tour == null)
            {
                throw new NotFoundException($"Itinerary not found!");
            }
            return new TourResponse<TopTourDto>(_mapper.Map<TopTourDto>(tour));
        }

    }
}
