using Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Application.Behaviours
{
    public class ExceptionBehaviour : IExceptionFilter
    {
        private readonly ILogger<ExceptionBehaviour> _logger;
        public ExceptionBehaviour(ILogger<ExceptionBehaviour> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(eventId: 500, exception: context.Exception, null, null);
            context.ExceptionHandled = true;
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ContentType = "application/json";
            context.Result = new ObjectResult(new JsonResponse(errors: new Error(context.Exception.Message, context.Exception.StackTrace)));
        }
    }
}
