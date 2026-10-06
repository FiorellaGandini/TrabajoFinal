using ValidacionLogica.Datos;

namespace ValidacionLogica.Servicios;

public class ResultadoValidacion
{
    public bool Valida { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public DateTime? FechaValidacion { get; set; }
}

public class ValidacionService
{
    private readonly EntradaRepositorio entradaRepositorio;
    private readonly EventoRepositorio eventoRepositorio;

    public ValidacionService()
    {
        entradaRepositorio = new EntradaRepositorio(RutasArchivos.Entradas);
        eventoRepositorio = new EventoRepositorio(RutasArchivos.Eventos);
    }

    // POST /api/validaciones recibe el código y el evento
    // que se controla en esa puerta, y dice si se puede dejar pasar o no,
    // y por qué "no existe, es de otro evento, ya fue usada"
    public ResultadoValidacion Validar(string codigo, int eventoId, DateTime momento)
    {
        var entradas = entradaRepositorio.Leer();
        var entrada = entradas.FirstOrDefault(e => e.Codigo == codigo.ToUpperInvariant());

        if (entrada is null)
            return new ResultadoValidacion { Valida = false, Motivo = "La entrada no existe." };

        if (entrada.EventoId != eventoId)
            return new ResultadoValidacion { Valida = false, Motivo = $"La entrada {entrada.Codigo} es de otro evento." };

        var evento = eventoRepositorio.ObtenerPorId(eventoId);
        if (evento is null)
            return new ResultadoValidacion { Valida = false, Motivo = $"No existe un evento con id {eventoId}." };

        if (!evento.EstaEnVentanaDeIngreso(momento))
            return new ResultadoValidacion { Valida = false, Motivo = $"La entrada {entrada.Codigo} está fuera del horario de ingreso." };

        try
        {
            entrada.MarcarComoUsada(momento);
        }
        catch (InvalidOperationException ex)
        {
            return new ResultadoValidacion { Valida = false, Motivo = ex.Message };
        }

        entradaRepositorio.Guardar(entradas);

        return new ResultadoValidacion
        {
            Valida = true,
            Motivo = "Ingreso confirmado.",
            Codigo = entrada.Codigo,
            FechaValidacion = momento
        };
    }
}