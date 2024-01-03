using BLL.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Transaction
{
    public class PlaceListDetailDto
    {
        public string Name { get; set; } = null!;
        public TimeSpan? Hour { get; set; }
        public decimal? Price { get; set; }
        public string? CategoryName { get; set; }
    }
}
