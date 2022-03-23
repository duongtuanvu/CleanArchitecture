using Data.UnitOfWork;
using Domain.Entities;
using Domain.Interface;
using MediatR;
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
            public async Task<bool> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
            {
                var example = new ExampleModel()
                {
                    Name = request.Name
                };
                _uow.ExampleRepository.Add(example);
                return true;
            }
        }
    }
}
