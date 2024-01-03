using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class ConversationResponse<T> : BaseResponse
    {
        public T Conversation { get; set; }
        public ConversationResponse(T data) : base()
        {
            Conversation = data;
        }
    }

    public class ConversationListResponse<T> : BaseResponse
    {
        public ConversationListResponse(T data) : base()
        {
            Conversations = data;
        }

        public T Conversations { get; set; }
    }
}
