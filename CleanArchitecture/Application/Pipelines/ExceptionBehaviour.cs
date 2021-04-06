using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Pipelines
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var status = StatusCodes.Status500InternalServerError;
            context.ExceptionHandled = true;
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = status;
            response.ContentType = "application/json";
            context.Result = new ObjectResult(new Response.Response(errors: "Server error occurred."));
        }
    }
}
