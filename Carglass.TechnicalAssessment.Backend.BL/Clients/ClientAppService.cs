using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ClientAppService : IClientAppService
{
    private readonly ICrudRepository<Client> clientIMRepository;
    private readonly IMapper mapper;
    private readonly IValidator<ClientDto> clientValidator;

    public ClientAppService(ICrudRepository<Client> clientIMRepository, IMapper mapper, IValidator<ClientDto> clientValidator)
    { 
        this.clientIMRepository = clientIMRepository;
        this.mapper = mapper;
        this.clientValidator = clientValidator;
    }

    public IEnumerable<ClientDto> GetAll()
    {
        var moneySpenders = clientIMRepository.GetAll();
        return mapper.Map<IEnumerable<ClientDto>>(moneySpenders);
    }

    public ClientDto GetById(params object[] id)
    {
        var client = clientIMRepository.GetById(id);
        return mapper.Map<ClientDto>(client);
    }

    public void Create(ClientDto client)
    {
        if (null != clientIMRepository.GetById(client.Id))
        {
            throw new Exception("Ya existe un cliente con este Id");
        }

        var clientEntity = mapper.Map<Client>(client);

        ValidateDto(client);
        
        clientIMRepository.Create(clientEntity);
    }

    public void Update(ClientDto client)
    {
        if (null == clientIMRepository.GetById(client.Id))
        {
            throw new Exception("No existe ningún cliente con este Id");
        }

        ValidateDto(client);

        var clientEntity = mapper.Map<Client>(client);
        clientIMRepository.Update(clientEntity);
    }

    public void Delete(ClientDto client)
    {
        var clientEntity = clientIMRepository.GetById(client.Id);

        if (clientEntity is null)
        {
            throw new Exception("No existe un cliente con este Id");
        }

        clientIMRepository.Delete(clientEntity);
    }

    private void ValidateDto(ClientDto item)
    {
        var validationResult = clientValidator.Validate(item);
        if (validationResult.Errors.Any())
        {
            string toShowErrors = string.Join("; ", validationResult.Errors.Select(s => s.ErrorMessage));
            throw new Exception($"El cliente especificado no cumple los requisitos de validación. Errores: '{toShowErrors}'");
        }
    }
}