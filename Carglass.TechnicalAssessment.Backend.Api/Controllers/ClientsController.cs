using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Api.Controllers;

/// <summary>Gestión de clientes.</summary>
[ApiController]
[Route("clients")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IClientAppService _clientAppService;

    public ClientsController(IClientAppService clientAppService)
    {
        _clientAppService = clientAppService;
    }

    /// <summary>Obtiene todos los clientes.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ClientDto>> GetAll()
    {
        return Ok(_clientAppService.GetAll());
    }

    /// <summary>Obtiene un cliente por su Id.</summary>
    /// <param name="id">Id del cliente.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<ClientDto> GetById(int id)
    {
        return Ok(_clientAppService.GetById(id));
    }

    /// <summary>Crea un cliente.</summary>
    /// <remarks>El Id y el número de documento deben ser únicos. Si el tipo de documento es "nif", el número debe tener 8 dígitos y 1 letra.</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] ClientDto dto)
    {
        _clientAppService.Create(dto);
        return Ok();
    }

    /// <summary>Actualiza un cliente existente.</summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update([FromBody] ClientDto dto)
    {
        _clientAppService.Update(dto);
        return Ok();
    }

    /// <summary>Elimina un cliente.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromBody] ClientDto dto)
    {
        _clientAppService.Delete(dto);
        return Ok();
    }
}
