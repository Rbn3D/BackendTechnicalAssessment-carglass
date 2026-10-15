using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ClientAppService : CrudAppService<ClientDto, Client>, IClientAppService
{
    public ClientAppService(ICrudRepository<Client> clientRepository, IMapper mapper, IValidator<ClientDto> clientValidator)
        : base(clientRepository, mapper, clientValidator)
    { }

    protected override string EntityName => "cliente";

    protected override void ValidateBusinessRulesOnCreate(ClientDto dto)
    {
        if (Repository.GetAll().Any(c => c.DocNum == dto.DocNum))
            throw new ConflictException("Ya existe un cliente con ese número de documento.");
    }
}
