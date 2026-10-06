using Newtonsoft.Json;
using NUnit.Framework;
using ValidacionLogica.Datos;
using ValidacionLogica.Entidades;
using ValidacionLogica.Enums;
using ValidacionLogica.Servicios;

namespace ValidacionLogicaTest;

// Base para los tests del servicio: aísla entradas.json / eventos.json en una carpeta temporal.
// Cambia el directorio de trabajo (es global al proceso), por eso no se paraleliza.
[NonParallelizable]
public abstract class ArchivosTestBase
{
    // Evento: 01/11/2026 20:00, ingreso desde 1 h antes y hasta 2 h después => ventana 19:00 a 22:00
    protected static readonly DateTime FechaEvento = new(2026, 11, 1, 20, 0, 0);
    protected static readonly DateTime DentroDeVentana = new(2026, 11, 1, 20, 30, 0);
    protected static readonly DateTime AntesDeApertura = new(2026, 11, 1, 18, 59, 0);
    protected static readonly DateTime DespuesDelCierre = new(2026, 11, 1, 22, 1, 0);

    private string directorioOriginal = string.Empty;
    private string raizTemporal = string.Empty;

    protected ValidacionService Servicio = null!;

    [SetUp]
    public void PrepararEntornoAislado()
    {
        directorioOriginal = Directory.GetCurrentDirectory();
        raizTemporal = Path.Combine(Path.GetTempPath(), "ValidacionTests_" + Guid.NewGuid().ToString("N"));

        var directorioDeTrabajo = Path.Combine(raizTemporal, "api", "bin");
        Directory.CreateDirectory(directorioDeTrabajo);
        Directory.CreateDirectory(Path.Combine(raizTemporal, "DatosCompartidos"));
        Directory.SetCurrentDirectory(directorioDeTrabajo);

        Servicio = new ValidacionService();
    }

    [TearDown]
    public void LimpiarEntornoAislado()
    {
        Directory.SetCurrentDirectory(directorioOriginal);
        if (Directory.Exists(raizTemporal))
            Directory.Delete(raizTemporal, recursive: true);
    }

    // Evento activo con ventana de ingreso 19:00 a 22:00
    protected static Evento CrearEvento(int id = 1, EstadoEvento estado = EstadoEvento.Activo) =>
        new Evento(id, estado, FechaEvento, 1, 2);

    // Orden del constructor: codigo, eventoId, modalidadEntradaId, compraId, estado, fechaValidacion
    protected static Entrada CrearEntrada(string codigo = "ABC123", int eventoId = 1,
        EstadoEntrada estado = EstadoEntrada.Vendida, int compraId = 1) =>
        new Entrada(codigo, eventoId, 1, compraId, estado, null);

    // eventos.json lo escribe la API de gestión; acá lo simulamos serializando igual que ella.
    protected static void GuardarEventos(params Evento[] eventos) =>
        File.WriteAllText(RutasArchivos.Eventos,
            JsonConvert.SerializeObject(eventos.ToList(), Formatting.Indented));

    protected static void GuardarEntradas(params Entrada[] entradas) =>
        new EntradaRepositorio(RutasArchivos.Entradas).Guardar(entradas.ToList());

    protected static Entrada LeerEntradaDelArchivo(string codigo) =>
        new EntradaRepositorio(RutasArchivos.Entradas).Leer().Single(e => e.Codigo == codigo);
}