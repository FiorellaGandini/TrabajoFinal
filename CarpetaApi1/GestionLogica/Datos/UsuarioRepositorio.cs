using Newtonsoft.Json;
using GestionLogica.Entidades;

namespace GestionLogica.Datos;

public class UsuarioRepositorio
{
    private readonly string rutaArchivo;

    public UsuarioRepositorio(string rutaArchivo) => this.rutaArchivo = rutaArchivo;

    public List<Usuario> Leer()
    {
        if (!File.Exists(rutaArchivo)) return new List<Usuario>();
        var json = File.ReadAllText(rutaArchivo);
        if (string.IsNullOrWhiteSpace(json)) return new List<Usuario>();
        return JsonConvert.DeserializeObject<List<Usuario>>(json) ?? new List<Usuario>();
    }

    public void Guardar(List<Usuario> usuarios)
    {
        var json = JsonConvert.SerializeObject(usuarios, Formatting.Indented);
        File.WriteAllText(rutaArchivo, json);
    }
}