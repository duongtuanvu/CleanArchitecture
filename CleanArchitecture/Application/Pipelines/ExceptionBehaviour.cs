using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Pipelines
{
    public class ExceptionBehaviour : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ContentType = "application/json";
            context.Result = new ObjectResult(new Response.Response(errors: "Server error occurred."));
        }
    }
}
