using FluentValidation;

using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Validators;

public class UpdateMyDomainRequestValidator : AbstractValidator<UpdateMyDomainRequest>
{
    public UpdateMyDomainRequestValidator()
    {
        RuleFor(x => x.name).NotNull().NotEmpty().WithMessage("Name is required");
    }
}