using BLL.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Language
{
    public class LanguageCodeDto
    {
        public string langCode { get; set; }
        public string langEnglishName { get; set; }
        public string langNativeName { get; set;}
    }

    public class LanguageCodeResponse<T> : BaseResponse
    {
        public T LanguagesCode { get; set; }
        public LanguageCodeResponse(T data) : base()
        {
            LanguagesCode = data;
        }
    }
}
