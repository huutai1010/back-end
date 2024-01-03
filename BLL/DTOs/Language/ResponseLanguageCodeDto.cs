using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Language
{
    public class ResponseLanguageCodeDto
    {
        public List<LanguageCodeDto> data { get; set; }
    }   
}
