using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal abstract class CrudAppService<TDto, TEntity> : ICrudAppService<TDto>
    where TDto : IDto
    where TEntity : class, IEntity
{
    private readonly IValidator<TDto> _validator;

    protected CrudAppService(ICrudRepository<TEntity> repository, IMapper mapper, IValidator<TDto> validator, ILogger logger)
    {
        Repository = repository;
        Mapper = mapper;
        _validator = validator;
        Logger = logger;
    }

    protected ICrudRepository<TEntity> Repository { get; }
    protected IMapper Mapper { get; }
    protected ILogger Logger { get; }

    /// <summary>Nombre de la entidad para los mensajes de error (p. ej. "cliente").</summary>
    protected abstract string EntityName { get; }

    public IEnumerable<TDto> GetAll()
    {
        return Mapper.Map<IEnumerable<TDto>>(Repository.GetAll());
    }

    public TDto GetById(params object[] keyValues)
    {
        var entity = Repository.GetById(keyValues)
                     ?? throw new NotFoundException(EntityName, keyValues[0]);
        return Mapper.Map<TDto>(entity);
    }

    public void Create(TDto dto)
    {
        _validator.ValidateAndThrow(dto);

        if (Repository.GetById(dto.Id) is not null)
            throw new ConflictException($"Ya existe un {EntityName} con Id '{dto.Id}'.");

        ValidateBusinessRulesOnCreate(dto);

        Repository.Create(Mapper.Map<TEntity>(dto));

        Logger.LogInformation("{EntityName} {EntityId} creado", EntityName, dto.Id);
    }

    public void Update(TDto dto)
    {
        if (Repository.GetById(dto.Id) is null)
            throw new NotFoundException(EntityName, dto.Id);

        _validator.ValidateAndThrow(dto);

        Repository.Update(Mapper.Map<TEntity>(dto));

        Logger.LogInformation("{EntityName} {EntityId} actualizado", EntityName, dto.Id);
    }

    public void Delete(TDto dto)
    {
        var entity = Repository.GetById(dto.Id)
                     ?? throw new NotFoundException(EntityName, dto.Id);

        Repository.Delete(entity);

        Logger.LogInformation("{EntityName} {EntityId} eliminado", EntityName, dto.Id);
    }

    /// <summary>Reglas de negocio adicionales al crear. Por defecto, ninguna.</summary>
    protected virtual void ValidateBusinessRulesOnCreate(TDto dto) { }
}
