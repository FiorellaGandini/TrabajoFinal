using GestionLogica.Datos;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogica.Servicios;

public class CompraService
{
    private readonly EventoRepositorio eventoRepositorio;
    private readonly EntradaRepositorio entradaRepositorio;
    private readonly CompraRepositorio compraRepositorio;
    private readonly UsuarioService usuarioService;

    public CompraService(UsuarioService usuarioService)
    {
        eventoRepositorio = new EventoRepositorio(RutasArchivos.Eventos);
        entradaRepositorio = new EntradaRepositorio(RutasArchivos.Entradas);
        compraRepositorio = new CompraRepositorio(RutasArchivos.Compras);
        this.usuarioService = usuarioService;
    }

    public Compra Registrar(int dniComprador, int eventoId, int modalidadId, int cantidad, DateTime ahora)
    {
        usuarioService.ValidarRol(dniComprador, RolUsuario.Comprador);

        var eventos = eventoRepositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == eventoId)
            ?? throw new ArgumentException($"No existe un evento con id {eventoId}.");

        evento.ActualizarEstadoSiCorresponde(ahora);
        if (!evento.SePuedeComprarEntrada(ahora))
            throw new InvalidOperationException(
                "No se puede comprar: el evento fue cancelado, ya finalizó, o su fecha ya pasó.");

        var modalidad = evento.ObtenerModalidad(modalidadId)
            ?? throw new ArgumentException($"El evento no tiene una modalidad con id {modalidadId}.");

        if (!modalidad.HayCupoDisponible(cantidad))
            throw new InvalidOperationException(
                $"No hay cupo disponible en la modalidad '{modalidad.Nombre}' para {cantidad} entradas.");

        var entradas = entradaRepositorio.Leer();
        var codigosExistentes = entradas.Select(e => e.Codigo);
        var codigosNuevos = GeneradorCodigos.GenerarLote(cantidad, codigosExistentes);

        modalidad.RegistrarVenta(cantidad);
        eventoRepositorio.Guardar(eventos);

        var compras = compraRepositorio.Leer();
        var nuevoIdCompra = compras.Count == 0 ? 1 : compras.Max(c => c.Id) + 1;
        var compra = new Compra(nuevoIdCompra, dniComprador, eventoId, modalidadId,
            cantidad, modalidad.Precio, ahora);

        foreach (var codigo in codigosNuevos)
        {
            var entrada = new Entrada(codigo, eventoId, modalidadId, compra.Id);
            entradas.Add(entrada);
            compra.AgregarCodigoEntrada(codigo);
        }

        entradaRepositorio.Guardar(entradas);
        compras.Add(compra);
        compraRepositorio.Guardar(compras);

        return compra;
    }

    public Compra? ObtenerPorId(int id) =>
        compraRepositorio.Leer().FirstOrDefault(c => c.Id == id);

    public List<Entrada> ObtenerEntradasDeCompra(int compraId) =>
        entradaRepositorio.Leer().Where(e => e.CompraId == compraId).ToList();
}