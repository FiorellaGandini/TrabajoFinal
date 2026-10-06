using Newtonsoft.Json;

namespace GestionLogica.Entidades;

public class Compra
{
    private const int CantidadMinimaParaDescuento = 5;
    private const decimal PorcentajeDescuento = 0.15m; // ajustable por el equipo — defender en el coloquio
    private int id;
    private int dniComprador;
    private int eventoId;
    private int modalidadEntradaId;
    private int cantidad;
    private decimal precioUnitario;
    private decimal precioTotal;
    private DateTime fecha;
    private readonly List<string> codigosEntradas;

    public int Id
    {
        get => id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El id debe ser un número positivo.");
            id = value;
        }
    }

    public int DniComprador
    {
        get => dniComprador;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El DNI debe ser un número positivo.");
            dniComprador = value;
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

    public int Cantidad
    {
        get => cantidad;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0.");
            cantidad = value;
        }
    }

    public decimal PrecioUnitario
    {
        get => precioUnitario;
        private set
        {
            if (value < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo.");
            precioUnitario = value;
        }
    }

    public decimal PrecioTotal
    {
        get => precioTotal;
        private set => precioTotal = value;
    }

    public DateTime Fecha
    {
        get => fecha;
        private set => fecha = value;
    }

    public IReadOnlyList<string> CodigosEntradas => codigosEntradas.AsReadOnly();

    // Constructor de negocio: compra nueva. Calcula el total con descuento
    // (si Cantidad >= 5) una sola vez, acá.
    public Compra(int id, int dniComprador, int eventoId, int modalidadEntradaId,
        int cantidad, decimal precioUnitario, DateTime fecha)
    {
        Id = id;
        DniComprador = dniComprador;
        EventoId = eventoId;
        ModalidadEntradaId = modalidadEntradaId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Fecha = fecha;
        PrecioTotal = CalcularPrecioTotal();
        codigosEntradas = new List<string>();
    }

    // Constructor de reconstrucción: PRIVADO. Solo Newtonsoft lo invoca al
    // leer compras.json, trayendo PrecioTotal y CodigosEntradas ya generados
    // (no se recalculan, para no perder la asociación con las entradas reales).
    [JsonConstructor]
    private Compra(int id, int dniComprador, int eventoId, int modalidadEntradaId,
        int cantidad, decimal precioUnitario, decimal precioTotal, DateTime fecha,
        List<string> codigosEntradas)
    {
        Id = id;
        DniComprador = dniComprador;
        EventoId = eventoId;
        ModalidadEntradaId = modalidadEntradaId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        PrecioTotal = precioTotal;
        Fecha = fecha;
        this.codigosEntradas = codigosEntradas ?? new List<string>();
    }

    public bool TieneDescuento() => Cantidad >= CantidadMinimaParaDescuento;

    private decimal CalcularPrecioTotal()
    {
        decimal total = PrecioUnitario * Cantidad;
        if (TieneDescuento())
            total -= total * PorcentajeDescuento;
        return total;
    }

    public void AgregarCodigoEntrada(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código no puede estar vacío.");
        if (codigosEntradas.Count >= Cantidad)
            throw new InvalidOperationException("Ya se generaron todas las entradas correspondientes a esta compra.");

        codigosEntradas.Add(codigo.ToUpperInvariant());
    }
}