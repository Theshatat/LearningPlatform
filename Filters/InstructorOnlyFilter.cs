using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LearningPlatform.Filters
{
    public class InstructorOnlyFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Instructor")
            {
                context.Result = new ForbidResult();
            }

        }
    }
}
