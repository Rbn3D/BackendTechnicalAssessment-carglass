using Carglass.TechnicalAssessment.Backend.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using FluentValidation;
using System.Text.Json;

namespace Carglass.TechnicalAssessment.Backend.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(exception, "Excepción tras iniciar la respuesta; no se puede reescribir.");
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problem = exception switch
        {
            ValidationException ve => CreateValidationProblem(ve),
            NotFoundException => CreateProblem(StatusCodes.Status404NotFound, exception.Message),
            ConflictException => CreateProblem(StatusCodes.Status409Conflict, exception.Message),
            BusinessException => CreateProblem(StatusCodes.Status400BadRequest, exception.Message),
            _ => CreateProblem(StatusCodes.Status500InternalServerError,
                                       _environment.IsDevelopment() ? exception.ToString()
                                                                   : "Se ha producido un error inesperado.")
        };

        if (problem.Status >= 500)
            _logger.LogError(exception, "Error no controlado en {Method} {Path}",
                context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("Error de negocio {ExceptionType} en {Method} {Path}: {Message}",
                exception.GetType().Name, context.Request.Method, context.Request.Path, exception.Message);

        problem.Instance = context.Request.Path;
        context.Response.StatusCode = problem.Status!.Value;
                await context.Response.WriteAsJsonAsync(problem, problem.GetType(),
            (JsonSerializerOptions?)null, "application/problem+json");
    }

    private static ProblemDetails CreateProblem(int status, string detail) =>
        new() { Status = status, Title = ReasonPhrases.GetReasonPhrase(status), Detail = detail };

    private static ValidationProblemDetails CreateValidationProblem(ValidationException exception) =>
        new(exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Uno o más errores de validación."
        };
}
