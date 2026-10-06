using NUnit.Framework;
using GestionLogica.Entidades;
using GestionLogica.Enums;
using GestionLogica.Servicios;

namespace GestionLogicaTest;

// Base para los tests de services: aisla los archivos JSON en una carpeta temporal
public abstract class ArchivosTestBase
{
    protected static readonly DateTime Ahora = new(2026, 10, 10, 12, 0, 0);
    protected static readonly DateTime FechaEvento = new(2026, 11, 1, 20, 0, 0);

    protected const int DniOrganizador = 30111222;
    protected const int DniOtroOrganizador = 29888777;
    protected const int DniComprador = 40123456;
    protected const int DniOtroComprador = 38456789;

    private string directorioOriginal = string.Empty;
    private string raizTemporal = string.Empty;

    protected UsuarioService Usuarios = null!;
    protected EventoService Eventos = null!;
    protected CompraService Compras = null!;
    protected EntradaService Entradas = null!;
    protected ReporteService Reportes = null!;

    [SetUp]
    public void PrepararEntornoAislado()
    {
        directorioOriginal = Directory.GetCurrentDirectory();
        raizTemporal = Path.Combine(Path.GetTempPath(), "GestionTests_" + Guid.NewGuid().ToString("N"));

        var directorioDeTrabajo = Path.Combine(raizTemporal, "api", "bin");
        Directory.CreateDirectory(directorioDeTrabajo);
        Directory.CreateDirectory(Path.Combine(raizTemporal, "DatosCompartidos"));
        Directory.SetCurrentDirectory(directorioDeTrabajo);

        Usuarios = new UsuarioService();
        Usuarios.PrecargarSiNoExisten();
        Eventos = new EventoService(Usuarios);
        Compras = new CompraService(Usuarios);
        Entradas = new EntradaService(Usuarios);
        Reportes = new ReporteService(Usuarios);
    }

    [TearDown]
    public void LimpiarEntornoAislado()
    {
        Directory.SetCurrentDirectory(directorioOriginal);
        if (Directory.Exists(raizTemporal))
            Directory.Delete(raizTemporal, recursive: true);
    }

    protected Evento CrearEventoConModalidad(int cupo = 10, decimal precio = 100m, string titulo = "Rock en vivo")
    {
        var evento = Eventos.Crear(DniOrganizador, titulo, "Gran show", FechaEvento,
            new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2);
        Eventos.AgregarModalidad(DniOrganizador, evento.Id, "General", precio, cupo);
        return Eventos.ObtenerPorId(evento.Id, Ahora)!;
    }
}