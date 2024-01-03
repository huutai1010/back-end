using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Visitor
{
    public class VisitorListDto : BaseDto
    {
        public string? FullName { get; set; }
        public string? Nationality { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
    }
}
