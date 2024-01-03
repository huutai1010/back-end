using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IJwtUtils
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims);
        public IEnumerable<Claim> ValidateToken(string token);

    }
}
