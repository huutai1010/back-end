using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Account
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
        public string Phone { get; set; } = null!;
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Status { get; set; }
    }
}
