using NUnit.Framework;
using ValidacionLogica.Entidades;
using ValidacionLogica.Enums;

namespace ValidacionLogicaTest;

[TestFixture]
public class EventoTest
{
    //Evento: 01/11/2026 20:00, abre 1 h antes (19:00), dura 2 h (cierra 22:00)
    private static readonly DateTime FechaEvento = new(2026, 11, 1, 20, 0, 0);

    private static Evento NuevoEvento(int id = 1, EstadoEvento estado = EstadoEvento.Activo,
        int horasAperturaAntes = 1, int duracionHoras = 2) =>
        new Evento(id, estado, FechaEvento, horasAperturaAntes, duracionHoras);

    //Constructor: datos válidos 

    [Test]
    public void Constructor_IdValido_GuardaElId()
    {
        // Arrange + Act
        var evento = NuevoEvento(id: 5);

        // Assert
        Assert.That(evento.Id, Is.EqualTo(5));
    }

    [Test]
    public void Constructor_EstadoCancelado_GuardaElEstado()
    {
        // Arrange + Act
        var evento = NuevoEvento(estado: EstadoEvento.Cancelado);

        // Assert
        Assert.That(evento.Estado, Is.EqualTo(EstadoEvento.Cancelado));
    }

    [Test]
    public void Constructor_FechaValida_GuardaLaFecha()
    {
        // Arrange + Act
        var evento = NuevoEvento();

        // Assert
        Assert.That(evento.FechaHora, Is.EqualTo(FechaEvento));
    }

    [Test]
    public void Constructor_HorasAperturaCero_LaAperturaCoincideConLaFechaDelEvento()
    {
        // Arrange + Act
        var evento = NuevoEvento(horasAperturaAntes: 0);

        // Assert
        Assert.That(evento.HorarioAperturaIngreso, Is.EqualTo(FechaEvento));
    }

    //Constructor: datos invalidos

    [Test]
    public void Constructor_IdCero_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevoEvento(id: 0));
    }

    [Test]
    public void Constructor_IdNegativo_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevoEvento(id: -1));
    }

    [Test]
    public void Constructor_HorasAperturaNegativas_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevoEvento(horasAperturaAntes: -1));
    }

    [Test]
    public void Constructor_DuracionCero_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevoEvento(duracionHoras: 0));
    }

    [Test]
    public void Constructor_DuracionNegativa_LanzaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => NuevoEvento(duracionHoras: -2));
    }

    [Test]
    public void Constructor_DuracionCero_MensajeIndicaQueDebeSerMayorACero()
    {
        // Arrange + Act
        var excepcion = Assert.Throws<ArgumentException>(() => NuevoEvento(duracionHoras: 0));

        // Assert
        Assert.That(excepcion.Message, Is.EqualTo("La duración debe ser mayor a 0."));
    }

    //Horarios de ingreso

    [Test]
    public void HorarioAperturaIngreso_UnaHoraAntes_EsUnaHoraAntesDelEvento()
    {
        // Arrange
        var evento = NuevoEvento(horasAperturaAntes: 1);

        // Act
        var apertura = evento.HorarioAperturaIngreso;

        // Assert
        Assert.That(apertura, Is.EqualTo(new DateTime(2026, 11, 1, 19, 0, 0)));
    }

    [Test]
    public void HorarioCierreIngreso_DuracionDosHoras_EsDosHorasDespuesDelEvento()
    {
        // Arrange
        var evento = NuevoEvento(duracionHoras: 2);

        // Act
        var cierre = evento.HorarioCierreIngreso;

        // Assert
        Assert.That(cierre, Is.EqualTo(new DateTime(2026, 11, 1, 22, 0, 0)));
    }

    //EstaEnVentanaDeIngreso

    [Test]
    public void EstaEnVentanaDeIngreso_EventoActivoDentroDeLaVentana_RetornaTrue()
    {
        // Arrange
        var evento = NuevoEvento();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 20, 30, 0));

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_JustoAlAbrir_RetornaTrue()
    {
        // Arrange
        var evento = NuevoEvento();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 19, 0, 0));

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_JustoAlCerrar_RetornaTrue()
    {
        // Arrange
        var evento = NuevoEvento();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 22, 0, 0));

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_UnMinutoAntesDeAbrir_RetornaFalse()
    {
        // Arrange
        var evento = NuevoEvento();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 18, 59, 0));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_UnMinutoDespuesDeCerrar_RetornaFalse()
    {
        // Arrange
        var evento = NuevoEvento();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 22, 1, 0));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_EventoCanceladoEnHorario_RetornaFalse()
    {
        // Arrange
        var evento = NuevoEvento(estado: EstadoEvento.Cancelado);

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 20, 30, 0));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_EventoFinalizadoEnHorario_RetornaFalse()
    {
        // Arrange
        var evento = NuevoEvento(estado: EstadoEvento.Finalizado);

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(new DateTime(2026, 11, 1, 20, 30, 0));

        // Assert
        Assert.That(resultado, Is.False);
    }
}