﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Chart
{
    public class ChartDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Total { get; set; }
        public int NumberIncreased { get; set; }
    }
}
