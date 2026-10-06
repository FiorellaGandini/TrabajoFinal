using Microsoft.AspNetCore.Mvc;
using GestionLogica.Servicios;

namespace GestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComprasController : ControllerBase
{
    private readonly CompraService servicio;

    public ComprasController(CompraService servicio) => this.servicio = servicio;

    public class RegistrarCompraRequest
    {
        public int DniComprador { get; set; }
        public int EventoId { get; set; }
        public int ModalidadId { get; set; }
        public int Cantidad { get; set; }
    }

    [HttpPost]
    public IActionResult Registrar([FromBody] RegistrarCompraRequest request)
    {
        try
        {
            var compra = servicio.Registrar(request.DniComprador, request.EventoId,
                request.ModalidadId, request.Cantidad, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified));
            return StatusCode(201, compra);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var compra = servicio.ObtenerPorId(id);
        if (compra is null) return NotFound();

        var entradas = servicio.ObtenerEntradasDeCompra(id);
        return Ok(new { compra, entradas });
    }
}