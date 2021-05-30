using Application.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Behaviours
{
    public class FilterBehaviour : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Where(x => x.Value.Errors.Count > 0)
                    .Select(x => x.Value.Errors.FirstOrDefault()?.ErrorMessage).ToList();
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.ContentType = "application/json";
                context.Result = new ObjectResult(new ResponseExtension(errors: errors));
                return;
            }
            await next();
        }
    }
}
