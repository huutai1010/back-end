using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Place.Excel
{
    public class ExcelPlaceDescOption
    {
        public List<ExcelPlaceDescDto>? excelPlaceDescs { get; set; }
        public List<IFormFile>? VoiceFiles { get; set; }
        public List<string> containerName { get; set; }
    }
}
