using System.Linq;
using SignalrAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SignalrAPI.Common
{
    public class Authenticate : TypeFilterAttribute
    {
        public Authenticate() : base(typeof(RouteRequirementFilter))
        {
            Arguments = System.Array.Empty<object>();
        }
    }

    public class RouteRequirementFilter : IAuthorizationFilter
    {
        private readonly AppSettings appSettings;
        public RouteRequirementFilter(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings.Value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context?.ActionDescriptor.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>().Any() ?? false;
            if (allowAnonymous) { return; }

            if (!context.HttpContext.Request.Headers.ContainsKey("Zola-Signal-R"))
            {
                context.Result = new ForbidResult();
                return;
            }
            var authHeader = context.HttpContext.Request.Headers["Zola-Signal-R"][0];
            if (authHeader.StartsWith("Bearer "))
            {
                var authToken = authHeader.Substring("Bearer ".Length);
                if (authToken == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                var test = HashHelper.DecryptText(authToken, appSettings.ZolaCryptoPassWord);
                if (test == null || test != "Zola-Server-Request")
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
