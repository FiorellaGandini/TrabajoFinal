using Newtonsoft.Json;
using GestionLogica.Enums;

namespace GestionLogica.Entidades;

public class Evento
{
    private const int HorasAperturaAntesPorDefecto = 1;
    private const int DuracionHorasPorDefecto = 2;
    private int id;
    private string titulo = string.Empty;
    private string descripcion = string.Empty;
    private DateTime fechaHora;
    private Ubicacion ubicacion = null!;
    private CategoriaEvento categoria;
    private EstadoEvento estado;
    private bool prioridad;
    private int horasAperturaAntes;
    private int duracionHoras;
    private readonly List<ModalidadEntrada> modalidades;

    public int Id
    {
        get => id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("El id debe ser un número positivo.");
            id = value;
        }
    }

    public string Titulo
    {
        get => titulo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El título no puede estar vacío.");
            titulo = value;
        }
    }

    public string Descripcion
    {
        get => descripcion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La descripción no puede estar vacía.");
            descripcion = value;
        }
    }

    public DateTime FechaHora
    {
        get => fechaHora;
        private set => fechaHora = value;
    }

    public Ubicacion Ubicacion
    {
        get => ubicacion;
        private set => ubicacion = value ?? throw new ArgumentException("La ubicación es obligatoria.");
    }

    public CategoriaEvento Categoria
    {
        get => categoria;
        private set => categoria = value;
    }

    public EstadoEvento Estado
    {
        get => estado;
        private set => estado = value;
    }

    public bool Prioridad
    {
        get => prioridad;
        private set => prioridad = value;
    }

    public int HorasAperturaAntes
    {
        get => horasAperturaAntes;
        private set
        {
            if (value < 0)
                throw new ArgumentException("Las horas de apertura no pueden ser negativas.");
            horasAperturaAntes = value;
        }
    }

    public int DuracionHoras
    {
        get => duracionHoras;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("La duración debe ser mayor a 0.");
            duracionHoras = value;
        }
    }

    public IReadOnlyList<ModalidadEntrada> Modalidades => modalidades.AsReadOnly();

    public DateTime HorarioAperturaIngreso => FechaHora.AddHours(-HorasAperturaAntes);
    public DateTime HorarioCierreIngreso => FechaHora.AddHours(DuracionHoras);

    // Constructor de negocio: crear un evento nuevo. Siempre arranca
    // Activo y sin modalidades — nadie desde afuera puede forzar otro estado.
    public Evento(int id, string titulo, string descripcion, DateTime fechaHora,
        Ubicacion ubicacion, CategoriaEvento categoria, bool prioridad = false,
        int horasAperturaAntes = HorasAperturaAntesPorDefecto,
        int duracionHoras = DuracionHorasPorDefecto)
    {
        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        Ubicacion = ubicacion;
        Categoria = categoria;
        Prioridad = prioridad;
        HorasAperturaAntes = horasAperturaAntes;
        DuracionHoras = duracionHoras;
        Estado = EstadoEvento.Activo;
        modalidades = new List<ModalidadEntrada>();
    }

    // Constructor de reconstrucción: PRIVADO. Solo Newtonsoft lo invoca al
    // leer eventos.json, trayendo Estado y Modalidades tal como estaban guardados.
    [JsonConstructor]
    private Evento(int id, string titulo, string descripcion, DateTime fechaHora,
        Ubicacion ubicacion, CategoriaEvento categoria, EstadoEvento estado,
        bool prioridad, int horasAperturaAntes, int duracionHoras,
        List<ModalidadEntrada> modalidades)
    {
        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        Ubicacion = ubicacion;
        Categoria = categoria;
        Estado = estado;
        Prioridad = prioridad;
        HorasAperturaAntes = horasAperturaAntes;
        DuracionHoras = duracionHoras;
        this.modalidades = modalidades ?? new List<ModalidadEntrada>();
    }

    public void ActualizarDatos(string titulo, string descripcion, DateTime fechaHora,
        Ubicacion ubicacion, CategoriaEvento categoria, bool prioridad,
        int horasAperturaAntes, int duracionHoras, DateTime ahora)
    {
        if (EstaCancelado())
            throw new InvalidOperationException("No se puede editar un evento cancelado.");
        if (fechaHora < ahora)
            throw new ArgumentException("La nueva fecha no puede estar en el pasado.");

        Titulo = titulo;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        Ubicacion = ubicacion;
        Categoria = categoria;
        Prioridad = prioridad;
        HorasAperturaAntes = horasAperturaAntes;
        DuracionHoras = duracionHoras;
    }

    public void Cancelar()
    {
        if (EstaCancelado())
            throw new InvalidOperationException("El evento ya está cancelado.");
        Estado = EstadoEvento.Cancelado;
    }

    public void Finalizar() => Estado = EstadoEvento.Finalizado;

    public bool EstaCancelado() => Estado == EstadoEvento.Cancelado;

    // Deja que el Service actualice el estado antes de devolver/usar el
    // evento, sin repetir esta condición en cada punto del Service.
    public void ActualizarEstadoSiCorresponde(DateTime ahora)
    {
        if (Estado == EstadoEvento.Activo && ahora > HorarioCierreIngreso)
            Finalizar();
    }

    public bool SePuedeComprarEntrada(DateTime ahora)
    {
        return Estado == EstadoEvento.Activo && ahora < FechaHora;
    }

    public bool EstaEnVentanaDeIngreso(DateTime ahora)
    {
        return Estado == EstadoEvento.Activo && ahora >= HorarioAperturaIngreso && ahora <= HorarioCierreIngreso;
    }

    public ModalidadEntrada? ObtenerModalidad(int modalidadEntradaId)
    {
        return modalidades.FirstOrDefault(m => m.Id == modalidadEntradaId);
    }

    public void AgregarModalidad(ModalidadEntrada modalidad)
    {
        if (EstaCancelado())
            throw new InvalidOperationException("No se pueden agregar modalidades a un evento cancelado.");
        if (modalidad.EventoId != Id)
            throw new ArgumentException("La modalidad no pertenece a este evento.");
        if (modalidades.Any(m => m.Id == modalidad.Id))
            throw new InvalidOperationException("Ya existe una modalidad con ese id en el evento.");

        modalidades.Add(modalidad);
    }
}