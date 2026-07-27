using Microsoft.AspNetCore.Mvc;
using studentmanagement.Filters;

namespace studentmanagement.Controllers
{
	[AdminAuthorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class BaseController : Controller
	{
	}
}