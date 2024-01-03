using AutoMapper;

using BLL.DTOs.Transaction;

using Common.Constants;

using DAL.Entities;

namespace BLL.MappingProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Booking, BookingListDto>()
                .ForPath(m => m.CustomerName, opt => opt.MapFrom(src => src.Account.FirstName + " " + src.Account.LastName))
                .AfterMap((entity, dto) =>
                {
                    dto.TotalPlace = entity.BookingPlaces.Count();

                    TimeSpan sumTime = new();
                    foreach (var item in entity.BookingPlaces)
                    {
                        if (item.Place.Hour != null)
                        {
                            sumTime = sumTime.Add((TimeSpan)item.Place.Hour);
                        }
                    }

                    dto.TotalTime = (decimal)sumTime.TotalHours;
                    dto.StatusType = Enum.ToObject(typeof(BookingStatus), entity.Status).ToString();
                })
                .ReverseMap();

            CreateMap<Booking, BookingDetailDto>()
                .ForMember(m => m.CustomerInfor, opt => opt.MapFrom(src => src.Account))
                .ForPath(m => m.PlaceDetail, opt => opt.MapFrom(src => src.BookingPlaces))
                .ForPath(m => m.historyTransactions, opt => opt.MapFrom(src => src.Transactions))
                .AfterMap((entity, dto) =>
                {
                    dto.StatusType = Enum.ToObject(typeof(BookingStatus), entity.Status).ToString();
                })
                .ReverseMap();

            CreateMap<BookingPlace, PlaceListDetailDto>()
                .ForPath(m => m.Name, opt => opt.MapFrom(x => x.Place.Name))
                .ForPath(m => m.Price, opt => opt.MapFrom(x => x.Place.Price))
                .ForPath(m => m.Hour, opt => opt.MapFrom(x => x.Place.Hour))
                .AfterMap((entity, dto) =>
                {
                    var category = entity.Place.PlaceCategories.SingleOrDefault(x => x.CategoryId == 8);
                    if (category != null)
                    {
                        dto.CategoryName = "food";
                    }
                    else
                    {
                        dto.CategoryName = "visit location";
                    }
                }).ReverseMap();
            CreateMap<Transaction, TransactionViewDto>();

            CreateMap<TransactionDetail, TransactionPaymentDetailDto>()
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => Enum.ToObject(typeof(TransactionStatus), src.Status).ToString()));
            CreateMap<Transaction, TransactionDetailViewDto>()
                .ForMember(m => m.StatusName, opt => opt.MapFrom(src => ((TransactionStatus)Enum.ToObject(typeof(TransactionStatus), src.Status)).ToString()))
                .ForMember(m => m.Details, opt => opt.MapFrom(src => src.TransactionDetails.First()))
                .ForMember(m => m.Currency, opt => opt.MapFrom(src => src.TransactionDetails.First().Currency))
                .ForMember(m => m.PaymentId, opt => opt.MapFrom(src => src.TransactionDetails.First().PaymentId));

            CreateMap<Transaction, TransactionListDto>()
                .ForPath(m => m.BookingId, opt => opt.MapFrom(x => x.Booking.Id))
                .ForMember(m => m.CustomerName, opt => opt.MapFrom(src => src.Account.FirstName + " " + src.Account.LastName))
                .ForMember(m => m.StatusType, opt => opt.MapFrom(src => Enum.ToObject(typeof(TransactionStatus), src.Status).ToString()))
                .ForMember(m => m.PaymentUrl, opt => opt.MapFrom(src => src.TransactionDetails.FirstOrDefault() != null 
                                                                        ? src.TransactionDetails.First().PaymentId 
                                                                        : null));

            CreateMap<Transaction, HistoryTransactionDto>()
                .AfterMap((entity, dto) =>
                {
                    dto.StatusType = Enum.ToObject(typeof(TransactionStatus), entity.Status).ToString();
                })
                .ReverseMap();

            CreateMap<Transaction, TransactionDetailDto>()
                .AfterMap((entity, dto) =>
                {
                    dto.StatusType = Enum.ToObject(typeof(TransactionStatus), entity.Status).ToString();
                })
                .ReverseMap();
        }
    }
}
