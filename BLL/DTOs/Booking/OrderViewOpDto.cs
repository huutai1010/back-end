using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Booking
{
    public class OrderViewOpDto : BaseDto
    {
        public string? CustomerImage { get; set; }
        public string? CustomerName { get; set; }
        public int? Status { get; set;}
        public string? StatusType { get; set; }
        public decimal TourTotalTime { get; set;}
    }
}
