using Newtonsoft.Json;
using ValidacionLogica.Entidades;

namespace ValidacionLogica.Datos;

// Solo lectura: ApiValidacion nunca crea ni edita eventos.
public class EventoRepositorio
{
    private readonly string rutaArchivo;

    public EventoRepositorio(string rutaArchivo)
    {
        this.rutaArchivo = rutaArchivo;
    }

    public List<Evento> Leer()
    {
        if (!File.Exists(rutaArchivo))
            return new List<Evento>();

        var json = File.ReadAllText(rutaArchivo);
        if (string.IsNullOrWhiteSpace(json))
            return new List<Evento>();

        return JsonConvert.DeserializeObject<List<Evento>>(json) ?? new List<Evento>();
    }

    public Evento? ObtenerPorId(int id) =>
        Leer().FirstOrDefault(e => e.Id == id);
}