using Newtonsoft.Json;
using ValidacionLogica.Entidades;

namespace ValidacionLogica.Datos;

public class EntradaRepositorio
{
    private readonly string rutaArchivo;

    public EntradaRepositorio(string rutaArchivo) => this.rutaArchivo = rutaArchivo;

    public List<Entrada> Leer()
    {
        if (!File.Exists(rutaArchivo)) return new List<Entrada>();
        var json = File.ReadAllText(rutaArchivo);
        if (string.IsNullOrWhiteSpace(json)) return new List<Entrada>();
        return JsonConvert.DeserializeObject<List<Entrada>>(json) ?? new List<Entrada>();
    }

    public void Guardar(List<Entrada> entradas)
    {
        var json = JsonConvert.SerializeObject(entradas, Formatting.Indented);
        File.WriteAllText(rutaArchivo, json);
    }
}