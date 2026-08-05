using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using store.Services;

namespace store.Attributes
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLower();
            if (path != null && path.Contains("/admin/login"))
            {
                base.OnActionExecuting(context);
                return;
            }

            var authService = context.HttpContext.RequestServices.GetService<AdminAuthService>();

            if (authService == null || !authService.IsAuthenticated())
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
