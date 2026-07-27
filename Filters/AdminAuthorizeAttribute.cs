using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace studentmanagement.Filters
{
	public class AdminAuthorizeAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			// Allow Anonymous Actions
			var allowAnonymous = context.ActionDescriptor.EndpointMetadata
				.OfType<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>()
				.Any();

			if (allowAnonymous)
			{
				base.OnActionExecuting(context);
				return;
			}
			var adminEmail = context.HttpContext.Session.GetString("AdminEmail");

			if (string.IsNullOrEmpty(adminEmail))
			{
				context.Result = new RedirectToActionResult("Login", "Account", null);
				return;
			}

			base.OnActionExecuting(context);
		}
	}
}