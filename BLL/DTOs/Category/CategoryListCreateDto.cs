﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Category
{
    public class CategoryListCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
