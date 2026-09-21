using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace onlineStore.Filters
{
    public class AuthFilter : ActionFilterAttribute
    {
        private readonly string? _requiredRole;
        public AuthFilter(string? requiredRole = null)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var isBlocked = httpContext.Session.GetString("IsBlocked");

            if (isBlocked == "True")
            {
                httpContext.Session.Clear();
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(_requiredRole))
            {
                var role = httpContext.Session.GetString("Role");
                if (role != _requiredRole)
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
