using Newtonsoft.Json;
using GestionLogica.Entidades;

namespace GestionLogica.Datos;

public class EventoRepositorio
{
    private readonly string rutaArchivo;

    public EventoRepositorio(string rutaArchivo) => this.rutaArchivo = rutaArchivo;

    public List<Evento> Leer()
    {
        if (!File.Exists(rutaArchivo)) return new List<Evento>();
        var json = File.ReadAllText(rutaArchivo);
        if (string.IsNullOrWhiteSpace(json)) return new List<Evento>();
        return JsonConvert.DeserializeObject<List<Evento>>(json) ?? new List<Evento>();
    }

    public void Guardar(List<Evento> eventos)
    {
        var json = JsonConvert.SerializeObject(eventos, Formatting.Indented);
        File.WriteAllText(rutaArchivo, json);
    }
}