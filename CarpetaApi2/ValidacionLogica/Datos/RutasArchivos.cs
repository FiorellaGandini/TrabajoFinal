namespace ValidacionLogica.Datos;

public static class RutasArchivos
{
    private const string CarpetaDatosCompartidos = "../../DatosCompartidos";

    public static string Entradas => Path.Combine(CarpetaDatosCompartidos, "entradas.json");
    public static string Eventos => Path.Combine(CarpetaDatosCompartidos, "eventos.json");
}