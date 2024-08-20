using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CineMatrix_API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
           
            _logger.LogError(context.Exception, "An unhandled exception occurred.");


            var response = new
            {
                success = false,
                message = "An unexpected error occurred. Please try again later."
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500 
            };

            context.ExceptionHandled = true; 
        }
    }
}
