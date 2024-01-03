using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Feedback
{
    public class FeedbackListViewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Category { get; set; }
        public string National { get; set; }
        public double? Rate { get; set; }
        public string? Content { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
    }
}
