using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Auth
{
    public class AuthDto : BaseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string? Gender { get; set; }
        public int RoleId { get; set; }
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; }
        public string RoleName { get; set; }
        public string AccessToken { get; set; }
    }
}
