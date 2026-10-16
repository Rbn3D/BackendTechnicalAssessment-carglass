using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Api.Controllers;

/// <summary>Gestión de productos.</summary>
[ApiController]
[Route("products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductAppService _productAppService;

    public ProductsController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    /// <summary>Obtiene todos los productos.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_productAppService.GetAll());

    /// <summary>Obtiene un producto por su Id.</summary>
    /// <param name="id">Id del producto.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id) => Ok(_productAppService.GetById(id));

    /// <summary>Crea un producto.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] ProductDto dto)
    {
        _productAppService.Create(dto);
        return Ok();
    }

    /// <summary>Actualiza un producto existente.</summary>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Update([FromBody] ProductDto dto)
    {
        _productAppService.Update(dto);
        return Ok();
    }

    /// <summary>Elimina un producto.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromBody] ProductDto dto)
    {
        _productAppService.Delete(dto);
        return Ok();
    }
}
