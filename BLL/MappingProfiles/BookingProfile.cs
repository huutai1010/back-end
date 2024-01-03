using AutoMapper;

using BLL.DTOs.Booking;
using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Place;
using BLL.DTOs.Place.CelebratedPlace;
using BLL.DTOs.Transaction;

using Common.Constants;
using Common.Models.Paypal;

using DAL.Entities;

namespace BLL.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<CelebrateImage, CelebratedImageDto>();
            CreateMap<BookingPlace, CelebratedPlaceDto>()
                .ForMember(m => m.CelebrateImages, opt => opt.MapFrom(src => src.CelebrateImages))
                .ForMember(m => m.PlaceName, opt => opt.MapFrom(src => src.Place.PlaceDescriptions.FirstOrDefault().Name));

            CreateMap<CelebratedImageDto, CelebrateImage>()
                .AfterMap((dto, entity) =>
                {
                    entity.Status = 1;
                });

            CreateMap<Booking, OrderViewOpDto>()
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(BookingStatus), src.Status).ToString()))
                .ForPath(x => x.CustomerName, opt => opt.MapFrom(src => src.Account.FirstName + " " + src.Account.LastName))
                .ForPath(x => x.CustomerImage, opt => opt.MapFrom(src => src.Account.Image))
                .AfterMap((entity, dto) =>
                {
                    TimeSpan sumTime = new();
                    foreach (var item in entity.BookingPlaces)
                    {
                        if (item.Place.Hour != null)
                        {
                            sumTime = sumTime.Add((TimeSpan)item.Place.Hour);
                        }
                    }

                    dto.TourTotalTime = (decimal)sumTime.TotalHours;
                });

            CreateMap<PlaceImage, PlaceImageViewDto>();
            CreateMap<Booking, HistoryBookingViewDto>()
                .ForMember(m => m.TotalPlaces, opt => opt.MapFrom(src => src.BookingPlaces.Count))
                .ForMember(m => m.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Booking, TransactionBookingOverviewDto>().ForMember(m => m.TourId, opt => opt.MapFrom(src => src.ItineraryId))
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => Enum.ToObject(typeof(BookingStatus), src.Status).ToString()))
                .ForMember(m => m.TotalPlaces, opt => opt.MapFrom(src => src.BookingPlaces.Count));


            CreateMap<Booking, BookingPlaceViewDto>()
                .ForMember(m => m.PlaceName, opt => opt.MapFrom(src => src.BookingPlaces.Select(x => x.Place.PlaceDescriptions.FirstOrDefault().Name)));

            CreateMap<ItineraryPlace, BookingPlace>()
                .AfterMap((_, bookingDetail) =>
                {
                    bookingDetail.Status = (int)BookingPlaceStatus.Future;
                });
            CreateMap<BookingPlaceDto, BookingPlace>()
                .ForMember(m => m.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
                .ForMember(m => m.Price, opt => opt.Ignore())
                .ForMember(m => m.Ordinal, opt => opt.MapFrom(src => src.Ordinal));
            CreateMap<Itinerary, Booking>()
                .ForMember(m => m.ItineraryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.BookingPlaces, opt => opt.MapFrom(src => src.TourDetails))
                .AfterMap((_, booking) =>
                {
                    booking.CreateTime = DateTime.Now;
                    booking.UpdateTime = null;
                    booking.Status = (int)BookingStatus.ToPay;
                    booking.IsPrepared = true;
                });
            CreateMap<Place, BookingPlace>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.PlaceId, opt => opt.MapFrom(src => src.Id));
            CreateMap<PostBookingDto, Booking>()
                .ForMember(m => m.BookingPlaces, opt => opt.MapFrom(src => src.BookingPlaces))
                .ForMember(m => m.ItineraryId, opt => opt.MapFrom(src => src.TourId))
                .AfterMap((_, booking) =>
                {
                    booking.CreateTime = DateTime.Now;
                    booking.UpdateTime = null;
                });

            CreateMap<Booking, Transaction>()
                .ForMember(m => m.Description, opt => opt.MapFrom(src => $"Thanh toán eTravel - {src.Id}"))
                .ForMember(m => m.Amount, opt => opt.MapFrom(src => src.Total))
                .ForMember(m => m.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Id, opt => opt.Ignore())
                .AfterMap((_, transaction) =>
                {
                    transaction.CreateTime = DateTime.Now;
                    transaction.UpdateTime = null;
                    transaction.Status = (int)TransactionStatus.Processing;
                });

            CreateMap<decimal, PaypalBaseAmount>()
                .ForMember(m => m.Value, opt => opt.MapFrom(src => src));
            CreateMap<Transaction, PaypalPurchaseUnit>()
                .ForMember(m => m.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<Booking, BookingViewDto>()
                .ForMember(m => m.TourId, opt => opt.MapFrom(src => src.ItineraryId))
                .AfterMap((entity, dto) =>
                {
                    dto.StatusName = Enum.ToObject(typeof(BookingStatus), entity.Status).ToString();
                });
        }

    }
}
