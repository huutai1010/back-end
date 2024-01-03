using AutoMapper;
using BLL.DTOs.Feedback;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Models;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IMapper _mapper;
        private readonly IItineraryRepository _tourRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJourneyRepository _journeyRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper, IPlaceRepository placeRepository, IItineraryRepository tourRepository, IUnitOfWork unitOfWork, IJourneyRepository journeyRepository)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
            _placeRepository = placeRepository;
            _tourRepository = tourRepository;
            _unitOfWork = unitOfWork;
            _journeyRepository = journeyRepository;
        }
        public async Task PostFeedbacks(AddFeedbackDto addFeedbackDto, int userId)
        {
            var feedbacks = new List<FeedBack>();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (addFeedbackDto.TourId != null && addFeedbackDto.TourId > 0)
                    {
                        var tourExist = await _tourRepository.Exist(addFeedbackDto.TourId);

                        if (!tourExist)
                        {
                            throw new NotFoundException($"Itinerary id {addFeedbackDto.TourId} not found!");
                        }
                        var tourFeedbackEntity = _mapper.Map<FeedBack>(addFeedbackDto);
                        tourFeedbackEntity.AccountId = userId;
                        feedbacks.Add(tourFeedbackEntity);
                        var tourRate = await _feedbackRepository.GetFeedbackAvg(addFeedbackDto.TourId ?? 0, false);
                        tourRate.Add(addFeedbackDto.TourRate ?? 0);
                        var newAvg = tourRate.Average();
                        var tourRateEntities = await _tourRepository.GetRateTour(addFeedbackDto.TourId);

                        tourRateEntities.Rate = Math.Round(newAvg, 1);
                        await _tourRepository.UpdateAsync(tourRateEntities);
                    }
                    else
                    {
                        if (addFeedbackDto.Places == null || !addFeedbackDto.Places.Any())
                        {
                            throw new BadRequestException("Places not populated!");
                        }
                        var placeFeedbackEntities = addFeedbackDto.Places.Select(placeFeedback =>
                        {
                            var entity = _mapper.Map<FeedBack>(placeFeedback);
                            entity.AccountId = userId;
                            return entity;
                        });
                        feedbacks.AddRange(placeFeedbackEntities);

                        foreach (var place in addFeedbackDto.Places)
                        {
                            var placeRate = await _feedbackRepository.GetFeedbackAvg(place.PlaceId ?? 0, true);
                            placeRate.Add(place.PlaceRate ?? 0);
                            var newRatePlaceAvg = placeRate.Average();
                            var placeRateEntities = await _placeRepository.GetRatePlace(place!.PlaceId!);
                            placeRateEntities.Rate = Math.Round(newRatePlaceAvg, 1);

                            await _placeRepository.UpdateAsync(placeRateEntities);

                        }
                    }
                    await _feedbackRepository.CreateManyAsync(feedbacks.ToArray());
                    var journeyEntitiesUpdate = await _journeyRepository.FindByIdAsync(addFeedbackDto.JourneyId);
                    if (journeyEntitiesUpdate == null)
                    {
                        throw new NotFoundException(nameof(Journey), addFeedbackDto.JourneyId);
                    }
                    journeyEntitiesUpdate.Status = (int)JourneyStatus.Completed;
                    await _journeyRepository.UpdateAsync(journeyEntitiesUpdate);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task<FeedbackListResponse<PagedResult<FeedbackListDto>>> GetFeedbacksAsync(QueryParameters queryParameters, int id, bool isplace)
        {
            var feedbacks = await _feedbackRepository.GetFeedbackAsync<FeedbackListDto>(queryParameters, id, isplace);

            return new FeedbackListResponse<PagedResult<FeedbackListDto>>(feedbacks);
        }

        public async Task<FeedbackResponse<FeedbackDetailDto>> GetFeedbackDetailAsync(int id)
        {
            var feedback = await _feedbackRepository.GetFeedbackDetailById(id);
            if (feedback is null)
            {
                throw new NotFoundException();
            }

            var feedbackResponse = _mapper.Map<FeedbackDetailDto>(feedback);

            return new FeedbackResponse<FeedbackDetailDto>(feedbackResponse);
        } 

        public async Task<FeedbackResponse<FeedbackListDto>> PutFeedback(int id, FeedbackListDto feedbackListDto)
        {
            var entity = await _feedbackRepository.FindByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Feedback '{id}' not found.");
            }
            feedbackListDto.CreateTime = entity.CreateTime;
            _mapper.Map(feedbackListDto, entity);
            entity.UpdateTime = DateTime.Now;
            entity.Id = id;
            await _feedbackRepository.UpdateAsync(entity);

            return new FeedbackResponse<FeedbackListDto>(_mapper.Map<FeedbackListDto>(entity));
        }
        public async Task<FeedbackResponse<FeedbackListDto>> DeleteFeedback(int id)
        {
            var entity = await _feedbackRepository.FindByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"Feedback '{id}' not found.");
            }
            entity.Status = 0;

            entity.UpdateTime = DateTime.Now;
            await _feedbackRepository.UpdateAsync(entity);

            return new FeedbackResponse<FeedbackListDto>(_mapper.Map<FeedbackListDto>(entity));
        }



        public async Task<bool> ChangeStatusFeedback(int feedbackId)
        {
            bool check = false;
            var isExist = await _feedbackRepository.IsExistFeedback(feedbackId);
            if (isExist)
            {
                var isSuccess = await _feedbackRepository.ChangeStatusFeedback(feedbackId);
                if(isSuccess)
                {
                    check = true;
                }
            }
            return check;
        }

        public async Task<FeedbackListResponse<PagedResult<FeedbackListViewDto>>> GetFeedbackViewList(QueryParameters queryParameters)
        {
            var task = await _feedbackRepository.GetAsyncWithConditions<FeedbackListViewDto>(queryParameters, includeDeleted : true, queryConditions: query =>
            {
                return query
                    .Include(x => x.Account)
                    .ThenInclude(x => x.NationalCodeNavigation)
                    .OrderByDescending(x => x.CreateTime);
            });
            return new FeedbackListResponse<PagedResult<FeedbackListViewDto>>(task);
        }

        public async Task<FeedbackListResponse<PagedResult<FeedbacksDto>>> GetFeedbackDto(QueryParameters queryParameters, int id, bool isPlace = false)
        {
            if (isPlace)
            {
                var isPlaceExist = await _placeRepository.IsPlaceExist(id);
                if (!isPlaceExist)
                {
                    throw new NotFoundException($"Place id {id} is not exist!");
                }

                var task = await _feedbackRepository.GetAsyncWithConditions<FeedbacksDto>(queryParameters, includeDeleted: true, queryConditions: query =>
                {
                    return query
                        .Where(x => x.PlaceId == id)
                        .Include(x => x.Account)
                        .OrderByDescending(x => x.CreateTime);
                });
                return new FeedbackListResponse<PagedResult<FeedbacksDto>>(task);
            }
            else
            {
                var isTour = await _tourRepository.IsTourExist(id);
                if (!isTour)
                {
                    throw new NotFoundException($"Tour id {id} is not exist!");
                }

                var task = await _feedbackRepository.GetAsyncWithConditions<FeedbacksDto>(queryParameters, includeDeleted: true, queryConditions: query =>
                {
                    return query
                        .Where(x => x.ItineraryId == id)
                        .Include(x => x.Account)
                        .OrderByDescending(x => x.CreateTime);
                });
                return new FeedbackListResponse<PagedResult<FeedbacksDto>>(task);
            }
        }
    }
}
