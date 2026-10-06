using GestionLogica.Datos;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogica.Servicios;

public class EventoService
{
    private readonly EventoRepositorio repositorio;
    private readonly UsuarioService usuarioService;

    public EventoService(UsuarioService usuarioService)
    {
        repositorio = new EventoRepositorio(RutasArchivos.Eventos);
        this.usuarioService = usuarioService;
    }

    public List<Evento> ObtenerTodos(DateTime ahora)
    {
        var eventos = repositorio.Leer();
        ActualizarEstadosYGuardarSiCambio(eventos, ahora);
        return eventos;
    }

    public Evento? ObtenerPorId(int id, DateTime ahora)
    {
        var eventos = repositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == id);
        if (evento is not null)
            ActualizarEstadosYGuardarSiCambio(eventos, ahora);
        return evento;
    }

    public Evento Crear(int dniSolicitante, string titulo, string descripcion, DateTime fechaHora,
        Ubicacion ubicacion, CategoriaEvento categoria, bool prioridad,
        int horasAperturaAntes, int duracionHoras)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Organizador);

        var eventos = repositorio.Leer();
        var nuevoId = eventos.Count == 0 ? 1 : eventos.Max(e => e.Id) + 1;

        var evento = new Evento(nuevoId, titulo, descripcion, fechaHora, ubicacion,
            categoria, prioridad, horasAperturaAntes, duracionHoras);

        eventos.Add(evento);
        repositorio.Guardar(eventos);
        return evento;
    }

    public Evento Actualizar(int dniSolicitante, int eventoId, string titulo, string descripcion,
        DateTime fechaHora, Ubicacion ubicacion, CategoriaEvento categoria, bool prioridad,
        int horasAperturaAntes, int duracionHoras, DateTime ahora)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Organizador);

        var eventos = repositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == eventoId)
            ?? throw new ArgumentException($"No existe un evento con id {eventoId}.");

        evento.ActualizarDatos(titulo, descripcion, fechaHora, ubicacion, categoria,
            prioridad, horasAperturaAntes, duracionHoras, ahora);

        repositorio.Guardar(eventos);
        return evento;
    }

    public Evento Cancelar(int dniSolicitante, int eventoId)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Organizador);

        var eventos = repositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == eventoId)
            ?? throw new ArgumentException($"No existe un evento con id {eventoId}.");

        evento.Cancelar();
        repositorio.Guardar(eventos);
        return evento;
    }

    public ModalidadEntrada AgregarModalidad(int dniSolicitante, int eventoId,
        string nombre, decimal precio, int cupoTotal, string? beneficios = null)
    {
        usuarioService.ValidarRol(dniSolicitante, RolUsuario.Organizador);

        var eventos = repositorio.Leer();
        var evento = eventos.FirstOrDefault(e => e.Id == eventoId)
            ?? throw new ArgumentException($"No existe un evento con id {eventoId}.");

        var nuevoId = evento.Modalidades.Count == 0 ? 1 : evento.Modalidades.Max(m => m.Id) + 1;
        var modalidad = new ModalidadEntrada(nuevoId, eventoId, nombre, precio, cupoTotal, beneficios);

        evento.AgregarModalidad(modalidad);
        repositorio.Guardar(eventos);
        return modalidad;
    }

    private void ActualizarEstadosYGuardarSiCambio(List<Evento> eventos, DateTime ahora)
    {
        bool huboCambios = false;
        foreach (var evento in eventos)
        {
            var estadoPrevio = evento.Estado;
            evento.ActualizarEstadoSiCorresponde(ahora);
            if (evento.Estado != estadoPrevio)
                huboCambios = true;
        }

        if (huboCambios)
            repositorio.Guardar(eventos);
    }
}