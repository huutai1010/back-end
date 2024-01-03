using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Responses
{
    public class FileStatusResponse : BaseResponse
    {
        public string Message { get; set; }
        public FileStatusResponse(string message) : base()
        {
            Message = message;
        }
    }
    public class UploadResponse : BaseResponse
    {
        public List<ResponseFileImageDto> ImageFiles { get; set; }
        public UploadResponse(List<ResponseFileImageDto> imageFiles) : base()
        {
            ImageFiles = imageFiles;
        }
    }
}
