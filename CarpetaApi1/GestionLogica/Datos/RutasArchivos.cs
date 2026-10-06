namespace GestionLogica.Datos;

public static class RutasArchivos
{
    private const string CarpetaDatosCompartidos = "../../DatosCompartidos";

    public static string Usuarios => Path.Combine(CarpetaDatosCompartidos, "usuarios.json");
    public static string Eventos => Path.Combine(CarpetaDatosCompartidos, "eventos.json");
    public static string Entradas => Path.Combine(CarpetaDatosCompartidos, "entradas.json");
    public static string Compras => Path.Combine(CarpetaDatosCompartidos, "compras.json");
}