using NUnit.Framework;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class EventoTest
{
    private static Ubicacion CrearUbicacion()
    {
        return new Ubicacion("Teatro Colón", "Cerrito 628");
    }

    private static Evento CrearEvento(DateTime fechaHora, int id = 1, int horasAperturaAntes = 1, int duracionHoras = 2)
    {
        return new Evento(id, "Recital", "Descripción del evento", fechaHora, CrearUbicacion(),
            CategoriaEvento.Concierto, prioridad: false, horasAperturaAntes, duracionHoras);
    }

    [Test]
    public void Constructor_TituloVacio_LanzaArgumentException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Evento(1, "", "Descripción", fechaHora, CrearUbicacion(), CategoriaEvento.Concierto));
    }

    [Test]
    public void Constructor_DescripcionVacia_LanzaArgumentException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Evento(1, "Recital", "", fechaHora, CrearUbicacion(), CategoriaEvento.Concierto));
    }

    [Test]
    public void Constructor_HorasAperturaAntesNegativas_LanzaArgumentException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);
        int horasAperturaAntesInvalidas = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CrearEvento(fechaHora, horasAperturaAntes: horasAperturaAntesInvalidas));
    }

    [Test]
    public void Constructor_DuracionHorasCero_LanzaArgumentException()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);
        int duracionInvalida = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CrearEvento(fechaHora, duracionHoras: duracionInvalida));
    }

    [Test]
    public void Constructor_EventoNuevo_ArrancaEnEstadoActivo()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);

        // Act
        var evento = CrearEvento(fechaHora);

        // Assert
        Assert.That(evento.Estado, Is.EqualTo(EstadoEvento.Activo));
    }

    [Test]
    public void Constructor_EventoNuevo_ArrancaSinModalidades()
    {
        // Arrange
        var fechaHora = DateTime.Now.AddDays(1);

        // Act
        var evento = CrearEvento(fechaHora);

        // Assert
        Assert.That(evento.Modalidades, Is.Empty);
    }

    [Test]
    public void HorarioAperturaIngreso_SeCalculaRestandoHorasAperturaAntes()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 3);

        // Act
        var apertura = evento.HorarioAperturaIngreso;

        // Assert
        Assert.That(apertura, Is.EqualTo(fechaHora.AddHours(-1)));
    }

    [Test]
    public void HorarioCierreIngreso_SeCalculaSumandoDuracionHoras()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 3);

        // Act
        var cierre = evento.HorarioCierreIngreso;

        // Assert
        Assert.That(cierre, Is.EqualTo(fechaHora.AddHours(3)));
    }

    [Test]
    public void EstaEnVentanaDeIngreso_MomentoDentroDeLaVentana_DevuelveTrue()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 2);

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(fechaHora);

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_MomentoAntesDeLaApertura_DevuelveFalse()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 2);

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(fechaHora.AddHours(-2));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_MomentoDespuesDelCierre_DevuelveFalse()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 2);

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(fechaHora.AddHours(3));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void EstaEnVentanaDeIngreso_EventoCancelado_DevuelveFalse()
    {
        // Arrange
        var fechaHora = new DateTime(2026, 10, 20, 20, 0, 0);
        var evento = CrearEvento(fechaHora, horasAperturaAntes: 1, duracionHoras: 2);
        evento.Cancelar();

        // Act
        var resultado = evento.EstaEnVentanaDeIngreso(fechaHora);

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void SePuedeComprarEntrada_EventoActivoYFechaFutura_DevuelveTrue()
    {
        // Arrange
        var ahora = DateTime.Now;
        var evento = CrearEvento(ahora.AddDays(1));

        // Act
        var resultado = evento.SePuedeComprarEntrada(ahora);

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void SePuedeComprarEntrada_FechaDelEventoYaPaso_DevuelveFalse()
    {
        // Arrange
        var ahora = DateTime.Now;
        var evento = CrearEvento(ahora.AddDays(1));

        // Act
        var resultado = evento.SePuedeComprarEntrada(ahora.AddDays(2));

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void SePuedeComprarEntrada_EventoCancelado_DevuelveFalse()
    {
        // Arrange
        var ahora = DateTime.Now;
        var evento = CrearEvento(ahora.AddDays(1));
        evento.Cancelar();

        // Act
        var resultado = evento.SePuedeComprarEntrada(ahora);

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void Cancelar_EventoActivo_CambiaEstadoACancelado()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));

        // Act
        evento.Cancelar();

        // Assert
        Assert.That(evento.Estado, Is.EqualTo(EstadoEvento.Cancelado));
    }

    [Test]
    public void Cancelar_EventoYaCancelado_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));
        evento.Cancelar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evento.Cancelar());
    }

    [Test]
    public void ActualizarDatos_EventoCancelado_LanzaInvalidOperationException()
    {
        // Arrange
        var ahora = DateTime.Now;
        var evento = CrearEvento(ahora.AddDays(1));
        evento.Cancelar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            evento.ActualizarDatos("Nuevo título", "Nueva descripción", ahora.AddDays(2),
                CrearUbicacion(), CategoriaEvento.Teatro, false, 1, 2, ahora));
    }

    [Test]
    public void ActualizarDatos_FechaNuevaEnElPasado_LanzaArgumentException()
    {
        // Arrange
        var ahora = DateTime.Now;
        var evento = CrearEvento(ahora.AddDays(1));
        var fechaEnElPasado = ahora.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            evento.ActualizarDatos("Nuevo título", "Nueva descripción", fechaEnElPasado,
                CrearUbicacion(), CategoriaEvento.Teatro, false, 1, 2, ahora));
    }

    [Test]
    public void AgregarModalidad_EventoCancelado_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));
        evento.Cancelar();
        var modalidad = new ModalidadEntrada(1, evento.Id, "General", 1000, 10);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evento.AgregarModalidad(modalidad));
    }

    [Test]
    public void AgregarModalidad_ModalidadDeOtroEvento_LanzaArgumentException()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));
        var modalidadDeOtroEvento = new ModalidadEntrada(1, evento.Id + 1, "General", 1000, 10);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evento.AgregarModalidad(modalidadDeOtroEvento));
    }

    [Test]
    public void AgregarModalidad_IdDeModalidadDuplicado_LanzaInvalidOperationException()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));
        evento.AgregarModalidad(new ModalidadEntrada(1, evento.Id, "General", 1000, 10));
        var modalidadConIdRepetido = new ModalidadEntrada(1, evento.Id, "VIP", 2000, 5);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evento.AgregarModalidad(modalidadConIdRepetido));
    }

    [Test]
    public void AgregarModalidad_ModalidadValida_QuedaEnLaListaDeModalidades()
    {
        // Arrange
        var evento = CrearEvento(DateTime.Now.AddDays(1));
        var modalidad = new ModalidadEntrada(1, evento.Id, "General", 1000, 10);

        // Act
        evento.AgregarModalidad(modalidad);

        // Assert
        Assert.That(evento.Modalidades, Has.Count.EqualTo(1));
    }
}