using Data.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Commands
{
    public class CreateExampleCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public class CreateExampleCommandHandler : IRequestHandler<CreateExampleCommand, bool>
        {
            private readonly IUnitOfWork _uow;
            public CreateExampleCommandHandler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public Task<bool> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
