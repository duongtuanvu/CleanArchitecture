using Application.Features.ExampleFeature.Commands;
using FluentValidation;

namespace Application.Features.ExampleFeature.Validations
{
    class CreateExampleCommandValidator : AbstractValidator<CreateExampleCommand>
    {
        public CreateExampleCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
