using Data.Context;
using MediatR;
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
            var typeName = request.GetType().Name;
            await using var transaction = _context.Database.BeginTransaction();
            try
            {
                _logger.LogInformation($"Start transaction Id {transaction.TransactionId} for {typeName}");
                var response = await next();
                _context.SaveChanges();
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation($"End transaction Id {transaction.TransactionId} for {typeName}");
                return response;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError($"Error transaction Id {transaction.TransactionId} for {typeName}: {e.Message}");
                throw;
            }
        }
    }
}
