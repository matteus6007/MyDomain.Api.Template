using FluentValidation;

using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Validators;

public class CreateMyDomainRequestValidator : AbstractValidator<CreateMyDomainRequest>
{
    public CreateMyDomainRequestValidator()
    {
        RuleFor(x => x.name).NotNull().NotEmpty().WithMessage("Name is required");
    }
}