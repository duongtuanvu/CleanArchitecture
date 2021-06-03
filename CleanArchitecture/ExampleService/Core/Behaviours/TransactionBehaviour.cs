using ExampleService.Core.Helpers;
using ExampleService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Core.Behaviours
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var method = _httpContextAccessor.HttpContext.Request.Method;
            if (method.Equals(Constant.POST) || method.Equals(Constant.PUT) || method.Equals(Constant.DELETE))
            {
                var typeName = request.GetType().Name;
                var strage = _context.Database.CreateExecutionStrategy();
                try
                {
                    await strage.ExecuteAsync(async () =>
                    {
                        await using var transaction = _context.Database.BeginTransaction();
                        _logger.LogInformation($"=====>  Start transaction for {typeName}");
                        var response = await next();
                        _context.SaveChanges();
                        await transaction.CommitAsync(cancellationToken);
                        _logger.LogInformation($"=====> End transaction for {typeName}");
                        return response;
                    });
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"=====> Error: {ex.Message}");
                    throw;
                }
            }
            response = await next();
            return response;
        }
    }
}
