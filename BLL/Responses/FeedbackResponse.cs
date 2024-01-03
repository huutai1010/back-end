using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class FeedbackResponse<T> : BaseResponse
    {
        public T Feedback { get; set; }

        public FeedbackResponse(T feedback) : base()
        {
            Feedback = feedback;
        }
    }

    public class FeedbackListResponse<T> : BaseResponse
    {
        public T Feedbacks { get; set; }

        public FeedbackListResponse(T feedbacks) : base()
        {
            Feedbacks = feedbacks;
        }
    }
}
