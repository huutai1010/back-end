﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Not Found!")
        {            
        }

        public NotFoundException(string name, object key) : base($"{name} ({key}) was not found!")
        {

        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}
