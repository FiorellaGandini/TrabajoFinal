using Newtonsoft.Json;

namespace GestionLogica.Entidades;

public class ModalidadEntrada
{
    private int id;
    private int eventoId;
    private string nombre = string.Empty;
    private string? beneficios;
    private decimal precio;
    private int cupoTotal;
    private int cupoVendido;

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

    public string Nombre
    {
        get => nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre de la modalidad no puede estar vacío.");
            nombre = value;
        }
    }

    public string? Beneficios
    {
        get => beneficios;
        private set => beneficios = value;
    }

    public decimal Precio
    {
        get => precio;
        private set
        {
            if (value < 0)
                throw new ArgumentException("El precio no puede ser negativo.");
            precio = value;
        }
    }

    public int CupoTotal
    {
        get => cupoTotal;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El cupo total debe ser mayor a 0.");
            cupoTotal = value;
        }
    }

    public int CupoVendido
    {
        get => cupoVendido;
        private set => cupoVendido = value;
    }

    public int CupoDisponible => CupoTotal - CupoVendido;

    // Constructor de negocio: modalidad nueva, sin ventas.
    public ModalidadEntrada(int id, int eventoId, string nombre, decimal precio,
        int cupoTotal, string? beneficios = null)
    {
        Id = id;
        EventoId = eventoId;
        Nombre = nombre;
        Precio = precio;
        CupoTotal = cupoTotal;
        Beneficios = beneficios;
        CupoVendido = 0;
    }

    // Constructor de reconstrucción: PRIVADO. Solo Newtonsoft lo invoca al
    // leer modalidades.json, trayendo el CupoVendido real.
    [JsonConstructor]
    private ModalidadEntrada(int id, int eventoId, string nombre, string? beneficios,
        decimal precio, int cupoTotal, int cupoVendido)
    {
        Id = id;
        EventoId = eventoId;
        Nombre = nombre;
        Beneficios = beneficios;
        Precio = precio;
        CupoTotal = cupoTotal;

        if (cupoVendido < 0 || cupoVendido > cupoTotal)
            throw new ArgumentException("El cupo vendido es inconsistente con el cupo total.");
        CupoVendido = cupoVendido;
    }

    public bool HayCupoDisponible(int cantidad) => cantidad > 0 && CupoDisponible >= cantidad;

    public void RegistrarVenta(int cantidad)
    {
        if (!HayCupoDisponible(cantidad))
            throw new InvalidOperationException($"No hay cupo suficiente en la modalidad '{Nombre}'.");

        CupoVendido += cantidad;
    }

    public void LiberarCupo(int cantidad = 1)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad a liberar debe ser mayor a 0.");

        CupoVendido = Math.Max(0, CupoVendido - cantidad);
    }
}