using Microsoft.AspNetCore.Mvc;
using GestionLogica.Servicios;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly EventoService servicio;

    public EventosController(EventoService servicio)
    {
        this.servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos(DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified)));
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var evento = servicio.ObtenerPorId(id, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified));
        if (evento is null)
            return NotFound();
        return Ok(evento);
    }

    public class CrearEventoRequest
    {
        public int DniSolicitante { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string UbicacionNombre { get; set; } = string.Empty;
        public string UbicacionDireccion { get; set; } = string.Empty;
        public CategoriaEvento Categoria { get; set; }
        public bool Prioridad { get; set; }
        public int HorasAperturaAntes { get; set; } = 1;
        public int DuracionHoras { get; set; } = 2;
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CrearEventoRequest request)
    {
        try
        {
            var ubicacion = new Ubicacion(request.UbicacionNombre, request.UbicacionDireccion);
            var evento = servicio.Crear(request.DniSolicitante, request.Titulo, request.Descripcion,
                request.FechaHora, ubicacion, request.Categoria, request.Prioridad,
                request.HorasAperturaAntes, request.DuracionHoras);
            return StatusCode(201, evento);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class EditarEventoRequest
    {
        public int DniSolicitante { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string UbicacionNombre { get; set; } = string.Empty;
        public string UbicacionDireccion { get; set; } = string.Empty;
        public CategoriaEvento Categoria { get; set; }
        public bool Prioridad { get; set; }
        public int HorasAperturaAntes { get; set; } = 1;
        public int DuracionHoras { get; set; } = 2;
    }

    [HttpPut("{id}")]
    public IActionResult Editar(int id, [FromBody] EditarEventoRequest request)
    {
        if (servicio.ObtenerPorId(id, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified)) is null)
            return NotFound($"No existe un evento con id {id}.");

        try
        {
            var ubicacion = new Ubicacion(request.UbicacionNombre, request.UbicacionDireccion);
            var evento = servicio.Actualizar(request.DniSolicitante, id, request.Titulo, request.Descripcion,
                request.FechaHora, ubicacion, request.Categoria, request.Prioridad,
                request.HorasAperturaAntes, request.DuracionHoras, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified)); 
            return Ok(evento);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class AgregarModalidadRequest
    {
        public int DniSolicitante { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CupoTotal { get; set; }
        public string? Beneficios { get; set; }
    }

    [HttpPost("{id}/modalidades")]
    public IActionResult AgregarModalidad(int id, [FromBody] AgregarModalidadRequest request)
    {
        if (servicio.ObtenerPorId(id, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified)) is null)
            return NotFound($"No existe un evento con id {id}.");

        try
        {
            var modalidad = servicio.AgregarModalidad(request.DniSolicitante, id,
                request.Nombre, request.Precio, request.CupoTotal, request.Beneficios);
            return StatusCode(201, modalidad);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/cancelar")]
    public IActionResult Cancelar(int id, [FromQuery] int dniSolicitante)
    {
        try
        {
            var evento = servicio.Cancelar(dniSolicitante, id);
            return Ok(evento);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}