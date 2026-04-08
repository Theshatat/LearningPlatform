using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LearningPlatform.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result
                = new ObjectResult(new 
                { error = context.Exception.Message })
                {
                    StatusCode = 500
                };
        }
    }
}
