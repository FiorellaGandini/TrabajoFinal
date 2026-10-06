namespace GestionLogica.Entidades;

public class Ubicacion
{
    private string nombre = string.Empty;
    private string direccion = string.Empty;
    private double? latitud;
    private double? longitud;

    public string Nombre
    {
        get => nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del lugar no puede estar vacío.");
            nombre = value;
        }
    }

    public string Direccion
    {
        get => direccion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La dirección no puede estar vacía.");
            direccion = value;
        }
    }

    // Null: se completan solo si se cargan coordenadas para mostrar el mapa.
    public double? Latitud => latitud;
    public double? Longitud => longitud;

    public Ubicacion(string nombre, string direccion, double? latitud = null, double? longitud = null)
    {
        Nombre = nombre;
        Direccion = direccion;
        FijarCoordenadas(latitud, longitud);
    }

    public bool TieneCoordenadas() => latitud.HasValue && longitud.HasValue;

    // Se llama una sola vez, al crear (o editar) el evento, con el
    // resultado de geocodificar la dirección. Si la geocodificación
    // falló o no se hizo, quedan en null y el link se arma con la
    // dirección en texto (ver ObtenerUrlGoogleMaps).
    public void FijarCoordenadas(double? nuevaLatitud, double? nuevaLongitud)
    {
        if (nuevaLatitud.HasValue != nuevaLongitud.HasValue)
            throw new ArgumentException("Latitud y longitud deben cargarse juntas.");
        if (nuevaLatitud is < -90 or > 90)
            throw new ArgumentException("La latitud debe estar entre -90 y 90.");
        if (nuevaLongitud is < -180 or > 180)
            throw new ArgumentException("La longitud debe estar entre -180 y 180.");

        latitud = nuevaLatitud;
        longitud = nuevaLongitud;
    }

    // Arma el link a Google Maps. Es solo texto (no llama a ninguna API),
    // por eso puede vivir acá: si hay coordenadas, usa el punto exacto;
    // si no, cae a una búsqueda por dirección (funciona igual).
    public string ObtenerUrlGoogleMaps()
    {
        if (TieneCoordenadas())
            return $"https://www.google.com/maps?q={latitud},{longitud}";

        var direccionCodificada = Uri.EscapeDataString($"{nombre}, {direccion}");
        return $"https://www.google.com/maps/search/?api=1&query={direccionCodificada}";
    }
}