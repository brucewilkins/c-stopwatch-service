using Chama.Assignment.Service.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Chama.Assignment.Service.Extensions
{
    public static class SecurityExtensions
    {
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
