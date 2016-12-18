using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Chama.Assignment.Service.Security;

namespace Chama.Assignment.Service.Middleware
{
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<Configuration.Options> _options;

        // Temporary workaround to avoid authorization on client test pages.
        // Solutions could be to use attribute based authorization, 
        // or move the test client to a different host project.
        private List<string> _excludedPaths = new List<string>()
        {
            "/client/index.html",
            "/client/client.js",
            "/signalr/hubs",
        };

        public BasicAuthenticationMiddleware(
            RequestDelegate next, 
            IOptions<Configuration.Options> options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
#if DEBUG
            if (string.IsNullOrEmpty(_options.Value.Secret) ||
                _excludedPaths.Contains(context.Request.Path))
            {
                await _next.Invoke(context);
                return;
            }
#endif
            var isAuthorized = false;

            try
            {
                var authorizationHeader = context.Request.Headers.FirstOrDefault(h => h.Key == "Authorization");

                var encoded = authorizationHeader.Value.ToString().TrimStart("BASIC ".ToCharArray());
                var decodedBytes = Convert.FromBase64String(encoded);
                var decodedValue = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var credentials = decodedValue.Split(':');

                if (credentials.Length == 2 &&
                    !string.IsNullOrEmpty(credentials[0]) &&
                    credentials[1] == _options.Value.Secret)
                {

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, credentials[0]));
                    var claimsIdentity = new ClaimsIdentity(claims);

                    context.User = new ClaimsPrincipal(new Identity(claimsIdentity));
                    isAuthorized = true;
                }
            }
            catch
            { }

            if (!isAuthorized)
            {
                ////var claims = new List<Claim>();
                ////claims.Add(new Claim(ClaimTypes.Name, "bruce"));
                ////var claimsIdentity = new ClaimsIdentity(claims);

                ////context.User = new ClaimsPrincipal(new Identity(claimsIdentity));

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(HttpStatusCode.Unauthorized.ToString());
                return;
            }

            await _next.Invoke(context);
        }
    }
}
