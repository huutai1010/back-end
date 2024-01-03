using BLL.DTOs.Booking.BookingDetail;
using BLL.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Booking
{
    public class BookingViewDto : BaseDto
    {
        public int? TourId { get; set; }
        public decimal Total { get; set; }
        public bool IsPrepared { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public string? StatusName { get; set; }

        public virtual List<BookingPlaceViewDto> BookingPlaces { get; set; }
        public virtual List<TransactionListDto> Transactions { get; set; }
    }
}
