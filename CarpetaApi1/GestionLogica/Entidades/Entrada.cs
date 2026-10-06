using Newtonsoft.Json;
using GestionLogica.Enums;

namespace GestionLogica.Entidades;

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

    // Constructor de negocio: entrada nueva generada por una compra. Arranca Vendida.
    public Entrada(string codigo, int eventoId, int modalidadEntradaId, int compraId)
    {
        Codigo = codigo;
        EventoId = eventoId;
        ModalidadEntradaId = modalidadEntradaId;
        CompraId = compraId;
        Estado = EstadoEntrada.Vendida;
    }

    // Constructor de reconstrucción: PRIVADO. Solo Newtonsoft lo invoca al
    // leer entradas.json (el archivo que comparten las dos APIs), trayendo
    // Estado y FechaValidacion tal como quedaron tras el último cambio.
    [JsonConstructor]
    private Entrada(string codigo, int eventoId, int modalidadEntradaId, int compraId,
        EstadoEntrada estado, DateTime? fechaValidacion)
    {
        Codigo = codigo;
        EventoId = eventoId;
        ModalidadEntradaId = modalidadEntradaId;
        CompraId = compraId;
        Estado = estado;
        FechaValidacion = fechaValidacion;
    }

    public bool FueUsada() => Estado == EstadoEntrada.Usada;
    public bool EstaCancelada() => Estado == EstadoEntrada.Cancelada;

    private bool EstadoPermiteIngreso() => Estado == EstadoEntrada.Vendida;

    public void MarcarComoUsada(Evento evento, DateTime momento)
    {
        if (evento.Id != EventoId)
            throw new InvalidOperationException($"La entrada {Codigo} es de otro evento.");
        if (!evento.EstaEnVentanaDeIngreso(momento))
            throw new InvalidOperationException($"La entrada {Codigo} está fuera del horario de ingreso.");
        if (!EstadoPermiteIngreso())
            throw new InvalidOperationException($"La entrada {Codigo} no puede ingresar: su estado actual es {Estado}.");

        Estado = EstadoEntrada.Usada;
        FechaValidacion = momento;
    }

    public void Cancelar()
    {
        if (FueUsada())
            throw new InvalidOperationException("No se puede cancelar una entrada que ya fue usada para ingresar.");

        if (EstaCancelada())
            throw new InvalidOperationException("La entrada ya fue cancelada.");
        Estado = EstadoEntrada.Cancelada;
    }
}