using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Feedback
{
    public class FeedbacksDto
    {
        public int Id { get; set; }
        public double? Rate { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public FeedbackAccountDto? Account { get; set; }
        public int Status { get; set; }
    }
}
