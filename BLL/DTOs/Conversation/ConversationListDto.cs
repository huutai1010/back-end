using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Conversation
{
    public class ConversationListDto : BaseDto
    {
        public int AccountOneId { get; set; }
        public string AccountOneUsername { get; set; } = null!;
        public string AccountOnePhone { get; set; } = null!;
        public string AccountOneFirstName { get; set; } = null!;
        public string AccountOneLastName { get; set; } = null!;
        public string? AccountOneImage { get; set; }
        public int AccountTwoId { get; set; }
        public string AccountTwoPhone { get; set; } = null!;

        public string AccountTwoFirstName { get; set; } = null!;
        public string AccountTwoLastName { get; set; } = null!;
        public string? AccountTwoImage { get; set; }
        public string AccountTwoUsername { get; set; } = null!;

        public int Status { get; set; }
    }
}
