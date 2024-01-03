using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class QueryParameters
    {
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 0;
        public string? SearchBy { get; set; }
        public string? Search { get; set; }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
    }
}
