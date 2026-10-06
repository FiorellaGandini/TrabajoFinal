using Microsoft.AspNetCore.Mvc;
using GestionLogica.Servicios;

namespace GestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntradasController : ControllerBase
{
    private readonly EntradaService servicio;

    public EntradasController(EntradaService servicio) => this.servicio = servicio;

    [HttpDelete("{codigo}")]
    public IActionResult Cancelar(string codigo, [FromQuery] int dniSolicitante)
    {
        try
        {
            var entrada = servicio.Cancelar(dniSolicitante, codigo);
            return Ok(entrada);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}