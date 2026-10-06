using GestionLogica.Datos;
using GestionLogica.Enums;

namespace GestionLogica.Servicios;

public class ReporteRecaudacionPorEvento
{
    public int EventoId { get; set; }
    public string TituloEvento { get; set; } = string.Empty;
    public decimal TotalRecaudado { get; set; }
    public int EntradasVendidas { get; set; }
}

public class ReporteService
{
    private readonly EventoRepositorio eventoRepositorio;
    private readonly CompraRepositorio compraRepositorio;
    private readonly UsuarioService usuarioService;

    public ReporteService(UsuarioService usuarioService)
    {
        eventoRepositorio = new EventoRepositorio(RutasArchivos.Eventos);
        compraRepositorio = new CompraRepositorio(RutasArchivos.Compras);
        this.usuarioService = usuarioService;
    }

    public List<ReporteRecaudacionPorEvento> ObtenerRecaudacion(int dniSolicitante)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Organizador);

        var eventos = eventoRepositorio.Leer();
        var compras = compraRepositorio.Leer();

        return eventos.Select(evento => new ReporteRecaudacionPorEvento
        {
            EventoId = evento.Id,
            TituloEvento = evento.Titulo,
            TotalRecaudado = compras.Where(c => c.EventoId == evento.Id).Sum(c => c.PrecioTotal),
            EntradasVendidas = compras.Where(c => c.EventoId == evento.Id).Sum(c => c.Cantidad)
        }).ToList();
    }
}