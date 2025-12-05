
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace server.Authorization
{
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _module;
        private readonly string _action;

        public PermissionAuthorizeAttribute(string module, string action)
        {
            _module = module;
            _action = action;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Not logged in
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var needed = $"{_module}:{_action}";

            var hasPermission = user.Claims.Any(c =>
                c.Type == "Permission" &&
                string.Equals(c.Value, needed, StringComparison.OrdinalIgnoreCase)
            );

            if (!hasPermission)
            {
                // Logged in but not enough permission
                context.Result = new ForbidResult();
            }
        }
    }
}
