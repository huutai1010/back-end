using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Feedback
{
    public class FeedbackListDto :BaseDto
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string NationalImage { get; set; } = null!;
        public string? Image { get; set; }
        public int? PlaceId { get; set; }
        public int? TourId { get; set; }
        public double? Rate { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsPlace { get; set; }
    }
}
