using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Staff
{
    public class StaffListDto : BaseDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public DateTime CreateTime { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
    }
}
