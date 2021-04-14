using Application.Features.ExampleFeature.Commands;
using FluentValidation;

namespace Application.Features.ExampleFeature.Validations
{
    class CreateCommandValidator : AbstractValidator<CreateCommand>
    {
        public CreateCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
