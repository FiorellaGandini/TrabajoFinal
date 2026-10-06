using Microsoft.AspNetCore.Mvc;
using GestionLogica.Servicios;

namespace GestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ReporteService servicio;

    public ReportesController(ReporteService servicio) => this.servicio = servicio;

    [HttpGet("recaudacion")]
    public IActionResult ObtenerRecaudacion([FromQuery] int dniSolicitante)
    {
        try
        {
            return Ok(servicio.ObtenerRecaudacion(dniSolicitante));
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
    }
}