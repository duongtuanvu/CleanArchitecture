using Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Pipelines
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly ApplicationDbContext _context;
        public TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
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
    }
}
