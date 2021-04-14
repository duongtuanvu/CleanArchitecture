using Data.UnitOfWork;
using Domain.Entities;
using Domain.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Commands
{
    public class CreateCommand : IRequest<bool>
    {
        public string Name { get; set; }

        public class CreateCommandHandler : IRequestHandler<CreateCommand, bool>
        {
            private readonly IUnitOfWork _uow;
            public CreateCommandHandler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public async Task<bool> Handle(CreateCommand request, CancellationToken cancellationToken)
            {
                var example = new ExampleModel()
                {
                    Name = request.Name
                };
                await _uow.exampleRepository.Add(example);
                return true;
            }
        }
    }
}
