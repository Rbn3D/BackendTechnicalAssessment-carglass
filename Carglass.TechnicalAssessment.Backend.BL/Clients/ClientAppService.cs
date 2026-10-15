using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ClientAppService : IClientAppService
{
    private readonly ICrudRepository<Client> _clientRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ClientDto> _clientValidator;

    public ClientAppService(ICrudRepository<Client> clientRepository, IMapper mapper, IValidator<ClientDto> clientValidator)
    { 
        this._clientRepository = clientRepository;
        this._mapper = mapper;
        this._clientValidator = clientValidator;
    }

    public IEnumerable<ClientDto> GetAll()
    {
        var clients = _clientRepository.GetAll();
        return _mapper.Map<IEnumerable<ClientDto>>(clients);
    }

    public ClientDto GetById(params object[] id)
    {
        var client = _clientRepository.GetById(id)
                     ?? throw new NotFoundException("cliente", id[0]);
        return _mapper.Map<ClientDto>(client);
    }

    public void Create(ClientDto client)
    {
        _clientValidator.ValidateAndThrow(client);

        if (_clientRepository.GetById(client.Id) is not null)
            throw new ConflictException($"Ya existe un cliente con Id '{client.Id}'.");

        if (_clientRepository.GetAll().Any(c => c.DocNum == client.DocNum))
            throw new ConflictException("Ya existe un cliente con ese número de documento.");

        _clientRepository.Create(_mapper.Map<Client>(client));
    }

    public void Update(ClientDto client)
    {
        if (null == _clientRepository.GetById(client.Id))
        {
            throw new NotFoundException("cliente", client.Id);
        }

        _clientValidator.ValidateAndThrow(client);

        var clientEntity = _mapper.Map<Client>(client);
        _clientRepository.Update(clientEntity);
    }

    public void Delete(ClientDto client)
    {
        var clientEntity = _clientRepository.GetById(client.Id);

        if (clientEntity is null)
        {
            throw new NotFoundException("cliente", client.Id);
        }

        _clientRepository.Delete(clientEntity);
    }
}