using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Category
{
    public class CategoryListDto : BaseDto
    {
        public string? Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
        public int TotalLanguage { get; set; }

    }
}
