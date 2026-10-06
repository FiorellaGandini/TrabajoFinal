using NUnit.Framework;
using GestionLogica.Entidades;
using GestionLogica.Enums;

namespace GestionLogicaTest;

[TestFixture]
public class EventoServiceTest : ArchivosTestBase
{
    [Test]
    public void Crear_SolicitanteOrganizador_CreaEventoEnEstadoActivo()
    {
        // Arrange & Act
        var evento = Eventos.Crear(DniOrganizador, "Recital", "Gran show", FechaEvento,
            new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2);

        // Assert
        Assert.That(evento.Estado, Is.EqualTo(EstadoEvento.Activo));
    }

    [Test]
    public void Crear_SolicitanteComprador_LanzaUnauthorizedAccessException()
    {
        // Arrange & Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() =>
            Eventos.Crear(DniComprador, "Recital", "Gran show", FechaEvento,
                new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2));
    }

    [Test]
    public void Crear_SolicitanteInexistente_LanzaArgumentException()
    {
        // Arrange
        int dniInexistente = 99999999;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Eventos.Crear(dniInexistente, "Recital", "Gran show", FechaEvento,
                new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2));
    }

    [Test]
    public void ObtenerPorId_IdInexistente_DevuelveNull()
    {
        // Arrange & Act
        var evento = Eventos.ObtenerPorId(999, Ahora);

        // Assert
        Assert.That(evento, Is.Null);
    }

    [Test]
    public void ObtenerPorId_MomentoPosteriorAlCierre_DejaElEventoFinalizado()
    {
        // Arrange
        var evento = CrearEventoConModalidad();
        var momentoPosteriorAlCierre = evento.HorarioCierreIngreso.AddHours(1);

        // Act
        var resultado = Eventos.ObtenerPorId(evento.Id, momentoPosteriorAlCierre);

        // Assert
        Assert.That(resultado!.Estado, Is.EqualTo(EstadoEvento.Finalizado));
    }

    [Test]
    public void Actualizar_EventoInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Eventos.Actualizar(DniOrganizador, 999, "Nuevo", "Desc", FechaEvento,
                new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2, Ahora));
    }

    [Test]
    public void Actualizar_SolicitanteNoOrganizador_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var evento = CrearEventoConModalidad();

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() =>
            Eventos.Actualizar(DniComprador, evento.Id, "Nuevo", "Desc", FechaEvento,
                new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2, Ahora));
    }

    [Test]
    public void Actualizar_CasoValido_ActualizaElTitulo()
    {
        // Arrange
        var evento = CrearEventoConModalidad();

        // Act
        var actualizado = Eventos.Actualizar(DniOrganizador, evento.Id, "Título nuevo", "Desc nueva",
            FechaEvento, new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2, Ahora);

        // Assert
        Assert.That(actualizado.Titulo, Is.EqualTo("Título nuevo"));
    }

    [Test]
    public void Cancelar_EventoActivo_CambiaEstadoACancelado()
    {
        // Arrange
        var evento = CrearEventoConModalidad();

        // Act
        var cancelado = Eventos.Cancelar(DniOrganizador, evento.Id);

        // Assert
        Assert.That(cancelado.Estado, Is.EqualTo(EstadoEvento.Cancelado));
    }

    [Test]
    public void Cancelar_EventoInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Eventos.Cancelar(DniOrganizador, 999));
    }

    [Test]
    public void AgregarModalidad_CasoValido_QuedaAsociadaAlEvento()
    {
        // Arrange
        var evento = Eventos.Crear(DniOrganizador, "Recital", "Gran show", FechaEvento,
            new Ubicacion("Estadio", "Calle 1"), CategoriaEvento.Concierto, false, 1, 2);

        // Act
        Eventos.AgregarModalidad(DniOrganizador, evento.Id, "General", 100m, 10);

        // Assert
        var eventoActualizado = Eventos.ObtenerPorId(evento.Id, Ahora);
        Assert.That(eventoActualizado!.Modalidades, Has.Count.EqualTo(1));
    }

    [Test]
    public void AgregarModalidad_EventoInexistente_LanzaArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Eventos.AgregarModalidad(DniOrganizador, 999, "General", 100m, 10));
    }
}