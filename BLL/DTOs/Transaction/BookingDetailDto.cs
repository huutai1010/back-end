using BLL.DTOs.Account;
namespace BLL.DTOs.Transaction
{
    public class BookingDetailDto : BaseDto
    {
        public decimal Total { get; set; }
        public int Status { get; set; }
        public string? StatusType { get; set; }
        public DateTime CreateTime { get; set; }
        public string? TourCreatetor { get; set; }
        public DateTime? TourCreateDate { get; set; }
        public TimeSpan? DurationExpected { get; set; }
        public AccountInforDto? CustomerInfor { get; set; }
        public List<PlaceListDetailDto>? PlaceDetail { get; set; }
        public List<HistoryTransactionDto>? historyTransactions { get; set; }
    }
}
