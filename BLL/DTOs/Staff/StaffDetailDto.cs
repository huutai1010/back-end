using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Staff
{
    public class StaffDetailDto : BaseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Gender { get; set; }
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
        public string Phone { get; set; } = null!;
        public string? NationalCode { get; set; }
        public string? Address { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? updateTime { get; set; }
        public int RoleId { get; set; }
        public int Status { get; set; }
    }
}
