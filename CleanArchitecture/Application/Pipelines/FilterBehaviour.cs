using Application.Response;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Pipelines
{
    public class FilterBehaviour : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;
        public FilterBehaviour(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var status = StatusCodes.Status400BadRequest;
                var errors = context.ModelState.Where(x => x.Value.Errors.Count > 0)
                    .Select(x => x.Value.Errors.FirstOrDefault()?.ErrorMessage).ToList();
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = status;
                response.ContentType = "application/json";
                context.Result = new ObjectResult(new Response.Response(errors: errors));
                return;
            }
            await next();
        }
    }
}
