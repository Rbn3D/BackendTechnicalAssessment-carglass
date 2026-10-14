using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Api.Controllers;

[ApiController]
[Route("clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientAppService _clientAppService;

    public ClientsController(IClientAppService clientAppService)
    {
        this._clientAppService = clientAppService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_clientAppService.GetAll());
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(_clientAppService.GetById(id));
    }

    [HttpPost]
    public IActionResult Create([FromBody] ClientDto dto)
    {
        _clientAppService.Create(dto);
        return Ok();
    }

    [HttpPut]
    public IActionResult Update([FromBody] ClientDto dto)
    {
        _clientAppService.Update(dto);
        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete([FromBody] ClientDto dto)
    {
        _clientAppService.Delete(dto);
        return Ok();
    }
}