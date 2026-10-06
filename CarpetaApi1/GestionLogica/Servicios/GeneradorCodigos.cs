namespace GestionLogica.Servicios;

public static class GeneradorCodigos
{
    private const string Caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int Longitud = 6;
    private static readonly Random Random = new();

    public static List<string> GenerarLote(int cantidad, IEnumerable<string> codigosExistentes)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad de códigos a generar debe ser mayor a 0.");

        var yaUsados = new HashSet<string>(codigosExistentes, StringComparer.OrdinalIgnoreCase);
        var generados = new List<string>();

        while (generados.Count < cantidad)
        {
            var candidato = GenerarUno();
            if (yaUsados.Add(candidato))
                generados.Add(candidato);
        }

        return generados;
    }

    private static string GenerarUno()
    {
        var chars = new char[Longitud];
        for (int i = 0; i < Longitud; i++)
            chars[i] = Caracteres[Random.Next(Caracteres.Length)];
        return new string(chars);
    }
}