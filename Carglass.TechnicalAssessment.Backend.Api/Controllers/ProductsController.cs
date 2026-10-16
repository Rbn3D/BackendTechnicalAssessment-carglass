using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductAppService _productAppService;

    public ProductsController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_productAppService.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(int id) => Ok(_productAppService.GetById(id));

    [HttpPost]
    public IActionResult Create([FromBody] ProductDto dto)
    {
        _productAppService.Create(dto);
        return Ok();
    }

    [HttpPut]
    public IActionResult Update([FromBody] ProductDto dto)
    {
        _productAppService.Update(dto);
        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete([FromBody] ProductDto dto)
    {
        _productAppService.Delete(dto);
        return Ok();
    }
}
