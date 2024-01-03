using AutoMapper;

using BLL.DTOs;
using BLL.DTOs.Booking;
using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Journey;
using BLL.DTOs.Place;
using BLL.DTOs.Place.CelebratedPlace;
using BLL.DTOs.Transaction;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Common.AppConfiguration;
using Common.Constants;
using Common.Interfaces;
using Common.Models;
using Common.Models.Paypal;
using Common.Services;

using DAL.Entities;
using DAL.Interfaces;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;

namespace BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingPlaceRepository _bookingDetailRepository;
        private readonly IItineraryRepository _tourRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly ITransactionDetailRepository _transactionDetailRepository;
        private readonly IPaypalService _paypalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        IFirebaseStorageService _firebaseStorageService;
        private readonly IAccountService _accountService;
        private readonly PaypalSettings _paypalSettings;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository,
            IMapper mapper,
            IBookingPlaceRepository bookingDetailRepository,
            IItineraryRepository tourRepository,
            IPlaceRepository placeRepository,
            ITransactionRepository transactionRepository,
            IJourneyRepository journeyRepository,
            ITransactionDetailRepository transactionDetailRepository,
            IPaypalService paypalService,
            IFirebaseStorageService firebaseStorageService,
            IUnitOfWork unitOfWork,
            IRedisCacheService redisCacheService,
            IAccountService accountService,
            IOptions<PaypalSettings> paypalSettings)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _bookingDetailRepository = bookingDetailRepository;
            _tourRepository = tourRepository;
            _placeRepository = placeRepository;
            _transactionRepository = transactionRepository;
            _journeyRepository = journeyRepository;
            _transactionDetailRepository = transactionDetailRepository;
            _paypalService = paypalService;
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
            _accountService = accountService;
            _paypalSettings = paypalSettings.Value;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<PlaceListResponse<List<CelebratedPlaceDto>>> GetCelebratedPlace(int journeyId, string languageCode)
        {
            var check = await _journeyRepository.FindByIdAsync(journeyId);
            if (check == null)
            {
                throw new NotFoundException("Journey not found!");
            }
            var bookingDetailList = await _bookingDetailRepository.GetCelebratedPlaceById(journeyId, languageCode);
            var dto = new List<CelebratedPlaceDto>();
            foreach (var item in bookingDetailList)
            {
                var celebratedPlaceDto = _mapper.Map<CelebratedPlaceDto>(item);
                if (celebratedPlaceDto.PlaceName == null)
                {
                    celebratedPlaceDto.PlaceName = item.Place.Name;
                }
                dto.Add(celebratedPlaceDto);
            }
            return new PlaceListResponse<List<CelebratedPlaceDto>>(dto);
        }
        public async Task PostCelebratedPlace([FromRoute] int bookingDetailId, List<IFormFile> imageFiles)
        {
            List<CelebratedImageDto> celebratedImageDto = new();

            bool isPrimary = true;
            foreach (IFormFile imageFile in imageFiles)
            {
                using var stream = new MemoryStream();
                await imageFile.CopyToAsync(stream);
                stream.Position = 0;
                var link = await _firebaseStorageService.UploadImageFirebase(stream, "Celebrate", imageFile.FileName);
                celebratedImageDto.Add(new CelebratedImageDto { ImageUrl = link, IsPrimary = isPrimary });
                isPrimary = false;
            }

            var celebrateEntity = _mapper.Map<List<CelebrateImage>>(celebratedImageDto);

            var bookingDetailEntity = await _bookingDetailRepository.FindByIdAsync(bookingDetailId);

            if (bookingDetailEntity == null)
            {
                throw new NotFoundException();
            }

            foreach (var entity in celebrateEntity)
            {
                bookingDetailEntity.CelebrateImages.Add(entity);
            }

            await _bookingDetailRepository.UpdateAsync(bookingDetailEntity);
        }

        #region Transaction operator
        public async Task<BookingListResponse<PagedResult<BookingListDto>>> GetListAsync(QueryParameters parameters)
        {
            var boookings = await _bookingRepository.GetAllBookingAsync<BookingListDto>(parameters);
            return new BookingListResponse<PagedResult<BookingListDto>>(boookings);
        }

        public async Task<BookingResponse<BookingDetailDto>> GetDetailAsync(int bookingId)
        {
            var isExist = await _bookingRepository.IsBookingExist(bookingId);
            if (isExist == false)
            {
                throw new NotFoundException("booking id not found!");
            }

            var booking = await _bookingRepository.GetBookingDetailAsync(bookingId);
            var response = _mapper.Map<BookingDetailDto>(booking);

            if (booking.ItineraryId != null)
            {
                var tour = await _tourRepository.GetDetailAsync((int)booking.ItineraryId);
                if (tour != null)
                {
                    response.TourCreateDate = tour.CreateTime;
                    response.TourCreatetor = tour.CreateBy.Email;
                }
            }

            TimeSpan sumTime = new();
            foreach (var item in booking.BookingPlaces)
            {
                if (item.Place.Hour != null)
                {
                    sumTime = sumTime.Add((TimeSpan)item.Place.Hour);
                }
            }
            response.DurationExpected = sumTime;

            return new BookingResponse<BookingDetailDto>(response);
        }

        public async Task<OrderStaticticalResponse<List<OrderViewOpDto>>> GetOrderCustomerAsync()
        {
            var result = await _bookingRepository.GetTopCustomerOrder();
            var items = _mapper.Map<List<OrderViewOpDto>>(result);
            return new OrderStaticticalResponse<List<OrderViewOpDto>>(items);
        }
        #endregion

        public async Task<BookingListResponse<PagedResult<HistoryBookingViewDto>>> GetHistoryBooking(QueryParameters queryParameters, int accountId, string languageCode)
        {
            var result = await _bookingRepository.GetHistoryBooking(queryParameters, accountId, languageCode);

            var bookingDtos = _mapper.Map<List<HistoryBookingViewDto>>(result.Data);

            bookingDtos = bookingDtos.Select(booking =>
            {
                booking.StatusName = ((BookingStatus)Enum.ToObject(typeof(BookingStatus), booking.Status)).ToString();
                var placeImages = result.Data.First(x => x.Id == booking.Id).BookingPlaces.Select(x => x.Place).SelectMany(x => x.PlaceImages).ToList();
                booking.PlaceImages = _mapper.Map<List<PlaceImageViewDto>>(placeImages);
                return booking;
            }).ToList();

            var dtoResult = new PagedResult<HistoryBookingViewDto>()
            {
                Data = bookingDtos,
                PageCount = result.PageCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
            };
            return new BookingListResponse<PagedResult<HistoryBookingViewDto>>(dtoResult);
        }

        public async Task<BookingPlaceListResponse<List<JourneyPlaceViewDto>>> GetHistoryBookingPlace(bool isJourney, string languageCode, int accountId)
        {
            var result = await _bookingRepository.GetHistoryBookingPlace(isJourney, languageCode, accountId);
            var allPlaces = new List<JourneyPlaceViewDto>();
            foreach (var entity in result)
            {

                var placesDto = _mapper.Map<List<JourneyPlaceViewDto>>(entity.BookingPlaces);
                allPlaces.AddRange(placesDto);
            }

            return new BookingPlaceListResponse<List<JourneyPlaceViewDto>>(allPlaces);
        }

        public async Task<CheckInPlaceResultDto> CheckInPlace(int bookingDetailId, bool isFinish)
        {
            var bookingDetailEntity = await _bookingDetailRepository.FindByIdAsync(bookingDetailId);

            if (bookingDetailEntity == null)
            {
                throw new NotFoundException();
            }

            if (isFinish == true)
            {
                var journeyEntity = await _journeyRepository.FindByIdAsync(bookingDetailEntity.JourneyId);
                if (journeyEntity == null)
                {
                    throw new NotFoundException();
                }
                journeyEntity.Status = (int)JourneyStatus.WaitRating;
                await _journeyRepository.UpdateAsync(journeyEntity);
            }
            bookingDetailEntity.Status = (int)BookingPlaceStatus.Completed;
            var now = DateTime.Now;
            bookingDetailEntity.StartTime = now;
            bookingDetailEntity.ExpiredTime = now.AddDays(7);

            await _bookingDetailRepository.UpdateAsync(bookingDetailEntity);

            return new CheckInPlaceResultDto
            {
                BookingId = bookingDetailEntity.BookingId,
                BookingPlaceId = bookingDetailEntity.Id,
                PlaceId = bookingDetailEntity.PlaceId,
                StartTime = bookingDetailEntity.StartTime,
                ExpiredTime = bookingDetailEntity.ExpiredTime
            };

        }

        public async Task<Link> CreateBookingAsync(int userId, PostBookingDto bookingDto)
        {
            var validator = new PostBookingDtoValidator();
            var validationResult = await validator.ValidateAsync(bookingDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult);
            }
            Booking? bookingEntity;
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (bookingDto.IsExistingTour)
                    {
                        var tour = await _tourRepository.GetBookingTour(bookingDto.TourId ?? throw new BadRequestException("Itinerary Id can't be blank!"));
                        if (tour == null)
                        {
                            throw new NotFoundException("Itinerary not found!");
                        }

                        bookingEntity = _mapper.Map<Booking>(tour);
                        bookingEntity.AccountId = userId;

                        bookingEntity.BookingPlaces = bookingEntity.BookingPlaces.Select(x =>
                        {
                            x.IsJourney = true;
                            x.Status = 0;
                            x.Ordinal = bookingDto.BookingPlaces.First(y => y.PlaceId == x.PlaceId).Ordinal;
                            return x;
                        }).ToList();
                        await _bookingRepository.CreateAsync(bookingEntity);
                    }
                    else
                    {
                        var placeIds = bookingDto.BookingPlaces.Select(x => x.PlaceId).ToList();
                        var places = await _placeRepository.GetListMultiplePlacesByListIds(placeIds);
                        var bookingDetails = _mapper.Map<List<BookingPlace>>(places);
                        bookingEntity = _mapper.Map<Booking>(bookingDto);
                        bookingEntity.AccountId = userId;
                        bookingEntity.BookingPlaces = bookingDetails.Select(detail =>
                        {
                            detail.IsJourney = false;
                            detail.Ordinal = bookingDto.BookingPlaces.First(x => x.PlaceId == detail.PlaceId).Ordinal;
                            return detail;
                        }).ToList();
                        bookingEntity.IsPrepared = false;
                        bookingEntity.Total = places.Sum(x => x.Price ?? 0);
                        await _bookingRepository.CreateAsync(bookingEntity);
                    }

                    var transactionEntity = _mapper.Map<Transaction>(bookingEntity);
                    transactionEntity.PaymentMethod = ((PaymentMethodsEnum)Enum.ToObject(
                            typeof(PaymentMethodsEnum),
                            bookingDto.PaymentMethod)).ToString();

                    await _transactionRepository.CreateAsync(transactionEntity);

                    var paypalOrderDto = new PaypalOrderDto
                    {

                        PurchaseUnits = new List<PaypalPurchaseUnit>
                        {
                            _mapper.Map<PaypalPurchaseUnit>(transactionEntity),
                        },
                    };
                    var paypalOrderResponse = await _paypalService.CreatePayment(paypalOrderDto);

                    var purchaseUnit = paypalOrderResponse!.PurchaseUnits.First();
                    var transactionDetailEntity = new TransactionDetail
                    {
                        PaymentId = paypalOrderResponse!.Id,
                        Currency = purchaseUnit.Amount.CurrencyCode,
                        Description = purchaseUnit.Description,
                        Amount = decimal.Parse(paypalOrderResponse.PurchaseUnits.First().Amount.Value),
                        CreateTime = paypalOrderResponse.CreateTime.ToLocalTime(),
                        CaptureId = null,
                        Status = 0,
                    };
                    transactionEntity.TransactionDetails = new List<TransactionDetail>
                    {
                        transactionDetailEntity
                    };

                    await _transactionRepository.UpdateAsync(transactionEntity);

                    await transaction.CommitAsync();

                    var jobDto = new CancelBookingSchedulerJobDto
                    {
                        AccountId = bookingEntity.AccountId,
                        BookingId = bookingEntity.Id,
                        ExpiredTime = transactionEntity.CreateTime.AddMinutes(_paypalSettings.BookingCancelIntervalInMinutes),
                        TransactionId = transactionEntity.Id,
                        JourneyTotalDistance = bookingDto.TotalDistance,
                        JourneyTotalTime = bookingDto.TotalTime,
                        IsJourney = bookingDto.IsExistingTour,
                    };

                    var jobs = await _redisCacheService.Get<ConcurrentBag<CancelBookingSchedulerJobDto>>(RedisCacheKeys.CANCEL_BOOKING);
                    if (jobs == null || !jobs.Any())
                    {
                        jobs = new ConcurrentBag<CancelBookingSchedulerJobDto>
                        {
                            jobDto
                        };
                    }
                    else
                    {
                        jobs.Add(jobDto);
                    }
                    await _redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.CANCEL_BOOKING, jobs);

                    return paypalOrderResponse.Links.First(q => q.Rel == "approve");

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<BookingResponse<BookingViewDto>> GetInformationBookingDetail(int bookingId, string languageCode)
        {
            var result = await _bookingRepository.GetInformationBookingDetail(bookingId, languageCode);
           var dto = _mapper.Map<BookingViewDto>(result);
            var dtoPlace = new List<BookingPlaceViewDto>();
            foreach (var journey in result.BookingPlaces)
            {
               var entity = _mapper.Map<BookingPlaceViewDto>(journey);
                var firstPlace = journey.Place.PlaceDescriptions.FirstOrDefault(x=>x.LanguageCode.Trim() == languageCode);
                if (firstPlace == null)
                {
                    entity.PlaceName = journey.Place.Name;
                }

                dtoPlace.Add(entity);
            }
            dto.BookingPlaces= dtoPlace;
            dto.Transactions = dto.Transactions.Select(x =>
            {
                x.PaymentUrl = $"{_paypalSettings.Address}checkoutnow?token={x.PaymentUrl}";
                return x;
            }).ToList();
            return new BookingResponse<BookingViewDto>(dto);
        }

        public async Task<object> ConfirmBooking(string paymentId, string paymentAccountId)
        {

            var transactionDetailEntity = await _transactionRepository.FindByPaymentId(paymentId);

            if (transactionDetailEntity == null)
            {
                throw new NotFoundException();
            }
            var cancelJobs = await _redisCacheService.Get<ConcurrentBag<CancelBookingSchedulerJobDto>>(RedisCacheKeys.CANCEL_BOOKING);
            CancelBookingSchedulerJobDto? currentJob = null;
            if (cancelJobs != null)
            {
                currentJob = cancelJobs.FirstOrDefault(x => x.TransactionId == transactionDetailEntity.TransactionId
                                                                && x.BookingId == transactionDetailEntity.Transaction.BookingId);
            }
            double totalDistance = 0, totalTime = 0;
            int journeyId = 0;
            if (currentJob != null)
            {
                totalDistance = currentJob.JourneyTotalDistance;
                totalTime = currentJob.JourneyTotalTime;

                if (currentJob.IsJourney)
                {
                    var journey = new Journey
                    {
                        BookingPlaces = transactionDetailEntity.Transaction.Booking.BookingPlaces,
                        TotalTime = totalTime,
                        TotalDistance = totalDistance,
                        Status = 0,
                    };
                    await _journeyRepository.CreateAsync(journey);
                    journeyId = journey.Id;
                    transactionDetailEntity.Transaction.Booking.BookingPlaces
                    = transactionDetailEntity.Transaction.Booking.BookingPlaces.Select(x =>
                    {
                        x.JourneyId = journey.Id;
                        return x;
                    }).ToList();

                }

                var updatedCancelJobs = cancelJobs?.Where(x => !(x.TransactionId == transactionDetailEntity.TransactionId
                                                                && x.BookingId == transactionDetailEntity.Transaction.BookingId)) ?? new List<CancelBookingSchedulerJobDto>();

                await _redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.CANCEL_BOOKING, updatedCancelJobs);
            }
            var orderCapture = await _paypalService.CapturePayment(paymentId);
            transactionDetailEntity.CaptureId = orderCapture!.PurchaseUnits[0].Payments.Captures[0].Id;
            transactionDetailEntity.Currency = orderCapture.PurchaseUnits[0].Amount.CurrencyCode;
            transactionDetailEntity.PaymentAccountId = orderCapture.Payer.PayerId;
            transactionDetailEntity.UpdateTime = orderCapture.UpdateTime.ToLocalTime();
            transactionDetailEntity.Status = (int)TransactionStatus.Completed;
            transactionDetailEntity.Transaction.Status = (int)TransactionStatus.Completed;
            transactionDetailEntity.Transaction.UpdateTime = orderCapture.UpdateTime.ToLocalTime();
            transactionDetailEntity.Transaction.Booking.Status = (int)BookingStatus.Active;
            transactionDetailEntity.Transaction.Booking.UpdateTime = DateTime.Now;

            await _transactionDetailRepository.UpdateAsync(transactionDetailEntity);

            try
            {
                var sender = transactionDetailEntity.Transaction.Booking.AccountId;
                var bookingId = transactionDetailEntity.Transaction.BookingId;
                await _accountService.SendNotification(sender, sender, bookingId, (int)NotificationTypes.PaymentSuccessful);
            }
            catch
            {
                // Don't care
            }

            return new
            {
                journeyId = journeyId
            };

        }

        public async Task CancelBooking(int bookingId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var bookingEntity = await _bookingRepository.FindByIdAsync(bookingId);
                    if (bookingEntity.Status != (int)BookingStatus.ToPay)
                    {
                        throw new BadRequestException("cannot_cancel_this_booking");
                    } 
                    if (bookingEntity == null)
                    {
                        throw new NotFoundException(nameof(Booking), bookingId);
                    }
                    bookingEntity.Status = (int)BookingStatus.Cancel;
                    bookingEntity.UpdateTime = DateTime.Now.ToLocalTime();

                    var transactionEntities = await _transactionRepository.FindByBookingIdAsync(bookingId);
                    foreach (var trans in transactionEntities)
                    {
                        if (trans.Status != (int)TransactionStatus.Cancel && trans.Status != (int)TransactionStatus.Completed)
                        {
                            trans.UpdateTime = DateTime.Now.ToLocalTime();
                            trans.Status = (int)TransactionStatus.Cancel;
                        }
                        await _transactionRepository.UpdateAsync(trans);
                    }
                    await transaction.CommitAsync();

                    try
                    {
                        var sender = bookingEntity.AccountId;
                        await _accountService.SendNotification(sender, sender, bookingId, (int)NotificationTypes.BookingCancelled);
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task CancelBatchBooking()
        {
            try
            {
                var allJobs = await _redisCacheService.Get<List<CancelBookingSchedulerJobDto>>(RedisCacheKeys.CANCEL_BOOKING);
                if (allJobs == null || !allJobs.Any())
                {
                    return;
                }
                var now = DateTime.Now;
                var jobsRemain = new List<CancelBookingSchedulerJobDto>();
                foreach (var job in allJobs)
                {
                    if (now > job.ExpiredTime)
                    {
                        await CancelBooking(job.BookingId);
                    }
                    else
                    {
                        jobsRemain.Add(job);
                    }
                }
                await _redisCacheService.SaveCachePermanentAsync(RedisCacheKeys.CANCEL_BOOKING, jobsRemain);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
