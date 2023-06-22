using Mapster;

using MyDomain.Application.Services.Commands.CreateMyDomain;
using MyDomain.Application.Services.Commands.UpdateMyDomain;
using MyDomain.Application.Common.Models;
using MyDomain.Contracts.Models.V1;
using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Common.Mapping;

public class MyDomainMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MyDomainResult, MyDomainDto>();
        config.NewConfig<CreateMyDomainRequest, CreateMyDomainCommand>();
        config.NewConfig<(UpdateMyDomainRequest Request, Guid Id), UpdateMyDomainCommand>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest, src => src.Request);
    }
}