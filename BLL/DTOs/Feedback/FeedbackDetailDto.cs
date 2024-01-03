using BLL.DTOs.Account;
using BLL.DTOs.Itinerary;
using BLL.DTOs.Place;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Feedback
{
    public class FeedbackDetailDto
    {
        public int Id { get; set; }
        public double? Rate { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
        public virtual AccountInforDto Account { get; set; }
        public virtual PlaceInfoDto Place { get; set; }
        public virtual ItineratyInfoDto Itinerary { get; set; }
    }
}
