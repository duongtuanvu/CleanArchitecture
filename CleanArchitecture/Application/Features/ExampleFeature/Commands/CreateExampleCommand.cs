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
            public async Task<bool> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
            {
                return true;
            }
        }
    }
}
