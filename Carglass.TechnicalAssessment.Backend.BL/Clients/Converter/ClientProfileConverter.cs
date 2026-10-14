using AutoMapper;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.BL.Clients.Converter;

public class ClientProfileConverter : Profile
{
    public ClientProfileConverter()
    {
        // Entity to Dto
        CreateMap<Client, ClientDto>();

        // Dto to Entity
        CreateMap<ClientDto, Client>();
    }
}
