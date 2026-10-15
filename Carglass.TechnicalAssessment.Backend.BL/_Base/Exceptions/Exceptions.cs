namespace Carglass.TechnicalAssessment.Backend.BL;

public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}

public sealed class NotFoundException : BusinessException
{
    public NotFoundException(string entityName, object key)
        : base($"No existe ningún {entityName} con Id '{key}'.") { }
}

public sealed class ConflictException : BusinessException
{
    public ConflictException(string message) : base(message) { }
}