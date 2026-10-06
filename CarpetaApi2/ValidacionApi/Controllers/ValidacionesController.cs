using Microsoft.AspNetCore.Mvc;
using ValidacionLogica.Servicios;

namespace ValidacionApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidacionesController : ControllerBase
{
    private readonly ValidacionService validacionService;

    public ValidacionesController(ValidacionService validacionService)
    {
        this.validacionService = validacionService;
    }

    [HttpPost]
    public IActionResult Validar([FromBody] ValidarEntradaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Codigo))
            return BadRequest(new { error = "El código de entrada es requerido." });

        var resultado = validacionService.Validar(request.Codigo, request.EventoId, DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-3), DateTimeKind.Unspecified));
        return resultado.Valida ? Ok(resultado) : BadRequest(resultado);
    }
}

public class ValidarEntradaRequest
{
    public string Codigo { get; set; } = string.Empty;
    public int EventoId { get; set; }
}