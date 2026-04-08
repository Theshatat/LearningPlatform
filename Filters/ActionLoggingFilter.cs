using Microsoft.AspNetCore.Mvc.Filters;

namespace LearningPlatform.Filters
{
    public class ActionLoggingFilter : IActionFilter
    {
        public DateTime time = DateTime.Now;
        private readonly ILogger<ActionLoggingFilter> _logger;

        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"Executing action : {context.ActionDescriptor.DisplayName} (Action Starts) at {time}");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Executed action : {context.ActionDescriptor.DisplayName} (Action Finished) at {time}");
        }

        
    }
}
