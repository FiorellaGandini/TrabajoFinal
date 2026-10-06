using Newtonsoft.Json;
using ValidacionLogica.Enums;

namespace ValidacionLogica.Entidades;
public class Entrada
{
    private string codigo = string.Empty;
    private int eventoId;
    private int modalidadEntradaId;
    private int compraId;
    private EstadoEntrada estado;
    private DateTime? fechaValidacion;

    public string Codigo
    {
        get => codigo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El código no puede estar vacío.");

            var normalizado = value.ToUpperInvariant();
            if (normalizado.Length != 6 || !normalizado.All(char.IsAsciiLetterOrDigit))
                throw new ArgumentException("El código debe ser alfanumérico de 6 caracteres (letras A-Z y dígitos 0-9).");

            codigo = normalizado;
        }
    }

    public int EventoId
    {
        get => eventoId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El id del evento debe ser un número positivo.");
            eventoId = value;
        }
    }

    public int ModalidadEntradaId
    {
        get => modalidadEntradaId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El id de la modalidad debe ser un número positivo.");
            modalidadEntradaId = value;
        }
    }

    public int CompraId
    {
        get => compraId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El id de la compra debe ser un número positivo.");
            compraId = value;
        }
    }

    public EstadoEntrada Estado
    {
        get => estado;
        private set => estado = value;
    }

    public DateTime? FechaValidacion
    {
        get => fechaValidacion;
        private set => fechaValidacion = value;
    }

    [JsonConstructor]
    public Entrada(string codigo, int eventoId, int modalidadEntradaId, int compraId,
        EstadoEntrada estado, DateTime? fechaValidacion)
    {
        Codigo = codigo;
        EventoId = eventoId;
        ModalidadEntradaId = modalidadEntradaId;
        CompraId = compraId;
        Estado = estado;
        FechaValidacion = fechaValidacion;
    }

    public void MarcarComoUsada(DateTime momento)
    {
        if (Estado == EstadoEntrada.Usada)
            throw new InvalidOperationException($"La entrada {Codigo} ya fue utilizada.");
        if (Estado == EstadoEntrada.Cancelada)
            throw new InvalidOperationException($"La entrada {Codigo} fue cancelada y no es válida.");

        Estado = EstadoEntrada.Usada;
        FechaValidacion = momento;
    }
}