using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BLL.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var claims = jwtUtils.ValidateToken(token);
                if (claims.Any())
                {
                    context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
                }
            }
            await _next(context);

        }
    }
}
