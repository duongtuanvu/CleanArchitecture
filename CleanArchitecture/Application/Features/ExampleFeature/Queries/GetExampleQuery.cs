using Data.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public class GetExampleQuery : IRequest<bool>
    {
        public class GetExampleQueryHandler : IRequestHandler<GetExampleQuery, bool>
        {
            private readonly IUnitOfWork _uow;
            public GetExampleQueryHandler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public Task<bool> Handle(GetExampleQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
