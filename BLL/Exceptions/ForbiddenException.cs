using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base("You don't have permission to do this action")
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
