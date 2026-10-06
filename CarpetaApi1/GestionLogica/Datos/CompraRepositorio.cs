using Newtonsoft.Json;
using GestionLogica.Entidades;

namespace GestionLogica.Datos;

public class CompraRepositorio
{
    private readonly string rutaArchivo;

    public CompraRepositorio(string rutaArchivo) => this.rutaArchivo = rutaArchivo;

    public List<Compra> Leer()
    {
        if (!File.Exists(rutaArchivo)) return new List<Compra>();
        var json = File.ReadAllText(rutaArchivo);
        if (string.IsNullOrWhiteSpace(json)) return new List<Compra>();
        return JsonConvert.DeserializeObject<List<Compra>>(json) ?? new List<Compra>();
    }

    public void Guardar(List<Compra> compras)
    {
        var json = JsonConvert.SerializeObject(compras, Formatting.Indented);
        File.WriteAllText(rutaArchivo, json);
    }
}