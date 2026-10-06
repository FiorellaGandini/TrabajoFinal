using GestionLogica.Datos;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogica.Servicios;

public class EntradaService
{
    private readonly EntradaRepositorio entradaRepositorio;
    private readonly CompraRepositorio compraRepositorio;
    private readonly EventoRepositorio eventoRepositorio;
    private readonly UsuarioService usuarioService;

    public EntradaService(UsuarioService usuarioService)
    {
        entradaRepositorio = new EntradaRepositorio(RutasArchivos.Entradas);
        compraRepositorio = new CompraRepositorio(RutasArchivos.Compras);
        eventoRepositorio = new EventoRepositorio(RutasArchivos.Eventos);
        this.usuarioService = usuarioService;
    }

    public Entrada? ObtenerPorCodigo(string codigo) =>
        entradaRepositorio.Leer().FirstOrDefault(e => e.Codigo == codigo.ToUpperInvariant());

    // DELETE /api/entradas/{codigo} — sección 12, equipos en promoción.
    // Solo el comprador dueño de la compra puede cancelar su entrada.
    // Si ya fue usada para ingresar, Entrada.Cancelar() la rechaza.
    public Entrada Cancelar(int dniSolicitante, string codigo)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Comprador);

        var entradas = entradaRepositorio.Leer();
        var entrada = entradas.FirstOrDefault(e => e.Codigo == codigo.ToUpperInvariant())
            ?? throw new ArgumentException($"No existe una entrada con código {codigo}.");

        var compra = compraRepositorio.Leer().FirstOrDefault(c => c.Id == entrada.CompraId)
            ?? throw new InvalidOperationException("No se encontró la compra asociada a esta entrada.");

        if (compra.DniComprador != dniSolicitante)
            throw new UnauthorizedAccessException("Esta entrada no pertenece al usuario solicitante.");

        entrada.Cancelar();
        entradaRepositorio.Guardar(entradas);

        var eventos = eventoRepositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == entrada.EventoId);
        var modalidad = evento?.ObtenerModalidad(entrada.ModalidadEntradaId);

        if (modalidad is not null && evento is not null)
        {
            modalidad.LiberarCupo(1);
            eventoRepositorio.Guardar(eventos);
        }

        return entrada;
    }
}