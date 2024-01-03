using Common.Constants;

using Newtonsoft.Json.Converters;

using System.Text.Json.Serialization;

namespace BLL.DTOs.Account
{
    public class AccountListDto : BaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string? Gender { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string Role { get; set; }
    }
}
