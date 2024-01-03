using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Account
{
    public class AccountInforDto
    {
        public string? CustomerName { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string? Image { get; set; }
    }
}
