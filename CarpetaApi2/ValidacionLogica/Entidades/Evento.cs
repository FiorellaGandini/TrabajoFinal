using Newtonsoft.Json;
using ValidacionLogica.Enums;

namespace ValidacionLogica.Entidades;

public class Evento
{
    private int id;
    private EstadoEvento estado;
    private DateTime fechaHora;
    private int horasAperturaAntes;
    private int duracionHoras;

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

    public EstadoEvento Estado
    {
        get => estado;
        private set => estado = value;
    }

    public DateTime FechaHora
    {
        get => fechaHora;
        private set => fechaHora = value;
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

    [JsonConstructor]
    public Evento(int id, EstadoEvento estado, DateTime fechaHora,
        int horasAperturaAntes, int duracionHoras)
    {
        Id = id;
        Estado = estado;
        FechaHora = fechaHora;
        HorasAperturaAntes = horasAperturaAntes;
        DuracionHoras = duracionHoras;
    }

    public DateTime HorarioAperturaIngreso => FechaHora.AddHours(-HorasAperturaAntes);
    public DateTime HorarioCierreIngreso => FechaHora.AddHours(DuracionHoras);

    public bool EstaEnVentanaDeIngreso(DateTime ahora) =>
        Estado == EstadoEvento.Activo && ahora >= HorarioAperturaIngreso && ahora <= HorarioCierreIngreso;
}