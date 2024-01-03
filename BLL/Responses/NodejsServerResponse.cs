using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class NodejsServerResponse<T> : BaseResponse
    {
        public T VoiceFiles { get; set; }
        public NodejsServerResponse(T data)
        {
            VoiceFiles = data;
        }
    }
}
