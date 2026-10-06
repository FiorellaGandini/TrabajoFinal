using Microsoft.AspNetCore.Mvc;
using GestionLogica.Servicios;

namespace GestionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService servicio;

    public UsuariosController(UsuarioService servicio)
    {
        this.servicio = servicio;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(servicio.ObtenerTodos());
    }
}